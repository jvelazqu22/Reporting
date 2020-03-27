using System;
using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TravelDetail
{
    [Serializable]
    public class TravDetRawData : IRouteWhere, ICollapsible
    {
        public string OrigTicket { get; set; } = string.Empty;

        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;

        [Currency(RecordType = RecordType.Air)]
        public decimal AirChg { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal Basefare { get; set; } = 0m;

        [Currency(RecordType = RecordType.Air)]
        public decimal Offrdchg { get; set; } = 0m;

        [Currency(RecordType = RecordType.Air)]
        public decimal SvcFee { get; set; } = 0m;

        [ExchangeDate1]
        public DateTime? InvDate { get; set; }

        public bool Exchange { get; set; }

        public string ValCarr { get; set; } = string.Empty;

        public string ValCarMode { get; set; } = string.Empty;

        public DateTime? TripStart { get; set; }

        public DateTime? TripEnd { get; set; }

        public string TranType { get; set; } = string.Empty;

        

        public string SegStatus { get; set; } = string.Empty;

        public string Invoice { get; set; } = string.Empty;
        [ExchangeDate2]
        public DateTime? DepDate { get; set; }
        [ExchangeDate3]
        public DateTime? ArrDate { get; set; }

        public string RecLoc { get; set; } = string.Empty;

        public int RecKey { get; set; } = 0;
        public int SeqNo { get; set; } = 0;
        public DateTime? RDepDate { get; set; }
        public DateTime? RArrDate { get; set; }
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Reascode { get; set; } = string.Empty;
        
        public string Ticket { get; set; } = string.Empty;
        public string PassLast { get; set; } = string.Empty;
        public string PassFrst { get; set; } = string.Empty;
        public string DitCode { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        
        public decimal Stndchg { get; set; } = 0m;
        public string Savingcode { get; set; } = string.Empty;
        public string SourceAbbr { get; set; } = string.Empty;
        public int PlusMin { get; set; } = 0;

        public decimal AirCo2 { get; set; }
        public decimal AltCarCo2 { get; set; }
        public decimal AltRailCo2 { get; set; }
        public string Udidtext { get; set; }


        // ICollapsible
        public string DomIntl { get; set; }
        public int Miles { get; set; }
        public decimal ActFare { get; set; }
        public decimal MiscAmt { get; set; }
        public string DepTime { get; set; }
        public string fltno { get; set; }
        public string ClassCode { get; set; }
        public int Seg_Cntr { get; set; }
    }
}
