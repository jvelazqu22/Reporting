using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.TripChangesSendOffReport
{
    [Serializable]
    public class RawData : IRouteWhere, ICollapsible
    {
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        public string AirCurrTyp { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public string Acct { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string Mtggrpnbr { get; set; } = string.Empty;
        public DateTime? Bookdate { get; set; } = DateTime.MinValue;
        public string Emailaddr { get; set; } = string.Empty;
        public string FirstDest { get; set; } = string.Empty;
        public int Seg_Cntr { get; set; }
        public string Recloc { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public DateTime? RDepDate { get; set; } = DateTime.MinValue;
        public string fltno { get; set; } = string.Empty;
        public string DepTime { get; set; } = string.Empty;
        public DateTime? RArrDate { get; set; } = DateTime.MinValue;
        public string Arrtime { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public string Classcat { get; set; } = string.Empty;
        public int SeqNo { get; set; }
        public string DomIntl { get; set; } = string.Empty;
        public int Miles { get; set; }
        public decimal ActFare { get; set; } = 0m;
        public decimal MiscAmt { get; set; } = 0m;
        public string Farebase { get; set; } = string.Empty;
        public string DitCode { get; set; } = string.Empty;
        public int SegNum { get; set; }
        public string Seat { get; set; } = string.Empty;
        public string Stops { get; set; } = string.Empty;
        public string SegStatus { get; set; } = string.Empty;
        public string TktDesig { get; set; } = string.Empty;
        public int RPlusmin { get; set; }
        public string OrigOrigin { get; set; } = string.Empty;
        public string OrigDest { get; set; } = string.Empty;
        public string OrigCarr { get; set; } = string.Empty;
        public DateTime? ChangStamp { get; set; }
        public decimal AirCo2 { get; set; } = 0;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }
}
