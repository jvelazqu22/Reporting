using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.CarActivity
{
    public class CarActivityHelper
    {
        public string GetReportName(string processId)
        {
            switch (processId)
            {
                case "70":
                    return "ibCarCity";
                case "72":
                    return "ibCarAdvRes";
                case "74":
                    return "ibCarVendor";
                default:
                    return "ibCarActivity";
            }
        }

        public IList<string> GetExportFields(string processId)
        {
            switch (processId)
            {
                case "70":
                    return GetExportFieldsForAnalysisByCityReport();
                case "72":
                    return GetExportFieldsForAdvanceReservationsReport();
                case "74":
                    return GetExportFieldsForAnalysisByVendorReport();
                case "76":
                    return GetExportFieldsForCarRentalActivityReport();
                default:
                    return GetExportFieldsForCarRentalActivityReport();
            }
        }

        public IList<string> GetExportFieldsForAnalysisByCityReport()
        {
            return new List<string>
            {
                "autocity",
                "autostat",
                "passlast",
                "passfrst",
                "recloc",
                "rentdate",
                "company",
                "cartype",
                "days",
                "abookrat",
                "ratetype",
            };
        }

        public IList<string> GetExportFieldsForAdvanceReservationsReport()
        {
            return new List<string>
            {
                "recloc",
                "cplusmin",
                "acct",
                "acctdesc",
                "break1",
                "break2",
                "break3",
                "passlast",
                "passfrst",
                "autocity",
                "autostat",
                "rentdate",
                "company",
                "cartype",
                "days",
                "abookrat",
                "milecost",
                "ratetype",
                "weeknum"
            };
        }

        public IList<string> GetExportFieldsForAnalysisByVendorReport()
        {
            return new List<string>
            {
                "company",
                "autocity",
                "autostat",
                "passlast",
                "passfrst",
                "recloc",
                "rentdate",
                "cartype",
                "days",
                "abookrat",
                "ratetype",
            };
        }

        public IList<string> GetExportFieldsForCarRentalActivityReport()
        {
            return new List<string>
            {
                "acct",
                "acctdesc",
                "break1",
                "break2",
                "break3",
                "passlast",
                "passfrst",
                "rentdate",
                "recloc",
                "autocity",
                "autostat",
                "company",
                "cartype",
                "days",
                "abookrat",
                "ratetype",
            };
        }

    }
}
