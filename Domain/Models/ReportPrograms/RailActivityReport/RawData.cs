using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.RailActivityReport
{
    [Serializable]
    public class RawData : IRouteWhere, ICollapsible
    {
        public RawData()
        {
            Exchange = false;
            BookDate = DateTime.MinValue;
            DepDate = DateTime.MinValue;
            InvDate = DateTime.MinValue;
            RDepDate = DateTime.MinValue;
            SortDate = DateTime.MinValue;
            RArrDate = DateTime.MinValue;
            AirChg = 0;
            OffRdChg = 0;
            SvcFee = 0;
            ActFare = 0;
            MiscAmt = 0;
            Miles = 0;
            PlusMin = 0;
            RecKey = 0;
            RplusMin = 0;
            SeqNo = 0;
            Acct = string.Empty;
            AirCurrTyp = string.Empty;
            Airline = string.Empty;
            ArrTime = string.Empty;
            Break1 = string.Empty;
            Break2 = string.Empty;
            Break3 = string.Empty;
            CardNum = string.Empty;
            ClassCat = string.Empty;
            ClassCode = string.Empty;
            Connect = string.Empty;
            DepTime = string.Empty;
            Destinat = string.Empty;
            DitCode = string.Empty;
            FareBase = string.Empty;
            Flt_Mkt = string.Empty;
            Flt_Mkt2 = string.Empty;
            fltno = string.Empty;
            Invoice = string.Empty;
            Mode = string.Empty;
            OrigCarr = string.Empty;
            OrigDest = string.Empty;
            Origin = string.Empty;
            OrigOrigin = string.Empty;
            OrigTicket = string.Empty;
            PassFrst = string.Empty;
            PassLast = string.Empty;
            PseudoCity = string.Empty;
            RecLoc = string.Empty;
            Seat = string.Empty;
            SegStatus = string.Empty;
            SFTranType = string.Empty;
            Stops = string.Empty;
            Ticket = string.Empty;
            TktDesig = string.Empty;
            TranType = string.Empty;
        }

        public string Acct { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal? ActFare { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal AirChg { get; set; }

        [AirCurrency]
        public string AirCurrTyp { get; set; }

        public string Airline { get; set; }

        public string ArrTime { get; set; }

        [ExchangeDate2]
        public DateTime? BookDate { get; set; }

        public string Break1 { get; set; }

        public string Break2 { get; set; }

        public string Break3 { get; set; }

        public string CardNum { get; set; }

        public string ClassCat { get; set; }

        public string ClassCode { get; set; }
        public string Connect { get; set; }
        public DateTime? DepDate { get; set; }
        public string DepTime { get; set; }
        public string Destinat { get; set; }
        public string DitCode { get; set; }
        public string DomIntl { get; set; }
        public bool Exchange { get; set; }
        public string FareBase { get; set; }
        public string Flt_Mkt { get; set; }
        public string Flt_Mkt2 { get; set; }
        public string fltno { get; set; }
        decimal ICollapsible.ActFare { get; set; }
        decimal ICollapsible.MiscAmt { get; set; }
        [ExchangeDate1]
        public DateTime? InvDate { get; set; }

        public string Invoice { get; set; }
        public int Miles { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal? MiscAmt { get; set; }

        public string Mode { get; set; }
        public decimal OffRdChg { get; set; }
        public string OrigCarr { get; set; }
        public string OrigDest { get; set; }
        public string Origin { get; set; }
        public string OrigOrigin { get; set; }
        public string OrigTicket { get; set; }
        public string PassFrst { get; set; }
        public string PassLast { get; set; }
        public int PlusMin { get; set; }
        public string PseudoCity { get; set; }
        public DateTime? RArrDate { get; set; }
        public DateTime? RDepDate { get; set; }
        public int RecKey { get; set; }
        public string RecLoc { get; set; }
        public int RplusMin { get; set; }
        public string Seat { get; set; }
        public int Seg_Cntr { get; set; }
        public string SegStatus { get; set; }
        public int SeqNo { get; set; }
        public string SFTranType { get; set; }
        public DateTime? SortDate { get; set; }
        public string Stops { get; set; }
        public decimal SvcFee { get; set; }
        public string Ticket { get; set; }
        public string TktDesig { get; set; }
        public string TranType { get; set; }
        public decimal AirCo2 { get; set; } = 0;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }
}
