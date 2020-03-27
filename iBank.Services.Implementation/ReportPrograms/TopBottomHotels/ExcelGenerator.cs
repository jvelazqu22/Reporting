using Domain.Helper;
using Domain.Models.ReportPrograms.TopBottomHotelsReport;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomHotels
{
    public static class ExcelGenerator
    {
        public static void ExportData(List<FinalData> finalDataList, bool secondRange, ReportGlobals globals)
        {
            var groupBy = globals.GetParmValue(WhereCriteria.GROUPBY);
            if (secondRange)
            {
                var temp = finalDataList.Select(s => new
                {
                    s.Category,
                    s.Stays,
                    RoomNights = s.Nights,
                    Volume = s.Hotelcost,
                    AvgCostPerNight = s.Nznights == 0 ? 0 : MathHelper.Round(s.Hotelcost / s.Nznights, 2),
                    s.Stays2,
                    RoomNights2 = s.Nights2,
                    Volume2 = s.Hotelcost2,
                    AvgCostPerNight2 = s.Nznights2 == 0 ? 0 : MathHelper.Round(s.Hotelcost2 / s.Nznights2, 2),

                }).ToList();
                var fieldList = new List<string> { "Category", "Stays", "RoomNights", "Volume", "AvgCostPerNight", "Stays2", "RoomNights2", "Volume2", "AvgCostPerNight2" };
                if (globals.OutputFormat == DestinationSwitch.Csv)
                {
                    ExportHelper.ConvertToCsv(temp, fieldList, globals);
                }
                else
                {
                    ExportHelper.ListToXlsx(temp, fieldList, globals);
                }

            }
            else if (groupBy.Equals("5") || groupBy.Equals("6"))
            {
                var temp = finalDataList.Select(s => new
                {
                    s.Category,
                    HotelChain = s.Cat2,
                    NbrOfStays = s.Stays2,
                    RoomNights = s.Nights2,
                    Volume = s.Hotelcost2,
                    AvgBook = MathHelper.Round(s.Avgbook2, 2),
                    AvgNbrNights = s.Stays2 == 0 ? 0 : MathHelper.Round((decimal)s.Nights2 / s.Stays2, 2),
                    AvgCostPerStay = s.Stays2 == 0 ? 0 : MathHelper.Round(s.Hotelcost2 / s.Stays2, 2),
                    AvgCostPerNight = s.Nznights2 == 0 ? 0 : MathHelper.Round(s.Hotelcost2 / s.Nznights2, 2)

                }).ToList();
                var fieldList = new List<string> { "Category", "HotelChain", "NbrOfStays", "RoomNights", "Volume", "AvgBook", "AvgNbrNights", "AvgCostPerStay", "AvgCostPerNight" };
                if (globals.OutputFormat == DestinationSwitch.Csv)
                {
                    ExportHelper.ConvertToCsv(temp, fieldList, globals);
                }
                else
                {
                    ExportHelper.ListToXlsx(temp, fieldList, globals);
                }
            }
            else if (groupBy.Equals("7"))
            {
                var temp = finalDataList.Select(s => new
                {
                    s.Category,
                    HotelProp = s.Cat2,
                    NbrOfStays = s.Stays2,
                    RoomNights = s.Nights2,
                    Volume = s.Hotelcost2,
                    AvgBook = MathHelper.Round(s.Avgbook2, 2),
                    AvgNbrNights = s.Stays2 == 0 ? 0 : MathHelper.Round((decimal)s.Nights2 / s.Stays2, 2),
                    AvgCostPerStay = s.Stays2 == 0 ? 0 : MathHelper.Round(s.Hotelcost2 / s.Stays2, 2),
                    AvgCostPerNight = s.Nznights2 == 0 ? 0 : MathHelper.Round(s.Hotelcost2 / s.Nznights2, 2)

                }).ToList();
                var fieldList = new List<string> { "Category", "HotelProp", "NbrOfStays", "RoomNights", "Volume", "AvgBook", "AvgNbrNights", "AvgCostPerStay", "AvgCostPerNight" };
                if (globals.OutputFormat == DestinationSwitch.Csv)
                {
                    ExportHelper.ConvertToCsv(temp, fieldList, globals);
                }
                else
                {
                    ExportHelper.ListToXlsx(temp, fieldList, globals);
                }
            }
            else if (groupBy.Equals("8"))
            {
                var temp = finalDataList.Select(s => new
                {
                    s.Category,
                    City = s.Cat2,
                    NbrOfStays = s.Stays2,
                    RoomNights = s.Nights2,
                    Volume = s.Hotelcost2,
                    AvgBook = MathHelper.Round(s.Avgbook2, 2),
                    AvgNbrNights = s.Stays2 == 0 ? 0 : MathHelper.Round((decimal)s.Nights2 / s.Stays2, 2),
                    AvgCostPerStay = s.Stays2 == 0 ? 0 : MathHelper.Round(s.Hotelcost2 / s.Stays2, 2),
                    AvgCostPerNight = s.Nznights2 == 0 ? 0 : MathHelper.Round(s.Hotelcost2 / s.Nznights2, 2)

                }).ToList();
                var fieldList = new List<string> { "Category", "City", "NbrOfStays", "RoomNights", "Volume", "AvgBook", "AvgNbrNights", "AvgCostPerStay", "AvgCostPerNight" };
                if (globals.OutputFormat == DestinationSwitch.Csv)
                {
                    ExportHelper.ConvertToCsv(temp, fieldList, globals);
                }
                else
                {
                    ExportHelper.ListToXlsx(temp, fieldList, globals);
                }
            }
            else
            {
                var temp = finalDataList.Select(s => new
                {
                    s.Category,
                    NbrOfStays = s.Stays,
                    RoomNights = s.Nights,
                    Volume = s.Hotelcost,
                    AvgBook = MathHelper.Round(s.Avgbook, 2),
                    AvgNbrNights = s.Stays == 0 ? 0 : MathHelper.Round((decimal)s.Nights / s.Stays, 2),
                    AvgCostPerStay = s.Stays == 0 ? 0 : MathHelper.Round(s.Hotelcost / s.Stays, 2),
                    AvgCostPerNight = s.Nznights == 0 ? 0 : MathHelper.Round(s.Hotelcost / s.Nznights, 2)

                }).ToList();
                var fieldList = new List<string> { "Category", "NbrOfStays", "RoomNights", "Volume", "AvgBook", "AvgNbrNights", "AvgCostPerStay", "AvgCostPerNight" };
                if (globals.OutputFormat == DestinationSwitch.Csv)
                {
                    ExportHelper.ConvertToCsv(temp, fieldList, globals);
                }
                else
                {
                    ExportHelper.ListToXlsx(temp, fieldList, globals);
                }
            }
        }
    }
}
