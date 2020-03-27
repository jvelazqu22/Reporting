using Domain.Models.ReportPrograms.TravelerbyCountryReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TravelerByCountry
{
    public class TravByCountryDataProcessor
    {

        public List<RawData> ProcessDestinations(List<RawData> rawDataList, bool includeOneWay)
        {
            var Destinations = new List<RawData>();
            var segmentCounter = 0;
            var segmentsForRecKeys = rawDataList.GroupBy(s => s.RecKey, (k, g) => new { RecKey = k, Count = g.Count() }).ToList();
            var origin = rawDataList.Count > 0 ? rawDataList.First().Origin : string.Empty;
            var previousRecord = new RawData();
            foreach (var item in rawDataList)
            {
                int totalSegmentsForCurrentRecKey;
                if (previousRecord.RecKey == item.RecKey)
                {
                    /**WE ARE ON THE SAME TRIP AS THE PREVIOUS ITERATION OF THE LOOP. **

                **WE'RE NOT ON THE FIRST SEGMENT OF A TRIP.  AS LONG AS WE ARE NOT ON **
                * *THE LAST SEGMENT, THEN WE NEED TO INSERT A ROW.  THE LAST SEGMENT**
                ** SHOULD BE THE RETURN SEGMENT --WE DON'T WANT THAT.                 **/
                    var record = segmentsForRecKeys.FirstOrDefault(s => s.RecKey == previousRecord.RecKey);
                    totalSegmentsForCurrentRecKey = record != null ? record.Count : 0;

                    if (totalSegmentsForCurrentRecKey > segmentCounter)
                    {
                        var days = (item.RDepDate.GetValueOrDefault() - previousRecord.RArrDate.GetValueOrDefault()).Days;
                        if (days == 0)
                            days = 1;

                        if (previousRecord.Plusmin < 0)
                            days = 0 - days;

                        var oneWayTrip = false;
                        if (totalSegmentsForCurrentRecKey == 1)
                        {
                            oneWayTrip = true;
                            days = 0;
                        }

                        AddRecord(Destinations, previousRecord, days, oneWayTrip);
                    }
                }
                else
                {
                    /** WE ARE ON THE FIRST SEGMENT OF A NEW TRIP.  THIS IS THE **
                ** OUTBOUND TRIP, SO WE DO NOT WANT TO INSERT A ROW YET.   **
                ** ALTERNATIVELY, WE ARE ON A ONE-WAY TRIP, IN WHICH CASE  **
                ** WE STILL DO NOT WANT TO INSERT A ROW.                   **

                ** 09/10/2009 - THEY WANT ONE-WAY TRIPS AS AN OPTION.  SO IF **
                ** THERE IS ONLY 1 SEGMENT ON THIS TRIP, WE WANT TO ADD IT.  **
                
                ** IMPORTANT TO NOTE: EVEN THOUGH THE RECORD POINTER IS ON THE FIRST **
                ** SEGMENT OF A NEW TRIP, THE MEMORY VAR'S CONTAIN DATA FOR THE      **
                ** PREVIOUS RECORD -- I.E., THE LAST LEG OF THE PREVIOUS TRIP.       **/
                    //Lookup the total segments for the current reckey
                    var record = segmentsForRecKeys.FirstOrDefault(s => s.RecKey == previousRecord.RecKey);
                    totalSegmentsForCurrentRecKey = record != null ? record.Count : 0;

                    if (includeOneWay && totalSegmentsForCurrentRecKey == 1)
                    {
                        AddRecord(Destinations, previousRecord, 0, true);
                    }
                    else
                    {
                        /** 09/10/2009 - WE'RE GOING AFTER THE LAST SEGMENT IF IT **
                    ** DOES NOT RETURN THE TRAVELER TO HIS ORIGINAL ORIGIN.  **/

                        if (origin != previousRecord.Destinat && previousRecord.RecKey != 0)
                        {
                            if (totalSegmentsForCurrentRecKey > 1)
                            {
                                AddRecord(Destinations, previousRecord, 0, false);
                            }
                        }
                    }

                    segmentCounter = 0;
                    origin = item.Origin;
                }

                previousRecord = item;
                //Increment the segment counter
                segmentCounter++;
            }

            /** 11/29/2010 -- NEED TO ADD THE LAST SEGMENT, IF WE DIDN'T ALREADY ADD IT **
            ** BY INCLUDING ONE-WAYS, AND IF IT DOES NOT RETURN THE TRAVELER TO HIS    **
            ** ORIGINAL ORIGIN.  NOTE: days = 0 BECAUSE HE'S NOT LEAVING.              **/
            var lastItem = rawDataList.Last();

            var segRecord = segmentsForRecKeys.FirstOrDefault(s => s.RecKey == lastItem.RecKey);
            var totalSegments = segRecord != null ? segRecord.Count : 0;

            if (origin != lastItem.Destinat && (includeOneWay || totalSegments > 1))
            {
                AddRecord(Destinations, lastItem, 0, false);
            }
            return Destinations;
        }

        public List<FinalData> GroupFinalData(List<RawData> destinations, IMasterDataStore masterStore, ReportGlobals globals)
        {
            var finalDataList = destinations.Select( s => new
                    {
                        Country = LookupFunctions.LookupCountry(masterStore, s.Destinat, false, s.Mode, globals),
                        s.Passfrst,
                        s.Passlast,
                        s.Plusmin,
                        s.Days,
                        s.OneWayTrip
                    })
                .GroupBy(s => new { s.Country, s.Passlast, s.Passfrst }, (k, g) => new FinalData
                {
                    Country = k.Country,
                    Passlast = k.Passlast,
                    Passfrst = k.Passfrst,
                    Tickets = g.Sum(t => t.OneWayTrip ? 0 : t.Plusmin ?? 0),
                    Dispticks = g.Sum(t => t.Plusmin ?? 0),
                    Totdays = g.Sum(t => t.Days),
                    Longstay = g.Max(t => t.Days)
                }).Where(s => s.Dispticks >= 0).ToList();

            var groupedData = finalDataList.GroupBy(s => s.Country, (k, g) => new FinalData
            {
                Country = k,
                Ctryticks = g.Sum(t => t.Dispticks),
                Ctrydays = g.Sum(t => t.Totdays),
                Ctrylong = g.Max(t => t.Longstay)
            });

           finalDataList = finalDataList.Join(groupedData, s => s.Country, t => t.Country,
                    (s, t) => new FinalData
                    {
                        Country = s.Country,
                        Passfrst = s.Passfrst,
                        Passlast = s.Passlast,
                        Tickets = s.Tickets,
                        Dispticks = s.Dispticks,
                        Totdays = s.Totdays,
                        Longstay = s.Longstay,
                        Ctryticks = t.Ctryticks,
                        Ctrydays = t.Ctrydays,
                        Ctrylong = t.Ctrylong,
                    }).OrderBy(s => s.Country)
                    .ThenBy(s => s.Passlast)
                    .ThenBy(s => s.Passfrst).ToList();
            return finalDataList;
        }

        private void AddRecord(List<RawData> destinations, RawData record, int days, bool oneWay)
        {
            record.Days = days;
            record.OneWayTrip = oneWay;
            destinations.Add(record);
        }
    }
}