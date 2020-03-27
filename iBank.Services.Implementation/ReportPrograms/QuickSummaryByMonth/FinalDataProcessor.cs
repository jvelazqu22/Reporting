using Domain.Models.ReportPrograms.QuickSummaryByMonthReport;
using iBank.Server.Utilities;
using iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth.Calculations;
using System;
using System.Collections.Generic;
using System.Linq;


namespace iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth
{
    public class FinalDataProcessor
    {
        public IList<AirFinalData> ConvertRawAirToFinalAir(IList<AirRawData> rawData, IList<string> monthAbbreviations)
        {
            var airCalculations = new AirDataCalculations();
            return rawData.Select(x => new AirFinalData
            {
                RptYear = x.Datecomp.GetYearNumber(1900),
                RptMonth = x.Datecomp.GetMonthNumber(1),
                RptMthText = BuildReportMonthText(monthAbbreviations, x.Datecomp),
                PlusMin = x.Plusmin,
                AirChg = x.Airchg,
                ReasCode = x.Reascode,
                SavingCode = x.Savingcode == null ? "" : x.Savingcode,
                StndChg = airCalculations.CalculateStandardCharge(x.Stndchg, x.Airchg),
                Savings = 000000000.00M,
                OffrdChg = airCalculations.CalculateOfferedCharge(x.Offrdchg, x.Airchg),
                NegoSvngs = 000000000.00M,
                LostAmt = 000000000.00M
            }).ToList();
        }
        
        public IList<AirFinalData> UpdateFinalAirData(IList<AirFinalData> finalAirData, IList<string> reasExclude)
        {
            var airCalculations = new AirDataCalculations();
            foreach (var item in finalAirData)
            {
                item.ReasCode = airCalculations.UpdateReasCode(item.ReasCode, reasExclude);

                item.LostAmt = airCalculations.CalculateLostAmount(item.AirChg, item.OffrdChg);

                item.Savings = airCalculations.CalculateSavings(item.AirChg, item.StndChg);

                if (airCalculations.IsOkToUpdateSavingCode(item.SavingCode, item.ReasCode, item.LostAmt, item.Savings))
                {
                    item.SavingCode = item.ReasCode;
                    item.ReasCode = "";
                }

                item.LostAmt = airCalculations.CalculateLostAmount2(item.LostAmt, item.PlusMin);
            }

            return finalAirData;
        }

        public IList<ReportFinalData> CombineAirDataIntoReportData(IList<AirFinalData> airFinalData)
        {
            var reportCalculations = new ReportDataCalculations();
            var reportFinal = airFinalData.GroupBy(data => new
            {
                data.RptYear,
                data.RptMonth,
                data.RptMthText
            }).Select(data => new ReportFinalData
            {
                RptYear = data.Key.RptYear,
                RptMonth = data.Key.RptMonth,
                RptMthText = data.Key.RptMthText,
                AirTrips = data.Sum(x => x.PlusMin),
                AirVolume = data.Sum(x => x.AirChg),
                AirSvgs = data.Sum(x => (x.StndChg - x.AirChg)),
                AirExcepts = data.Sum(x => x.OffrdChg != 0 && x.LostAmt != 0 ? x.PlusMin : 0000000),
                AirLost = data.Sum(x => x.LostAmt)
            }).ToList();

            return reportFinal;
        }

        public IEnumerable<HotelFinalData> ConvertRawHotelToFinalHotelData(IList<HotelRawData> rawData, IList<string> monthAbbreviations, IList<string> reasExclude)
        {
            var hotelCalculations = new HotelCalculations();
            return rawData.Select(x => new HotelFinalData
            {
                RptYear = x.Datecomp.GetYearNumber(1900),
                RptMonth = x.Datecomp.GetMonthNumber(1),
                RptMthText = BuildReportMonthText(monthAbbreviations, x.Datecomp),
                HPlusMin = x.Hplusmin,
                Nights = x.Nights,
                Rooms = x.Rooms,
                BookRate = x.Bookrate,
                ReasCodh = x.Reascodh,
                Hexcprat = x.Hexcprat,
                HotExcepts = hotelCalculations.CalculateHotelExcepts(x.Reascodh, reasExclude, x.Hplusmin),
                HotLost = hotelCalculations.CalculateHotelLost(x.Reascodh, reasExclude, x.Bookrate, x.Hexcprat, x.Nights, x.Rooms)               
            });
        }

        public IList<ReportFinalData> CombineHotelDataIntoReportData(IEnumerable<HotelFinalData> hotelData)
        {
            
            return hotelData.GroupBy(data => new
            {
                data.RptYear,
                data.RptMonth,
                data.RptMthText
            }).Select(data => new ReportFinalData
            {
                RptYear = data.Key.RptYear,
                RptMonth = data.Key.RptMonth,
                RptMthText = data.Key.RptMthText,
                HotStays = data.Sum(x => x.HPlusMin),
                HotNights = data.Sum(x => (x.Nights * x.HPlusMin * x.Rooms)),
                HotVolume = data.Sum(x => (x.BookRate * x.Nights * x.Rooms)),
                HotExcepts = data.Sum(x => x.HotExcepts),
                HotLost = data.Sum(x => x.HotLost)
            }).ToList();
        }

        public IEnumerable<CarFinalData> ConvertRawCarToFinalCarData(IList<CarRawData> rawData, IList<string> monthAbbreviations, IList<string> reasExclude)
        {
            var carCalculations = new CarDataCalculations();
            return rawData.Select(x => new CarFinalData
            {
                RptYear = x.Datecomp.GetYearNumber(1900),
                RptMonth = x.Datecomp.GetMonthNumber(1),
                RptMthText = BuildReportMonthText(monthAbbreviations, x.Datecomp),
                CPlusMin = x.Cplusmin,
                Days = x.Days,
                ABookRat = x.Abookrat,
                ReasCoda = x.Reascoda,
                AExcprat = x.Aexcprat,
                CarExcepts = carCalculations.CalculateCarExcepts(x.Reascoda, reasExclude, x.Cplusmin),
                CarLost = carCalculations.CalculateCarLost(x.Reascoda, reasExclude, x.Abookrat, x.Aexcprat, x.Days)
            });
        }
        
        public IList<ReportFinalData> CombineCarDataIntoReportData(IEnumerable<CarFinalData> carData)
        {
            return carData.GroupBy(data => new
            {
                data.RptYear,
                data.RptMonth,
                data.RptMthText                                      
            }).Select(data => new ReportFinalData
            {
                RptYear = data.Key.RptYear,
                RptMonth = data.Key.RptMonth,
                RptMthText = data.Key.RptMthText,
                CarRents = data.Sum(x => x.CPlusMin),
                CarDays = data.Sum(x => (x.Days * x.CPlusMin)),
                CarVolume = data.Sum(x => (x.ABookRat * x.Days)),
                CarExcepts = data.Sum(x => x.CarExcepts),
                CarLost = data.Sum(x => x.CarLost)                                                                  
            }).ToList();
        }

        public IList<ReportFinalData> FinalizeReportData(IList<ReportFinalData> reportData)
        {
            return reportData.GroupBy(data => new
            {
                data.RptYear,
                data.RptMonth,
                data.RptMthText
            }).Select(data => new ReportFinalData
            {
                RptYear = data.Key.RptYear,
                RptMonth = data.Key.RptMonth,
                RptMthText = data.Key.RptMthText,
                AirTrips = data.Sum(x => x.AirTrips),
                AirVolume = data.Sum(x => x.AirVolume),
                AirSvgs = data.Sum(x => x.AirSvgs),
                AirExcepts = data.Sum(x => x.AirExcepts),
                AirLost = data.Sum(x => x.AirLost),
                CarRents = data.Sum(x => x.CarRents),
                CarDays = data.Sum(x => x.CarDays),
                CarVolume = data.Sum(x => x.CarVolume),
                CarExcepts = data.Sum(x => x.CarExcepts),
                CarLost = data.Sum(x => x.CarLost),
                HotStays = data.Sum(x => x.HotStays),
                HotNights = data.Sum(x => x.HotNights),
                HotVolume = data.Sum(x => x.HotVolume),
                HotExcepts = data.Sum(x => x.HotExcepts),
                HotLost = data.Sum(x => x.HotLost)
            }).OrderBy(x => x.RptYear)
                .ThenBy(x => x.RptMonth)
                .ToList();
        }

        public IList<GraphFinalData> ConvertToGraphData(IList<ReportFinalData> reportData)
        {
            return reportData.Select(x => new GraphFinalData
            {
                RecNumber = reportData.Count,
                CatDesc = x.RptMthText.Left(3),
                Data1 = x.AirVolume,
                Data2 = x.AirSvgs
            }).ToList();
        }

        private string BuildReportMonthText(IList<string> monthAbbreviations, DateTime? dateComp)
        {
            var monthAbbreviation = dateComp.GetMonthNumber(1).GetMonthAbbreviationFromNumber(monthAbbreviations);

            var year = dateComp.GetYearNumber(1900);

            return monthAbbreviation + ", " + year;
        }
        
    }
}
