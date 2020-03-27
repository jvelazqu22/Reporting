using System.Collections.Generic;
using Domain.Helper;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;

namespace iBank.Services.Implementation.ReportPrograms.WtsFareSavings
{
    public static class WtsFareSavingsHelper
    {
        public static string GetReportName(ReportGlobals globals)
        {
            if (globals.IsParmValueOn(WhereCriteria.CBSUMPAGEONLY)) return "wtsFareSaveSum";

            return globals.IsParmValueOn(WhereCriteria.CBSORTBYSVGSCODE) ? "wtsFareSave2" : "wtsFareSave";
        }

        public static List<string> GetExportFields(UserInformation user)
        {
            var fields = new List<string>();
            if (user.AccountBreak)
            {
                fields.Add("acct");
                fields.Add("acctdesc");
            }
            var userBreaks = SharedProcedures.SetUserBreaks(user.ReportBreaks);

            if (userBreaks.UserBreak1) fields.Add("break1 as " + user.Break1Name);

            if (userBreaks.UserBreak2) fields.Add("break2 as " + user.Break2Name);

            if (userBreaks.UserBreak3) fields.Add("break3 as " + user.Break3Name);

            fields.Add("reckey");
            fields.Add("bookdate");
            fields.Add("invdate");
            fields.Add("depdate");
            fields.Add("passlast");
            fields.Add("passfrst");
            fields.Add("ticket");
            fields.Add("invoice");
            fields.Add("valcarr");
            fields.Add("Itinerary");
            fields.Add("airchg");
            fields.Add("stndchg");
            fields.Add("savings");
            fields.Add("offrdchg");
            fields.Add("lostamt");
            fields.Add("reascode");
            fields.Add("reasdesc");

            return fields;
        }
    }
}
