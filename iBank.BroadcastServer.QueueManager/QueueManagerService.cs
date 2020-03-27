using System;
using System.Configuration;
using System.Reflection;
using System.ServiceProcess;
using System.Timers;
using com.ciswired.libraries.CISLogger;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Services;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Logging;

namespace iBank.BroadcastServer.QueueManager
{
    public partial class QueueManagerService : ServiceBase
    {
        private readonly HeartbeatLogger _heartbeatLogger;
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        private readonly string _machineName = Environment.MachineName;
        private readonly int _serverNumber = ConfigurationManager.AppSettings["ServerNumber"].TryIntParse(0);
        private readonly ServerType _serverType = ServerType.BroadcastQueueManager;

        private readonly Timer _queueBuilderTimer;
        private readonly Timer _heartbeatTimer;

        private readonly double _queueBuilderInterval = double.Parse(ConfigurationManager.AppSettings["QueueBuilderServiceInterval"]);
        private static readonly ApplicationInformation _applicationInformation = new ApplicationInformation(Assembly.GetExecutingAssembly());

        public QueueManagerService()
        {
            InitializeComponent();

            _heartbeatLogger = new HeartbeatLogger(_serverType, _serverNumber, _machineName, _applicationInformation);

            LOG.Info("Manager starting up.");
            LOG.Debug($"Queue builder runs every [{_queueBuilderInterval}] ms");
            _queueBuilderTimer = new Timer(_queueBuilderInterval) { AutoReset = false };
            _heartbeatTimer = new Timer(120000) { AutoReset = false };

            _queueBuilderTimer.Elapsed += RunQueueBuilder;
            _heartbeatTimer.Elapsed += SendHeartbeat;
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

        //add records to the queue
        private void RunQueueBuilder(object sender, EventArgs e)
        {
            _queueBuilderTimer.Stop();

            try
            {
                LOG.Debug("Queue builder kicking off.");
                var queueManagementServer = new QueueManagementServer();

                try
                {
                    queueManagementServer.BuildQueue(new CacheService());
                    queueManagementServer.CleanQueue();
                }
                catch (AggregateException ex)
                {
                    foreach (var exc in ex.InnerExceptions)
                    {
                        LOG.Error(exc.Message, exc);
                    }
                }
                catch (Exception ex)
                {
                    LOG.Error("Exception encountered in queue builder. Message " + ex.Message, ex);

                    //if we fail give the environment a minute to potentially reset
                    LOG.Error($"Error occurred in Queue Manager. [Queue builder Failed. Exception: {ex.Message}]");
                    _queueBuilderTimer.Interval = 60000;
                }

                _queueBuilderTimer.Interval = _queueBuilderInterval;
                LOG.Debug("Queue builder going to sleep");
            }
            catch (Exception ex)
            {
                LOG.Error(ex.Message, ex);
            }
            finally
            {
                _queueBuilderTimer.Start();
            }
        }

        protected override void OnStart(string[] args)
        {
            LOG.Info("Queue Manager service starting...");

            StartAllTimers();

            try
            {
                _heartbeatLogger.SendHearbeat();
            }
            catch (Exception ex)
            {
                //swallow exception so logging doesn't kill app
                LOG.Error("Error occurred during heartbeat logging.", ex);
            }
        }

        protected override void OnStop()
        {
            LOG.Info("Queue Manager service stopping...");
            StopAllTimers();

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

        private void StartAllTimers()
        {
            _queueBuilderTimer.Start();
            _heartbeatTimer.Start();
        }

        private void StopAllTimers()
        {
            _queueBuilderTimer.Stop();
            _heartbeatTimer.Stop();
        }
    }
}
