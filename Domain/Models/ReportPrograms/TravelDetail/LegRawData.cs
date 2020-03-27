using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TravelDetail
{
    [Serializable]
    public class LegRawData : IRouteWhere, ICarbonCalculations, ICollapsible, IAirMileage
    {
        public string Connect { get; set; } = string.Empty;

        public string Class { get; set; } = string.Empty;
        public string ArrTime { get; set; } = string.Empty;

        public string DepTime { get; set; } = string.Empty;

        public string fltno { get; set; } = string.Empty;

        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        public DateTime? TripStart { get; set; }
        public string SourceAbbr { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public int SeqNo { get; set; } = 0;
        public DateTime? RDepDate { get; set; }
        public DateTime? RArrDate { get; set; }
        public bool Exchange { get; set; }
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        [ExchangeDate1]
        public DateTime? Invdate { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal ActFare { get; set; } = 0m;
        public string Udidtext { get; set; }


        public int Plusmin { get; set; }
        // Air Carbon Calc fields
        public string ClassCat { get; set; }
        public int Miles { get; set; }
        public string DitCode { get; set; }
        public decimal AirCo2 { get; set; }
        public decimal AltCarCo2 { get; set; }
        public decimal AltRailCo2 { get; set; }
        public string HaulType { get; set; }

        // ICollapsible
        public string DomIntl { get; set; }
        public decimal MiscAmt { get; set; }
        public string ClassCode { get; set; }
        public int Seg_Cntr { get; set; }
    }
}
