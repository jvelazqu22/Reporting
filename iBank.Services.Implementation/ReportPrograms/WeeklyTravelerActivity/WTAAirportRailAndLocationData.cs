using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Models.ReportPrograms.WeeklyTravelerActivity;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Linq;


using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.WeeklyTravelerActivity
{
    public class WTAAirportRailAndLocationData
    {
        public List<LocationsData> SetAirportOrRailLocation(List<LocationsData> dayLocs2, bool useAirportCodes, List<LocationsData> dayLocs2Original, ReportGlobals globals)
        {
            /*
                04/03/2007 - NEED TO PUT A "[r]" PREFIX IN FRONT OF THE     
                LOCATION IF IT IS A RAILROAD STATION INSTEAD OF AN AIRPORT. 

                06/12/2009 - RAIL: NEED TO INCLUDE THE MODE IF WE ARE DOING THE 
                AIRPORT/RRSTATIONS LOOKUP.  IF NOT DOING THAT LOOKUP, THEN WE   
                NEED TO USE THE origOrigin/origDest CODES.                      
            */
            for (var j = 0; j < 7; j++)
            {
                var rrPrefix = string.Empty;
                var mode = Constants.AirCode;
                var s = dayLocs2[j].CityOrMetro.Trim();

                if (s.Length == 6)
                {
                    rrPrefix = "[r]";
                    mode = Constants.RailMode;
                }
                else
                {
                    //WATCH OUT FOR "EXPONENTIAL".
                    var buffer = s;
                    if (!buffer.Contains("E"))
                    {
                        var n = buffer.TryIntParse(0);
                        if (n > 0)
                        {
                            rrPrefix = "[r]";
                            mode = Constants.RailMode;
                        }
                    }
                }

                if (useAirportCodes)
                {
                    if (mode == Constants.RailMode)
                    {
                        dayLocs2[j].CityOrMetro = rrPrefix + dayLocs2Original[j].CityOrMetro;
                    }
                    else
                    {
                        dayLocs2[j].CityOrMetro = rrPrefix + dayLocs2[j].CityOrMetro;
                    }
                }
                else
                {
                    dayLocs2[j].CityOrMetro = rrPrefix + AportLookup.LookupAport(new MasterDataStore(), dayLocs2[j].CityOrMetro, mode, globals.Agency);
                }
            }

            return dayLocs2;
        }

        public bool SetDayLocations2(List<string> dayLocs, List<string> dayLocsOriginal, List<LocationsData> dayLocs2, List<LocationsData> dayLocs2Original, string alertList, string[] DelimStrings, string DelimString)
        {
            bool addedAirportOrRailLocation = false;
            for (var i = 0; i < 7; i++)
            {
                //Parse the first location for this day for processing below, and remove it from the string of
                //all locations for this day in preparation for the next loop.
                var loc = dayLocs[i];
                var locs = loc.Split(DelimStrings, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (locs.Any())
                {
                    loc = locs[0];
                    locs.RemoveAt(0);
                    dayLocs[i] = string.Join(DelimString, locs);
                }

                //Array from delimited string of locations for the day, used for Rail.
                var locOriginal = string.Empty;
                var locsOriginal = dayLocsOriginal[i].Split(DelimStrings, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (locsOriginal.Any())
                {
                    locOriginal = locsOriginal[0];
                    locsOriginal.RemoveAt(0);
                    dayLocsOriginal[i] = string.Join(DelimString, locsOriginal);
                }

                //2ND COLUMN OF ARRAY FLAGS "ALERT" CITIES.
                if (loc.IsNullOrWhiteSpace())
                {
                    dayLocs2.Add(new LocationsData());
                    dayLocs2Original.Add(new LocationsData());
                }
                else
                {
                    dayLocs2.Add(new LocationsData { CityOrMetro = loc, Alert = alertList.Contains(loc) });
                    dayLocs2Original.Add(new LocationsData { CityOrMetro = locOriginal, Alert = alertList.Contains(locOriginal) });
                    addedAirportOrRailLocation = true;
                }
            }
            return addedAirportOrRailLocation;
        }

        public void SetAirportAndRailDaily(TripLevelData row, List<string> dayLocs, List<string> dayLocsOriginal, TripLevelData tripRow, DateTime BeginDate, List<RawData> RawDataList)
        {
            /*
                THIS IS REALLY DIFFICULT, LOGICALLY.  HERE'S THE APPROACH:  FOR EACH    
                TRIP, WE'LL WALK THROUGH THE LEGS AND, FOR EACH OF THE 7 DAYS, IF IT IS 
                DETERMINED  THAT THE GUY IS AT THE PLACE OF ORIGIN FOR THAT DAY, THEN   
                WE WE'LL ADD  IT DO THE LIST, UNLESS IT'S ALREADY IN THE LIST.  WE'LL   
                DO THE SAME FOR DESTINATIONS.  WE'LL USE THE AIRPORT CODES IN THE LIST, 
                FOR NOW, AND SEPARATE THEM USING KEEP THE PIPE ("|") CHARACTER AS       
                DELIMITER, SO WE CAN RIP IT APART LATER.  AT THAT TIME, WE CAN ALSO     
                CHECK EACH AIRPORT CODE AGAINST THE "ALERT" LIST (IF IT EXISTS), AND    
                FLAG THAT LOCATION, AND AT THE SAME TIME DE-CODE IT TO A CITY/STATE, IF 
                NECESSARY.                                                              
            */

            var count = 0;
            var prevDest = string.Empty;
            var prevOrigDest = string.Empty;
            var prevArr = BeginDate.AddDays(100);
            var rawData = RawDataList.Where(s => s.RecKey == row.RecKey);

            foreach (var rawDataRow in rawData)
            {
                count++;
                var origin = rawDataRow.Origin.Trim();
                var destination = rawDataRow.Destinat.Trim();
                var originalOrigin = rawDataRow.OrigOrigin.Trim(); //Rail-related
                var originalDestination = rawDataRow.OrigDest.Trim(); //Rail-related
                for (var i = 0; i < 7; i++)
                {
                    var currentWorkedDate = BeginDate.AddDays(i);
                    /*
                            THE ORIGIN GOES INTO THE FIELD IF THE DEPARTURE DATE IS LESS THAN OR
                            EQUAL TO THE DATE IN QUESTION, AND THE ARRIVAL DATE IS NOT BEFORE THE DATE.                                                         
                        */
                    if (rawDataRow.RDepDate <= currentWorkedDate && rawDataRow.RArrDate >= currentWorkedDate)
                    {
                        if (!dayLocs[i].Contains(origin))
                        {
                            dayLocs[i] = dayLocs[i].IsNullOrWhiteSpace() ? origin : dayLocs[i] + "|" + origin;
                            dayLocsOriginal[i] = dayLocsOriginal[i].IsNullOrWhiteSpace() ? originalOrigin : dayLocsOriginal[i] + "|" + originalOrigin;
                        }
                    }

                    /*
                            THE DESTINATION GOES INTO THE FIELD IF THE ARRIVAL IS BEFORE OR ON 
                            THE DATE IN QUESTION AND IT IS THE LAST LEG, OR THE GUY ARRIVED ON 
                            THE DATE IN QUESTION.  OTHERWISE, WE ASSUME THE ORIGIN OF THE NEXT 
                            LEG WILL PICK IT UP.  ONLY DO THIS IF THE TRIP HASN'T ENDED ON A  
                            PREVIOUS DAY.
                        */
                    if (((rawDataRow.RArrDate <= currentWorkedDate && count == tripRow.NumLegs) || rawDataRow.RArrDate == currentWorkedDate)
                        && currentWorkedDate <= tripRow.ArrDate)
                    {
                        if (!dayLocs[i].Contains(destination))
                        {
                            dayLocs[i] = dayLocs[i].IsNullOrWhiteSpace() ? destination : dayLocs[i] + "|" + destination;
                            dayLocsOriginal[i] = dayLocs[i].IsNullOrWhiteSpace() ? originalDestination : dayLocsOriginal[i] + "|" + originalDestination;
                        }
                    }

                    /*
                             IF THE CURRENT DAY HAS NOTHING IN IT, AND A PREVIOUS DESINATION  
                             EXISTS, THEN PLUG IT IN.   ONLY DO THIS IF THE TRIP HASN'T ENDED 
                             ON APREVIOUS DAY.                                                
                        */
                    if (dayLocs[i].IsNullOrWhiteSpace() && !prevDest.IsNullOrWhiteSpace() && prevArr <= currentWorkedDate && rawDataRow.RDepDate > currentWorkedDate
                        && currentWorkedDate <= tripRow.ArrDate)
                    {
                        dayLocs[i] = dayLocs[i].IsNullOrWhiteSpace() ? prevDest : dayLocs[i] + "|" + prevDest;
                        dayLocsOriginal[i] = dayLocsOriginal[i].IsNullOrWhiteSpace() ? prevOrigDest : dayLocsOriginal[i] + "|" + prevOrigDest;
                    }
                }
                prevDest = destination;
                prevOrigDest = originalDestination;
                prevArr = rawDataRow.RArrDate.GetValueOrDefault();
            }
        }
    }
}
