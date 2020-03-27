using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.ServiceFeeDetailByTrip
{
    public class SvcFeeDetailByTripCalculations
    {
        public string GetCrystalReportName()
        {
            return "ibSvcFeeDetTrip";
        }

        public IList<string> GetExportFields(bool accountBreak)
        {
            var fieldList = new List<string>();

            if (accountBreak)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }
            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("invoice");
            fieldList.Add("invdate");
            fieldList.Add("recloc");
            fieldList.Add("ticket");
            fieldList.Add("depdate");
            fieldList.Add("Itinerary");
            fieldList.Add("Descript");
            fieldList.Add("trandate");
            fieldList.Add("svcfee");

            return fieldList;
        }
    }
}
