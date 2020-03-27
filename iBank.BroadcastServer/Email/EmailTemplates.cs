using System;
using System.Collections.Generic;

using Domain.Constants;

namespace iBank.BroadcastServer.Email
{
    public static class EmailTemplates
    {
        public static List<string> EmailTemplatesText { get; set; }
        public static List<string> EmailTemplatesHtml { get; set; }

        public enum EmailTemplateSection
        {
            Header,
            Body,
            Footer
        }

        public static Dictionary<EmailTemplateSection, string> EmailTemplatesTextDict { get; set; }
        public static Dictionary<EmailTemplateSection, string> EmailTemplatesHtmlDict { get; set; }

        static EmailTemplates()
        {
            SetTextTemplates();

            SetHtmlTemplates();
        }

        private static void SetTextTemplates()
        {
            EmailTemplatesTextDict = new Dictionary<EmailTemplateSection, string>();

            var header = $"{EmailPlaceholders.STYLE_TEXT_HEADER}{Environment.NewLine}{EmailPlaceholders.REPORT_GROUP}{Environment.NewLine}";
            var body  = string.Format("{0}{1}{2}{1}{3}{1}{4}{1}", EmailPlaceholders.TEXT_DETAIL_LINE_1, Environment.NewLine, EmailPlaceholders.TEXT_DETAIL_LINE_2,
                    EmailPlaceholders.TEXT_DETAIL_LINE_3, EmailPlaceholders.TEXT_EFECTS_LINE);
            var footer = string.Format("{0}{1}{2}{1}{1}{3}{1}{4}{1}", EmailPlaceholders.STYLE_TEXT_FOOTER, Environment.NewLine, EmailPlaceholders.IMPORTANT_NOTE_STRING,
                    EmailPlaceholders.BATCH_PROCESSED_STRING, EmailPlaceholders.BATCH_CREATED_BY_USER);

            EmailTemplatesTextDict.Add(EmailTemplateSection.Header, header);
            EmailTemplatesTextDict.Add(EmailTemplateSection.Body, body);
            EmailTemplatesTextDict.Add(EmailTemplateSection.Footer, footer);
        }

        private static void SetHtmlTemplates()
        {
            EmailTemplatesHtmlDict = new Dictionary<EmailTemplateSection, string>();

            var header = $@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">
                                    <html>
                                    <head>
                                    <title>Untitled Document</title>
                                    <meta http-equiv=""Content-Type"" content=""text/html; charset=iso-8859-1"">
                                    <link href=""https://www.ibanksystems.com/ibankv4/overpages.css"" rel=""stylesheet"" type=""text/css"">
                                    </head>
                                    <body bgcolor=""#FFFFFF"" text=""#000000"">
                                    <table width=""460"" border=""0"" cellpadding=""0"" cellspacing=""0"">
                                    <tr><td width=""403"" height=""45"" class=""overpagesRule"" align=""left"">
                                    <div align=""left"">{EmailPlaceholders.AGENCY_LOGO}<br>{EmailPlaceholders.TAG_BC_OFFLINE_REPORT}</div></td></tr>
                                    </table>
                                    <br>
                                    <table width=""458"" border=""0"" cellpadding=""1"" cellspacing=""1"">
                                    <tr><td width=""454"" height=""215"" class=""overpagesText"" align=""left"">
                                    <p>{EmailPlaceholders.STYLE_HTML_HEADER}</p>
                                    <p class=""reportGroup"" >{EmailPlaceholders.REPORT_GROUP}</p>";

            var body = @"<p class=""reportItem""><a href=""^rpt_url^"" target=""_new"">^rpt_caption^</a> ^rpt_time_span^
                                    ^rpt_err_line^
                                    </p>";

            var footer = @"^style_html_footer^
                                    <p><font color=""red"">^important_note^: </font>^important_note_text^</p>
                                    <p>^batch_processed^: ^rpt_process_time^ (^local_time_ibank^)<br>
                                    ^rpt_userid^</p>
                                    </td></tr>
                                    <tr><td height=""40"" valign=""bottom"" class=""overpagesRule"">
                                    </td></tr>
                                    </table>
                                    </body></html>";

            EmailTemplatesHtmlDict.Add(EmailTemplateSection.Header, header);
            EmailTemplatesHtmlDict.Add(EmailTemplateSection.Body, body);
            EmailTemplatesHtmlDict.Add(EmailTemplateSection.Footer, footer);
        }
    }
}
