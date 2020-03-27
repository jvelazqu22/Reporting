using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.AgentProductivity;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using MoreLinq;

namespace iBank.Services.Implementation.ReportPrograms.AgentProductivity
{
    public class AgentProductivityProcessor
    {
        public List<RawData> GetFilterRawData(List<RawData> rawDataList, ReportGlobals globals)
        {
            var rawData = rawDataList.GroupBy(s => new { s.AgentID, s.Valcarr, s.ValcarMode }, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new RawData
                {
                    AgentID = key.AgentID.Trim(),
                    Valcarr = key.Valcarr.Trim(),
                    ValcarMode = key.ValcarMode.Trim(),
                    Airchg = reclist.Sum(s => s.Airchg),
                    Trips = reclist.Count
                };
            }).ToList();

            var valCarCriteria = globals.GetParmValue(WhereCriteria.VALCARR) + globals.GetParmValue(WhereCriteria.INVALCARR);
            if (!string.IsNullOrEmpty(valCarCriteria))
            {
                var notIn = globals.IsParmValueOn(WhereCriteria.NOTINVALCARR);

                var valCarList = valCarCriteria.Split(',');

                //Separate raw data into "in the list" and "not in the list" for Other column. 
                rawData = notIn
                    ? rawData.Where(s => !valCarList.Contains(s.Valcarr)).ToList()
                    : rawData.Where(s => valCarList.Contains(s.Valcarr)).ToList();
            }

            return rawData;
        }

        public List<Tuple<string, string>> GetCarrierList(List<RawData> rawDataList, IMasterDataStore masterStore, bool countTrips, int maxCarriers)
        {
            var temp = rawDataList.Select(s => new
            {
                ValCarr = s.Valcarr,
                CarrDesc = LookupFunctions.LookupAline(masterStore, s.Valcarr, s.Valcarr.Length == 4 ? "R" : "A"),
                s.Trips,
                s.Airchg

            }).GroupBy(s => new { s.ValCarr, s.CarrDesc }, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new
                {
                    key.ValCarr,
                    key.CarrDesc,
                    OrderBy = reclist.Sum(s => s.Trips),
                    //OrderBy = countTrips ? reclist.Sum(s => s.Trips) : reclist.Sum(s => s.Airchg),
                };
            }).OrderByDescending(s => s.OrderBy);

            return temp.Take(maxCarriers)
                .Select(s => new Tuple<string, string>(s.ValCarr, s.CarrDesc.Contains("NOT FOU") ? "UNKNOWN" : s.CarrDesc.Trim().Left(12)))
                .ToList();
        }

        public List<FinalData> GetFinalData(List<RawData> rawDataList, bool countTrips, 
            List<Tuple<string, string>> carrierList, ReportGlobals globals)
        {
            return rawDataList.GroupBy(s => s.AgentID, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new FinalData
                {
                    Agentid = key,
                    Carr1 = AgentProductivityHelpers.GetAirCharge(countTrips, reclist, 0, carrierList, globals.OutputFormat),
                    Carr2 = AgentProductivityHelpers.GetAirCharge(countTrips, reclist, 1, carrierList, globals.OutputFormat),
                    Carr3 = AgentProductivityHelpers.GetAirCharge(countTrips, reclist, 2, carrierList, globals.OutputFormat),
                    Carr4 = AgentProductivityHelpers.GetAirCharge(countTrips, reclist, 3, carrierList, globals.OutputFormat),
                    Carr5 = AgentProductivityHelpers.GetAirCharge(countTrips, reclist, 4, carrierList, globals.OutputFormat),
                    Carr6 = AgentProductivityHelpers.GetAirCharge(countTrips, reclist, 5, carrierList, globals.OutputFormat),
                    Carr7 = AgentProductivityHelpers.GetAirCharge(countTrips, reclist, 6, carrierList, globals.OutputFormat),
                    Carr8 = AgentProductivityHelpers.GetAirCharge(countTrips, reclist, 7, carrierList, globals.OutputFormat),
                    Carr9 = AgentProductivityHelpers.GetAirCharge(countTrips, reclist, 8, carrierList, globals.OutputFormat),
                    Carr10 = AgentProductivityHelpers.GetAirCharge(countTrips, reclist, 9, carrierList, globals.OutputFormat),
                    Carr11 = AgentProductivityHelpers.GetAirCharge(countTrips, reclist, 10, carrierList, globals.OutputFormat),
                    Carr12 = AgentProductivityHelpers.GetAirCharge(countTrips, reclist, 11, carrierList, globals.OutputFormat),
                    Carr13 = AgentProductivityHelpers.GetAirCharge(countTrips, reclist, 12, carrierList, globals.OutputFormat),
                    Carr14 = AgentProductivityHelpers.GetAirCharge(countTrips, reclist, 13, carrierList, globals.OutputFormat),
                    Tottrips = reclist.Sum(s => s.Trips),
                    Airchg = reclist.Sum(s => s.Airchg)
                };
            })
            .OrderBy(s => s.Agentid)
            .ToList();
        }
    }
}
