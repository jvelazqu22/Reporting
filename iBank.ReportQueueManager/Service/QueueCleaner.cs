using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using Domain.Orm.iBankMastersCommands.PendingRecords;
using Domain.Orm.iBankMastersQueries.ReportQueueManager;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

namespace iBank.ReportQueueManager.Service
{
    public class QueueCleaner
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void CleanQueue(DateTime threshold, IMasterDataStore store)
        {
            var getOldQueueRecordsQuery = new GetOldQueueRecordsQuery(store.MastersQueryDb, threshold);
            var recordsToRemove = getOldQueueRecordsQuery.ExecuteQuery().ToList();
            RemoveOldQueueRecords(recordsToRemove, store);
        }

        private void RemoveOldQueueRecords(IList<PendingReports> queueRecords, IMasterDataStore store)
        {
            LOG.Debug($"Start {MethodBase.GetCurrentMethod().Name} ");

            foreach (var rec in queueRecords)
            {
                // remove one record at a time in case one fails because already has been deleted by the report server
                try
                {
                    LOG.Debug($"Record id to delete: {rec.ReportId}");
                    new RemovePendingReportsCommand(store.MastersCommandDb, rec).ExecuteCommand();
                }
                catch (Exception)
                {
                    // Ignore exception to be able to continue to delete the rest of the records that need to be deleted.
                    LOG.Info($"Failed to delete report id: {rec.ReportId}");
                }
            }
        }
    }
}
