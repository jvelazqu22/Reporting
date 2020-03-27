using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Exceptions;
using Domain.Helper;
using Domain.Services;
using iBank.Entities.AdministrationEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iBank.UnitTesting.iBank.Server.Utilities.Helpers
{
    [TestClass]
    public class ThreadLimitsBroadcastTests
    {
        [TestMethod]

        public void GetMaxConcurrentBroadcastsForServer_MaxConcurrentBroadcasts_Server_20_Returns10Threads()
        {
            // Arrange
            var serverNumber = 20;
            var cache = new CacheService();
            var db = new Mock<IAdministrationQueryable>();
            var listOfReportingConfiguration = new List<ReportingConfiguration>() { new ReportingConfiguration() { Name = "MaxConcurrentBroadcasts_Server_20", Value = "10" } };
            db.Setup(x => x.ReportingConfiguration).Returns(listOfReportingConfiguration.AsQueryable());

            // Act 
            var result = ThreadLimits.GetMaxConcurrentBroadcastsForServer(cache, serverNumber, db.Object);

            // Assert
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void GetMaxConcurrentBroadcastsForServer_CacheNotEmtpyAndHasNotExpired_ReturnValueFromCache()
        {
            // Arrange
            var serverNumber = 20;
            var cacheConfig = new ReportingConfiguration() { Name = "MaxConcurrentBroadcasts_Server_20", Value = "5" };
            var cache = new CacheService();
            cache.Set(CacheKeys.MaxConcurrentBroadcasts_Server_20, cacheConfig, DateTime.Now.AddSeconds(120));
            var db = new Mock<IAdministrationQueryable>();
            var listOfReportingConfiguration = new List<ReportingConfiguration>() { new ReportingConfiguration() { Name = "MaxConcurrentBroadcasts_Server_20", Value = "10" } };
            db.Setup(x => x.ReportingConfiguration).Returns(listOfReportingConfiguration.AsQueryable());

            // Act 
            var result = ThreadLimits.GetMaxConcurrentBroadcastsForServer(cache, serverNumber, db.Object);

            // Assert
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void GetMaxConcurrentBroadcastsForServer_CacheNotEmtpyButHasNotExpired_ReturnValueFromDb()
        {
            // Arrange
            var serverNumber = 20;
            var cacheConfig = new ReportingConfiguration() { Name = "MaxConcurrentBroadcasts_Server_20", Value = "5" };
            var cache = new CacheService();
            cache.Set(CacheKeys.MaxConcurrentBroadcasts_Server_20, cacheConfig, DateTime.Now.AddSeconds(0.000000001));
            var db = new Mock<IAdministrationQueryable>();
            var listOfReportingConfiguration = new List<ReportingConfiguration>() { new ReportingConfiguration() { Name = "MaxConcurrentBroadcasts_Server_20", Value = "10" } };
            db.Setup(x => x.ReportingConfiguration).Returns(listOfReportingConfiguration.AsQueryable());

            // Act 
            var result = ThreadLimits.GetMaxConcurrentBroadcastsForServer(cache, serverNumber, db.Object);

            // Assert
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDatabaseConfigurationException))]
        public void GetServerNameCacheKey_InvalidServerName_ThrowInvalidDatabaseConfigurationException()
        {
            // Arrange
            const string serverName = "MaxConcurrentBroadcasts_Server_1000";

            // Act 
            ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
        }

    }
}
