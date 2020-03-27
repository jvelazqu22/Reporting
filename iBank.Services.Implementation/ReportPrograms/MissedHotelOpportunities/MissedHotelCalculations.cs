using Domain.Helper;
using iBank.Server.Utilities.Classes;
using System;
using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.MissedHotelOpportunities
{
    public class MissedHotelCalculations
    {
        public decimal GetTripDuration(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                return 1;
            }

            return endDate.Value.Subtract(startDate.Value).Days;
        }

        public List<string> GetExportFields(bool groupByHomeCountry, ReportGlobals globals, UserBreaks userBreaks)
        {
            var fieldList = new List<string>();

            if (groupByHomeCountry)
                fieldList.Add("homectry");

            if (globals.User.AccountBreak)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }
            if (userBreaks.UserBreak1)
            {
                var break1 = string.IsNullOrEmpty(globals.User.Break1Name) ? "break_1" : globals.User.Break1Name;
                fieldList.Add($"break1 as {break1}");
            }
            if (userBreaks.UserBreak2)
            {
                var break2 = string.IsNullOrEmpty(globals.User.Break2Name) ? "break_2" : globals.User.Break2Name;
                fieldList.Add($"break2 as {break2}");
            }
            if (userBreaks.UserBreak3)
            {
                var break3 = string.IsNullOrEmpty(globals.User.Break3Name) ? "break_3" : globals.User.Break3Name;
                fieldList.Add($"break3 as {break3}");
            }

            fieldList.Add("agentid");
            fieldList.Add("invoice");
            fieldList.Add("invdate");
            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("trantype");
            fieldList.Add("tripstart");
            fieldList.Add("tripend");
            fieldList.Add("tripduratn");
            fieldList.Add("itinerary");
            fieldList.Add("hotelbkd");

            return fieldList;
        }
    }
}
