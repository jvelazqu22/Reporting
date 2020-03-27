using System;

using CODE.Framework.Core.Utilities;

using Domain.Helper;
using Domain.Interfaces.BroadcastServer;
using Domain.Orm.Classes;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Helpers;

namespace iBank.BroadcastServer.Email
{
    public class EmailDateFormatter
    {
        public IMasterDataStore MasterDataStore { get; set; }

        public EmailDateFormatter(IMasterDataStore masterDataStore)
        {
            MasterDataStore = masterDataStore;
        }

        public string FormatDate(BroadcastReportInformation report, ibuser user, IUserBroadcastSettings bcstSettings)
        {
            var dateTemp = "";
            //special cases for various reports
            switch (report.ProcessKey)
            {
                case (int)ReportTitles.iXMLUserDefinedExport:
                    if (report.ReportStart == report.ReportEnd)
                    {
                        if (report.ReportEnd.ToDateTimeSafe().Second == 0)
                        {
                            report.ReportEnd = report.ReportEnd.ToDateTimeSafe().Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                        }
                    }
                    dateTemp = SharedProcedures.DateToString(report.ReportStart, user.country, user.GblDateFmt, bcstSettings.BroadcastLanguage)
                        + " - " + SharedProcedures.DateToString(report.ReportEnd, user.country, user.GblDateFmt, bcstSettings.BroadcastLanguage);
                    break;
                case (int)ReportTitles.TicketTrackerLogReport:
                case (int)ReportTitles.TicketTrackerEmailLog:
                case (int)ReportTitles.TicketTrackerOverview:
                    if (report.DateType == 1 || report.DateType == 9)
                    {
                        dateTemp = "for the event date";
                    }
                    if (report.DateType == 2)
                    {
                        dateTemp = "for the ticket date";
                    }
                    dateTemp += " period " + report.ReportStart.ToShortDateString() + " - " + report.ReportEnd.ToShortDateString();
                    break;
                case (int)ReportTitles.TicketTrackerUnusedTickets:
                case (int)ReportTitles.TicketTrackerCustomerNotifications:
                    dateTemp = report.DateType == 2
                        ? "for tickets expiring in 90 days"
                        : "for all tickets";
                    break;
                case (int)ReportTitles.TicketTrackerDetailReport:
                    if (report.SavedReportNumber == 0)
                    {
                        if (report.DateType == 2)
                        {
                            dateTemp = "for tickets expiring in 90 days";
                        }
                        else
                        {
                            dateTemp = "Open tickets, travel before " +
                                   SharedProcedures.DateToString(DateTime.Now, user.country, user.GblDateFmt, bcstSettings.BroadcastLanguage, true);
                        }

                    }
                    else
                    {
                        var temp = SharedProcedures.DateToString(report.ReportStart, user.country, user.GblDateFmt, bcstSettings.BroadcastLanguage)
                        + " - " + SharedProcedures.DateToString(report.ReportEnd, user.country, user.GblDateFmt, bcstSettings.BroadcastLanguage, true);
                        switch (report.DateType)
                        {
                            case 1:
                                dateTemp = " - Ticket Expiration Dates from " + temp;
                                break;
                            case 2:
                                dateTemp = " - Ticket Dates from " + temp;
                                break;
                            case 4:
                                dateTemp = " - Trip Departure Dates from " + temp;
                                break;
                            case 5:
                                dateTemp = " - Tickets Used from " + temp;
                                break;
                            case 6:
                                dateTemp = " - Ticket End Dates from " + temp;
                                break;
                            case 7:
                                dateTemp = " - Last Update from " + temp;
                                break;
                        }
                    }
                    break;
                case (int)ReportTitles.OneScoreTravelScorecard:
                case (int)ReportTitles.ExpandedOneScoreTravelScorecard:
                    dateTemp = " - Invoice Dates from " + SharedProcedures.DateToString(report.ReportStart, user.country, user.GblDateFmt, bcstSettings.BroadcastLanguage, true)
                        + " - " + SharedProcedures.DateToString(report.ReportEnd, user.country, user.GblDateFmt, bcstSettings.BroadcastLanguage, true);
                    break;
                default:
                    switch (report.DateType)
                    {
                        case 1:
                            dateTemp = " - " + bcstSettings.GetLanguageTranslation("xDateDescTripDep");
                            break;
                        case 2:
                            dateTemp = " - " + bcstSettings.GetLanguageTranslation("xDateDescInv");
                            break;
                        case 3:
                            dateTemp = " - " + bcstSettings.GetLanguageTranslation("xDateDescBooked");
                            break;
                        case 4:
                            dateTemp = " - " + bcstSettings.GetLanguageTranslation("xDateDescDep");
                            break;
                        case 5:
                            dateTemp = " - " + bcstSettings.GetLanguageTranslation("xDateDescArr");
                            break;
                        case 12:
                            dateTemp = " - " + bcstSettings.GetLanguageTranslation("xDateDescStatus");
                            break;
                    }
                    if (report.DateType == 8 && (report.ProcessKey == 142 || report.ProcessKey == 144 || report.ProcessKey == 146))
                    {
                        dateTemp = " - " + bcstSettings.GetLanguageTranslation("DateDescTrans");
                    }
                    var intlSettings = GetIntlSettings(user.country, bcstSettings.BroadcastLanguage);

                    dateTemp = dateTemp.ReplaceFirst("[xxxxx]", GetDateDisplay(report.ReportStart, intlSettings, user.GblDateFmt, bcstSettings.BroadcastLanguage));
                    dateTemp = dateTemp.ReplaceFirst("[xxxxx]", GetDateDisplay(report.ReportEnd, intlSettings, user.GblDateFmt, bcstSettings.BroadcastLanguage));
                    //Language translators used 4 x's instead of 5. 
                    dateTemp = dateTemp.ReplaceFirst("[xxxx]", GetDateDisplay(report.ReportStart, intlSettings, user.GblDateFmt, bcstSettings.BroadcastLanguage));
                    dateTemp = dateTemp.ReplaceFirst("[xxxx]", GetDateDisplay(report.ReportEnd, intlSettings, user.GblDateFmt, bcstSettings.BroadcastLanguage));
                    break;
            }

            return dateTemp;
        }

        private InternationalSettingsInformation GetIntlSettings(string country, string langCode)
        {
            var intlSettingsQuery = new GetSettingsByCountryAndLangCodeQuery(MasterDataStore.MastersQueryDb, country, langCode);
            var intlSettings = intlSettingsQuery.ExecuteQuery();

            return intlSettings;
        }

        private string GetDateDisplay(DateTime dateToConvert, InternationalSettingsInformation intlSettings, bool useGblDateFormat, string broadcastLanguage)
        {
            if (useGblDateFormat)
            {
                return dateToConvert.Day.ToString().PadLeft(2, '0') + SharedProcedures.GetShortMonthMl(dateToConvert.Month, broadcastLanguage)
                       + dateToConvert.Year;
            }

            if (intlSettings == null) return dateToConvert.ToShortDateString();

            //make sure the date display formatting is what is expected by .NET
            var display = intlSettings.DateDisplay.ToLower();

            var temp = display.ToCharArray();
            for (var i = 0; i < temp.Length; i++)
            {
                if (temp[i] == 'm') temp[i] = 'M';
            }
            display = new string(temp);
            display = display.Replace("/", intlSettings.DateMark);

            return dateToConvert.ToString(display);
        }
    }
}
