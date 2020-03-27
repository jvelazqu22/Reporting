using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.CarrierConcentrationReport
{
    [Serializable]
    public class RawData : IRouteWhere, IFareByMileage, ICollapsible, IAirMileage
    {
        public RawData()
        {
            BookDate = DateTime.Now;
            InvDate = DateTime.Now;
            AirCurrTyp = string.Empty;
            RecKey = 0;
            Recloc = string.Empty;
            Valcarr = string.Empty;
            Airchg = 0m;
            BaseFare = 0m;
            Faretax = 0m;
            Seg_Cntr = 0;
            Acct = string.Empty;
            Flt_mkt = string.Empty;
            Flt_mkt2 = string.Empty;
            Orgdestemp = string.Empty;
            RecordNo = 0;
            Plusmin = 0;
            Origin = string.Empty;
            Destinat = string.Empty;
            Airline = string.Empty;
            Mode = string.Empty;
            Connect = string.Empty;
            RDepDate = DateTime.Now;
            fltno = string.Empty;
            DepTime = string.Empty;
            RArrDate = DateTime.Now;
            Arrtime = string.Empty;
            ClassCode = string.Empty;
            Classcat = string.Empty;
            SeqNo = 0;
            Miles = 0;
            ActFare = 0m;
            MiscAmt = 0m;
            Farebase = string.Empty;
            DitCode = string.Empty;
            Seat = string.Empty;
            Segstatus = string.Empty;
            Tktdesig = string.Empty;
            Rplusmin = 0;
            OrigOrigin = string.Empty;
            OrigDest = string.Empty;
            OrigCarr = string.Empty;
        }
        [ExchangeDate2]
        public DateTime? BookDate { get; set; }
        [ExchangeDate1]
        public DateTime? InvDate { get; set; }
        [AirCurrency]
        public string AirCurrTyp { get; set; }
        public int RecKey { get; set; }
        public string Recloc { get; set; }
        public string Valcarr { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal BaseFare { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Faretax { get; set; }
        public int Seg_Cntr { get; set; }
        public string Acct { get; set; }
        public string Flt_mkt { get; set; }
        public string Flt_mkt2 { get; set; }
        public string Orgdestemp { get; set; }
        public int RecordNo { get; set; }
        public int Plusmin { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public string Airline { get; set; }
        public string Mode { get; set; }
        public string Connect { get; set; }
        public DateTime? RDepDate { get; set; }
        public string fltno { get; set; }
        public string DepTime { get; set; }
        public DateTime? RArrDate { get; set; }
        public string Arrtime { get; set; }
        public string ClassCode { get; set; }
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
        public string Segstatus { get; set; }
        public string Tktdesig { get; set; }
        public int Rplusmin { get; set; }
        public string OrigOrigin { get; set; }
        public string OrigDest { get; set; }
        public string OrigCarr { get; set; }
        public decimal AirCo2 { get; set; }
        public decimal AltCarCo2 { get; set; }
        public decimal AltRailCo2 { get; set; }
    }
}
