using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TopBottomCityPairReport
{
    [Serializable]
    public class RawData : IFareByMileage, IRouteWhere, ICarbonCalculations, ICollapsible
    {
        public int RecKey { get; set; } = 0;

        [ExchangeDate3]
        public DateTime? RDepDate { get; set; } = DateTime.Now;

        public string DepTime { get; set; }

        [ExchangeDate2]
        public DateTime? BookDate { get; set; } = DateTime.Now;

        [ExchangeDate1]
        public DateTime? InvDate { get; set; } = DateTime.Now;

        public DateTime? RArrDate { get; set; }

        [AirCurrency]
        public string AirCurrTyp { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; } = 0m;

        [Currency(RecordType = RecordType.Air)]
        public decimal ActFare { get; set; } = 0m;

        public decimal MiscAmt { get; set; } = 0m;
        public string Connect { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal Faretax { get; set; } = 0m;

        [Currency(RecordType = RecordType.Air)]
        public decimal BaseFare { get; set; } = 0m;

        public string Orgdestemp { get; set; } = string.Empty;//TODO: VERIFY TYPE: space(6) as orgdestemp
        public string Bktool { get; set; } = string.Empty;
        public int Plusmin { get; set; } = 0;
        public decimal NumTicks { get; set; } = 0;//TODO: VERIFY TYPE: 00.000 as NumTicks
        public decimal OnlineTkts { get; set; } = 0; //TODO: VERIFY TYPE: 00.000 as onlineTkts
        public decimal AgentTkts { get; set; } = 0; //TODO: VERIFY TYPE: 00.000 as agentTkts
        public int RecordNo { get; set; } = 0;//TODO: VERIFY TYPE: 00000000 as RecordNo
        public bool Exchange { get; set; } = false;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string DomIntl { get; set; }
        public int Miles { get; set; } = 0;
        public string DitCode { get; set; } = string.Empty;
        public int SeqNo { get; set; } = 0;
        public string Airline { get;set; } = string.Empty;
        public string fltno { get; set; }
        public string ClassCode { get; set; }
        public int Seg_Cntr { get; set; }
        public string Mode { get;set; } = string.Empty;
        public string ClassCat { get; set; } = string.Empty;
        public decimal AirCo2 { get; set; } = 0m;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
        public string HaulType { get; set; } = string.Empty;
    }
}

