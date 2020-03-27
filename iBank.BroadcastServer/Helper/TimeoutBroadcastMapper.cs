using System;

using iBank.Entities.MasterEntities;

namespace iBank.BroadcastServer.Helper
{
    public class TimeoutBroadcastMapper
    {
        public timeout_broadcasts MapFromQueueRecord(bcstque4 bcst)
        {
            return new timeout_broadcasts
            {
                agency = bcst.agency?.Trim(),
                batchnum = bcst.batchnum ?? 0, 
                batchname = bcst.batchname?.Trim(),
                UserNumber = bcst.UserNumber ?? 0,
                database_name = bcst.dbname?.Trim(),
                nextrun = bcst.nextrun ?? new DateTime(1900, 1, 1),
                time_stamp = DateTime.Now
            };
        }
    }
}
