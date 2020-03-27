using iBank.Server.Utilities.Helpers;
using System.Collections.Generic;
using Domain.Helper;
using System.Linq;
using iBank.Services.Implementation.Shared.SpecifyUdid;

namespace iBank.Services.Implementation.ReportPrograms.CCRecon
{
    public class CCReconHelper
    {
        public IList<string> GetExportFields(bool accountBreak, string break1Name, string break2Name, string break3Name,
            UserBreaks userBreaks, List<int> udidNumbers, List<string> udidLabels, string crystalReportName)
        {
            switch (crystalReportName)
            {
                case "ibCCRecon":
                    return GetExportFieldsForibCCReconReport(accountBreak, break1Name, break2Name, break3Name, userBreaks, udidNumbers, udidLabels);
                case "ibCCReconA":
                    return GetExportFieldsForibCCReconReportA(accountBreak, break1Name, break2Name, break3Name, userBreaks, udidNumbers, udidLabels);
                case "ibCCRecon2":
                    return GetExportFieldsForibCCReconReport2(accountBreak, break1Name, break2Name, break3Name, userBreaks, udidNumbers, udidLabels);
                case "ibCCRecon2A":
                    return GetExportFieldsForibCCReconReport2A(accountBreak, break1Name, break2Name, break3Name, userBreaks, udidNumbers, udidLabels);
                default:
                    return GetExportFieldsForibCCReconReport(accountBreak, break1Name, break2Name, break3Name, userBreaks, udidNumbers, udidLabels);
            }
        }

        public IList<string> GetExportFieldsForibCCReconReport(bool accountBreak, string break1Name, string break2Name, string break3Name,
            UserBreaks userBreaks, List<int> udidNumbers, List<string> udidLabels)
        {
            var fieldList = new List<string>();

            fieldList.Add("cardnum");

            AddAccountAndBreaks(fieldList, accountBreak, userBreaks, break1Name, break2Name, break3Name);

            fieldList.Add("airlinenbr");
            fieldList.Add("ticket");
            fieldList.Add("invoice");
            fieldList.Add("airlndesc");
            fieldList.Add("trantype");
            fieldList.Add("trandate");
            fieldList.Add("depdate");
            fieldList.Add("arrdate");
            fieldList.Add("PassName");
            fieldList.Add("descript");
            fieldList.Add("airchg");

            ExportSpecifyUdidFields.AddUdidFieldList(fieldList, udidNumbers, udidLabels);

            return fieldList;
        }

        private void AddAccountAndBreaks(List<string> fieldList, bool accountBreak, UserBreaks userBreaks, string break1Name, string break2Name, string break3Name)
        {
            if (accountBreak)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }
            if (userBreaks.UserBreak1)
            {
                fieldList.Add("break1 as " + break1Name);
            }
            if (userBreaks.UserBreak2)
            {
                fieldList.Add("break2 as " + break2Name);
            }
            if (userBreaks.UserBreak3)
            {
                fieldList.Add("break3 as " + break3Name);
            }
        }
        
        public IList<string> GetExportFieldsForibCCReconReport2(bool accountBreak, string break1Name, string break2Name, string break3Name,
            UserBreaks userBreaks, List<int> udidNumbers, List<string> udidLabels)
        {
            var fieldList = new List<string>();

            AddAccountAndBreaks(fieldList, accountBreak, userBreaks, break1Name, break2Name, break3Name);

            fieldList.Add("airlinenbr");
            fieldList.Add("ticket");
            fieldList.Add("invoice");
            fieldList.Add("airlndesc");
            fieldList.Add("trantype");
            fieldList.Add("trandate");
            fieldList.Add("depdate");
            fieldList.Add("arrdate");
            fieldList.Add("PassName");
            fieldList.Add("descript");
            fieldList.Add("airchg");

            ExportSpecifyUdidFields.AddUdidFieldList(fieldList, udidNumbers, udidLabels);

            return fieldList;
        }

        public IList<string> GetExportFieldsForibCCReconReportA(bool accountBreak, string break1Name, string break2Name, string break3Name,
            UserBreaks userBreaks, List<int> udidNumbers, List<string> udidLabels)
        {
            var fieldList = new List<string>();
            fieldList.Add("cardnum");

            AddAccountAndBreaks(fieldList, accountBreak, userBreaks, break1Name, break2Name, break3Name);

            fieldList.Add("airlinenbr");
            fieldList.Add("ticket");
            fieldList.Add("invoice");
            fieldList.Add("airlndesc");
            fieldList.Add("trantype");
            fieldList.Add("trandate");
            fieldList.Add("depdate");
            fieldList.Add("arrdate");
            fieldList.Add("PassName");
            fieldList.Add("descript");
            fieldList.Add("airchg");

            ExportSpecifyUdidFields.AddUdidFieldList(fieldList, udidNumbers, udidLabels);

            return fieldList;
        }

        public IList<string> GetExportFieldsForibCCReconReport2A(bool accountBreak, string break1Name, string break2Name, string break3Name,
            UserBreaks userBreaks, List<int> udidNumbers, List<string> udidLabels)
        {
            var fieldList = new List<string>();

            AddAccountAndBreaks(fieldList, accountBreak, userBreaks, break1Name, break2Name, break3Name);

            fieldList.Add("airlinenbr");
            fieldList.Add("ticket");
            fieldList.Add("invoice");
            fieldList.Add("airlndesc");
            fieldList.Add("trantype");
            fieldList.Add("trandate");
            fieldList.Add("depdate");
            fieldList.Add("arrdate");
            fieldList.Add("PassName");
            fieldList.Add("descript");
            fieldList.Add("airchg");

            ExportSpecifyUdidFields.AddUdidFieldList(fieldList, udidNumbers, udidLabels);

            return fieldList;
        }


    }
}
