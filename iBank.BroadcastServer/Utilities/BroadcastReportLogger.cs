using com.ciswired.libraries.CISLogger;
using Domain.Interfaces.BroadcastServer;
using Domain.Orm.iBankClientCommands;
using iBank.Server.Utilities.Logging;

using System;
using System.Data.Entity.Validation;
using System.Reflection;

using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;

using iBankDomain.RepositoryInterfaces;

namespace iBank.BroadcastServer.Utilities
{
    public class BroadcastReportLogger
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void AddEmailLogRecord(bcstque4 batch, bool batchOk, IRecordTimingDetails bcstTiming, string emailAddress, string mailLog, 
            bool runSpecial, string agency, string emailText, int userNumber, ICommandDb clientCommandDb)
        {
            var bcControlFields = CalculateBcControlFields(runSpecial, batch);
            var logRecord = CreateReportLogRecord(batchOk, agency, userNumber, batch, bcstTiming, emailAddress, emailText, bcControlFields, mailLog);

            try
            {
                var addBcstReportLogCmd = new AddBcReportLogRecordCommand(clientCommandDb, logRecord);
                addBcstReportLogCmd.ExecuteCommand();
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

        }

        private static string CalculateBcControlFields(bool runSpecial, bcstque4 batch)
        {
            return runSpecial
                ? Environment.NewLine + "RUN SPCL"
                : Environment.NewLine + "PH" + batch.prevhist + ",WM" + batch.weekmonth + ",MS" + batch.monthstart + ",MR" + batch.monthrun + ",WS" +
                                batch.weekstart + ",WR" + batch.weekrun + ",SETBY:" + batch.setby;
        }

        private static bcreportlog CreateReportLogRecord(bool batchOk, string agency, int userNumber, bcstque4 batch, IRecordTimingDetails bcstTiming, 
            string emailAddress, string emailText, string bcControlFields, string mailLog)
        {
            return new bcreportlog
            {
                rundatetime = DateTime.Now,
                runokay = batchOk,
                agency = agency,
                UserNumber = userNumber,
                batchnum = batch.batchnum,
                startdate = bcstTiming.NextReportPeriodStart,
                enddate = bcstTiming.NextReportPeriodEnd,
                emailaddr = emailAddress,
                emailmsg = emailText + "," + bcControlFields,
                emaillog = mailLog,
                emailpda = string.Empty
            };
        }


    }
}
