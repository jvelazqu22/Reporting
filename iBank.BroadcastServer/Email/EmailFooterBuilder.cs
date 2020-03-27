using Domain.Interfaces.BroadcastServer;

using iBank.Server.Utilities.Helpers;

using System;
using System.Globalization;

using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;

namespace iBank.BroadcastServer.Email
{
    public class EmailFooterBuilder
    {
        public IEmailSection Footer { get; set; }
        public IUserBroadcastSettings BcstSettings { get; set; }
        public bool IsOfflineReport { get; set; }
        public ibuser User { get; set; }
        
        public EmailFooterBuilder(IUserBroadcastSettings bcstSettings, bool isOfflineReport, ibuser user)
        {
            BcstSettings = bcstSettings;
            IsOfflineReport = isOfflineReport;
            User = user;

            Footer = new WorkingEmailSection
            {
                Html = EmailTemplates.EmailTemplatesHtmlDict[EmailTemplates.EmailTemplateSection.Footer],
                Text = EmailTemplates.EmailTemplatesTextDict[EmailTemplates.EmailTemplateSection.Footer]
            };
        }

        public void BuildFooter(int reportCount, bcstque4 batch)
        {
            Footer.Html = BuildHtmlFooter(Footer.Html, reportCount, batch);

            Footer.Text = BuildTextFooter(Footer.Text, batch);
        }

        private string BuildHtmlFooter(string htmlVersion, int reportCount, bcstque4 batch)
        {
            htmlVersion = htmlVersion.Replace("^plural_s^", reportCount > 1 ? "s" : string.Empty);

            if (!IsOfflineReport && batch.displayuid)
            {
                htmlVersion = htmlVersion.Replace("^rpt_userid^",
                    BcstSettings.GetLanguageTranslation("xBatchCreatedBy") + ": " + User.userid);
            }
            else
            {
                htmlVersion = htmlVersion.Replace("^rpt_userid^", string.Empty);
            }
            
            if (!IsOfflineReport && !string.IsNullOrEmpty(BcstSettings.StyleHtmlFooter))
            {
                htmlVersion = htmlVersion.Replace("^style_html_footer^", BcstSettings.StyleHtmlFooter);
            }
            else
            {
                htmlVersion = htmlVersion.Replace("^style_html_footer^", string.Empty);
            }

            htmlVersion = htmlVersion.Replace("^important_note^", BcstSettings.GetLanguageTranslation("xImportantNote"));
            htmlVersion = htmlVersion.Replace("^important_note_text^", BcstSettings.GetLanguageTranslation("xBcastRptEmailTxt1") + "&nbsp;&nbsp;" + BcstSettings.GetLanguageTranslation("xBcastRptEmailTxt2"));
            htmlVersion = htmlVersion.Replace("^batch_processed^", BcstSettings.GetLanguageTranslation("xBatchProcAt"));
            htmlVersion = htmlVersion.Replace("^rpt_process_time^", DateTime.Now.ToString(@"MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture));
            htmlVersion = htmlVersion.Replace("^local_time_ibank^", BcstSettings.GetLanguageTranslation("xBcastRptEmailTxt4"));

            return htmlVersion;
        }

        private string BuildTextFooter(string textVersion, bcstque4 batch)
        {
            if (!IsOfflineReport && !string.IsNullOrEmpty(BcstSettings.StyleTextFooter))
            {
                textVersion = textVersion.Replace("^style_text_footer^", BcstSettings.StyleTextFooter);
            }
            else
            {
                textVersion = textVersion.Replace("^style_text_footer^", string.Empty);
            }
            textVersion = textVersion.Replace("^important_note_string^", BcstSettings.GetLanguageTranslation("xImportantNote") + ": "
                                                + BcstSettings.GetLanguageTranslation("xBcastRptEmailTxt1") + Environment.NewLine
                                                + BcstSettings.GetLanguageTranslation("xBcastRptEmailTxt2"));
            textVersion = textVersion.Replace("^batch_processed_string^", BcstSettings.GetLanguageTranslation("xBatchProcAt") + ": "
                                                + SharedProcedures.DateToString(DateTime.Now, User.country, User.GblDateFmt, BcstSettings.BroadcastLanguage)
                                                + " (" + BcstSettings.GetLanguageTranslation("xBcastRptEmailTxt4") + ")");
            if (!IsOfflineReport && batch.displayuid)
            {
                
                textVersion = textVersion.Replace("^batch_created_by_user^", BcstSettings.GetLanguageTranslation("xBatchCreatedBy") + ": " + User.userid);
            }
            else
            {
                textVersion = textVersion.Replace("^batch_created_by_user^", string.Empty);
            }

            return textVersion;
        }
    }
}
