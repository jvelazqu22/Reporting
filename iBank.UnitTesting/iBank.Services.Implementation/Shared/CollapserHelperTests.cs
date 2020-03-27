using System;
using System.Collections.Generic;

using Domain.Interfaces;

using iBank.Services.Implementation.Shared;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared
{
    [TestClass]
    public class CollapserHelperTests
    {
        [TestMethod]
        public void GetDomesticInternationalType_InternationalLegExists_ReturnInternational()
        {
            var data = new List<MockData>
            {
                new MockData { DitCode = "D" },
                new MockData { DitCode = "T" },
                new MockData { DitCode = "I" }
            };

            var output = CollapserHelper.GetDomesticInternationalType(data);

            Assert.AreEqual("I", output);
        }

        [TestMethod]
        public void GetDomesticInternationalType_NoInternationalLegExists_TransborderLegExists_ReturnTransborder()
        {
            var data = new List<MockData>
                           {
                               new MockData { DitCode = "D" },
                               new MockData { DitCode = "T" }
                           };

            var output = CollapserHelper.GetDomesticInternationalType(data);

            Assert.AreEqual("T", output);
        }

        [TestMethod]
        public void GetDomesticInternationalType_NoInternationalLegExists_NoTransborderLegExists_ReturnDomestic()
        {
            var data = new List<MockData>
                           {
                               new MockData { DitCode = "D" },
                               new MockData { DitCode = "D" }
                           };

            var output = CollapserHelper.GetDomesticInternationalType(data);

            Assert.AreEqual("D", output);
        }
    }

    class MockData : ICollapsible
    {
        public int RecKey { get; set; }
        public string DitCode { get; set; }
        public string DomIntl { get; set; }
        public int Miles { get; set; }
        public string Mode { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public decimal ActFare { get; set; }
        public decimal MiscAmt { get; set; }
        public string Connect { get; set; }
        public int SeqNo { get; set; }
        public DateTime? RDepDate { get; set; }
        public string DepTime { get; set; }
        public string Airline { get; set; }
        public string fltno { get; set; }
        public string ClassCode { get; set; }
        public int Seg_Cntr { get; set; }
        public decimal AirCo2 { get; set; }
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }
}
