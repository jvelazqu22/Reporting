using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.InvoiceReport
{
    public class UdidRawData : IRecKey
    {
        public int RecKey { get; set; } = 0;
        public int UdidNbr { get; set; } = 0;
        public string UdidText { get; set; } = string.Empty;
        public int Dummy { get; set; } = 0;
    }
}
