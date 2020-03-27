using Domain.Helper;
using iBank.Server.Utilities.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;
using Domain.Exceptions;
using Domain.Helper;
using Domain.Services;
using iBank.Entities.AdministrationEntities;
using iBank.Repository.SQL.Interfaces;
using Moq;


namespace iBank.UnitTesting.iBank.Server.Utilities.Helpers
{
    [TestClass]
    public class NETWaitForFoxProToReturnPropertyTests
    {
        [TestMethod]
        public void GetMinutesToWaitForFoxProToReturnForTheServer_BCST_20_FoundInCacheKeys()
        {
            // Arrange
            const string configName = "MinutesToWaitForFoxProToReturn_BCST_20";

            // Act 
            var result = NETWaitForFoxProToReturnProperty.GetServerCacheKey(configName);

            // Assert
            Assert.AreEqual(CacheKeys.MinutesToWaitForFoxProToReturn_BCST_20, result);
        }

        [TestMethod]
        public void GetMinutesToWaitForFoxProToReturnForTheServer_BCST_21_FoundInCacheKeys()
        {
            // Arrange
            const string configName = "MinutesToWaitForFoxProToReturn_BCST_21";

            // Act 
            var result = NETWaitForFoxProToReturnProperty.GetServerCacheKey(configName);

            // Assert
            Assert.AreEqual(CacheKeys.MinutesToWaitForFoxProToReturn_BCST_21, result);
        }

        [TestMethod]
        public void GetMinutesToWaitForFoxProToReturnForTheServer_BCST_24_FoundInCacheKeys()
        {
            // Arrange
            const string configName = "MinutesToWaitForFoxProToReturn_BCST_24";

            // Act 
            var result = NETWaitForFoxProToReturnProperty.GetServerCacheKey(configName);

            // Assert
            Assert.AreEqual(CacheKeys.MinutesToWaitForFoxProToReturn_BCST_24, result);
        }

        [TestMethod]
        public void GetMinutesToWaitForFoxProToReturnForTheServer_BCST_25_FoundInCacheKeys()
        {
            // Arrange
            const string configName = "MinutesToWaitForFoxProToReturn_BCST_25";

            // Act 
            var result = NETWaitForFoxProToReturnProperty.GetServerCacheKey(configName);

            // Assert
            Assert.AreEqual(CacheKeys.MinutesToWaitForFoxProToReturn_BCST_25, result);
        }

        [TestMethod]
        public void GetMinutesToWaitForFoxProToReturnForTheServer_BCST_26_FoundInCacheKeys()
        {
            // Arrange
            const string configName = "MinutesToWaitForFoxProToReturn_BCST_26";

            // Act 
            var result = NETWaitForFoxProToReturnProperty.GetServerCacheKey(configName);

            // Assert
            Assert.AreEqual(CacheKeys.MinutesToWaitForFoxProToReturn_BCST_26, result);
        }

        [TestMethod]
        public void GetMinutesToWaitForFoxProToReturnForTheServer_BCST_27_FoundInCacheKeys()
        {
            // Arrange
            const string configName = "MinutesToWaitForFoxProToReturn_BCST_27";

            // Act 
            var result = NETWaitForFoxProToReturnProperty.GetServerCacheKey(configName);

            // Assert
            Assert.AreEqual(CacheKeys.MinutesToWaitForFoxProToReturn_BCST_27, result);
        }

        [TestMethod]
        public void GetMinutesToWaitForFoxProToReturnForTheServer_BCST_30_FoundInCacheKeys()
        {
            // Arrange
            const string configName = "MinutesToWaitForFoxProToReturn_BCST_30";

            // Act 
            var result = NETWaitForFoxProToReturnProperty.GetServerCacheKey(configName);

            // Assert
            Assert.AreEqual(CacheKeys.MinutesToWaitForFoxProToReturn_BCST_30, result);
        }
        [TestMethod]
        public void GetMinutesToWaitForFoxProToReturnForTheServer_BCST_31_FoundInCacheKeys()
        {
            // Arrange
            const string configName = "MinutesToWaitForFoxProToReturn_BCST_31";

            // Act 
            var result = NETWaitForFoxProToReturnProperty.GetServerCacheKey(configName);

            // Assert
            Assert.AreEqual(CacheKeys.MinutesToWaitForFoxProToReturn_BCST_31, result);
        }

        [TestMethod]
        public void GetMinutesToWaitForFoxProToReturnForTheServer_BCST_200_FoundInCacheKeys()
        {
            // Arrange
            const string configName = "MinutesToWaitForFoxProToReturn_BCST_200";

            // Act 
            var result = NETWaitForFoxProToReturnProperty.GetServerCacheKey(configName);

            // Assert
            Assert.AreEqual(CacheKeys.MinutesToWaitForFoxProToReturn_BCST_200, result);
        }

        [TestMethod]
        public void GetMinutesToWaitForFoxProToReturnForTheServer_BCST_201_FoundInCacheKeys()
        {
            // Arrange
            const string configName = "MinutesToWaitForFoxProToReturn_BCST_201";

            // Act 
            var result = NETWaitForFoxProToReturnProperty.GetServerCacheKey(configName);

            // Assert
            Assert.AreEqual(CacheKeys.MinutesToWaitForFoxProToReturn_BCST_201, result);
        }


        [TestMethod]
        public void GetMinutesToWaitForFoxProToReturnForTheServer_ToWait_Server_20_Returns120Minutes()
        {
            // Arrange
            var serverNumber = 20;
            var cache = new CacheService();
            var db = new Mock<IAdministrationQueryable>();
            var listOfReportingConfiguration = new List<ReportingConfiguration>() { new ReportingConfiguration() { Name = "MinutesToWaitForFoxProToReturn_BCST_20", Value = "120" } };
            db.Setup(x => x.ReportingConfiguration).Returns(listOfReportingConfiguration.AsQueryable());

            // Act 
            var result = NETWaitForFoxProToReturnProperty.GetMinutesToWaitForFoxProToReturnForTheServer(cache, serverNumber, db.Object);

            // Assert
            Assert.AreEqual(120, result);
        }


        [TestMethod]
        public void GetMinutesToWaitForFoxProToReturnForTheServer_ToWait_Server_99_Returns180Minutes()
        {
            // Arrange
            var serverNumber = 99;
            var cache = new CacheService();
            var db = new Mock<IAdministrationQueryable>();
            var listOfReportingConfiguration = new List<ReportingConfiguration>() { new ReportingConfiguration() { Name = "MinutesToWaitForFoxProToReturn_BCST_20", Value = "120" } };
            db.Setup(x => x.ReportingConfiguration).Returns(listOfReportingConfiguration.AsQueryable());

            // Act 
            var result = NETWaitForFoxProToReturnProperty.GetMinutesToWaitForFoxProToReturnForTheServer(cache, serverNumber, db.Object);

            // Assert
            Assert.AreEqual(180, result);
        }
    }
}
