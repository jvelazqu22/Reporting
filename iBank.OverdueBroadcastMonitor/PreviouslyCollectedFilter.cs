using System.Collections.Generic;
using System.Linq;

using Domain.Orm.iBankMastersQueries.OverdueMonitor;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;

namespace iBank.OverdueBroadcastMonitor
{
    public class BroadcastFilter
    {
        public IList<ibbatch> FilterOutPreviouslyCollectedBroadcasts(IList<ibbatch> overdueBroadcasts, IMasterDataStore store)
        {
            var query = new GetExistingOverdueBroadcastRecordsQuery(store.MastersQueryDb);
            var existingOverdueBroadcasts = query.ExecuteQuery();

            if (!existingOverdueBroadcasts.Any()) return overdueBroadcasts;

            var filteredBroadcasts = new List<ibbatch>();
            foreach (var bcst in overdueBroadcasts)
            {
                var rec = existingOverdueBroadcasts.FirstOrDefault(x => x.agency?.Trim() == bcst.agency?.Trim()
                                                                   && x.batchname?.Trim() == bcst.batchname?.Trim()
                                                                   && x.batchnum == bcst.batchnum
                                                                   && x.nextrun == bcst.nextrun
                                                                   && x.UserNumber == bcst.UserNumber);
                if (rec == null) filteredBroadcasts.Add(bcst);
            }

            return filteredBroadcasts;
        }

        public IList<ibbatch> FilterOutOfflineBroadcasts(IList<ibbatch> overdueBroadcasts)
        {
            var filteredBroadcasts = new List<ibbatch>();

            foreach (var bcst in overdueBroadcasts)
            {
                if (!BroadcastBatchTypeConditionals.IsBatchRunOffline(bcst.batchname)) filteredBroadcasts.Add(bcst);
            }

            return filteredBroadcasts;
        }

        public IList<ibbatch> FilterOutInactiveAgencies(IList<ibbatch> overdueBroadcasts, IMasterDataStore store, string databaseName)
        {
            var filteredBroadcasts = new List<ibbatch>();

            var query = new GetAllActiveAgenciesOnDatabaseQuery(store.MastersQueryDb, databaseName);
            var activeAgencies = query.ExecuteQuery();

            foreach (var bcst in overdueBroadcasts)
            {
                if (activeAgencies.Contains(bcst.agency.Trim())) filteredBroadcasts.Add(bcst);
            }

            return filteredBroadcasts;
        }
    }
}
