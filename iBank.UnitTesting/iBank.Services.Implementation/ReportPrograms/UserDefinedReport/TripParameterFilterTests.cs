using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.UserDefinedReport;

using iBank.Services.Implementation.ReportPrograms.UserDefined;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport
{
    [TestClass]
    public class TripParameterFilterTests
    {
        private List<RawData> _rawDataOne;

        private List<RawData> _rawDataTwo;

        private List<RawData> _rawDataThree;

        private Dictionary<int, List<RawData>> _rawDataDict;

        private List<HotelRawData> _hotelRawOne;

        private List<HotelRawData> _hotelRawTwo;

        private Dictionary<int, List<HotelRawData>> _hotelRawDict;

        [TestInitialize]
        public void Init()
        {
            // Raw Data
            //key one
            _rawDataOne = new List<RawData>();
            _rawDataOne.Add(new RawData { RecKey = 1, Recloc = "abc", Agency = "correct" });
            _rawDataOne.Add(new RawData { RecKey = 1, Recloc = "zzz", Agency = "not correct" });

            //key two
            _rawDataTwo = new List<RawData>();
            _rawDataTwo.Add(new RawData { RecKey = 2, Recloc = "abc", Agency = "correct" });
            _rawDataTwo.Add(new RawData { RecKey = 2, Recloc = "zzz", Agency = "not correct" });

            //key three
            _rawDataThree = new List<RawData>();
            _rawDataThree.Add(new RawData { RecKey = 3, Recloc = "aaa", Agency = "extra" });

            //raw data dict
            _rawDataDict = new Dictionary<int, List<RawData>>();
            _rawDataDict.Add(1, _rawDataOne);
            _rawDataDict.Add(2, _rawDataTwo);
            _rawDataDict.Add(3, _rawDataThree);

            //Hotel Raw Data
            //key one
            _hotelRawOne = new List<HotelRawData>();
            _hotelRawOne.Add(new HotelRawData { RecKey = 1, Recloc = "abc", Agency = "correct" });
            _hotelRawOne.Add(new HotelRawData { RecKey = 1, Recloc = "zzz", Agency = "not correct" });

            //key two
            _hotelRawTwo = new List<HotelRawData>();
            _hotelRawTwo.Add(new HotelRawData { RecKey = 2, Recloc = "abc", Agency = "correct" });
            _hotelRawTwo.Add(new HotelRawData { RecKey = 2, Recloc = "zzz", Agency = "not correct" });

            //raw data dict
            _hotelRawDict = new Dictionary<int, List<HotelRawData>>();
            _hotelRawDict.Add(1, _hotelRawOne);
            _hotelRawDict.Add(2, _hotelRawTwo);


        }

        [TestMethod]
        public void GetDataFromRecloc_RawData_ReclocExistsAcrossMultipleTrips_ReturnAllMatchingRecords()
        {
            var output = TripParameterFilter.GetDataFromRecloc(_rawDataDict, "abc");

            Assert.AreEqual(2, output.Count);
            Assert.AreEqual(true, output.Any(x => x.Agency.Equals("correct") && x.RecKey == 1));
            Assert.AreEqual(true, output.Any(x => x.Agency.Equals("correct") && x.RecKey == 2));
        }

        [TestMethod]
        public void GetDataFromRecloc_HotelRawData_ReclocExistsAcrossMultipleTrips_ReturnAllMatchingRecords()
        {
            var output = TripParameterFilter.GetDataFromRecloc(_hotelRawDict, "abc");

            Assert.AreEqual(2, output.Count);
            Assert.AreEqual(true, output.Any(x => x.Agency.Equals("correct") && x.RecKey == 1));
            Assert.AreEqual(true, output.Any(x => x.Agency.Equals("correct") && x.RecKey == 2));
        }

        [TestMethod]
        public void GetDataFromRecloc_RawData_NoReclocExists_ReturnEmptyList()
        {
            var output = TripParameterFilter.GetDataFromRecloc(_rawDataDict, "xxx");

            Assert.AreEqual(0, output.Count);
        }

        [TestMethod]
        public void GetDataFromRecloc_HotelRawData_NoReclocExists_ReturnEmptyList()
        {
            var output = TripParameterFilter.GetDataFromRecloc(_hotelRawDict, "xxx");

            Assert.AreEqual(0, output.Count);
        }

        [TestMethod]
        public void GetCountOfMatchingRecKeys()
        {
            var output = TripParameterFilter.GetCountOfMatchingRecKeys(new List<int> { 1, 2 }, _rawDataDict);

            Assert.AreEqual(4, output);
        }

        [TestMethod]
        public void GetDataFromReckeys()
        {
            var output = TripParameterFilter.GetDataFromReckeys(new List<int> { 1, 2 }, _rawDataDict);

            Assert.AreEqual(4, output.Count);
            Assert.AreEqual(2, output.Count(x => x.RecKey == 1));
            Assert.AreEqual(2, output.Count(x => x.RecKey == 2));
        }
    }
}
