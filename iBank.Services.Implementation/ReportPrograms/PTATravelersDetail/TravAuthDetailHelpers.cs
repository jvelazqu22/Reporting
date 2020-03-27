using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.PTATravelersDetailReport;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.PTARequestActivity;
using Domain.Orm.Classes;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Repository.SQL.Repository;

namespace iBank.Services.Implementation.ReportPrograms.PTATravelersDetail
{
    public static class TravAuthDetailHelpers
    {
        public static List<GroupedTripAuthData> ProcessOutOfPolicy(List<GroupedTripAuthData> finalDataList, ReportGlobals globals)
        {

            var rowsToKeep = new List<GroupedTripAuthData>();
            var oopCrit = globals.GetParmValue(WhereCriteria.INOOPCODES);
            if (string.IsNullOrEmpty(oopCrit)) return finalDataList;

            var oopCritList = oopCrit.Split(',');

            var notIn = globals.IsParmValueOn(WhereCriteria.NOTINOOPCODES);
            foreach (var row in finalDataList)
            {
                if (!string.IsNullOrEmpty(row.OutPolCods))
                {
                    var codes = row.OutPolCods.Split(',');
                    foreach (var code in codes)
                    {
                        if (notIn)
                        {
                            if (!oopCritList.Contains(code)) rowsToKeep.Add(row);
                        }
                        else
                        {
                            if (oopCritList.Contains(code)) rowsToKeep.Add(row);
                        }
                    }
                }
            }

            return rowsToKeep;
        }

        /// <summary>
        /// Extension method that converts a currency value to a string, using international symbols. 
        /// </summary>
        /// <param name="val"></param>
        /// <param name="intl">The international settings that hold the currency symbol, position, and decimal separator</param>
        /// <returns></returns>
        public static string CurrencyTransform(this decimal val, InternationalSettingsInformation intl)
        {
            var stringVal = val.ToString("C");

            if (intl.Symbol.Equals("$") && intl.Decimal.Equals(".") && intl.Position.Equals("L")) return stringVal;

            stringVal = stringVal.Replace("$", intl.Symbol);
            if (intl.Position.Equals("L")) stringVal = stringVal.Right(stringVal.Length - 1) + intl.Symbol;

            if (intl.Decimal.Equals(",")) stringVal = stringVal.Replace(".", ",");

            return stringVal;
        }

        public static List<SummaryFinalData> GetSummary(List<FinalData> finalDataList)
        {
            return
               finalDataList.GroupBy(
                   s => new { s.Reckey, s.Travauthno, Statusdesc = s.Statusdesc.PadRight(24), s.Airchg, s.Tottripchg },
                   (key, recs) =>
                   {
                       var reclist = recs.ToList();
                       return new
                       {
                           key.Reckey,
                           key.Travauthno,
                           key.Statusdesc,
                           key.Airchg,
                           key.Tottripchg,
                           CarCost = reclist.Sum(s => s.Carcost),
                           HotelCost = reclist.Sum(s => s.Hotelcost)
                       };
                   })
                   .GroupBy(s => s.Statusdesc.ToUpper().Equals("NOTIFICATION") ? "** Notification Only **" : s.Statusdesc, (key, recs) =>
                   {
                       var reclist = recs.ToList();
                       return new SummaryFinalData
                       {
                           Statusdesc = key,
                           Numtrips = reclist.Count,
                           Airchg = reclist.Sum(s => s.Airchg),
                           Carcost = reclist.Sum(s => s.CarCost),
                           Hotelcost = reclist.Sum(s => s.HotelCost),
                           Tottripchg = reclist.Sum(s => s.Tottripchg)
                       };
                   }).ToList();
        }

        public static List<SummaryFinalData> GetSummaryOnly(List<GroupedTripAuthData> groupedTripAuthData, List<RawData> rawDataList,List<CarRawData> carRawDataList,List<HotelRawData> hotelRawDataList, bool includeNotifyOnly,string userLanguage)
        {
            var authStatuses = new GetMiscParamListQuery(new iBankMastersQueryable(), "TRAVAUTHSTAT", userLanguage).ExecuteQuery().ToList();

            var dbf1A = groupedTripAuthData.GroupBy(s => new { s.SGroupNbr, s.AuthStatus }, (key, recs) => new SummaryTempData
            {
                SGroupNbr = key.SGroupNbr,
                AuthStatus = key.AuthStatus,
                NumTrips = recs.Count()
            }).ToList();

            var curAirCost = rawDataList.GroupBy(s => new { s.RecKey, s.TravAuthNo, s.AirChg }, (key, recs) => new
            {
                key.RecKey,
                key.TravAuthNo,
                key.AirChg,
                dummy = recs.Count()
            });

            var airTemp = groupedTripAuthData.Join(curAirCost, t => new { t.RecKey, t.TravAuthNo }, a => new { a.RecKey, a.TravAuthNo },
                (t, a) => new SummaryTempData
                {
                    SGroupNbr = t.SGroupNbr,
                    AuthStatus = t.AuthStatus,
                    AirChg = a.AirChg

                }).GroupBy(s => new { s.SGroupNbr, s.AuthStatus }, (key, recs) => new SummaryTempData
                {
                    SGroupNbr = key.SGroupNbr,
                    AuthStatus = key.AuthStatus,
                    AirChg = recs.Sum(s => s.AirChg)
                });
            dbf1A.AddRange(airTemp);

            var carTemp = groupedTripAuthData.Join(carRawDataList, t => new { t.RecKey, t.TravAuthNo }, a => new { a.RecKey, a.TravAuthNo },
                (t, a) => new SummaryTempData
                {
                    SGroupNbr = t.SGroupNbr,
                    AuthStatus = t.AuthStatus,
                    CarCost = a.Days * a.ABookRat

                }).GroupBy(s => new { s.SGroupNbr, s.AuthStatus }, (key, recs) => new SummaryTempData
                {
                    SGroupNbr = key.SGroupNbr,
                    AuthStatus = key.AuthStatus,
                    CarCost = recs.Sum(s => s.CarCost)
                });
            dbf1A.AddRange(carTemp);

            var hotelTemp = groupedTripAuthData.Join(hotelRawDataList, t => new { t.RecKey, t.TravAuthNo }, a => new { a.RecKey, a.TravAuthNo },
                (t, a) => new SummaryTempData
                {
                    SGroupNbr = t.SGroupNbr,
                    AuthStatus = t.AuthStatus,
                    HotelCost = a.Nights * a.Rooms * a.BookRate

                }).GroupBy(s => new { s.SGroupNbr, s.AuthStatus }, (key, recs) => new SummaryTempData
                {
                    SGroupNbr = key.SGroupNbr,
                    AuthStatus = key.AuthStatus,
                    HotelCost = recs.Sum(s => s.HotelCost)
                });
            dbf1A.AddRange(hotelTemp);

            var summaryDataList = dbf1A.Select(s => new SummaryFinalData
            {
                AuthStatus = s.AuthStatus,
                Statusdesc = PtaLookups.LookupAuthStatus(authStatuses, s.AuthStatus),
                Numtrips = s.NumTrips,
                Airchg = s.AirChg,
                Carcost = s.CarCost,
                Hotelcost = s.HotelCost,
                Tottripchg = s.TotTripChg
            }).ToList();


            if (includeNotifyOnly)
            {
                foreach (var row in summaryDataList.Where(s => s.AuthStatus.Equals("N")))
                {
                    row.Statusdesc = "** Notification Only **";
                }
            }
            else
            {
                summaryDataList.RemoveAll(s => s.AuthStatus.Equals("N"));
            }

            return summaryDataList.GroupBy(s => s.Statusdesc, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new SummaryFinalData
                {
                    Statusdesc = key,
                    Numtrips = reclist.Sum(s => s.Numtrips),
                    Airchg = reclist.Sum(s => s.Airchg),
                    Carcost = reclist.Sum(s => s.Carcost),
                    Hotelcost = reclist.Sum(s => s.Hotelcost),
                    Tottripchg = reclist.Sum(s => s.Airchg + s.Hotelcost + s.Carcost)
                };
            }).ToList();
        }

        public static string SafeLeft(this string val, int len)
        {
            return val.Length > len
                ? val.Left(len)
                : val;
        }
    }
}
