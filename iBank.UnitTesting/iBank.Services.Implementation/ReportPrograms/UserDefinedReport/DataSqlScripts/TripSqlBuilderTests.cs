using iBank.Services.Implementation.ReportPrograms.UserDefined.DataSqlScripts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport.DataSqlScripts
{
    /// <summary>
    /// Summary description for TripSqlBuilderTests
    /// </summary>
    [TestClass]
    public class TripSqlBuilderTests
    {

        [TestMethod]
        public void GetTripFromClause_NoUdid_PreviewNotTLS_ReturnMatch()
        {
            //Arrange
            var sqlBuilder = new UserReportTripSqlBuilder();
            string exp = "ibtrips T1";

            //Act
            string act = sqlBuilder.GetTripFromClause(0, true, false);

            //Assert
            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void GetTripFromClause_NoUdid_NotPreviewNotTLS_ReturnMatch()
        {
            //Arrange
            var sqlBuilder = new UserReportTripSqlBuilder();
            string exp = "hibtrips T1";

            //Act
            string act = sqlBuilder.GetTripFromClause(0, false, false);

            //Assert
            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void GetTripFromClause_HasUdid_PreviewNotTLS_ReturnMatch()
        {
            //Arrange
            var sqlBuilder = new UserReportTripSqlBuilder();
            string exp = "ibtrips T1, ibudids T3";

            //Act
            string act = sqlBuilder.GetTripFromClause(99, true, false);

            //Assert
            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void GetTripFromClause_HasUdid_NotPreviewNotTLS_ReturnMatch()
        {
            //Arrange
            var sqlBuilder = new UserReportTripSqlBuilder();
            string exp = "hibtrips T1, hibudids T3";

            //Act
            string act = sqlBuilder.GetTripFromClause(99, false, false);

            //Assert
            Assert.AreEqual(exp, act);
        }
    }
}
