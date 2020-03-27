using Domain.Interfaces.BroadcastServer;
using Domain.Models.BroadcastServer;
using iBank.BroadcastServer.Utilities;
using System.Collections.Generic;
using System.Net.Mail;
using Domain.Helper;
using Domain.Interfaces;
using Domain.Orm.iBankMastersQueries.BroadcastQueries;
using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

namespace iBank.BroadcastServer.Email
{
    public class BroadcastEmail
    {
        public string HtmlVersion { get; set; }

        public string TextVersion { get; set; }

        public IUserBroadcastSettings BcstSettings { get; set; }

        public bool IsOfflineReport { get; set; }

        public string Agency { get; set; }

        public ibuser User { get; set; }
        public BroadcastServerInformation BcstServerConfig { get; set; }

        public EmailHeaderBuilder HeaderBuilder { get; set; }
        public EmailBodyBuilder BodyBuilder { get; set; }
        public EmailFooterBuilder FooterBuilder { get; set; }

        private readonly EmailConditionals _conditionals = new EmailConditionals();

        public IMasterDataStore MasterDataStore { get; set; }
        public IClientDataStore ClientDataStore { get; set; }
        public IEmailer Emailer { get; set; }
        
        public BroadcastEmail(IBatchManager batchManager, bool isOfflineReport, BroadcastServerInformation bcstServerConfig,
            IMasterDataStore masterDataStore, IClientDataStore clientDataStore, IEmailer emailer)
        {
            BcstSettings = batchManager.UserBroadcastSettings;
            IsOfflineReport = isOfflineReport;
            Agency = batchManager.QueuedRecord.agency;
            User = batchManager.UserManager.User;
            BcstServerConfig = bcstServerConfig;
            MasterDataStore = masterDataStore;
            ClientDataStore = clientDataStore;

            HeaderBuilder = new EmailHeaderBuilder(bcstServerConfig, isOfflineReport, MasterDataStore.MastersQueryDb, MasterDataStore.MastersCommandDb, batchManager.UserBroadcastSettings);
            BodyBuilder = new EmailBodyBuilder(bcstServerConfig, isOfflineReport, MasterDataStore.MastersQueryDb, MasterDataStore.MastersCommandDb, batchManager.UserBroadcastSettings,
                batchManager.UserManager.User, batchManager.QueuedRecord.agency);
            FooterBuilder = new EmailFooterBuilder(batchManager.UserBroadcastSettings, isOfflineReport, batchManager.UserManager.User);
            Emailer = emailer;
        }
        
        public void AssembleEmail(bcstque4 batch, bool allReportsEmpty, bool runSpecial, bool batchOk, IRecordTimingDetails bcstTiming)
        {
            var assembler = new EmailAssembler();
            HtmlVersion = assembler.GetCombinedEmailSections(true, HeaderBuilder.Header, BodyBuilder.Body, FooterBuilder.Footer);
            TextVersion = assembler.GetCombinedEmailSections(false, HeaderBuilder.Header, BodyBuilder.Body, FooterBuilder.Footer);

            var infoRetriever = new BroadcastEmailInformationRetriever();
            var bcstEmailMasterInfoQuery = new GetBroadcastEmailMasterInfoQuery(MasterDataStore.MastersQueryDb, Agency);
            var emailInfo = infoRetriever.GetBroadcastSenderEmailInfo(batch, bcstEmailMasterInfoQuery, BcstServerConfig);

            var agencyLogo = LogoRetriever.GetAgencyLogo(MasterDataStore, ClientDataStore, User.SGroupNbr, Agency, BcstServerConfig.CrystalReportDirectory,
                BcstServerConfig.ReportOutputDirectory);
            HtmlVersion = LogoRetriever.ReplaceHTMLLogoPlaceholder(HtmlVersion, agencyLogo);

            var mailLog = string.Empty;
            var emailCount = 0;
            var emailAddressList = batch.emailaddr.Split(';');
            foreach (var emailAddress in emailAddressList)
            {
                emailCount++;
                assembler.SetEmailInformation(emailInfo, emailAddress, batch, emailCount, BcstSettings, IsOfflineReport, ClientDataStore);

                //user can decide to set option to not send an email if there is no data in the report
                var atLeastOneReportHasData = _conditionals.AtLeastOneReportHasData(batch.nodataoptn, allReportsEmpty);

                mailLog += atLeastOneReportHasData
                               ? SendEmailReturnMessage(batch, emailInfo, emailAddress)
                               : EmailLogText.GetNoDataMessage(emailAddress);

                var logging = new BroadcastReportLogger();
                logging.AddEmailLogRecord(batch, batchOk, bcstTiming, emailAddress, mailLog, runSpecial,
                    Agency, TextVersion, User.UserNumber, ClientDataStore.ClientCommandDb);
            }
        }

        private string SendEmailReturnMessage(bcstque4 broadcast, EmailInformation emailInfo, string emailAddress)
        {
            emailInfo.IsBodyHtml = _conditionals.IsHtmlVersion(broadcast);
            emailInfo.Message = emailInfo.IsBodyHtml ? HtmlVersion : TextVersion;

            if (_conditionals.IsPlainTextWithHtmlAttachment(broadcast))
            {
                var attachment = Attachment.CreateAttachmentFromString(HtmlVersion, "text/html");
                emailInfo.Attachments = new List<Attachment> { attachment };
            }
            
            var success = Emailer.SendEmail(emailInfo);

            return EmailLogText.GetEmailSendMessage(success, emailAddress);
        }
    }
}
