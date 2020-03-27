
using System.Collections.Generic;


namespace Domain.Models.ReportPrograms.XmlExtractReport
{
    public class XmlExtractParameters
    {
        public List<RawData> TripRawDataList { get; set; } = new List<RawData>();
        public List<LegRawData> LegRawDataList { get; set; } = new List<LegRawData>();
        public List<CarRawData> CarRawDataList { get; set; } = new List<CarRawData>();
        public List<HotelRawData> HotelRawDataList { get; set; } = new List<HotelRawData>();
        public List<UdidRawData> UdidRawDataList { get; set; } = new List<UdidRawData>();
        public List<SvcFeeRawData> SvcFeeRawDataList { get; set; } = new List<SvcFeeRawData>();
        public List<MktSegRawData> MktSegRawDataList { get; set; } = new List<MktSegRawData>();
    }
}
