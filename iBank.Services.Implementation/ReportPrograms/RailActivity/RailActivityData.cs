using System;
using System.Collections.Generic;

using Domain.Helper;
using Domain.Models.ReportPrograms.RailActivityReport;

namespace iBank.Services.Implementation.ReportPrograms.RailActivity
{
    public class RailActivityData
    {
        public List<string> GetExportFields(bool acctBrk, UserBreaks userBreaks, bool excludeSvcFee, bool PrePostPreview, UserInformation user)
        {
            var fieldList = new List<string>();
            if (acctBrk)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }
            if (userBreaks.UserBreak1)
            {
                var break1 = string.IsNullOrEmpty(user.Break1Name) ? "break_1" : user.Break1Name;
                fieldList.Add($"break1 as {break1}");
            }
            if (userBreaks.UserBreak2)
            {
                var break2 = string.IsNullOrEmpty(user.Break2Name) ? "break_2" : user.Break2Name;
                fieldList.Add($"break2 as {break2}");
            }
            if (userBreaks.UserBreak3)
            {
                var break3 = string.IsNullOrEmpty(user.Break3Name) ? "break_3" : user.Break3Name;
                fieldList.Add($"break3 as {break3}");
            }

            fieldList.Add("invoice");
            fieldList.Add("recloc");
            fieldList.Add("ticket");
            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            if (PrePostPreview && excludeSvcFee)
            {
                fieldList.Add("pseudocity");
            }
            fieldList.Add("reckey");
            fieldList.Add("cardnum");
            fieldList.Add("seqno");
            fieldList.Add("origin");
            fieldList.Add("orgdesc");
            fieldList.Add("destinat");
            fieldList.Add("destdesc");
            fieldList.Add("rdepdate");
            fieldList.Add("airline");
            fieldList.Add("fltno");
            fieldList.Add("tktdesig");
            fieldList.Add("classcode as class");
            fieldList.Add("airchg");
            if (!excludeSvcFee) fieldList.Add("svcfee");
            fieldList.Add("exchange");
            fieldList.Add("origticket");

            return fieldList;
        }

        public DateTime GetSortDate(FinalData rec, string sortBy)
        {
            switch (sortBy)
            {
                case "2":
                    return rec.InvDate;

                case "3":
                    return rec.BookDate;
                default:
                    return rec.DepDate;
            }
        }
    }
}
