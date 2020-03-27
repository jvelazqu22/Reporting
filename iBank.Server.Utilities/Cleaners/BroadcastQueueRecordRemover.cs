using com.ciswired.libraries.CISLogger;
using Domain.Interfaces.BroadcastServer;
using iBank.Server.Utilities.Logging;
using System;
using System.Reflection;
using Domain.Orm.iBankMastersCommands.Broadcast;
using iBank.Entities.MasterEntities;
using iBankDomain.RepositoryInterfaces;

namespace iBank.Server.Utilities.Cleaners
{
    public class BroadcastQueueRecordRemover : IBroadcastQueueRecordRemover
    {
        private static readonly IBroadcastLogger LOG = new BroadcastLogger(new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType));

        public void RemoveBroadcastFromQueue(bcstque4 broadcastToRemove, ICommandDb masterCommandDb)
        {
            try
            {
                var removeBcstBatch = new RemoveBatchFromBroadcastQueueCommand(masterCommandDb, broadcastToRemove);
                removeBcstBatch.ExecuteCommand();
            }
            catch (Exception e)
            {
                LOG.WarnLogWithBatchInfo(broadcastToRemove, e.Message, e);
            }
        }
    }
}
