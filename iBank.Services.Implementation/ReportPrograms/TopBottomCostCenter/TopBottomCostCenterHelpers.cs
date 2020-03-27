using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Models.ReportPrograms.TopBottomCostCenter;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomCostCenter
{
    public static class TopBottomCostCenterHelpers
    {
        public static List<FinalData> SortData(List<FinalData> finalDataList, ReportGlobals globals)
        {
            var sortBy = globals.GetParmValue(WhereCriteria.SORTBY);
            var howMany = globals.GetParmValue(WhereCriteria.HOWMANY).TryIntParse(0);
            var descending = !globals.ParmValueEquals(WhereCriteria.RBSORTDESCASC, "2") && !sortBy.Equals("4");

            switch (sortBy)
            {
                case "1":
                    finalDataList = descending
                        ? finalDataList.OrderByDescending(s => s.Totalcost).ToList()
                        : finalDataList.OrderBy(s => s.Totalcost).ToList();
                    break;
                case "2":
                    finalDataList = descending
                        ? finalDataList.OrderByDescending(s => s.Numtrips).ToList()
                        : finalDataList.OrderBy(s => s.Numtrips).ToList();
                    break;
               default:
                    finalDataList = finalDataList.OrderBy(s => s.GrpCol).ToList();
                    howMany = 0;
                    break;
            }

            return howMany > 0
                ? finalDataList.Take(howMany).ToList()
                : finalDataList;

        }

        public static void FixCharges(List<RawData> rawDataList)
        {
            foreach (var row in rawDataList)
            {
                if (row.OffrdChg > 0 && row.AirChg < 0) row.OffrdChg = 0 - row.OffrdChg;

                if (row.OffrdChg == 0 && row.AirChg != 0 || (row.AirChg - row.OffrdChg < 0 && row.Plusmin > 0) || (row.AirChg - row.OffrdChg > 0 && row.Plusmin < 0))
                {
                    row.OffrdChg = row.AirChg;
                }
            }
        }

        public static string GetBreakName(ReportGlobals globals)
        {
            switch (globals.GetParmValue(WhereCriteria.GROUPBY))
            {
                case "2":
                    return globals.User.Break2Name;
                case "3":
                    return globals.User.Break3Name;
                default:
                    return globals.User.Break1Name;
            }
        }

        public static List<FinalData> BuildFinalData(List<FinalData> finalDataList, List<FinalData> carFinal, List<FinalData> hotelFinal)
        {
            var emptyStringFoundInFinalDataList = finalDataList.Where(w => string.IsNullOrWhiteSpace(w.GrpCol)).Any();
            var emptyStringFoundInCarFinalList = carFinal.Where(w => string.IsNullOrWhiteSpace(w.GrpCol)).Any();
            var emptyStringFoundInHotelFinalList = hotelFinal.Where(w => string.IsNullOrWhiteSpace(w.GrpCol)).Any();

            if (!emptyStringFoundInFinalDataList && (emptyStringFoundInCarFinalList || emptyStringFoundInHotelFinalList))
                finalDataList.Add(new FinalData() { GrpCol = string.Empty });

            foreach (var row in finalDataList)
            {
                var car = carFinal.FirstOrDefault(s => s.GrpCol.EqualsIgnoreCase(row.GrpCol));
                if (car != null)
                {
                    row.Rentals = car.Rentals;
                    row.Days = car.Days;
                    row.Carcost = car.Carcost;
                    carFinal.Remove(car);
                }

                var hotel = hotelFinal.FirstOrDefault(s => s.GrpCol.EqualsIgnoreCase(row.GrpCol));
                if (hotel != null)
                {
                    row.Nights = hotel.Nights;
                    row.Stays = hotel.Stays;
                    row.Hotelcost = hotel.Hotelcost;
                    hotelFinal.Remove(hotel);
                }

                if (string.IsNullOrEmpty(row.GrpCol)) row.GrpCol = "[None]";
            }

            //add car and hotel data that had no match
            foreach (var row in carFinal)
            {
                var newRow = new FinalData();
                Mapper.Map(row, newRow);
                finalDataList.Add(newRow);
            }

            foreach (var row in hotelFinal)
            {
                var newRow = new FinalData();
                Mapper.Map(row, newRow);
                finalDataList.Add(newRow);
            }

            return finalDataList;
        }
    }
}
