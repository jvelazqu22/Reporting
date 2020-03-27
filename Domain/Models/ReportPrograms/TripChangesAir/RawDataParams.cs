using System.Collections.Generic;

namespace Domain.Models.ReportPrograms.TripChangesAir
{
    public class RawDataParams
    {
        public List<RawData> CancelledRawDataList { get; set; } = new List<RawData>();
        public List<RawData> RoutingRawDataList { get; set; } = new List<RawData>();
        public List<RawData> RawDataList { get; set; } = new List<RawData>();
    }
}
