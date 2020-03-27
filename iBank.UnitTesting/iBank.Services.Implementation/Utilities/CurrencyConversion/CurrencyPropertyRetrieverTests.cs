using System;

using Domain.Helper;
using iBank.Services.Implementation.Utilities.CurrencyConversion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities.CurrencyConversion
{
    [TestClass]
    public class CurrencyPropertyRetrieverTests
    {
        [TestMethod]
        public void IsConversionRequired_OnlyOneMoneyTypeAlreadyOfSameType_ReturnFalse()
        {
            var retriever = new CurrencyPropertyRetriever();
            var data = new DecoratedData
                           {
                               AirCurrType = "USD",
                               CarCurrType = "USD",
                               HotCurrType = "USD",
                               FeeCurrType = "USD",
                               MiscSegCurrType = "USD"
                           };
            var dataList = new List<DecoratedData> { data };
            var currencyTypeProperties = retriever.GetCurrencyTypeProperties(dataList[0]);
            var moneyType = "USD";

            var output = retriever.IsConversionRequired(dataList, currencyTypeProperties, moneyType);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void IsConversionRequired_OnlyOneMoneyTypeAlreadyOfDifferentType_ReturnTrue()
        {
            var retriever = new CurrencyPropertyRetriever();
            var data = new DecoratedData
            {
                AirCurrType = "GBP",
                CarCurrType = "GBP"
            };
            var dataList = new List<DecoratedData> { data };
            var currencyTypeProperties = retriever.GetCurrencyTypeProperties(dataList[0]);
            var moneyType = "USD";

            var output = retriever.IsConversionRequired(dataList, currencyTypeProperties, moneyType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsConversionRequired_MultipleMoneyTypesOneOfDifferentType_ReturnTrue()
        {
            var retriever = new CurrencyPropertyRetriever();
            var data = new DecoratedData
            {
                AirCurrType = "GBP",
                CarCurrType = "USD"
            };
            var dataList = new List<DecoratedData> { data };
            var currencyTypeProperties = retriever.GetCurrencyTypeProperties(dataList[0]);
            var moneyType = "USD";

            var output = retriever.IsConversionRequired(dataList, currencyTypeProperties, moneyType);

            Assert.AreEqual(true, output);
        }
        
        [TestMethod]
        public void IsConversionRequired_MultipleMoneyTypesBothDifferentType_ReturnTrue()
        {
            var retriever = new CurrencyPropertyRetriever();
            var data = new DecoratedData
            {
                AirCurrType = "GBP",
                CarCurrType = "FOO"
            };
            var dataList = new List<DecoratedData> { data };
            var currencyTypeProperties = retriever.GetCurrencyTypeProperties(dataList[0]);
            var moneyType = "USD";

            var output = retriever.IsConversionRequired(dataList, currencyTypeProperties, moneyType);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void GetCurrency_PropertyIsNull_ReturnEmptyString()
        {
            var retriever = new CurrencyPropertyRetriever();
            var data = new DecoratedData();

            var output = retriever.GetCurrency(null, data);

            Assert.AreEqual("", output);
        }

        [TestMethod]
        public void GetCurrency_PropertyIsValid_ReturnPropertyValue()
        {
            var retriever = new CurrencyPropertyRetriever();
            var data = new DecoratedData { AirCurrType = "USD" };
            var info = typeof(DecoratedData).GetProperties().FirstOrDefault(x => x.Name == "AirCurrType");
            
            var output = retriever.GetCurrency(info, data);

            Assert.AreEqual("USD", output);
        }

        [TestMethod]
        public void GetCurrencyTypeProperties()
        {
            var list = GenerateLargeList();
            var sut = new CurrencyPropertyRetriever();

            var output = sut.GetCurrencyTypeProperties(list[0]);

            Assert.AreEqual(5, output.Count);
        }

        [TestMethod]
        public void GetCurrencyValueProperties()
        {
            var list = GenerateLargeList();
            var sut = new CurrencyPropertyRetriever();
            var type = list[0].GetType();
            var props = new List<PropertyInfo>(type.GetProperties());

            var output = sut.GetCurrencyValueProperties(props);

            Assert.AreEqual(5, output.Count);
        }

        [TestMethod]
        public void GetDecoratedProperty()
        {
            var list = GenerateLargeList();
            var sut = new CurrencyPropertyRetriever();
            var type = list[0].GetType();
            var props = new List<PropertyInfo>(type.GetProperties());

            var date1 = sut.GetDecoratedProperty(props, typeof(ExchangeDate1));
            var date2 = sut.GetDecoratedProperty(props, typeof(ExchangeDate2));
            var date3 = sut.GetDecoratedProperty(props, typeof(ExchangeDate3));

            var airCurr = sut.GetDecoratedProperty(props, typeof(AirCurrency));
            var hotCurr = sut.GetDecoratedProperty(props, typeof(HotelCurrency));
            var carCurr = sut.GetDecoratedProperty(props, typeof(CarCurrency));
            var feeCurr = sut.GetDecoratedProperty(props, typeof(FeeCurrency));
            var miscSegCurr = sut.GetDecoratedProperty(props, typeof(MiscSegCurrency));

            Assert.AreEqual("Date", date1.Name);
            Assert.AreEqual("Date2", date2.Name);
            Assert.AreEqual("Date3", date3.Name);
            Assert.AreEqual("AirCurrType", airCurr.Name);
            Assert.AreEqual("HotCurrType", hotCurr.Name);
            Assert.AreEqual("CarCurrType", carCurr.Name);
            Assert.AreEqual("FeeCurrType", feeCurr.Name);
            Assert.AreEqual("MiscSegCurrType", miscSegCurr.Name);
        }

        [TestMethod]
        public void GetCurrency()
        {
            var list = GenerateLargeList();

            var retriever = new CurrencyPropertyRetriever();
            var prop = typeof(DecoratedData).GetProperty(nameof(DecoratedData.AirCurrType));

            foreach (var item in list)
            {
                retriever.GetCurrency(prop, item);
            }

            Assert.AreEqual("GBP", retriever.GetCurrency(prop, list[0]));
        }

        [TestMethod]
        public void GetDateToUse()
        {
            var list = GenerateLargeList();

            var retriever = new CurrencyPropertyRetriever();
            var prop = typeof(DecoratedData).GetProperty(nameof(DecoratedData.Date));

            foreach (var item in list)
            {
                retriever.GetDateToUse(prop, item);
            }

            Assert.AreEqual(new DateTime(2015, 1, 1), retriever.GetDateToUse(prop, list[0]));
        }

        [TestMethod]
        public void GetCurrencyRecordType()
        {
            var sut = new CurrencyPropertyRetriever();
            var prop = typeof(DecoratedData).GetProperty(nameof(DecoratedData.HotelCurrency));

            var output = sut.GetCurrencyRecordType(prop);

            Assert.AreEqual(RecordType.Hotel, output);

        }

        private List<DecoratedData> GenerateLargeList()
        {
            var list = new List<DecoratedData>();

            for (var i = 0; i < 500000; i++)
            {
                list.Add(new DecoratedData { AirCurrType = "GBP", CarCurrType = "GBP" });
            }
            
            return list;
        }

        internal class DecoratedData
        {
            [Currency(RecordType = RecordType.Air)]
            public decimal AirCurrency { get; set; }

            [Currency(RecordType = RecordType.Car)]
            public decimal CarCurrency { get; set; }

            [Currency(RecordType = RecordType.Hotel)]
            public decimal HotelCurrency { get; set; }

            [Currency(RecordType = RecordType.SvcFee)]
            public decimal FeeCurrency { get; set; }

            [Currency(RecordType = RecordType.MiscSeg)]
            public decimal MiscSegCurrency { get; set; }

            [AirCurrency]
            public string AirCurrType { get; set; }

            [CarCurrency]
            public string CarCurrType { get; set; }

            [HotelCurrency]
            public string HotCurrType { get; set; }

            [FeeCurrency]
            public string FeeCurrType { get; set; }

            [MiscSegCurrency]
            public string MiscSegCurrType { get; set; }

            [ExchangeDate1]
            public DateTime? Date { get; set; }

            [ExchangeDate2]
            public DateTime? Date2 { get; set; }

            [ExchangeDate3]
            public DateTime? Date3 { get; set; }

            public DecoratedData()
            {
                AirCurrency = 1;
                CarCurrency = 2;
                HotelCurrency = 3;
                FeeCurrency = 4;
                MiscSegCurrency = 5;

                AirCurrType = "GBP";
                CarCurrType = "GBP";
                HotCurrType = "GBP";
                FeeCurrType = "GBP";
                MiscSegCurrType = "GBP";

                Date = new DateTime(2015, 1, 1);
                Date2 = new DateTime(2016, 1, 1);
                Date3 = new DateTime(2017, 1, 1);

            }
        }
    }
}
