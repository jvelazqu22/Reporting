using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper;
using Domain.Models.ReportPrograms.MarketReport;

using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.Market
{
    public class MarketDataProcessor
    {
        public IList<GroupedData> GroupRawData(IList<RawData> rawData)
        {
            var grouped = rawData.GroupBy(x => x.RecKey, (key, g) =>
            {
                var temp = g as IList<RawData> ?? g.ToList();
                var first = temp.FirstOrDefault();
                return new 
                {
                    RecKey = key,
                    RecCount = temp.Count,
                    BaseAirChg = first.AirChg - first.FareTax,
                    first.ActFare
                };
            })
                .Where(s => s.ActFare == 0)
                .ToList();

            return grouped.Select(x => new GroupedData
            {
                RecKey = x.RecKey,
                RecCount = x.RecCount,
                BaseAirChg = x.BaseAirChg.Value,
                ActFare = x.ActFare
            }).ToList();
        }

        //if userMode is blank - could be both air and rail, use data original mode (R or A)
        public IList<FinalData> MapRawToFinalData(IList<RawData> rawData, Carrier carrier1, Carrier carrier2, Carrier carrier3, bool useAirportCodes,
            string userMode, ReportGlobals globals)
        {
            return rawData.GroupBy(s => new { s.Origin, s.Destinat, s.Mode },
                (key, g) =>
                {
                    var temp = g.ToList();
                    var finalData = new FinalData();
                    finalData.Origin = key.Origin.Trim();
                    finalData.Destinat = key.Destinat.Trim();
                    finalData.Flt_Mkt = key.Origin.Trim() + "-" + key.Destinat.Trim();
                    finalData.Flt_Mkt2 = key.Destinat.Trim() + "-" + key.Origin.Trim();
                    finalData.OrgDesc = useAirportCodes
                        ? key.Origin.Trim()
                        : AportLookup.LookupAport(new MasterDataStore(), key.Origin, key.Mode, globals.Agency).Trim();
                    finalData.DestDesc = useAirportCodes
                        ? key.Destinat.Trim()
                        : AportLookup.LookupAport(new MasterDataStore(), key.Destinat, key.Mode, globals.Agency).Trim();
                    finalData.Segments = temp.Sum(s => s.PlusMin);
                    finalData.Fare = temp.Sum(s => s.ActFare);
                    finalData.Mode = key.Mode;
                    finalData.Carr1Segs =
                        Math.Abs(temp.Sum(s => carrier1.ExpandedCarriers.Contains(s.Airline.Trim()) ? s.PlusMin : 0)); //while using picklist Carriers show as "U-734" or "DL", while ExpandedCarriers will always be "DL"
                    finalData.Carr1Fare =
                        temp.Sum(s => carrier1.ExpandedCarriers.Contains(s.Airline.Trim()) ? s.ActFare : 0);
                    finalData.Carr2Segs =
                        Math.Abs(
                            temp.Sum(s => carrier2.ExpandedCarriers.IndexOf(s.Airline.Trim()) >= 0 ? s.PlusMin : 0));
                    finalData.Carr2Fare =
                        temp.Sum(s => carrier2.ExpandedCarriers.IndexOf(s.Airline.Trim()) >= 0 ? s.ActFare : 0);
                    finalData.Carr3Segs =
                        Math.Abs(
                            temp.Sum(s => carrier3.ExpandedCarriers.IndexOf(s.Airline.Trim()) >= 0 ? s.PlusMin : 0));
                    finalData.Carr3Fare =
                        temp.Sum(s => carrier3.ExpandedCarriers.IndexOf(s.Airline.Trim()) >= 0 ? s.ActFare : 0);

                    return finalData;
                }).ToList();
        }

        public List<FinalData> FilterFlightSegments(IList<FinalData> finalData, string flightSegments)
        {
            var fltsegs = flightSegments.Split(',').Select(s => s.Trim()).ToList();

            return finalData.Where(x=> fltsegs.Contains(x.Flt_Mkt, StringComparer.OrdinalIgnoreCase)).ToList();
        }

        public IList<FinalData> OrderFinalData(IList<FinalData> finalData, string sortBy, bool useAirportCodes)
        {
            switch (sortBy)
            {
                case Constants.SortByTotalNumberOfSegments:
                    finalData = useAirportCodes
                        ? finalData.OrderByDescending(s => s.Segments)
                                    .ThenBy(s => s.Origin)
                                    .ThenBy(s => s.Destinat)
                                    .ToList()
                        : finalData.OrderByDescending(s => s.Segments)
                                    .ThenBy(s => s.OrgDesc)
                                    .ThenBy(s => s.DestDesc)
                                    .ToList();
                    break;
                case Constants.SortByTotalRevenue:
                    finalData = useAirportCodes
                        ? finalData.OrderByDescending(s => s.Fare)
                                    .ThenBy(s => s.Origin)
                                    .ThenBy(s => s.Destinat)
                                    .ToList()
                        : finalData.OrderByDescending(s => s.Fare)
                                    .ThenBy(s => s.OrgDesc)
                                    .ThenBy(s => s.DestDesc)
                                    .ToList();
                    break;
                default:
                    finalData = useAirportCodes
                        ? finalData.OrderBy(s => s.Origin)
                                    .ThenBy(s => s.Destinat)
                                    .ToList()
                        : finalData.OrderBy(s => s.OrgDesc)
                                    .ThenBy(s => s.DestDesc)
                                    .ToList();
                    break;
            }

            return finalData;
        }

        public IList<RawData> FlipOriginAndDestination(IList<RawData> rawData)
        {
            foreach (var row in rawData)
            {
                var origin = row.Origin;
                if (string.Compare(origin, row.Destinat, StringComparison.CurrentCultureIgnoreCase) < 1) continue;

                row.Origin = row.Destinat;
                row.Destinat = origin;
            }

            return rawData;
        }
    }
}
