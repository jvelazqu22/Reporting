using System;

using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.UpcomingPlans
{
    [Serializable]
    public class RawData : IRouteWhere, ICollapsible
    {
        public RawData()
        {
            AirCurrTyp = string.Empty;
            Recloc = string.Empty;
            Acct = string.Empty;
            Break1 = string.Empty;
            Break2 = string.Empty;
            Break3 = string.Empty;
            Passlast = string.Empty;
            Passfrst = string.Empty;
            Trantype = string.Empty;
            Origin = string.Empty;
            Destinat = string.Empty;
            Airline = string.Empty;
            Mode = string.Empty;
            Arrtime = string.Empty;
            ClassCode = string.Empty;
            Classcat = string.Empty;
            DitCode = string.Empty;

            BookDate = DateTime.MinValue;
            InvDate = DateTime.MinValue;
            RArrDate = DateTime.MinValue;
            RDepDate = DateTime.MinValue;
        }

        public DateTime? BookDate { get; set; }
        public DateTime? InvDate { get; set; }
        public string AirCurrTyp { get; set; }
        public string Recloc { get; set; }
        public int RecKey { get; set; }
        public string Connect { get; set; }
        public int SeqNo { get; set; }
        public string Acct { get; set; }
        public string Break1 { get; set; }
        public string Break2 { get; set; }
        public string Break3 { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string Trantype { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public decimal ActFare { get; set; }
        public decimal MiscAmt { get; set; }
        public string DepTime { get; set; }
        public string Airline { get; set; }
        public string fltno { get; set; }
        public string Mode { get; set; }
        public DateTime? RArrDate { get; set; }
        public string Arrtime { get; set; }
        public string ClassCode { get; set; }
        public int Seg_Cntr { get; set; }
        public string Classcat { get; set; }
        public string DomIntl { get; set; }
        public int Miles { get; set; }
        public string DitCode { get; set; }
        public DateTime? RDepDate { get; set; }
        public decimal AirCo2 { get; set; } = 0;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }
}
