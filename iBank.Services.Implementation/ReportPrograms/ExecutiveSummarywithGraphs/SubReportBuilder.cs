using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.ExecutiveSummarywithGraphsReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.ExecutiveSummarywithGraphs
{
    public static class SubReportBuilder
    {
        public static List<AirSumData> BuildSummaryAir(IEnumerable<RawData> list,decimal svcFee, bool rail, int miles, decimal airCo2)
        {
            var tempList = rail
                ? list.Where(s => s.Valcarr.Trim().Length != 4 && !s.ValcarMode.EqualsIgnoreCase("R")).ToList()
                : list.ToList();

            var sumData = new AirSumData
            {
                Invoices = tempList.Sum(s => s.Plusmin == 1 ? 1 : 0),
                Credits = tempList.Sum(s => s.Plusmin == 1 ? 0 : 1),
                Trips = tempList.Sum(s => s.Plusmin),
                Airchg = tempList.Sum(s =>s.Airchg),
                Savings = tempList.Sum(s => s.Savings),
                Airexcepts = tempList.Sum(s => s.Offrdchg != 0 && s.LostAmt != 0?s.Plusmin:0),
                Lostamt = tempList.Sum(s => s.LostAmt),
                Negosvngs = tempList.Sum(s => s.NegoSvngs),
                Airco2 = airCo2,
                Miles = miles,
                Svcfee = svcFee
            };
            sumData.Avgairchg = sumData.Trips == 0 ? 0 : sumData.Airchg / sumData.Trips;
            sumData.Avgsvgs = sumData.Trips == 0 ? 0 : sumData.Savings / sumData.Trips;
            sumData.Avglost = sumData.Trips == 0 ? 0 : sumData.Lostamt / sumData.Trips;

            return new List<AirSumData> { sumData };
        }

        public static List<RailSumData> BuildSummaryRail(IEnumerable<RawData> list)
        {
            var railList = list.Where(s => s.ValcarMode.EqualsIgnoreCase("R")).ToList();
            var sumData = new RailSumData
            {
                Invoices = railList.Sum(s => s.Plusmin == 1 ? 1 : 0),
                Credits = railList.Sum(s => s.Plusmin == 1 ? 0 : 1),
                Trips = railList.Sum(s => s.Plusmin),
                RailChg = railList.Sum(s => s.Airchg)
            };
           
            return new List<RailSumData> { sumData };
        }

        public static List<TopCityData> BuildTopCity(List<CityPairRawData> list, bool combine, bool rail)
        {
            if (combine)
            {
                foreach (var item in list)
                {
                    var tempOrigin = item.Origin;
                    var tempDest = item.Destinat;

                    if (string.Compare(tempOrigin, tempDest, StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        item.Origin = tempDest;
                        item.Destinat = tempOrigin;
                    }
                }
            }

            var grouped = list.GroupBy(s => new {s.Origin, s.Destinat, s.Mode, s.Airline}, (key, recs) =>
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
                    Cost = reclist.Sum(s => s.Plusmin * Math.Abs(s.ActFare + s.Miscamt)),
                    Miles = reclist.Sum(s => s.Plusmin * Math.Abs(s.Miles)), 
                    CityPair = TrimPad(orgDesc,18) + separator + TrimPad(destDesc,18)
                };

            });

            return grouped.GroupBy(s => new { OrgDesc = s.Origin.Trim() + s.Destinat.Trim(),s.CityPair}, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new TopCityData
                {
                    Citypair = key.CityPair,
                    Cost = reclist.Sum(s => s.Cost),
                    Segments = reclist.Sum(s => s.Segments)
                };
            }).OrderByDescending(s => s.Segments)
          .ThenByDescending(s => s.Cost)
          .Take(5)
          .ToList();
        }

        private static string TrimPad(string val, int len)
        {
            return val.Trim().PadRight(len).Left(len).Trim();
        }

        public static List<CarSumData> BuildCarSum(List<CarRawData> list)
        {
            var carSumData = new CarSumData
            {
                Numcars = list.Sum(s => s.Cplusmin),
                Numdays = list.Sum(s => s.Cplusmin*s.Days),
                Carchg = list.Sum(s => s.Days * s.Abookrat)
            };
            
            var bookRateCount = list.Sum(s => s.Abookrat == 0 ? 0 : s.Cplusmin);
            var bookRateNights = list.Sum(s => s.Abookrat == 0 ? 0 : s.Cplusmin * s.Days);

            var sumBookRate = list.Sum(s => s.Abookrat);
            carSumData.Avgrate = bookRateCount == 0 ? 0 : sumBookRate / bookRateCount;

            carSumData.Avgdaycost = bookRateNights == 0 ? 0m : carSumData.Carchg / bookRateNights;
            carSumData.Avgnumdays = carSumData.Numcars == 0 ? 0 : carSumData.Numdays / carSumData.Numcars;

            return new List<CarSumData> {carSumData};
        }

        public static List<TopCarData> BuildTopCar(List<CarRawData> list)
        {
            return list.GroupBy(s => s.Autocity.Trim() + ", " + s.Autostat, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new TopCarData
                {
                    Carcity = key,
                    Carcost = reclist.Sum(s => s.Cplusmin == 0?0: s.Abookrat * s.Days),
                    Numdays = reclist.Sum(s => s.Days)
                };
            }).OrderByDescending(s => s.Numdays)
            .ThenByDescending(s => s.Carcost).Take(5).ToList();
        }

        public static List<HotSumData> BuildHotelSum(List<HotelRawData> list)
        {
            var hotSumData = new HotSumData();
            hotSumData.Numhotels = list.Sum(s => s.Hplusmin);
            hotSumData.Numnits = list.Sum(s => s.Hplusmin * s.Nights * s.Rooms);
            hotSumData.Roomchg = list.Sum(s => s.Nights * s.Rooms * s.Bookrate);

            var temp = list.Sum(s => s.Bookrate == 0 ? 0 : s.Hplusmin);
            var sumBookRate = list.Sum(s => s.Bookrate);
            hotSumData.Avgrate = temp == 0 ? 0 : sumBookRate / temp;
            hotSumData.Avgnitcost = hotSumData.Numnits == 0 ? 0 : hotSumData.Roomchg / hotSumData.Numnits;
            hotSumData.Avgnumnits = hotSumData.Numhotels == 0 ? 0 : hotSumData.Numnits / hotSumData.Numhotels;

            return new List<HotSumData> { hotSumData };
        }

        public static List<TopHotData> BuildTopHotel(List<HotelRawData> list)
        {

            return list.GroupBy(s => s.Hotcity.Trim() + ", " + s.Hotstate, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new TopHotData
                {
                    Hotcity = key,
                    Hotcost = reclist.Sum(s => s.Hplusmin == 0 ? 0 : s.Bookrate * s.Nights * s.Rooms),
                    Numnits = reclist.Sum(s => s.Nights * s.Rooms)
                };
            }).OrderByDescending(s => s.Numnits)
            .ThenByDescending(s => s.Hotcost)
            .Take(5)
            .ToList();
        }

        public static List<PieData> BuildAirPie(IMasterDataStore store, List<RawData> list, string dataToShow, bool railData, string title, int howMany = 5)
        {

            var temp = list.Where(s => !"$$,ZZ,XD".Contains(s.Valcarr) && !string.IsNullOrEmpty(s.Valcarr))
                .Select(s => new
            {
                s.Valcarr,
                CarrDesc =
                    LookupFunctions.LookupAline(store, s.Valcarr, s.ValcarMode),
                s.Airchg,
                s.Plusmin
            }).GroupBy(s => new {s.Valcarr, s.CarrDesc}, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new 
                {
                    key.Valcarr,
                    key.CarrDesc,
                    AirChg = reclist.Sum(s => s.Airchg),
                    transacts = reclist.Sum(s => s.Plusmin)
                };
            });

            var total = !dataToShow.Equals("2") ? list.Sum(s => s.Airchg) : list.Sum(s => s.Plusmin);
            var airPieData = temp.GroupBy(s =>s.CarrDesc, (key, recs) =>
                                    {
                                        decimal byCarr = !dataToShow.Equals("2") ? recs.Sum(s => s.AirChg) : recs.Sum(s => s.transacts);

                                        return new PieData
                                        {
                                            Catdesc = key,
                                            Data1 =  (list.Count == 0 || total == 0) ? 0 : byCarr / total * 100,
                                            Pietitle = title
                                        };

                                    }).OrderByDescending(s => s.Data1).ToList();
            return TrimPieData(airPieData,howMany,title);
        }

        private static List<PieData> TrimPieData(List<PieData> pieData, int howMany, string title)
        {
            if (pieData.Count > howMany)
            {
                var keepers = pieData.Take(5).ToList();
                pieData.RemoveRange(0, 5);

                keepers.Add(new PieData
                {
                    Catdesc = "Other",
                    Data1 = pieData.Sum(s => s.Data1),
                    Pietitle = title
                });
                return keepers;
            }

            if (pieData.Count == 0)
            {
                pieData.Add(new PieData
                {
                    Catdesc = "NO DATA FOUND",
                    Data1 = 100,
                    Pietitle = title
                });
            }
            return pieData;
        }

        public static List<PieData> BuildCarPie(List<CarRawData> list, string dataToShow, string title, int howMany = 5)
        {
            var temp = list.GroupBy(s => s.Company, (company, recs) =>
            {
                var reclist = recs.ToList();
                return new
                {
                    Company = company,
                    Rents = reclist.Sum(s => s.Cplusmin),
                    Days = reclist.Sum(s => s.Days*s.Cplusmin),
                    CarCost = reclist.Sum(s => s.Abookrat*s.Days)
                };
            }).ToList();

            decimal total = dataToShow.Equals("3") ? list.Sum(s => s.Days * s.Abookrat)  : list.Sum(s => s.Cplusmin * s.Days);
            var carPieData = temp.Select(s => new PieData
            {
                Pietitle = title,
                Catdesc = s.Company,
                Data1 = total == 0 ? 0 : dataToShow.Equals("3") ? s.CarCost : s.Days / total
            }).OrderByDescending(s => s.Data1).ToList();

            carPieData = carPieData.Where(s => s.Data1 > 0).ToList();

            return TrimPieData(carPieData,howMany,title);
        }

        public static List<PieData> BuildHotelPie(List<HotelRawData> list, string dataToShow, string title, int howMany = 5)
        {
            var temp = list.GroupBy(s => s.Chaincod, (chain, recs) =>
            {
                var reclist = recs.ToList();
                return new
                {
                    ChainCode = chain,
                    HotelChain = LookupFunctions.LookupChains(chain, new MasterDataStore()),
                    Stays = reclist.Sum(s => s.Hplusmin),
                    Nights = reclist.Sum(s => s.Nights * s.Rooms * s.Hplusmin),
                    HotCost = reclist.Sum(s => s.Bookrate * s.Nights * s.Rooms)
                };
            }).ToList();

            decimal total = dataToShow.Equals("3") ? list.Sum(s => s.Nights * s.Bookrate * s.Rooms) : list.Sum(s => s.Hplusmin * s.Nights * s.Rooms);
            var carPieData = temp.Select(s => new PieData
            {
                Pietitle = title,
                Catdesc = s.HotelChain,
                Data1 = total == 0 ? 0 : dataToShow.Equals("3") ? s.HotCost : s.Nights / total
            }).OrderByDescending(s => s.Data1).ToList();

            carPieData = carPieData.Where(s => s.Data1 > 0).ToList();

            foreach (var row in carPieData.Where(s => string.IsNullOrEmpty(s.Catdesc)))
            {
                row.Catdesc = "NO CHAIN";
            }

            return TrimPieData(carPieData, howMany, title);
        }

        public static List<BarData> BuildAirBar(List<RawData> list, DateTime beginDate, string dataToShow, bool railData, string title, string langCode, List<string> months)
        {
            var results = list.OrderBy(s => s.UseDate).GroupBy(s => new { s.UseDate.GetValueOrDefault().Month, s.UseDate.GetValueOrDefault().Year}, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new BarData
                {
                    Bartitle = title,
                    MonthNum = key.Month,
                    Catdesc = months[key.Month - 1],
                    YearNum = key.Year,
                    Data1 = reclist.Any() ? dataToShow.Equals("2") ? reclist.Sum(s => s.Plusmin) : reclist.Sum(s => s.Airchg) : 0
                };

            })
            .OrderBy(x => x.YearNum)
            .ThenBy(x => x.MonthNum)
            .ToList();

            if (results.Any() && results.Max(s => s.Data1 > 50000))
            {
                foreach (var pieData in results)
                {
                    pieData.Bartitle += "(000s)";
                    pieData.Data1 = pieData.Data1 / 1000;
                }
            }

            return AddEmptyMonths(results, beginDate, title, months);
        }

        public static List<BarData> BuildCarBar(List<CarRawData> list, DateTime beginDate, string dataToShow, string title, string langCode, List<string> months)
        {
            var results = list.GroupBy(s => new { s.UseDate.GetValueOrDefault().Month, s.UseDate.GetValueOrDefault().Year }, (key, recs) => new BarData
            {
                Bartitle = title,
                MonthNum = key.Month,
                Catdesc = months[key.Month - 1],
                YearNum = key.Year,
                Data1 = GetData1ForCarBar(dataToShow, recs.ToList())
            })
            .OrderBy(x=>x.YearNum)
            .ThenBy(x=>x.MonthNum)
            .ToList();

            return AddEmptyMonths(results, beginDate, title, months);
        }

        private static decimal GetData1ForCarBar(string dataToShow, List<CarRawData> rows)
        {
            if (!rows.Any()) return 0;

            switch (dataToShow)
            {
                case "1":
                    return rows.Sum(s => s.Cplusmin*s.Days);
                case "D":
                    return rows.Sum(s => s.Cplusmin);
                default:
                    return rows.Sum(s => s.Abookrat * s.Days);
            }
        }

        public static List<BarData> BuildHotelBar(List<HotelRawData> list, DateTime beginDate, string dataToShow, string title, string langCode, List<string> months)
        {
            if (!list.Any()) return new List<BarData>();


            var results = list.GroupBy(s => new { s.UseDate.GetValueOrDefault().Month, s.UseDate.GetValueOrDefault().Year }, (key, recs) => new BarData
            {
                Bartitle = title,
                MonthNum = key.Month,
                Catdesc = months[key.Month-1],
                YearNum = key.Year,
                Data1 = GetData1ForHotelBar(dataToShow,recs.ToList()) 
            })
            .OrderBy(x => x.YearNum)
            .ThenBy(x => x.MonthNum)
            .ToList();

            return AddEmptyMonths(results, beginDate,title, months);
        }

        private static decimal GetData1ForHotelBar(string dataToShow, List<HotelRawData> rows)
        {
            if (!rows.Any()) return 0;

            switch (dataToShow)
            {
                case "1":
                    return rows.Sum(s => s.Hplusmin * s.Nights);
                case "D":
                    return rows.Sum(s => s.Hplusmin);
                default:
                    return rows.Sum(s => s.Bookrate * s.Nights);
            }
        }

        private static List<BarData> AddEmptyMonths(List<BarData> list, DateTime beginDate, string title, List<string> months)
        {
            if (list.Count == 12) return list;
            var newList = new List<BarData>();

            var month = beginDate.Month;
            var year = beginDate.Year;
            for (int i = 0; i < 12; i++)
            {
                var rec = list.FirstOrDefault(s => s.MonthNum == month && s.YearNum == year);
                if (rec == null)
                {
                    rec = new BarData
                    {
                        MonthNum = month,
                        YearNum = year,
                        Bartitle = title,
                        Catdesc = months[month-1],
                        Data1 = 0
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
    }
}

