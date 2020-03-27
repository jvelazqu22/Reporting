using CODE.Framework.Core.Utilities.Extensions;
using com.ciswired.libraries.CISLogger;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Logging;
using System;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using Domain.Models;
using Domain.Models.ReportPrograms.TripChangesMeetAndGreetReport;
using Domain.Orm.iBankAdministrationCommands;
using Domain.Orm.iBankAdministrationQueries;
using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using Timer = System.Timers.Timer;

namespace iBank.ReportServer.Service
{
    public partial class OnlineReportService : ServiceBase
    {
        private readonly HeartbeatLogger _heartbeatLogger;
        private readonly string _machineName = Environment.MachineName;
        private readonly int _serverNumber = ConfigurationManager.AppSettings["ServerNumber"].TryIntParse(0);
        private readonly int _numberOfMinutesToWaitForMaintenanceModeRequestInReStartService = 3;

        private readonly ServerType _serverType = ServerType.OnlineReportServer;

        private readonly Timer _reportServerTimer;
        private readonly Timer _heartbeatTimer;
        public int TimerInterval { get; set; }

        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ApplicationInformation _applicationInformation = new ApplicationInformation(Assembly.GetExecutingAssembly());
        private readonly LoadedListsParams _loadedParams = new LoadedListsParams();

        public OnlineReportService()
        {
            InitializeComponent();
            SetUpAutoMapper();
            _heartbeatLogger = new HeartbeatLogger(_serverType, _serverNumber, _machineName, _applicationInformation);
            _heartbeatTimer = new Timer(120000) { AutoReset = false };
            _reportServerTimer = new Timer(5000) { AutoReset = false };

            _reportServerTimer.Elapsed += RunReportServer;
            _heartbeatTimer.Elapsed += SendHeartbeat;

            // Do not apply the ReStart() logic on the GSA server by checking the machine name
            if (_machineName.Equals("GSA02", StringComparison.OrdinalIgnoreCase)) return;

            // Do not apply the ReStart() logic on the GSA server by checking the environment
#if ProductionGSA
                return;
#endif

            LOG.Info($" before - Task.Run(() => ReStart());");
            Task.Run(() => ReStart());
            LOG.Info($"after - Task.Run(() => ReStart());");
        }

        private void SetUpAutoMapper()
        {
            try
            {
                if (Features.AutoMapperInitializer.IsEnabled())
                {
                    // This set up needs to happen once in one place otherwise, the following exception will be thrown:
                    // AutoMapper.AutoMapperMappingException: Missing type map configuration or unsupported mapping.
                    Mapper.Initialize(cfg => {
                        cfg.CreateMap<FinalData, FinalDataExport>();
                        cfg.CreateMap<SqlScript, SqlScript>();
                    });
                }
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
        }

        private void RunReportServer(object sender, EventArgs e)
        {
            LOG.Info($"RunBroadcastServer for {ServerName}.");

            _reportServerTimer.Stop();
            try
            {
                try
                {
                    var reportServer = new ReportServer(new MasterDataStore(), false, _loadedParams);
                    reportServer.Run();
                    _reportServerTimer.Interval = 5000;
                }
                catch (DbEntityValidationException ex)
                {
                    var errorMsg = ex.GetValidationErrors();
                    LOG.Error($"Error executing report server. {errorMsg}");
                    LOG.Error($"iBank Report Server reported an exception: {errorMsg}");
                    _reportServerTimer.Interval = 60000;//if we fail, don't try again for a full minute
                }
                catch (Exception ex)
                {
                    LOG.Error(ex.Message, ex);
                    LOG.Error($"iBank Report Server reported an exception: {ex.Message}");
                    _reportServerTimer.Interval = 60000;//if we fail, don't try again for a full minute
                }
            }
            catch (Exception ex)
            {
                LOG.Error("iBank.ReportServer encountered exception.", ex);
            }
            finally
            {
                _reportServerTimer.Start();
            }
        }

        private void SendHeartbeat(object sender, EventArgs e)
        {
            try
            {
                _heartbeatTimer.Stop();
                _heartbeatLogger.SendHearbeat();
            }
            catch (Exception ex)
            {
                LOG.Error("Error occurred sending heartbeat.", ex);
                //swallow exception so heartbeat logging can't kill app
            }
            finally
            {
                _heartbeatTimer.Start();
            }
        }

        protected override void OnStart(string[] args)
        {
            LOG.Info($"Method OnStart for {ServerName}.");
            try
            {
                RemoveMaintenanceMode(); // this is here in case the service is in maintenance mode

                LOG.Debug("Starting heartbeat.");
                _heartbeatLogger.StartHeartbeat();
            }
            catch (Exception ex)
            {
                //swallow exception so logging doesn't kill app
                LOG.Error("Error occurred during heartbeat logging.", ex);
            }

            LOG.Info("iBank Report Server starting...");

            _reportServerTimer.Start();
            _heartbeatTimer.Start();
        }

        protected override void OnStop()
        {
            _reportServerTimer.Stop();
            LOG.Info("iBank Report Server shutting down...");
            KillHeartBeatLogger();
        }

        private void KillHeartBeatLogger()
        {
            try
            {
                _heartbeatLogger.KillHeartbeat();
            }
            catch (Exception ex)
            {
                //swallow exception so logging doesn't kill app
                LOG.Error("Error occurred during heartbeat logging.", ex);
            }
        }

        private void RemoveMaintenanceMode()
        {
            try
            {
                var svrStatusRec = new GetSvrStatusRecordQuery(new iBankAdministrationQueryable(), ServerName).ExecuteQuery();
                svrStatusRec.maintenance_mode_requested = false;
                svrStatusRec.in_maintenance_mode = false;
                new UpdateSvrStatusRecordCommand(new iBankAdministrationCommandDb(), svrStatusRec).ExecuteCommand();
                LOG.Info($"RemoveMaintenanceMode() for {ServerName}.");
            }
            catch (Exception ex)
            {
                LOG.Error($"Error occurred during RemoveMaintenanceMode() for {ServerName}.", ex);
            }
        }

        public string ServerName => _serverType + "." + _serverNumber;

        /*
         * Sometimes we find events that require us to re-start the service to flush them out.
         * this method stops the service with an error. And the service is 
         * configure to re-start itself when an error is found.
         */
        private void ReStart()
        {
            while (true)
            {
                try
                {
                    var timer = new Randomizer().GetRandomNumberOfMillisecondsBeforeNextReStart();
                    var nextRestartTime = DateTime.Now.AddMilliseconds(timer.TotalTimeInMilliseconds);
                    nextRestartTime = nextRestartTime.AddMinutes(_numberOfMinutesToWaitForMaintenanceModeRequestInReStartService);
                    var nextRestartDateAndTimeStr = string.Format("Day: {0:d} Time: {0:t}\n", nextRestartTime);
                    var logMsg = $"Sleep Hours: {timer.Hours} Sleep Minutes: {timer.Minutes} "
                                 + $"Total sleep time in milliseconds: {timer.TotalTimeInMilliseconds} "
                                 + $"Server: {ServerName} next RestartTime including extra 3 minutes {nextRestartDateAndTimeStr}";
                    LOG.Info(logMsg);
                    Thread.Sleep(timer.TotalTimeInMilliseconds);
                }
                catch (Exception ex)
                {
                    LOG.Error($"Error occurred during ReStart for {ServerName}.", ex);
                }

                UpdateCleanAndStopService();
            }
        }

        private void UpdateCleanAndStopService()
        {
            try
            {
                var svrStatusRec = new GetSvrStatusRecordQuery(new iBankAdministrationQueryable(), ServerName).ExecuteQuery();
                svrStatusRec.maintenance_mode_requested = true;
                new UpdateSvrStatusRecordCommand(new iBankAdministrationCommandDb(), svrStatusRec).ExecuteCommand();

                KillHeartBeatLogger();
                _reportServerTimer.Stop();

                // wait 3 minutes
                var logMsg = $"Sleep Minutes: {_numberOfMinutesToWaitForMaintenanceModeRequestInReStartService} "
                             + $"Total sleep time in milliseconds: {_numberOfMinutesToWaitForMaintenanceModeRequestInReStartService * 60 * 1000} "
                             + $"Server: {ServerName}";
                LOG.Info(logMsg);

                Thread.Sleep(_numberOfMinutesToWaitForMaintenanceModeRequestInReStartService * 60 * 1000);

                LOG.Info($"before - Environment.Exit(1);");
                Environment.Exit(1);
                LOG.Info($"after - Environment.Exit(1);"); // this should not log, but just adding it anyway just in case while debugging stuff
            }
            catch (Exception ex)
            {
                LOG.Error($"Error occurred during UpdateCleanAndStopService for {ServerName}.", ex);
            }
        }
    }
}
