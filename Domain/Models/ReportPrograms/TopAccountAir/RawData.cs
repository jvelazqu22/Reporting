using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TopAccountAir
{
    [Serializable]
    public class RawData : IRouteWhere, ICollapsible
    {
        public RawData()
        {
            BookDate = DateTime.MinValue;
            InvDate = DateTime.MinValue;
            AirCurrTyp = string.Empty;
            SourceAbbr = string.Empty;
            Acct = string.Empty;
            AcctName = string.Empty;
            AirChg = 0m;
            OffrdChg = 0m;
            ACommisn = 0m;
            SvcFee = 0m;
            Plusmin = 0;
            Origin = string.Empty;
            Destinat = string.Empty;
            Airline = string.Empty;
            Mode = string.Empty;
            Connect = string.Empty;
            RDepDate = DateTime.MinValue;
            fltno = string.Empty;
            DepTime = string.Empty;
            RArrDate = DateTime.MinValue;
            ArrTime = string.Empty;
            Class = string.Empty;
            ClassCode = string.Empty;
            Classcat = string.Empty;
            SeqNo = 0;
            Miles = 0;
            ActFare = 0m;
            MiscAmt = 0m;
            Farebase = string.Empty;
            DitCode = string.Empty;
            Seat = string.Empty;
            Stops = string.Empty;
            Segstatus = string.Empty;
            Tktdesig = string.Empty;
            Rplusmin = 0;
            OrigOrigin = string.Empty;
            OrigDest = string.Empty;
            OrigCarr = string.Empty;
        }
        public int RecKey { get; set; }
        [ExchangeDate1]
        public DateTime? BookDate { get; set; }
        [ExchangeDate2]
        public DateTime? InvDate { get; set; }
        [AirCurrency]
        public string AirCurrTyp { get; set; }
        public string SourceAbbr { get; set; }
        public string Acct { get; set; }
        public string AcctName { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal AirChg { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal OffrdChg { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal ACommisn { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal SvcFee { get; set; }
        public int Plusmin { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public string Airline { get; set; }
        public string fltno { get; set; }
        public string Mode { get; set; }
        public string Connect { get; set; }
        public DateTime? RDepDate { get; set; }
        public string DepTime { get; set; }
        public DateTime? RArrDate { get; set; }
        public string ArrTime { get; set; }
        public string Class { get; set; }
        public string ClassCode { get; set; }
        public int Seg_Cntr { get; set; }
        public string Classcat { get; set; }
        public int SeqNo { get; set; }
        public string DomIntl { get; set; }
        public int Miles { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal ActFare { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal MiscAmt { get; set; }
        public string Farebase { get; set; }
        public string DitCode { get; set; }
        public string Seat { get; set; }
        public string Stops { get; set; }
        public string Segstatus { get; set; }
        public string Tktdesig { get; set; }
        public int Rplusmin { get; set; }
        public string OrigOrigin { get; set; }
        public string OrigDest { get; set; }
        public string OrigCarr { get; set; }
        public decimal AirCo2 { get; set; } = 0;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }
}
