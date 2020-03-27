using System;

using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.DeparturesReport
{
    [Serializable]
    public class RawData : IRouteWhere, ICollapsible
    {
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public decimal ActFare { get; set; } = 0m;
        public decimal? AirChg { get; set; } = 0m;
        public string AirCurrTyp { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string ArrTime { get; set; } = string.Empty;
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        public string ClassCat { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public string DepTime { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string DitCode { get; set; } = string.Empty;
        public string DomIntl { get; set; } = string.Empty;
        public string FareBase { get; set; } = string.Empty;
        public string fltno { get; set; } = string.Empty;
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        public string Invoice { get; set; } = string.Empty;
        public int Miles { get; set; } = 0;
        public decimal MiscAmt { get; set; } = 0;
        public string Mode { get; set; } = string.Empty;
        public decimal? OffRdChg { get; set; } = 0m;
        public string OrigCarr { get; set; } = string.Empty;
        public string OrigDest { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string OrigOrigin { get; set; } = string.Empty;
        public string PassFrst { get; set; } = string.Empty;
        public string PassLast { get; set; } = string.Empty;
        public int PlusMin { get; set; } = 0;
        public string PseudoCity { get; set; } = string.Empty;
        public DateTime? RArrDate { get; set; } = DateTime.MinValue;
        public DateTime? RDepDate { get; set; } = DateTime.MinValue;
        public int RecKey { get; set; } = 0;
        public string RecLoc { get; set; } = string.Empty;
        public int RplusMin { get; set; } = 0;
        public string Seat { get; set; } = string.Empty;
        public int Seg_Cntr { get; set; } = 0;
        public int SegNum { get; set; } = 0;
        public string SegStatus { get; set; } = string.Empty;
        public int SeqNo { get; set; } = 0;
        public string Stops { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string TktDesig { get; set; } = string.Empty;
        public decimal AirCo2 { get; set; } = 0m;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }
}
