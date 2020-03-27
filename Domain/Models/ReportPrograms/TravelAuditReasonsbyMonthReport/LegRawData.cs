using System;

using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TravelAuditReasonsbyMonthReport
{
    public class LegRawData : IRouteWhere, ICollapsible
    {
        public LegRawData()
        {
            RecKey = 0;
            Acct = string.Empty;
            TravAuthNo = 0;
            Origin = string.Empty;
            Destinat = string.Empty;
            Airline = string.Empty;
            RDepDate = DateTime.MinValue;
            RArrDate = DateTime.MinValue;
            DepTime = string.Empty;
            Class = string.Empty;
            Connect = string.Empty;
            Mode = string.Empty;
            SeqNo = 0;
            Segnum = 0;
            ActFare = 0m;
            MiscAmt = 0m;
            Miles = 0;
            fltno = string.Empty;
            DitCode = string.Empty;
        }
        public int RecKey { get; set; }
        public string Acct { get; set; }
        public int TravAuthNo { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public decimal ActFare { get; set; }
        public decimal MiscAmt { get; set; }
        public string DepTime { get; set; }
        public string Airline { get; set; }
        public string fltno { get; set; }
        public string ClassCode { get; set; }
        public int Seg_Cntr { get; set; }
        public string Class { get; set; }
        public string Connect { get; set; }
        public string Mode { get; set; }
        public int SeqNo { get; set; }

        public DateTime? RDepDate { get; set; }
        public DateTime? RArrDate { get; set; }
        public int Segnum { get; set; }
        public string DomIntl { get; set; }
        public int Miles { get; set; }
        public string DitCode { get; set; }
        public decimal AirCo2 { get; set; } = 0;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }
}
