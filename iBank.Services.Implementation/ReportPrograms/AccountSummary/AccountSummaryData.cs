using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.AccountSummary
{
    public class AccountSummaryData
    {
        public List<string> GetExportFields()
        {
            var fieldList = new List<string>();

            fieldList.Add("acct");
            fieldList.Add("acctdesc");
            fieldList.Add("pytrips");
            fieldList.Add("pyamt");
            fieldList.Add("cytrips");
            fieldList.Add("cyamt");
            fieldList.Add("vartrips");
            fieldList.Add("varamt");

            return fieldList;
        }
    }
}
