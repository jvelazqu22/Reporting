using System;
using System.Configuration;
using System.Reflection;
using System.ServiceProcess;
using System.Timers;

using com.ciswired.libraries.CISLogger;

using CODE.Framework.Core.Utilities.Extensions;

using iBank.Server.Utilities;
using iBank.Server.Utilities.Logging;

namespace iBank.OverdueBroadcastMonitor
{
    partial class OverdueMonitorService : ServiceBase
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly HeartbeatLogger _heartbeatLogger;
        private readonly System.Timers.Timer _heartbeatTimer;
        private static readonly ApplicationInformation _applicationInformation = new ApplicationInformation(Assembly.GetExecutingAssembly());
        private readonly int _serverNumber = ConfigurationManager.AppSettings["ServerNumber"].TryIntParse(0);

        private readonly Timer _monitorTimer;
        public OverdueMonitorService()
        {
            InitializeComponent();

            _heartbeatLogger = new HeartbeatLogger(ServerType.OverdueBroadcastMonitor, _serverNumber, Environment.MachineName, _applicationInformation);
            _heartbeatTimer = new System.Timers.Timer(120000) { AutoReset = false };
            _heartbeatTimer.Elapsed += new ElapsedEventHandler(SendHeartbeat);

            _monitorTimer = new Timer(60000) { AutoReset = false };
            _monitorTimer.Elapsed += new ElapsedEventHandler(RunOverdueMonitor);
            _monitorTimer.Start();
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

        private void RunOverdueMonitor(object sender, ElapsedEventArgs e)
        {
            try
            {
                _monitorTimer.Stop();

                var monitor = new Monitor();
                monitor.Run();
            }
            catch (Exception ex)
            {
                LOG.Error(ex.ToString(), ex);
            }
            finally
            {
                _monitorTimer.Start();
            }
        }

        protected override void OnStart(string[] args)
        {
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
            }
        }

        protected override void OnStop()
        {
            _monitorTimer.Stop();
            _heartbeatTimer.Stop();

            try
            {
                LOG.Info(string.Format("Killing heartbeat for [{0}].[{1}]", ServerType.OverdueBroadcastMonitor, _serverNumber));
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
