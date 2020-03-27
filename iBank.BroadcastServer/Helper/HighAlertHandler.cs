using System;
using System.Linq;
using System.Reflection;
using System.Text;
using com.ciswired.libraries.CISLogger;
using Domain;
using Domain.Helper;
using Domain.Interfaces;
using Domain.Orm.iBankMastersCommands.Monitoring;
using Domain.Orm.iBankMastersQueries.Monitoring;
using Domain.Orm.iBankMastersQueries.OverdueMonitor;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;

namespace iBank.BroadcastServer.Helper
{
    public class HighAlertHandler
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IEmailer _emailer;

        public HighAlertHandler()
        {
            _emailer = new Emailer();
        }

        public HighAlertHandler(IEmailer emailer)
        {
            _emailer = emailer;
        }

        public void NotificationIfHighAlertAgency(IMasterDataStore store, bcstque4 queueRecord, bool isError)
        {
            try
            {
                var query = new GetAllHighAlertAgenciesQuery(store.MastersQueryDb);
                var highAlertAgencies = query.ExecuteQuery();

                LOG.Debug($"Retrieved [{highAlertAgencies.Count}] total high alert agencies.");
                timeout_broadcasts timeoutBroadcast = null;
                if (!isError)
                {
                    LOG.Debug("Processing timeout/deadlock alert.");
                    //check to see if we have already inserted a record for this
                    timeoutBroadcast = GetTimeoutBroadcast(store, queueRecord);
                    if (timeoutBroadcast != null) return;

                    timeoutBroadcast = new TimeoutBroadcastMapper().MapFromQueueRecord(queueRecord);
                    timeoutBroadcast.notification_sent = true;
                    var cmd = new AddTimeoutBroadcastCommand(store.MastersCommandDb, timeoutBroadcast);
                    cmd.ExecuteCommand();
                }

                var highAlert = highAlertAgencies.FirstOrDefault(x => x.agency.Trim().EqualsIgnoreCase(queueRecord.agency?.Trim()));
                if (highAlert == null) return;

                if(Features.HighAlerts.IsEnabled())
                {
                    if (isError)
                    {
                        LOG.Debug($"Retrieved matching high alert agency. Agency: [{highAlert.agency}] | Contact: [{highAlert.contact}]");
                        var msg = CreateEmailMessage(queueRecord, isError);
                        var emailInfo = CreateEmailInformation(highAlert, msg, isError);
                        _emailer.SendEmail(emailInfo);

                        LOG.Debug("Sent high alert email.");

                        timeoutBroadcast.notification_sent = true;
                        var update = new UpdateTimeoutBroadcastCommand(store.MastersCommandDb, timeoutBroadcast);
                        update.ExecuteCommand();
                    }
                }
                else
                {
                    LOG.Debug($"Retrieved matching high alert agency. Agency: [{highAlert.agency}] | Contact: [{highAlert.contact}]");
                    var msg = CreateEmailMessage(queueRecord, isError);
                    var emailInfo = CreateEmailInformation(highAlert, msg, isError);
                    _emailer.SendEmail(emailInfo);

                    LOG.Debug("Sent high alert email.");
                    if (isError) return;

                    timeoutBroadcast.notification_sent = true;
                    var update = new UpdateTimeoutBroadcastCommand(store.MastersCommandDb, timeoutBroadcast);
                    update.ExecuteCommand();

                }
            }
            catch (Exception e)
            {
                LOG.Error($"Exception encountered while attempting to process for high alert agency. [{e}]", e);
            }
        }

        private timeout_broadcasts GetTimeoutBroadcast(IMasterDataStore store, bcstque4 queueRecord)
        {
            var query = new GetTimeoutBroadcastQuery(store.MastersQueryDb, queueRecord);
            return query.ExecuteQuery();
        }

        private string CreateEmailMessage(bcstque4 queueRecord, bool isError)
        {
            var sb = new StringBuilder();

            sb.Append(isError ? "An error was encountered in the below broadcast." : "An timeout/deadlock was encountered for the below broadcast.");
            sb.Append(Environment.NewLine);

            sb.Append($"Agency: {queueRecord.agency.Trim()}");
            sb.Append(Environment.NewLine);

            sb.Append($"Batch Number: {queueRecord.batchnum}");
            sb.Append(Environment.NewLine);

            sb.Append($"Batch Name: {queueRecord.batchname.Trim()}");
            sb.Append(Environment.NewLine);

            sb.Append($"User Number: {queueRecord.UserNumber}");
            sb.Append(Environment.NewLine);

            sb.Append($"Database: {queueRecord.dbname.Trim()}");
            sb.Append(Environment.NewLine);

            return sb.ToString();
        }

        private EmailInformation CreateEmailInformation(broadcast_high_alert_agency highAlert, string msg, bool isError)
        {
            return new EmailInformation
            {
                RecipientAddress = highAlert.contact,
                SenderAddress = "noreply@ciswired.com",
                Subject = isError ? $"Broadcast Error Alert - Agency {highAlert.agency}" : $"Broadcast Timeout/Deadlock Alert - Agency {highAlert.agency}",
                IsBodyHtml = false,
                Message = msg
            };
        }
    }
}
