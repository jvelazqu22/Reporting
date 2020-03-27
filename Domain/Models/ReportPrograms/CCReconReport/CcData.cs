using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.CCReconReport
{
    public class CcData : IRouteItineraryInformation
    {
        public int RecKey { get; set; }
        public string CardType { get; set; } = string.Empty;
        public string CardNum { get; set; } = string.Empty;
        public decimal TransAmt { get; set; }
        public string RefNbr { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string Trantype { get; set; } = string.Empty;
        public string PassName { get; set; } = string.Empty;
        public DateTime TranDate { get; set; } = DateTime.MinValue;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public int SeqNo { get; set; }
        public string Mode { get; set; } = string.Empty;
        public string OrigOrigin { get; set; } = string.Empty;
        public string OrigDest { get; set; } = string.Empty;
        public string Descript { get; set; } = string.Empty;
    }
}
