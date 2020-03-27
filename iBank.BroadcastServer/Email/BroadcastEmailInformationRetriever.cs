using Domain.Helper;
using Domain.Models.BroadcastServer;
using Domain.Orm.Classes;

using iBank.Entities.MasterEntities;

using iBankDomain.RepositoryInterfaces;

namespace iBank.BroadcastServer.Email
{
    public class BroadcastEmailInformationRetriever
    {
        public EmailInformation GetBroadcastSenderEmailInfo(bcstque4 batch, IQuery<BroadcastEmailMasterInfo> getEmailMasterInfoQuery, BroadcastServerInformation bcstServerConfig)
        {
            var bcstEmailMasterInfo = getEmailMasterInfoQuery.ExecuteQuery();

            var senderEmail = GetSenderEmailAddress(batch, bcstEmailMasterInfo, bcstServerConfig);

            var senderName = GetSenderName(batch, bcstEmailMasterInfo, bcstServerConfig);

            return new EmailInformation { SenderAddress = senderEmail, SenderName = senderName };
        }

        private string GetSenderEmailAddress(bcstque4 batch, BroadcastEmailMasterInfo emailMasterInfo, BroadcastServerInformation bcstServerConfig)
        {
            if (!string.IsNullOrEmpty(batch.bcsenderemail.Trim())) return batch.bcsenderemail.Trim();

            if (!string.IsNullOrEmpty(emailMasterInfo.SenderEmail.Trim())) return emailMasterInfo.SenderEmail.Trim();

            return bcstServerConfig.SenderEmailAddress.Trim();
        }

        private string GetSenderName(bcstque4 batch, BroadcastEmailMasterInfo emailMasterInfo, BroadcastServerInformation bcstServerConfig)
        {
            if (!string.IsNullOrEmpty(batch.bcsendername.Trim())) return batch.bcsendername.Trim();

            if (!string.IsNullOrEmpty(emailMasterInfo.SenderName.Trim())) return emailMasterInfo.SenderName.Trim();

            return bcstServerConfig.SenderName.Trim();
        }
    }
}
