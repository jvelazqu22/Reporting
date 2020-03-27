using Domain.Models.ReportPrograms.TopBottomValidatingCarriers;
using iBank.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Linq;
using iBank.Server.Utilities.Classes;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomValidatingCarriers
{
    public class FinalDataHelper
    {
        private ReportGlobals _globals;
        private string _orderBy;
        private DataTypes.GroupBy _groupBy;
        private DataTypes.SortBy _sortBy;

        public FinalDataHelper(ReportGlobals globals, string OrderBy, DataTypes.GroupBy groupBy, DataTypes.SortBy sortBy)
        {
            _globals = globals;
            _orderBy = OrderBy;
            _groupBy = groupBy;
            _sortBy = sortBy;
        }

        public List<FinalData> UnionTwoLists(List<RawData> rawData1List, List<RawData> rawData2List, bool isCtryCode, bool isValCarr, IMasterDataStore store)
        {
            var finalQuery = rawData1List.Select(s => new FinalData
            {
                CtryCode = isCtryCode ? LookupFunctions.LookupHomeCountryCode(s.SourceAbbr, _globals, store) : "NA",
                HomeCtry = isCtryCode ? LookupFunctions.LookupHomeCountryName(s.SourceAbbr, _globals, store) : "NA",
                ValCarr = isValCarr ? s.ValCarr : "NA",
                Carrdesc = LookupFunctions.LookupAline(store, s.ValCarr, s.ValCarMode),
                Amt = s.AirChg,
                Trips = s.Plusmin,
                Avgcost = 0,
            }).GroupBy(s => new
            {
                CtryCode = s.CtryCode,
                HomeCtry = s.HomeCtry,
                ValCarr = s.ValCarr,
                Carrdesc = s.Carrdesc,
            }).Select(s => new FinalData
            {
                CtryCode = s.Key.CtryCode,
                HomeCtry = s.Key.HomeCtry,
                ValCarr = s.Key.ValCarr,
                Carrdesc = s.Key.Carrdesc,
                Amt = s.Sum(x => x.Amt),
                Trips = s.Sum(x => x.Trips),
                Avgcost = (s.Sum(x => x.Trips) > 0) ? s.Sum(x => x.Amt) / s.Sum(x => x.Trips) : 0,
            });

            var final = finalQuery.ToList();

            final = FinalDataAddRange(final, rawData2List, isCtryCode, isValCarr, store);

            return final;

        }
        public List<FinalData> UpdateSubTotalRange(List<FinalData> list)
        {
            List<FinalData> subList = null;
            if (_groupBy == DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER)
            {
                subList = list.GroupBy(s => new
                {
                    CtryCode = s.CtryCode,
                    HomeCtry = s.HomeCtry,
                }).Select(s => new FinalData
                {
                    CtryCode = s.Key.CtryCode,
                    HomeCtry = s.Key.HomeCtry,
                    SubAmt = s.Sum(x => x.Amt),
                    SubTrips = s.Sum(x => x.Trips),
                    SubAmt2 = s.Sum(x => x.Amt2),
                    SubTrips2 = s.Sum(x => x.Trips2),
                }).ToList();

                for (int i = 0; i < list.Count; i++)
                {
                    list[i].SubAmt = subList.Where(s => s.HomeCtry == list[i].HomeCtry && s.CtryCode == list[i].CtryCode).Sum(x => x.SubAmt);
                    list[i].SubTrips = subList.Where(s => s.HomeCtry == list[i].HomeCtry && s.CtryCode == list[i].CtryCode).Sum(x => x.SubTrips);
                    list[i].SubAmt2 = subList.Where(s => s.HomeCtry == list[i].HomeCtry && s.CtryCode == list[i].CtryCode).Sum(x => x.SubAmt2);
                    list[i].SubTrips2 = subList.Where(s => s.HomeCtry == list[i].HomeCtry && s.CtryCode == list[i].CtryCode).Sum(x => x.SubTrips2);
                }
            }
            else
            {
                subList = list.GroupBy(s => new
                {
                    Carrdesc = s.Carrdesc,
                    ValCarr = s.ValCarr
                }).Select(s => new FinalData
                {
                    Carrdesc = s.Key.Carrdesc,
                    ValCarr = s.Key.ValCarr,
                    SubAmt = s.Sum(x => x.Amt),
                    SubTrips = s.Sum(x => x.Trips),
                    SubAmt2 = s.Sum(x => x.Amt2),
                    SubTrips2 = s.Sum(x => x.Trips2),
                }).ToList();

                for (int i = 0; i < list.Count; i++)
                {
                    list[i].SubAmt = subList.Where(s => s.Carrdesc == list[i].Carrdesc).Sum(x => x.SubAmt);
                    list[i].SubTrips = subList.Where(s => s.Carrdesc == list[i].Carrdesc).Sum(x => x.SubTrips);
                    list[i].SubAmt2 = subList.Where(s => s.Carrdesc == list[i].Carrdesc).Sum(x => x.SubAmt2);
                    list[i].SubTrips2 = subList.Where(s => s.Carrdesc == list[i].Carrdesc).Sum(x => x.SubTrips2);
                }

            }
            return list;
        }

        public List<FinalData> FinalDataAddRange(List<FinalData> finalList, List<RawData> list2, bool isCtryCode, bool isValCarr, IMasterDataStore store)
        {
            if (list2.Count == 0) return finalList;

            var finalTemp = list2.Select(s => new FinalData
            {
                CtryCode = isCtryCode ? LookupFunctions.LookupHomeCountryCode(s.SourceAbbr, _globals, new MasterDataStore()) : "NA",
                HomeCtry = isCtryCode ? LookupFunctions.LookupHomeCountryName(s.SourceAbbr, _globals, new MasterDataStore()) : "NA",
                ValCarr = isValCarr ? s.ValCarr : "NA",
                Carrdesc = LookupFunctions.LookupAline(store, s.ValCarr, s.ValCarMode),
                Amt2 = s.AirChg,
                Trips2 = s.Plusmin,
                Avgcost2 = 0
            }).GroupBy(s => new
            {
                CtryCode = s.CtryCode,
                HomeCtry = s.HomeCtry,
                ValCarr = s.ValCarr,
                Carrdesc = s.Carrdesc,
            }).Select(s => new FinalData
            {
                CtryCode = s.Key.CtryCode,
                HomeCtry = s.Key.HomeCtry,
                ValCarr = s.Key.ValCarr,
                Carrdesc = s.Key.Carrdesc,
                Amt2 = s.Sum(x => x.Amt2),
                Trips2 = s.Sum(x => x.Trips2),
                Avgcost2 = (s.Sum(x => x.Trips2) > 0) ? s.Sum(x => x.Amt2) / s.Sum(x => x.Trips2) : 0,
            }).ToList();

            finalList.AddRange(finalTemp.GroupBy(data => new
            {
                data.CtryCode,
                data.HomeCtry,
                data.ValCarr,
                data.Carrdesc
            }).Select(data => new FinalData
            {
                CtryCode = data.Key.CtryCode,
                HomeCtry = data.Key.HomeCtry,
                ValCarr = data.Key.ValCarr,
                Carrdesc = data.Key.Carrdesc,
                Amt2 = data.Sum(x => x.Amt2),
                Trips2 = data.Sum(x => x.Trips2),
                Avgcost2 = data.Sum(x => x.Avgcost2),
            }));

            return finalList.GroupBy(data => new
            {
                data.CtryCode,
                data.HomeCtry,
                data.ValCarr,
                data.Carrdesc
            }).Select(data => new FinalData
            {
                CtryCode = data.Key.CtryCode,
                HomeCtry = data.Key.HomeCtry,
                ValCarr = data.Key.ValCarr,
                Carrdesc = data.Key.Carrdesc,
                Amt = data.Sum(x => x.Amt),
                Trips = data.Sum(x => x.Trips),
                Avgcost = data.Sum(x => x.Avgcost),
                Amt2 = data.Sum(x => x.Amt2),
                Trips2 = data.Sum(x => x.Trips2),
                Avgcost2 = data.Sum(x => x.Avgcost2)
            }).ToList();

        }
        public List<FinalData> GroupFinalData(List<FinalData> list, bool isGraphOutput, DataTypes.Sort sort, int nHowMany)
        {
            string group = (_groupBy == DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER) ? "HomeCtry" : "ValCarr";

            if (sort == DataTypes.Sort.Descending)
            {
                if (nHowMany == 0)
                {
                    if (_sortBy == DataTypes.SortBy.CARRIER_HOME_COUNTRY)
                    {
                        return list.OrderByDescending(x => x.GetType().GetProperty(_orderBy).GetValue(x, null))
                            .OrderByDescending(x => x.HomeCtry)
                            .GroupBy(x => x.GetType().GetProperty(group).GetValue(x, null))
                            .SelectMany(x => x)
                            .ToList();
                    }

                    return list.OrderByDescending(x => x.GetType().GetProperty(_orderBy).GetValue(x, null))
                        .GroupBy(x => x.GetType().GetProperty(group).GetValue(x, null))
                        .SelectMany(x => x)
                        .ToList();
                }

                return list.OrderByDescending(x => x.GetType().GetProperty(_orderBy).GetValue(x, null))
                    .GroupBy(x => x.GetType().GetProperty(group).GetValue(x, null))
                    .Take(nHowMany)
                    .SelectMany(x => x)
                    .ToList();
            }
            else
            {
                if (nHowMany == 0)
                {
                    if (_sortBy == DataTypes.SortBy.CARRIER_HOME_COUNTRY && _groupBy == DataTypes.GroupBy.VALIDATING_CARRIER_HOME_COUNTRY)
                    {
                        return list.OrderBy(x => x.GetType().GetProperty(_orderBy).GetValue(x, null))
                            .ThenBy(x=>x.HomeCtry)
                            .GroupBy(x => x.GetType().GetProperty(group).GetValue(x, null))
                            .SelectMany(x => x)
                            .ToList();
                    }

                    return list.OrderBy(x => x.GetType().GetProperty(_orderBy).GetValue(x, null))
                        .GroupBy(x => x.GetType().GetProperty(group).GetValue(x, null))
                        .SelectMany(x => x)
                        .ToList();
                }

                return list.OrderBy(x => x.GetType().GetProperty(_orderBy).GetValue(x, null))
                    .GroupBy(x => x.GetType().GetProperty(group).GetValue(x, null))
                    .Take(nHowMany)
                    .SelectMany(x => x)
                    .ToList();
            }
                        
        }
    }
}
