using System;

namespace Domain.Helper
{
    /// <summary>
    /// Assign this property to any field that may need to be converted to a different currency. 
    /// Should only be applied to decimal fields. 
    /// </summary>
    public class Currency : Attribute
    {
        public override string ToString()
        {
            return "Currency";
        }

        public RecordType RecordType { get; set; }
    }

    public class AirCurrency : Attribute
    {
        public override string ToString()
        {
            return "AirCurrency";
        }
    }

    public class CarCurrency : Attribute
    {
        public override string ToString()
        {
            return "CarCurrency";
        }
    }

    public class HotelCurrency : Attribute
    {
        public override string ToString()
        {
            return "HotelCurrency";
        }
    }

    public class FeeCurrency : Attribute
    {
        public override string ToString()
        {
            return "FeeCurrency";
        }
    }

    public class MiscSegCurrency : Attribute
    {
        public override string ToString()
        {
            return "MiscSegCurrency";
        }
    }


    /// <summary>
    /// Assign this property to the first field that will be used to retrieve the currency. 
    /// </summary>
    public class ExchangeDate1 : Attribute
    {
        public override string ToString()
        {
            return "CurrencyDate1";
        }

    }

    /// <summary>
    /// Assign this property to the first field that will be used to retrieve the currency. 
    /// </summary>
    public class ExchangeDate2 : Attribute
    {
        public override string ToString()
        {
            return "CurrencyDate2";
        }

    }

    /// <summary>
    /// Assign this property to the first field that will be used to retrieve the currency. 
    /// </summary>
    public class ExchangeDate3 : Attribute
    {
        public override string ToString()
        {
            return "CurrencyDate3";
        }

    }

    public enum RecordType
    {
        Air,
        Car,
        Hotel,
        SvcFee,
        SvcAmt,
        MiscSeg
    }
}
