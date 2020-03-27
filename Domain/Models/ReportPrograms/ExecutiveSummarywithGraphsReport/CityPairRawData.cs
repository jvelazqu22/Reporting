using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ExecutiveSummarywithGraphsReport
{
    public class CityPairRawData :  ICarbonCalculations, IFareByMileage, IRouteWhere, IAirMileage
    {
        public CityPairRawData()
        {
            Recloc = string.Empty;
            RecKey = 0;
            SeqNo = 0;
            Origin = string.Empty;
            Destinat = string.Empty;
            Connect = string.Empty;
            Airline = string.Empty;
            ActFare = 0m;
            BaseFare = 0m;
            RDepDate = DateTime.Now;
            RArrDate = DateTime.Now;
            Class = string.Empty;
            Fltno = string.Empty;
            Deptime = string.Empty;
            Arrtime = string.Empty;
            Mode = string.Empty;
            Miles = 0;
            Miscamt = 0m;
            Farebase = string.Empty;
            Plusmin = 0;
            DitCode = string.Empty;
            Exchange = false;
            ClassCat = string.Empty;

            DitCode = string.Empty;
            HaulType = string.Empty;
        }
        public string Recloc { get; set; }
        public int RecKey { get; set; }
        public int SeqNo { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public string Connect { get; set; }
        public string Airline { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal ActFare { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal BaseFare { get; set; }
        public DateTime? RDepDate { get; set; }
        public DateTime? RArrDate { get; set; }
        public string Class { get; set; }
        public string Fltno { get; set; }
        public string Deptime { get; set; }
        public string Arrtime { get; set; }
        public string Mode { get; set; }
        public int Miles { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Miscamt { get; set; }
        public string Farebase { get; set; }
        public int Plusmin { get; set; }

        public string DitCode { get; set; }

        public decimal AirCo2 { get; set; }
        public decimal AltCarCo2 { get; set; }
        public decimal AltRailCo2 { get; set; }
        public string HaulType { get; set; }
        public bool Exchange { get; set; }
        public string ClassCat { get; set; }

        [ExchangeDate1]
        public DateTime? BookDate { get; set; }
        [ExchangeDate2]
        public DateTime? InvDate { get; set; }
        [AirCurrency]
        public string AirCurrTyp { get; set; }
    }
}
