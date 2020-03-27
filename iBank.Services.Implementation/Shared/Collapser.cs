using Domain.Interfaces;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper;
using Domain;
using Domain.Extensions;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace iBank.Services.Implementation.Shared
{
    public static class Collapser<T> where T : class, ICollapsible, new()
    {
        public enum CollapseType
        {
            /// <summary>
            /// Depart
            /// </summary>
            First,
            /// <summary>
            /// Arrival
            /// </summary>
            Last,
            Both,
            LongestMile,
            DepartAndArrive
        }

        public static List<T> Collapse(IList<T> dataToCollapse, CollapseType collapseType)
        {
           return Collapse(dataToCollapse: dataToCollapse,
                                collapseType: collapseType,
                                useClassCatHierarchy: false,
                                store: null,
                                agency: "",
                                isMileageTable: false,
                                isSegFareMileage: false);            
        }

        public static List<T> Collapse(IList<T> dataToCollapse, CollapseType collapseType, bool isMileageTable, bool isSegFareMileage)
        {
            return  Collapse(dataToCollapse: dataToCollapse,
                        collapseType: collapseType,
                        useClassCatHierarchy: false,
                        store: null,
                        agency: "",
                        isMileageTable: isMileageTable,
                        isSegFareMileage: isSegFareMileage);
            
        }

        public static List<T> CollapseUsingClassHierarchy(IList<T> dataToCollapse, CollapseType collapseType, bool useClassCatHierarchy,
                                                          IClientDataStore store, string agency)
        {
            return Collapse(dataToCollapse: dataToCollapse,
                            collapseType: collapseType,
                            useClassCatHierarchy: useClassCatHierarchy,
                            store: store,
                            agency: agency,
                            isMileageTable: false,
                            isSegFareMileage: false);            
        }


        ///dataToCollapase should be sorted by reckey and seqno, otherwise it may not be collapsed correctly.
        ///if both isSegFareMileage and iMileageTable are used, we need to lookup each leg mileage from AirMileage table first then sum the miles 
        ///if only isSegFareMileage is used, we need to sum the miles first, then lookup the mileage from AirMileage table.
        ///otherwise, just sum the miles up when collapse
        ///Use DeepClone so return List is a copy not a reference of partial or full of original object
        public static List<T> Collapse(IList<T> dataToCollapse, CollapseType collapseType, bool useClassCatHierarchy,
            IClientDataStore store, string agency, bool isMileageTable, bool isSegFareMileage)
        {
            var results = new List<T>();

            var dict = SetSegCounter(dataToCollapse);
            foreach (var trip in dict.Values)
            {
                if (trip.Count == 1 && trip.First().Connect != null && trip.First().Connect.Equals("X")) continue;

                var segNumbers = trip.Select(x => x.Seg_Cntr).Distinct().OrderBy(x => x);

                foreach (var seg in segNumbers)
                {
                    var legsInSegRef = trip.Where(x => x.Seg_Cntr == seg).OrderBy(x => x.SeqNo).ToList();
                    var legsInSegment = legsInSegRef.Select(x => x.DeepClone()).ToList();

                    var totalLegsInSegment = legsInSegment.Count;
                    if (totalLegsInSegment == 1)
                    {
                        if (isMileageTable)
                        {
                            AirMileageCalculator<T>.CalculateAirMileageFromTable(legsInSegment);
                        }
                        results.Add(legsInSegment[0]);
                        continue;
                    }
                    var departureLeg = legsInSegment[0];
                    var arrivalLeg = legsInSegment[totalLegsInSegment - 1];
                    if (isMileageTable && isSegFareMileage)
                    {
                        AirMileageCalculator<T>.CalculateAirMileageFromTable(legsInSegment);
                    }
                    var totalMilesInSegment = legsInSegment.Sum(x => x.Miles);
                    var totalSegmentFare = legsInSegment.Sum(s => s.ActFare);
                    var totalAirCo2 = legsInSegment.Sum(s => s.AirCo2);
                    var totalAltCarCo2 = legsInSegment.Sum(s => s.AltCarCo2);
                    var totalAltRailCo2 = legsInSegment.Sum(s => s.AltRailCo2);

                    T legToAdd;
                    switch (collapseType)
                    {
                        case CollapseType.First:
                            legToAdd = CollapserMapper<T>.MapDepartureLeg(departureLeg, arrivalLeg, totalMilesInSegment);
                            break;
                        case CollapseType.Last:
                        case CollapseType.Both:
                            legToAdd = CollapserMapper<T>.MapArrivalLeg(arrivalLeg, departureLeg, totalMilesInSegment, totalSegmentFare, totalAirCo2);
                            break;
                        case CollapseType.LongestMile:
                            legToAdd = CollapserMapper<T>.MapLongestMileLeg(arrivalLeg, departureLeg, totalMilesInSegment, totalSegmentFare, totalAirCo2);
                            break;
                        case CollapseType.DepartAndArrive:
                            legToAdd = CollapserMapper<T>.MapDepartAndArriveLeg(arrivalLeg, departureLeg, totalMilesInSegment, totalSegmentFare, totalAirCo2, totalAltCarCo2, totalAltRailCo2);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(collapseType), collapseType, null);
                    }

                    legToAdd.DitCode = CollapserHelper.GetDomesticInternationalType(legsInSegment);

                    if (useClassCatHierarchy)
                    {
                        var query = new GetAllClassCategoriesQuery(store.ClientQueryDb, agency);
                        legToAdd = CollapserMapper<T>.RemapAccordingToClassHierarchy(query, legsInSegment, legToAdd);
                    }
                    results.Add(legToAdd);
                }
            }
            return results;
        }


        ///dataToCollapase should be sorted by reckey and seqno, otherwise it may not be collapsed correctly.
        ///if both isSegFareMileage and iMileageTable are used, we need to lookup each leg mileage from AirMileage table first then sum the miles 
        ///if only isSegFareMileage is used, we need to sum the miles first, then lookup the mileage from AirMileage table.
        ///otherwise, just sum the miles up when collapse
        public static List<T> CollapseReference(IList<T> dataToCollapse, CollapseType collapseType, bool useClassCatHierarchy,
                    IClientDataStore store, string agency, bool isMileageTable, bool isSegFareMileage) 
        {
            var results = new List<T>();

            var dict = SetSegCounter(dataToCollapse);
            foreach (var trip in dict.Values)
            {
                if (trip.Count == 1 && trip.First().Connect != null && trip.First().Connect.Equals("X")) continue;
                
                var segNumbers = trip.Select(x => x.Seg_Cntr).Distinct().OrderBy(x => x);

                foreach (var seg in segNumbers)
                {
                    var legsInSegment = trip.Where(x => x.Seg_Cntr == seg).OrderBy(x => x.SeqNo).ToList();
                    var totalLegsInSegment = legsInSegment.Count;
                    if (totalLegsInSegment == 1)
                    {
                        if (isMileageTable)
                        {
                            AirMileageCalculator<T>.CalculateAirMileageFromTable(legsInSegment);
                        }
                        results.Add(legsInSegment[0]);
                        continue;
                    }
                    var departureLeg = legsInSegment[0];
                    var arrivalLeg = legsInSegment[totalLegsInSegment - 1];
                    if (isMileageTable && isSegFareMileage)
                    {
                        AirMileageCalculator<T>.CalculateAirMileageFromTable(legsInSegment);
                    }
                    var totalMilesInSegment = legsInSegment.Sum(x => x.Miles);
                    var totalSegmentFare = legsInSegment.Sum(s => s.ActFare);
                    var totalAirCo2 = legsInSegment.Sum(s => s.AirCo2);

                    T legToAdd;
                    switch (collapseType)
                    {
                        case CollapseType.First:
                            legToAdd = CollapserMapper<T>.MapDepartureLeg(departureLeg, arrivalLeg, totalMilesInSegment);
                            break;
                        case CollapseType.Last:
                        case CollapseType.Both:
                            legToAdd = CollapserMapper<T>.MapArrivalLeg(arrivalLeg, departureLeg, totalMilesInSegment, totalSegmentFare, totalAirCo2);
                            break;
                        case CollapseType.LongestMile:
                            legToAdd = CollapserMapper<T>.MapLongestMileLeg(arrivalLeg, departureLeg, totalMilesInSegment, totalSegmentFare, totalAirCo2);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(collapseType), collapseType, null);
                    }

                    legToAdd.DitCode = CollapserHelper.GetDomesticInternationalType(legsInSegment);

                    if (useClassCatHierarchy)
                    {
                        var query = new GetAllClassCategoriesQuery(store.ClientQueryDb, agency);
                        legToAdd = CollapserMapper<T>.RemapAccordingToClassHierarchy(query, legsInSegment, legToAdd);
                    }
                    results.Add(legToAdd);
                }
            }
            return results;
        }

        private static bool IsConnection(string connectionIndicator)
        {
            return connectionIndicator.EqualsIgnoreCase("X");
        }

        public static Dictionary<int, IList<T>> SetSegCounter(IList<T> list)
        {
            var dict = new Dictionary<int, IList<T>>();
            //must be sort by reckey and seqno first
            list = list.OrderBy(x => x.RecKey).ThenBy(x => x.SeqNo).ToList();

            var groupedReckeys = list.GroupBy(x => x.RecKey).Select(grp => new { Reckey = grp.Key, Trip = grp });

            foreach (var t in groupedReckeys)
            {
                var segCounter = 1;
                foreach (var trip in t.Trip)
                {
                    trip.Seg_Cntr = segCounter;
                    if (!IsConnection(trip.Connect)) segCounter++;
                }
                dict.Add(t.Reckey, t.Trip.ToList());
            }

            return dict;
        }
    }
}
