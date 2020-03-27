using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Orm.Classes;
using Domain.Helper;

using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.Utilities.CurrencyConversion
{
    public class CurrencyConversionSettings
    {
        private readonly ExchangePropertyHolder _propertyHolder;

        private readonly CurrencyPropertyRetriever _propertyRetriever;

        private readonly string _defaultCurrency;
        public string SourceCurrency { get; set; }
        public List<DateTime> ConversionDates { get; set; }

        public Dictionary<string, Dictionary<DateTime, CurrencyConversionInformation>> SourceExchangeRates { get; set; }

        public CurrencyConversionSettings(ExchangePropertyHolder holder, CurrencyPropertyRetriever retriever, string defaultCurrency)
        {
            _propertyHolder = holder;
            _propertyRetriever = retriever;
            _defaultCurrency = defaultCurrency;
            ConversionDates = new List<DateTime>();
            SourceExchangeRates = new Dictionary<string, Dictionary<DateTime, CurrencyConversionInformation>>();
        }

        private static readonly DateTime _beginOfTime = new DateTime(1900, 1, 1);
        public void SetConversionDates<T>(T item, DateTime today)
        {
            ConversionDates = new List<DateTime>();
            var date1 = _propertyRetriever.GetDateToUse(_propertyHolder.Date1Property, item);
            if (date1 != null && date1 != _beginOfTime) ConversionDates.Add(date1.Value);

            var date2 = _propertyRetriever.GetDateToUse(_propertyHolder.Date2Property, item);
            if (date2 != null && date2 != _beginOfTime) ConversionDates.Add(date2.Value);

            var date3 = _propertyRetriever.GetDateToUse(_propertyHolder.Date3Property, item);
            if (date3 != null && date3 != _beginOfTime) ConversionDates.Add(date3.Value);

            //if all dates were null, check todays date
            if (!ConversionDates.Any()) ConversionDates.Add(today);
        }
        
        public void SetCurrencyTypes<T>(T item, RecordType recType)
        {
            var currency = _propertyRetriever.GetCurrency(_propertyHolder.SourceCurrencyProperty, item);
            SourceCurrency = string.IsNullOrEmpty(currency) ? _defaultCurrency : currency;
        }
        
        public void AddSourceCurrencies(string sourceCurrency, IQuery<IList<CurrencyConversionInformation>> currencyConversionQuery)
        {
            if (!string.IsNullOrEmpty(sourceCurrency) && sourceCurrency != _defaultCurrency && !SourceExchangeRates.ContainsKey(sourceCurrency))
            {
                var rates = ConversionRateRetriever.GetExchangeRates(sourceCurrency, currencyConversionQuery);
                SourceExchangeRates.Add(sourceCurrency, rates);
            }
        }
    }
}
