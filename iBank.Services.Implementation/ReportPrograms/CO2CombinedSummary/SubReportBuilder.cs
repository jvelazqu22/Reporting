using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.CO2CombinedSummaryReport;

using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;
using iBank.Services.Implementation.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.CO2CombinedSummary
{
    public static class SubReportBuilder
    {
        public static List<AirSumData> BuildSummaryAir(List<RawData> list)
        {

            var byRecKey = list.GroupBy(s => s.RecKey, (reckey, recs) => recs.FirstOrDefault()).ToList();
            var sumData = new AirSumData
            {
                Invoices = byRecKey.Sum(s => s.Plusmin == 1 ? 1 : 0),
                Credits = byRecKey.Sum(s => s.Plusmin == 1 ? 0 : 1),
                Trips = byRecKey.Sum(s => s.Plusmin),
                Airchg = byRecKey.Sum(s => s.AirChg),
                Miles = list.Sum(s => s.Plusmin * Math.Abs(s.Miles)),
                Airco2 = (int)list.Sum(s => s.AirCo2),
                Altrailco2 = (int)list.Sum(s => s.AltRailCo2),
                Altcarco2 = (int)list.Sum(s => s.AltCarCo2)
            };

            sumData.Avgairchg = sumData.Trips == 0 ? 0 : sumData.Airchg / sumData.Trips;
            sumData.Avgairco2 = sumData.Trips == 0 ? 0 : sumData.Airco2 / sumData.Trips;
            sumData.Costperco2 = sumData.Airco2 == 0 ? 0 : sumData.Airchg / sumData.Airco2;

            return new List<AirSumData> { sumData };
        }

        public static List<CityPairData> BuildCityPair(List<RawData> list, bool combine)
        {
            if (combine)
            {
                foreach (var item in list)
                {
                    var tempOrigin = item.Origin;
                    var tempDest = item.Destinat;

                    if (string.Compare(tempOrigin, tempDest, StringComparison.OrdinalIgnoreCase) > 1)
                    {
                        item.Origin = tempDest;
                        item.Destinat = tempOrigin;
                    }
                }
            }

            var grouped = list.GroupBy(s => new { Origin = s.Origin.Trim(), Destinat = s.Destinat.Trim(), Mode = s.Mode.Trim(), Airline = s.Airline.Trim() }, (key, recs) =>
             {
                 var separator = combine ? "<->" : "-->";

                 var reclist = recs.ToList();
                 var airline = key.Mode.EqualsIgnoreCase("R") ? key.Airline : string.Empty;
                 var orgDesc = AportLookup.LookupAport(new MasterDataStore(), key.Origin, key.Mode, airline);
                 if (orgDesc.Contains(","))
                 {
                     orgDesc = orgDesc.Split(',').FirstOrDefault();
                 }
                 var destDesc = AportLookup.LookupAport(new MasterDataStore(), key.Destinat, key.Mode, airline);
                 if (destDesc.Contains(","))
                 {
                     destDesc = destDesc.Split(',').FirstOrDefault();
                 }
                 return new
                 {
                     key.Origin,
                     OrgDesc = orgDesc,
                     key.Destinat,
                     DestDesc = destDesc,
                     key.Mode,
                     Segments = reclist.Sum(s => s.Plusmin),
                     AirCo2 = reclist.Sum(s => s.Plusmin * Math.Abs(s.AirCo2 + s.MiscAmt)),
                     Miles = reclist.Sum(s => s.Plusmin * Math.Abs(s.Miles)),
                     CityPair = TrimPad(orgDesc, 18) + separator + TrimPad(destDesc, 18),
                     CityPairBig = TrimPad(orgDesc, 40) + separator + TrimPad(destDesc, 40)
                 };

             });

            return grouped.GroupBy(s => new { OrgDesc = s.Origin.Trim() + s.Destinat.Trim(), s.CityPair, s.CityPairBig }, (key, recs) =>
             {
                 var reclist = recs.ToList();
                 return new CityPairData
                 {
                     Citypair = key.CityPair,
                     Ctypairbig = key.CityPairBig,
                     Segments = reclist.Sum(s => s.Segments),
                     Miles = reclist.Sum(s => s.Miles),
                     Airco2 = (int)reclist.Sum(s => s.AirCo2)
                 };
             }).OrderByDescending(s => s.Airco2)
          .ThenByDescending(s => s.Miles)
          .Take(5)
          .ToList();
        }

        private static string TrimPad(string val, int len)
        {
            return val.Trim().Left(len);
        }

        public static List<CarSumData> BuildCarSummary(List<CarRawData> list)
        {
            var carSumData = new CarSumData
            {
                Rentals = list.Sum(s => s.CPlusMin),
                Days = list.Sum(s => s.CPlusMin * s.Days),
                Carcost = list.Sum(s => s.CPlusMin * s.Days * s.Abookrat),
                Carco2 = (int)list.Sum(s => s.CPlusMin * s.CarCo2)
            };
            carSumData.Avgdaycost = carSumData.Days == 0 ? 0 : carSumData.Carcost / carSumData.Days;
            carSumData.Avgdays = carSumData.Rentals == 0 ? 0 : (decimal)carSumData.Days / carSumData.Rentals;

            return new List<CarSumData> { carSumData };
        }

        public static List<TopCarData> BuildTopCar(List<CarRawData> list)
        {
            return list.GroupBy(s => s.Autocity.Trim() + ", " + s.Autostat, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new TopCarData
                {
                    Category = key,
                    Carco2 = reclist.Sum(s => s.CarCo2),
                    Days = reclist.Sum(s => s.Days * s.CPlusMin)
                };
            }).OrderByDescending(s => s.Carco2)
            .ThenByDescending(s => s.Days).Take(5).ToList();
        }

        public static List<HotelSumData> BuildHotelSum(List<HotelRawData> list)
        {
            var hotSumData = new HotelSumData
            {
                Bookings = list.Sum(s => s.HPlusMin),
                Nights = list.Sum(s => s.HPlusMin * s.Nights * s.Rooms),
                Hotelcost = list.Sum(s => s.Nights * s.Rooms * s.Bookrate),
                Hotelco2 = (int)list.Sum(s => s.HotelCo2)
            };
            hotSumData.Costperco2 = hotSumData.Hotelco2 == 0 ? 0 : hotSumData.Hotelcost / hotSumData.Hotelco2;
            hotSumData.Avgntcost = hotSumData.Nights == 0 ? 0 : hotSumData.Hotelcost / hotSumData.Nights;
            hotSumData.Avgnites = hotSumData.Bookings == 0 ? 0 : (decimal)hotSumData.Nights / hotSumData.Bookings;

            return new List<HotelSumData> { hotSumData };
        }

        public static List<TopHotelData> BuildTopHotel(List<HotelRawData> list)
        {

            return list.GroupBy(s => s.Hotcity.Trim() + ", " + s.Hotstate, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new TopHotelData
                {
                    Category = key,
                    Hotelco2 = reclist.Sum(s => s.HotelCo2),
                    Nights = reclist.Sum(s => s.Nights * s.Rooms)
                };
            }).OrderByDescending(s => s.Hotelco2)
            .ThenByDescending(s => s.Nights)
            .Take(5)
            .ToList();
        }

        public static List<AirGraphData> BuildAirCo2(List<RawData> list, DateTime beginDate)
        {
            var results = list.OrderBy(s => s.UseDate).GroupBy(s => new { s.UseDate.GetValueOrDefault().Month, s.UseDate.GetValueOrDefault().Year }, (key, recs) =>
             {
                 var reclist = recs.ToList();
                 return new AirGraphData
                 {
                     MthName = Helpers.GetMonthName(key.Month).Left(3),
                     Airco2 = (int)reclist.Sum(s => s.AirCo2),
                     MonthNum = key.Month,
                     YearNum = key.Year
                 };

             }).ToList();

            return AddEmptyMonths(results, beginDate);

        }
        private static List<T> AddEmptyMonths<T>(List<T> list, DateTime beginDate) where T : IBarGraph, new()
        {
            if (list.Count == 12) return list;
            var newList = new List<T>();

            var month = beginDate.Month;
            var year = beginDate.Year;
            for (int i = 0; i < 12; i++)
            {
                var rec = list.FirstOrDefault(s => s.MonthNum == month && s.YearNum == year);
                if (rec == null)
                {
                    rec = new T
                    {
                        MonthNum = month,
                        YearNum = year,
                        MthName = Helpers.GetMonthName(month).Left(3)
                    };

                }
                newList.Add(rec);
                month++;
                if (month == 13)
                {
                    month = 1;
                    year++;
                }
            }

            return newList;
        }

        public static List<AirGraphData> BuildAltCo2(List<RawData> list, DateTime beginDate)
        {
            var results = list.OrderBy(s => s.UseDate).GroupBy(s => new { s.UseDate.GetValueOrDefault().Month, s.UseDate.GetValueOrDefault().Year }, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new AirGraphData
                {
                    MthName = Helpers.GetMonthName(key.Month).Left(3),
                    Airco2 = (int)reclist.Sum(s => s.AirCo2),
                    Altrailco2 = (int)reclist.Sum(s => s.AltRailCo2),
                    Altcarco2 = (int)reclist.Sum(s => s.AltCarCo2),
                    MonthNum = key.Month,
                    YearNum = key.Year
                };

            }).ToList();

            return AddEmptyMonths(results, beginDate);
        }

        public static List<CarGraphData> BuildCarGraph(List<CarRawData> list, DateTime beginDate)
        {
            var results = list.OrderBy(s => s.UseDate).GroupBy(s => new { s.UseDate.GetValueOrDefault().Month, s.UseDate.GetValueOrDefault().Year }, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new CarGraphData
                {
                    MthName = Helpers.GetMonthName(key.Month).Left(3),
                    Carco2 = (int)reclist.Sum(s => s.CarCo2),
                    MonthNum = key.Month,
                    YearNum = key.Year
                };

            }).ToList();
            return AddEmptyMonths(results, beginDate);
        }

        public static List<HotelGraphData> BuildHotelGraph(List<HotelRawData> list, DateTime beginDate)
        {
            var results = list.OrderBy(s => s.UseDate).GroupBy(s => new { s.UseDate.GetValueOrDefault().Month, s.UseDate.GetValueOrDefault().Year }, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new HotelGraphData
                {
                    MthName = Helpers.GetMonthName(key.Month).Left(3),
                    Hotelco2 = (int)reclist.Sum(s => s.HotelCo2),
                    MonthNum = key.Month,
                    YearNum = key.Year
                };

            }).ToList();
            return AddEmptyMonths(results, beginDate);
        }
    }
}
