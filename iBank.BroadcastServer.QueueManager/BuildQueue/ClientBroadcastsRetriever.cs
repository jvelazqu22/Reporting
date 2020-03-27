using System;
using System.Collections.Generic;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using Domain.Orm.iBankClientQueries.BroadcastQueries;
using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

namespace iBank.BroadcastServer.QueueManager.BuildQueue
{
    public class ClientBroadcastsRetriever
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IClientDataStore ClientDataStore { get; set; }
        public DateTime CycleTimeZone { get; set; }
        public IList<string> AgencyNames { get; set; }

        public ClientBroadcastsRetriever(IClientDataStore clientDataStore, DateTime cycleTimeZone, IList<string> agencyNames)
        {
            ClientDataStore = clientDataStore;
            CycleTimeZone = cycleTimeZone;
            AgencyNames = agencyNames;
        }

        public List<ibbatch> GetAllClientBatches()
        {
            var allBatches = new GetAllClientBatchQuery(ClientDataStore.ClientQueryDb, CycleTimeZone, AgencyNames).ExecuteQuery();
            LOG.Debug($"Retrieved [{allBatches.Count}] all batches.");

            return allBatches;
        }
    }
}
