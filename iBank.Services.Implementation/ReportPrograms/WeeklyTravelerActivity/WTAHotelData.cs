using Domain.Models.ReportPrograms.WeeklyTravelerActivity;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Utilities.ClientData;

namespace iBank.Services.Implementation.ReportPrograms.WeeklyTravelerActivity
{
    public class WTAHotelData
    {
        public List<HotelData> GetHotelData(BuildWhere BuildWhere, int UdidNumber, bool IsPreview, ReportGlobals Globals, IClientQueryable ClientQueryableDb)
        {
            var whereClauseFull = string.IsNullOrWhiteSpace(BuildWhere.WhereClauseAdvanced)
                ? BuildWhere.WhereClauseFull
                : BuildWhere.WhereClauseFull + " and " + BuildWhere.WhereClauseAdvanced;

            var whereHotelClause = Regex.Replace(whereClauseFull, "depdate", "DateIn", RegexOptions.IgnoreCase);
            whereHotelClause = Regex.Replace(whereHotelClause, "arrdate", "DateOut", RegexOptions.IgnoreCase);

            string whereClause;
            string fromClause;

            if (UdidNumber == 0)
            {
                fromClause = IsPreview ? "ibtrips T1, ibHotel T5" : "hibtrips T1, hibHotel T5";
                whereClause = "T1.reckey = T5.reckey and " +
                              whereHotelClause;
            }
            else
            {
                fromClause = IsPreview
                    ? "ibtrips T1, ibHotel T5, ibudids T3"
                    : "hibtrips T1, hibHotel T5, hibudids T3";
                whereClause =
                    "T1.reckey = T5.reckey and T1.reckey = T3.reckey and T1.agency = T5.agency and T1.agency = T3.agency and " +
                    whereHotelClause;
            }
            var fieldList =
                @"T1.recloc, T1.reckey, invoice, invdate, ticket, depdate, arrdate, acct, break1, break2, break3, passlast, passfrst, DateIn, DateOut, HotCity, HotState, Metro ";
            var orderClause = "order by T1.reckey ";
            var fullSql = SqlProcessor.ProcessSql(fieldList, false, fromClause, whereClause, orderClause, Globals);
            
            return ClientDataRetrieval.GetUdidFilteredOpenQueryData<HotelData>(fullSql, Globals, BuildWhere.Parameters, IsPreview).ToList();
        }

        public List<LocationsData> SetHotelLocation(List<DayHotelsData> dayHotels, bool useAirportCodes, string alertList, List<LocationsData> dayLocs2, string[] DelimStrings, string DelimString)
        {
            for (var j = 0; j < 7; j++)
            {
                var dayHotel = dayHotels[j];

                var metroLoc = string.Empty;
                var metroLocs = dayHotel.MetroCode.Split(DelimStrings, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (metroLocs.Any())
                {
                    metroLoc = metroLocs[0];
                    metroLocs.RemoveAt(0);
                    dayHotels[j].MetroCode = string.Join(DelimString, metroLocs);
                }
                var cityLoc = string.Empty;
                var cityLocs = dayHotel.CityState.Split(DelimStrings, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (cityLocs.Any())
                {
                    cityLoc = cityLocs[0];
                    cityLocs.RemoveAt(0);
                    dayHotels[j].CityState = string.Join(DelimString, cityLocs);
                }

                string value = useAirportCodes ? metroLoc : cityLoc;
                if (!value.IsNullOrWhiteSpace())
                {
                    dayLocs2[j].CityOrMetro = "[h]" + value;
                    dayLocs2[j].Alert = alertList.Contains(metroLoc);
                }
            }

            return dayLocs2;
        }

        public List<DayHotelsData> SetHotelDaily(List<HotelData> hotelDataList, TripLevelData row, DateTime BeginDate)
        {
            /*
                HOTEL IS A LOT EASIER THAN AIR.  IF THE DATE IN QUESTION IS BETWEEN  
                THE DATEIN AND DATEOUT, THEN THE CITY GOES INTO THE LIST.  HOWEVER, 
                WE NEED AN ARRAY TO CARRY BOTH THE METRO CODE AND THE CITY/STATE,   
                SINCE WE NEED THE METRO CODE FOR THE "ALERT" LIST, AND WE MAY NEED  
                THE CITY/STATE, DEPENDING UPON WHETHER WE ARE DISPLAYING CODES OR   
                CITIES.
            */
            var dayHotels = new List<DayHotelsData>();
            for (var i = 0; i < 7; i++)
            {
                dayHotels.Add(new DayHotelsData { MetroCode = string.Empty, CityState = string.Empty });
            }

            var hotelData = hotelDataList.Where(s => s.RecKey == row.RecKey);
            foreach (var hotel in hotelData)
            {
                var metroCode = hotel.Metro.Trim();
                string cityState;
                if (!hotel.HotCity.IsNullOrWhiteSpace() && !hotel.HotState.IsNullOrWhiteSpace())
                {
                    cityState = hotel.HotCity.Trim() + "," + hotel.HotState.Trim();
                }
                else if (hotel.HotCity.IsNullOrWhiteSpace() && hotel.HotState.IsNullOrWhiteSpace())
                {
                    cityState = metroCode;
                }
                else
                {
                    cityState = hotel.HotCity.Trim() + hotel.HotState.Trim();
                }
                for (var i = 0; i < 7; i++)
                {
                    var date = BeginDate.AddDays(i);
                    /*
                                THE ORIGIN GOES INTO THE FIELD THE DEPARTURE DATE IS LESS THAN OR
                                EQUAL TO THE DATE IN QUESTION, AND THE ARRIVAL DATE IS NOT BEFORE
                                THE DATE.
                            */
                    if (date.IsBetween(hotel.DateIn, hotel.DateOut))
                    {
                        if (!dayHotels[i].MetroCode.Contains(metroCode))
                        {
                            dayHotels[i].MetroCode = dayHotels[i].MetroCode.IsNullOrWhiteSpace() ? metroCode : dayHotels[i].MetroCode + "|" + metroCode;
                        }
                        if (!dayHotels[i].CityState.Contains(cityState))
                        {
                            dayHotels[i].CityState = dayHotels[i].CityState.IsNullOrWhiteSpace() ? cityState : dayHotels[i].CityState + "|" + cityState;
                        }
                    }
                }
            }

            return dayHotels;
        }
    }
}
