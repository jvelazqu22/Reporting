using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.TopTravelersAuditedReport
{
    public class LegRawData : ICollapsible, IRouteWhere
    {
        public int RecKey { get; set; } = 0;
        public int TravAuthNo { get; set; } = 0;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public decimal ActFare { get; set; } = 0m;
        public decimal MiscAmt { get; set; } = 0m;
        public string DepTime { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;

        public string ClassCode { get; set; } = string.Empty;
        public int Seg_Cntr { get; set; }
        public string Class { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public int SeqNo { get; set; } = 0;
        public DateTime? RDepDate { get; set; } = DateTime.MinValue;
        public DateTime? RArrDate { get; set; }
        public int Segnum { get; set; } = 0;
        public string DomIntl { get; set; } = string.Empty;
        public int Miles { get; set; } = 0;
        public string fltno { get; set; } = string.Empty;
        public string DitCode { get; set; } = string.Empty;
        public decimal AirCo2 { get; set; } = 0;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }
}
