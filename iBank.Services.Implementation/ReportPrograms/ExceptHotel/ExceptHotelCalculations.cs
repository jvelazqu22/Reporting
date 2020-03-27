using System.Collections.Generic;

using Domain.Helper;

namespace iBank.Services.Implementation.ReportPrograms.ExceptHotel
{
    public class ExceptHotelCalculations
    {
        public string GetCrystalReportName()
        {
            return "ibExceptHotel";
        }

        public IList<string> GetExportFields(bool accountBreak, UserBreaks userBreaks, string break1Name, string break2Name, string break3Name)
        {
            var fieldList = new List<string>();

            if (accountBreak)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }

            if (userBreaks.UserBreak1) fieldList.Add("break1 as " + break1Name);

            if (userBreaks.UserBreak2) fieldList.Add("break2 as " + break2Name);

            if (userBreaks.UserBreak3) fieldList.Add("break3 as " + break3Name);

            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("reascodh");
            fieldList.Add("reasdesc");
            fieldList.Add("datein");
            fieldList.Add("hotelnam");
            fieldList.Add("hotcity");
            fieldList.Add("hotstate");
            fieldList.Add("nights");
            fieldList.Add("rooms");
            fieldList.Add("roomtype");
            fieldList.Add("bookrate");
            fieldList.Add("hexcprat");

            return fieldList;
        }
    }
}