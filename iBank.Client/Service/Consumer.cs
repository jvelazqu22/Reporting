using CODE.Framework.Core.Utilities.Extensions;
using com.ciswired.libraries.CISLogger;
using Domain.Models.OnlineReportServer;
using Domain.Orm.Classes;
using iBank.Server.Utilities.Logging;
using iBank.Services.Implementation.ReportPrograms;
using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Orm.iBankMastersCommands.PendingRecords;
using iBank.Entities.MasterEntities;
using iBank.ReportServer.Helpers;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;

namespace iBank.ReportServer.Service
{
    //
    public class Consumer
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly IReportServerLogger RPT_LOG = new ReportServerLogger(new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType));

        public void ProcessReports(Parameters p, LoadedListsParams loadedParams)
        {
            var maxThreads = ThreadLimits.GetMaxConcurrentReportsForServer(p.Cache, p.ServerNumber, new iBankAdministrationQueryable());
#if DEBUG
            maxThreads = 1;
#endif
            var options = new ParallelOptions { MaxDegreeOfParallelism = maxThreads };
            Parallel.ForEach(p.ReportsToExecute.GetConsumingPartitioner(), options, report => { RunReport(report, p, loadedParams); });
        }

        //virtual for unit testing
        public virtual int GetMsToAddForOffline() => ConfigurationManager.AppSettings["PushOfflineIntervalInMs"].TryIntParse(60000);
        private void RunReport(PendingReportInformation report, Parameters p, LoadedListsParams loadedParams)
        {
            if (p.IsMaintenanceModeRequested) return;

            try
            {
                if (!IsQueueNotified(report, p.MasterDataStore)) return;

                if (report.DateCreated.AddMilliseconds(GetMsToAddForOffline()) < GetNow())
                {
                    //if the report has gone stale on the queue just convert it to a broadcast
                    var delayer = new ReportDelayer(p.MasterDataStore);
                    delayer.ConvertPastDueReportToBroadcast(p.Cache, report, p.ServerNumber);

                    report.ReturnCode = 2;
                    report.ErrorMessage = "This report has been sent to run offline due to the time it is taking to run online.";
                    // This method is called in order to add a message that can get displayed in the UI. Also, we need to update the
                    // report status to done in the reporthandoff table, and we need to remove the report from the pending_reports table.
                    ReportHelper.PostError(report);

                    return;
                }

                var rs = new ReportSwitch(p.MasterDataStore, p.ServerNumber)
                {
                    IsOfflineServer = false,
                    DevMode = p.IsDevMode,
                    DoneEvent = new ManualResetEvent(false)
                };

                var reportHistoryHandler = new ReportHistoryHandler();

                RPT_LOG.InfoLogWithReportInfo(report, "Started running report.");

                reportHistoryHandler.UpdateReportHistoryStartRun(report.ReportId, p.MasterDataStore);

                rs.RunReport(report, loadedParams);

                reportHistoryHandler.UpdateReportHistoryFinishedRun(report.ReportId, p.MasterDataStore, rs.FinalRecordsCount);

                RPT_LOG.InfoLogWithReportInfo(report, "Finished running report");
            }
            catch (Exception e)
            {
                LOG.Fatal(e);

                ErrorLogger.LogException(report.UserNumber, report.Agency, e, MethodBase.GetCurrentMethod(), ServerType.OnlineReportServer, LOG);
                new ReportHistoryHandler().UpdateReportHistoryFinishedRun(report.ReportId, p.MasterDataStore);
            }

        }

        private bool IsQueueNotified(PendingReportInformation report, IMasterDataStore store)
        {
            try
            {
                NotifyQueueReportIsRunning(report, store);
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                //another server beat us to this report so log and swallow
                LOG.Warn($"Another server picked up reportid [{report.ReportId}] first");
                return false;
            }
        }

        public virtual DateTime GetNow() => DateTime.Now;
        private void NotifyQueueReportIsRunning(PendingReportInformation pendingReport, IMasterDataStore masterStore)
        {
            var queueRec = new PendingReports
            {
                Agency = pendingReport.Agency,
                IsDotNet = pendingReport.IsDotNet,
                IsRunning = true,
                ReportId = pendingReport.ReportId,
                RowVersion = pendingReport.RowVersion,
                TimeStamp = GetNow(),
                UserNumber = pendingReport.UserNumber
            };
            var cmd = new UpdatePendingReportCommand(masterStore.MastersCommandDb, queueRec);
            cmd.ExecuteCommand();
        }
    }
}
