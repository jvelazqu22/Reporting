using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.ReportServer.Helpers;
using iBank.Repository.SQL.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iBank.UnitTesting.Miscellaneous
{
    [TestClass]
    public class PendingReportsFilterTests
    {

        [TestMethod]
        public void RetrieveStagedAgencyReports_ListsNotEmpty_PendingReportsIsUpdated()
        {
            // arrange
            var pendingReports = new List<PendingReportInformation>()
            {
                new PendingReportInformation() { Agency = "test1" },
                new PendingReportInformation() { Agency = "test2" },
                new PendingReportInformation() { Agency = "test3" },
            };
            IList<string> agenciesOnStage = new List<string>()
            {
                "test2",
                "test3",
            };
            var mock = new Mock<IMasterDataStore>();

            // act
            new PendingReportsFilter(mock.Object).RemoveStagedAgencies(pendingReports, agenciesOnStage);

            // assert
            Assert.AreEqual(1, pendingReports.Count);
            Assert.AreEqual("test1", pendingReports.First().Agency);
        }

        [TestMethod]
        public void RetrieveStagedAgencyReports_EmptyAgenciesStageList_PendingReportsIsNotUpdated()
        {
            // arrange
            var pendingReports = new List<PendingReportInformation>()
            {
                new PendingReportInformation() { Agency = "test1" },
                new PendingReportInformation() { Agency = "test2" },
                new PendingReportInformation() { Agency = "test3" },
            };
            IList<string> agenciesOnStage = new List<string>()
            {
            };
            var mock = new Mock<IMasterDataStore>();

            // act
            new PendingReportsFilter(mock.Object).RemoveStagedAgencies(pendingReports, agenciesOnStage);

            // assert
            Assert.AreEqual(3, pendingReports.Count);
            Assert.AreEqual("test1", pendingReports[0].Agency);
            Assert.AreEqual("test2", pendingReports[1].Agency);
            Assert.AreEqual("test3", pendingReports[2].Agency);
        }

        [TestMethod]
        public void RetrieveStagedAgencyReports_VariousValueInEmptyAgenciesStageListThatDontMatch_PendingReportsIsNotUpdated()
        {
            // arrange
            var pendingReports = new List<PendingReportInformation>()
            {
                new PendingReportInformation() { Agency = "test1" },
                new PendingReportInformation() { Agency = "test2" },
                new PendingReportInformation() { Agency = "test3" },
            };
            IList<string> agenciesOnStage = new List<string>()
            {
                string.Empty,
                null,
                "",
                "",
                "test4",
                "test5",
            };
            var mock = new Mock<IMasterDataStore>();

            // act
            new PendingReportsFilter(mock.Object).RemoveStagedAgencies(pendingReports, agenciesOnStage);

            // assert
            Assert.AreEqual(3, pendingReports.Count);
            Assert.AreEqual("test1", pendingReports[0].Agency);
            Assert.AreEqual("test2", pendingReports[1].Agency);
            Assert.AreEqual("test3", pendingReports[2].Agency);
        }
    }
}
