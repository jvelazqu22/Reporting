using CODE.Framework.Core.Utilities.Extensions;
using iBank.BroadcastServer.QueueManager.Cleaner;

using System;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using Domain.Services;

namespace iBank.BroadcastServer.QueueManager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            ICacheService _cache = new CacheService();
            var server = new QueueManagementServer();
            server.BuildQueue(_cache);
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new QueueManagerService()
            };
            ServiceBase.Run(ServicesToRun);
#endif

        }
    }
}
