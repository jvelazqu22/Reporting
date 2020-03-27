using Domain.Helper;
using iBank.Server.Utilities.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Server.Utilities.Helpers
{
    [TestClass]
    public class ThreadLimitsGetServerNameCacheKeyForBroadcastThreadsTests
    {

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentBroadcasts_Server_20_ServerIsFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentBroadcasts_Server_20";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentBroadcasts_Server_20, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentBroadcasts_Server_21_ServerIsFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentBroadcasts_Server_21";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentBroadcasts_Server_21, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentBroadcasts_Server_22_ServerIsFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentBroadcasts_Server_22";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentBroadcasts_Server_22, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentBroadcasts_Server_23_ServerIsFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentBroadcasts_Server_23";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentBroadcasts_Server_23, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentBroadcasts_Server_24_ServerIsFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentBroadcasts_Server_24";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentBroadcasts_Server_24, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentBroadcasts_Server_25_ServerIsFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentBroadcasts_Server_25";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentBroadcasts_Server_25, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentBroadcasts_Server_26_ServerIsFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentBroadcasts_Server_26";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentBroadcasts_Server_26, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentBroadcasts_Server_27_ServerIsFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentBroadcasts_Server_27";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentBroadcasts_Server_27, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentBroadcasts_Server_30_ServerIsFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentBroadcasts_Server_30";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentBroadcasts_Server_30, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentBroadcasts_Server_200_ServerIsFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentBroadcasts_Server_200";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentBroadcasts_Server_200, result);
        }

        [TestMethod]
        public void GetMaxConcurrentReportsForServer_MaxConcurrentBroadcasts_Server_201_ServerIsFoundInCacheKeys()
        {
            // Arrange
            const string serverName = "MaxConcurrentBroadcasts_Server_201";

            // Act 
            var result = ThreadLimits.GetServerNameCacheKey(serverName);

            // Assert
            Assert.AreEqual(CacheKeys.MaxConcurrentBroadcasts_Server_201, result);
        }

    }
}
