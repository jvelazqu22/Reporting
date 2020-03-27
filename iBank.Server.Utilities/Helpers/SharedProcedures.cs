using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CODE.Framework.Core.Utilities;
using Domain.Helper;
using Domain.Interfaces;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Repository.SQL.Repository;

namespace iBank.Server.Utilities.Helpers
{
    public static class SharedProcedures
    {

        /// <summary>
        /// Strips out unwanted characters and delimits list items with ticks, upper case.
        /// Textlist(" foo,bar, foo bar,'genius',  sill(y)") returns "'FOO','BAR','FOO BAR','GENIUS','SILLY'"
        /// </summary>
        public static string TextList(string textList)
        {
            if (string.IsNullOrEmpty(textList)) return "";

            var s = Regex.Replace(textList.ToUpperInvariant().Trim(@" ,".ToCharArray()), @"["")(']", string.Empty).RemoveUnintentionalSpaces();
            return @"'" + Regex.Replace(s, ",", @"','") + @"'";
        }

        //******************************************************************************
        //** FUNCTION fixWildcard - REPLACES "*" AND "?" WILDCARD CHARACTER WITH THE
        //**   APPROPRIATE FOXPRO SQL "%" AND "_" WILD-CARDS.
        //**
        //** IT ALSO CONVERTS THE VALUE TO UPPER CASE.
        //**
        //** RETURNS:  THE "FIXED" VALUE WITH THE APPROPRIATE SQL OPERATOR BEFORE IT.
        //**           IF A WILDCARD IS PRESENT, THE OPERATOR IS "LIKE";
        //**           OTHERWISE THE OPERATOR IS "=".
        //** EXAMPLES:
        //**           value: ABC*    RETURN:  LIKE 'ABC%'
        //**           value: ABC     RETURN:  = 'ABC'
        //**
        //******************************************************************************
        public static string FixWildcard(string str)
        {
            //convert to uppercase
            str = str.ToUpper();
            
            str = str.ReplaceWildcards();

            //if the wildcards have already been processed just return
            if (str.Contains("LIKE") && str.HasWildCards() || str.Contains("=")) return str;

            //escape apostrophe in data value for SQL
            str = str.Replace("'", "''");

            //if there are wildcards and the LIKE clause has not already been added need to add it, otherwise add =
            str = str.HasWildCards() ? $" LIKE '{str}'" : $" = '{str}'";

            return str;
        }

        /// <summary>
        /// Takes a list of strings returns an OR clause that can be used by SQL in a Where clause. 
        /// Items are assumed to be of type String in the database. 
        /// </summary>
        /// <param name="inList">a list of strings</param>
        /// <param name="fieldName">The name of the field to search</param>
        /// <param name="notIn">If true, the resulting phrase will be a "Not" phrase (!("1" || "2" || "3"))</param>
        /// <param name="isNumeric">If true, the values in the list are numeric, so they won't have quotes around them.</param>
        /// <returns>the Or clause ("1" || "2" || "3")</returns>
        public static string OrList(List<string> inList, string fieldName, bool notIn = false, bool isNumeric = false)
        {
            if (!inList.Any()) return string.Empty;

            for (var i = 0; i < inList.Count; i++)
            {
                inList[i] = inList[i].Trim();
            }
            
            var whereString = string.Empty;
            if (inList.Count == 1)
            {
                inList[0] = FixWildcard(inList[0]);
            }
           
            for (var i = 0; i < inList.Count; i++)
            {
                if(isNumeric)
                {
                    whereString += $"{fieldName} = {inList[i]}";
                }
                else
                {
                    if (inList[i].Contains("LIKE") || inList[i].Contains("="))
                    {
                        whereString += $"{fieldName}  {inList[i]}";
                    }
                    else
                    {
                        whereString += $"{fieldName} = '{inList[i]}'";
                    }
                }

                if (i + 1 < inList.Count) whereString += " OR ";
            }

            return notIn ? "NOT(" + whereString + ")" : "(" + whereString + ")";
        }
        
        public static string DateToString(DateTime date, string country, bool globalDateFormat,string langCode, bool forceDate = false)
        {
            var timeOfDay = " " + date.ToShortTimeString();

            if (globalDateFormat)
            {
                return date.Day.ToString().PadLeft(2, '0') + GetShortMonthMl(date.Month, langCode) + date.Year + timeOfDay;
            }

            var intlSettingsQuery = new GetSettingsByCountryAndLangCodeQuery(new iBankMastersQueryable(), country, langCode);
            var intlSettings = intlSettingsQuery.ExecuteQuery();

            if (intlSettings == null) return date.ToShortDateString();

            var day = date.Day.ToString().PadLeft(2, '0');
            var month = date.Month.ToString().PadLeft(2, '0');
            var year = date.Year.ToString().PadLeft(4, '0');
            var dm = intlSettings.DateMark;
            switch (intlSettings.DateFormat.Trim())
            {
                case "AMERICAN": //MDY
                    return month + dm + day + dm + year + timeOfDay;
                case "ANSI": //YMD
                case "JAPAN":
                case "TAIWAN":
                    return year + dm + month + dm + day + timeOfDay;
                default: //DMY
                    return day + dm + month + dm + year + timeOfDay;
            }
        }

        public static string GetShortMonthMl(int month, string langCode)
        {
            if (month < 1 || month > 12) return string.Empty;
            
            var translations = new GetShortMonthMlTranslationsQuery(new iBankMastersQueryable(), langCode).ExecuteQuery();

            if (translations.Count != 12)
            {
                translations = new List<string>{"Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"};
            }

            return translations[month-1];
        }

        public static int GetMonthNum(string month)
        {
            month = month.Trim();
            month = month.Length > 3 ? month.Left(3) : month;

            var months = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            
            return months.IndexOf(month) + 1;
        }

        /// <summary>
        /// Saves the client's logo to a temp directory for use by the Crystal report. 
        /// </summary>
        /// <param name="logoName"></param>
        /// <param name="bytes">the bytes making up the picture</param>
        /// <returns>the randomly generated filename.</returns>
        public static string SaveTempLogo(string logoName, byte[] bytes)
        {
            var folder = ConfigurationManager.AppSettings["LogoTempDirectory"];
            var fileName = string.Format("{0}{1}_{2}", StringHelper.AddBS(folder), Guid.NewGuid(), logoName);

            File.WriteAllBytes(fileName, bytes);
            
            return fileName.Trim();
        }

        public static void DeleteOldFiles(string folder, DateTime threshold)
        {
            if (!Directory.Exists(folder)) throw new DirectoryNotFoundException($"Directory [{folder}] not found.");

            try
            {
                foreach (var file in Directory.GetFiles(folder).Where(file => File.GetCreationTime(file) < threshold))
                {
                    File.Delete(file);
                }
            }
            catch (Exception)
            {
                //swallow the exception
            }
        }

        private static readonly List<int> UserBreaks1 = new List<int> { 1, 2, 3, 4}; 
        private static readonly List<int> UserBreaks2 = new List<int> { 2, 3, 20,21 };
        private static readonly List<int> UserBreaks3 = new List<int> { 3, 4, 21, 30 };
        public static UserBreaks SetUserBreaks(int reportBreaks)
        {
            return new UserBreaks
            {
                UserBreak1 = UserBreaks1.Contains(reportBreaks),
                UserBreak2 = UserBreaks2.Contains(reportBreaks),
                UserBreak3 = UserBreaks3.Contains(reportBreaks)
            };
        }

        public static string ConvertTime(string time)
        {
            if (string.IsNullOrEmpty(time))  return new string(' ',8);

            time = time.Trim().PadLeft(5, '0');
            var hourString = time.Left(2);
            var minString = time.Right(2);

            int hour;
            if (!int.TryParse(hourString,out hour)) 
                throw new Exception("Invalid Hour Value " + hourString);

            int minute;
            if (!int.TryParse(minString, out minute))
            {
                //TODO: Found a record with 12:5P, which fails. 
                //throw new Exception("Invalid Minute Value " + minString);
                minute = 0;
            }
            var amPm = "am";
            if (hour == 12 && minute == 0)
            {
                amPm = "n";
            }
            else if (hour == 12 && minute > 0)
            {
                amPm = "pm";
            }
            else if (hour == 0 && minute == 0)
            {
                hour = 12;
                amPm = "mi";
            }
            else if (hour == 0 && minute > 0)
            {
                hour = 12;
                amPm = "am";
            }
            else if (hour > 12)
            {
                hour = hour - 12;
                amPm = "pm";
            }


            return string.Format("{0}:{1} {2}", hour.ToString().PadLeft(2, '0'), minute.ToString().PadLeft(2, '0'), amPm);
        }


        /// <summary>
        /// Converts the passed in time (as a string) to the respective TimeSpan; similar to ConvertTime function
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static TimeSpan ConvertTimeToTimeSpan(string time)
        {
            if (string.IsNullOrEmpty(time)) return new TimeSpan();

            time = time.Trim().PadLeft(5, '0');
            var hourString = time.Left(2);
            var minString = time.Right(2);

            int hour;
            if (!int.TryParse(hourString, out hour))
                throw new Exception("Invalid Hour Value " + hourString);

            int minute;
            if (!int.TryParse(minString, out minute)) minute = 0;

            return new TimeSpan(hours: hour, minutes: minute, seconds: 0);

        }

        /// <summary>
        /// Sometimes a column name comes in as a paramter, and needs to be checked to make sure it is a valid FoxPro column name. 
        /// This doesn't really apply anymore, but it can't hurt to clean up the name. 
        /// </summary>
        /// <param name="colname">The original column name</param>
        /// <returns>The "sanitized" version. </returns>
        private const string AllowedChars = "_ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static string FixDbfColumnName(string colname)
        {
            colname = colname.Replace("#", "NO")
                .Replace(" ","_")
                .Replace(".", "_")
                .Replace(",", "_")
                .Replace("&", "_")
                .Replace("'", "_")
                .Replace("-", "_");

            var temp = string.Empty;
            //* ALLOW ONLY: A-Z, a-z, 0-9, "_"
            foreach (var colChar in colname)
            {
                if (AllowedChars.Contains(colChar)) temp += colChar;
            }

            return temp.Length >10?temp.Left(10): temp;
        }

        /// <summary>
        /// Return the list of trip itineraries given a list of appropriate data. 
        /// </summary>
        /// <param name="riList"></param>
        /// <param name="legs">Set to true if using "leg" level, false if "segment" level.</param>
        /// <returns></returns>
        public static Dictionary<int, string> GetRouteItinerary(IEnumerable<IRouteItineraryInformation> riList, bool legs)
        {
            var result = new Dictionary<int, string>();

            //create an array to avoid multiple enumeration
            var routeItineraryInformations = riList as IRouteItineraryInformation[] ?? riList.ToArray();
            if (!routeItineraryInformations.Any()) return result;
            

            var trips = routeItineraryInformations.Select(s => s.RecKey).Distinct();

            foreach (var trip in trips)
            {
                var stops = routeItineraryInformations.Where(s => s.RecKey == trip);
                var itinerary = string.Empty;
                var priorDestination = string.Empty;
                foreach (var stop in stops)
                {
                    //** 06/29/2009 - AGENCIES WHO LOAD RAIL DATA THAT IS NOT THROUGH THE NEW  **
                    //** DATA CLEANSER MAY HAVE A NUMERIC CODE (FROM EVOLVI, FOR INSTANCE).    **
                    //** THE CODE COMES INTO iBank AS "701-VT", AND int(val("701-VT")) RETURNS **
                    //** THE NUMBER 701.  SO, WE HAVE TO FILTER FOR A HYPHEN.                  **
                    var isRail = false;
                    if (stop.Mode.EqualsIgnoreCase("R") && stop.Origin.IndexOf("E", StringComparison.OrdinalIgnoreCase) < 0 && stop.Origin.IndexOf("-", StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        int stationNumber;
                        isRail = int.TryParse(stop.Origin, out stationNumber);
                    }

                    var origin = stop.Origin.Trim();
                    var dest = stop.Destinat.Trim();
                    if (isRail)
                    {
                        
                        if ( !string.IsNullOrEmpty(origin) )
                        {
                            // checks to avoid ArgumentOutOfRangeException
                            if (origin.Length >= 4 && origin.Substring(3,1).Equals("-"))
                            {
                                origin = origin.Left(3);
                            }
                            if (dest.Length >= 4 && dest.Substring(3, 1).Equals("-"))
                            {
                                dest = dest.Left(3);
                            }
                        }
                    }
                    else
                    {
                        origin = origin.Left(3);
                        dest = dest.Left(3);
                    }

                    if (!origin.Equals(priorDestination)) itinerary += origin + " ";

                    if (!stop.Connect.EqualsIgnoreCase("X"))
                    {
                        itinerary += dest + " ";
                    }
                    else
                    {
                        if (legs) itinerary += dest.PadRight(3, ' ') + ".";
                    }

                    priorDestination = dest;
                    
                }
                result.Add(trip,itinerary);
            }

            return result;
        }
    }


}
