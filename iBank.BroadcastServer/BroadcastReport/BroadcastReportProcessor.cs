using CODE.Framework.Core.Utilities;
using com.ciswired.libraries.CISLogger;
using Domain.Exceptions;
using Domain.Helper;
using Domain.Interfaces.BroadcastServer;
using Domain.Models.BroadcastServer;
using iBank.BroadcastServer.Email;
using iBank.BroadcastServer.Helper;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Domain.Services;
using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using iBankDomain.Exceptions;

namespace iBank.BroadcastServer.BroadcastReport
{
    public class BroadcastReportProcessor : IBroadcastReportProcessor
    {
        private static readonly com.ciswired.libraries.CISLogger.ILogger LOG =
            new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private IMasterDataStore MasterDataStore { get; set; }
        private IClientDataStore ClientDataStore { get; set; }
        
        private IList<ProcessCaptionInformation> ProcessCaptions { get; set; }
        private BroadcastEmail BcstEmail { get; set; }
        private Object _object = new Object();
        private LoadedListsParams _loadedParams;

        public BroadcastReportProcessor(IMasterDataStore masterDataStore, IClientDataStore clientDataStore,  
            IList<ProcessCaptionInformation> processCaptions, BroadcastEmail bcstEmail, LoadedListsParams loadedParams)
        {
            MasterDataStore = masterDataStore;
            ClientDataStore = clientDataStore;
            ProcessCaptions = processCaptions;
            BcstEmail = bcstEmail;
            _loadedParams = loadedParams;
        }

        public bool RunAllReports(ICacheService cache, IList<BroadcastReportInformation> allReportsInBatch, IRecordTimingDetails broadcastRecTiming, BroadcastServerInformation serverConfiguration, 
            bool isOfflineReport, ibuser user, bool runSpecial, bcstque4 broadcast)
        {
            var reportCount = allReportsInBatch.Count;
            BcstEmail.HeaderBuilder.BuildHeader(broadcast.batchname, allReportsInBatch.Count);
            
            var allBatchesOk = true;
            var reportWithDataExists = false;
            var maxThreads = ThreadLimits.GetMaxConcurrentReportsForServer(cache, serverConfiguration.ServerNumber, new iBankAdministrationQueryable());
#if DEBUG
            maxThreads = 1;
#endif

            var options = new ParallelOptions { MaxDegreeOfParallelism = maxThreads };
            var bodySections = new List<IEmailSection>();
            Parallel.ForEach(allReportsInBatch, options, report => RunReport(report, bodySections, serverConfiguration, isOfflineReport,
                broadcastRecTiming, broadcast, user, ref reportWithDataExists, ref allBatchesOk, _loadedParams)
            );

            //now build all the body sections into one body
            BcstEmail.BodyBuilder.BuildBody(bodySections, broadcast);
            
            BcstEmail.FooterBuilder.BuildFooter(reportCount, broadcast);

            var allReportsEmpty = !reportWithDataExists;

            var sendSuccess = AssembleEmail(broadcast, allReportsEmpty, runSpecial, allBatchesOk, broadcastRecTiming, serverConfiguration, user);
            if (!sendSuccess) return false;

            return allBatchesOk;
        }

        private bool AssembleEmail(bcstque4 broadcast, bool allReportsEmpty, bool runSpecial, bool allBatchesOk, IRecordTimingDetails broadcastRecTiming,
            BroadcastServerInformation serverConfiguration, ibuser user)
        {
            //put the email together and send it out
            try
            {
                BcstEmail.AssembleEmail(broadcast, allReportsEmpty, runSpecial, allBatchesOk, broadcastRecTiming);
            }
            catch (FormatException formatEx)
            {
                var errorNumber = ErrorLogger.LogException(user.UserNumber, broadcast.agency, formatEx, MethodBase.GetCurrentMethod(),
                    ServerType.BroadcastServer, LOG);

                if (broadcast.send_error_email)
                {
                    new ErrorHandler().SendErrorEmail(LOG, broadcast, errorNumber, MasterDataStore, serverConfiguration);
                }
            }
            catch (ArgumentException argEx)
            {
                var errorNumber = ErrorLogger.LogException(user.UserNumber, broadcast.agency, argEx, MethodBase.GetCurrentMethod(),
                    ServerType.BroadcastServer, LOG);

                if (broadcast.send_error_email)
                {
                    new ErrorHandler().SendErrorEmail(LOG, broadcast, errorNumber, MasterDataStore, serverConfiguration);
                }
            }
            catch (SendEmailException seEx)
            {
                var errorNumber = ErrorLogger.LogException(user.UserNumber, broadcast.agency, seEx, MethodBase.GetCurrentMethod(),
                ServerType.BroadcastServer, LOG);

                if (broadcast.send_error_email)
                {
                    new ErrorHandler().SendErrorEmail(LOG, broadcast, errorNumber, MasterDataStore, serverConfiguration);
                }

                return false;
            }

            return true;
        }

        private void RunReport(BroadcastReportInformation report, List<IEmailSection> bodySections, BroadcastServerInformation serverConfiguration,
            bool isOfflineReport, IRecordTimingDetails broadcastRecTiming, bcstque4 broadcast, ibuser user, ref bool reportWithDataExists, 
            ref bool allBatchesOk, LoadedListsParams loadedParams)
        {
            // I could be wrong, but I don't think broadcast reports have report ids, so I am making one up for tracking purposes.
            var fakeReportId = Guid.NewGuid();
            var reportHistoryHandler = new ReportHistoryHandler();
            reportHistoryHandler.InsertNewBroadcastReportIntoReportHistory(fakeReportId, DateTime.Now, MasterDataStore, broadcast);

            SetupRunningBroadcastReport(report, serverConfiguration, isOfflineReport, broadcastRecTiming, broadcast);
            var reportRunner = new BroadcastReportRunner(MasterDataStore, ClientDataStore);

            var reportRunResults = new ReportRunResults();

            try
            {
                reportRunResults = reportRunner.RunReport(broadcast, report, serverConfiguration, user, loadedParams);

                //if an effects delivery
                if (broadcast.outputdest.Equals(BroadcastCriteria.EffectsOutputDest) && broadcast.eProfileNo > 0)
                {
                    reportRunResults.eFFECTSDeliveryReturnMessage = reportRunner.Globals.ReportInformation.ReturnText;
                }
            }
            catch (RecoverableSqlException)
            {
                reportHistoryHandler.UpdateReportHistoryFinishedRun(fakeReportId.ToString(), MasterDataStore);
                //since we believe we can recover from this exception let's throw it up a level so the whole broadcast will get retried
                throw;
            }
            catch (ReportAbandonedException)
            {
                reportHistoryHandler.UpdateReportHistoryFinishedRun(fakeReportId.ToString(), MasterDataStore);
                throw;
            }
            catch (Exception e)
            {
                //reportHistoryHandler.UpdateReportHistoryFinishedRun(fakeReportId.ToString(), MasterDataStore);
                var errorNumber = ErrorLogger.LogException(user.UserNumber, reportRunner.Globals.Agency, e, MethodBase.GetCurrentMethod(), ServerType.BroadcastServer, LOG);

                if (broadcast.send_error_email) new ErrorHandler().SendErrorEmail(LOG, broadcast, errorNumber, MasterDataStore, serverConfiguration);
            }

            LogReportResult(reportRunResults, reportRunner.Globals.ReportLogKey, report.ProcessKey, report.BatchNumber,
                reportRunner.Globals.Agency);

            if (!reportRunResults.ReportHasNoData) reportWithDataExists = true;

            //a report with no data will return success = false, but that doesn't mean an error occurred
            if (!reportRunResults.ReportRunSuccess && !reportRunResults.ReportHasNoData) allBatchesOk = false;
            lock (_object)
            {
                BcstEmail.BodyBuilder = new EmailBodyBuilder(BcstEmail);
                BcstEmail.BodyBuilder.CreateBodySection(broadcast, report, reportRunResults, reportRunner.Globals);
                bodySections.Add(BcstEmail.BodyBuilder.Body);
            }

            reportHistoryHandler.UpdateReportHistoryFinishedRunAndReplaceFakeReportIdWithActualReportName(fakeReportId.ToString(), MasterDataStore, reportRunResults.FinalRecordsCount, reportRunResults.ReportId);
        }

        private void LogReportResult(ReportRunResults reportRunResults, int reportLogKey, int processKey, int batchNumber, string agency)
        {
            var msg = string.Format("Report key [{0}], batch [{1}] for agency [{2}] [{3}] process successfully.", processKey, batchNumber, agency,
                         (!reportRunResults.ReportRunSuccess && !reportRunResults.ReportHasNoData) ? "did not" : "did");

            if (!reportRunResults.ReportRunSuccess && !reportRunResults.ReportHasNoData) LOG.Warn(msg.FormatMessageWithReportLogKey(reportLogKey));
            else LOG.Info(msg.FormatMessageWithReportLogKey(reportLogKey));
        }

        private void SetupRunningBroadcastReport(BroadcastReportInformation report, BroadcastServerInformation serverConfiguration, bool isOfflineReport,
            IRecordTimingDetails broadcastRecTiming, bcstque4 broadcast)
        {
            var reportDir = StringHelper.AddBS(serverConfiguration.ReportOutputDirectory) + broadcast.agency;
            if (!Directory.Exists(reportDir))
            {
                LOG.Info(string.Format("Creating directory {0}", reportDir));
                Directory.CreateDirectory(reportDir);
            }

            if (broadcastRecTiming.RunNewData)
            {
                //tells ibxmlexport to use "select data last changed last x hrs/mins" checkbox from criteria screen
                report.DateType = 91;
            }

            report.AccountList = broadcast.acctlist;
            report.PrevHist = broadcast.prevhist ?? 0;
            
            if (report.SavedReportNumber > 0 && isOfflineReport)
            {
                var caption = ProcessCaptions.FirstOrDefault(s => s.ProcessKey == report.ProcessKey);
                if (caption != null)
                {
                    report.UserReportName = caption.Caption.Trim();
                }
            }

            report.IsOfflineReport = isOfflineReport;

            var reportPeriodStartDate = broadcastRecTiming.NextReportPeriodStart.ToDateTimeSafe();
            var reportPeriodEndDate = broadcastRecTiming.NextReportPeriodEnd.ToDateTimeSafe();
            if (broadcast.usespcl.HasValue && broadcast.usespcl.Value)
            {
                reportPeriodStartDate = broadcast.spclstart.ToDateTimeSafe();
                reportPeriodEndDate = broadcast.spclend.ToDateTimeSafe();
            }

            report.ReportStart = reportPeriodStartDate;
            report.ReportEnd = reportPeriodEndDate;
        }
        
    }
}
