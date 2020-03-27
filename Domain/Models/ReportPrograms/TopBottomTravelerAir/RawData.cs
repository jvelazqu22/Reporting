using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TopBottomTravelerAir
{
    [Serializable]
    public class RawData : ICollapsible, IRouteWhere
    {

        public int RecKey { get; set; }
        public string DitCode { get; set; }
        public string DomIntl { get; set; }
        public int Miles { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public string Mode { get; set; }
        public decimal ActFare { get; set; }
        public decimal MiscAmt { get; set; }
        public string Connect { get; set; }
        public int SeqNo { get; set; }
        public DateTime? RDepDate { get; set; }
        public DateTime? RArrDate { get; set; }
        public string DepTime { get; set; }
        public string Airline { get; set; }
        public string fltno { get; set; }
        public string ClassCode { get; set; }
        public int Seg_Cntr { get; set; }
        [AirCurrency]
        public string AirCurrTyp { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Offrdchg { get; set; }
        [ExchangeDate1]
        public DateTime? Invdate { get; set; }
        [ExchangeDate2]
        public DateTime? Bookdate { get; set; }
        public string SourceAbbr { get; set; }
        public DateTime? Depdate { get; set; }
        public int Plusmin { get; set; }
        public decimal AirCo2 { get; set; } = 0;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;

    }
}
