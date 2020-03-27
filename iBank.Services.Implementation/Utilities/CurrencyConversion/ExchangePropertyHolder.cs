using System;
using System.Collections.Generic;
using System.Reflection;

using Domain.Exceptions;
using Domain.Helper;

namespace iBank.Services.Implementation.Utilities.CurrencyConversion
{
    public class ExchangePropertyHolder
    {
        public PropertyInfo Date1Property { get; set; }
        public PropertyInfo Date2Property { get; set; }
        public PropertyInfo Date3Property { get; set; }

        public PropertyInfo SourceCurrencyProperty { get; set; }

        public ExchangePropertyHolder(IList<PropertyInfo> properties, RecordType recType)
        {
            var retriever = new CurrencyPropertyRetriever();
            Date1Property = retriever.GetDecoratedProperty(properties, typeof(ExchangeDate1));
            Date2Property = retriever.GetDecoratedProperty(properties, typeof(ExchangeDate2));
            Date3Property = retriever.GetDecoratedProperty(properties, typeof(ExchangeDate3));

            if (Date1Property == null && Date2Property == null && Date3Property == null)
            {
                throw new CurrencyConversionException("Currency conversion attempted on a class with no CurrencyDate attributes");
            }

            Type currencyType = null;
            switch (recType)
            {
                case RecordType.Air:
                    currencyType = typeof(AirCurrency);
                    break;
                case RecordType.Car:
                    currencyType = typeof(CarCurrency);
                    break;
                case RecordType.Hotel:
                    currencyType = typeof(HotelCurrency);
                    break;
                case RecordType.SvcAmt:
                case RecordType.SvcFee:
                    currencyType = typeof(FeeCurrency);
                    break;
                case RecordType.MiscSeg:
                    currencyType = typeof(MiscSegCurrency);
                    break;
            }

            if(currencyType == null) throw new CurrencyConversionException($"An unhandled record type was used to retrieve the source currency. Record type: [{recType}].");

            SourceCurrencyProperty = retriever.GetDecoratedProperty(properties, currencyType);

            if(SourceCurrencyProperty == null) throw new CurrencyConversionException($"Currency conversion attempted on data with no source currency defined. Record type: [{recType}]");
        }
    }
}
