using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ExecutiveSummaryYearToYearReport
{
    public class LegRawData : ICarbonCalculations,  IRecKey, IAirMileage
    {
        [AirCurrency]
        public string AirCurrTyp { get; set; }
        public int RecKey { get; set; }
        public DateTime? UseDate { get; set; } = DateTime.MinValue;
        public int PlusMin { get; set; } = 0;
        public string ClassCat { get; set; }
        public int Miles { get; set; } = 0;
        public string DitCode { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal AirCo2 { get; set; } = 0;
        [Currency(RecordType = RecordType.Air)]
        public decimal AltCarCo2 { get; set; } = 0;
        [Currency(RecordType = RecordType.Air)]
        public decimal AltRailCo2 { get; set; } = 0;
        public string HaulType { get; set; } = string.Empty;
        public bool Exchange { get; set; } = false;
        public string Mode { get; set; } = string.Empty;
        [ExchangeDate2]
        public DateTime? Bookdate { get; set; }
        [ExchangeDate1]
        public DateTime? Invdate { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
    }
}
