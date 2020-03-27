using iBank.ReportServer.Service;
using System.ServiceProcess;
using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Shared;

namespace iBank.ReportServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            var server = new Service.ReportServer(new MasterDataStore(), true, new LoadedListsParams());
            server.Run();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new OnlineReportService()
            };
            ServiceBase.Run(ServicesToRun);

#endif

        }
    }
}
