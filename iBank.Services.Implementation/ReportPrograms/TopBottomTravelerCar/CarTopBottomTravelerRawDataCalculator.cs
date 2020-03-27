using Domain.Models.ReportPrograms.TopBottomTravelersCar;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomTravelerCar
{
    public class CarTopBottomTravelerRawDataCalculator
    {
        public List<RawData> GetSummaryReservationRawData(List<RawData> rawDataList)
        {
            return rawDataList.GroupBy(s => new { s.Passlast, s.Passfrst, s.ABookRat },
                    (key, g) =>
                    {
                        return new RawData
                        {
                            Passlast = g.First().Passlast,
                            Passfrst = g.First().Passfrst,
                            ABookRat = g.First().ABookRat,
                            Rentals = g.Count(),
                            Days = g.Sum(s => s.Days),
                            CarCost = g.Sum(s => s.ABookRat * s.Days),
                            sumbkrate = g.Sum(s => s.ABookRat)
                        };
                    }).ToList();
        }

        public List<RawData> GetSummaryBakcOfficeRawData(List<RawData> rawDataList)
        {
            return rawDataList.GroupBy(s => new { s.Passlast, s.Passfrst, s.ABookRat },
                    (key, g) =>
                    {
                        return new RawData
                        {
                            Passlast = g.First().Passlast,
                            Passfrst = g.First().Passfrst,
                            ABookRat = g.First().ABookRat,
                            Rentals = g.Sum(s => s.CPlusMin),
                            Days = g.Sum(s => s.Days * s.CPlusMin),
                            CarCost = g.Sum(s => s.ABookRat * s.Days),
                            sumbkrate = g.Sum(s => s.ABookRat)
                        };
                    }).ToList();
        }

        public int GetRawListTotalRentals(List<RawData> rawDataList)
        {
            int results = 0;
            foreach (var item in rawDataList.ToList())
                results += item.Rentals;
            return results;
        }

        public int GetRawListTotalDays(List<RawData> rawDataList)
        {
            int results = 0;
            foreach (var item in rawDataList.ToList())
                results += item.Days;
            return results;
        }

        public decimal GetRawListTotalCarCosts(List<RawData> rawDataList)
        {
            decimal results = 0;
            foreach (var item in rawDataList.ToList())
                results += item.CarCost;
            return results;
        }
    }
}
