using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Models.ReportPrograms.TopBottomDestinations;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomDestinations
{
    public class SortHandler
    {
        public static List<FinalData> SortAndGroupFinalData(List<FinalData> finalDataList, ReportGlobals globals, IMasterDataStore store)
        {
            var oneLevelGroup = new List<string> { "1", "4", "6" };

            var howMany = globals.GetParmValue(WhereCriteria.HOWMANY).TryIntParse(0);
            var sortBy = globals.GetParmValue(WhereCriteria.SORTBY);
            var useYtdNumbers = globals.IsParmValueOn(WhereCriteria.CBUSEYTDNBRS);
            var descending = globals.ParmValueEquals(WhereCriteria.RBSORTDESCASC, "1") && !sortBy.Equals("4");

            if (oneLevelGroup.Contains(globals.GetParmValue(WhereCriteria.GROUPBY)))
            {
                var results = SortSingleLevel(finalDataList, sortBy, useYtdNumbers, descending);

                if (!globals.ParmValueEquals(WhereCriteria.GROUPBY, "6"))
                {
                    foreach (var row in results)
                    {
                        row.Catdesc1 = GetCatDesc1(row, globals, store);
                        row.Catdesc2 = GetCatDesc2(row, globals, store);
                    }
                }
                if (sortBy.Equals("4")) howMany = 0;
                finalDataList = howMany == 0
                    ? results.ToList()
                    : results.Take(howMany).ToList();
            }
            else
            {
                var dbf1A = SortTwoLevel(finalDataList, sortBy, useYtdNumbers, descending, howMany);
                //join with original records
                finalDataList = dbf1A.Join(finalDataList, d => d.Cat1, f => f.Cat1, (d, f) => new FinalData
                {
                    Cat1 = f.Cat1,
                    Catdesc1 = GetCatDesc1(f, globals, store),
                    Cat2 = f.Cat2,
                    Catdesc2 = GetCatDesc2(f, globals, store),
                    Trips = f.Trips,
                    Volume = f.Volume,
                    Ytdtrips = f.Ytdtrips,
                    Ytdvolume = f.Ytdvolume,
                    Subtrips = d.Subtrips,
                    Subvolume = d.Subvolume,
                    Subytdtrps = d.Subytdtrps,
                    Subytdvol = d.Subytdvol

                }).ToList();

                finalDataList = Sort(finalDataList, sortBy, useYtdNumbers, descending);
            }
            return finalDataList;
        }

        private static List<FinalData> Sort(List<FinalData> finalDataList, string sortBy, bool useYtdNumbers, bool descending)
        {
            switch (sortBy)
            {
                case "1":
                    return useYtdNumbers
                        ? descending ? finalDataList.OrderByDescending(s => s.Subytdvol).ThenByDescending(s => s.Ytdvolume).ToList() : finalDataList.OrderBy(s => s.Subytdvol).ThenBy(s => s.Ytdvolume).ToList()
                        : descending ? finalDataList.OrderByDescending(s => s.Subvolume).ThenByDescending(s => s.Volume).ToList() : finalDataList.OrderBy(s => s.Subvolume).ThenBy(s => s.Volume).ToList();
                case "2":
                    return useYtdNumbers
                        ? descending ? finalDataList.OrderByDescending(s => s.SubYtdavgcost).ThenByDescending(s => s.Ytdavgcost).ToList() : finalDataList.OrderBy(s => s.SubYtdavgcost).ThenBy(s => s.Ytdavgcost).ToList()
                        : descending ? finalDataList.OrderByDescending(s => s.SubAvgcost).ThenByDescending(s => s.Avgcost).ToList() : finalDataList.OrderBy(s => s.SubAvgcost).ThenBy(s => s.Avgcost).ToList();
                case "3":
                    return useYtdNumbers
                        ? descending ? finalDataList.OrderByDescending(s => s.Subytdtrps).ThenByDescending(s => s.Ytdtrips).ToList() : finalDataList.OrderBy(s => s.Subytdtrps).ThenBy(s => s.Ytdtrips).ToList()
                        : descending ? finalDataList.OrderByDescending(s => s.Subtrips).ThenByDescending(s => s.Trips).ToList() : finalDataList.OrderBy(s => s.Subtrips).ThenBy(s => s.Trips).ToList();
                default:
                    return descending
                        ? finalDataList.OrderByDescending(s => s.Cat1).ThenByDescending(s => s.Catdesc2).ToList()
                        : finalDataList.OrderBy(s => s.Cat1).ThenByDescending(s => s.Catdesc2).ToList();
            }
        }

        private static IEnumerable<FinalData> SortSingleLevelByAveCost(List<FinalData> finalDataList, bool useYtdNumbers, bool descending)
        {
            IEnumerable<FinalData> results;
            if (useYtdNumbers)
            {
                results = descending
                    ? finalDataList.OrderByDescending(s => s.Ytdavgcost)
                    : finalDataList.OrderBy(s => s.Ytdavgcost);
            }
            else
            {
                results = descending
                ? finalDataList.OrderByDescending(s => s.Avgcost)
                : finalDataList.OrderBy(s => s.Avgcost);
            }
            return results;
        }

        private static IEnumerable<FinalData> SortSingleLevelByNumOfTrips(List<FinalData> finalDataList, bool useYtdNumbers, bool descending)
        {
            IEnumerable<FinalData> results;
            if (useYtdNumbers)
            {
                results = descending
                    ? finalDataList.OrderByDescending(s => s.Ytdtrips)
                    : finalDataList.OrderBy(s => s.Ytdtrips);
            }
            else
            {
                results = descending
                ? finalDataList.OrderByDescending(s => s.Trips)
                : finalDataList.OrderBy(s => s.Trips);
            }
            return results;
        }

        private static IEnumerable<FinalData> SortSingleLevelByCatDesc(List<FinalData> finalDataList, bool useYtdNumbers, bool descending)
        {
            IEnumerable<FinalData> results;
            if (useYtdNumbers)
            {
                results = descending
                    ? finalDataList.OrderByDescending(s => s.Catdesc2)
                    : finalDataList.OrderBy(s => s.Catdesc2);
            }
            else
            {
                results = descending
                ? finalDataList.OrderByDescending(s => s.Catdesc1)
                : finalDataList.OrderBy(s => s.Catdesc1);
            }

            return results;
        }

        private static IEnumerable<FinalData> SortSingleLevelByVolume(List<FinalData> finalDataList, bool useYtdNumbers, bool descending)
        {
            IEnumerable<FinalData> results;
            if (useYtdNumbers)
            {
                results = descending
                    ? finalDataList.OrderByDescending(s => s.Ytdvolume)
                    : finalDataList.OrderBy(s => s.Ytdvolume);
            }
            else
            {
                results = descending
                ? finalDataList.OrderByDescending(s => s.Volume)
                : finalDataList.OrderBy(s => s.Volume);
            }

            return results;
        }

        private static IEnumerable<FinalData> SortSingleLevel(List<FinalData> finalDataList, string sortBy, bool useYtdNumbers, bool descending)
        {
            IEnumerable<FinalData> results;
            switch (sortBy)
            {
                case "2": //Avg cost
                    return SortSingleLevelByAveCost(finalDataList, useYtdNumbers, descending);
                case "3": //# of trips
                    return SortSingleLevelByNumOfTrips(finalDataList, useYtdNumbers, descending);
                case "4": //Alpha
                    return SortSingleLevelByCatDesc(finalDataList, useYtdNumbers, descending);
                default: //volume booked
                    return SortSingleLevelByVolume(finalDataList, useYtdNumbers, descending);
            }
        }

        private static IEnumerable<FinalData> SortTwoLevelByAveCost(List<FinalData> finalDataList, bool useYtdNumbers, bool descending, IEnumerable<FinalData> dbf1A)
        {
            if (useYtdNumbers)
            {
                dbf1A = descending
                    ? dbf1A.OrderByDescending(s => s.SubYtdavgcost)
                    : dbf1A.OrderBy(s => s.SubYtdavgcost);
            }
            else
            {
                dbf1A = descending
                ? dbf1A.OrderByDescending(s => s.SubAvgcost)
                : dbf1A.OrderBy(s => s.SubAvgcost);
            }
            return dbf1A;
        }

        private static IEnumerable<FinalData> SortTwoLevelByNumOfTrips(List<FinalData> finalDataList, bool useYtdNumbers, bool descending, IEnumerable<FinalData> dbf1A)
        {
            if (useYtdNumbers)
            {
                dbf1A = descending
                    ? dbf1A.OrderByDescending(s => s.Subytdtrps)
                    : dbf1A.OrderBy(s => s.Subytdtrps);
            }
            else
            {
                dbf1A = descending
                ? dbf1A.OrderByDescending(s => s.Subtrips)
                : dbf1A.OrderBy(s => s.Subtrips);
            }
            return dbf1A;
        }

        private static IEnumerable<FinalData> SortTwoLevelByVolume(List<FinalData> finalDataList, bool useYtdNumbers, bool descending, IEnumerable<FinalData> dbf1A)
        {
            if (useYtdNumbers)
            {
                dbf1A = descending
                    ? dbf1A.OrderByDescending(s => s.Subytdvol)
                    : dbf1A.OrderBy(s => s.Subytdvol);
            }
            else
            {
                dbf1A = descending
                ? dbf1A.OrderByDescending(s => s.Subvolume)
                : dbf1A.OrderBy(s => s.Subvolume);
            }

            return dbf1A;
        }


        // Sorts data with two grouping levels
        private static IEnumerable<FinalData> SortTwoLevel(List<FinalData> finalDataList, string sortBy, bool useYtdNumbers, bool descending, int howMany)
        {
            //figure out what top level groups to keep
            var dbf1A = GetTopLevel(finalDataList);

            if (sortBy.Equals("1") || sortBy.Equals("2") || sortBy.Equals("3"))
            {
                switch (sortBy)
                {
                    case "2": //Avg cost
                        dbf1A = SortTwoLevelByAveCost(finalDataList, useYtdNumbers, descending, dbf1A);
                        break;
                    case "3": //# of trips
                        dbf1A = SortTwoLevelByNumOfTrips(finalDataList, useYtdNumbers, descending, dbf1A);
                        break;
                    default: //volume booked
                        dbf1A = SortTwoLevelByVolume(finalDataList, useYtdNumbers, descending, dbf1A);
                        break;
                }

                if (howMany != 0) dbf1A = dbf1A.Take(howMany).ToList();

                return dbf1A;
            }
            else
            {
                return dbf1A;
            }
        }

        // Gets first grouping field
        private static string GetCatDesc1(FinalData row, ReportGlobals globals, IMasterDataStore store)
        {
            switch (globals.GetParmValue(WhereCriteria.GROUPBY))
            {
                case "4":
                case "5":
                case "6":
                    return AportLookup.LookupAport(store, row.Cat1, row.Mode, globals.Agency);
                default:
                    return LookupFunctions.LookupCountryName(row.Cat1, globals, store);
            }
        }

        // Gets second grouping field
        private static string GetCatDesc2(FinalData row, ReportGlobals globals, IMasterDataStore store)
        {
            switch (globals.GetParmValue(WhereCriteria.GROUPBY))
            {
                case "2":
                    return LookupFunctions.LookupCountryName(row.Cat2, globals, store);
                case "3":
                case "6":
                    return AportLookup.LookupAport(store, row.Cat2, row.Mode, globals.Agency);
                case "5":
                    return LookupFunctions.LookupAline(store, row.Cat2, row.Mode);
                default:
                    return string.Empty;
            }
        }

        private static IEnumerable<FinalData> GetTopLevel(IEnumerable<FinalData> finalDataList)
        {
            return finalDataList.GroupBy(s => new { s.Cat1, s.Cat2 }, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new FinalData
                {
                    Cat1 = key.Cat1,
                    Cat2 = key.Cat2,
                    Trips = reclist.Sum(s => s.Trips),
                    Ytdtrips = reclist.Sum(s => s.Ytdtrips),
                    Volume = reclist.Sum(s => s.Volume),
                    Ytdvolume = reclist.Sum(s => s.Ytdvolume),
                };
            })
                  .GroupBy(s => s.Cat1, (key, recs) =>
                  {
                      var reclist = recs.ToList();
                      return new FinalData
                      {
                          Cat1 = key,
                          Trips = reclist.Sum(s => s.Trips),
                          Volume = reclist.Sum(s => s.Volume),
                          Ytdtrips = reclist.Sum(s => s.Ytdtrips),
                          Ytdvolume = reclist.Sum(s => s.Ytdvolume),

                      };
                  })
                .Select(s => new FinalData
                {
                    Cat1 = s.Cat1,
                    Subtrips = s.Trips,
                    Subvolume = s.Volume,
                    Subytdtrps = s.Ytdtrips,
                    Subytdvol = s.Ytdvolume
                });
        }
    }
}
