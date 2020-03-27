using System;
using System.Linq;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models.ReportPrograms.TopBottomTravelerAir;
using System.Collections.Generic;

using iBank.Repository.SQL.Repository;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomTravelerAir
{
    public class TopTravAirDataProcessor
    {
        // Transforms data to FinalData
        public IList<FinalData> ConvertRawDataToFinalData(IList<RawData> rawDataList, ReportGlobals globals,
            IMasterDataStore masterStore, bool useHomeCountry, bool isReservationReport)
        {
            var sourceAbbrs = rawDataList.Select(s => s.SourceAbbr.Trim()).Distinct();
            var hccLookups = sourceAbbrs.Select(s => new Tuple<string, string>(s, LookupFunctions.LookupHomeCountryCode(s, globals, masterStore))).ToList();

            // Convert raw data to MidData. Performs initial calculations
            var midList = rawDataList.Select(s => new MidData
            {
                Passlast = s.Passlast.Trim(),
                Passfrst = s.Passfrst.Trim(),
                SourceAbbr = s.SourceAbbr.Trim(),
                CtryCode = useHomeCountry ? SpeedLookup.Lookup(s.SourceAbbr, hccLookups) : "NA",
                Plusmin = s.Plusmin,
                Airchg = s.Airchg,
                Invdate = s.Invdate,
                Bookdate = s.Bookdate,
                Depdate = s.Depdate,
                Offrdchg = s.Offrdchg > 0 && s.Airchg < 0
                   ? 0 - s.Offrdchg
                   : s.Offrdchg == 0 ? s.Airchg : s.Offrdchg,
                BkDays = s.Plusmin * (isReservationReport ? (s.Depdate.GetValueOrDefault() - s.Bookdate.GetValueOrDefault()).Days : (s.Depdate.GetValueOrDefault() - s.Invdate.GetValueOrDefault()).Days),
                LostAmt = s.Airchg - s.Offrdchg
            }).ToList();

            // Filters list by home country
            midList = FilterForHomeCountry(midList, globals);

            if (useHomeCountry) midList = ProcessHomeCountry(midList);
            // Makes a reference list of country codes
            var countryCodes = useHomeCountry ? midList.Select(s => s.CtryCode.Trim()).Distinct() : new List<string>();
            var ccLookups = useHomeCountry ? countryCodes.Select(s => new Tuple<string, string>(s, LookupFunctions.LookupCountryName(s, globals, masterStore))).ToList() : new List<Tuple<string, string>>();

            // Groups records by passenger name, totals trips, volume booked,
            // lost amount and advance purchase time
            var finalDataList = midList.GroupBy(s => new { s.Passlast, s.Passfrst, s.CtryCode }, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new FinalData
                {
                    Passlast = key.Passlast,
                    Passfrst = key.Passfrst,
                    Homectry = useHomeCountry ? SpeedLookup.Lookup(key.CtryCode, ccLookups) : key.CtryCode,
                    Amt = reclist.Sum(s => s.Airchg),
                    Trips = reclist.Sum(s => s.Plusmin),
                    Lostamt = reclist.Sum(s => s.LostAmt),
                    Totbkdays = reclist.Sum(s => s.BkDays)
                };
            }).ToList();

            return finalDataList;
        } //End Convert

        private List<MidData> ProcessHomeCountry(List<MidData> midList)
        {
            //IF THERE ARE MULTIPLE HOME COUNTRIES FOR A TRAVELER, WE'RE GOING TO **
            //ASSIGN THE MOST FREQUENTLY USED ONE TO THE TRAVELER.   
            var temp1 = midList.GroupBy(s => new { Passlast = s.Passlast.Trim(), Passfrst = s.Passfrst.Trim(), CtryCode = s.CtryCode.Trim() }, (key, recs) => new
            {

                key.Passlast,
                key.Passfrst,
                key.CtryCode,
                TripCount = recs.Count()
            })
            .OrderBy(s => s.Passlast)
            .ThenBy(s => s.Passfrst)
            .ThenBy(s => s.CtryCode)
            .ThenBy(s => s.TripCount);

            var temp2 = midList.GroupBy(s => new { s.Passlast, s.Passfrst }, (key, recs) => new
            {

                key.Passlast,
                key.Passfrst,
                TripCount = recs.Count()
            }).Where(s => s.TripCount > 0);

            foreach (var item in temp2)
            {
                var temp = temp1.FirstOrDefault(s => s.Passfrst.Equals(item.Passfrst) && s.Passlast.Equals(item.Passlast));
                if (temp != null)
                {
                    foreach (var row in midList.Where(s => s.Passfrst.Equals(temp.Passfrst) && s.Passlast.Equals(temp.Passlast)))
                    {
                        row.CtryCode = temp.CtryCode;
                    }
                }
            }
            return midList;
        } // End Process Home


        // Filters list based on home country criteria
        public static List<MidData> FilterForHomeCountry(List<MidData> midList, ReportGlobals globals)
        {
            var homeCountry = globals.GetParmValue(WhereCriteria.HOMECTRY);
            homeCountry += globals.GetParmValue(WhereCriteria.INHOMECTRY);
            if (!string.IsNullOrEmpty(homeCountry))
            {
                var notIn = globals.IsParmValueOn(WhereCriteria.NOTINHOMECTRY);
                var notInText = notIn ? "not " : string.Empty;

                var homeCountries = homeCountry.Split(',');
                var saList = homeCountries.Select(s => LookupFunctions.LookupHomeCountry(s, globals, new MasterDataStore()));

                midList = midList.Where(s => saList.Contains(s.SourceAbbr)).ToList();

                globals.WhereText = globals.WhereText += " Home Country " + notInText + " = " + homeCountry;
            }

            return midList;
        }

        // Creates report document and sets necessary parameters
        public ReportDocument SetUpReportSource(string reportFilePath, List<FinalData> finalDataList, int totCount,
            decimal totCharge, decimal totBkDays, decimal totLost, bool includeLostSavings, string reportTitle)
        {
            var reportSource = new ReportDocument();

            reportSource.Load(reportFilePath);

            reportSource.SetDataSource(finalDataList);

            reportSource.SetParameterValue("nTotCnt", totCount);
            reportSource.SetParameterValue("nTotChg", totCharge);
            reportSource.SetParameterValue("nTotBkDays", totBkDays);
            reportSource.SetParameterValue("nTotLost", totLost);
            reportSource.SetParameterValue("lLogGen1", includeLostSavings);
            reportSource.SetParameterValue("rptTitle", reportTitle);

            return reportSource;
        }

    } // End Class
}
