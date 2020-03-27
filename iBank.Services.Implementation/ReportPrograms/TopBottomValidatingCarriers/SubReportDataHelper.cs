using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.TopBottomValidatingCarriers;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomValidatingCarriers
{
    public class SubReportDataHelper
    {
        public List<SubReportData> SetSubReportData(List<FinalData> finalDataList)
        {
            return finalDataList.Select(s => new SubReportData
            {
                HomeCtry = s.HomeCtry,
                Trips = s.Trips,
                Amt = s.Amt,
                Avgcost = s.Avgcost,
                Trips2 = s.Trips2,
                Amt2 = s.Amt2,
                Avgcost2 = s.Avgcost2
            }).GroupBy(s => new
            {
                s.HomeCtry
            }).Select(s => new SubReportData
            {
                HomeCtry = s.Key.HomeCtry,
                Amt = s.Sum(x => x.Amt),
                Trips = s.Sum(x => x.Trips),
                Avgcost = (s.Sum(x => x.Trips) > 0) ? s.Sum(x => x.Amt) / s.Sum(x => x.Trips) : 0,
                Amt2 = s.Sum(x => x.Amt2),
                Trips2 = s.Sum(x => x.Trips2),
                Avgcost2 = (s.Sum(x => x.Trips2) > 0) ? s.Sum(x => x.Amt2) / s.Sum(x => x.Trips2) : 0
            }).ToList();
        }
    }
}
