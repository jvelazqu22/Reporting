using Domain.Helper;
using Domain.Models.ReportPrograms.CCReconReport;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.CCRecon
{
    public class CcReconCalculations
    {
        public string GetCrystalReportName(bool isCreditCardReconcile, bool displayBreaks)
        {
            var reportName = "ibCCRecon";

            if (isCreditCardReconcile) reportName = reportName + "2";

            if (displayBreaks) reportName = reportName + "A";

            return reportName;
        }
        public string GetUdids(int reckey, int number, IList<Udid> udids)
        {
            var udidDesc = string.Empty;
            if(udids != null)
            {
                var udid = udids.FirstOrDefault(s => s.RecKey == reckey && s.UdidNbr == number);
                if (udid != null)
                {
                    udidDesc = udid.UdidText.Trim();
                }
            }

            return udidDesc.PadRight(80);
        }

        public string GetBreakField(string sortBy, FinalData row)
        {
            switch (sortBy)
            {
                case "1":
                case "2":
                case "3":
                    return row.Airlinenbr.PadLeft(3);
                case "5":
                case "6":
                case "11":
                    return row.Trandate.ToShortDateString();
                default:
                    return "A";
            }
        }

        public bool AreMultipleCardsOnReport(string creditCardNumber)
        {
            return !string.IsNullOrEmpty(creditCardNumber) && (creditCardNumber.Contains("%") || creditCardNumber.Contains("_"));
        }

        public bool IsInvoice(ReportGlobals globals)
        {
            var s = globals.GetParmValue(WhereCriteria.INVCRED);

            return s.Length >= 4 && s.Left(4) == "INVO";
        }

        public bool IsCredit(ReportGlobals globals)
        {
            var s = globals.GetParmValue(WhereCriteria.INVCRED);

            return s.Length >= 4 && s.Left(4) == "CRED";
        }

        public bool DisplayBreaks(ReportGlobals globals)
        {
            var useUserBreaks = globals.IsParmValueOn(WhereCriteria.CBBRKBYUSERSETTINGS);

            if (!useUserBreaks)
            {
                return globals.IsParmValueOn(WhereCriteria.CBBRKSPERUSERSETTINGS);
            }

            return false;
        }
    }
}
