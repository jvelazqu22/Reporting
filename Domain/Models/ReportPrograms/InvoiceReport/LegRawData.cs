using System;

using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.InvoiceReport
{
    public class LegRawData : IRecKey
    {
        public int RecKey { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; }
        public string Airline { get; set; } = string.Empty;
        public string FltNo { get; set; } = string.Empty;
        public DateTime? RDepDate { get; set; } = DateTime.MinValue;
        public string Deptime { get; set; } = string.Empty;
        public DateTime? RArrDate { get; set; } = DateTime.MinValue;
        public string Arrtime { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
    }
}
