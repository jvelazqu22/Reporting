using System;
using System.Collections.Generic;

using Domain.Helper;
using Domain.Interfaces;
using Domain.Models.BroadcastServer;
using Domain.Orm.iBankMastersQueries.BroadcastQueries;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

namespace iBank.BroadcastServer.Email
{
    public class AdminEmailer
    {
        public string RecipientEmailAddress { get; set; }

        public IMasterDataStore MasterDataStore { get; set; }
        public IEmailer Emailer { get; set; }

        public AdminEmailer(string recipientEmailAddress, IMasterDataStore masterDataStore, IEmailer emailer)
        {
            RecipientEmailAddress = recipientEmailAddress;
            MasterDataStore = masterDataStore;
            Emailer = emailer;
        }

        public void SendErrorEmail(BroadcastServerInformation config, bcstque4 broadcast, int errorNumber, DateTime broadcastProcessTime)
        {
            var messageBuiler = new AdminEmailErrorMessageBuilder();
            var infoRetriever = new BroadcastEmailInformationRetriever();
            var infoQuery = new GetBroadcastEmailMasterInfoQuery(MasterDataStore.MastersQueryDb, broadcast.agency);

            var message = messageBuiler.BuildEmail(broadcast, errorNumber, broadcastProcessTime);
            var emailInfo = infoRetriever.GetBroadcastSenderEmailInfo(broadcast, infoQuery, config);

            ConstructAndSendErrorEmail(message, emailInfo.SenderAddress, emailInfo.SenderName, broadcast);
        }

        public void SendErrorEmail(BroadcastServerInformation config, bcstque4 broadcast, int errorNumber, DateTime broadcastProcessTime,
                                   BroadcastReportInformation report)
        {
            var messageBuiler = new AdminEmailErrorMessageBuilder();
            var infoRetriever = new BroadcastEmailInformationRetriever();
            var infoQuery = new GetBroadcastEmailMasterInfoQuery(MasterDataStore.MastersQueryDb, broadcast.agency);

            var message = messageBuiler.BuildEmail(broadcast, errorNumber, broadcastProcessTime, report);
            var emailInfo = infoRetriever.GetBroadcastSenderEmailInfo(broadcast, infoQuery, config);
            
            ConstructAndSendErrorEmail(message, emailInfo.SenderAddress, emailInfo.SenderName, broadcast);
        }

        private void ConstructAndSendErrorEmail(string message, string senderEmailAddress, string senderName, bcstque4 broadcast)
        {
            //set up the email information
            var emailInformation = new EmailInformation
            {
                SenderAddress = senderEmailAddress.Trim(),
                SenderName = senderName.Trim(),
                RecipientAddress = RecipientEmailAddress,
                Subject = string.IsNullOrEmpty(broadcast.emailsubj.Trim()) ? "iBank Broadcast Report" : broadcast.emailsubj.Trim(),
                IsBodyHtml = false,
                Message = message,
                CCList = new List<string>(),
                BCCList = new List<string>()
            };

            //send the email
            Emailer.SendEmail(emailInformation);
        }
    }
}
