using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.CO2CombinedSummaryReport;

using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;
using iBank.Repository.SQL.Interfaces;

namespace iBank.Services.Implementation.ReportPrograms.CO2AirSummary
{
    public static class SubReportBuilderAir
    {
        public static List<TopCarrierData> BuildTopCarrier(IMasterDataStore store, List<RawData> list)
        {
            return list.GroupBy(s => new {Airline = s.Airline.Trim()}, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new TopCarrierData
                {
                    Airline = key.Airline,
                    AirlineDes =
                        LookupFunctions.LookupAline(store, key.Airline, "A"),
                    Airco2 = (int) reclist.Sum(s => s.AirCo2),
                    Miles = reclist.Sum(s => s.Miles*s.Plusmin),
                    Segs = reclist.Count
                };
            }).OrderByDescending(s => s.Airco2).Take(5).ToList();
        }

        public static List<ServiceClassData> BuildClassOfService(List<RawData> list)
        {

            var classes = new List<string> { "E", "F", "B" };
            foreach (var row in list.Where(s => !classes.Contains(s.ClassCat.Trim())))
            {
                row.ClassCat = "E";
            }
            var classCat = list.GroupBy(s => new {ClassCat = s.ClassCat.Trim()}, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new
                {
                    key.ClassCat,
                    Segs = reclist.Count,
                    Miles = reclist.Sum(s => s.Plusmin*Math.Abs(s.Miles)),
                    AirCo2 = reclist.Sum(s => s.AirCo2)
                };
            }).ToList();

            var haulType = list.GroupBy(s => new {HaulType = s.HaulType.Trim()}, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new
                {
                    key.HaulType,
                    Segs = reclist.Count,
                    Miles = reclist.Sum(s => s.Plusmin*Math.Abs(s.Miles)),
                    AirCo2 = reclist.Sum(s => s.AirCo2)
                };
            }).ToList();

            var classOfService = new ServiceClassData();
            var classRec = classCat.FirstOrDefault(s => s.ClassCat.EqualsIgnoreCase("F"));
            if (classRec != null)
            {
                classOfService.Fclssegs = classRec.Segs;
                classOfService.Fclsmiles = classRec.Miles;
                classOfService.Fclsco2 = (int) classRec.AirCo2;
            }
            classRec = classCat.FirstOrDefault(s => s.ClassCat.EqualsIgnoreCase("B"));
            if (classRec != null)
            {
                classOfService.Bclssegs = classRec.Segs;
                classOfService.Bclsmiles = classRec.Miles;
                classOfService.Bclsco2 = (int) classRec.AirCo2;
            }
            classRec = classCat.FirstOrDefault(s => s.ClassCat.EqualsIgnoreCase("E"));
            if (classRec != null)
            {
                classOfService.Eclssegs = classRec.Segs;
                classOfService.Eclsmiles = classRec.Miles;
                classOfService.Eclsco2 = (int) classRec.AirCo2;
            }

            var haulTypeRec = haulType.FirstOrDefault(s => s.HaulType.EqualsIgnoreCase("L"));
            if (haulTypeRec != null)
            {
                classOfService.Lhaulsegs = haulTypeRec.Segs;
                classOfService.Lhaulmiles = haulTypeRec.Miles;
                classOfService.Lhaulco2 = (int) haulTypeRec.AirCo2;
            }
            haulTypeRec = haulType.FirstOrDefault(s => s.HaulType.EqualsIgnoreCase("M"));
            if (haulTypeRec != null)
            {
                classOfService.Mhaulsegs = haulTypeRec.Segs;
                classOfService.Mhaulmiles = haulTypeRec.Miles;
                classOfService.Mhaulco2 = (int) haulTypeRec.AirCo2;
            }
            haulTypeRec = haulType.FirstOrDefault(s => s.HaulType.EqualsIgnoreCase("S"));
            if (haulTypeRec != null)
            {
                classOfService.Shaulsegs = haulTypeRec.Segs;
                classOfService.Shaulmiles = haulTypeRec.Miles;
                classOfService.Shaulco2 = (int)haulTypeRec.AirCo2;
            }
            return new List<ServiceClassData> {classOfService};
        }

        public static List<AccountBarData> BuildAccountBarGraph(List<RawData> list)
        {
           var barData = list.GroupBy(s => s.GroupField, (key, recs) => new AccountBarData
            {
                Groupfld = key,
                Airco2 = recs.Sum(s => s.AirCo2)
            })
            .OrderByDescending(s => s.Airco2).ToList();

            if (barData.Count > 5)
            {
                var firstFive = barData.Take(5).ToList();
                barData.RemoveRange(0, 5);
                var otherAirC02 = barData.Sum(s => s.Airco2);
                firstFive.Add(new AccountBarData
                {
                    Groupfld = "OTHER",
                    Airco2 = otherAirC02
                });
                return firstFive;
            }

            return barData;

        }

        public static List<TopGroupData> BuildTop5GroupField(List<RawData> list)
        {
            return list.GroupBy(s => s.GroupField.Trim(), (key, recs) =>
            {
                var reclist = recs.ToList();
                return new TopGroupData
                {
                    GroupFld = string.IsNullOrEmpty(key)?"[None]": key,
                    Airco2 = (int)reclist.Sum(s => s.AirCo2),
                    Miles = reclist.Sum(s => s.Miles*s.Plusmin),
                    Segs = reclist.Count
                };
            })
            .OrderByDescending(s => s.Airco2)
            .Take(5)
            .ToList();
        }
    }
}
