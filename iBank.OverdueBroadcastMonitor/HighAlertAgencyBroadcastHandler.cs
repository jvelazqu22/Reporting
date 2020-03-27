using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper.OverdueMonitor;
using Domain.Orm.iBankClientCommands.OverdueMonitor;

using iBank.Entities.MasterEntities;
using iBank.Server.Utilities;

using iBankDomain.RepositoryInterfaces;

namespace iBank.OverdueBroadcastMonitor
{
    public class HighAlertAgencyBroadcastHandler
    {

        public Dictionary<string, HighAlertAgencyBroadcasts> PairOverdueBroadcastsWithHighAlertAgency(IList<overdue_broadcasts> overdueBroadcasts,
                                                                                            IList<broadcast_high_alert_agency> highAlertAgencies)
        {
            var dict = new Dictionary<string, HighAlertAgencyBroadcasts>();

            foreach (var highAlert in highAlertAgencies)
            {
                if (!dict.ContainsKey(highAlert.agency.Trim()))
                {
                    var alert = new HighAlertAgencyBroadcasts
                    {
                        BroadcastHighAlertAgency = highAlert,
                        OverdueBroadcasts = overdueBroadcasts.Where(x => x.agency.Trim().EqualsIgnoreCase(highAlert.agency.Trim())).ToList()
                    };
                    dict.Add(highAlert.agency.Trim(), alert);
                }
                else
                {
                    dict[highAlert.agency.Trim()].OverdueBroadcasts.ToList().AddRange(overdueBroadcasts.Where(x => x.agency.Trim().EqualsIgnoreCase(highAlert.agency.Trim())).ToList());
                }
            }

            return dict;
        }

        public void SendNotification(HighAlertAgencyBroadcasts highAlert, DateTime threshold, ICommandDb db)
        {
            var notifier = new HighAlertNotifier();
            notifier.SendEmail(highAlert, threshold);

            foreach (var rec in highAlert.OverdueBroadcasts)
            {
                rec.notification_sent = true;
            }

            var cmd = new UpdateOverdueBroadcastsCommand(db, highAlert.OverdueBroadcasts);
            cmd.ExecuteCommand();
        }
    }
}
