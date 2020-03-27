using System;
using iBank.Entities.MasterEntities;
using iBankDomain.Interfaces;

namespace iBank.BroadcastServer.QueueManager.Factories
{
    public class BroadcastHistoryRecordFactory : IFactory<BroadcastHistory>
    {
        private readonly bcstque4 _broadcast;
        public BroadcastHistoryRecordFactory(bcstque4 broadcast)
        {
            _broadcast = broadcast;
        }

        public BroadcastHistory Build()
        {
            return new BroadcastHistory
            {
                batchname = _broadcast.batchname,
                batchnum = _broadcast.batchnum,
                agency = _broadcast.agency,
                schedule_to_run = _broadcast.nextrun,
                created_on = DateTime.Now
            };
        }
    }
}
