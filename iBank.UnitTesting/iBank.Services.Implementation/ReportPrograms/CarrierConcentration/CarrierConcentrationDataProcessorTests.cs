using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.CarrierConcentrationReport;

using iBank.Services.Implementation.ReportPrograms.CarrierConcentration;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.CarrierConcentration
{
    [TestClass]
    public class CarrierConcentrationDataProcessorTests
    {
        [TestMethod]
        public void SetFlightMarket()
        {
            var rawData = new List<RawData>
                              {
                                new RawData { Origin = "ORIGIN", Destinat = "DESTINATION" }
                              };
            var sut = new CarrierConcentrationDataProcessor();

            sut.SetFlightMarkets(rawData);

            Assert.AreEqual(rawData[0].Flt_mkt, "ORIGIN-DESTINATION");
            Assert.AreEqual(rawData[0].Flt_mkt2, "DESTINATION-ORIGIN");
        }

        [TestMethod]
        public void GetDataFilteredOnFlightSegments_Bidirectional_ReturnDataWithFlightMarket1Or2()
        {
            var flightSegments = "FOO,BAR";
            var data = new List<IntermediaryData>
            {
                new IntermediaryData { Flt_mkt = "FOO", Flt_mkt2 = "BAR", Origin = "BOTH_ARE_IN" },
                new IntermediaryData { Flt_mkt = "FOO", Flt_mkt2 = "FOOBAR", Origin = "FIRST_MATCH" },
                new IntermediaryData { Flt_mkt = "FOOBAR", Flt_mkt2 = "BAR", Origin = "LAST_MATCH" },
                new IntermediaryData { Flt_mkt = "FOOBAR", Flt_mkt2 = "FOOBAR", Origin = "NO_MATCH" },
            };
            var bidirectional = true;
            var sut = new CarrierConcentrationDataProcessor();

            var output = sut.GetDataFilteredOnFlightSegments(data, flightSegments, bidirectional);

            Assert.AreEqual(3, output.Count);
            Assert.AreEqual(1, output.Count(x => x.Origin == "BOTH_ARE_IN"));
            Assert.AreEqual(1, output.Count(x => x.Origin == "FIRST_MATCH"));
            Assert.AreEqual(1, output.Count(x => x.Origin == "LAST_MATCH"));
            Assert.AreEqual(0, output.Count(x => x.Origin == "NO_MATCH"));
        }

        [TestMethod]
        public void GetDataFilteredOnFlightSegments_NotBidirectional_ReturnDataWithFlightMarket1()
        {
            var flightSegments = "FOO,BAR";
            var data = new List<IntermediaryData>
            {
                new IntermediaryData { Flt_mkt = "FOO", Flt_mkt2 = "BAR", Origin = "BOTH_ARE_IN" },
                new IntermediaryData { Flt_mkt = "FOO", Flt_mkt2 = "FOOBAR", Origin = "FIRST_MATCH" },
                new IntermediaryData { Flt_mkt = "FOOBAR", Flt_mkt2 = "BAR", Origin = "LAST_MATCH" },
                new IntermediaryData { Flt_mkt = "FOOBAR", Flt_mkt2 = "FOOBAR", Origin = "NO_MATCH" },
            };
            var bidirectional = false;
            var sut = new CarrierConcentrationDataProcessor();

            var output = sut.GetDataFilteredOnFlightSegments(data, flightSegments, bidirectional);

            Assert.AreEqual(2, output.Count);
            Assert.AreEqual(1, output.Count(x => x.Origin == "BOTH_ARE_IN"));
            Assert.AreEqual(1, output.Count(x => x.Origin == "FIRST_MATCH"));
            Assert.AreEqual(0, output.Count(x => x.Origin == "LAST_MATCH"));
            Assert.AreEqual(0, output.Count(x => x.Origin == "NO_MATCH"));
        }

        [TestMethod]
        public void GetDataFilteredOnFlightSegments_Bidirectional_NoMatchingFlightMarket_ReturnEmpty()
        {
            var flightSegments = "XXX";
            var data = new List<IntermediaryData>
            {
                new IntermediaryData { Flt_mkt = "FOO", Flt_mkt2 = "BAR", Origin = "BOTH_ARE_IN" },
                new IntermediaryData { Flt_mkt = "FOO", Flt_mkt2 = "FOOBAR", Origin = "FIRST_MATCH" },
                new IntermediaryData { Flt_mkt = "FOOBAR", Flt_mkt2 = "BAR", Origin = "LAST_MATCH" },
                new IntermediaryData { Flt_mkt = "FOOBAR", Flt_mkt2 = "FOOBAR", Origin = "NO_MATCH" },
            };
            var bidirectional = true;
            var sut = new CarrierConcentrationDataProcessor();

            var output = sut.GetDataFilteredOnFlightSegments(data, flightSegments, bidirectional);

            Assert.AreEqual(0, output.Count);
        }

        [TestMethod]
        public void AllocateAirCharge()
        {
            var data = new List<RawData>
            {
                new RawData { ActFare = 0, RecKey = 1, Airchg = 4, Faretax = 2 },
                new RawData { ActFare = 0, RecKey = 1, Airchg = 6, Faretax = 4 },
                new RawData { ActFare = 1, RecKey = 2, Airchg = 4, Faretax = 2 },
                new RawData { ActFare = 2, RecKey = 3, Airchg = 6, Faretax = 4 }
            };
            var sut = new CarrierConcentrationDataProcessor();

            sut.AllocateAirCharge(data);

            Assert.AreEqual(4, data.Count);
            Assert.AreEqual(1, data[0].ActFare);
            Assert.AreEqual(1, data[1].ActFare);
            Assert.AreEqual(2, data[3].ActFare);
        }
    }
}
