using System;
using System.Data.Entity.Validation;
using System.Reflection;

using com.ciswired.libraries.CISLogger;

using Domain.Orm.iBankMastersCommands;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;

using iBankDomain.RepositoryInterfaces;

namespace iBank.Server.Utilities.Logging
{
    public class ReportLogLogger
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public enum ReportStatus
        {
            SUCCESS,
            SYSERROR,
            PENDING,
            SENTOFFL,
            USRERROR,
            NODATA
        }

        public enum ReportMode
        {
            OFFLINE,
            REGULAR,
            BROADCAST,
            TRAVET
        }

        private IMastersQueryable MasterQueryDb
        {
            get
            {
                return new iBankMastersQueryable();
            }
        }

        private ICommandDb MasterCommandDb
        {
            get
            {
                return new iBankMastersCommandDb();
            }
        }

        public void UpdateLog(ibRptLog rptLog, DateTime startTime, ReportMode mode, byte serverNumber)
        {
            rptLog.starttime = startTime;
            rptLog.rptmode = mode.ToString();
            rptLog.svrnbr = serverNumber;

            try
            {
                var updateReportLogCmd = new UpdateRptLogRecordCommand(MasterCommandDb, rptLog);
                updateReportLogCmd.ExecuteCommand();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        var msg = string.Format("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                        LOG.Error(msg, dbEx);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLogger.LogException(rptLog.usernumber, rptLog.agency, e, MethodBase.GetCurrentMethod(), ServerType.Unknown, LOG);
                Console.WriteLine(e);
            }
        }

        public void UpdateLog(int reportLogKey, ReportStatus status, int recordCount)
        {
            var rptLog = new GetReportLogByLogKeyQuery(MasterQueryDb, reportLogKey).ExecuteQuery();
            if (rptLog != null)
            {
                var secs = DateTime.Now - rptLog.starttime;
                rptLog.numseconds = secs.Seconds;
                rptLog.numrecords = recordCount;
                rptLog.rptstatus = status.ToString();

                try
                {
                    var updateRptLog = new UpdateRptLogRecordCommand(MasterCommandDb, rptLog);
                    updateRptLog.ExecuteCommand();
                }

                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            var msg = string.Format("Property: {0} Error: {1}",
                                                    validationError.PropertyName,
                                                    validationError.ErrorMessage);
                            LOG.Error(msg, dbEx);
                        }
                    }
                }
                catch (Exception e)
                {
                    ErrorLogger.LogException(rptLog.usernumber, rptLog.agency, e, MethodBase.GetCurrentMethod(), ServerType.Unknown, LOG);
                    Console.WriteLine(e);
                }
            }
        }

        public void UpdateLog(int reportLogKey, ReportStatus status)
        {
            var rptLog = new GetReportLogByLogKeyQuery(MasterQueryDb, reportLogKey).ExecuteQuery();
            if (rptLog != null)
            {
                rptLog.rptstatus = status.ToString();

                try
                {
                    var updateRptLog = new UpdateRptLogRecordCommand(MasterCommandDb, rptLog);
                    updateRptLog.ExecuteCommand();
                }

                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            var msg = string.Format("Property: {0} Error: {1}",
                                                    validationError.PropertyName,
                                                    validationError.ErrorMessage);
                            LOG.Error(msg, dbEx);
                        }
                    }
                }
                catch (Exception e)
                {
                    ErrorLogger.LogException(rptLog.usernumber, rptLog.agency, e, MethodBase.GetCurrentMethod(), ServerType.Unknown, LOG);
                    Console.WriteLine(e);
                }
            }
        }

        public ibRptLog CreateLog(string agency, string userId, string userName, ReportMode mode, int userNumber, string iBankVersion,
            int processKey, byte serverNumber, ReportStatus status)
        {
            var currentDate = DateTime.Now;
            var rptlog = new ibRptLog
            {
                rptdate = currentDate,
                submittime = currentDate,
                starttime = currentDate,
                agency = agency,
                userid = userId,
                username = userName, 
                rptprogram = null,
                numrecords = null,
                numseconds = null,
                rptmode = mode.ToString(),
                usernumber = userNumber,
                iBankVer = iBankVersion,
                processkey = processKey,
                svrnbr = serverNumber,
                rptstatus = status.ToString()
            };

            try
            {
                var addLogCommand = new AddRptLogRecordCommand(MasterCommandDb, rptlog);
                addLogCommand.ExecuteCommand();
            }

            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        var msg = string.Format("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                        LOG.Error(msg, dbEx);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLogger.LogException(userNumber, agency, e, MethodBase.GetCurrentMethod(), ServerType.Unknown, LOG);
                Console.WriteLine(e);
            }

            return rptlog;
        }
    }
}
