using System;
using System.Collections.Generic;

using Domain.Helper;
using Domain.Orm.Classes;

using iBankDomain.RepositoryInterfaces;

using MoreLinq;

namespace iBank.Services.Implementation.Utilities.CurrencyConversion
{
    public class ConversionRateRetriever
    {
        public static Dictionary<DateTime, CurrencyConversionInformation> GetExchangeRates(string currency, IQuery<IList<CurrencyConversionInformation>> rateInfoQuery)
        {
            //Get the currency conversion table for the requested currency
            var data = rateInfoQuery.ExecuteQuery();

            var dict = new Dictionary<DateTime, CurrencyConversionInformation>();
            foreach (var rate in data)
            {
                if (!dict.ContainsKey(rate.CurrencyDate)) dict.Add(rate.CurrencyDate, rate);
            }
            return dict;
        }

        public void SetRate(DateTime dateToUse, Dictionary<DateTime, CurrencyConversionInformation> exchangeRates, ConversionRates rateToUse, bool isSourceRate, bool lastTry = false)
        {
            CurrencyConversionInformation rate;
            if (exchangeRates.TryGetValue(dateToUse, out rate))
            {
                //we prefer to use the date from the actual data
                AssignRateValues(rateToUse, rate, isSourceRate);
                return;
            }

            var maxDateRate = exchangeRates.Values.MaxBy(x => x.CurrencyDate);
            if (dateToUse > maxDateRate.CurrencyDate)
            {
                //but if that didn't exist then if that date is greater than the dataset use the maximum date
                AssignRateValues(rateToUse, maxDateRate, isSourceRate);
                return;
            }

            var minDateRate = exchangeRates.Values.MinBy(x => x.CurrencyDate);
            if (dateToUse < minDateRate.CurrencyDate)
            {
                //or if the date is smaller than the dataset use the minimum date
                AssignRateValues(rateToUse, minDateRate, isSourceRate);
                return;
            }

            if (lastTry)
            {
                AssignRateValues(rateToUse, maxDateRate, isSourceRate);
                return;
            }
        }

        public void AssignRateValues(ConversionRates rateToUse, CurrencyConversionInformation rateToPullFrom, bool isSourceRate)
        {
            if (isSourceRate)
            {
                rateToUse.SourceRate = rateToPullFrom.USDFactor;
                rateToUse.SourceRateFound = true;
            }
            else
            {
                rateToUse.DestinationRate = rateToPullFrom.USDFactor;
                rateToUse.DestinationRateFound = true;
            }
        }

    }
}
