using System;
using System.Collections.Generic;

using Domain.Models.ReportPrograms.ExecutiveSummaryYearToYearReport;

namespace iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryYearToYear
{
    public class RowBuilder
    {
        private DateTime BegMonth { get; set; }
        private DateTime BegMonth2 { get; set; }
        private DateTime EndDate { get; set; }
        private DateTime EndDate2 { get; set; }
        private string PoundsKilos { get; set; }
        private string MilesKilos { get; set; }
        private bool SplitRail { get; set; }
        private bool CarbonReporting { get; set; }
        private bool ExcludeMiles { get; set; }
        private bool ExcludeExceptions { get; set; }
        private bool ExcludeServiceFees { get; set; }
        private bool ExcludeNegotiatedSavings { get; set; }
        private bool ExcludeSavings { get; set; }

        public List<FinalData> Rows { get; set; } = new List<FinalData>();

        public RowBuilder(DateTime begMonth, DateTime begMonth2, DateTime endDate, DateTime endDate2, string weightMeasurement,
            string distanceMeasurement, bool carbonReporting, bool splitRail, bool excludeExceptions, bool excludeMiles, 
            bool excludeServiceFees, bool excludeNegotiatedSavings, bool excludeSavings)
        {
            BegMonth = begMonth;
            BegMonth2 = begMonth2;
            EndDate = endDate;
            EndDate2 = endDate2;
            PoundsKilos = weightMeasurement;
            MilesKilos = distanceMeasurement;
            CarbonReporting = carbonReporting;
            SplitRail = splitRail;
            ExcludeExceptions = excludeExceptions;
            ExcludeMiles = excludeMiles;
            ExcludeServiceFees = excludeServiceFees;
            ExcludeNegotiatedSavings = excludeNegotiatedSavings;
            ExcludeSavings = excludeSavings;
        }

        public void AddTripRows(int groupSort, string groupName, List<RawData> airRawDataPy, List<RawData> airRawDataCy, List<FeeRawData> feeRawDataPy, 
            List<FeeRawData> feeRawDataCy, List<LegRawData> legRawDataPy, List<LegRawData> legRawDataCy)
        {
            var airBuilder = new AirSummaryBuilder();
            airBuilder.AddTripRows(groupSort, groupName, airRawDataPy, airRawDataCy, feeRawDataPy, feeRawDataCy, legRawDataPy, legRawDataCy, 
                Rows, BegMonth, EndDate, BegMonth2, EndDate2, CarbonReporting, PoundsKilos, SplitRail, ExcludeMiles, ExcludeExceptions, ExcludeServiceFees, 
                ExcludeNegotiatedSavings, MilesKilos, ExcludeSavings);
        }

        public void AddCarRows(List<CarRawData> carRawDataPy, List<CarRawData> carRawDataCy)
        {
            var builder = new CarSummaryBuilder();
            builder.AddCarRows(carRawDataPy, carRawDataCy, Rows, BegMonth, EndDate, BegMonth2, EndDate2, CarbonReporting, PoundsKilos);
        }

        public void AddHotelRows(List<HotelRawData> hotelRawDataPy, List<HotelRawData> hotelRawDataCy)
        {
            var builder = new HotelSummaryBuilder();
            builder.AddHotelRows(hotelRawDataPy, hotelRawDataCy, Rows, BegMonth, EndDate, BegMonth2, EndDate2, CarbonReporting, PoundsKilos);
        }

        public void AddSummaryRows()
        {
            var builder = new ReportSummaryBuilder();
            builder.AddSummaryRows(Rows, SplitRail);
        }
    }
}
