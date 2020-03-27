using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.ExecutiveSummaryYearToYearReport;

namespace iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryYearToYear
{
    public class ReportSummaryBuilder
    {
        public void AddSummaryRows(List<FinalData> Rows, bool SplitRail)
        {
            const string groupTitle = "  Report Summary";
            //get the rows for each data type
            var airTotals = Rows.FirstOrDefault(s => s.RowNum == 10 && s.GrpSort == 1);
            var railTotals = Rows.FirstOrDefault(s => s.RowNum == 10 && s.GrpSort == 2);
            var carTotals = Rows.FirstOrDefault(s => s.RowNum == 25 && s.GrpSort == 3);
            var hotelTotals = Rows.FirstOrDefault(s => s.RowNum == 32 && s.GrpSort == 4);

            if (airTotals == null) airTotals = new FinalData();

            if (carTotals == null) carTotals = new FinalData();

            if (hotelTotals == null) hotelTotals = new FinalData();

            if (railTotals == null) railTotals = new FinalData();

            var totCharges = new FinalData
            {
                RowNum = 35,
                Grp = groupTitle,
                GrpSort = 5,
                SubGrp = 1,
                Descrip = "Total Travel Charges:",
                DisplayType = DisplayType.Currency,
                MonthValuePreviousYear = airTotals.MonthValuePreviousYear + railTotals.MonthValuePreviousYear + carTotals.MonthValuePreviousYear + hotelTotals.MonthValuePreviousYear,
                YearToDatePreviousYear = airTotals.YearToDatePreviousYear + railTotals.YearToDatePreviousYear + carTotals.YearToDatePreviousYear + hotelTotals.YearToDatePreviousYear,
                MonthValueCurrentYear = airTotals.MonthValueCurrentYear + railTotals.MonthValueCurrentYear + carTotals.MonthValueCurrentYear + hotelTotals.MonthValueCurrentYear,
                YearToDateCurrentYear = airTotals.YearToDateCurrentYear + railTotals.YearToDateCurrentYear + carTotals.YearToDateCurrentYear + hotelTotals.YearToDateCurrentYear
            };

            Rows.Add(totCharges);

            Rows.Add(new FinalData
            {
                RowNum = 36,
                Grp = groupTitle,
                GrpSort = 5,
                SubGrp = 1,
                Descrip = "% Air:",
                DisplayType = DisplayType.Percentage,
                MonthValuePreviousYear = totCharges.MonthValuePreviousYear == 0 ? 0 : airTotals.MonthValuePreviousYear / totCharges.MonthValuePreviousYear,
                YearToDatePreviousYear = totCharges.YearToDatePreviousYear == 0 ? 0 : airTotals.YearToDatePreviousYear / totCharges.YearToDatePreviousYear,
                MonthValueCurrentYear = totCharges.MonthValueCurrentYear == 0 ? 0 : airTotals.MonthValueCurrentYear / totCharges.MonthValueCurrentYear,
                YearToDateCurrentYear = totCharges.YearToDateCurrentYear == 0 ? 0 : airTotals.YearToDateCurrentYear / totCharges.YearToDateCurrentYear
            });

            if (SplitRail)
            {
                Rows.Add(new FinalData
                {
                    RowNum = 37,
                    Grp = groupTitle,
                    GrpSort = 5,
                    SubGrp = 1,
                    Descrip = "% Rail:",
                    DisplayType = DisplayType.Percentage,
                    MonthValuePreviousYear = totCharges.MonthValuePreviousYear == 0 ? 0 : railTotals.MonthValuePreviousYear / totCharges.MonthValuePreviousYear,
                    YearToDatePreviousYear = totCharges.YearToDatePreviousYear == 0 ? 0 : railTotals.YearToDatePreviousYear / totCharges.YearToDatePreviousYear,
                    MonthValueCurrentYear = totCharges.MonthValueCurrentYear == 0 ? 0 : railTotals.MonthValueCurrentYear / totCharges.MonthValueCurrentYear,
                    YearToDateCurrentYear = totCharges.YearToDateCurrentYear == 0 ? 0 : railTotals.YearToDateCurrentYear / totCharges.YearToDateCurrentYear
                });
            }

            Rows.Add(new FinalData
            {
                RowNum = 38,
                Grp = groupTitle,
                GrpSort = 5,
                SubGrp = 1,
                Descrip = "% Car:",
                DisplayType = DisplayType.Percentage,
                MonthValuePreviousYear = totCharges.MonthValuePreviousYear == 0 ? 0 : carTotals.MonthValuePreviousYear / totCharges.MonthValuePreviousYear,
                YearToDatePreviousYear = totCharges.YearToDatePreviousYear == 0 ? 0 : carTotals.YearToDatePreviousYear / totCharges.YearToDatePreviousYear,
                MonthValueCurrentYear = totCharges.MonthValueCurrentYear == 0 ? 0 : carTotals.MonthValueCurrentYear / totCharges.MonthValueCurrentYear,
                YearToDateCurrentYear = totCharges.YearToDateCurrentYear == 0 ? 0 : carTotals.YearToDateCurrentYear / totCharges.YearToDateCurrentYear
            });

            Rows.Add(new FinalData
            {
                RowNum = 39,
                Grp = groupTitle,
                GrpSort = 5,
                SubGrp = 1,
                Descrip = "% Hotel:",
                DisplayType = DisplayType.Percentage,
                MonthValuePreviousYear = totCharges.MonthValuePreviousYear == 0 ? 0 : hotelTotals.MonthValuePreviousYear / totCharges.MonthValuePreviousYear,
                YearToDatePreviousYear = totCharges.YearToDatePreviousYear == 0 ? 0 : hotelTotals.YearToDatePreviousYear / totCharges.YearToDatePreviousYear,
                MonthValueCurrentYear = totCharges.MonthValueCurrentYear == 0 ? 0 : hotelTotals.MonthValueCurrentYear / totCharges.MonthValueCurrentYear,
                YearToDateCurrentYear = totCharges.YearToDateCurrentYear == 0 ? 0 : hotelTotals.YearToDateCurrentYear / totCharges.YearToDateCurrentYear
            });
        }
    }
}
