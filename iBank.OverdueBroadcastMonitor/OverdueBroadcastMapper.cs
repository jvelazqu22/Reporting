using System;
using System.Collections.Generic;

using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;

namespace iBank.OverdueBroadcastMonitor
{
    public class OverdueBroadcastMapper
    {
        public IList<overdue_broadcasts> MapToOverdueBroadcasts(IList<ibbatch> batchRecords, string databaseName)
        {
            var overdueBroadcasts = new List<overdue_broadcasts>();
            foreach (var batch in batchRecords)
            {
                overdueBroadcasts.Add(new overdue_broadcasts
                                          {
                                                agency = batch.agency?.Trim(),
                                                batchname = batch.batchname?.Trim(),
                                                batchnum = batch.batchnum,
                                                database_name = databaseName?.Trim(),
                                                UserNumber = batch.UserNumber,
                                                nextrun = batch.nextrun,
                                                time_stamp = DateTime.Now
                                          });
            }

            return overdueBroadcasts;
        }
    }
}
