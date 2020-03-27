using Domain.Helper;
using Domain.Models.ReportPrograms.TopBottomDestinations;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Linq;
using Domain.Services;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomDestinations
{
    public static class FinalDataCalculator
    {
        // Finds furthest destination
        public static List<RawData> DetermineFurthestDestination(List<RawData> rawDataList)
        {
            //rawDataList = rawDataList.Where(s => s.Airchg == 497.24m).ToList();
            var temp = rawDataList.Select(s => new
            {
                s.RecKey,
                s.SourceAbbr,
                s.ValCarr,
                s.Airchg,
                s.Plusmin,
                s.SeqNo,
                s.FirstOrg,
                s.Origin,
                s.Destinat,
                Miles = AirMileageCalculator<RawData>.LookupAirMileage(s.FirstOrg.Left(3), s.Destinat.Left(3), s.Miles)

            })
                .OrderBy(s => s.RecKey)
                .ThenBy(s => s.SeqNo);

            var maxMiles = temp.GroupBy(s => s.RecKey, (key, recs) => new
            {
                RecKey = key,
                MaxMiles = recs.Max(s => s.Miles)
            });

            rawDataList = maxMiles.Join(temp, t => new { t.RecKey, Miles = t.MaxMiles }, r => new { r.RecKey, r.Miles }, (t, r) => new RawData
            {
                RecKey = r.RecKey,
                SourceAbbr = r.SourceAbbr,
                ValCarr = r.ValCarr,
                Airchg = r.Airchg,
                Plusmin = r.Plusmin,
                SeqNo = r.SeqNo,
                FirstOrg = r.FirstOrg,
                Origin = r.Origin,
                Destinat = r.Destinat,
                Miles = r.Miles
            }).ToList();

            //remove duplicates
            var dupes = rawDataList.GroupBy(s => s.RecKey, (key, recs) => new
            {
                RecKey = key,
                DupeCnt = recs.Count()
            })
            .Where(s => s.DupeCnt > 1);

            foreach (var dupe in dupes)
            {
                var firstMatch = rawDataList.Where(s => s.RecKey == dupe.RecKey).OrderBy(s => s.SeqNo).FirstOrDefault();
                if (firstMatch != null)
                {
                    rawDataList.RemoveAll(s => s.RecKey == firstMatch.RecKey && s.SeqNo != firstMatch.SeqNo);
                }
            }

            return rawDataList;
        }

        private static void GroupBy2(ref IEnumerable<FinalData> temp, ref IEnumerable<FinalData> tempYtd, List<RawData> rawDataList, List<RawData> rawDataListYtd, ReportGlobals globals)
        {
            temp = rawDataList.Select(s => new FinalData
            {
                Cat1 = LookupFunctions.LookupCountryCode(s.Destinat, new MasterDataStore()),
                Cat2 = LookupFunctions.LookupHomeCountryCode(s.SourceAbbr, globals, new MasterDataStore()),
                Trips = s.Plusmin,
                Volume = s.Airchg,
                Mode = s.Mode
            });
            tempYtd = rawDataListYtd.Select(s => new FinalData
            {
                Cat1 = LookupFunctions.LookupCountryCode(s.Destinat, new MasterDataStore()),
                Cat2 = LookupFunctions.LookupHomeCountryCode(s.SourceAbbr, globals, new MasterDataStore()),
                Ytdtrips = s.Plusmin,
                Ytdvolume = s.Airchg,
                Mode = s.Mode
            });
        }

        private static void GroupBy3(ref IEnumerable<FinalData> temp, ref IEnumerable<FinalData> tempYtd, List<RawData> rawDataList, List<RawData> rawDataListYtd)
        {
            temp = rawDataList.Select(s => new FinalData
            {
                Cat1 = LookupFunctions.LookupCountryCode(s.Destinat, new MasterDataStore()),
                Cat2 = s.Destinat,
                Trips = s.Plusmin,
                Volume = s.Airchg,
                Mode = s.Mode
            });
            tempYtd = rawDataListYtd.Select(s => new FinalData
            {
                Cat1 = LookupFunctions.LookupCountryCode(s.Destinat, new MasterDataStore()),
                Cat2 = s.Destinat,
                Ytdtrips = s.Plusmin,
                Ytdvolume = s.Airchg,
                Mode = s.Mode
            });
        }

        private static void GroupBy4(ref IEnumerable<FinalData> temp, ref IEnumerable<FinalData> tempYtd, List<RawData> rawDataList, List<RawData> rawDataListYtd)
        {
            temp = rawDataList.Select(s => new FinalData
            {
                Cat1 = s.Destinat,
                Trips = s.Plusmin,
                Volume = s.Airchg,
                Mode = s.Mode
            });
            tempYtd = rawDataListYtd.Select(s => new FinalData
            {
                Cat1 = s.Destinat,
                Ytdtrips = s.Plusmin,
                Ytdvolume = s.Airchg,
                Mode = s.Mode
            });
        }

        private static void GroupBy5(ref IEnumerable<FinalData> temp, ref IEnumerable<FinalData> tempYtd, List<RawData> rawDataList, List<RawData> rawDataListYtd)
        {
            temp = rawDataList.Select(s => new FinalData
            {
                Cat1 = s.Destinat,
                Cat2 = s.ValCarr,
                Trips = s.Plusmin,
                Volume = s.Airchg,
                Mode = s.Mode
            });
            tempYtd = rawDataListYtd.Select(s => new FinalData
            {
                Cat1 = s.Destinat,
                Cat2 = s.ValCarr,
                Ytdtrips = s.Plusmin,
                Ytdvolume = s.Airchg,
                Mode = s.Mode
            });
        }

        private static void GroupByDefault(ref IEnumerable<FinalData> temp, ref IEnumerable<FinalData> tempYtd, List<RawData> rawDataList, List<RawData> rawDataListYtd)
        {
            temp = rawDataList.Select(s => new FinalData
            {
                Cat1 = LookupFunctions.LookupCountryCode(s.Destinat, new MasterDataStore()),
                Cat2 = string.Empty,
                Trips = s.Plusmin,
                Volume = s.Airchg,
                Mode = s.Mode
            });
            tempYtd = rawDataListYtd.Select(s => new FinalData
            {
                Cat1 = LookupFunctions.LookupCountryCode(s.Destinat, new MasterDataStore()),
                Cat2 = string.Empty,
                Ytdtrips = s.Plusmin,
                Ytdvolume = s.Airchg,
                Mode = s.Mode
            });
        }

        // Groups Raw Data into Final Data
        public static List<FinalData> GroupAndCombine(List<RawData> rawDataList, List<RawData> rawDataListYtd, ReportGlobals globals)
        {
            var groupBy = globals.GetParmValue(WhereCriteria.GROUPBY);

            IEnumerable<FinalData> temp = new List<FinalData>();
            IEnumerable<FinalData> tempYtd = new List<FinalData>();
            switch (groupBy)
            {
                case "2":
                    GroupBy2(ref temp, ref tempYtd, rawDataList, rawDataListYtd, globals);
                    break;
                case "3":
                    GroupBy3(ref temp, ref tempYtd, rawDataList, rawDataListYtd);
                    break;
                case "4":
                    GroupBy4(ref temp, ref tempYtd, rawDataList, rawDataListYtd);
                    break;
                case "5":
                    GroupBy5(ref temp, ref tempYtd, rawDataList, rawDataListYtd);
                    break;
                default:
                    GroupByDefault(ref temp, ref tempYtd, rawDataList, rawDataListYtd);
                    break;
            }

            var results = temp.GroupBy(s => new { s.Cat1, s.Cat2, s.Mode }, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new FinalData
                {
                    Cat1 = key.Cat1,
                    Cat2 = key.Cat2,
                    Trips = reclist.Sum(s => s.Trips),
                    Volume = reclist.Sum(s => s.Volume),
                    Mode = key.Mode
                };
            }).ToList();

            var ytdFinalData = tempYtd.GroupBy(s => new { s.Cat1, s.Cat2, s.Mode }, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new FinalData
                {
                    Cat1 = key.Cat1,
                    Cat2 = key.Cat2,
                    Ytdtrips = reclist.Sum(s => s.Ytdtrips),
                    Ytdvolume = reclist.Sum(s => s.Ytdvolume),
                    Mode = key.Mode
                };
            }).ToList();

            if (groupBy.Equals("3") || groupBy.Equals("5"))
            {
                results = CombineFinalDataTwoLevels(results, ytdFinalData);
            }
            else
            {
                results = CombineFinalData(results, ytdFinalData);
            }

            return results;
        }

        // Groups Raw Data into Final Data by Trip/City Pairs
        public static List<FinalData> GroupAndCombineTripCityPair(List<RawData> rawDataList, List<RawData> rawDataListYtd, ReportGlobals globals)
        {
            var finals = rawDataList.Select(s => new
            {
                Cat1 = s.FirstOrg,
                Cat2 = s.Destinat,
                PlusMin = s.Plusmin,
                s.Airchg,
                Mode = s.Mode
            })
                .GroupBy(s => new { s.Cat1, s.Cat2 }, (key, recs) =>
                  {
                      var reclist = recs.ToList();
                      return new FinalData
                      {
                          Cat1 = key.Cat1,
                          Cat2 = key.Cat2,
                          OrgDestTemp = key.Cat2,
                          Trips = reclist.Sum(s => s.PlusMin),
                          Volume = reclist.Sum(s => s.Airchg), 
                     };
                  }).ToList();

            var finalsYtd = rawDataListYtd.Select(s => new
            {
                Cat1 = s.FirstOrg,
                Cat2 = s.Destinat,
                PlusMin = s.Plusmin,
                s.Airchg
            })
               .GroupBy(s => new { s.Cat1, s.Cat2 }, (key, recs) =>
               {
                   var reclist = recs.ToList();
                   return new FinalData
                   {
                       Cat1 = key.Cat1,
                       Cat2 = key.Cat2,
                       OrgDestTemp = key.Cat2,
                       Ytdtrips = reclist.Sum(s => s.PlusMin),
                       Ytdvolume = reclist.Sum(s => s.Airchg)
                   };
               }).ToList();

            foreach (var row in finals)
            {
                row.OrgDestTemp = string.CompareOrdinal(row.Cat1, row.Cat2) < 0 ? row.Cat1 : row.Cat2;
                row.Cat2 = string.CompareOrdinal(row.Cat1, row.Cat2) > 0 ? row.Cat1 : row.Cat2;
                row.Cat1 = row.OrgDestTemp;
                row.Catdesc1 = AportLookup.LookupAport(new MasterDataStore(), row.Cat1, "A", globals.Agency) + "--" + AportLookup.LookupAport(new MasterDataStore(), row.Cat2, "A", globals.Agency);
            }

            foreach (var row in finalsYtd)
            {
                row.OrgDestTemp = string.CompareOrdinal(row.Cat1, row.Cat2) < 0 ? row.Cat1 : row.Cat2;
                row.Cat2 = string.CompareOrdinal(row.Cat1, row.Cat2) > 0 ? row.Cat1 : row.Cat2;
                row.Cat1 = row.OrgDestTemp;
                row.Catdesc1 = AportLookup.LookupAport(new MasterDataStore(), row.Cat1, "A", globals.Agency);
                row.Catdesc1 = AportLookup.LookupAport(new MasterDataStore(), row.Cat1, "A", globals.Agency) + "--" + AportLookup.LookupAport(new MasterDataStore(), row.Cat2, "A", globals.Agency);
            }

            finals = CombineFinalDataByCityPair(finals, finalsYtd);

            return finals.GroupBy(s => s.Catdesc1, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new FinalData
                {
                    Catdesc1 = key,
                    Trips = reclist.Sum(s => s.Trips),
                    Volume = reclist.Sum(s => s.Volume),
                    Ytdtrips = reclist.Sum(s => s.Ytdtrips),
                    Ytdvolume = reclist.Sum(s => s.Ytdvolume)
                };
            }).ToList();
        }


        // Filter Home Country
        public static List<RawData> FilterHomeCountry(List<RawData> rawDataList, ReportGlobals globals, IMasterDataStore masterStore)
        {
            var homeCountry = globals.GetParmValue(WhereCriteria.HOMECTRY);
            var inHomeCountry = globals.GetParmValue(WhereCriteria.INHOMECTRY);
            var notIn = globals.IsParmValueOn(WhereCriteria.NOTINHOMECTRY);
            if (!string.IsNullOrEmpty(homeCountry) || !string.IsNullOrEmpty(inHomeCountry))
            {
                var homeCountries = HomeCountriesLookup.GetHomeCountries(new CacheService(), masterStore.MastersQueryDb, globals.ClientType, globals.Agency);
                var homeCountryList = (homeCountry + inHomeCountry).Split(',');
                var sourceAbbrs = notIn
                    ? homeCountries.Where(s => !homeCountryList.Contains(s.Value)).Select(s => s.Key.Trim())
                    : homeCountries.Where(s => homeCountryList.Contains(s.Value)).Select(s => s.Key.Trim());
                return rawDataList.Where(s => sourceAbbrs.Contains(s.SourceAbbr.Trim())).ToList();
            }
            return rawDataList;
        }

        // Gets first column heading for pdf report
        public static string GetColHead1(string groupBy)
        {
            switch (groupBy)
            {
                case "4":
                case "5":
                    return "Destination City";
                case "6":
                    return "Trip City Pair";
                default:
                    return "Destination Country";
            }
        }

        // Gets second column heading for pdf report
        public static string GetColHead2(string groupBy)
        {
            switch (groupBy)
            {
                case "2":
                    return "Home Country";
                case "3":
                    return "Destination City";
                case "5":
                    return "Validating Carrier";
                default:
                    return string.Empty;
            }
        }

        // Sets first origin for raw data
        public static void SetFirstOrg(List<RawData> rawDataList)
        {
            var recKeys = rawDataList.Select(s => s.RecKey).Distinct();
            foreach (var recKey in recKeys)
            {
                var legs = rawDataList.Where(s => s.RecKey == recKey).OrderBy(s => s.SeqNo);
                var firstLeg = legs.FirstOrDefault();
                if (firstLeg != null)
                {
                    var firstOrg = firstLeg.Origin;
                    foreach (var leg in legs)
                    {
                        leg.FirstOrg = firstOrg;
                    }
                }

            }
        }

        // Gets export fields for xlsx/csv reports
        public static List<string> GetExportFields(string groupBy)
        {
            var fields = new List<string>();

            switch (groupBy)
            {
                case "2":
                    fields.Add("CatDesc1 as DestCtry");
                    fields.Add("CatDesc2 as HomeCtry");
                    break;
                case "3":
                    fields.Add("CatDesc1 as DestCtry");
                    fields.Add("CatDesc2 as DestCity");
                    break;
                case "4":
                    fields.Add("CatDesc1 as DestCity");
                    break;
                case "5":
                    fields.Add("CatDesc1 as DestCity");
                    fields.Add("CatDesc2 as ValCarr");
                    break;
                case "6":
                    fields.Add("CatDesc1 as CityPair");
                    break;
                default:
                    fields.Add("CatDesc1 as DestCtry");
                    break;

            }
            fields.Add("Trips");
            fields.Add("Volume");
            fields.Add("AvgCost");
            fields.Add("YTDTrips");
            fields.Add("YTDVolume");
            fields.Add("YTDAvgCost");


            return fields;
        }

        // Combines year-to-date and filtered data into one list
        private static List<FinalData> CombineFinalData(List<FinalData> mainList, List<FinalData> ytdList)
        {
            foreach (var row in mainList)
            {
                var ytdRow = ytdList.FirstOrDefault(s => s.Cat1.EqualsIgnoreCase(row.Cat1));
                if (ytdRow != null)
                {
                    row.Ytdtrips = ytdRow.Ytdtrips;
                    row.Ytdvolume = ytdRow.Ytdvolume;
                    ytdList.Remove(ytdRow);
                }
            }

            mainList.AddRange(ytdList);

            return mainList;
        }

        // Combines year-to-date and filtered data into one list by city pair
        private static List<FinalData> CombineFinalDataByCityPair(List<FinalData> mainList, List<FinalData> ytdList)
        {
            foreach (var row in mainList)
            {
                var ytdRow = ytdList.FirstOrDefault(s => s.Catdesc1.EqualsIgnoreCase(row.Catdesc1));
                if (ytdRow != null)
                {
                    row.Ytdtrips = ytdRow.Ytdtrips;
                    row.Ytdvolume = ytdRow.Ytdvolume;
                    ytdList.Remove(ytdRow);
                }
            }

            mainList.AddRange(ytdList);

            return mainList;
        }

        // Combines year-to-date and filtered data into one list by grouping fields
        private static List<FinalData> CombineFinalDataTwoLevels(List<FinalData> mainList, List<FinalData> ytdList)
        {
            foreach (var row in mainList)
            {
                var ytdRow = ytdList.FirstOrDefault(s => s.Cat1.EqualsIgnoreCase(row.Cat1) && s.Cat2.EqualsIgnoreCase(row.Cat2));
                if (ytdRow != null)
                {
                    row.Ytdtrips = ytdRow.Ytdtrips;
                    row.Ytdvolume = ytdRow.Ytdvolume;
                    ytdList.Remove(ytdRow);
                }
            }

            mainList.AddRange(ytdList);

            return mainList;
        }
    }
}
