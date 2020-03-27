using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.AirFareSavingsReport;

using iBank.Services.Implementation.ReportPrograms.AirFareSavings;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.AirFareSavings
{
    [TestClass]
    public class AirFareSavingsDataTests
    {
        [TestMethod]
        public void GetExportFields_TestingForUdids_UdidsExist_MapLabelToColumnName_ThenMapTextValueToLabel()
        {
            var data = new List<FinalData>
            {
                new FinalData { Udidlbl1 = "foo", Udidtext1 = "bar" }
            };
            var sut = new AirFareSavingsData();

            var output = sut.GetExportFields(false, false, false, false, null, data);

            Assert.AreEqual(true, output.Any(x => x.Equals("Udidlbl1 AS foo")));
            Assert.AreEqual(true, data.Any(x => x.Udidlbl1.Equals("bar")));
        }

        [TestMethod]
        public void GetExportFields_TestingForUdids_NoUdidsExist_DontIncludeHeaders()
        {
            var data = new List<FinalData>
                           {
                               new FinalData { Reckey = 1, Ticket = "blah" }
                           };
            var sut = new AirFareSavingsData();

            var output = sut.GetExportFields(false, false, false, false, null, data);

            Assert.AreEqual(false, output.Any(x => x.StartsWith("Udid")));
        }
    }
}
