using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.UserDefinedReport
{
    public class UdidRawData : IRecKey
    {
        public int RecKey { get; set; }
        public int UdidNo { get; set; }
        public string UdidText { get; set; }
    }
}
