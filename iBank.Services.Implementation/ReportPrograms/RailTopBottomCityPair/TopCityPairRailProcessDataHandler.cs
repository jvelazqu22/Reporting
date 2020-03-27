using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.TopBottomCityPairRail;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Utilities;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.RailTopBottomCityPair
{
    public class TopCityPairRailProcessDataHandler
    {
        public List<FinalData> GetFinalData(ReportGlobals globals, List<RawData> rawDataList, BuildWhere buildWhere, GlobalsCalculator globalCalc, IMasterDataStore masterStore, bool useTicketCount)
        {
            var finalDataList = new List<FinalData>();

            rawDataList = ApplyUserTicketCountLogic(rawDataList, useTicketCount);
            rawDataList = ApplyRbflTmktOneWayBothWwaysLogic(globals, rawDataList);
            rawDataList = ApplyRbOneWayBothWaysLogic(globals, rawDataList, buildWhere, globalCalc);

            //TODO: Do we still need this code? Question is out to Bob. Case doesn't seem to ever happen if we use mktsegs. 
            TopCityPairRailHelpers.AllocateAirCharge(rawDataList);

            var totalQuery = TotalFinalListQuery(rawDataList);
            var cityPairQuery = GetCityPairFinalDataQuery(totalQuery);
            finalDataList = GetFinalDataList(totalQuery, cityPairQuery, masterStore, useTicketCount, globals);

            return finalDataList;
        }

        private List<RawData> ApplyUserTicketCountLogic(List<RawData> rawDataList, bool useTicketCount)
        {
            if (useTicketCount)
            {
                foreach (var row in rawDataList)
                {
                    var numSegs = rawDataList.Count(s => s.RecKey == row.RecKey);
                    row.NumTicks = numSegs == 0 ? 0 : Math.Round((decimal)1 / numSegs, 3);
                }
            }

            return rawDataList;
        }

        private List<RawData> ApplyRbflTmktOneWayBothWwaysLogic(ReportGlobals globals, List<RawData> rawDataList)
        {
            if (globals.ParmValueEquals(WhereCriteria.RBFLTMKTONEWAYBOTHWAYS, "1"))
            {
                foreach (var row in rawDataList)
                {
                    if (string.Compare(row.Origin.Trim(), row.Destinat.Trim(), StringComparison.Ordinal) > 0)
                    {
                        var temp = row.Origin;
                        row.Origin = row.Destinat;
                        row.Destinat = temp;
                    }
                }
            }
            return rawDataList;
        }

        private List<RawData> ApplyRbOneWayBothWaysLogic(ReportGlobals globals, List<RawData> rawDataList, BuildWhere buildWhere, GlobalsCalculator globalCalc)
        {
            if (globals.ParmValueEquals(WhereCriteria.RBONEWAYBOTHWAYS, "1") && buildWhere.HasRoutingCriteria)
            {
                //we really only want the record numbers here, and we don't want to change the original list. 
                var firstPass = new List<RawData>(rawDataList);
                var recnosFirstPass = globalCalc.IsAppliedToLegLevelData() ? buildWhere.ApplyWhereRoute(firstPass, true).Select(s => s.RecordNo) : buildWhere.ApplyWhereRoute(firstPass, false).Select(s => s.RecordNo);

                TopCityPairRailHelpers.SwapOriginsAndDestinations(globals);

                var recnosSecondPass = globalCalc.IsAppliedToLegLevelData() ? buildWhere.ApplyWhereRoute(firstPass, true).Select(s => s.RecordNo) : buildWhere.ApplyWhereRoute(firstPass, false).Select(s => s.RecordNo);

                rawDataList = rawDataList.Where(s => recnosFirstPass.Contains(s.RecordNo) || recnosSecondPass.Contains(s.RecordNo)).ToList();
            }
            else
            {
                rawDataList = globalCalc.IsAppliedToLegLevelData() ? buildWhere.ApplyWhereRoute(rawDataList, true) : buildWhere.ApplyWhereRoute(rawDataList, false);
            }
            return rawDataList;
        }

        private List<FinalData> TotalFinalListQuery(List<RawData> rawDataList)
        {
            var totalQuery = rawDataList.Where(s => !string.IsNullOrEmpty(s.Origin.Trim()) && !string.IsNullOrEmpty(s.Destinat.Trim()))
                .GroupBy(s => new { Origin = s.Origin.Trim(), Destinat = s.Destinat.Trim(), Mode = s.Mode.Trim(), Airline = s.Airline.Trim() },
                    (key, recs) =>
                    {
                        var reclist = recs.ToList();
                        return new FinalData
                        {
                            Origin = key.Origin,
                            Destinat = key.Destinat,
                            Airline = key.Airline,
                            Mode = key.Mode,
                            Segments = reclist.Sum(s => s.Plusmin),
                            Cost = reclist.Sum(s => s.Plusmin * Math.Abs(s.ActFare)),
                            Miles = reclist.Sum(s => s.Plusmin * Math.Abs(s.Miles)),
                            Numticks = reclist.Sum(s => s.Plusmin * s.NumTicks)
                        };
                    }).ToList();

            return totalQuery;
        }

        private List<FinalData> GetCityPairFinalDataQuery(List<FinalData> totalQuery)
        {
            var cityPairQuery = totalQuery.GroupBy(s => new { s.Origin, s.Destinat, s.Mode },
                (key, recs) =>
                {
                    var reclist = recs.ToList();
                    return new FinalData
                    {
                        Origin = key.Origin,
                        Destinat = key.Destinat,
                        Cpsegs = reclist.Sum(s => s.Segments),
                        Cpcost = reclist.Sum(s => s.Cost),
                        Cpmiles = reclist.Sum(s => s.Miles),
                        Cpnumticks = reclist.Sum(s => s.Numticks),
                        Cpavgcost = 0,
                        Mode = key.Mode
                    };
                }).ToList();

            return cityPairQuery;
        }

        private List<FinalData> GetFinalDataList(List<FinalData> totalQuery, List<FinalData> cityPairQuery, IMasterDataStore masterStore, bool useTicketCount, ReportGlobals globals)
        {
            //join the two data sets
            var finalDataList = totalQuery.Join(cityPairQuery, f => new { f.Origin, f.Destinat }, s => new { s.Origin, s.Destinat },
                (f, s) => new FinalData
                {
                    Origin = f.Origin,
                    Orgdesc = AportLookup.LookupAport(masterStore, f.Origin, f.Mode, globals.Agency),
                    Destinat = f.Destinat,
                    Destdesc = AportLookup.LookupAport(masterStore, f.Destinat, f.Mode, globals.Agency),
                    Cpsegs = s.Cpsegs,
                    Cpnumticks = s.Cpnumticks,
                    Cpcost = s.Cpcost,
                    Cpavgcost = s.Cpsegs == 0 ? 0 : Math.Round(s.Cpcost / (useTicketCount ? s.Cpnumticks : s.Cpsegs), 2),
                    Cpmiles = s.Cpmiles,
                    Airline = f.Airline,
                    Alinedesc = LookupFunctions.LookupAline(masterStore, f.Airline, f.Mode),
                    Segments = f.Segments,
                    Numticks = f.Numticks,
                    Cost = f.Cost,
                    Miles = f.Miles,
                }).ToList();

            return finalDataList;
        }

    }
}
