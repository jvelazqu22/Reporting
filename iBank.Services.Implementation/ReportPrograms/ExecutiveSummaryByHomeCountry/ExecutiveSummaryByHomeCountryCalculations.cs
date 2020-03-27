using Domain.Helper;
using iBank.Server.Utilities.Classes;
using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.ExecutiveSummaryByHomeCountryReport;

using iBank.Server.Utilities;
using iBank.Services.Implementation.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryByHomeCountry
{
    public class ExecutiveSummaryByHomeCountryCalculations
    {
        public void AddTotals(List<FinalData> FinalDataList)
        {
            var totalData = FinalDataList.GroupBy(s => new { s.RowType, s.RowDesc }, (key, recs) =>
            {
                var reclist = recs as IList<FinalData> ?? recs.ToList();
                var netTrans = reclist.Sum(s => s.NetTrans);
                var volume = reclist.Sum(s => s.Volume);
                var days = reclist.Sum(s => s.Days);
                var standardCharge = reclist.Sum(s => s.StandardCharge);
                var savings = reclist.Sum(s => s.Savings);
                var lostAmt = reclist.Sum(s => s.LostAmt);
                var avgCost = 0m;
                var avgDays = 0d;
                var savingsPct = 0d;
                var lossPct = 0d;
                if ((key.RowType.Equals("A") || key.RowType.Equals("D") || key.RowType.Equals("E")) && netTrans != 0) avgCost = MathHelper.Round(volume / netTrans, 2);
                if ((key.RowType.Equals("B") || key.RowType.Equals("C")) && days != 0) avgCost = MathHelper.Round(volume / days, 2);
                if ((key.RowType.Equals("A") || key.RowType.Equals("B") || key.RowType.Equals("C") || key.RowType.Equals("D")) && netTrans != 0) avgDays = MathHelper.Round((double)days / netTrans, 1);
                if ((key.RowType.Equals("A") || key.RowType.Equals("D")) && standardCharge != 0)
                {
                    savingsPct = MathHelper.Round(100 * (double)(savings / standardCharge), 2);
                    lossPct = MathHelper.Round(100 * (double)(lostAmt / volume), 2);
                }

                return new FinalData
                {
                    HomeCtry = "zzzReport Totals",
                    RowType = key.RowType,
                    RowDesc = key.RowDesc,
                    NetTrans = netTrans,
                    Volume = volume,
                    AvgCost = avgCost,
                    AvgDays = avgDays,
                    SvngsPct = savingsPct,
                    LossPct = lossPct
                };
            });

            FinalDataList.AddRange(totalData);

            foreach (var rec in FinalDataList.Where(s => s.HomeCtry.EqualsIgnoreCase("ZZZREPORT TOTALS")))
            {
                rec.HomeCtry = "Report Totals";
            }
        }

        public bool IncludeServiceFeeNoMatch(ReportGlobals globals)
        {
            return globals.IsParmValueOn(WhereCriteria.CBINCLSVCFEENOMATCH);
        }

        public IList<string> GetExportFields()
        {
            return new List<string>
            {
                "rowtype",
                "homectry",
                "rowdesc",
                "nettrans",
                "volume",
                "avgcost",
                "avgdays",
                "svngspct",
                "losspct",
                "standardcharge",
                "days",
                "savings",
                "lostamt"
            };
        }
    }
}