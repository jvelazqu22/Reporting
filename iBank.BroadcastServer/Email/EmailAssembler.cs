using Domain.Interfaces.BroadcastServer;
using iBank.Server.Utilities;
using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Orm.iBankClientQueries;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

namespace iBank.BroadcastServer.Email
{
    public class EmailAssembler
    {
        public string GetCombinedEmailSections(bool isHtml, IEmailSection header, IEmailSection body, IEmailSection footer)
        {
            return isHtml
                       ? header.Html + body.Html + footer.Html
                       : header.Text + body.Text + footer.Text;
        }

        public void SetEmailInformation(EmailInformation emailInfo, string emailAddress, bcstque4 broadcast, int emailCount, IUserBroadcastSettings broadcastSettings, bool isOfflineReport,
            IClientDataStore clientDataStore)
        {
            emailInfo.RecipientAddress = emailAddress;
            if (!string.IsNullOrEmpty(broadcast.emailsubj.Trim()))
            {
                emailInfo.Subject = broadcast.emailsubj;
            }
            else
            {
                var subjectAccount = string.Empty;
                if (!string.IsNullOrEmpty(broadcast.acctlist.Trim()))
                {
                    var acct = broadcast.acctlist.Left(5).Equals("[NOT]")
                        ? broadcast.acctlist.Replace("[NOT]", "").Split(',').First()
                        : broadcast.acctlist.Split(',').First();

                    var acctNameQuery = new GetMasterAccountNameByAcctAndAgencyQuery(clientDataStore.ClientQueryDb, broadcast.agency, acct);
                    var acctName = acctNameQuery.ExecuteQuery();

                    if (!string.IsNullOrEmpty(acctName)) subjectAccount = " - " + broadcastSettings.GetLanguageTranslation("lt_Account") + ": " + acctName;
                }

                emailInfo.Subject = isOfflineReport
                    ? "iBank" + broadcastSettings.GetLanguageTranslation("xOfflineRpt") + subjectAccount
                    : "iBank" + broadcastSettings.GetLanguageTranslation("xBroadcastRpt") + subjectAccount;
            }

            emailInfo.CCList = SetCCAddresses(emailCount, broadcast);
        }

        private IList<string> SetCCAddresses(int emailCount, bcstque4 batch)
        {
            //only want to cc on the first email so we don't spam anyone
            var isFirstEmail = emailCount == 1;
            return isFirstEmail ? batch.emailccadr.Trim().Split(';') : (IList<string>)new List<string>();
        }
    }
}
