using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TopBottomUdids
{
    public class RawData : IRecKey
    {
        public RawData()
        {
            RecKey = 0;
            UdidText = string.Empty;
            UdidCount = 0;
        }

        public int UdidCount { get; set; }
        public int RecKey { get; set; }
        public string UdidText { get; set; }
    }
}
