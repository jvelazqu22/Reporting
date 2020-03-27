using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.AirActivityReport
{
    [Serializable]
    public class RawData : IRouteWhere, ICarbonCalculations, IFlightSegment, ICollapsible, IAirMileage
    {
        public string Recloc { get; set; } = string.Empty;
        public int RecKey { get; set; }
        public string Invoice { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public decimal Svcfee { get; set; }
        public string SfTranType { get; set; } = string.Empty;
        public DateTime? Depdate { get; set; } = DateTime.MinValue;
        public string Cardnum { get; set; } = string.Empty;
        public string Pseudocity { get; set; } = string.Empty;
        public string Trantype { get; set; } = string.Empty;
        public string Origticket { get; set; } = string.Empty;
        public int Plusmin { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public DateTime? RDepDate { get; set; } = DateTime.MinValue;
        public string ClassCode { get; set; } = string.Empty;
        public int SeqNo { get; set; }
        public int Miles { get; set; }
        public DateTime? Sortdate { get; set; } = DateTime.MinValue;
        public string AcctDesc { get; set; } = string.Empty;
        public string OrgDesc { get; set; } = string.Empty;
        public string DestDesc { get; set; } = string.Empty;
        public decimal AirCo2 { get; set; }
        public decimal AltCarCo2 { get; set; }
        public decimal AltRailCo2 { get; set; }
        public string HaulType { get; set; } = string.Empty;
        public string Tktdesig { get; set; } = string.Empty;
        public bool Exchange { get; set; }
        public string Flt_mkt { get; set; } = string.Empty;
        public string Flt_mkt2 { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Offrdchg { get; set; }
        [ExchangeDate1]
        public DateTime? Invdate { get; set; } = DateTime.MinValue;
        [ExchangeDate2]
        public DateTime? Bookdate { get; set; } = DateTime.MinValue;
        public string DepTime { get; set; } = string.Empty;
        public string fltno { get; set; } = string.Empty;
        public int Seg_Cntr { get; set; }
        public DateTime? RArrDate { get; set; } = DateTime.MinValue;
        public string Arrtime { get; set; } = string.Empty;
        public string ClassCat { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal ActFare { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal MiscAmt { get; set; }
        public string Farebase { get; set; } = string.Empty;
        public string DitCode { get; set; } = string.Empty;
        public string Seat { get; set; } = string.Empty;
        public string Stops { get; set; } = string.Empty;
        public string Segstatus { get; set; } = string.Empty;
        public int Rplusmin { get; set; }
        public string OrigOrigin { get; set; } = string.Empty;
        public string OrigDest { get; set; } = string.Empty;
        public string OrigCarr { get; set; } = string.Empty;
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        //Carb fields, may not be used
        public string DomIntl { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
    }
}
