using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.ExecutiveSummaryYearToYearReport;

namespace iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryYearToYear
{
    public class CarSummaryBuilder
    {
        public void AddCarRows(List<CarRawData> carRawDataPy, List<CarRawData> carRawDataCy, List<FinalData> Rows, DateTime BegMonth, 
            DateTime EndDate, DateTime BegMonth2, DateTime EndDate2, bool CarbonReporting, string PoundsKilos)
        {

            var groupTitle = "  Car Rentals Summary";
            var totRentals = new FinalData
            {
                RowNum = 21,
                Grp = groupTitle,
                GrpSort = 3,
                SubGrp = 1,
                Descrip = "# of Car Rentals:",
                DisplayType = DisplayType.Integer,
                MonthValuePreviousYear = carRawDataPy.Count(s => s.UseDate >= BegMonth && s.UseDate <= EndDate),
                YearToDatePreviousYear = carRawDataPy.Count,
                MonthValueCurrentYear = carRawDataCy.Count(s => s.UseDate >= BegMonth2 && s.UseDate <= EndDate2),
                YearToDateCurrentYear = carRawDataCy.Count,
            };

            Rows.Add(totRentals);

            //Need the row that holds the Net Tickets
            var netTickets = Rows.FirstOrDefault(s => s.GrpSort == 1 && s.RowNum == 4);
            if (netTickets != null)
            {

                Rows.Add(new FinalData
                {
                    RowNum = 22,
                    Grp = groupTitle,
                    GrpSort = 3,
                    SubGrp = 1,
                    Descrip = "Car Rentals as % of Trips:",
                    DisplayType = DisplayType.Percentage,
                    MonthValuePreviousYear = netTickets.MonthValuePreviousYear == 0 ? 0 : totRentals.MonthValuePreviousYear / netTickets.MonthValuePreviousYear,
                    YearToDatePreviousYear = netTickets.YearToDatePreviousYear == 0 ? 0 : totRentals.YearToDatePreviousYear / netTickets.YearToDatePreviousYear,
                    MonthValueCurrentYear = netTickets.MonthValueCurrentYear == 0 ? 0 : totRentals.MonthValueCurrentYear / netTickets.MonthValueCurrentYear,
                    YearToDateCurrentYear = netTickets.YearToDateCurrentYear == 0 ? 0 : totRentals.YearToDateCurrentYear / netTickets.YearToDateCurrentYear
                });
            }
            var totalDays = new FinalData
            {
                RowNum = 23,
                Grp = groupTitle,
                GrpSort = 3,
                SubGrp = 1,
                Descrip = "Total # of Days Cars Rented:",
                DisplayType = DisplayType.Integer,
                MonthValuePreviousYear = carRawDataPy.Where(s => s.UseDate >= BegMonth && s.UseDate <= EndDate).Sum(s => s.Days),
                YearToDatePreviousYear = carRawDataPy.Sum(s => s.Days),
                MonthValueCurrentYear = carRawDataCy.Where(s => s.UseDate >= BegMonth2 && s.UseDate <= EndDate2).Sum(s => s.Days),
                YearToDateCurrentYear = carRawDataCy.Sum(s => s.Days),
            };
            Rows.Add(totalDays);

            Rows.Add(new FinalData
            {
                RowNum = 24,
                Grp = groupTitle,
                GrpSort = 3,
                SubGrp = 1,
                Descrip = "Average # of Days Rented:",
                DisplayType = DisplayType.OneDecimal,
                MonthValuePreviousYear = totRentals.MonthValuePreviousYear == 0 ? 0 : totalDays.MonthValuePreviousYear / totRentals.MonthValuePreviousYear,
                YearToDatePreviousYear = totRentals.YearToDatePreviousYear == 0 ? 0 : totalDays.YearToDatePreviousYear / totRentals.YearToDatePreviousYear,
                MonthValueCurrentYear = totRentals.MonthValueCurrentYear == 0 ? 0 : totalDays.MonthValueCurrentYear / totRentals.MonthValueCurrentYear,
                YearToDateCurrentYear = totRentals.YearToDateCurrentYear == 0 ? 0 : totalDays.YearToDateCurrentYear / totRentals.YearToDateCurrentYear
            });

            var totalExpense = new FinalData
            {
                RowNum = 25,
                Grp = groupTitle,
                GrpSort = 3,
                SubGrp = 1,
                Descrip = "Total Car Rental Expense:",
                DisplayType = DisplayType.Currency,
                MonthValuePreviousYear = carRawDataPy.Where(s => s.UseDate >= BegMonth && s.UseDate <= EndDate).Sum(s => s.Days * s.Abookrat),
                YearToDatePreviousYear = carRawDataPy.Sum(s => s.Days * s.Abookrat),
                MonthValueCurrentYear = carRawDataCy.Where(s => s.UseDate >= BegMonth2 && s.UseDate <= EndDate2).Sum(s => s.Days * s.Abookrat),
                YearToDateCurrentYear = carRawDataCy.Sum(s => s.Days * s.Abookrat)
            };
            Rows.Add(totalExpense);

            Rows.Add(new FinalData
            {
                RowNum = 26,
                Grp = groupTitle,
                GrpSort = 3,
                SubGrp = 1,
                Descrip = "Average Cost per Day:",
                DisplayType = DisplayType.Currency,
                MonthValuePreviousYear = totalDays.MonthValuePreviousYear == 0 ? 0 : totalExpense.MonthValuePreviousYear / totalDays.MonthValuePreviousYear,
                YearToDatePreviousYear = totalDays.YearToDatePreviousYear == 0 ? 0 : totalExpense.YearToDatePreviousYear / totalDays.YearToDatePreviousYear,
                MonthValueCurrentYear = totalDays.MonthValueCurrentYear == 0 ? 0 : totalExpense.MonthValueCurrentYear / totalDays.MonthValueCurrentYear,
                YearToDateCurrentYear = totalDays.YearToDateCurrentYear == 0 ? 0 : totalExpense.YearToDateCurrentYear / totalDays.YearToDateCurrentYear
            });

            if (CarbonReporting)
            {
                var totCo2 = new FinalData
                {
                    RowNum = 27,
                    Grp = groupTitle,
                    GrpSort = 3,
                    SubGrp = 1,
                    Descrip = "Car CO2 Emissions (" + PoundsKilos + "):",
                    DisplayType = DisplayType.Integer,
                    MonthValuePreviousYear = carRawDataPy.Where(s => s.UseDate >= BegMonth && s.UseDate <= EndDate).Sum(s => s.CarCo2),
                    YearToDatePreviousYear = carRawDataPy.Sum(s => s.CarCo2),
                    MonthValueCurrentYear = carRawDataCy.Where(s => s.UseDate >= BegMonth2 && s.UseDate <= EndDate2).Sum(s => s.CarCo2),
                    YearToDateCurrentYear = carRawDataCy.Sum(s => s.CarCo2),
                };

                Rows.Add(totCo2);
            }
        }
    }
}
