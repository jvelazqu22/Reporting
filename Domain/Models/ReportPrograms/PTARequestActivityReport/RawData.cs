using System;
using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.PTARequestActivityReport
{
    [Serializable]
    public class RawData : IRouteWhere, ICollapsible
    {
        [ExchangeDate1]
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        [ExchangeDate2]
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public DateTime? Depdate { get; set; } = DateTime.MinValue;
        public DateTime? Bookedgmt { get; set; } = DateTime.MinValue;
        public DateTime? Statustime { get; set; } = DateTime.MinValue;
        public string Recloc { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public int TravAuthNo { get; set; } = 0;
        public string Rtvlcode { get; set; } = string.Empty;
        public string OutPolCods { get; set; } = string.Empty;
        public int AuthrzrNbr { get; set; } = 0;
        public string AuthStatus { get; set; } = string.Empty;
        public string DetlStatus { get; set; } = string.Empty;
        public string ApvReason { get; set; } = string.Empty;
        public int ApSequence { get; set; } = 0;
        public DateTime? DetStatTim { get; set; } = DateTime.MinValue;
        public string DitCode { get; set; }
        public string DomIntl { get; set; }
        public int Miles { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public decimal ActFare { get; set; }
        public decimal MiscAmt { get; set; }
        public string Connect { get; set; }
        public string Mode { get; set; }
        public string DepTime { get; set; }
        public string Airline { get; set; }
        public string fltno { get; set; }
        public string ClassCode { get; set; }
        public int Seg_Cntr { get; set; }
        public int RecKey { get; set; } = 0;
        public int SeqNo { get; set; }
        public DateTime? RDepDate { get; set; }
        public DateTime? RArrDate { get; set; }
        public int Sgroupnbr { get; set; } = 0;
        public int RecordNo { get; set; } = 0;
        public string Auth1Email { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal AirChg { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal OffrdChg { get; set; } = 0m;
        public string CliAuthNbr { get; set; } = string.Empty;
        public decimal AirCo2 { get; set; } = 0;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }
}
