using Domain.Models.ReportPrograms.TopBottomCityPairReport;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities;
using System.Collections.Generic;
using System.Linq;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomCityPair
{
    public class FinalDataProducer
    {
        public FinalDataProducer()
        {
        }

        public List<FinalData> GetFirstDraftData(IMasterDataStore store, bool isCityPairByMetro, List<SemiFinalData> semiFinalDataList)
        {
            return semiFinalDataList.Select(s => new FinalData
            {
                Origin = s.Origin,
                Orgdesc = isCityPairByMetro ? LookupFunctions.LookupMetro(s.Origin, store) : AportLookup.LookupAport(store, s.Origin, s.Mode, s.Airline),
                Destinat = s.Destinat,
                Destdesc = isCityPairByMetro ? LookupFunctions.LookupMetro(s.Destinat, store) : AportLookup.LookupAport(store, s.Destinat, s.Mode, s.Airline),
                Cpsegs = s.Segments,
                CpNumticks = s.Numticks,
                CpPctTtl = 0,
                Cpcost = s.Cost,
                CpAvgcost = s.Miles != 0 ? MathHelper.Round(s.Cost / s.Miles, 2) : 0,
                CpOnlnTkts = s.OnlineTkts,
                CpAgntTkts = s.AgentTkts,
                CpAgntCost = s.AgentCost,
                CpOnlnCost = s.OnlineCost,
                CpAgntSegs = s.AgentSegs,
                CpOnlnSegs = s.OnlineSegs,
                AirCO2 = s.AirCO2,
                Cpmiles = s.Miles,
                Airline = s.Airline,
                Alinedesc = LookupFunctions.LookupAline(store, s.Airline, s.Mode),
                Segments = s.Segments,
                PctTtl = 0m,
                Numticks = s.Numticks,
                Cost = s.Cost,
                Miles = s.Miles,
                Grp1fld = string.Empty
            }).OrderBy(x=>x.Alinedesc).ToList();
        }


        public List<FinalData> GetTopOrgDestGroup(List<FinalData> list, DataTypes.SortBy sortBy, DataTypes.Sort sort, bool useTickCnt, int nHowMany)
        {
            var grouped = list.GroupBy(x => new
            {
                x.Origin,
                x.Destinat,
                x.Orgdesc,
                x.Destdesc
            }).Select(x => new FinalData
            {
                Origin = x.Key.Origin,
                Destinat = x.Key.Destinat,
                Orgdesc = x.Key.Orgdesc,
                Destdesc = x.Key.Destdesc,
                Cpsegs = x.Sum(s => s.Segments),
                Cpcost = x.Sum(s => s.Cost),
                Cpmiles = x.Sum(s => s.Miles),
                CpAirCO2 = x.Sum(s=>s.AirCO2),
                CpNumticks = x.Sum(s => s.Numticks),
                CpAgntSegs = x.Sum(s => s.CpAgntSegs),
                CpAgntCost = x.Sum(s => s.CpAgntCost),
                CpAgntTkts = x.Sum(s => s.CpAgntTkts),
                CpOnlnSegs = x.Sum(s => s.CpOnlnSegs),
                CpOnlnCost = x.Sum(s => s.CpOnlnCost),
                CpOnlnTkts = x.Sum(s => s.CpOnlnTkts)
            }).ToList();

            for (int j = 0; j < grouped.Count; j++)
            {
                var item = grouped[j];
                
                item.CpAvgcost = useTickCnt
                    ? item.CpNumticks != 0 ? (decimal)MathHelper.Round(item.Cpcost / item.CpNumticks, 2) : 0.00m
                    : item.Cpsegs != 0 ? (decimal)MathHelper.Round(item.Cpcost / item.Cpsegs, 2) : 0.00m;
                item.CpOnlnAvg = useTickCnt
                    ? item.CpOnlnTkts != 0 ? (decimal)MathHelper.Round(item.CpOnlnCost / item.CpOnlnTkts, 2) : 0.00m
                    : item.CpOnlnSegs != 0 ? (decimal)MathHelper.Round(item.CpOnlnCost / item.CpOnlnSegs, 2) : 0.00m;
                item.CpAgntAvg = useTickCnt
                    ? item.CpAgntTkts != 0 ? (decimal)MathHelper.Round(item.CpAgntCost / item.CpAgntTkts, 2) : 0.00m
                    : item.CpAgntSegs != 0 ? (decimal)MathHelper.Round(item.CpAgntCost / item.CpAgntSegs, 2) : 0.00m;
               
            }

            grouped = GroupFinalData(grouped, sortBy, sort, useTickCnt, nHowMany);

            var newFinal = from final in grouped
                           join raw in list
                            on new
                            {
                                final.Origin,
                                final.Destinat
                            }
                            equals new
                            {
                                raw.Origin,
                                raw.Destinat
                            }
                           into temp
                           from raw in temp
                           select new FinalData
                           {
                               Origin = raw.Origin,
                               Orgdesc = raw.Orgdesc,
                               Destinat = raw.Destinat,
                               Destdesc = raw.Destdesc,
                               Cpsegs = final.Cpsegs,
                               CpNumticks = final.CpNumticks,
                               CpPctTtl = 0,
                               Cpcost = final.Cpcost,
                               CpAvgcost = final.CpAvgcost,
                               CpOnlnCost = final.CpOnlnCost,
                               CpAgntCost = final.CpAgntCost,
                               CpAgntSegs = final.CpAgntSegs,
                               CpOnlnSegs = final.CpOnlnSegs,
                               CpAgntAvg = final.CpAgntAvg,
                               CpOnlnAvg = final.CpOnlnAvg,
                               CpAgntTkts = final.CpAgntTkts,
                               CpOnlnTkts = final.CpOnlnTkts,
                               CpAirCO2 = final.Cpmiles == 0 ? 0 : final.CpAirCO2,
                               Cpmiles = final.Cpmiles,
                               Airline = raw.Airline,
                               Alinedesc = raw.Alinedesc,
                               AirCO2 = raw.Miles == 0 ? 0 : raw.AirCO2,
                               Segments = raw.Segments,
                               PctTtl = 0m,
                               Numticks = raw.Numticks,
                               Cost = raw.Cost,
                               Miles = raw.Miles,
                               Grp1fld = final.Grp1fld
                           };

            return newFinal.ToList();
        }

        public List<FinalData> GroupByOrgDest(List<FinalData> list, bool useTickCnt)
        {
            var temp = list.GroupBy(x => new
            {
                x.Origin,
                x.Destinat,
                x.Orgdesc,
                x.Destdesc
            }).Select(x => new FinalData
            {
                Origin = x.Key.Origin,
                Destinat = x.Key.Destinat,
                Orgdesc = x.Key.Orgdesc,
                Destdesc = x.Key.Destdesc,
                Cpsegs = x.Sum(s => s.Segments),
                Cpcost = x.Sum(s => s.Cost),
                Cpmiles = x.Sum(s => s.Miles),
                CpNumticks = x.Sum(s=>s.Numticks),
                CpAgntSegs = x.Sum(s=>s.CpAgntSegs),
                CpAgntCost = x.Sum(s=>s.CpAgntCost),
                CpAgntTkts = x.Sum(s=>s.CpAgntTkts),
                CpOnlnSegs = x.Sum(s=>s.CpOnlnSegs),
                CpOnlnCost = x.Sum(s=>s.CpOnlnCost),
                CpOnlnTkts = x.Sum(s=>s.CpOnlnTkts)
            }).ToList();

            for(int i=0; i<list.Count; i++)
            {
                var item = list[i];
                for (int j=0; j<temp.Count; j++)
                {
                    if (item.Origin == temp[j].Origin && item.Destinat == temp[j].Destinat )
                    {
                        item.Cpsegs = temp[j].Cpsegs;
                        item.Cpcost = temp[j].Cpcost;
                        item.Cpmiles = temp[j].Cpmiles;
                        item.CpNumticks = temp[j].CpNumticks;
                        item.CpOnlnTkts = temp[j].CpOnlnTkts;
                        item.CpAgntTkts = temp[j].CpAgntTkts;
                        item.CpOnlnSegs = temp[j].CpOnlnSegs;
                        item.CpAgntSegs = temp[j].CpAgntSegs;
                        item.CpAvgcost = useTickCnt 
                            ? temp[j].CpNumticks != 0 ? (decimal)MathHelper.Round(temp[j].Cpcost / temp[j].CpNumticks, 2) : 0.00m
                            : temp[j].Cpsegs != 0 ? (decimal)MathHelper.Round(temp[j].Cpcost / temp[j].Cpsegs, 2) : 0.00m;
                        item.CpOnlnAvg = useTickCnt
                            ? temp[j].CpOnlnTkts != 0 ? (decimal)MathHelper.Round(temp[j].CpOnlnCost / temp[j].CpOnlnTkts, 2) : 0.00m
                            : temp[j].CpOnlnSegs != 0 ? (decimal)MathHelper.Round(temp[j].CpOnlnCost / temp[j].CpOnlnSegs, 2) : 0.00m;
                        item.CpAgntAvg = useTickCnt
                            ? temp[j].CpAgntTkts != 0 ? (decimal)MathHelper.Round(temp[j].CpAgntCost / temp[j].CpAgntTkts, 2) : 0.00m
                            : temp[j].CpAgntSegs != 0 ? (decimal)MathHelper.Round(temp[j].CpAgntCost / temp[j].CpAgntSegs, 2) : 0.00m;
                    }
                }
            }
            return list;

        }

        public List<FinalData> GroupFinalData(List<FinalData> list, DataTypes.SortBy sortBy, DataTypes.Sort sort, bool isUseTickCnt, int nHowMany)
        {
            var takeTo = (nHowMany == 0) ? list.Count(): nHowMany;
            List<FinalData> result = new List<FinalData>();
            if (sort == DataTypes.Sort.DESCENDING)
            {
                switch (sortBy)
                {
                    case DataTypes.SortBy.VOLUME_BOOKED:
                        result = list.OrderByDescending(x => x.Cpcost)
                            .ThenBy(x => x.Orgdesc)
                            .ThenBy(x => x.Destdesc)
                            .ThenBy(x => x.Airline)
                            .GroupBy(x => new { x.Origin, x.Destinat })
                            .Take(takeTo)
                            .SelectMany(x => x)
                            .ToList();
                        break;
                    case DataTypes.SortBy.AVG_COST_PER_SEGMENT_OR_TRIP:
                        result = list.OrderByDescending(x => x.CpAvgcost)
                            .ThenBy(x => x.Orgdesc)
                            .ThenBy(x => x.Destdesc)
                            .ThenBy(x => x.Airline)
                            .GroupBy(x => new { x.Origin, x.Destinat })
                            .Take(takeTo)
                            .SelectMany(x => x)
                            .ToList();

                        break;
                    case DataTypes.SortBy.NO_OF_SEGMENT_OR_TRIP:
                        result = isUseTickCnt 
                            ? list.OrderByDescending(x => x.CpNumticks)
                            .ThenBy(x => x.Orgdesc)
                            .ThenBy(x => x.Destdesc)
                            .ThenBy(x => x.Airline)
                            .GroupBy(x => new { x.Origin, x.Destinat })
                            .Take(takeTo)
                            .SelectMany(x => x)
                            .ToList()
                            : list.OrderByDescending(x => x.Cpsegs)
                            .ThenBy(x => x.Orgdesc)
                            .ThenBy(x => x.Destdesc)
                            .ThenBy(x => x.Airline)
                            .GroupBy(x => new { x.Origin, x.Destinat })
                            .Take(takeTo)
                            .SelectMany(x => x)
                            .ToList();
                        break;
                    case DataTypes.SortBy.CITY_PAIR:
                        result = list.OrderBy(x => x.Orgdesc)
                            .ThenBy(x => x.Destdesc)
                            .ThenBy(x => x.Airline)
                            .GroupBy(x => new { x.Origin, x.Destinat })
                            .SelectMany(x => x)
                            .ToList();
                        break;
                }
            }
            else
            {
                switch (sortBy)
                {
                    case DataTypes.SortBy.VOLUME_BOOKED:
                        result = list.OrderBy(x => x.Cpcost)
                            .ThenBy(x => x.Orgdesc)
                            .ThenBy(x => x.Destdesc)
                            .ThenBy(x => x.Airline)
                            .GroupBy(x => new { x.Origin, x.Destinat })
                            .Take(takeTo)
                            .SelectMany(x => x)
                            .ToList();
                        break;
                    case DataTypes.SortBy.AVG_COST_PER_SEGMENT_OR_TRIP:
                        result = list.OrderBy(x => x.CpAvgcost)
                            .ThenBy(x => x.Orgdesc)
                            .ThenBy(x => x.Destdesc)
                            .ThenBy(x => x.Airline)
                            .GroupBy(x => new { x.Origin, x.Destinat })
                            .Take(takeTo)
                            .SelectMany(x => x)
                            .ToList();

                        break;
                    case DataTypes.SortBy.NO_OF_SEGMENT_OR_TRIP:
                        result = isUseTickCnt
                            ? list.OrderBy(x => x.Cpsegs)
                            .ThenBy(x => x.Orgdesc)
                            .ThenBy(x => x.Destdesc)
                            .ThenBy(x => x.Airline)
                            .GroupBy(x => new { x.Origin, x.Destinat })
                            .Take(takeTo)
                            .SelectMany(x => x)
                            .ToList()
                            : list.OrderBy(x => x.Cpsegs)
                            .ThenBy(x => x.Orgdesc)
                            .ThenBy(x => x.Destdesc)
                            .ThenBy(x => x.Airline)
                            .GroupBy(x => new { x.Origin, x.Destinat })
                            .Take(takeTo)
                            .SelectMany(x => x)
                            .ToList();
                        break;
                    case DataTypes.SortBy.CITY_PAIR:
                        result = list.OrderBy(x => x.Orgdesc)
                            .ThenBy(x => x.Destdesc)
                            .ThenBy(x => x.Airline)
                            .GroupBy(x => new { x.Orgdesc, x.Destdesc })
                            .SelectMany(x => x)
                            .ToList();
                        break;
                }
            }

            var idx = 1;
            foreach (var item in result)
            {
                item.Grp1fld = idx++.ToString();
            }
            return result;
        }

        public decimal CalculateTotalCount(List<FinalData> list, bool isUseTicketCnt)
        {
            if (isUseTicketCnt)
            {
                return list.Sum(x => x.CpNumticks);
            }
            else
            {
                return list.Sum(x => x.Cpsegs);
            }
        }

        public List<FinalData> CalculateExcelCost(List<FinalData> list, decimal total, bool isExcludeMileage)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (total > 0)
                {
                    list[i].CpPctTtl = MathHelper.Round((decimal)list[i].Cpsegs / total * 100, 2);
                }
                list[i].PctTtl = list[i].Cpsegs == 0 ? 0m : MathHelper.Round((decimal)list[i].Segments / list[i].Cpsegs * 100, 2);
                if (!isExcludeMileage)
                {
                    list[i].Cpcst_Mile = list[i].Cpsegs == 0 || list[i].Cpmiles == 0 ? 0m : MathHelper.Round((decimal)list[i].Cpcost / list[i].Cpmiles, 2);
                    list[i].Cst_Mile = list[i].Cpsegs == 0 || list[i].Miles == 0? 0m : MathHelper.Round((decimal)list[i].Cost / list[i].Miles, 2);
                    list[i].Cpcst_km = list[i].Cpcst_Mile;
                    list[i].Cst_km = list[i].Cst_Mile;
                }
            }
            return list;
        }
    }
}
