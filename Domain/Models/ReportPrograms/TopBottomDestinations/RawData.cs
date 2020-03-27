using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TopBottomDestinations
{
    [Serializable]
    public class RawData : IRecKey, ICollapsible
    {
        [ExchangeDate1]
        public DateTime? BookDate { get; set; }
        [ExchangeDate2]
        public DateTime? InvDate { get; set; }
        [AirCurrency]
        public string AirCurrTyp { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; } = 0m;

        public int RecKey { get; set; } = 0;
        public string SourceAbbr { get; set; } = string.Empty;
        public string ValCarr { get; set; } = string.Empty;
        public int Plusmin { get; set; } = 0;
        public string FirstOrg { get; set; } 
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
 
        public DateTime Rarrdate { get; set; } = DateTime.MinValue;
        public string Arrtime { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string Classcat { get; set; } = string.Empty;
        public int SeqNo { get; set; } = 0;
        public int Miles { get; set; } = 0;
        public string Farebase { get; set; } = string.Empty;
        public string DitCode { get; set; } = string.Empty;

        public string Tktdesig { get; set; } = string.Empty;
        public int Rplusmin { get; set; } = 0;
        public string OrigOrigin { get; set; } = string.Empty;
        public string OrigDest { get; set; } = string.Empty;
        public string OrigCarr { get; set; } = string.Empty;

        // Collapse data
        public string DomIntl { get; set; }
        public decimal ActFare { get; set; } = 0m;
        public decimal MiscAmt { get; set; } = 0m;
        public DateTime? RDepDate { get; set; } = DateTime.MinValue;
        public string DepTime { get; set; }
        public string fltno { get; set; } = string.Empty;
        public string ClassCode { get; set; }
        public int Seg_Cntr { get; set; }
        public decimal AirCo2 { get; set; } = 0;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }
}
