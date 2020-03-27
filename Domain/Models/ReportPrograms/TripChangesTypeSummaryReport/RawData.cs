using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.TripChangesTypeSummaryReport
{
    public class RawData : IRecKey
    {
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        public string AirCurrTyp { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public int ChangeCode { get; set; } = 0;
        public string ChangeDesc { get; set; } = string.Empty;
        public string ChangeGrp { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public DateTime? Rdepdate { get; set; } = DateTime.MinValue;
        public string Fltno { get; set; } = string.Empty;
        public string Deptime { get; set; } = string.Empty;
        public DateTime? Rarrdate { get; set; } = DateTime.MinValue;
        public string Arrtime { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public string Classcat { get; set; } = string.Empty;
        public int Seqno { get; set; } 
        public int Miles { get; set; } 
        public decimal Actfare { get; set; } = 0m;
        public decimal Miscamt { get; set; } = 0m;
        public string Farebase { get; set; } = string.Empty;
        public string DitCode { get; set; } = string.Empty;
        public int Segnum { get; set; } 
        public string Seat { get; set; } = string.Empty;
        public string Stops { get; set; } = string.Empty;
        public string Segstatus { get; set; } = string.Empty;
        public string Tktdesig { get; set; } = string.Empty;
        public int Rplusmin { get; set; } 
        public string OrigOrigin { get; set; } = string.Empty;
        public string OrigDest { get; set; } = string.Empty;
        public string OrigCarr { get; set; } = string.Empty;
        public int NumChngs { get; set; } = 0;
    }
}
