using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ExecutiveSummaryByHomeCountryReport
{
    public class CarRawData : IRecKey
    {
        public string HomeCtry { get; set; } = string.Empty;
        [CarCurrency]
        public string CarCurrTyp { get; set; }
        [ExchangeDate3]
        public DateTime? BookDate { get; set; }
        [ExchangeDate2]
        public DateTime? InvDate { get; set; }
        public string SourceAbbr { get; set; } = string.Empty;
        public int CPlusMin { get; set; } = 0;
        public int Days { get; set; } = 0;
        [Currency(RecordType = RecordType.Car)]
        public decimal ABookRat { get; set; } = 0m;

        public DateTime? RentDate { get; set; } = DateTime.Now;
        [ExchangeDate1]
        public DateTime ReturnDate { get { return RentDate.GetValueOrDefault().AddDays(Days); } }

        public int RecKey { get; set; }
    }
}
