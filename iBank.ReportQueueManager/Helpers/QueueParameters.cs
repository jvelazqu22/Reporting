using System.Collections.Concurrent;
using Domain.Helper;
using iBank.Entities.MasterEntities;

namespace iBank.ReportQueueManager.Helpers
{
    public class QueueParameters
    {
        public BlockingCollection<PendingReports> ReportsToValidate { get; set; }

        public bool IsMaintenanceModeRequested { get; set; }
    }
}
