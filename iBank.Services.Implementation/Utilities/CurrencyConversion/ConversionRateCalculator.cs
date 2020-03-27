using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Exceptions;
using Domain.Helper;
using Domain.Orm.Classes;

using MoreLinq;

namespace iBank.Services.Implementation.Utilities.CurrencyConversion
{
    public static class ConversionRateCalculator
    {
        public static decimal ConvertValue(decimal originalValue, decimal sourceRate, decimal destinationRate)
        {
            return MathHelper.Round((originalValue / sourceRate) * destinationRate, 2);
        }

        public static ConversionRates GetConversionRates(string sourceCurrency, string destinationCurrency, List<DateTime> conversionDates, Dictionary<DateTime, CurrencyConversionInformation> sourceExchangeRates,
            Dictionary<DateTime, CurrencyConversionInformation> destinationExchangeRates, string defaultCurrency)
        {
            var rates = new ConversionRates();

            if (!IsConversionRequired(sourceCurrency, defaultCurrency))
            {
                rates.SourceRateFound = true;
                rates.SourceRate = 1;
            }

            if (!IsConversionRequired(destinationCurrency, defaultCurrency))
            {
                rates.DestinationRateFound = true;
                rates.DestinationRate = 1;
            }

            if (rates.SourceRateFound && rates.DestinationRateFound) return rates;

            if (!conversionDates.Any()) throw new CurrencyConversionException("No conversion dates were found!");

            var rateRetriever = new ConversionRateRetriever();
            DateTime lastDate = DateTime.MinValue;
            foreach (var date in conversionDates)
            {
                //iterate over the possible dates till you find one to use
                if (!rates.SourceRateFound)
                {
                    rateRetriever.SetRate(date, sourceExchangeRates, rates, true);
                }

                if (!rates.DestinationRateFound)
                {
                    rateRetriever.SetRate(date, destinationExchangeRates, rates, false);
                }

                if (rates.SourceRateFound && rates.DestinationRateFound) break;
                lastDate = date;
            }
            //we should always have a rate, in this case use the last rate
            if (!rates.SourceRateFound && sourceExchangeRates.Any())
            {
                rates.SourceRate = sourceExchangeRates.Values.MaxBy(x => x.CurrencyDate).USDFactor;
                rates.SourceRateFound = true;
                rates.DestinationRate = destinationExchangeRates.Where(x => x.Value.CurrencyCode == destinationCurrency).Any()
                    ? destinationExchangeRates.Values.MaxBy(x => x.CurrencyDate).USDFactor
                    : 1;
                rates.DestinationRateFound = true;
            }
            //unless sourceExchangeRates is empty
            if (!rates.SourceRateFound)
            {
                var errorMessage = string.Format("Currency conversion data does not exist for dates [{0}], currency code [{1}]", string.Join(",", conversionDates),
                            sourceCurrency);
                throw new CurrencyConversionException(errorMessage);
            }

            //if rates.DestinationRate is 0 means the rate is in between
            if (rates.DestinationRate == 0  && lastDate != DateTime.MinValue)
            {
                //Force to get last rate again.
                rateRetriever.SetRate(lastDate, destinationExchangeRates, rates, false, true);
            }
            return rates;
        }

        private static bool IsConversionRequired(string currency, string defaultCurrency)
        {
            return !currency.Equals(defaultCurrency, StringComparison.OrdinalIgnoreCase);
        }
    }
}
