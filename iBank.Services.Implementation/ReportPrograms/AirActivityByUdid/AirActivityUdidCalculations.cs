using System.Collections.Generic;

using Domain.Helper;

namespace iBank.Services.Implementation.ReportPrograms.AirActivityByUdid
{
    public class AirActivityUdidCalculations
    {
        public string GetCrystalReportName()
        {
            return "ibAirUdid";
        }

        public IList<string> GetExportFields(bool accountBreak, UserBreaks userBreaks, UserInformation user)
        {
            var fieldList = new List<string>();

            if (accountBreak)
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
            fieldList.Add("udidtext");
            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("ticket");
            fieldList.Add("reckey");
            fieldList.Add("seqno");
            fieldList.Add("origin");
            fieldList.Add("orgdesc");
            fieldList.Add("destinat");
            fieldList.Add("destdesc");
            fieldList.Add("connect");
            fieldList.Add("rdepdate");
            fieldList.Add("airline");
            fieldList.Add("trantype");
            fieldList.Add("fltno");
            fieldList.Add("class");
            fieldList.Add("airchg");

            return fieldList;
        }
    }
}
