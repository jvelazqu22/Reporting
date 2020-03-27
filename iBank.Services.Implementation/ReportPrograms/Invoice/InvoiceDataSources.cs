using Domain.Models.ReportPrograms.InvoiceReport;
using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.Invoice
{
    public class InvoiceDataSources
    {
        public List<LegRawData> LegData { get; set; }
        public List<HotelRawData> HotelData { get; set; }
        public List<CarRawData> CarData { get; set; }
        public List<RawData> RawData { get; set; }
        
        public Udid UdidOne { get; set; }

        public Udid UdidTwo { get; set; }

        public List<SvcFeeRawData> SvcFeeData { get; set; }

        public InvoiceDataSources(List<LegRawData> legData, List<HotelRawData> hotelData, List<CarRawData> carData, List<RawData> rawData, List<SvcFeeRawData> svcFeeData,
                                  Udid udidOne, Udid udidTwo)
        {
            LegData = legData;
            HotelData = hotelData;
            CarData = carData;
            RawData = rawData;
            SvcFeeData = svcFeeData;
            UdidOne = udidOne;
            UdidTwo = udidTwo;

        }
    }
}
