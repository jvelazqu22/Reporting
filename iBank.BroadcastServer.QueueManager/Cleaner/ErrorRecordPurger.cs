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
    public class ErrorRecordPurger
    {
        private readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public void PurgeOldErroredOutBatches(int thresholdDays, DateTime today, IList<DatabaseInformation> databasesToPurge)
        {
            var threshold = today.AddDays(-thresholdDays);

            foreach (var database in databasesToPurge)
            {
                PurgeErroredOutBroadcastsInDatabase(database.DatabaseName, threshold);
            }
        }

        private void PurgeErroredOutBroadcastsInDatabase(string databaseName, DateTime threshold)
        {
            try
            {
                var clientServer = iBankServerInformationRetrieval.GetServerName(databaseName);
                LOG.Debug(string.Format("Cleaning server [{0}], database [{1}] of errored out batches.", clientServer, databaseName));

                var clientDataStore = new ClientDataStore(clientServer, databaseName);
                var purger = new BatchPurger(clientDataStore);
                purger.PurgeOldErroredOutBatches(threshold);
            }
            catch (ServerAddressNotFoundException ex)
            {
                LOG.Warn(ex.Message, ex);
            }
        }
    }
}
