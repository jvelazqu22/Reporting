
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using com.ciswired.libraries.CISLogger;
using Domain.Models.BroadcastServer;
using Domain.Orm.Classes;
using Domain.Orm.iBankAdministrationQueries;
using Domain.Orm.iBankMastersQueries.BroadcastQueries;

using iBank.BroadcastServer.BroadcastBatch;
using iBank.Entities.MasterEntities;
using iBank.ReportQueueManager.Helpers;
using iBank.ReportQueueManager.Service;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Cleaners;
using iBank.Server.Utilities.Logging;
using iBank.Services.Implementation.ReportPrograms;
using iBank.Services.Implementation.Shared;

namespace iBank.ReportTester
{
    public partial class Form1 : Form
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Form1()
        {
            InitializeComponent();
        }

        private async void btnSubmitReport_Click(object sender, EventArgs e)
        {
            LOG.Info($"Start Test {GetReportId()}");
            try
            {
                lblProcessing.Visible = true;

                await Task.WhenAll(ProcessReportAsync());
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error encountered.");
            }
            finally
            {
                lblProcessing.Visible = false;
            }

            LOG.Info($"End Test {GetReportId()}");
        }

        private async Task ProcessReportAsync()
        {
            await Task.Run(() =>
                {
                    var start = DateTime.Now;
                    var reportId = GetReportId();

                    var report = GetReport(reportId);

                    RunReport(report);

                    var end = DateTime.Now;

                    LOG.Info($"End Testing");
                    var msg = $"Report finished processing in {(end - start).TotalSeconds} (sec)";
                    MessageBox.Show(msg, "Report Finished");
                });
        }

        private string GetReportId()
        {
            var reportId = txtReportId.Text;

            if (string.IsNullOrEmpty(reportId)) throw new Exception("Must supply a report id.");

            return reportId;
        }

        private PendingReportInformation GetReport(string reportId)
        {
            using (var db = new iBankMastersQueryable())
            {
                var reportRec = db.ReportHandoff.FirstOrDefault(x => x.reportid.Equals(reportId));
                LOG.Info($"In {MethodBase.GetCurrentMethod().Name} | reportRec.agency {reportRec.agency} | reportRec.cfbox {reportRec.cfbox} | reportRec.usernumber {reportRec.usernumber} | reportRec.svrnumber: {reportRec.svrnumber} ");

                if (reportRec == null)
                {
                    LOG.Info($"No report found for report id {reportId}");
                    throw new Exception($"No report found for report id {reportId}");
                }
                return new PendingReportInformation
                           {
                               ReportId = reportRec.reportid,
                               Agency = reportRec.agency,
                               ColdFusionBox = reportRec.cfbox,
                               UserNumber = reportRec.usernumber,
                               ServerNumber = reportRec.svrnumber
                           };
            }
        }

        private void RunReport(PendingReportInformation report)
        {
            LOG.Info($"Start {MethodBase.GetCurrentMethod().Name} ");
            var rs = new ReportSwitch { DevMode = true, DoneEvent = new ManualResetEvent(false), IsOfflineServer = false };
            rs.RunReport(report, new LoadedListsParams());
        }

        private async void btnRunBroadcastServer_Click(object sender, EventArgs e)
        {
            LOG.Info($"Start Test Batch:{GetBatchNumber()}");

            try
            {
                lblProcessingBcst.Visible = true;

                await Task.WhenAll(ProcessBcstAsync());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                lblProcessingBcst.Visible = false;
            }
        }

        private async Task ProcessBcstAsync()
        {
            await Task.Run(() =>
                {
                    var batchNumber = GetBatchNumber();

                    var bcst = GetBatchByBatchNumber(batchNumber);

                    var serverNumber = GetServerNumber();

                    RunBroadcast(bcst, serverNumber);
                });
        }

        private int GetBatchNumber()
        {
            var batchNumber = txtBatchNumber.Text;

            if (string.IsNullOrEmpty(batchNumber))
            {
                throw new Exception("Must supply batch number.");
            }

            var batchNum = 0;
            if (!int.TryParse(batchNumber, out batchNum))
            {
                throw new Exception("Batch number must be an integer.");
            }

            return batchNum;
        }

        private bcstque4 GetBatchByBatchNumber(int batchNumber)
        {
            var query = new GetBroadcastByBatchNumberQuery(new iBankMastersQueryable(), batchNumber);
            var bcst = query.ExecuteQuery();

            if (bcst == null) throw new Exception($"No broadcast found using batch number {batchNumber}");

            return bcst;
        }

        private int GetServerNumber()
        {
            var temp = txtServerNumber.Text;

            if (string.IsNullOrEmpty(temp))
            {
                throw new Exception("Must supply batch number.");
            }

            var serverNumber = 0;
            if (!int.TryParse(temp, out serverNumber))
            {
                throw new Exception("Server number must be an integer.");
            }

            return serverNumber;
        }

        private void RunBroadcast(bcstque4 bcst, int serverNumber)
        {
            LOG.Info($"Start {MethodBase.GetCurrentMethod().Name} ");
            var p = SetUpParameters(serverNumber);

            var consumer = new BroadcastServer.Service.Consumer();
            consumer.ProcessBatch(bcst, p, new LoadedListsParams());
        }

        private Parameters SetUpParameters(int serverNumber)
        {
            LOG.Info($"Start {MethodBase.GetCurrentMethod().Name} | ServerNumber: {serverNumber}");
            var p = new Parameters(new BroadcastQueueRecordRemover(), new BroadcastRecordUpdatesManager())
                        {
                            MasterDataStore = new MasterDataStore(),
                            DatabaseInfoQuery = null,
                            ClientDataStore = null,
                            ServerConfiguration = new BroadcastServerInformation
                                                      {
                                                          ReportLogoDirectory = ConfigurationManager.AppSettings["LogoTempDirectory"],
                                                          CrystalReportDirectory = ConfigurationManager.AppSettings["CrystalReportsDirectory"],
                                                          ServerNumber = serverNumber,
                                                          ReportOutputDirectory = ConfigurationManager.AppSettings["ReportOutputDirectory"],
                                                          SenderEmailAddress = ConfigurationManager.AppSettings["SenderEmailAddress"],
                                                          SenderName = ConfigurationManager.AppSettings["SenderName"],
                                                          IbankBaseUrl = ConfigurationManager.AppSettings["IbankBaseUrl"]
                                                      }
                        };


            var getServerFunctionQuery = new GetBroadcastServerFunctionByServerNumberQuery(new iBankAdministrationQueryable(), p.ServerConfiguration.ServerNumber);
            p.ServerConfiguration.ServerFunction = getServerFunctionQuery.ExecuteQuery();

            LOG.Info($"End {MethodBase.GetCurrentMethod().Name} | ServerNumber: {serverNumber}");
           return p;
        }

        private void btnRunReportQueueManager_Click(object sender, EventArgs e)
        {
            var wrapper = new ConfigurationWrapper();
            var mgr = new QueueManager(new MasterDataStore(), wrapper);
            var state = new MaintenanceModeState(ServerType.ReportQueueManager, wrapper.ServerNumber);
            mgr.Run(state);
        }
    }
}
