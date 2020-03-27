using System;
using System.Configuration;
using System.Reflection;
using System.ServiceProcess;

using com.ciswired.libraries.CISLogger;

using CODE.Framework.Core.Utilities.Extensions;

using iBank.Server.Utilities;
using iBank.Server.Utilities.Logging;

namespace iBank.OverdueBroadcastMonitor
{
    class Program
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //entry point for process
        static void Main(string[] args)
        {

#if DEBUG
             HeartbeatLogger _heartbeatLogger;
            ApplicationInformation _applicationInformation = new ApplicationInformation(Assembly.GetExecutingAssembly());
            int _serverNumber = ConfigurationManager.AppSettings["ServerNumber"].TryIntParse(0);

            _heartbeatLogger = new HeartbeatLogger(ServerType.OverdueBroadcastMonitor, _serverNumber, Environment.MachineName, _applicationInformation);
            _heartbeatLogger.StartHeartbeat();

            var monitor = new Monitor();
            monitor.Run();
#else
            try
            {
                var servicesToRun = new ServiceBase[]
                                        {
                                            new OverdueMonitorService()
                                        };
                ServiceBase.Run(servicesToRun);
            }
            catch (Exception ex)
            {
                LOG.Error(ex.ToString(), ex);
            }
#endif

        }
    }
}
