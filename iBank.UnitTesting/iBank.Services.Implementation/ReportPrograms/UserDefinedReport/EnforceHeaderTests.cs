using iBank.Services.Implementation.ReportPrograms.UserDefined;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport
{
    [TestClass]
    public class EnforceHeaderTests
    {
        [TestMethod]
        public void EnforceHeaderChangeHeaderToMetric_Header1PassMilesAndLB_ReturnMatchExpected()
        {
            //Arrange
            var exp = new List<UserReportColumnInformation>
            {
                new UserReportColumnInformation {Header1 = "Miles" },
                new UserReportColumnInformation {Header1 = "LB"},
            };
            var act = new List<UserReportColumnInformation>
            {
                new UserReportColumnInformation {Header1 = "KILOMETERS" },
                new UserReportColumnInformation {Header1 = "KG"},
            };
            //Act
            EnforceHeader.ChangeHeadersToMetric(exp);

            //Assert
            Assert.AreEqual(exp.ToString(), act.ToString());
        }

        [TestMethod]
        public void EnforceHeaderChangeHeaderToMetric_Header2PassMilesAndPound_ReturnMatchExpected()
        {
            //Arrange
            var exp = new List<UserReportColumnInformation>
            {
                new UserReportColumnInformation {Header2 = "Miles" },
                new UserReportColumnInformation {Header2 = "POUND"},
            };
            var act = new List<UserReportColumnInformation>
            {
                new UserReportColumnInformation {Header2 = "KILOMETERS" },
                new UserReportColumnInformation {Header2 = "KG"},
            };
            //Act
            EnforceHeader.ChangeHeadersToMetric(exp);

            //Assert
            Assert.AreEqual(exp.ToString(), act.ToString());
        }

    }
}
