using System.Collections.Generic;

using iBank.Entities.MasterEntities;

namespace Domain.Helper.OverdueMonitor
{
    public class HighAlertAgencyBroadcasts
    {
        public broadcast_high_alert_agency BroadcastHighAlertAgency { get; set; }
        public IList<overdue_broadcasts> OverdueBroadcasts { get; set; }
    }
}
