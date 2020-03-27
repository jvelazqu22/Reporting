using com.ciswired.libraries.CISLogger;

using Domain.Orm.iBankClientCommands;
using Domain.Orm.iBankClientQueries.BroadcastQueries;

using System;
using System.Linq;
using System.Reflection;

using Domain.Orm.iBankClientQueries;

using iBank.Repository.SQL.Interfaces;

namespace iBank.Server.Utilities
{
    public class BatchPurger
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private IClientDataStore ClientDataStore { get; }

        public BatchPurger(IClientDataStore clientDataStore)
        {
            ClientDataStore = clientDataStore;
        }

        /// <summary>
        /// Removes offline batches that are marked PENDING or DONE but are older than the threshold
        /// </summary>
        /// <param name="threshold"></param>
        public void PurgeOldOfflineBatches(DateTime threshold)
        {
            var deleteBatchesQuery = new GetOldOfflineBatchesQuery(ClientDataStore.ClientQueryDb, threshold);
            var deleteBatches = deleteBatchesQuery.ExecuteQuery().Select(x => x.batchnum).ToList();
            LOG.Debug($"Found [{deleteBatches.Count()}] offline batches that had passed the threshold of [{threshold}].");

            foreach (var batchKey in deleteBatches)
            {
                try
                {
                    RemoveClientBatches(batchKey);
                }
                catch (Exception e)
                {
                    LOG.Error(string.Format("Error encountered deleting old offline batch [{0}], message [{1}]", batchKey, e.Message), e);
                }
            }
        }

        public void PurgeOldErroredOutBatches(DateTime threshold)
        {
            var errorBatchesQuery = new GetErroredOutOfflineBatchesQuery(ClientDataStore.ClientQueryDb, threshold);
            var batches = errorBatchesQuery.ExecuteQuery().Select(x => x.batchnum).ToList();
            LOG.Debug($"Found [{batches.Count()}] errored out batches that had passed the threshold of [{threshold}].");

            foreach (var key in batches)
            {
                try
                {
                    RemoveClientBatches(key);
                }
                catch (Exception ex)
                {
                    LOG.Error(string.Format("Error encountered deleting errored out offline batch [{0}]", key), ex);
                }
            }
        }

        public void RemoveClientBatches(int batchKey)
        {
            var savedrptnumberQuery = new GetSavedRptNumberFromBatchNumberQuery(ClientDataStore.ClientQueryDb, batchKey);
            var savedRptNumber = savedrptnumberQuery.ExecuteQuery();

            if (savedRptNumber != null)
            {
                LOG.Debug(string.Format("Removing saved report records for record number/record link [{0}]", savedRptNumber));
                RemoveExistingSavedRpt1ByBatchKey(savedRptNumber);
                RemoveExistingSavedRpt2ByBatchKey(savedRptNumber);
                RemoveExistingSavedRpt3ByBatchKey(savedRptNumber);
            }

            LOG.Debug(string.Format("Removing batch records for batch number [{0}]", batchKey));
            RemoveExistingBatch2ByBatchKey(batchKey);
            RemoveExistingBatchByBatchKey(batchKey);
        }

        private void RemoveExistingBatchByBatchKey(int batchKey)
        {
            var batch = new GetAllBatchesByBatchNumQuery(ClientDataStore.ClientQueryDb, batchKey).ExecuteQuery();
            new RemoveiBBatchCommand(ClientDataStore.ClientCommandDb, batch).ExecuteCommand();
        }

        private void RemoveExistingBatch2ByBatchKey(int batchKey)
        {
            var batch2 = new GetAllBatch2ByBatchNumQuery(ClientDataStore.ClientQueryDb, batchKey).ExecuteQuery();
            new RemoveiBBatch2Command(ClientDataStore.ClientCommandDb, batch2).ExecuteCommand();
        }

        private void RemoveExistingSavedRpt3ByBatchKey(int? batchKey)
        {
            var savedRpt3 = new GetAllSavedReport3ByRecordLinkQuery(ClientDataStore.ClientQueryDb, batchKey).ExecuteQuery();
            new RemoveSavedRpt3Command(ClientDataStore.ClientCommandDb, savedRpt3).ExecuteCommand();
        }

        private void RemoveExistingSavedRpt2ByBatchKey(int? batchKey)
        {
            var savedRpt2 = new GetAllSavedReport2ByRecordLinkQuery(ClientDataStore.ClientQueryDb, batchKey).ExecuteQuery();
            new RemoveSavedRpt2Command(ClientDataStore.ClientCommandDb, savedRpt2).ExecuteCommand();
        }

        private void RemoveExistingSavedRpt1ByBatchKey(int? batchKey)
        {
            var savedRpt1 = new GetAllSavedRpt1ByRecordNumQuery(ClientDataStore.ClientQueryDb, batchKey).ExecuteQuery();
            new RemoveSavedRpt1Command(ClientDataStore.ClientCommandDb, savedRpt1).ExecuteCommand();
        }
    }
}
