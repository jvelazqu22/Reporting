using CODE.Framework.Core.Utilities;
using Domain.Interfaces;
using Domain.Orm.Classes;
using System.Collections.Generic;
using System.Linq;

using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.Shared
{
    public class CollapserMapper<T> where T : class, ICollapsible, new()
    {
        public static T MapDepartureLeg(T departureLeg, T arrivalLeg, int totalMilesInSegment)
        {
            var legToAdd = new T();

            Mapper.Map(departureLeg, legToAdd);
            legToAdd.ActFare = arrivalLeg.ActFare;
            legToAdd.MiscAmt = arrivalLeg.MiscAmt;
            legToAdd.Destinat = arrivalLeg.Destinat;
            legToAdd.SeqNo = arrivalLeg.SeqNo;
            legToAdd.Miles = totalMilesInSegment;

            return legToAdd;
        }

        public static T MapArrivalLeg(T arrivalLeg, T departureLeg, int totalMilesInSegment, decimal totalSegmentFare, decimal totalAirCo2)
        {
            var legToAdd = new T();

            Mapper.Map(arrivalLeg, legToAdd);
            legToAdd.RDepDate = departureLeg.RDepDate;
            legToAdd.DepTime = departureLeg.DepTime;
            legToAdd.Miles = totalMilesInSegment;
            legToAdd.Origin = departureLeg.Origin;
            legToAdd.ActFare = totalSegmentFare;
            legToAdd.AirCo2 = totalAirCo2;
            // if one of the legs has an international flight, then the collapse flight should be deemed international
            if (departureLeg.DitCode.Equals("I")) legToAdd.DitCode = departureLeg.DitCode;

            return legToAdd;
        }

        public static T MapLongestMileLeg(T arrivalLeg, T departureLeg, int totalMilesInSegment, decimal totalSegmentFare, decimal totalAirCo2)
        {
            return arrivalLeg.Miles > departureLeg.Miles
                ? MapArrivalLeg(arrivalLeg, departureLeg, totalMilesInSegment, totalSegmentFare, totalAirCo2)
                : MapDepartureLeg(departureLeg, arrivalLeg, totalMilesInSegment);
        }
        //collapsed fltno take from departure leg, destinat takes from arrival leg
        public static T MapDepartAndArriveLeg(T arrivalLeg, T departureLeg, int totalMilesInSegment, decimal totalSegmentFare, decimal totalAirCo2, decimal totalAltCarCo2, decimal totalAltRailCo2)
        {
            var legToAdd = new T();

            Mapper.Map(arrivalLeg, legToAdd);
            legToAdd.fltno = departureLeg.fltno;
            legToAdd.RDepDate = departureLeg.RDepDate;
            legToAdd.DepTime = departureLeg.DepTime;
            legToAdd.Miles = totalMilesInSegment;
            legToAdd.Origin = departureLeg.Origin;
            legToAdd.Destinat = arrivalLeg.Destinat;
            legToAdd.ActFare = totalSegmentFare;
            legToAdd.AirCo2 = totalAirCo2;
            legToAdd.AltCarCo2 = totalAltCarCo2;
            legToAdd.AltRailCo2 = totalAltRailCo2;

            // if one of the legs has an international flight, then the collapse flight should be deemed international
            if (departureLeg.DitCode.Equals("I")) legToAdd.DitCode = departureLeg.DitCode;

            return legToAdd;

        }

        public static T RemapAccordingToClassHierarchy(IQuery<IList<ClassCategoryInformation>> getClassCategoryInfoQuery, IList<T> legsInSegment,
                                                       T legToAdd)
        {
            if (getClassCategoryInfoQuery == null) throw new System.Exception("No query supplied to get class hierarchy information.");

            var classesInSegment = legsInSegment.Select(x => x.ClassCode).Distinct().ToList();

            if (classesInSegment.Count <= 1)
            {
                var airline = legsInSegment.First(x => x.SeqNo == legsInSegment.Max(y => y.SeqNo)).Airline;

                if (!string.IsNullOrEmpty(airline)) legToAdd.Airline = airline;
                return legToAdd;
            }
            
            //get the hierarchy
            var hierarchy = getClassCategoryInfoQuery.ExecuteQuery().Where(x => classesInSegment.Contains(x.Category)).ToList();

            if (hierarchy.Count == 0) return legToAdd;

            //make sure we are only returning the hierarchy for one agency
            if (hierarchy.Select(x => x.Agency.Trim().ToLower()).Distinct().Count() != 1)
            {
                throw new System.Exception("A specific agency must be specified in the class hierarchy lookup.");
            }

            var classToUse = hierarchy.First(x => x.Heirarchy == hierarchy.Min(y => y.Heirarchy)).Category;
            var legsWithThatClass = legsInSegment.Where(x => x.ClassCode == classToUse).ToList();

            var airlineToUse = legsWithThatClass.Count == 1
                                   ? legsWithThatClass.First().Airline
                                   : legsWithThatClass.First(x => x.SeqNo == legsWithThatClass.Max(y => y.SeqNo)).Airline;

            legToAdd.ClassCode = classToUse;
            legToAdd.Airline = airlineToUse;

            return legToAdd;
        }
    }
}
