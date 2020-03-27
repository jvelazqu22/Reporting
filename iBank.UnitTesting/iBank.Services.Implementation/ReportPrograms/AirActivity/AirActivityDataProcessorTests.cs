using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iBank.Services.Implementation.ReportPrograms.AirActivity;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.AirActivity
{
    [TestClass]
    public class AirActivityDataProcessorTests
    {
        [TestMethod]
        public void GetZeroOutFields_ExcludeServiceFees_ReturnAirChg()
        {
            var processor = new AirActivityDataProcessor(new ClientFunctions());
            var expected = new List<string> { "airchg" };

            var output = processor.GetZeroOutFields(true);

            CollectionAssert.AreEqual(expected, output.ToList());
        }

        [TestMethod]
        public void GetZeroOutFields_IncludeServiceFees_ReturnAirChgSvcFee()
        {
            var processor = new AirActivityDataProcessor(new ClientFunctions());
            var expected = new List<string> { "airchg", "svcfee" };

            var output = processor.GetZeroOutFields(false);

            CollectionAssert.AreEqual(expected, output.ToList());
        }

    }
}
