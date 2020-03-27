using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using CODE.Framework.Core.Utilities.Extensions;

namespace iBank.ReportQueueManager.Helpers
{
    public interface IConfigurationWrapper
    {
        bool IsDevVersion { get; }

        IEnumerable<int> DemoUsers { get; }

        int MaxThreads { get; }

        int ServerNumber { get; }
    }

    public class ConfigurationWrapper : IConfigurationWrapper
    {
        public bool IsDevVersion
        {
            get
            {
                var setting = ConfigurationManager.AppSettings["DevVersion"];

                return setting.Equals("true", StringComparison.OrdinalIgnoreCase);
            }
        }

        public IEnumerable<int> DemoUsers
        {
            get
            {
                var demoUsers = ConfigurationManager.AppSettings["DemoUsers"];
                return demoUsers.Split(',').Select(int.Parse);
            }
        }

        public int MaxThreads
        {
            get
            {
#if DEBUG
            return 1;
#else
                var maxThreadsInConfig = ConfigurationManager.AppSettings["MaxThreads"];
                int.TryParse(maxThreadsInConfig, out var maxThreads);

                return maxThreads;
#endif
            }
        }

        public int ServerNumber => ConfigurationManager.AppSettings["ServerNumber"].TryIntParse(50);
    }
}
