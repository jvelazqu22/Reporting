using Domain.Exceptions;
using Domain.Orm.Classes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using Fasterflect;

using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;

namespace iBank.Services.Implementation.Utilities.CurrencyConversion
{
    public class CurrencyConverter
    {
        private readonly IMasterDataStore _dataStore;

        private readonly DateTime _today;

        private readonly string _defaultCurrency;

        public CurrencyConverter()
        {
            _dataStore = new MasterDataStore();
            _today = DateTime.Today;
            _defaultCurrency = "USD";
        }

        public CurrencyConverter(IMasterDataStore dataStore, DateTime today, string defaultCurrency)
        {
            _dataStore = dataStore;
            _today = today;
            _defaultCurrency = defaultCurrency;
        }

        /// <summary>
        /// given a list of objects, this process will convert all decimal properties in each object that are decorated with the Currency attribute to a new currency.
        /// There must also be a string property decorated with the SourceCurrency attribute, and at least one attribute set as an Exchange Date. 
        /// There may be up to three Exchange Dates, set using attributes ExchangeDate1, ExchangeDate2, and ExchangeDate3
        /// NOTE: Currency objects MUST be decimal, and ExchangeDate objects MUST be DateTimes. The SourceCurrency MUST be a valid currency abbreviation (USD,GBP, etc). 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="destCurrency"></param>
        public void ConvertCurrency<T>(List<T> list, string destCurrency)
        {
            var retriever = new CurrencyPropertyRetriever();
            if (string.IsNullOrEmpty(destCurrency)) destCurrency = _defaultCurrency;
            
            var classProperties = new List<PropertyInfo>(list[0].GetType().Properties());
            var propertiesToConvert = retriever.GetCurrencyValueProperties(classProperties);

            if (!propertiesToConvert.Any()) return;

            //don't worry about null here - we got propertiesToConvert list from the class properties
            var recType = retriever.GetCurrencyRecordType(classProperties.FirstOrDefault(x => propertiesToConvert.Contains(x.Name)));
            var propertyHolder = new ExchangePropertyHolder(classProperties, recType);

            //Get the currency conversion table for the requested currency
            var destinationRateQuery = new GetCurrencyConversionByCurrencyCodeQuery(_dataStore.MastersQueryDb, destCurrency);
            var destinationExchangeRates = ConversionRateRetriever.GetExchangeRates(destCurrency, destinationRateQuery);

            var settings = new CurrencyConversionSettings(propertyHolder, retriever, _defaultCurrency);
            
            foreach (var item in list)
            {
                try
                {
                    settings.SetConversionDates(item, _today);
                    settings.SetCurrencyTypes(item, recType);
                    settings.AddSourceCurrencies(settings.SourceCurrency, new GetCurrencyConversionByCurrencyCodeQuery(_dataStore.MastersQueryDb, settings.SourceCurrency));
                }
                catch (InvalidCastException castException)
                {
                    throw new CurrencyConversionException(string.Format("Currency attributes assiged to properties of wrong type!{0}", castException.Message));
                }

                try
                {
#if DEBUG
                    foreach(var propertyInfo in classProperties)
                    {
                        AttemptPropertyConversion(item, propertiesToConvert, propertyInfo, settings, destCurrency, destinationExchangeRates);
                    }
#else
                    Parallel.ForEach(classProperties, new ParallelOptions { MaxDegreeOfParallelism = GetThreads() }, propertyInfo =>
                        {
                            AttemptPropertyConversion(item, propertiesToConvert, propertyInfo, settings, destCurrency, destinationExchangeRates);
                        });
#endif
                }
                catch (Exception e)
                {
                    //don't want to throw the AggregateException - just the inner exception
                    ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                }
            }
        }

        private void AttemptPropertyConversion<T>(T item, HashSet<string> propertiesToConvert, PropertyInfo propertyInfo, CurrencyConversionSettings settings,
                                                      string destinationCurrency, Dictionary<DateTime, CurrencyConversionInformation> destinationExchangeRates)
        {
            if (!propertiesToConvert.Contains(propertyInfo.Name)) return;

            //Get the value to be converted 
            if (null == propertyInfo.Get(item)) return;
            var sourceVal = (decimal)propertyInfo.Get(item);
            if (sourceVal == 0) return;
            
            var convertedValue = GetConvertedValue(sourceVal, destinationCurrency, destinationExchangeRates, settings);
            propertyInfo.Set(item, convertedValue);
        }

        /// <summary>
        /// Converts a value from one currency to another. If no exchange rate is found for the destination currency, returned value will be zero. 
        /// </summary>
        /// <param name="val">The source value</param>
        /// <param name="sourceCurrency">Currency to convert from</param>
        /// <param name="destCurrency">currency to convert to</param>
        /// <param name="destinationExchangeRates"></param>
        /// <param name="settings"></param>
        /// <returns>The converted value</returns>
        private decimal GetConvertedValue(decimal val, string destCurrency, Dictionary<DateTime, CurrencyConversionInformation> destinationExchangeRates, CurrencyConversionSettings settings)
        {
            Dictionary<DateTime, CurrencyConversionInformation> sourceExchangeRates = new Dictionary<DateTime, CurrencyConversionInformation>();
            if (settings.SourceCurrency != _defaultCurrency && !settings.SourceExchangeRates.TryGetValue(settings.SourceCurrency, out sourceExchangeRates))
            {
                var msg = $"Currency conversion data does not exist for currency code [{settings.SourceCurrency}].";
                throw new CurrencyConversionException(msg);
            }
            
            var rates = ConversionRateCalculator.GetConversionRates(settings.SourceCurrency, destCurrency, settings.ConversionDates, sourceExchangeRates, destinationExchangeRates,
                _defaultCurrency);

            return ConversionRateCalculator.ConvertValue(val, (decimal)rates.SourceRate, (decimal)rates.DestinationRate);
        }

        private int GetThreads()
        {
#if DEBUG
            return 1;
#else
            return 10;
#endif
        }
    }
}
