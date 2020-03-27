using System.Collections.Generic;
using System.Linq;
using iBank.Entities.ClientEntities;

namespace iBank.BroadcastServer.QueueManager.BuildQueue
{
    public class DuplicateRemoverHandler
    {
        public void RemoveDuplicates(List<ibbatch> batches)
        {
            var tempList = batches.ToList();
            foreach (var batch in tempList)
            {
                var list = batches
                    .Where(w => w.batchnum != 0)
                    .Where(w => w.batchnum == batch.batchnum)
                    .Where(w => w.agency.Equals(batch.agency))
                    .Where(w => w.batchname.Equals(batch.batchname))
                    .Where(w => w.UserNumber == batch.UserNumber).ToList();

                if (list.Count > 1)
                {
                    var tempBatch = list.First();
                    list.ForEach(r => batches.Remove(r));
                    batches.Add(tempBatch);
                }
            }
        }
    }
}
