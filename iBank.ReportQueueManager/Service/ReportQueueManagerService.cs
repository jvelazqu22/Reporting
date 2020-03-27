using System;
using System.Configuration;
using System.Reflection;
using System.ServiceProcess;
using System.Timers;
using com.ciswired.libraries.CISLogger;
using CODE.Framework.Core.Utilities.Extensions;
using iBank.ReportQueueManager.Helpers;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Logging;
using Timer = System.Timers.Timer;

namespace iBank.ReportQueueManager.Service
{
    public partial class ReportQueueManagerService : ServiceBase
    {
        private readonly HeartbeatLogger _heartbeatLogger;
        private readonly string _machineName = Environment.MachineName;
        private readonly int _serverNumber = ConfigurationManager.AppSettings["ServerNumber"].TryIntParse(0);
        private readonly ServerType _serverType = ServerType.ReportQueueManager;
        private readonly Timer _queueManagerTimer;
        private readonly Timer _heartbeatTimer;
        private readonly Timer _cleanerTimer;

        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ApplicationInformation _applicationInformation = new ApplicationInformation(Assembly.GetExecutingAssembly());

        public ReportQueueManagerService()
        {
            LOG.Debug($"Start {MethodBase.GetCurrentMethod().Name} ");
            InitializeComponent();

            _heartbeatLogger = new HeartbeatLogger(_serverType, _serverNumber, _machineName, _applicationInformation);
            _heartbeatTimer = new Timer(2*60*1000) { AutoReset = false };
            _heartbeatTimer.Elapsed += SendHeartbeat;

            // run every 5 seconds
            _queueManagerTimer = new Timer(5*1000) { AutoReset = false }; 
            _queueManagerTimer.Elapsed += RunManager;

            //run every 15 minutes
            _cleanerTimer = new Timer(15 * 60 * 1000) { AutoReset = false };
            _cleanerTimer.Elapsed += RunCleaner;
        }

        private void RunCleaner(object sender, ElapsedEventArgs e)
        {
            LOG.Debug($"Start {MethodBase.GetCurrentMethod().Name} ");
            _cleanerTimer.Stop();

            try
            {
                new QueueCleaner().CleanQueue(DateTime.Now.AddHours(-1), new MasterDataStore());
            }
            catch (Exception ex)
            {
                LOG.Error("ibank report queue manager queue cleaner encountered an exception.", ex);
            }
            finally
            {
                _cleanerTimer.Start();
            }
        }

        private void RunManager(object sender, ElapsedEventArgs e)
        {
            LOG.Debug($"Start {MethodBase.GetCurrentMethod().Name} ");
            _queueManagerTimer.Stop();

            try
            {
                var store = new MasterDataStore();
                var wrapper = new ConfigurationWrapper();
                var mgr = new QueueManager(store, wrapper);
                var state = new MaintenanceModeState(ServerType.ReportQueueManager, wrapper.ServerNumber);
                mgr.Run(state);
            }
            catch (Exception ex)
            {
                LOG.Error("iBank.ReportQueueManager encountered an exception.", ex);
            }
            finally
            {
                _queueManagerTimer.Start();
            }
        }

        private void SendHeartbeat(object sender, ElapsedEventArgs e)
        {
            LOG.Debug($"Start {MethodBase.GetCurrentMethod().Name} ");
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
            LOG.Info($"Start {MethodBase.GetCurrentMethod().Name} ");
            try
            {
                _heartbeatLogger.StartHeartbeat();
            }
            catch (Exception ex)
            {
                //swallow exception so logging doesn't kill app
                LOG.Error("Error occurred during heartbeat logging.", ex);
            }
            finally
            {
                _heartbeatTimer.Start();
                _queueManagerTimer.Start();
                _cleanerTimer.Start();
            }
        }

        protected override void OnStop()
        {
            LOG.Info($"Start {MethodBase.GetCurrentMethod().Name} ");
            _queueManagerTimer.Stop();
            _heartbeatTimer.Stop();
            _cleanerTimer.Stop();

            try
            {
                LOG.Info($"Killing heartbeat for [{ServerType.ReportQueueManager}].[{_serverNumber}]");
                _heartbeatLogger.KillHeartbeat();
            }
            catch (Exception ex)
            {
                //swallow exception so logging doesn't kill app
                LOG.Error("Error occurred during heartbeat logging.", ex);
            }
        }
    }
}
