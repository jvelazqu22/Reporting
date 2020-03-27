using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Domain.Exceptions;
using Domain.Helper;
using Fasterflect;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.Utilities.CurrencyConversion
{
    public class CurrencyPropertyRetriever
    {
        public IList<PropertyInfo> GetCurrencyTypeProperties<T>(T objectToConvert)
        {
            return objectToConvert.GetType().Properties().Where(x => Attribute.IsDefined(x, typeof(AirCurrency))
                                                                    || Attribute.IsDefined(x, typeof(HotelCurrency))
                                                                    || Attribute.IsDefined(x, typeof(CarCurrency))
                                                                    || Attribute.IsDefined(x, typeof(FeeCurrency))
                                                                    || Attribute.IsDefined(x, typeof(MiscSegCurrency))).ToList();
        }

        public HashSet<string> GetCurrencyValueProperties(IList<PropertyInfo> propertiesOfClass)
        {
            return propertiesOfClass.Where(x => Attribute.IsDefined(x, typeof(Currency))).Select(x => x.Name).ToHashSet();
        }

        public PropertyInfo GetDecoratedProperty(IList<PropertyInfo> propertiesToConvert, Type attribute)
        {
            return propertiesToConvert.FirstOrDefault(x => Attribute.IsDefined(x, attribute));
        }

        public DateTime? GetDateToUse<T>(PropertyInfo dateProperty, T item)
        {
            return (DateTime?)dateProperty?.Get(item);
        }

        public string GetCurrency<T>(PropertyInfo currencyProperty, T item)
        {
            if (currencyProperty == null) return "";

            var currency = (string)currencyProperty.Get(item);

            if (currency == null) throw new CurrencyConversionException("Currency Property is not supplied");

            return currency;
        }

        public bool IsConversionRequired<T>(IList<T> listToConvert, IList<PropertyInfo> currencyTypeProperties, string moneyType)
        {
            moneyType = moneyType.ToUpper().Trim();

            if ( string.IsNullOrWhiteSpace(moneyType) ) return false;
            //is there any currency types in the decorated properties that don't have the same type as the moneyType parameter
            foreach (var record in listToConvert)
            {
                foreach(var prop in currencyTypeProperties)
                {
                    var currency = GetCurrency(prop, record);
                    if ( !currency.ToUpper().Trim().Equals(moneyType) ) return true;
                }
            }
            return false;
        }

        public RecordType GetCurrencyRecordType(PropertyInfo propertyInfo)
        {
            var currencyAttribute = (Currency)propertyInfo.GetCustomAttributes(typeof(Currency), true).FirstOrDefault();

            return currencyAttribute?.RecordType ?? RecordType.Air;
        }
    }
}
