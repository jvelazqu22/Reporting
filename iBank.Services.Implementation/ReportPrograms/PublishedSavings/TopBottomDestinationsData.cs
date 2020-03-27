using Domain.Helper;
using iBank.Server.Utilities.Classes;
using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.PublishedSavings
{
    // TODO: rename class to be in line with namespace
    public class TopBottomDestinationsData
    {
        public List<string> GetExportFields(ReportGlobals globals, UserBreaks userBreaks)
        {
            var fieldList = new List<string>();

            if (globals.User.AccountBreak)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }

            if (userBreaks.UserBreak1)
            {
                fieldList.Add($"break1 as {globals.User.Break1Name}");
            }
            if (userBreaks.UserBreak2)
            {
                fieldList.Add($"break2 as {globals.User.Break2Name}");
            }
            if (userBreaks.UserBreak3)
            {
                fieldList.Add($"break3 as {globals.User.Break3Name}");
            }

            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("savingcode");
            fieldList.Add("svngdesc");
            fieldList.Add("reckey");
            fieldList.Add("ticket");
            fieldList.Add("seqno");
            fieldList.Add("origin");
            fieldList.Add("orgdesc");
            fieldList.Add("destinat");
            fieldList.Add("destdesc");
            fieldList.Add("airline");
            fieldList.Add("carrdesc");
            fieldList.Add("class");
            fieldList.Add("connect");
            fieldList.Add("airchg");
            fieldList.Add("stndchg");
            fieldList.Add("savings");

            return fieldList;
        }
    }
}
