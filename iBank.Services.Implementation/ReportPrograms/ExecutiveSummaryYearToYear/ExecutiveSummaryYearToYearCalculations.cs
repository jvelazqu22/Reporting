using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryYearToYear
{
    public class ExecutiveSummaryYearToYearCalculations
    {
        public string GetCrystalReportName(string reportOption)
        {
            var reportName = "ibQView3";

            if (!string.IsNullOrEmpty(reportOption) && reportOption == "3") reportName = reportName + "A";

            return reportName;
        }

        public string GetReportOption(ReportGlobals globals)
        {
            return globals.GetParmValue(WhereCriteria.DDRPTOPTION);
        }

        public bool ExcludeServiceFee(bool isReservationReport, ReportGlobals globals)
        {
            return !isReservationReport && globals.IsParmValueOn(WhereCriteria.CBEXCLUDESVCFEES);
        }

        public bool UseServiceFees(bool excludeServiceFees, ReportGlobals globals)
        {
            //TODO: revisit this logic -- doesn't make sense that we would set use service fee to true if exclude service fee is true
            return excludeServiceFees || globals.AgencyInformation.UseServiceFees;
        }

        public bool OrphanServiceFees(bool useServiceFees, ReportGlobals globals)
        {
            var orphanServiceFees = globals.IsParmValueOn(WhereCriteria.CBINCLSVCFEENOMATCH);

            if (!useServiceFees) orphanServiceFees = false;

            return orphanServiceFees;
        }

        public string GetUseDate(ReportGlobals globals)
        {
            return globals.ParmValueEquals(WhereCriteria.DATERANGE, "2") ? "invdate" : "depdate";
        }

        public string GetOrphanServiceFeeWhereText()
        {
            return "Svc Fees Not Matched to Trips Included";
        }

        public string GetFiscalYearWhereText(string fiscalMonth)
        {
            return "FY Start: " + fiscalMonth + "; ";
        }

        public bool IsQuarterToQuarterOption(string reportOption)
        {
            return reportOption.Equals("2");
        }

        public string GetStartMonthName(ReportGlobals globals)
        {
            //TODO: this should return the month name if it is already present as a string (which can happen from the broadcast server) or if it is a number
            var month = globals.GetParmValue(WhereCriteria.STARTMONTH);

            var monthNumber = 0;
            return int.TryParse(month, out monthNumber) ? monthNumber.MonthNameFromNumber() : month;
        }

        public int GetStartYear(ReportGlobals globals)
        {
            return globals.GetParmValue(WhereCriteria.STARTYEAR).TryIntParse(-1);
        }
        
        public string GetFiscalMonthName(ReportGlobals globals)
        {
            return globals.GetParmValue(WhereCriteria.TXTFYSTARTMTH);
        }

        public bool UseMetric(ReportGlobals globals)
        {
            return globals.IsParmValueOn(WhereCriteria.METRIC);
        }

        public string GetWeightMeasurement(bool useMetric)
        {
            return useMetric ? "Kgs" : "Lbs.";
        }

        public string GetDistanceMeasurement(bool useMetric)
        {
            return useMetric ? "Km" : "Mile";
        }
        
        public IList<string> GetExportFields(string monthName, string monthName2, string reportOption, int startYear)
        {
            string col1;
            var col2 = "YTD_" + (startYear - 1);
            string col3;
            var col4 = "YTD_" + startYear;

            switch (reportOption)
            {
                case "3":
                    return new List<string>
                    {
                        "Grp",
                        "Descrip as Descriptn",
                        "colval2 as " + col2,
                        "colval4 as " + col4,
                        "colval6 as YTY_Change"
                    };

                case "2":
                    col1 = monthName.Left(3) + monthName2 + (startYear - 1);
                    col3 = monthName.Left(3) + monthName2 + startYear;
                    return new List<string>
                    {
                        "Grp",
                        "Descrip as Descriptn",
                        "colval1 as " + col1,
                        "colval2 as " + col2,
                        "colval3 as " + col3,
                        "colval4 as " + col4,
                        "colval5 as Qtr_Change",
                        "colval6 as YTY_Change"
                    };

                default:
                    col1 = monthName.Left(3) + "_" + (startYear - 1);
                    col3 = monthName.Left(3) + "_" + startYear;
                    return new List<string>
                    {
                        "Grp",
                        "Descrip as Descriptn",
                        "colval1 as " + col1,
                        "colval2 as " + col2,
                        "colval3 as " + col3,
                        "colval4 as " + col4,
                        "colval5 as Mth_Change",
                        "colval6 as YTY_Change"
                    };

            }
        }

        public int GetStartQuarterMonth(int fiscalYearStartMonth, int startMonth)
        {
            switch (fiscalYearStartMonth)
            {
                case 2:
                case 5:
                case 8:
                case 11:
                    switch (startMonth)
                    {
                        case 2:
                        case 3:
                        case 4:
                            return 2;
                        case 5:
                        case 6:
                        case 7:
                            return 5;
                        case 8:
                        case 9:
                        case 10:
                            return 8;
                        default:
                            return 11;
                    }
                case 3:
                case 6:
                case 9:
                case 12:
                    switch (startMonth)
                    {
                        case 3:
                        case 4:
                        case 5:
                            return 3;
                        case 6:
                        case 7:
                        case 8:
                            return 6;
                        case 9:
                        case 10:
                        case 11:
                            return 9;
                        default:
                            return 12;
                    }
                default:
                    switch (startMonth)
                    {
                        case 1:
                        case 2:
                        case 3:
                            return 1;
                        case 4:
                        case 5:
                        case 6:
                            return 4;
                        case 7:
                        case 8:
                        case 9:
                            return 7;
                        default:
                            return 10;
                    }
            }
        }
    }
}