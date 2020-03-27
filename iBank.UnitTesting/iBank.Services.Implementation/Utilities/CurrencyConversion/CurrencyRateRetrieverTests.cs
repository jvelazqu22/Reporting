using System;
using System.Collections.Generic;

using Domain.Helper;
using Domain.Orm.Classes;

using iBank.Services.Implementation.Utilities.CurrencyConversion;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities.CurrencyConversion
{
    [TestClass]
    public class CurrencyRateRetrieverTests
    {
        private ConversionRates _rates;
        private Dictionary<DateTime, CurrencyConversionInformation> _exchangeRates;
        private readonly DateTime _preferredDate = new DateTime(2017, 1, 1);
        private readonly DateTime _minDate = new DateTime(2015, 1, 1);
        private readonly DateTime _maxDate = new DateTime(2019, 1, 1);

        [TestInitialize]
        public void Init()
        {
            _rates = new ConversionRates { DestinationRate = -1, DestinationRateFound = false, SourceRate = -1, SourceRateFound = false };
            _exchangeRates = new Dictionary<DateTime, CurrencyConversionInformation>();
            _exchangeRates.Add(_preferredDate, new CurrencyConversionInformation { CurrencyCode = "USD", CurrencyDate = _preferredDate, USDFactor = 2 });
            _exchangeRates.Add(_minDate, new CurrencyConversionInformation { CurrencyCode = "USD", CurrencyDate = _minDate, USDFactor = 1 });
            _exchangeRates.Add(_maxDate, new CurrencyConversionInformation { CurrencyCode = "USD", CurrencyDate = _maxDate, USDFactor = 3 });
        }

        [TestMethod]
        public void GetRate_SourceRate_DateExists_ReturnRateForThatDate()
        {
            var sut = new ConversionRateRetriever();

            sut.SetRate(_preferredDate, _exchangeRates, _rates, true);

            Assert.AreEqual(2, _rates.SourceRate);
            Assert.AreEqual(true, _rates.SourceRateFound);
        }

        [TestMethod]
        public void GetRate_DestinationRate_DateExists_UseRateForThatDate()
        {
            var sut = new ConversionRateRetriever();

            sut.SetRate(_preferredDate, _exchangeRates, _rates, false);

            Assert.AreEqual(2, _rates.DestinationRate);
            Assert.AreEqual(true, _rates.DestinationRateFound);
        }

        [TestMethod]
        public void GetRate_SourceRate_DateDoesntExist_IsGreaterThanDataSet_UseRateForMaxDate()
        {
            var dateGreaterThan = new DateTime(2030, 1, 1);
            var sut = new ConversionRateRetriever();

            sut.SetRate(dateGreaterThan, _exchangeRates, _rates, true);

            Assert.AreEqual(3, _rates.SourceRate);
            Assert.AreEqual(true, _rates.SourceRateFound);
        }

        [TestMethod]
        public void GetRate_DestinationRate_DateDoesntExist_IsGreaterThanDataSet_UseRateForMaxDate()
        {
            var dateGreaterThan = new DateTime(2030, 1, 1);
            var sut = new ConversionRateRetriever();

            sut.SetRate(dateGreaterThan, _exchangeRates, _rates, false);

            Assert.AreEqual(3, _rates.DestinationRate);
            Assert.AreEqual(true, _rates.DestinationRateFound);
        }

        [TestMethod]
        public void GetRate_SourceRate_DateDoesntExist_IsLessThanDataSet_UseRateForMaxDate()
        {
            var dateLessThan = new DateTime(2000, 1, 1);
            var sut = new ConversionRateRetriever();

            sut.SetRate(dateLessThan, _exchangeRates, _rates, true);

            Assert.AreEqual(1, _rates.SourceRate);
            Assert.AreEqual(true, _rates.SourceRateFound);
        }

        [TestMethod]
        public void GetRate_DestinationRate_DateDoesntExist_IsLessThanDataSet_UseRateForMaxDate()
        {
            var dateLessThan = new DateTime(2000, 1, 1);
            var sut = new ConversionRateRetriever();

            sut.SetRate(dateLessThan, _exchangeRates, _rates, false);

            Assert.AreEqual(1, _rates.DestinationRate);
            Assert.AreEqual(true, _rates.DestinationRateFound);
        }

        [TestMethod]
        public void GetRate_SourceRate_DateDoesntExist_IsInDataSetRange_DontModify()
        {
            var dateLessThan = new DateTime(2016, 1, 1);
            var sut = new ConversionRateRetriever();

            sut.SetRate(dateLessThan, _exchangeRates, _rates, true);

            Assert.AreEqual(-1, _rates.SourceRate);
            Assert.AreEqual(false, _rates.SourceRateFound);
        }

        [TestMethod]
        public void GetRate_DestinationRate_DateDoesntExist_IsInDataSetRange_DontModify()
        {
            var dateLessThan = new DateTime(2016, 1, 1);
            var sut = new ConversionRateRetriever();

            sut.SetRate(dateLessThan, _exchangeRates, _rates, false);

            Assert.AreEqual(-1, _rates.DestinationRate);
            Assert.AreEqual(false, _rates.DestinationRateFound);
        }


        [TestMethod]
        public void GetRate_DestinationRate_DateDoesntExist_IsInDataSetRange_SetToLastBeforeTheDate()
        {
            var dateLessThan = new DateTime(2016, 1, 1);
            var sut = new ConversionRateRetriever();

            sut.SetRate(dateLessThan, _exchangeRates, _rates, false, true);

            Assert.AreEqual(3, _rates.DestinationRate);
            Assert.AreEqual(true, _rates.DestinationRateFound);
        }
    }
}
