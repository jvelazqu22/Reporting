using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Models.ReportPrograms.TopBottomTravelerAir;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomTravelerAir
{
    public static class TopTravAirHelpers
    {

        // Sorts processed data by criteria given in the report and returns the list of records to be shown
        public static List<FinalData> SortData(List<FinalData> finalDataList, ReportGlobals globals)
        {

            var sortBy = globals.GetParmValue(WhereCriteria.SORTBY);
            var howMany = globals.GetParmValue(WhereCriteria.HOWMANY).TryIntParse(0);
            var descending = !globals.ParmValueEquals(WhereCriteria.RBSORTDESCASC, "2") && !sortBy.Equals("4");

            switch (sortBy)
            {
                // Volume booked
                case "1":
                    finalDataList = descending
                        ? finalDataList.OrderByDescending(s => s.Amt).ToList()
                        : finalDataList.OrderBy(s => s.Amt).ToList();
                    break;
                // Avg cost per trip
                case "2":
                    finalDataList = descending
                        ? finalDataList.OrderByDescending(s => s.Avgcost).ToList()
                        : finalDataList.OrderBy(s => s.Avgcost).ToList();
                    break;
                // Number of trips
                case "3":
                    finalDataList = descending
                        ? finalDataList.OrderByDescending(s => s.Trips).ToList()
                        : finalDataList.OrderBy(s => s.Trips).ToList();
                    break;
                // Traveler name
                default:
                    finalDataList = descending
                        ? finalDataList.OrderByDescending(s => s.Passlast).ThenByDescending(s => s.Passfrst).ToList()
                        : finalDataList.OrderBy(s => s.Passlast).ThenBy(s => s.Passfrst).ToList();
                    howMany = 0;
                    break;

            }

            // Returns top results to how many are asked in the report
            return howMany > 0
                ? finalDataList.Take(howMany).ToList()
                : finalDataList;

        }

        // Creates a list of strings for the fields in an xls/csv report
        public static List<string> GetExportFields(bool homeCountry, bool lostAmt)
        {
            var fields = new List<string>();
            fields.Add("PassLast");
            fields.Add("PassFrst");
            if (homeCountry)
            {
                fields.Add("HomeCtry");
            }
            fields.Add("Amt");
            fields.Add("Trips");
            if (lostAmt)
            {
                fields.Add("LostAmt");
            }

            fields.Add("TotBkDays");
            fields.Add("AvgCost");
            fields.Add("AvgBkDays");
            return fields;
        }

        

        // Returns Crystal Report to use
        public static string GetCrystalReportName(bool useHomeCountry)
        {
            if (useHomeCountry)
            {
                return "ibTopTravs2";
            }
            else
            {
                return "ibTopTravs";
            }
        }
    }
}
