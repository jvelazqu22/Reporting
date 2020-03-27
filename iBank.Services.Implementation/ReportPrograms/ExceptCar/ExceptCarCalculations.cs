using System.Collections.Generic;

using Domain.Helper;

namespace iBank.Services.Implementation.ReportPrograms.ExceptCar
{
    public class ExceptCarCalculations
    {
        public string GetCrystalReportName()
        {
            return "ibExceptCar";
        }

        public IList<string> GetExportFields(bool acctBreak, UserBreaks userBreaks)
        {
            var fieldList = new List<string>();

            if (acctBreak)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }

            if (userBreaks.UserBreak1) fieldList.Add("break1");

            if (userBreaks.UserBreak2) fieldList.Add("break2");

            if (userBreaks.UserBreak3) fieldList.Add("break3");

            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("reascoda");
            fieldList.Add("reasdesc");
            fieldList.Add("rentdate");
            fieldList.Add("company");
            fieldList.Add("autocity");
            fieldList.Add("autostat");
            fieldList.Add("days");
            fieldList.Add("cartype");
            fieldList.Add("ctypedesc");
            fieldList.Add("abookrat");
            fieldList.Add("aexcprat");

            return fieldList;
        }
    }
}
