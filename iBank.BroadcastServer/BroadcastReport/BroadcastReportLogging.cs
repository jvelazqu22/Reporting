using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Logging;

namespace iBank.BroadcastServer.BroadcastReport
{
    public class BroadcastReportLogging
    {
        public void UpdateReportLogResultsRecord(ReportRunResults results, int reportLogKey, IMasterDataStore masterDataStore)
        {
            var resultsLogger = new ReportLogResultsLogger();
            var getLoggerQuery = new GetReportLogResultsByLogKeyQuery(masterDataStore.MastersQueryDb, reportLogKey);
            var resultLogRecord = getLoggerQuery.ExecuteQuery();

            if (resultLogRecord == null)
            {
                resultsLogger.Create(reportLogKey, results.ReportHref, results.ErrorMessage, 0, masterDataStore.MastersCommandDb);
            }
            else
            {
                resultsLogger.Update(resultLogRecord, results.ReportHref, results.ErrorMessage, 0, masterDataStore.MastersCommandDb);
            }
        }

        public void UpdateReportLog(ReportRunResults results, ReportLogLogger rptLogger, int reportLogKey)
        {
            if (results.ReportHasNoData)
            {
                rptLogger.UpdateLog(reportLogKey, ReportLogLogger.ReportStatus.NODATA);
            }
            else if (results.ReportRunSuccess)
            {
                rptLogger.UpdateLog(reportLogKey, ReportLogLogger.ReportStatus.SUCCESS);
            }
            else
            {
                rptLogger.UpdateLog(reportLogKey, ReportLogLogger.ReportStatus.SYSERROR);
            }
        }
    }
}
