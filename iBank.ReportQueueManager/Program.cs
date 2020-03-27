using System.ServiceProcess;
using iBank.ReportQueueManager.Helpers;
using iBank.ReportQueueManager.Service;
using iBank.Repository.SQL.Repository;

namespace iBank.ReportQueueManager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            var rptQueueMgr = new QueueManager(new MasterDataStore(), new ConfigurationWrapper());
            rptQueueMgr.RunCleaner();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ReportQueueManagerService()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
