using Domain.Helper;
using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.TravelManagementSummary
{
    public class LegRawData : ICarbonCalculations, IRecKey, IAirMileage
    {
        public LegRawData()
        {
            RecKey = 0;
            Plusmin = 0;
            SeqNo = 0;
            Miles = 0;
            Actfare = 0m;
            Miscamt = 0m;
            Rplusmin = 0;
        }
        public int RecKey { get; set; } = 0;
        [ExchangeDate2]
        public DateTime BookDate { get; set; } = DateTime.MinValue;
        [ExchangeDate1]
        public DateTime InvDate { get; set; } = DateTime.MinValue;
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        public int Plusmin { get; set; } = 0;
        public string Domintl { get; set; } = string.Empty;
        public bool Exchange { get; set; }

        [ExchangeDate1]
        public DateTime UseDate { get; set; } = DateTime.MinValue;
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
        public int SeqNo { get; set; } = 0;

        public string ClassCat { get; set; } = string.Empty;
        public int Miles { get; set; } = 0;
        [Currency(RecordType = RecordType.Air)]
        public decimal Actfare { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal Miscamt { get; set; } = 0m;

        public string Farebase { get; set; } = string.Empty;
        public string DitCode { get; set; } = string.Empty;
        public decimal AirCo2 { get; set; } = 0m;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
        public string HaulType { get; set; } = string.Empty;
        public string Seat { get; set; } = string.Empty;
        public string Stops { get; set; } = string.Empty;
        public string Segstatus { get; set; } = string.Empty;
        public string Tktdesig { get; set; } = string.Empty;
        public int Rplusmin { get; set; } = 0;
        public string OrigOrigin { get; set; } = string.Empty;
        public string OrigDest { get; set; } = string.Empty;
        public string OrigCarr { get; set; } = string.Empty;
    }

}
