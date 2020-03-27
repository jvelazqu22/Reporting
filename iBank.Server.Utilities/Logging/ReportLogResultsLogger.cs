using System;
using System.Data.Entity.Validation;
using System.Reflection;

using com.ciswired.libraries.CISLogger;

using Domain.Orm.iBankMastersCommands;

using iBank.Entities.MasterEntities;

using iBankDomain.RepositoryInterfaces;

namespace iBank.Server.Utilities.Logging
{
    public class ReportLogResultsLogger
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void Create(int reportLogNumber, string reportUrl, string errorMessage, int returnCode, ICommandDb mastersCommandDb)
        {
            var record = new ibRptLogResults
            {
                rptlogno = reportLogNumber,
                reporturl = reportUrl,
                errormsg = errorMessage,
                returncode = returnCode
            };

            try
            {
                var addRecordCommand = new AddRptLogResultsRecordCommand(mastersCommandDb, record);
                addRecordCommand.ExecuteCommand();
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
                ErrorLogger.LogException(null, null, e, MethodBase.GetCurrentMethod(), ServerType.Unknown, LOG);
                Console.WriteLine(e);
            }
        }

        public void Update(ibRptLogResults record, string reportUrl, string errorMessage, int returnCode, ICommandDb mastersCommandDb)
        {
            record.reporturl = reportUrl;
            record.errormsg = errorMessage;
            record.returncode = returnCode;

            try
            {
                var updateCommand = new UpdateRptLogResultsRecordCommand(mastersCommandDb, record);
                updateCommand.ExecuteCommand();
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
                ErrorLogger.LogException(null, null, e, MethodBase.GetCurrentMethod(), ServerType.Unknown, LOG);
                Console.WriteLine(e);
            }
        }
    }
}
