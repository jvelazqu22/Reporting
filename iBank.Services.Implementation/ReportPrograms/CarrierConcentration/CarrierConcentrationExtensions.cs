using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.CarrierConcentrationReport;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.CarrierConcentration
{
    public static class CarrierConcentrationExtensions
    {
        public static List<IntermediaryData> ToIntermediaryData(this List<RawData> rawData, bool useAirportCodes, string segmentCarrier, ReportGlobals globals)
        {
            var tempList = rawData.Select(s => new
            {
                OrgDesc = !useAirportCodes ? AportLookup.LookupAport(new MasterDataStore(), s.Origin, s.Mode, globals.Agency) : s.Origin,
                DestDesc = !useAirportCodes ? AportLookup.LookupAport(new MasterDataStore(), s.Destinat, s.Mode, globals.Agency) : s.Destinat,
                s.Origin,
                s.Destinat,
                s.Flt_mkt,
                s.Flt_mkt2,
                s.Mode,
                s.Airline,
                s.DitCode,
                s.Plusmin,
                s.ActFare,
                s.RecordNo,
                cCarr1 = segmentCarrier
            }).ToList();

            return
                tempList.GroupBy(s => new { s.OrgDesc, s.DestDesc, s.Origin, s.Destinat, s.Flt_mkt, s.Flt_mkt2, s.Mode },
                    (key, recs) =>
                    {
                        var currentRecs = recs.ToList();
                        return new IntermediaryData
                        {
                            OrgDesc = key.OrgDesc,
                            DestDesc = key.DestDesc,
                            Origin = key.Origin,
                            Destinat = key.Destinat,
                            Flt_mkt = key.Flt_mkt.Trim(),
                            Flt_mkt2 = key.Flt_mkt2.Trim(),
                            Mode = key.Mode,
                            Segments = currentRecs.Sum(s => s.Plusmin),
                            Fare = currentRecs.Sum(s => s.Plusmin * Math.Abs(s.ActFare)),
                            Carr1Segs = currentRecs.Sum(s => s.Airline.EqualsIgnoreCase(segmentCarrier) ? s.Plusmin : 0),
                            Carr1Fare = currentRecs.Sum( s => s.Airline.EqualsIgnoreCase(segmentCarrier) ? s.Plusmin * Math.Abs(s.ActFare) : 0)

                        };
                    }).ToList();
        }

        public static IEnumerable<IntermediaryData> ToSortedData(this List<IntermediaryData> data, bool sortAscending, string sortBy)
        {
            var sorted = data.Select(s => new IntermediaryData
                                        {
                                            OrgDesc = s.OrgDesc,
                                            DestDesc = s.DestDesc,
                                            Flt_mkt = s.Flt_mkt,
                                            Segments = s.Segments < 0 ? 0 : s.Segments,
                                            Fare = s.Fare,
                                            Carr1Segs = s.Carr1Segs < 0 ? 0 : s.Carr1Segs,
                                            Carr1Fare = s.Carr1Fare
                                        });

            switch (sortBy)
            {
                case "2":
                    return sortAscending
                        ? sorted.OrderBy(s => s.Segments).ThenBy(s => s.OrgDesc).ThenBy(s => s.DestDesc)
                        : sorted.OrderByDescending(s => s.Segments).ThenBy(s => s.OrgDesc).ThenBy(s => s.DestDesc);
                case "3":
                    return sortAscending
                        ? sorted.OrderBy(s => s.Fare).ThenBy(s => s.OrgDesc).ThenBy(s => s.DestDesc)
                        : sorted.OrderByDescending(s => s.Fare).ThenBy(s => s.OrgDesc).ThenBy(s => s.DestDesc);
                default:
                    return sorted.OrderBy(s => s.OrgDesc).ThenBy(s => s.DestDesc);
            }
        }

        public static List<FinalData> ToFinalData(this IEnumerable<IntermediaryData> data, int howMany, bool useRecordLimit, bool isExcelOrCsv, string carr1Desc)
        {
            var counter = 0;
            var finalData = new List<FinalData>();
            foreach (var row in data)
            {
                counter++;
                if ((counter > howMany && howMany != 0) && useRecordLimit) break;

                var calculatedValues = new CarrierCalculatedValues(row.Fare, row.Segments, row.Carr1Segs, row.Carr1Fare);

                if (isExcelOrCsv)
                {
                    finalData.Add(new FinalData
                    {
                        Origin = row.OrgDesc,
                        Destinat = row.DestDesc,
                        Segments = row.Segments,
                        Totvolume = row.Fare,
                        Avgsegcost = calculatedValues.AverageFare,
                        Carrier = carr1Desc,
                        Carrsegs = row.Carr1Segs,
                        Pcntoftot = calculatedValues.CarrierPercentage,
                        Carrvolume = row.Carr1Fare,
                        Carravgseg = calculatedValues.CarrierAverage,
                        Avgsegdiff = calculatedValues.AvgSegmentDifference,
                        Carrsvngs = calculatedValues.CarrierSavings
                    });

                    finalData.Add(new FinalData
                    {
                        Carrier = "Total Others",
                        Carrsegs = row.Segments - row.Carr1Segs,
                        Pcntoftot = calculatedValues.OtherPercentage,
                        Carrvolume = row.Fare - row.Carr1Fare,
                        Carravgseg = calculatedValues.OtherAverage,
                        Avgsegdiff = calculatedValues.AvgSegmentDifference,
                        Othsvngs = calculatedValues.OtherSavings
                    });
                }
                else
                {
                    finalData.Add(new FinalData
                    {
                        Origin = row.OrgDesc,
                        Destinat = row.DestDesc,
                        Segments = row.Segments,
                        Totvolume = row.Fare,
                        Avgsegcost = calculatedValues.AverageFare,
                        Carrier = carr1Desc,
                        Carrsegs = row.Carr1Segs,
                        Pcntoftot = calculatedValues.CarrierPercentage,
                        Carrvolume = row.Carr1Fare,
                        Carravgseg = calculatedValues.CarrierAverage,
                        Avgsegdiff = calculatedValues.AvgSegmentDifference,
                        Carrsvngs = calculatedValues.CarrierSavings,
                        Othsvngs = calculatedValues.OtherSavings
                    });
                }
            }

            return finalData;
        }

    }
}
