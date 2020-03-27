using iBank.Services.Implementation.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.UnitTesting.Core_Functionality
{
    [TestClass]
    public class ZeroOutTests
    {
        [TestMethod]
        public void ProcessSameCaseOneField()
        {
            var mockData = MockData.GenerateMockData();
            
            var oneField = new List<string> { "FieldToConvert1" };
            mockData = ZeroOut<MockData>.Process(mockData, oneField);
            
            //first reckey
            var actualTotalOneField = mockData.Where(x => x.RecKey == 1234).Sum(x => x.FieldToConvert1);
            var expectedTotalOneField = 5M;
            Assert.AreEqual(expectedTotalOneField, actualTotalOneField, "reckey 1234 failed");

            //second reckey
            actualTotalOneField = mockData.Where(x => x.RecKey == 5678).Sum(x => x.FieldToConvert1);
            expectedTotalOneField = 10M;
            Assert.AreEqual(expectedTotalOneField, actualTotalOneField, "reckey 5678 failed");
            
            //non-specified field
            actualTotalOneField = mockData.Where(x => x.RecKey == 1234).Sum(x => x.FieldToConvert2);
            expectedTotalOneField = 40M;
            Assert.AreEqual(expectedTotalOneField, actualTotalOneField, "non-specified field did not sum correctly");
        }

        [TestMethod]
        public void ProcessSameCaseMultipleFields()
        {
            var mockData = MockData.GenerateMockData();

            var oneField = new List<string> { "FieldToConvert1" , "FieldToConvert2"};
            mockData = ZeroOut<MockData>.Process(mockData, oneField);

            //first reckey, first field
            var actualTotalOneField = mockData.Where(x => x.RecKey == 1234).Sum(x => x.FieldToConvert1);
            var expectedTotalOneField = 5M;
            Assert.AreEqual(expectedTotalOneField, actualTotalOneField, "reckey 1234 failed - first reckey, first field");

            //first reckey, second field
            actualTotalOneField = mockData.Where(x => x.RecKey == 1234).Sum(x => x.FieldToConvert2);
            expectedTotalOneField = 20M;
            Assert.AreEqual(expectedTotalOneField, actualTotalOneField, "reckey 1234 failed - first reckey, second field");

            //second reckey, first field
            actualTotalOneField = mockData.Where(x => x.RecKey == 5678).Sum(x => x.FieldToConvert1);
            expectedTotalOneField = 10M;
            Assert.AreEqual(expectedTotalOneField, actualTotalOneField, "reckey 5678 failed - second reckey, first field");

            //second reckey, second field
            actualTotalOneField = mockData.Where(x => x.RecKey == 1234).Sum(x => x.FieldToConvert2);
            expectedTotalOneField = 20M;
            Assert.AreEqual(expectedTotalOneField, actualTotalOneField, "reckey 5678 failed - second reckey, second field");
        }

        [TestMethod]
        public void ProcessDifferentCase()
        {
            var mockData = MockData.GenerateMockData();

            var oneField = new List<string> { "fieldtoconvert1" };
            mockData = ZeroOut<MockData>.Process(mockData, oneField);

            //first reckey
            var actualTotalOneField = mockData.Where(x => x.RecKey == 1234).Sum(x => x.FieldToConvert1);
            var expectedTotalOneField = 5M;
            Assert.AreEqual(expectedTotalOneField, actualTotalOneField, "reckey 1234 failed");
        }

        [TestMethod]
        public void ProcessNoRecKey()
        {
            //no reckey
            var mockData = MockDataNoRecKey.GenerateMockDataNoRecKey();

            var oneField = new List<string> { "FieldToConvert1" };
            try
            {
                mockData = ZeroOut<MockDataNoRecKey>.Process(mockData, oneField);
            }
            catch (Exception ex)
            {
                var expectedExMessage = "Class must have a field called reckey!";
                Assert.AreEqual(expectedExMessage, ex.Message);
            }
        }

        [TestMethod]
        public void ProcessNoFieldMatch()
        {
            var mockData = MockData.GenerateMockData();

            var oneField = new List<string> { "fakefield" };
            mockData = ZeroOut<MockData>.Process(mockData, oneField);

            //first reckey, first field
            var actualTotalOneField = mockData.Where(x => x.RecKey == 1234).Sum(x => x.FieldToConvert1);
            var expectedTotalOneField = 10M;
            Assert.AreEqual(expectedTotalOneField, actualTotalOneField, "reckey 1234 failed");

            //first reckey, second field
            actualTotalOneField = mockData.Where(x => x.RecKey == 1234).Sum(x => x.FieldToConvert2);
            expectedTotalOneField = 40M;
            Assert.AreEqual(expectedTotalOneField, actualTotalOneField, "reckey 1234 failed");

            //second reckey, first field
            actualTotalOneField = mockData.Where(x => x.RecKey == 5678).Sum(x => x.FieldToConvert1);
            expectedTotalOneField = 20M;
            Assert.AreEqual(expectedTotalOneField, actualTotalOneField, "reckey 5678 failed");

            //second reckey, second field
            actualTotalOneField = mockData.Where(x => x.RecKey == 5678).Sum(x => x.FieldToConvert2);
            expectedTotalOneField = 60M;
            Assert.AreEqual(expectedTotalOneField, actualTotalOneField, "reckey 5678 failed");
        }

        [TestMethod]
        public void ProcessNonZeroableType()
        {
            var mockData = MockData.GenerateMockData();

            var oneField = new List<string> { "NonZeroableType" };
            mockData = ZeroOut<MockData>.Process(mockData, oneField);

            //first reckey, first field
            var actualTotalOneField = mockData.Where(x => x.RecKey == 1234).Sum(x => x.FieldToConvert1);
            var expectedTotalOneField = 10M;
            Assert.AreEqual(expectedTotalOneField, actualTotalOneField, "reckey 1234 failed");

            //first reckey, second field
            actualTotalOneField = mockData.Where(x => x.RecKey == 1234).Sum(x => x.FieldToConvert2);
            expectedTotalOneField = 40M;
            Assert.AreEqual(expectedTotalOneField, actualTotalOneField, "reckey 1234 failed");

            //second reckey, first field
            actualTotalOneField = mockData.Where(x => x.RecKey == 5678).Sum(x => x.FieldToConvert1);
            expectedTotalOneField = 20M;
            Assert.AreEqual(expectedTotalOneField, actualTotalOneField, "reckey 5678 failed");

            //second reckey, second field
            actualTotalOneField = mockData.Where(x => x.RecKey == 5678).Sum(x => x.FieldToConvert2);
            expectedTotalOneField = 60M;
            Assert.AreEqual(expectedTotalOneField, actualTotalOneField, "reckey 5678 failed");
        }
        internal class MockData
        {
            public int RecKey { get; set; }
            public decimal FieldToConvert1 { get; set; }
            public decimal FieldToConvert2 { get; set; }

            public string NonZeroableType { get; set; }

            public static List<MockData> GenerateMockData()
            {
                return new List<MockData>
                           {
                               new MockData
                                   {
                                       RecKey = 1234,
                                       FieldToConvert1 = 5,
                                       FieldToConvert2 = 20,
                                       NonZeroableType = "foo"
                                   },
                               new MockData
                                   {
                                       RecKey = 1234,
                                       FieldToConvert1 = 5,
                                       FieldToConvert2 = 20,
                                       NonZeroableType = "foo"
                                   },
                               new MockData
                                   {
                                       RecKey = 5678,
                                       FieldToConvert1 = 10,
                                       FieldToConvert2 = 30,
                                       NonZeroableType = "foo"
                                   },
                               new MockData
                                   {
                                       RecKey = 5678,
                                       FieldToConvert1 = 10,
                                       FieldToConvert2 = 30,
                                       NonZeroableType = "foo"
                                   }
                           };
            }
        }

        internal class MockDataNoRecKey
        {
            public decimal FieldToConvert1 { get; set; }
            public decimal FieldToConvert2 { get; set; }

            public static List<MockDataNoRecKey> GenerateMockDataNoRecKey()
            {
                return new List<MockDataNoRecKey>
                           {
                               new MockDataNoRecKey
                                   {
                                       FieldToConvert1 = 5,
                                       FieldToConvert2 = 20
                                   }
                           };
            }
        }


    }
    
    
}
