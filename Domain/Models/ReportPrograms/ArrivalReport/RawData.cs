using System;

using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ArrivalReport
{
    [Serializable]
    public class RawData : IRouteWhere, ICollapsible
    {

        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        public string AirCurrTyp { get; set; } = string.Empty;
        public string Recloc { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public string Invoice { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string Pseudocity { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string DomIntl { get; set; } = string.Empty;
        public decimal? Airchg { get; set; } = 0m;
        public decimal? Offrdchg { get; set; } = 0m;
        public int Seg_Cntr { get; set; } = 0;
        public int PlusMin { get; set; } = 0;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public DateTime? RDepDate { get; set; } = DateTime.MinValue;
        public string fltno { get; set; } = string.Empty;
        public DateTime? RArrDate { get; set; } = DateTime.MinValue;
        public string ArrTime { get; set; } = string.Empty;
        public string Classcat { get; set; } = string.Empty;
        public int SeqNo { get; set; } = 0;
        public int Miles { get; set; } = 0;
        public decimal ActFare { get; set; } = 0m;
        public decimal MiscAmt { get; set; } = 0m;
        public string Farebase { get; set; } = string.Empty;
        public string DitCode { get; set; } = string.Empty;
        public string Seat { get; set; } = string.Empty;
        public string Stops { get; set; } = string.Empty;
        public string Segstatus { get; set; } = string.Empty;
        public string Tktdesig { get; set; } = string.Empty;
        public int Rplusmin { get; set; } = 0;
        public string OrigOrigin { get; set; } = string.Empty;
        public string OrigDest { get; set; } = string.Empty;
        public string OrigCarr { get; set; } = string.Empty;
        public string DepTime { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public int SegNum { get; set; } = 0;
        public decimal AirCo2 { get; set; } = 0m;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }
}
