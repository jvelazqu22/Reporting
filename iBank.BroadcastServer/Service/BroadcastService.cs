using System;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using com.ciswired.libraries.CISLogger;
using CODE.Framework.Core.Utilities.Extensions;
using Domain;
using Domain.Models;
using Domain.Models.ReportPrograms.TripChangesMeetAndGreetReport;
using Domain.Orm.iBankAdministrationCommands;
using Domain.Orm.iBankAdministrationQueries;
using Domain.Orm.iBankMastersCommands.Broadcast;
using Domain.Orm.iBankMastersQueries.BroadcastQueries;
using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Helpers;
using iBank.Server.Utilities.Logging;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.MacroHelpers;
using iBank.Services.Implementation.Utilities;
using Timer = System.Timers.Timer;

namespace iBank.BroadcastServer.Service
{
    public partial class BroadcastService : ServiceBase
    {
        private readonly HeartbeatLogger _heartbeatLogger;
        private readonly string _machineName = Environment.MachineName;
        private readonly int _serverNumber = ConfigurationManager.AppSettings["ServerNumber"].TryIntParse(0);
        private readonly int _numberOfMinutesToWaitForMaintenanceModeRequestInReStartService = ConfigurationManager.AppSettings["NumberOfMinutesToWaitForMaintenanceModeRequestInReStartService"].TryIntParse(3);
        private readonly ServerType _serverType = ServerType.BroadcastServer;

        private readonly Timer _broadcastTimer;
        private readonly Timer _heartbeatTimer;

        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly ApplicationInformation _applicationInformation = new ApplicationInformation(Assembly.GetExecutingAssembly());
        private readonly LoadedListsParams _loadedParams = new LoadedListsParams();

        public BroadcastService()
        {
            InitializeComponent();
            SetUpAutoMapper();
            _heartbeatLogger = new HeartbeatLogger(_serverType, _serverNumber, _machineName, _applicationInformation);

            _heartbeatTimer = new Timer(120000) { AutoReset = false };
            _broadcastTimer = new Timer(5000) { AutoReset = false };

            _heartbeatTimer.Elapsed += SendHeartbeat;
            _broadcastTimer.Elapsed += RunBroadcastServer;

            new ExcelFunctions().KillOldExcelProcesses();

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
                        cfg.CreateMap<ibbatch, ibbatchhistory>();
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

        private void RunBroadcastServer(object sender, EventArgs e)
        {
            LOG.Info($"RunBroadcastServer for {ServerName}.");

            _broadcastTimer.Stop();

            try
            {
                var bcServer = new BroadcastServer(new MasterDataStore(), _loadedParams);
                bcServer.Run();
                _broadcastTimer.Interval = 5000;
            }
            catch (DbEntityValidationException ex)
            {
                LOG.Error($"iBank Broadcast Server Services reported an exception: {EFExceptionHelper.ProcessDbEntityValidationErrors("Failed to run Broadcasts", ex)}");
                _broadcastTimer.Interval = 60000;//if we fail, don't try again for a full minute
            }
            catch (Exception ex)
            {
                LOG.Error($"iBank Broadcast Server Services reported an exception: {ex.Message}");
                LOG.Error("iBank Broadcast Server Service failed.", ex);
                _broadcastTimer.Interval = 60000;//if we fail, don't try again for a full minute
            }
            finally
            {
                _broadcastTimer.Start();
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
                _heartbeatLogger.StartHeartbeat();
            }
            catch (Exception ex)
            {
                //swallow exception so logging doesn't kill app
                LOG.Error("Error occurred during heartbeat or RemoveMaintenanceMode logging.", ex);
            }

            _broadcastTimer.Start();
            _heartbeatTimer.Start();
        }

        protected override void OnStop()
        {
            LOG.Info($"Method OnStop for {ServerName}.");
            StopEventsKillHeartBeatAndCleanupBroadcasts();
        }

        private void StopEventsKillHeartBeatAndCleanupBroadcasts()
        {
            LOG.Info($"Method StopEventsKillHeartBeatAndCleanupBroadcasts for {ServerName}.");

            _broadcastTimer.Stop();
            _heartbeatTimer.Stop();

            try
            {
                CleanUpRunningBroadcasts();

                LOG.Info($"Killing heartbeat for [{ServerType.BroadcastServer}].[{_serverNumber}]");
                _heartbeatLogger.KillHeartbeat();
            }
            catch (Exception ex)
            {
                //swallow exception so logging doesn't kill app
                LOG.Error("Error occurred during heartbeat logging.", ex);
            }
        }

        private void CleanUpRunningBroadcasts()
        {
            var store = new MasterDataStore();
            var query = new GetBroadcastsRunningOnServerQuery(store.MastersQueryDb, _serverNumber);
            var runningBroadcasts = query.ExecuteQuery();

            var cmd = new RemoveBatchFromBroadcastQueueCommand(store.MastersCommandDb, runningBroadcasts);
            cmd.ExecuteCommand();
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

        private void ReStart()
        {
            while (true)
            {
                LOG.Info($"ReStart for {ServerName}.");

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
                // wait 3 minutes
                var logMsg = $"Sleep Minutes: {_numberOfMinutesToWaitForMaintenanceModeRequestInReStartService} "
                             + $"Total sleep time in milliseconds: {_numberOfMinutesToWaitForMaintenanceModeRequestInReStartService * 60 * 1000} "
                             + $"Server: {ServerName}";
                LOG.Info(logMsg);
                Thread.Sleep(_numberOfMinutesToWaitForMaintenanceModeRequestInReStartService * 60 * 1000);
                StopEventsKillHeartBeatAndCleanupBroadcasts();

                RemoveMaintenanceMode();

                // terminates the process with error code 1. The service has been set up to re-start when it errors.
                // This did not work: Stop();
                LOG.Info($"before - Environment.Exit(1);");
                Environment.Exit(1);
                LOG.Info($"after - Environment.Exit(1);"); // this should not log, but just adding it anyway just in case while debugging stuffB
            }
            catch (Exception ex)
            {
                LOG.Error($"Error occurred during UpdateCleanAndStopService for {ServerName}.", ex);
            }
        }

    }
}
