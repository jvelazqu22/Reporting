using System;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using Domain.Orm.iBankMastersCommands.Broadcast;
using Domain.Orm.iBankMastersQueries.BroadcastQueries.BroadcastHistoryQueries;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace iBank.BroadcastServer.Helper
{
    public class BroadcastHistoryHandler
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly BroadcastHistory _broadcastHistory;

        public BroadcastHistoryHandler(bcstque4 bcst, IMasterDataStore dbDataStore)
        {
            try
            {
                _broadcastHistory = new GetBroadcastHistoryMissingCreatedOfRunQuery(dbDataStore.MastersQueryDb, bcst).ExecuteQuery();
            }
            catch (Exception e)
            {
                LOG.Error("Attempt to get broadcast history record failed.", e);
            }
        }

        public void UpdateBroadcastHistoryStartRun(ICommandDb mastersCommandDb)
        {
            try
            {
                if (_broadcastHistory == null) return;

                _broadcastHistory.start_of_run = DateTime.Now;
                var updateBroadcastHistoryCommand = new UpdateBroadcastHistoryRecordCommand(mastersCommandDb, _broadcastHistory);
                updateBroadcastHistoryCommand.ExecuteCommand();
            }
            catch (Exception e)
            {
                LOG.Warn("Attempt to update start run broadcast history record failed.", e);
            }
        }

        public void UpdateBroadcastHistoryFinishedRun(ICommandDb mastersCommandDb)
        {
            try
            {
                if (_broadcastHistory == null) return;

                _broadcastHistory.finished_run = DateTime.Now;
                var updateBroadcastHistoryCommand = new UpdateBroadcastHistoryRecordCommand(mastersCommandDb, _broadcastHistory);
                updateBroadcastHistoryCommand.ExecuteCommand();
            }
            catch (Exception e)
            {
                LOG.Warn("Attempt to update finished run broadcast history record failed.", e);
            }
        }

        public void RemoveOlfBroadcastHistoryRecords(IMastersQueryable db, ICommandDb mastersCommandDb)
        {
            try
            {
            }
            catch (Exception e)
            {
                LOG.Debug("Attempt to update broadcast history record failed.", e);
            }
        }

    }
}
