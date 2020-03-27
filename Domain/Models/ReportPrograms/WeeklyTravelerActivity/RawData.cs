using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.WeeklyTravelerActivity
{
    //there is no currency displayed in the report, hence no currency conversion or attribute decoration
    public class RawData : IRouteWhere
    {
        public string Acct { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal? ActFare { get; set; } = 0;
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string ArrTime { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string ClassCat { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public string DepTime { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string DitCode { get; set; } = string.Empty;
        public string FareBase { get; set; } = string.Empty;
        public string FltNo { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty;
        public int Miles { get; set; } = 0;
        public decimal? MiscAmt { get; set; } = 0;
        public string Mode { get; set; } = string.Empty;
        public string OrigCarr { get; set; } = string.Empty;
        public string OrigDest { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string OrigOrigin { get; set; } = string.Empty;
        public string PassFrst { get; set; } = string.Empty;
        public string PassLast { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public string RecLoc { get; set; } = string.Empty;
        public int RplusMin { get; set; } = 0;
        public string Seat { get; set; } = string.Empty;
        public string SegStatus { get; set; } = string.Empty;
        public int SeqNo { get; set; } = 0;
        public string Stops { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string TktDesig { get; set; } = string.Empty;
        [ExchangeDate1]
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        [ExchangeDate2]
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        [ExchangeDate3]
        public DateTime? DepDate { get; set; } = DateTime.MinValue;
        public DateTime? RArrDate { get; set; } = DateTime.MinValue;
        public DateTime? RDepDate { get; set; } = DateTime.MinValue;
        public DateTime? ArrDate { get; set; } = DateTime.MinValue;
    }
}
