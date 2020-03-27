using com.ciswired.libraries.CISLogger;
using Domain.Helper;
using Domain.Orm.iBankAdministrationQueries;
using Domain.Orm.iBankMastersQueries.BroadcastQueries;
using Domain.Services;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace iBank.BroadcastServer.Helper
{
    public class LongRunningHandler
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        private static readonly CacheKeys _key = CacheKeys.BroadcastLongRunningServers;

        public List<int> GetOtherBroadcastLongRunningServer(ICacheService cache, int serverNumber)
        {
            List<int> temp;
            if (!cache.TryGetValue(_key, out temp))
            {
                temp = new GetBroadcastLongRunningOtherServerQuery(new iBankAdministrationQueryable(), serverNumber).ExecuteQuery();
                cache.Set(_key, temp, DateTime.Now.AddDays(1));
            }

            return temp;
        }

        public bool IsThereDatabaseConstraint(IMasterDataStore masterStore, ICacheService cache, int serverNumber, bcstque4 batch)
        {
            // for that agency, is it regular or shared
            // get db(s) that agency uses - if shared, get agencies that belong to shared parent
            //check mstragcy for the agency or agencies (if shared) and get the db(s)
            var getBroadcastLongRunningOtherDatabasesQuery = new GetBroadcastLongRunningOtherDatabasesQuery(masterStore.MastersQueryDb, GetOtherBroadcastLongRunningServer(cache, serverNumber));
            var allDatabasesOtherLongRunningServerIsUsing = getBroadcastLongRunningOtherDatabasesQuery.ExecuteQuery();

            //Free to run when no longrunning is using any dbs
            if (!allDatabasesOtherLongRunningServerIsUsing.Any()) return false;

            //get all databases current batch need - shared corpacct will have more than one databases
            var getBroadcastLongRunningByBatchDatabasesQuery = new GetBroadcastLongRunningByBatchDatabasesQuery(masterStore.MastersQueryDb, batch.batchnum ?? 0);
            var allDatabasesCurrentBatchNeed = getBroadcastLongRunningByBatchDatabasesQuery.ExecuteQuery();

            var result = allDatabasesOtherLongRunningServerIsUsing.Where(a => allDatabasesCurrentBatchNeed.Any(b => string.Compare(a.DatabaseName, b.DatabaseName, true) == 0));

            if (result.Any())
            {
                LOG.Debug($"Batch Name: {batch.batchname} (Batch Number: {batch.batchnum}) needs {string.Join(",", result.Select(x => x.DatabaseName))} database(s)");
                return true;
            }
            return false;
        }
    }
}
