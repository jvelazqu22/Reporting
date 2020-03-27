using System;
using System.Collections.Generic;

using Domain.Exceptions;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Utilities.CurrencyConversion;
using iBank.UnitTesting.TestingHelpers.MockHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities.CurrencyConversion
{
    [TestClass]
    public class CurrencyConverterTests
    {
        private IMasterDataStore _dataStore;

        private readonly DateTime _today = new DateTime(2017, 1, 1);

        [TestInitialize]
        public void Init()
        {
            var currencyDateOne = new DateTime(2016, 1, 1);
            var currencyDateTwo = new DateTime(2016, 2, 2);
            var currencyDateThree = new DateTime(2016, 3, 3);
            var mockDb = new Mock<IMastersQueryable>();
            var curConversionData = new List<curconversion>
                                        {
                                            //destination currencies
                                            new curconversion { curcode = "GBP", curdate = currencyDateOne, usdfactor = 4 },
                                            new curconversion { curcode = "GBP", curdate = currencyDateTwo, usdfactor = 5 },
                                            new curconversion { curcode = "GBP", curdate = currencyDateThree, usdfactor = 6 },
                                            new curconversion { curcode = "GBP", curdate = _today, usdfactor = 7 },

                                            //source currencies
                                            new curconversion { curcode = "USD", curdate = currencyDateOne, usdfactor = 2 },
                                            new curconversion { curcode = "USD", curdate = currencyDateTwo, usdfactor = 4 },
                                            new curconversion { curcode = "USD", curdate = currencyDateThree, usdfactor = 6 },
                                            new curconversion { curcode = "USD", curdate = _today, usdfactor = 8 },

                                            new curconversion { curcode = "AUD", curdate = currencyDateOne, usdfactor = 3 },
                                            new curconversion { curcode = "AUD", curdate = currencyDateTwo, usdfactor = 5 },
                                            new curconversion { curcode = "AUD", curdate = currencyDateThree, usdfactor = 7 },
                                            new curconversion { curcode = "AUD", curdate = _today, usdfactor = 9 },
                                        };
            mockDb.Setup(x => x.CurConversion).Returns(MockHelper.GetListAsQueryable(curConversionData).Object);
            var mockStore = new Mock<IMasterDataStore>();
            mockStore.Setup(x => x.MastersQueryDb).Returns(mockDb.Object);
            _dataStore = mockStore.Object;
        }



        [TestMethod]
        public void ConvertCurrency_ValidAirData()
        {
            var destinationCurrency = "GBP";
            var validAirData = new List<ValidAirData>
                                   {
                                        new ValidAirData { Charge = 4M, CurrTyp = "USD", DateOne = new DateTime(2016, 1, 1) }
                                   };
            var sut = new CurrencyConverter(_dataStore, _today, "XXX");
            sut.ConvertCurrency(validAirData, destinationCurrency);

            Assert.AreEqual(8, validAirData[0].Charge);
        }

        [TestMethod]
        public void ConvertCurrency_ValidHotelData()
        {
            var destinationCurrency = "GBP";
            var validData = new List<ValidHotelData>
                                   {
                                        new ValidHotelData { Charge = 4M, CurrTyp = "USD", DateOne = new DateTime(2016, 1, 1) }
                                   };
            var sut = new CurrencyConverter(_dataStore, _today, "XXX");
            sut.ConvertCurrency(validData, destinationCurrency);

            Assert.AreEqual(8, validData[0].Charge);
        }

        [TestMethod]
        public void ConvertCurrency_ValidCarData()
        {
            var destinationCurrency = "GBP";
            var validData = new List<ValidCarData>
                                   {
                                        new ValidCarData { Charge = 4M, CurrTyp = "USD", DateOne = new DateTime(2016, 1, 1) }
                                   };
            var sut = new CurrencyConverter(_dataStore, _today, "XXX");
            sut.ConvertCurrency(validData, destinationCurrency);

            Assert.AreEqual(8, validData[0].Charge);
        }

        [TestMethod]
        public void ConvertCurrency_ValidSvcFeeData()
        {
            var destinationCurrency = "GBP";
            var validData = new List<ValidSvcFeeData>
                                   {
                                        new ValidSvcFeeData { Charge = 4M, CurrTyp = "USD", DateOne = new DateTime(2016, 1, 1) }
                                   };
            var sut = new CurrencyConverter(_dataStore, _today, "XXX");
            sut.ConvertCurrency(validData, destinationCurrency);

            Assert.AreEqual(8, validData[0].Charge);
        }

        [TestMethod]
        public void ConvertCurrency_ValidSvcAmtData()
        {
            var destinationCurrency = "GBP";
            var validData = new List<ValidSvcAmtData>
                                   {
                                        new ValidSvcAmtData { Charge = 4M, CurrTyp = "USD", DateOne = new DateTime(2016, 1, 1) }
                                   };
            var sut = new CurrencyConverter(_dataStore, _today, "XXX");
            sut.ConvertCurrency(validData, destinationCurrency);

            Assert.AreEqual(8, validData[0].Charge);
        }

        [TestMethod]
        public void ConvertCurrency_ValidMiscSegsData()
        {
            var destinationCurrency = "GBP";
            var validData = new List<ValidMiscSegsData>
                                   {
                                        new ValidMiscSegsData { Charge = 4M, CurrTyp = "USD", DateOne = new DateTime(2016, 1, 1) }
                                   };
            var sut = new CurrencyConverter(_dataStore, _today, "XXX");
            sut.ConvertCurrency(validData, destinationCurrency);

            Assert.AreEqual(8, validData[0].Charge);
        }

        [TestMethod]
        public void ConvertCurrency_ValidAirDataDefaultCurrencyIsSameAsSourceCurrency()
        {
            var destinationCurrency = "GBP";
            var validAirData = new List<ValidAirData>
                                   {
                                        new ValidAirData { Charge = 4M, CurrTyp = "USD", DateOne = new DateTime(2016, 1, 1) }
                                   };
            var sut = new CurrencyConverter(_dataStore, _today, "USD");
            sut.ConvertCurrency(validAirData, destinationCurrency);

            Assert.AreEqual(16, validAirData[0].Charge);
        }

        [TestMethod]
        public void ConvertCurrency_ValidAirDataDestinationCurrencySameAsSource_DontConvert()
        {
            var destinationCurrency = "USD";
            var validAirData = new List<ValidAirData>
                                   {
                                        new ValidAirData { Charge = 4M, CurrTyp = "USD", DateOne = new DateTime(2016, 1, 1) }
                                   };
            var sut = new CurrencyConverter(_dataStore, _today, "USD");
            sut.ConvertCurrency(validAirData, destinationCurrency);

            Assert.AreEqual(4, validAirData[0].Charge);
        }

        [TestMethod]
        public void ConvertCurrency_MultipleValidSourceCurrencies_Convert()
        {
            var destinationCurrency = "GBP";
            var validAirData = new List<ValidAirData>
                                   {
                                        new ValidAirData { Charge = 4M, CurrTyp = "USD", DateOne = new DateTime(2016, 1, 1) },
                                        new ValidAirData { Charge = 4M, CurrTyp = "AUD", DateOne = new DateTime(2016, 1, 1) }
                                   };
            var sut = new CurrencyConverter(_dataStore, _today, "XXX");
            sut.ConvertCurrency(validAirData, destinationCurrency);

            Assert.AreEqual(8, validAirData[0].Charge);
            Assert.AreEqual(5.33M, validAirData[1].Charge);
        }

        [ExpectedException(typeof(CurrencyConversionException))]
        [TestMethod]
        public void ConvertCurrency_NoCurrencyTypeDecoration_ThrowCurrencyConversionException()
        {
            var destinationCurrency = "GBP";
            var validAirData = new List<NoCurrencyTypeDecoration>
                                   {
                                        new NoCurrencyTypeDecoration { Charge = 4M, CurrTyp = "USD", DateOne = new DateTime(2017, 1, 1) }
                                   };
            var sut = new CurrencyConverter(_dataStore, _today, "XXX");
            sut.ConvertCurrency(validAirData, destinationCurrency);
        }

        [ExpectedException(typeof(CurrencyConversionException))]
        [TestMethod]
        public void ConvertCurrency_NoDateDecoration_ThrowCurrencyConversionException()
        {
            var destinationCurrency = "GBP";
            var validAirData = new List<NoDateDecoration>
                                   {
                                        new NoDateDecoration { Charge = 4M, CurrTyp = "USD", DateOne = new DateTime(2017, 1, 1) }
                                   };
            var sut = new CurrencyConverter(_dataStore, _today, "XXX");
            sut.ConvertCurrency(validAirData, destinationCurrency);
        }

        [ExpectedException(typeof(CurrencyConversionException))]
        [TestMethod]
        public void ConvertCurrency_MismatchedCurrencyTypeAndRecordType_ThrowCurrencyConversionException()
        {
            var destinationCurrency = "GBP";
            var validAirData = new List<MismatchedCurrencyTypeAndRecordType>
                                   {
                                        new MismatchedCurrencyTypeAndRecordType { Charge = 4M, CurrTyp = "USD", DateOne = new DateTime(2017, 1, 1) }
                                   };
            var sut = new CurrencyConverter(_dataStore, _today, "XXX");
            sut.ConvertCurrency(validAirData, destinationCurrency);
        }

        [ExpectedException(typeof(CurrencyConversionException))]
        [TestMethod]
        public void ConvertCurrency_DateDecoratesAString_ThrowCurrencyConversionException()
        {
            var destinationCurrency = "GBP";
            var validAirData = new List<InvalidCastData>
                                   {
                                        new InvalidCastData { Charge = 4M, CurrTyp = "USD", DateOne = "foo" }
                                   };
            var sut = new CurrencyConverter(_dataStore, _today, "XXX");
            sut.ConvertCurrency(validAirData, destinationCurrency);
        }
    }
}
