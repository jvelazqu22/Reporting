using System.Collections.Concurrent;
using System.Collections.Generic;
using Domain.Helper;
using Domain.Orm.Classes;
using Domain.Services;
using iBank.Repository.SQL.Interfaces;

namespace Domain.Models.OnlineReportServer
{
    public class Parameters
    {
        public Parameters(IMasterDataStore masterDataStore)
        {
            MasterDataStore = masterDataStore;
        }
        public IMasterDataStore MasterDataStore { get; set; }
        public bool IsMaintenanceModeRequested { get; set; }
        public BlockingCollection<PendingReportInformation> ReportsToExecute { get; set; } = new BlockingCollection<PendingReportInformation>();
        public int ServerNumber { get; set; }
        public bool IsDemoVersion { get; set; }
        public bool IsDevMode { get; set; }
        public IList<int> DemoUsers { get; set; } = new List<int>();
        public ReportServerFunction ServerFunction { get; set; }
        public ICacheService Cache { get; set; } = new CacheService();
    }
}
