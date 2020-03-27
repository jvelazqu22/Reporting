using System.Collections.Generic;

using Domain.Helper;

namespace iBank.Services.Implementation.ReportPrograms.ExceptAir
{
    public class ExceptAirCalculations
    {
        public string GetCrystalReportName()
        {
            return "ibExceptAir";
        }

        public IList<string> GetExportFields(bool acctBrk, UserBreaks userBreaks, string break1Name, string break2Name, string break3Name, bool useBaseFare)
        {
            var fieldList = new List<string>();

            if (acctBrk)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }

            if (userBreaks.UserBreak1) fieldList.Add("break1 as " + break1Name);

            if (userBreaks.UserBreak2) fieldList.Add("break2 as " + break2Name);

            if (userBreaks.UserBreak3) fieldList.Add("break3 as " + break3Name);

            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("reascode");
            fieldList.Add("reasdesc");
            fieldList.Add("reckey");
            fieldList.Add("ticket");
            fieldList.Add("rdepdate");
            fieldList.Add("seqno");
            fieldList.Add("origin");
            fieldList.Add("orgdesc");
            fieldList.Add("destinat");
            fieldList.Add("destdesc");
            fieldList.Add("airline");
            fieldList.Add("carrdesc");
            fieldList.Add("class");
            fieldList.Add("connect");

            fieldList.Add("ColHead1");

            fieldList.Add(useBaseFare ? "airchg as baseFare" : "airchg");

            fieldList.Add("offrdchg");
            fieldList.Add("lostamt");


            return fieldList;
        }

        public string GetColumnOneHeader(bool useBaseFare)
        {
            return useBaseFare ? "Base Fare" : "Amt Paid";
        }

        public IList<string> GetZeroFields(bool useBaseFare)
        {
            return new List<string> { useBaseFare ? "basefare" : "airchg", "offrdchg", "lostamt" };
        }
    }
}
