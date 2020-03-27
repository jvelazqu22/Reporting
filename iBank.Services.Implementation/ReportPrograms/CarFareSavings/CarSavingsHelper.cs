using Domain.Helper;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Utilities;
using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.CarFareSavings
{
    public class CarSavingsHelper
    {
        public List<string> GetExportFields(bool acctBrk, UserBreaks userBreaks, ReportGlobals Globals, bool excludeSavings, bool _homeCountryBreak)
        {
            var fieldList = new List<string>();

            if (acctBrk)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }
            if (userBreaks.UserBreak1) fieldList.Add("break1 as " + Globals.User.Break1Name);
            if (userBreaks.UserBreak2) fieldList.Add("break2 as " + Globals.User.Break2Name);
            if (userBreaks.UserBreak3) fieldList.Add("break3 as " + Globals.User.Break3Name);
            if (_homeCountryBreak) fieldList.Add("HomeCtry");
            fieldList.Add("confirmNo");
            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("reckey");
            fieldList.Add("invdate");
            fieldList.Add("rentdate");
            fieldList.Add("company");
            fieldList.Add("autocity");
            fieldList.Add("autostat");
            fieldList.Add("days");
            fieldList.Add("cartype");
            if (!excludeSavings) fieldList.Add("carStdRate");
            fieldList.Add("aExcpRat");
            fieldList.Add("aBookRat");
            if (!excludeSavings)
            {
                fieldList.Add("carSvgCode");
                fieldList.Add("savings");
            }
            fieldList.Add("reascoda");
            fieldList.Add("lostamt");
            if (_homeCountryBreak) fieldList.Add("LostPct");

            return fieldList;
        }
    }
}
