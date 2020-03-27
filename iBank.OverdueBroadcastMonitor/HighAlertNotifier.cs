using System;
using System.Text;

using Domain.Helper;
using Domain.Helper.OverdueMonitor;

using iBank.Server.Utilities;

namespace iBank.OverdueBroadcastMonitor
{
    public class HighAlertNotifier
    {
        public void SendEmail(HighAlertAgencyBroadcasts highAlert, DateTime threshold)
        {
            var emailer = new Emailer();
            var msg = CreateEmailMessage(highAlert, threshold);
            var emailInfo = CreateEmailInfo(highAlert, msg);
            emailer.SendEmail(emailInfo);
        }
        
        private EmailInformation CreateEmailInfo(HighAlertAgencyBroadcasts highAlert, string msg)
        {
            return new EmailInformation
            {
                RecipientAddress = highAlert.BroadcastHighAlertAgency.contact,
                SenderAddress = "noreply@ciswired.com",
                Subject = $"Overdue Broadcast Alert - Agency {highAlert.BroadcastHighAlertAgency.agency}",
                IsBodyHtml = false,
                Message = msg
            };
        }

        private string CreateEmailMessage(HighAlertAgencyBroadcasts highAlert, DateTime threshold)
        {
            var sb = new StringBuilder();
            sb.Append($"The following broadcasts for agency {highAlert.BroadcastHighAlertAgency.agency.Trim()} in database {highAlert.OverdueBroadcasts[0].database_name.Trim()} are overdue based on a threshold time of {threshold}");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            foreach (var alert in highAlert.OverdueBroadcasts)
            {
                sb.Append($"Batch Number: {alert.batchnum} | Batch Name: {alert.batchname} | Agency: {alert.agency} | User #: {alert.UserNumber} | Next Run: {alert.nextrun}");
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }
    }
}
