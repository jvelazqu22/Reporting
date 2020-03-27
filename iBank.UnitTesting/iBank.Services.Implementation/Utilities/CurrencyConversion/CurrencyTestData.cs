using System;

using Domain.Helper;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities.CurrencyConversion
{
    internal class ValidAirData
    {
        [ExchangeDate1]
        public DateTime DateOne { get; set; }
        [AirCurrency]
        public string CurrTyp { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Charge { get; set; }
    }

    internal class NoCurrencyTypeDecoration
    {
        [ExchangeDate1]
        public DateTime DateOne { get; set; }
        public string CurrTyp { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Charge { get; set; }
    }

    internal class NoDateDecoration
    {
        public DateTime DateOne { get; set; }
        [AirCurrency]
        public string CurrTyp { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Charge { get; set; }
    }

    internal class MismatchedCurrencyTypeAndRecordType
    {
        [ExchangeDate1]
        public DateTime DateOne { get; set; }
        [AirCurrency]
        public string CurrTyp { get; set; }
        [Currency(RecordType = RecordType.Hotel)]
        public decimal Charge { get; set; }
    }

    internal class InvalidCastData
    {
        [ExchangeDate1]
        public string DateOne { get; set; }
        [AirCurrency]
        public string CurrTyp { get; set; }
        [Currency(RecordType = RecordType.Hotel)]
        public decimal Charge { get; set; }
    }

    
    internal class ValidHotelData
    {
        [ExchangeDate1]
        public DateTime DateOne { get; set; }
        [HotelCurrency]
        public string CurrTyp { get; set; }
        [Currency(RecordType = RecordType.Hotel)]
        public decimal Charge { get; set; }
    }

    internal class ValidCarData
    {
        [ExchangeDate1]
        public DateTime DateOne { get; set; }
        [CarCurrency]
        public string CurrTyp { get; set; }
        [Currency(RecordType = RecordType.Car)]
        public decimal Charge { get; set; }
    }

    internal class ValidSvcFeeData
    {
        [ExchangeDate1]
        public DateTime DateOne { get; set; }
        [FeeCurrency]
        public string CurrTyp { get; set; }
        [Currency(RecordType = RecordType.SvcFee)]
        public decimal Charge { get; set; }
    }

    internal class ValidSvcAmtData
    {
        [ExchangeDate1]
        public DateTime DateOne { get; set; }
        [FeeCurrency]
        public string CurrTyp { get; set; }
        [Currency(RecordType = RecordType.SvcAmt)]
        public decimal Charge { get; set; }
    }

    internal class ValidMiscSegsData
    {
        [ExchangeDate1]
        public DateTime DateOne { get; set; }
        [MiscSegCurrency]
        public string CurrTyp { get; set; }
        [Currency(RecordType = RecordType.MiscSeg)]
        public decimal Charge { get; set; }
    }
}
