using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomCostCenter
{
    public class TopBottomCostCenterData
    {
        public List<string> GetExportFields(string breakNumber)
        {

            //											
            var fieldList = new List<string>();
            //fieldList.Add("break_1");
            if (breakNumber.Equals("Break1"))
                fieldList.Add("GrpCol as break_1");
            else if (breakNumber.Equals("Break2"))
                fieldList.Add("GrpCol as break_2");
            else if (breakNumber.Equals("Break3"))
                fieldList.Add("GrpCol as break_3");

            fieldList.Add("airchg");
            fieldList.Add("lostamt");
            fieldList.Add("numtrips");
            fieldList.Add("nohotel");
            fieldList.Add("stays");
            fieldList.Add("nights");
            fieldList.Add("hotelcost");
            fieldList.Add("rentals");
            fieldList.Add("days");
            fieldList.Add("carcost");
            fieldList.Add("totalcost");

            return fieldList;
        }
    }
}
