using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.ServiceFeeDetailByTransaction
{
    public class SvcFeeDettailByTransaction
    {
        public List<string> GetExportFields(bool AccountBreak)
        {
            var fieldList = new List<string>();

            if (AccountBreak)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }
            fieldList.Add("descript");
            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("trandate");
            fieldList.Add("recloc");
            fieldList.Add("ticket");
            fieldList.Add("invoice");
            fieldList.Add("depdate");
            fieldList.Add("Itinerary");
            fieldList.Add("svcfee");

            return fieldList;
        }

    }
}
