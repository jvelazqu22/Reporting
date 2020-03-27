using System;

using Domain.Constants;
using Domain.Helper;

using iBank.Entities.MasterEntities;
using iBank.Server.Utilities;

namespace iBank.BroadcastServer.Email
{
    public class AdminEmailErrorMessageBuilder
    {
        public string BuildEmail(bcstque4 broadcast, int errorNumber, DateTime broadcastProcessTime)
        {
            var isOfflineBroadcast = broadcast.batchname.IsOfflineBroadcast();

            var header = BuildHeader(isOfflineBroadcast, broadcast);
            var body = BuildBody(isOfflineBroadcast, errorNumber);
            var footer = BuildFooter(broadcastProcessTime);

            return $"{header}{body}{footer}";
        }

        public string BuildEmail(bcstque4 broadcast, int errorNumber, DateTime broadcastProcessTime, BroadcastReportInformation report)
        {
            var isOfflineBroadcast = broadcast.batchname.IsOfflineBroadcast();

            var header = BuildHeader(isOfflineBroadcast, broadcast);
            var body = BuildBody(report, errorNumber);
            var footer = BuildFooter(broadcastProcessTime);

            return $"{header}{body}{footer}";
        }

        private string BuildHeader(bool isOfflineReport, bcstque4 broadcast)
        {
            var header = EmailTemplates.EmailTemplatesTextDict[EmailTemplates.EmailTemplateSection.Header];
            return isOfflineReport ? BuildHeaderOfflineReportText(header) : BuildHeaderBroadcastBatchText(header, broadcast);
        }

        private string BuildBody(bool isOfflineReport, int errorNumber)
        {
            var body = EmailTemplates.EmailTemplatesTextDict[EmailTemplates.EmailTemplateSection.Body];
            return isOfflineReport ? BuildBodyOfflineReportText(body, errorNumber) : BuildBodyBroadcastBatchText(body, errorNumber);
        }

        private string BuildBody(BroadcastReportInformation report, int errorNumber)
        {
            var body = EmailTemplates.EmailTemplatesTextDict[EmailTemplates.EmailTemplateSection.Body];
            return BuildBodyBroadcastBatchText(body, report, errorNumber);
        }

        private string BuildFooter(DateTime batchProcessTime)
        {
            var footer = EmailTemplates.EmailTemplatesTextDict[EmailTemplates.EmailTemplateSection.Footer];
            return BuildFooterReportText(footer, batchProcessTime);
        }

        private string BuildHeaderOfflineReportText(string template)
        {
            var header = template.Replace(EmailPlaceholders.STYLE_TEXT_HEADER, $"{Environment.NewLine}Your iBank Offline Report has received an error.");

            header = header.Replace(EmailPlaceholders.REPORT_GROUP, "");

            return header;
        }

        private string BuildHeaderBroadcastBatchText(string template, bcstque4 broadcast)
        {
            var header = template.Replace(EmailPlaceholders.STYLE_TEXT_HEADER, $"{Environment.NewLine}Your iBank Broadcast Report has received an error.");

            header = header.Replace(EmailPlaceholders.REPORT_GROUP, $"Report Group: {broadcast.batchname}");

            return header;
        }

        private string BuildBodyOfflineReportText(string template, int errorNumber)
        {
            var body = template.Replace(EmailPlaceholders.TEXT_DETAIL_LINE_1, "");
            body = body.Replace(EmailPlaceholders.TEXT_DETAIL_LINE_2, "Offline report not produced.");
            body = body.Replace(EmailPlaceholders.TEXT_DETAIL_LINE_3, $"Reason: An error was received that requires your attention. Error Number: [{errorNumber}]");
            body = body.Replace(EmailPlaceholders.TEXT_EFECTS_LINE, "");

            return body;
        }

        private string BuildBodyBroadcastBatchText(string template, int errorNumber)
        {
            var body = template.Replace(EmailPlaceholders.TEXT_DETAIL_LINE_1, "");
            body = body.Replace(EmailPlaceholders.TEXT_DETAIL_LINE_2, "Broadcast not produced.");
            body = body.Replace(EmailPlaceholders.TEXT_DETAIL_LINE_3, $"Reason: An error was received that requires your attention. Error Number: [{errorNumber}]");
            body = body.Replace(EmailPlaceholders.TEXT_EFECTS_LINE, "");

            return body;
        }

        private string BuildBodyBroadcastBatchText(string template, BroadcastReportInformation report, int errorNumber)
        {
            var body = template.Replace(EmailPlaceholders.TEXT_DETAIL_LINE_1, "");
            body = body.Replace(EmailPlaceholders.TEXT_DETAIL_LINE_2, $"Report: {report.UserReportName} {report.ReportStart} - {report.ReportEnd} Report not produced.");
            body = body.Replace(EmailPlaceholders.TEXT_DETAIL_LINE_3, $"Reason: An error was received that requires your attention. Error Number: [{errorNumber}]");
            body = body.Replace(EmailPlaceholders.TEXT_EFECTS_LINE, "");

            return body;
        }

        private string BuildFooterReportText(string template, DateTime batchProcessTime)
        {
            var footer = template.Replace(EmailPlaceholders.STYLE_TEXT_FOOTER, "");
            footer = footer.Replace(EmailPlaceholders.IMPORTANT_NOTE_STRING,"");
            footer = footer.Replace(EmailPlaceholders.BATCH_PROCESSED_STRING, $"Batch processed at: {batchProcessTime.FormatWholeDateWithAmPm()} (local time at iBank server)");
            footer = footer.Replace(EmailPlaceholders.BATCH_CREATED_BY_USER, "");

            return footer;
        }
    }
}
