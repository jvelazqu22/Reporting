using Domain.Helper;
using iBank.Server.Utilities.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Server.Utilities.Helpers
{
    [TestClass]
    public class ThreadLimitsGetServerNameCacheKeyForReportThreadsTests
    {
        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentReports_Server_10_ServersAreFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentReports_Server_10";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentReports_Server_10, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentReports_Server_11_ServersAreFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentReports_Server_11";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentReports_Server_11, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentReports_Server_20_ServersAreFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentReports_Server_20";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentReports_Server_20, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentReports_Server_21_ServersAreFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentReports_Server_21";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentReports_Server_21, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentReports_Server_22_ServersAreFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentReports_Server_22";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentReports_Server_22, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentReports_Server_23_ServersAreFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentReports_Server_23";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentReports_Server_23, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentReports_Server_24_ServersAreFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentReports_Server_24";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentReports_Server_24, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentReports_Server_25_ServersAreFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentReports_Server_25";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentReports_Server_25, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentReports_Server_26_ServersAreFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentReports_Server_26";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentReports_Server_26, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentReports_Server_27_ServersAreFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentReports_Server_27";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentReports_Server_27, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentReports_Server_30_ServersAreFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentReports_Server_30";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentReports_Server_30, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentReports_Server_31_ServersAreFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentReports_Server_31";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentReports_Server_31, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentReports_Server_100_ServersAreFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentReports_Server_100";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentReports_Server_100, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentReports_Server_200_ServersAreFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentReports_Server_200";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentReports_Server_200, result);
        }

    }
}
