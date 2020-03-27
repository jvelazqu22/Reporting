using System;

using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.SameCityReport
{
    [Serializable]
    public class RawData : ICollapsible, IRouteWhere
    {
        public DateTime? BookDate { get; set; }
        public DateTime? InvDate { get; set; }
        public string AirCurrTyp { get; set; }
        public string Recloc { get; set; }
        public int RecKey { get; set; }
        public string Invoice { get; set; }
        public string Ticket { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public int Plusmin { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public string Airline { get; set; }
        public string Mode { get; set; }
        public string Connect { get; set; }
        public string Arrtime { get; set; }
        public string Class { get; set; }
        public string Classcat { get; set; }
        public string Farebase { get; set; }
        public string DitCode { get; set; }
        public string Seat { get; set; }
        public string Stops { get; set; }
        public string Segstatus { get; set; }
        public string Tktdesig { get; set; }
        public int Rplusmin { get; set; }
        public string OrigOrigin { get; set; }
        public string OrigDest { get; set; }
        public string OrigCarr { get; set; }

        public string DomIntl { get; set; }

        public int Miles { get; set; }

        public decimal ActFare { get; set; }

        public decimal MiscAmt { get; set; }

        public int SeqNo { get; set; }

        public DateTime? RDepDate { get; set; }

        public string DepTime { get; set; }

        public string fltno { get; set; }

        public string ClassCode { get; set; }

        public int Seg_Cntr { get; set; }

        public DateTime? RArrDate { get; set; }
        public decimal AirCo2 { get; set; } = 0;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }
}
