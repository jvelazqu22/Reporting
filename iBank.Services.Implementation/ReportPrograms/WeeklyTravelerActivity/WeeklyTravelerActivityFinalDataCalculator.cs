using Domain.Helper;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using Domain.Models.ReportPrograms.WeeklyTravelerActivity;

namespace iBank.Services.Implementation.ReportPrograms.WeeklyTravelerActivity
{
    public class WeeklyTravelerActivityFinalDataCalculator
    {
        private UdidCalculator _udidCalculator = new UdidCalculator();
        private WeeklyTravelerActivityData _weeklyTravelerActivitydata = new WeeklyTravelerActivityData();
        private WTAHotelData _wTAHotelData = new WTAHotelData();
        private WTAAirportRailAndLocationData _wTAAirportRailAndLocationData = new WTAAirportRailAndLocationData();

        public List<TripLevelData> GetTripLevelData(List<HotelData> hotelDataList, List<RawData> RawDataList)
        {
            List<TripLevelData> tripLevels =
                RawDataList.GroupBy(
                    s =>
                    new
                    {
                        s.RecKey,
                        s.RecLoc,
                        s.Ticket,
                        s.InvDate,
                        s.DepDate,
                        s.ArrDate,
                        s.Acct,
                        s.Break1,
                        s.Break2,
                        s.Break3,
                        s.PassLast,
                        s.PassFrst
                    },
                    (key, g) => new TripLevelData
                    {
                        Acct = key.Acct,
                        ArrDate = key.ArrDate,
                        Break1 = key.Break1,
                        Break2 = key.Break2,
                        Break3 = key.Break3,
                        DepDate = key.DepDate,
                        InvDate = key.InvDate,
                        PassFrst = key.PassFrst,
                        PassLast = key.PassLast,
                        RecKey = key.RecKey,
                        RecLoc = key.RecLoc,
                        Ticket = key.Ticket,
                        NumLegs = g.Count(),
                    }).OrderBy(l => l.PassLast).ThenBy(f => f.PassFrst).ThenBy(r => r.RecKey).ToList();

            if (hotelDataList.Count > 0)
            {
                //Add  hotel records to tripLevels if the hotel's RecKey isn't in the set (hotels without trip records, a.k.a. hotel-only records.
                var result =
                    hotelDataList.Select(s => new TripLevelData
                    {
                        Acct = s.Acct,
                        ArrDate = s.ArrDate,
                        Break1 = string.IsNullOrWhiteSpace(s.Break1) ? "NONE" : s.Break1,
                        Break2 = string.IsNullOrWhiteSpace(s.Break2) ? "NONE" : s.Break2,
                        Break3 = string.IsNullOrWhiteSpace(s.Break3) ? "NONE" : s.Break3,
                        DepDate = s.DepDate,
                        InvDate = s.InvDate,
                        PassFrst = s.PassFrst,
                        PassLast = s.PassLast,
                        RecKey = s.RecKey,
                        RecLoc = s.RecLoc,
                        Ticket = s.Ticket
                    }).Where(p => tripLevels.All(p2 => p2.RecKey != p.RecKey)).ToList();
                tripLevels.AddRange(result);
            }

            //need to order again to get the hotel data ordered in the list
            return tripLevels.OrderBy(x => x.PassLast).ThenBy(x => x.PassFrst).ThenBy(x => x.RecKey).ToList();
        }

        public FinalData CreateFinalDataObject(TripLevelData tripRow, List<LocationsData> dayLocs2, int udidNo, string udidLabel, string udidText, string CityAlert)
        {
            var break1 = tripRow.Break1.IsNullOrWhiteSpace() ? "" : tripRow.Break1.Trim();
            var break2 = tripRow.Break2.IsNullOrWhiteSpace() ? "" : tripRow.Break2.Trim();
            var break3 = tripRow.Break3.IsNullOrWhiteSpace() ? "" : tripRow.Break3.Trim();

            var backslash = " / ";
            var none = "NONE";
            var breaksField = tripRow.Acct.Trim() + backslash +
                              (break1.IsNullOrWhiteSpace() ? none : break1) + backslash +
                              (break2.IsNullOrWhiteSpace() ? none : break2) + backslash +
                              (break3.IsNullOrWhiteSpace() ? none : break3);

            return new FinalData
            {
                BreaksFld = breaksField,
                Day1Locn = dayLocs2[0].CityOrMetro,
                Day2Locn = dayLocs2[1].CityOrMetro,
                Day3Locn = dayLocs2[2].CityOrMetro,
                Day4Locn = dayLocs2[3].CityOrMetro,
                Day5Locn = dayLocs2[4].CityOrMetro,
                Day6Locn = dayLocs2[5].CityOrMetro,
                Day7Locn = dayLocs2[6].CityOrMetro,
                Loc1Flag = dayLocs2[0].Alert ? CityAlert : "",
                Loc2Flag = dayLocs2[1].Alert ? CityAlert : "",
                Loc3Flag = dayLocs2[2].Alert ? CityAlert : "",
                Loc4Flag = dayLocs2[3].Alert ? CityAlert : "",
                Loc5Flag = dayLocs2[4].Alert ? CityAlert : "",
                Loc6Flag = dayLocs2[5].Alert ? CityAlert : "",
                Loc7Flag = dayLocs2[6].Alert ? CityAlert : "",
                PassFrst = tripRow.PassFrst,
                PassLast = tripRow.PassLast,
                Reckey = tripRow.RecKey,
                RecLoc = tripRow.RecLoc,
                Ticket = tripRow.Ticket,
                InvDate = tripRow.InvDate ?? DateTime.MinValue,
                UdidNo = udidNo,
                UdidLabel = udidLabel,
                UdidText = udidText
            };
        }

        public List<FinalData> BuildReportData(List<HotelData> hotelDataList, bool includeHotel, List<UdidData> reportSettingsUdidDataList, 
            ReportGlobals Globals, List<RawData> RawDataList, DateTime BeginDate, List<UdidData> ReportSettingsUdidInfoList, CommaDelimitedStringCollection ReportSettingsUdidsString, 
            string[] DelimStrings, string DelimString, string CityAlert)
        {
            var dayHotels = new List<DayHotelsData>();
            var alertList = Globals.GetParmValue(WhereCriteria.TXTALERTCITIES);
            var tripLevels = GetTripLevelData(hotelDataList, RawDataList);
            var outputIsXlsOrCsv = (Globals.OutputFormat == DestinationSwitch.Xls || Globals.OutputFormat == DestinationSwitch.Csv);
            var useAirportCodes = Globals.IsParmValueOn(WhereCriteria.CBUSEAIRPORTCODES);
            var finalDataList = new List<FinalData>();

            foreach (var tripRow in tripLevels)
            {
                var row = tripRow;
                List<string> dayLocs = new List<string>(new[] { "", "", "", "", "", "", "" });
                List<string> dayLocsOriginal = new List<string>(new[] { "", "", "", "", "", "", "" }); // RAIL; ADDED TO HOLD origOrigin/ origDest.

                //modifies dayLocs and dayLocsOriginal
                _wTAAirportRailAndLocationData.SetAirportAndRailDaily(row, dayLocs, dayLocsOriginal, tripRow, BeginDate, RawDataList);

                if (includeHotel)
                {
                    dayHotels = _wTAHotelData.SetHotelDaily(hotelDataList, row, BeginDate);
                }

                var cntr = -1;
                do
                {
                    cntr++;
                    var udid = _udidCalculator.GetUdidsForTrip(row, cntr, outputIsXlsOrCsv, reportSettingsUdidDataList, ReportSettingsUdidsString, ReportSettingsUdidInfoList);

                    bool moreData =
                          dayLocs.Any(s => !s.IsNullOrWhiteSpace()) ||
                          dayHotels.Any(s => !s.CityState.IsNullOrWhiteSpace()) ||
                          dayHotels.Any(s => !s.MetroCode.IsNullOrWhiteSpace());

                    //Look for the Udid in the Hotels, Locations lists
                    if ((!moreData && udid.UdidText.IsNullOrWhiteSpace()) || cntr >= Constants.MaxUdids) break;

                    /*
                        THE ARRAY dayLocs2 WILL BE BUILT FOR EACH ITERATION OF THIS LOOP 
                        (UP TO 10).  WE ARE ALLOWING UP TO 10 LOCATIONS FOR ANY 1 DAY.   
                        11 / 10 / 2006 - IF WE ADD A LOCATION FOR THE AIRPORT IN THIS ITERATION
                        OF THE LOOP, THEN WE DON'T WANT TO ADD A HOTEL LOCATION.  WE'LL USE 
                        THE VARIABLE addedDayLoc TO KEEP TRACK OF THIS FOR US.             
                    */

                    //Only one location for each day is parsed in each loop. So, if a day has multiple locations,
                    //"SEA|DEN" for the first day of the week, the entire loop will execute for first "SEA," then "DEN."
                    var dayLocs2 = new List<LocationsData>();
                    var dayLocs2Original = new List<LocationsData>(); // RAIL;

                    //modifies dayLocs2 and dayLocs2Original
                    var addedAirportOrRailLocation = _wTAAirportRailAndLocationData.SetDayLocations2(dayLocs, dayLocsOriginal, dayLocs2, dayLocs2Original, alertList, DelimStrings, DelimString);

                    dayLocs2 = addedAirportOrRailLocation
                        ? _wTAAirportRailAndLocationData.SetAirportOrRailLocation(dayLocs2, useAirportCodes, dayLocs2Original, Globals)
                        : _wTAHotelData.SetHotelLocation(dayHotels, useAirportCodes, alertList, dayLocs2, DelimStrings, DelimString);

                    finalDataList.Add(CreateFinalDataObject(tripRow, dayLocs2, udid.UdidNo, udid.UdidLabel, udid.UdidText, CityAlert));
                } while (true);
            }

            return finalDataList;
        }
    }
}
