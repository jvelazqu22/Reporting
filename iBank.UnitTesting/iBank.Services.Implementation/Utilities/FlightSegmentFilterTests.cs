using Domain.Interfaces;
using iBank.Services.Implementation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities
{
    [TestClass]
    public class FlightSegmentFilterTests
    {
        [TestMethod]
        public void UpdateFlightMarket()
        {
            var data = MockData.ReturnMock();
            var filter = new FlightSegmentFilter<MockData>();
            var expected = new MockData
                               {
                                   Flt_mkt = "foo-bar",
                                   Flt_mkt2 = "bar-foo",
                                   Origin = "foo",
                                   Destinat = "bar"
                               };

            var output = filter.UpdateFlightMarkets(data);

            Assert.AreEqual(expected.Flt_mkt, output[0].Flt_mkt);
            Assert.AreEqual(expected.Flt_mkt2, output[0].Flt_mkt2);
            Assert.AreEqual(expected.Origin, output[0].Origin);
            Assert.AreEqual(expected.Destinat, output[0].Destinat);
        }

        private class MockData : IFlightSegment
        {
            public string Flt_mkt { get; set; }
            public string Flt_mkt2 { get; set; }
            public string Origin { get; set; }
            public string Destinat { get; set; }

            public static IList<MockData> ReturnMock()
            {
                return new List<MockData>
                {
                    new MockData
                    {
                        Flt_mkt = "",
                        Flt_mkt2 = "",
                        Origin = "foo",
                        Destinat = "bar" 
                    }
                };
            }
        }
    }
}
