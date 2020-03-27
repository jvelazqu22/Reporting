using System;

namespace Domain.Models.ReportPrograms.InvoiceReport
{
    public class SubReportData
    {
        public int Reckey { get; set; } = 0;
        public DateTime Trandate { get; set; } = DateTime.MinValue;
        public string Chargedesc { get; set; } = string.Empty;
        public decimal Charge { get; set; } = 0m;
        public string Taxname { get; set; } = string.Empty;
        public decimal Taxamt { get; set; } = 0m;
    }
}
