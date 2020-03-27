using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using iBank.BroadcastServer.Service;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Logging;
using iBank.Services.Implementation.Shared;

namespace iBank.BroadcastServer
{
    static class Program
    {
        private static readonly ApplicationInformation _applicationInformation = new ApplicationInformation(Assembly.GetExecutingAssembly());
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            var bc = new Service.BroadcastServer(new MasterDataStore(), new LoadedListsParams());
            bc.Run();
#else
             ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new BroadcastService()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
