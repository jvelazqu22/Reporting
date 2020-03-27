using System.Collections.Generic;
using System.Linq;
using Domain.Services;
using iBank.Entities.AdministrationEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iBank.UnitTesting.iBank.Server.Utilities.Helpers
{
    [TestClass]
    public class ThreadLimitsErrorsTests
    {
        [TestMethod]

        public void GetMaxConcurrentBroadcastsForServer_InvalidServerNumber_CatchExceptionAndReturnDefaultValue()
        {
            // Arrange
            var serverNumber = 0;
            var cache = new CacheService();
            var db = new Mock<IAdministrationQueryable>();
            var listOfReportingConfiguration = new List<ReportingConfiguration>() { new ReportingConfiguration() { Name = "MaxConcurrentBroadcasts_Server_20", Value = "10" } };
            db.Setup(x => x.ReportingConfiguration).Returns(listOfReportingConfiguration.AsQueryable());

            // Act 
            var result = ThreadLimits.GetMaxConcurrentBroadcastsForServer(cache, serverNumber, db.Object);

            // Assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]

        public void GetMaxConcurrentReportsForServer_InvalidServerNumber_CatchExceptionAndReturnDefaultValue()
        {
            // Arrange
            var serverNumber = 0;
            var cache = new CacheService();
            var db = new Mock<IAdministrationQueryable>();
            var listOfReportingConfiguration = new List<ReportingConfiguration>() { new ReportingConfiguration() { Name = "MaxConcurrentReports_Server_11", Value = "10" } };
            db.Setup(x => x.ReportingConfiguration).Returns(listOfReportingConfiguration.AsQueryable());

            // Act 
            var result = ThreadLimits.GetMaxConcurrentReportsForServer(cache, serverNumber, db.Object);

            // Assert
            Assert.AreEqual(3, result);
        }
    }
}
