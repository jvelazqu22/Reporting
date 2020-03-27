using System;
using System.Configuration;
using System.Reflection;
using System.Threading.Tasks;

using com.ciswired.libraries.CISLogger;

using CODE.Framework.Core.Utilities.Extensions;

using Domain.Orm.iBankMastersQueries.OverdueMonitor;

using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;

namespace iBank.OverdueBroadcastMonitor
{
    public class Monitor
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IMasterDataStore MasterDataStore { get; set; }

        public Monitor()
        {
            MasterDataStore = new MasterDataStore();
        }

        public Monitor(IMasterDataStore store)
        {
            MasterDataStore = store;
        }

        public void Run()
        {
            try
            {
                //get all the databases that need to be monitored
                var dbQuery = new GetAllActiveDatabasesQuery(MasterDataStore.MastersQueryDb);
                var databases = dbQuery.ExecuteQuery();

                var thresholdInMinutes = ConfigurationManager.AppSettings["OverdueThresholdInMinutes"].TryIntParse(30);
                var threshold = ThresholdCalculator.CalculateThreshold(DateTime.Now, thresholdInMinutes);

                foreach (var database in databases)
                {
                    var dbMonitor = new DatabaseMonitor(MasterDataStore);
                    dbMonitor.ProcessDatabase(database, threshold);
                }
            }
            catch (Exception ex)
            {
                LOG.Error(ex.ToString(), ex);
            }
        }
    }
}
