using iBank.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.AirTopBottomSegment;
using iBank.Repository.SQL.Interfaces;

namespace iBank.Services.Implementation.ReportPrograms.AirTopBottomSegment
{
    public class AirTopBottomSegmentCalculations
    {
        public List<FinalData> GetInitializeFinalDataFromRawData(List<RawData> rawDataList, IMasterDataStore store)
        {
            var finalDataList = new List<FinalData>();
            foreach (var s in rawDataList)
            {
                var finalData = new FinalData
                {
                    Airline = s.Airline,
                    Carrdesc = LookupFunctions.LookupAline(store, s.Airline,  s.Mode),
                    Segs = s.Plusmin,
                    Actfare = s.Plusmin*Math.Abs((s.ActFare + s.MiscAmt))
                };


                finalDataList.Add(finalData);
            }

            return finalDataList;
        }

        public List<FinalData> GetCalculatedValues(List<FinalData> finalDataList)
        {
            finalDataList = finalDataList.Where(f => !string.IsNullOrWhiteSpace(f.Airline))
                .GroupBy(s => new { s.Carrdesc, s.Airline },
                    (key, g) =>
                    {
                        var reclist = g.ToList();
                        var segSum = reclist.Sum(s => s.Segs);
                        return new FinalData()
                        {
                            Airline = reclist.First().Airline,
                            Carrdesc = reclist.First().Carrdesc,
                            Segs = segSum,
                            Actfare = reclist.Sum(s => s.Actfare),
                            Avgcost = segSum == 0 ? 0 : reclist.Sum(s => s.Actfare) / segSum,
                        };
                    }).ToList();

            return finalDataList;
        }

        public void UpdateTotalTripsAndTotalCharges(List<FinalData> finalDataList, ref int totalTrips, ref decimal totalCharges)
        {
            totalTrips = finalDataList.Sum(x => x.Segs);
            totalCharges = finalDataList.Sum(x => x.Actfare);
        }
    }
}
