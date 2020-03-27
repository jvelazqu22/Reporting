using System;
using System.Collections.Generic;
using System.Linq;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomValidatingCarriers
{
    public class DataHelper
    {

        public bool IsDateRangeValid(DateTime? begDate, DateTime? endDate, ReportGlobals globals)
        {
            if (!begDate.HasValue && endDate.HasValue && begDate <= endDate)
            {
                globals.ReportInformation.ReturnCode = 2;
                globals.ReportInformation.ErrorMessage = globals.ReportMessages.RptMsg_DateRange;
                return false;
            }

            return true;
        }

        public List<string> GetExportFields(DataTypes.GroupBy groupBy, bool isCtryCode, bool isValCarr, bool isSecondRange)
        {
            var fieldList = new List<string>();

            if (groupBy == DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER)
            {
                if (isCtryCode) fieldList.Add("ctrycode");
                if (isCtryCode) fieldList.Add("homectry");
            }

            if (isValCarr) fieldList.Add("valcarr");

            fieldList.Add("carrdesc");

            if (groupBy == DataTypes.GroupBy.VALIDATING_CARRIER_HOME_COUNTRY)
            {
                if (isCtryCode) fieldList.Add("ctrycode");
                if (isCtryCode) fieldList.Add("homectry");
            }

            fieldList.Add("trips");
            fieldList.Add("amt");
            fieldList.Add("avgcost");

            if (isSecondRange)
            {
                fieldList.Add("trips2");
                fieldList.Add("amt2");
                fieldList.Add("avgcost2");
            }

            return fieldList;
        }
        

        public DataTypes.GroupBy GetGroupBy(string groupBy)
        {
            if (groupBy == "") return DataTypes.GroupBy.VALIDATING_CARRIER_ONLY;
            var gby = (DataTypes.GroupBy)Convert.ToInt32(groupBy);
            if (gby != DataTypes.GroupBy.VALIDATING_CARRIER_ONLY &&
                gby != DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER &&
                gby != DataTypes.GroupBy.VALIDATING_CARRIER_HOME_COUNTRY)
            {
                gby = DataTypes.GroupBy.VALIDATING_CARRIER_ONLY;
            }
            return gby;
        }

        public DataTypes.Sort GetSort(string sort)
        {
            return (DataTypes.Sort)Convert.ToInt32(sort);
        }

        public string GetExtraWhereText(int mode, string inHomeCtry, string notInCtry)
        {
            string cWhereModeTxt = "";
            string cWhereHomeCtryTxt = "";
            cWhereModeTxt = (mode == 0) ? "" : (mode == 1) ? " Air Only " : " Rail Only ";
            cWhereHomeCtryTxt = (inHomeCtry != "") ? "Home Country in " + inHomeCtry : " ";
            cWhereHomeCtryTxt += (notInCtry != "") ? " not in " + notInCtry : " ";

            return  cWhereModeTxt.TrimStart().Trim() !="" ? cWhereModeTxt.TrimStart().Trim() + " " + cWhereHomeCtryTxt.TrimStart().Trim() : cWhereHomeCtryTxt.TrimStart().Trim();
        }


        public void AdjustDate2Values(bool includeYTDTotals, DateTime? begDate, DateTime? endDate, ref DateTime? dBegDate2, ref DateTime? dEndDate2, string month)
        {
            //REPLACING "GENERIC" PARAMETERS. 
            //_isGeneric1 = Globals.IsParmValueOn(WhereCriteria.CBGENERIC1);
            if (includeYTDTotals && dBegDate2.HasValue && dEndDate2.HasValue)
            {
                int mth = month.MonthNumberFromName();
                mth = (mth == 0) ? 1 : mth;
                dBegDate2 = new DateTime(Convert.ToDateTime(begDate).Year, mth, 1);

                if (dBegDate2 > Convert.ToDateTime(begDate)) dBegDate2 = Convert.ToDateTime(dBegDate2).AddMonths(-12);

                dEndDate2 = Convert.ToDateTime(endDate);
            }
        }
        public void AdjustDate2Values(bool includeYTDTotals, DateTime? begDate, DateTime? endDate, TopBottomValidatingCarriers obj, string month)
        {
            //REPLACING "GENERIC" PARAMETERS. 
            //_isGeneric1 = Globals.IsParmValueOn(WhereCriteria.CBGENERIC1);
            DateTime? dBegDate2 = obj.dBegDate2;
            DateTime? dEndDate2 = obj.dEndDate2;

            if (includeYTDTotals && dBegDate2 == null && dEndDate2 == null)
            {
                int mth = month.MonthNumberFromName();
                mth = (mth == 0) ? 1 : mth;
                dBegDate2 = new DateTime(Convert.ToDateTime(begDate).Year, mth, 1);

                if (dBegDate2 > Convert.ToDateTime(begDate)) dBegDate2 = Convert.ToDateTime(dBegDate2).AddMonths(-12);

                dEndDate2 = Convert.ToDateTime(endDate);
                obj.dBegDate2 = dBegDate2;
                obj.dEndDate2 = dEndDate2;
            }
        }


        public string GetOrderBy(bool bYTDSort, DataTypes.SortBy sortBy)
        {
            string orderBy;
            switch (sortBy)
            {
                case DataTypes.SortBy.VOLUME_BOOKED:
                    orderBy = bYTDSort ? "Amt2" : "Amt";
                    break;
                case DataTypes.SortBy.AVG_COST_PER_TRIP:
                    orderBy = bYTDSort ? "AvgCost2" : "Avgcost";
                    break;
                case DataTypes.SortBy.NO_OF_TRIPS:
                    orderBy = bYTDSort ? "Trips2" : "Trips";
                    break;
                default:
                    orderBy = "Carrdesc";
                    break;
            }
            return orderBy;
        }

        public int HowManyRecords(string howMany, bool isGraphOutput, DataTypes.GroupBy groupBy, DataTypes.SortBy sortBy)
        {
            int records = Convert.ToInt32(howMany);
            if (isGraphOutput) return records;

            if (groupBy == DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER) return 0;

            switch (sortBy)
            {
                case DataTypes.SortBy.VOLUME_BOOKED:
                case DataTypes.SortBy.AVG_COST_PER_TRIP:
                case DataTypes.SortBy.NO_OF_TRIPS:
                    break;
                default:
                    records = 0;
                    break;
            }
            return records;
        }


        public string GetCrystalReportName(DataTypes.GroupBy gby, bool isSecondRange, bool isGraphReportOutput)
        {
            var standardRpt = (isSecondRange) ? "ibTopValCarr2" : "ibTopValCarr1";
            var graphRpt = (isSecondRange) ? "ibGraph2" : "ibGraph1";

            if (isGraphReportOutput) return graphRpt;

            if (gby == DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER) return standardRpt + "B";

            if (gby == DataTypes.GroupBy.VALIDATING_CARRIER_HOME_COUNTRY) return standardRpt + "C";

            return standardRpt;
        }




        public bool IsYTDSort(bool cbUseYTDNbr, bool validYTDOption)
        {
            return cbUseYTDNbr && validYTDOption;
        }

        public bool IsPageBreakHomeCtry(bool cbPgBrkHomeCtry, DataTypes.GroupBy groupBy)
        {
            return cbPgBrkHomeCtry && groupBy == DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER;
        }

        public bool IsGraphReportOutput(string outputType)
        {
            var graphOutputTypes = new List<string> { "4", "6", "8", "RG", "XG" };

            return graphOutputTypes.Any(type => type == outputType);
        }

        public bool IsValCarr(DataTypes.GroupBy gby, bool isGraph)
        {
            return (gby == DataTypes.GroupBy.VALIDATING_CARRIER_ONLY || 
                ((gby == DataTypes.GroupBy.VALIDATING_CARRIER_HOME_COUNTRY || gby == DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER) && !isGraph)) ;
        }

        public bool IsCtryCode(DataTypes.GroupBy gby, bool isGraphOutput)
        {
            return (isGraphOutput && gby == DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER)
                ? true
                : (gby == DataTypes.GroupBy.VALIDATING_CARRIER_HOME_COUNTRY || gby == DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER)
                    ? true
                    : false;
        }
    }
}
