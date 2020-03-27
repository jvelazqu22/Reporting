using Domain.Models.ReportPrograms.TopBottomTravelersCar;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomTravelerCar
{
    public class CarTopBottomTravelerFinalDataCalculator
    {
        public List<FinalData> GetFinalDataFromRawData(List<RawData> rawDataList)
        {
            return rawDataList.GroupBy(s => new { s.Passlast, s.Passfrst },
                (key, g) =>
                {
                    return  new FinalData
                    {
                        Passlast = g.First().Passlast,
                        Passfrst = g.First().Passfrst,
                        Rentals = g.Sum(s => s.Rentals),
                        Days = g.Sum(s => s.Days),
                        Carcost = g.Sum(s => s.CarCost),
                        Bookrate = g.Sum(s => s.sumbkrate),
                        Bookcnt = g.Sum(s => s.ABookRat == 0 ? 0 : s.Rentals),
                        Avgbook = g.Sum(s => s.Rentals) == 0 ? 0 :g.Sum(s => s.sumbkrate) / g.Sum(s => s.Rentals)
                    };
                }).ToList();
        }

        public decimal GetFinalListTotalRate(List<FinalData> finalDataList)
        {
            decimal results = 0;
            foreach (var item in finalDataList.ToList())
                results += item.Bookrate;
            return results;
        }

        public decimal GetFinalListTotalBookCount(List<FinalData> finalDataList)
        {
            decimal results = 0;
            foreach (var item in finalDataList.ToList())
                results += item.Bookcnt;
            return results;
        }
    }
}
