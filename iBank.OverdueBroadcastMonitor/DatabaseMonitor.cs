using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.ciswired.libraries.CISLogger;

using Domain.Helper;
using Domain.Helper.OverdueMonitor;
using Domain.Orm.iBankClientCommands.OverdueMonitor;
using Domain.Orm.iBankClientQueries.OverdueMonitor;
using Domain.Orm.iBankMastersQueries.OverdueMonitor;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;

namespace iBank.OverdueBroadcastMonitor
{
    public class DatabaseMonitor
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IMasterDataStore MasterDataStore { get; set; }

        public DatabaseMonitor(IMasterDataStore store)
        {
            MasterDataStore = store;
        }

        public void ProcessDatabase(DatabaseAddress db, DateTime threshold)
        {
            try
            {
                LOG.Debug($"Working database [{db.DatabaseName}]");
                var clientDb = new iBankClientQueryable(db.ServerAddress, db.DatabaseName);

                //find any broadcasts that are over the threshold and are not present in the broadcast queue
                var overdueQuery = new GetAllOverdueBroadcastsQuery(clientDb, threshold);
                var overdueBatches = overdueQuery.ExecuteQuery();
                if (!overdueBatches.Any()) return;

                var filter = new BroadcastFilter();
                overdueBatches = filter.FilterOutInactiveAgencies(overdueBatches, MasterDataStore, db.DatabaseName);
                if (!overdueBatches.Any()) return;
                
                overdueBatches = filter.FilterOutPreviouslyCollectedBroadcasts(overdueBatches, MasterDataStore);
                if (!overdueBatches.Any()) return;

                overdueBatches = filter.FilterOutOfflineBroadcasts(overdueBatches);
                if (!overdueBatches.Any()) return;
                
                //insert the overdue records
                var mapper = new OverdueBroadcastMapper();
                var mappedRecords = mapper.MapToOverdueBroadcasts(overdueBatches, db.DatabaseName);
                InsertOverdueRecords(mappedRecords);

                var highAlertAgencies = GetHighAlertAgencies();
                if (!highAlertAgencies.Any()) return;

                var highAlertHandler = new HighAlertAgencyBroadcastHandler();
                var pairs = highAlertHandler.PairOverdueBroadcastsWithHighAlertAgency(mappedRecords, highAlertAgencies);
                if (!pairs.Any()) return;

                foreach (var pair in pairs.Values.Where(pair => pair.OverdueBroadcasts.Count != 0))
                {
                    ProcessAgency(pair, threshold, MasterDataStore);
                }
            }
            catch (Exception e)
            {
                LOG.Error(e.ToString(), e);
            }
        }

        private void ProcessAgency(HighAlertAgencyBroadcasts highAlert, DateTime threshold, IMasterDataStore store)
        {
            try
            {
                var highAlertHandler = new HighAlertAgencyBroadcastHandler();
                highAlertHandler.SendNotification(highAlert, threshold, store.MastersCommandDb);
            }
            catch (Exception ex)
            {
                LOG.Error(ex.ToString(), ex);
            }
        }

        private void InsertOverdueRecords(IList<overdue_broadcasts> mappedRecords)
        {
            var cmd = new InsertOverdueBroadcastsCommand(MasterDataStore.MastersCommandDb, mappedRecords);
            cmd.ExecuteCommand();
        }

        private IList<broadcast_high_alert_agency> GetHighAlertAgencies()
        {
            var query = new GetAllHighAlertAgenciesQuery(MasterDataStore.MastersQueryDb);
            return query.ExecuteQuery();
        }

        
    }
}
