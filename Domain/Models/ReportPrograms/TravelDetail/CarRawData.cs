using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TravelDetail
{
    public class CarRawData : ICarbonCalculationsCar, IRecKey
    {
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        [ExchangeDate1]
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        public int RecKey { get; set; } = 0;
        public string Company { get; set; } = string.Empty;
        public string Autostat { get; set; } = string.Empty;
        public string Autocity { get; set; } = string.Empty;
        [ExchangeDate2]
        public DateTime? RentDate { get; set; } = DateTime.MinValue;
        public DateTime? DateBack { get; set; } = DateTime.MinValue;
        public string ReasCoda { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Car)]
        public decimal Abookrat { get; set; } = 0m;
        public string Milecost { get; set; } = string.Empty;
        public string RateType { get; set; } = string.Empty;
        public string Citycode { get; set; } = string.Empty;
        public string Confirmno { get; set; } = string.Empty;
        public string Cartrantyp { get; set; } = string.Empty;
        public string CarType { get; set; } = string.Empty;
        public int Days { get; set; } = 0;
        public decimal CarCo2 { get; set; }
        public int CPlusMin { get; set; } = 0;
        [CarCurrency]
        public string CarCurrTyp { get; set; }

        public string PassFrst { get; set; } = string.Empty;
        public string PassLast { get; set; } = string.Empty;
        public string RecLoc { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public DateTime? TripStart { get; set; }
        public DateTime? TripEnd { get; set; }
        public DateTime? DepDate { get; set; }
        public DateTime? ArrDate { get; set; }
        public string SourceAbbr { get; set; } = string.Empty;
        public string Udidtext { get; set; }

    }
}
