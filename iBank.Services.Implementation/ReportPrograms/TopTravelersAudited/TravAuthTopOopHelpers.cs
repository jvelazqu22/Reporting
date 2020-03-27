using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Models.ReportPrograms.TopTravelersAuditedReport;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.TopTravelersAudited
{
    public static class TravAuthTopOopHelpers
    {
        public static List<RawData> ProcessOutOfPolicy(List<RawData> rawDataList, ReportGlobals globals)
        {
            var rowsToKeep = new List<RawData>();
            var oopCrit = globals.GetParmValue(WhereCriteria.INOOPCODES);
            if (string.IsNullOrEmpty(oopCrit)) return rawDataList;

            var oopCritList = oopCrit.Split(',');

            var notIn = globals.IsParmValueOn(WhereCriteria.NOTINOOPCODES);
            foreach (var row in rawDataList)
            {
                if (!string.IsNullOrEmpty(row.OutPolCods))
                {
                    var codes = row.OutPolCods.Split(',');
                    foreach (var code in codes)
                    {
                        if (notIn)
                            if (!oopCritList.Contains(code))
                                rowsToKeep.Add(row);
                        else
                            if (oopCritList.Contains(code))
                                rowsToKeep.Add(row);
                    }
                }
            }

            return rowsToKeep;
        }

        public static List<FinalData> SortFinalData(List<FinalData> finalDatalist, ReportGlobals globals)
        {
            IOrderedEnumerable<FinalData> query;
            bool desc = globals.ParmValueEquals(WhereCriteria.RBSORTDESCASC, "1");
            switch (globals.GetParmValue(WhereCriteria.SORTBY))
            {
                case "2":
                    query = desc 
                        ? finalDatalist.OrderByDescending(s => s.Approved) 
                        : finalDatalist.OrderBy(s => s.Approved);
                    break;
                case "3":
                    query = desc
                        ? finalDatalist.OrderByDescending(s => s.Notifyonly)
                        : finalDatalist.OrderBy(s => s.Notifyonly);
                    break;
                case "4":
                    query = desc 
                        ? finalDatalist.OrderByDescending(s => s.Declined)
                        : finalDatalist.OrderBy(s => s.Declined);
                    break;
                case "5":
                    query = desc 
                        ? finalDatalist.OrderByDescending(s => s.Bookvolume)
                        : finalDatalist.OrderBy(s => s.Bookvolume);
                    break;
                case "6":
                    query = desc 
                        ? finalDatalist.OrderByDescending(s => s.Avgcost)
                        : finalDatalist.OrderBy(s => s.Avgcost);
                    break;
                default:
                    query = desc 
                        ? finalDatalist.OrderByDescending(s => s.Trips)
                        : finalDatalist.OrderBy(s => s.Trips);
                    break;
            }

            var howmany = globals.GetParmValue(WhereCriteria.HOWMANY).TryIntParse(0);
            return howmany == 0
                ? query.ToList()
                : query.Take(howmany).ToList();
        }

        public static List<RawData> ConvertHotelToRawData(List<HotelRawData> hotelRawDataList, List<RawData> rawDataList)
        {
            return rawDataList.Join(hotelRawDataList, r => r.RecKey, c => c.RecKey, (r, c) => new
            {
                r.RecKey,
                r.Passlast,
                r.Passfrst,
                r.AuthStatus,
                c.Nights,
                c.Rooms,
                c.Bookrate,
            })
            .GroupBy(s => new { s.RecKey, s.Passlast, s.Passfrst, s.AuthStatus }, (key, recs) => new RawData
            {
                RecKey = key.RecKey,
                Passlast = key.Passlast,
                Passfrst = key.Passfrst,
                AuthStatus = key.AuthStatus,
                BookVolume = recs.Sum(s => s.Nights * s.Rooms * s.Bookrate)
            }).ToList();

        }

        public static List<RawData> ConvertCarToRawData(List<CarRawData> carRawDataList, List<RawData> rawDataList )
        {
            return rawDataList.Join(carRawDataList, r => r.RecKey, c => c.RecKey, (r, c) => new
            {
                r.RecKey,
                r.Passlast,
                r.Passfrst,
                r.AuthStatus,
                c.Days,
                c.Abookrat
            })
            .GroupBy(s => new { s.RecKey, s.Passlast, s.Passfrst, s.AuthStatus }, (key, recs) => new RawData
            {
                RecKey = key.RecKey,
                Passlast = key.Passlast,
                Passfrst = key.Passfrst,
                AuthStatus = key.AuthStatus,
                BookVolume = recs.Sum(s => s.Days * s.Abookrat)
            }).ToList();
        }

        public static List<string> GetExportFields()
        {
            return new List<string>
            {
                "passname as Passenger_Name",
                "Approved",
                "NotifyOnly as Notification",
                "Declined",
                "Expired",
                "Trips as Number_of_Trips",
                "bookVolume as Total_Trip_Cost",
                "AvgCost as Average_Trip_Cost",
            };
        }

    }
}
