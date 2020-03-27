using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.ExecutiveSummaryYearToYearReport;

namespace iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryYearToYear
{
    public class HotelSummaryBuilder
    {
        public void AddHotelRows(List<HotelRawData> hotelRawDataPy, List<HotelRawData> hotelRawDataCy, List<FinalData> Rows, 
            DateTime BegMonth, DateTime EndDate, DateTime BegMonth2, DateTime EndDate2, bool CarbonReporting, string PoundsKilos)
        {
            const string groupTitle = "  Hotel Bookings Summary";

            var totRentals = new FinalData
            {
                RowNum = 28,
                Grp = groupTitle,
                GrpSort = 4,
                SubGrp = 1,
                Descrip = "# of Hotel Bookings:",
                DisplayType = DisplayType.Integer,
                MonthValuePreviousYear = hotelRawDataPy.Count(s => s.UseDate >= BegMonth && s.UseDate <= EndDate),
                YearToDatePreviousYear = hotelRawDataPy.Count,
                MonthValueCurrentYear = hotelRawDataCy.Count(s => s.UseDate >= BegMonth2 && s.UseDate <= EndDate2),
                YearToDateCurrentYear = hotelRawDataCy.Count,
            };

            Rows.Add(totRentals);

            //Need the row that holds the Net Tickets
            var netTickets = Rows.FirstOrDefault(s => s.GrpSort == 1 && s.RowNum == 4);
            if (netTickets != null)
            {

                Rows.Add(new FinalData
                {
                    RowNum = 29,
                    Grp = groupTitle,
                    GrpSort = 4,
                    SubGrp = 1,
                    Descrip = "Hotel Bookings as % of Trips:",
                    DisplayType = DisplayType.Percentage,
                    MonthValuePreviousYear = netTickets.MonthValuePreviousYear == 0 ? 0 : totRentals.MonthValuePreviousYear / netTickets.MonthValuePreviousYear,
                    YearToDatePreviousYear = netTickets.YearToDatePreviousYear == 0 ? 0 : totRentals.YearToDatePreviousYear / netTickets.YearToDatePreviousYear,
                    MonthValueCurrentYear = netTickets.MonthValueCurrentYear == 0 ? 0 : totRentals.MonthValueCurrentYear / netTickets.MonthValueCurrentYear,
                    YearToDateCurrentYear = netTickets.YearToDateCurrentYear == 0 ? 0 : totRentals.YearToDateCurrentYear / netTickets.YearToDateCurrentYear
                });
            }
            var totalDays = new FinalData
            {
                RowNum = 30,
                Grp = groupTitle,
                GrpSort = 4,
                SubGrp = 1,
                Descrip = "Total # of Room Nights Booked:",
                DisplayType = DisplayType.Integer,
                MonthValuePreviousYear = hotelRawDataPy.Where(s => s.UseDate >= BegMonth && s.UseDate <= EndDate).Sum(s => s.Nights * s.Rooms),
                YearToDatePreviousYear = hotelRawDataPy.Sum(s => s.Nights * s.Rooms),
                MonthValueCurrentYear = hotelRawDataCy.Where(s => s.UseDate >= BegMonth2 && s.UseDate <= EndDate2).Sum(s => s.Nights * s.Rooms),
                YearToDateCurrentYear = hotelRawDataCy.Sum(s => s.Nights * s.Rooms),
            };
            Rows.Add(totalDays);

            Rows.Add(new FinalData
            {
                RowNum = 31,
                Grp = groupTitle,
                GrpSort = 4,
                SubGrp = 1,
                Descrip = "Average # of Room Nights Booked:",
                DisplayType = DisplayType.OneDecimal,
                MonthValuePreviousYear = totRentals.MonthValuePreviousYear == 0 ? 0 : totalDays.MonthValuePreviousYear / totRentals.MonthValuePreviousYear,
                YearToDatePreviousYear = totRentals.YearToDatePreviousYear == 0 ? 0 : totalDays.YearToDatePreviousYear / totRentals.YearToDatePreviousYear,
                MonthValueCurrentYear = totRentals.MonthValueCurrentYear == 0 ? 0 : totalDays.MonthValueCurrentYear / totRentals.MonthValueCurrentYear,
                YearToDateCurrentYear = totRentals.YearToDateCurrentYear == 0 ? 0 : totalDays.YearToDateCurrentYear / totRentals.YearToDateCurrentYear
            });

            var totalExpense = new FinalData
            {
                RowNum = 32,
                Grp = groupTitle,
                GrpSort = 4,
                SubGrp = 1,
                Descrip = "Total Hotel Bookings Expense:",
                DisplayType = DisplayType.Currency,
                MonthValuePreviousYear = hotelRawDataPy.Where(s => s.UseDate >= BegMonth && s.UseDate <= EndDate).Sum(s => s.Rooms * s.Nights * s.Bookrate),
                YearToDatePreviousYear = hotelRawDataPy.Sum(s => s.Rooms * s.Nights * s.Bookrate),
                MonthValueCurrentYear = hotelRawDataCy.Where(s => s.UseDate >= BegMonth2 && s.UseDate <= EndDate2).Sum(s => s.Rooms * s.Nights * s.Bookrate),
                YearToDateCurrentYear = hotelRawDataCy.Sum(s => s.Rooms * s.Nights * s.Bookrate)
            };
            Rows.Add(totalExpense);

            Rows.Add(new FinalData
            {
                RowNum = 33,
                Grp = groupTitle,
                GrpSort = 4,
                SubGrp = 1,
                Descrip = "Average Cost per RoomNight:",
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
                    RowNum = 34,
                    Grp = groupTitle,
                    GrpSort = 4,
                    SubGrp = 1,
                    Descrip = "Hotel CO2 Emissions (" + PoundsKilos + "):",
                    DisplayType = DisplayType.Integer,
                    MonthValuePreviousYear = hotelRawDataPy.Where(s => s.UseDate >= BegMonth && s.UseDate <= EndDate).Sum(s => s.HotelCo2),
                    YearToDatePreviousYear = hotelRawDataPy.Sum(s => s.HotelCo2),
                    MonthValueCurrentYear = hotelRawDataCy.Where(s => s.UseDate >= BegMonth2 && s.UseDate <= EndDate2).Sum(s => s.HotelCo2),
                    YearToDateCurrentYear = hotelRawDataCy.Sum(s => s.HotelCo2),
                };

                Rows.Add(totCo2);
            }
        }
    }
}
