using Domain.Interfaces.BroadcastServer;
using Domain.Models.BroadcastServer;

using System;

using Domain.Constants;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace iBank.BroadcastServer.Email
{
    public class EmailHeaderBuilder : AbstractEmailBuilder
    {
        public IEmailSection Header { get; set; }

        public EmailHeaderBuilder(BroadcastServerInformation bcstServerConfig, bool isOfflineReport, IMastersQueryable masterQueryDb, ICommandDb masterCommandDb, IUserBroadcastSettings bcstSettings)
            : base(bcstServerConfig, isOfflineReport, masterQueryDb, masterCommandDb, bcstSettings)
        {
            Header = new WorkingEmailSection
            {
                Html = EmailTemplates.EmailTemplatesHtmlDict[EmailTemplates.EmailTemplateSection.Header],
                Text = EmailTemplates.EmailTemplatesTextDict[EmailTemplates.EmailTemplateSection.Header]
            };
        }

        public void BuildHeader(string batchName, int reportCount)
        {
            Header = IsOfflineReport ? BuildDelayedReportHeader(Header) : BuildNonDelayedReportHeader(Header, batchName);

            var offline = IsOfflineReport ? BcstSettings.GetLanguageTranslation("xOfflineRpt") : BcstSettings.GetLanguageTranslation("xBroadCastRpt");
            Header.Html = Header.Html.Replace(EmailPlaceholders.TAG_BC_OFFLINE_REPORT, offline);
            Header.Html = Header.Html.Replace("^bc_offline^", offline);
            Header.Html = Header.Html.Replace("^plural_s^", reportCount > 1 ? "s" : string.Empty);
            Header.Html = Header.Html.Replace("^is_or_are^", reportCount > 1 ? "are" : "is");
        }

        private IEmailSection BuildDelayedReportHeader(IEmailSection workSections)
        {
            //html version
            workSections.Html = workSections.Html.Replace(EmailPlaceholders.REPORT_GROUP, string.Empty);
            workSections.Html = workSections.Html.Replace(EmailPlaceholders.STYLE_HTML_HEADER, BcstSettings.BroadcastLanguage.Equals("EN") ? "Your iBank Offline Report has processed." : string.Empty);

            //text version
            BcstSettings.StyleTextHeader = BcstSettings.StyleTextHeader.Replace(EmailPlaceholders.TAG_BC_OFFLINE_REPORT, BcstSettings.GetLanguageTranslation("xOfflineRpt"));
            workSections.Text = workSections.Text.Replace(EmailPlaceholders.REPORT_GROUP, string.Empty);
            workSections.Text = BcstSettings.BroadcastLanguage.Equals("EN")
                ? workSections.Text.Replace(EmailPlaceholders.STYLE_TEXT_HEADER, Environment.NewLine + "Your iBank Offline Report has processed.")
                : workSections.Text.Replace(EmailPlaceholders.STYLE_TEXT_HEADER, string.Empty);

            return workSections;
        }

        private IEmailSection BuildNonDelayedReportHeader(IEmailSection workSections, string batchName)
        {
            workSections.Html = workSections.Html.Replace(EmailPlaceholders.STYLE_HTML_HEADER, BcstSettings.StyleHtmlHeader)
                    .Replace(EmailPlaceholders.REPORT_GROUP, BcstSettings.GetLanguageTranslation("xReportGroup") + ": " + batchName);

            BcstSettings.StyleTextHeader = BcstSettings.StyleTextHeader.Replace(EmailPlaceholders.TAG_BC_OFFLINE_REPORT, BcstSettings.GetLanguageTranslation("xBroadCastRpt"));
            workSections.Text = workSections.Text.Replace(EmailPlaceholders.STYLE_TEXT_HEADER, BcstSettings.StyleTextHeader)
                .Replace(EmailPlaceholders.REPORT_GROUP, BcstSettings.GetLanguageTranslation("xReportGroup") + ": " + batchName + Environment.NewLine);

            return workSections;
        }
    }
}
