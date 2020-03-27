using Domain.Helper;
using Domain.Interfaces.BroadcastServer;
using Domain.Models.BroadcastServer;

using iBank.BroadcastServer.BroadcastReport;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Domain.Constants;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;

using iBankDomain.RepositoryInterfaces;

namespace iBank.BroadcastServer.Email
{
    public class EmailBodyBuilder : AbstractEmailBuilder
    {
        public IEmailSection Body { get; set; }

        public ibuser User { get; set; }
        public string Agency { get; set; }

        public EmailBodyBuilder(BroadcastServerInformation bcstServerConfig, bool isOfflineReport, IMastersQueryable masterQueryDb, ICommandDb masterCommandDb, IUserBroadcastSettings bcstSettings,
            ibuser user, string agency)
            : base(bcstServerConfig, isOfflineReport, masterQueryDb, masterCommandDb, bcstSettings)
        {
            User = user;
            Agency = agency;

            Body = new WorkingEmailSection
            {
                Html = EmailTemplates.EmailTemplatesHtmlDict[EmailTemplates.EmailTemplateSection.Body],
                Text = EmailTemplates.EmailTemplatesTextDict[EmailTemplates.EmailTemplateSection.Body]
            };
        }

        public EmailBodyBuilder(BroadcastEmail bcstEmail) 
            : base(bcstEmail.BcstServerConfig, bcstEmail.IsOfflineReport, bcstEmail.MasterDataStore.MastersQueryDb, bcstEmail.MasterDataStore.MastersCommandDb, bcstEmail.BcstSettings)
        {
            User = bcstEmail.User;
            Agency = bcstEmail.Agency;

            Body = new WorkingEmailSection
            {
                Html = EmailTemplates.EmailTemplatesHtmlDict[EmailTemplates.EmailTemplateSection.Body],
                Text = EmailTemplates.EmailTemplatesTextDict[EmailTemplates.EmailTemplateSection.Body]
            };
        }

        public void CreateBodySection(bcstque4 batch, BroadcastReportInformation report, ReportRunResults results, ReportGlobals globals)
        {
            var emailCaption = report.UserReportName.Trim();
            Body.Html = Body.Html.Replace("^rpt_caption^", emailCaption);

            Body = !IsOfflineReport
                ? BuildNonDelayedReportBody(Body, report, emailCaption)
                : BuildDelayedReportBody(Body, emailCaption);

            Body = string.IsNullOrEmpty(results.ErrorMessage)
                                 ? BuildReportBodyWithNoErrorMsg(Body, batch, results, globals)
                                 : BuildReportBodyWithErrorMsg(Body, batch, results, globals);
        }

        public void BuildBody(List<IEmailSection> workingSections, bcstque4 broadcast)
        {
            var html = new StringBuilder();
            var txt = new StringBuilder();
            foreach (var section in workingSections)
            {
                html.Append(section.Html);
                txt.Append(section.Text);
            }

            Body.Html = string.IsNullOrEmpty(broadcast.emailbody) ? html.ToString() : $"{html.ToString()} <p> {broadcast.emailbody}</p><br/><br/><br/>";
            Body.Text = string.IsNullOrEmpty(broadcast.emailbody) ? txt.ToString() : $"{txt.ToString()} {broadcast.emailbody} \r\r\r";
        }

        private IEmailSection BuildDelayedReportBody(IEmailSection workSection, string emailCaption)
        {
            workSection.Html = workSection.Html.Replace(EmailPlaceholders.RPT_CAPTION, emailCaption);
            workSection.Html = workSection.Html.Replace(EmailPlaceholders.RPT_TIME_SPAN, string.Empty);

            workSection.Text = workSection.Text.Replace(EmailPlaceholders.TEXT_DETAIL_LINE_1, string.Empty);

            return workSection;
        }

        private IEmailSection BuildNonDelayedReportBody(IEmailSection workSection, BroadcastReportInformation report, string emailCaption)
        {
            var dateManager = new EmailDateFormatter(new MasterDataStore());
            var formattedDate = dateManager.FormatDate(report, User, BcstSettings);

            workSection.Html = workSection.Html.Replace("^rpt_time_span^", formattedDate);
            workSection.Text = workSection.Text.Replace(EmailPlaceholders.TEXT_DETAIL_LINE_1, BcstSettings.GetLanguageTranslation("lt_Report") + ": " + emailCaption + " " + formattedDate);

            return workSection;
        }

        private IEmailSection BuildReportBodyWithNoErrorMsg(IEmailSection workSection, bcstque4 batch, ReportRunResults results, ReportGlobals globals)
        {
            //Missing report URL indicates an error
            if (string.IsNullOrEmpty(results.ReportHref))
            {
                workSection.Html = workSection.Html.Replace("^rpt_err_line^", "Report not produced because of an unexpected error. The URL to the report is not specified. Contact your travel agency for assistance.");

                workSection.Html = GetReplacedLinkPlaceHolder(workSection.Html);

                workSection.Text = workSection.Text.Replace(EmailPlaceholders.TEXT_DETAIL_LINE_2, "Report not produced because of an unexpected error.");
                workSection.Text = workSection.Text.Replace(EmailPlaceholders.TEXT_DETAIL_LINE_3, "The URL to the report is not specified. Contact your travel agency for assistance.");
            }
            else
            {
                if (BcstSettings.ViewLogging)
                {
                    var viewLoggingHandler = new ViewLoggingHandler(batch, results, globals, MasterQueryDb, BcstSettings.IsLogInRequired);
                    viewLoggingHandler.AddBroadcastReportInstanceRecord(MasterCommandDb);
                    var rptAccessUrl = viewLoggingHandler.GetBroadcastReportUrl(BcstServerConfig.IbankBaseUrl);

                    viewLoggingHandler.CopyRecordToViewLoggingDirectory(MasterQueryDb, Path.GetFileName(results.ReportHref), globals.ResultsDirectory, globals.Agency);

                    workSection = AddUrlAndDetail(workSection, rptAccessUrl);
                }
                else
                {
                    //* LEGACY - NO LOGGING - INSERT URL TO BCST REPORT FILE
                    //* IF REPORT OUTPUT IS CRYSTAL, WE HAVE A DIFFERENT FORMAT FOR THE REPORT URL
                    if (IsCrystalReport(results.ReportHref))
                    {
                        results.ReportHref = HandleCrystalReportHref(results.ReportHref, globals.OutputType);
                    }

                    workSection = AddUrlAndDetail(workSection, results.ReportHref);
                }
                
                if (batch.outputdest.Equals(BroadcastCriteria.EffectsOutputDest) && globals.EProfileNumber > 0)
                {
                    var eProfileQuery = new GetEProfileByNumberAndAgencyQuery(MasterQueryDb, Agency, globals.EProfileNumber);
                    var eProfile = eProfileQuery.ExecuteQuery();
                    
                    if (string.IsNullOrEmpty(results.eFFECTSDeliveryReturnMessage))
                    {
                        workSection.Html = workSection.Html.Replace("^rpt_err_line^", "<br>" + BcstSettings.GetLanguageTranslation("xBcastRptEmailTxt3"));
                        workSection.Text = workSection.Text.Replace("^text_effects_line^",
                            BcstSettings.GetLanguageTranslation("xBcastRptEmailTxt3") + Environment.NewLine);
                    }
                    else
                    {
                        workSection.Html = workSection.Html.Replace("^rpt_err_line^", "<br>" + results.eFFECTSDeliveryReturnMessage);
                        workSection.Text = workSection.Text.Replace("^text_effects_line^", results.eFFECTSDeliveryReturnMessage + Environment.NewLine);
                    }
                }
            }
            workSection.Html = workSection.Html.Replace("^rpt_err_line^", string.Empty);
            workSection.Text = workSection.Text.Replace("^text_effects_line^", string.Empty);

            return workSection;
        }

        private IEmailSection BuildReportBodyWithErrorMsg(IEmailSection workSection, bcstque4 batch, ReportRunResults results, ReportGlobals globals)
        {
            if (results.ErrorMessage.IndexOf("XML/XSD VALIDATION ERROR", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                BcstSettings.SetLanguageTranslation("xAvailableToView", results.ErrorMessage + " -- View the file at ");
                if (BcstSettings.ViewLogging)
                {
                    var viewLoggingHandler = new ViewLoggingHandler(batch, results, globals, MasterQueryDb, BcstSettings.IsLogInRequired);
                    viewLoggingHandler.AddBroadcastReportInstanceRecord(MasterCommandDb);
                    var rptAccessUrl = viewLoggingHandler.GetBroadcastReportUrl(BcstServerConfig.IbankBaseUrl);

                    viewLoggingHandler.CopyRecordToViewLoggingDirectory(MasterQueryDb, Path.GetFileName(results.ReportHref), globals.ResultsDirectory, globals.Agency);

                    workSection = AddUrlAndDetail(workSection, rptAccessUrl);
                }
                else
                {
                    if (IsCrystalReport(results.ReportHref))
                    {
                        results.ReportHref = HandleCrystalReportHref(results.ReportHref, globals.OutputType);
                    }

                    workSection = AddUrlAndDetail(workSection, results.ReportHref);
                }

                workSection.Text = workSection.Text.Replace("^text_effects_line^", string.Empty);
            }
            else
            {
                workSection.Html = workSection.Html.Replace("^rpt_err_line^", "<br>" + BcstSettings.GetLanguageTranslation("xRptNotProduced") + ". <br>" + BcstSettings.GetLanguageTranslation("xReason") + ": " + results.ErrorMessage);
                workSection.Text = workSection.Text.Replace(EmailPlaceholders.TEXT_DETAIL_LINE_2, BcstSettings.GetLanguageTranslation("xRptNotProduced") + ".");
                workSection.Text = workSection.Text.Replace(EmailPlaceholders.TEXT_DETAIL_LINE_3, BcstSettings.GetLanguageTranslation("xReason") + ": " + results.ErrorMessage);
                workSection.Text = workSection.Text.Replace("^text_effects_line^", string.Empty);

                workSection.Html = GetReplacedLinkPlaceHolder(workSection.Html);
            }

            return workSection;
        }

        private IEmailSection AddUrlAndDetail(IEmailSection workSection, string rptAccessUrl)
        {
            workSection.Html = workSection.Html.Replace("^rpt_url^", rptAccessUrl);
            workSection.Text = workSection.Text.Replace(EmailPlaceholders.TEXT_DETAIL_LINE_2, BcstSettings.GetLanguageTranslation("xAvailableToView"));
            workSection.Text = workSection.Text.Replace(EmailPlaceholders.TEXT_DETAIL_LINE_3, rptAccessUrl);

            return workSection;
        }

        private bool IsCrystalReport(string reportHref)
        {
            return reportHref.Right(4).EqualsIgnoreCase(".rpt");
        }

        private string HandleCrystalReportHref(string reportHref, string outputType)
        {
            reportHref = reportHref.ToLower().Replace(".rpt", string.Empty);
            if (outputType.Equals("1") || outputType.Equals("6"))
            {
                reportHref = reportHref.ToLower().Replace("zxc.", "bcx.");
            }
            else
            {
                reportHref = reportHref.ToLower().Replace("zxc.", "bch.");
            }

            return reportHref;
        }
    }
}
