using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.TripDuration
{
    public class TripDurationCalculations
    {
        public List<string> GetExportFields(bool AccountBreak, bool UserBreak1, bool UserBreak2, bool UserBreak3, string Break1Name, string Break2Name, string Break3Name)
        {
            var fieldList = new List<string>();

            if (AccountBreak)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }
            if (UserBreak1)
            {
                fieldList.Add("break1 as " + Break1Name);
            }
            if (UserBreak2)
            {
                fieldList.Add("break2 as " + Break2Name);
            }
            if (UserBreak3)
            {
                fieldList.Add("break3 as " + Break3Name);
            }

            fieldList.Add("reckey");
            fieldList.Add("invoice");
            fieldList.Add("recloc");
            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("itinerary");
            fieldList.Add("depdate");
            fieldList.Add("arrdate");
            fieldList.Add("days");
            fieldList.Add("airchg");

            return fieldList;
        }
    }
}