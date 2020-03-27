using com.ciswired.libraries.CISLogger;
using Domain.Exceptions;
using Domain.Orm.Classes;
using iBank.Server.Utilities;

using System;
using System.Collections.Generic;
using System.Reflection;

using iBank.Repository.SQL.Repository;

namespace iBank.BroadcastServer.QueueManager.Cleaner
{
    public class OfflineRecordPurger
    {
        private readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void PurgeOldOfflineBatches(int thresholdInMinutes, DateTime today, IList<DatabaseInformation> databasesToPurge)
        {
            var threshold = today.AddMinutes(-thresholdInMinutes);

            foreach (var database in databasesToPurge)
            {
                PurgeOfflineRecordsInDatabase(database.DatabaseName, threshold);
            }
        }

        private void PurgeOfflineRecordsInDatabase(string databaseName, DateTime threshold)
        {
            try
            {
                var clientServer = iBankServerInformationRetrieval.GetServerName(databaseName);
                LOG.Debug(string.Format("Cleaning server [{0}], database [{1}] of old offline broadcasts.", clientServer, databaseName));

                var clientDataStore = new ClientDataStore(clientServer, databaseName);
                var purger = new BatchPurger(clientDataStore);
                purger.PurgeOldOfflineBatches(threshold);
            }
            catch (ServerAddressNotFoundException ex)
            {
                LOG.Warn(ex.Message, ex);
            }
        }
    }
}
