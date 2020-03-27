using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.TravelDetail;

namespace iBank.Services.Implementation.ReportPrograms.TravelDetail
{
    public class TopTravAllFinalGroupData
    {
        // Group list of final data by passenger name
        public static List<TopTravAllFinalData> GetGroupFinalData(List<TopTravAllFinalData> finalDataList)
        {
            var results = finalDataList.GroupBy(s => new { s.passfrst, s.passlast },
                (key, g) =>
                {
                    var reclist = g.ToList();
                    var accList = g.GroupBy(a => new { a.acct, a.acctdesc }).ToList().Count;
                    var finalData = new TopTravAllFinalData
                    {
                        passfrst = key.passfrst,
                        passlast = key.passlast,
                        acct = (accList > 1) ? "[mult]" : reclist.First().acct,
                        acctdesc = (accList > 1) ? "[Multiple Accounts]" : reclist.First().acctdesc,
                        Break1 = reclist.First().Break1,
                        Break2 = reclist.First().Break2,
                        Break3 = reclist.First().Break3,
                        tripcount = reclist.Sum(s => s.tripcount),
                        airchg = reclist.Sum(s => s.airchg),
                        railcount = reclist.Sum(s => s.railcount),
                        railchg = reclist.Sum(s => s.railchg),
                        cardays = reclist.Sum(s => s.cardays),
                        carcost = reclist.Sum(s => s.carcost),
                        hotnights = reclist.Sum(s => s.hotnights),
                        hotelcost = reclist.Sum(s => s.hotelcost),
                        daysonroad = reclist.First().daysonroad,
                        Airco2 = reclist.Sum(s => s.Airco2),
                        Carco2 = reclist.Sum(s => s.Carco2),
                        HotelCo2 = reclist.Sum(s => s.HotelCo2)
                    };
                    finalData.TotCO2 = finalData.Airco2 + finalData.Carco2 + finalData.HotelCo2;
                    finalData.homectry = reclist.First().homectry;

                    return finalData;
                }).ToList();

            UpdateDaysInTheRoad(finalDataList, results);

            return results;
        }

        // Iterates through a list to update days on the road
        public static void UpdateDaysInTheRoad(List<TopTravAllFinalData> finalDataList, List<TopTravAllFinalData> groupedFinalDataList)
        {
            foreach (var item in groupedFinalDataList)
            {
                var listOfRecordsPerCustomer = finalDataList.Where(f => f.passfrst.Equals(item.passfrst) && f.passlast.Equals(item.passlast)).ToList();

                foreach (var rec in listOfRecordsPerCustomer)
                {
                    item.daysonroad += GetDaysOnTheRoad(rec.TripStart, rec.TripEnd, rec.DepDate, rec.ArrDate, rec.PlusMin);
                }
            }
        }

        //Calculate days on the road
        public static int GetDaysOnTheRoad(DateTime? tripStart, DateTime? tripEnd, DateTime? depDate, DateTime? arrDate, int plusMin)
        {
            var result = 0;
            var lowDate = new DateTime(1990, 1, 1);
            if (tripStart.HasValue && tripEnd.HasValue && tripStart > lowDate && tripEnd > lowDate)
            {
                result = plusMin * (((DateTime)tripEnd).Subtract((DateTime)tripStart).Days + 1);
            }
            else if (depDate.HasValue && arrDate.HasValue && depDate > lowDate && arrDate > lowDate)
            {
                result = plusMin * (((DateTime)arrDate).Subtract((DateTime)depDate).Days + 1);
            }
            return result;
        }

    }
}
