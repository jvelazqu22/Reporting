using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Services;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iBank.UnitTesting.Domain.LongRunningServiceTests
{
    [TestClass]
    public class IsLongRunningBroadcastTests
    {
        //TODO: long running - test adding default if not exists
        private IMasterDataStore _store;
        private ICacheService _cache;

        [TestInitialize]
        public void Init()
        {
            var db = new Mock<IMastersQueryable>();
            db.Setup(x => x.BroadcastLongRunningThreshold).Returns(GetThresholds);

            var mock = new Mock<IMasterDataStore>();
            mock.Setup(x => x.MastersQueryDb).Returns(db.Object);
            _store = mock.Object;

            _cache = GetMockCache();
        }

        private IQueryable<BroadcastLongRunningThreshold> GetThresholds()
        {
            return new List<BroadcastLongRunningThreshold>
            {
                new BroadcastLongRunningThreshold
                {
                    Id = 1,
                    Agency = Constants.DefaultFontPlaceholder,
                    MonthsInRangeThreshold = 12
                },
                new BroadcastLongRunningThreshold
                {
                    Id = 2,
                    Agency = "FOO",
                    MonthsInRangeThreshold = 3
                }
            }.AsQueryable();
        }

        private Dictionary<string, int> GetCacheDict()
        {
            return new Dictionary<string, int>
            {
                {Constants.DefaultFontPlaceholder, 6},
                {"bar", 9}
            };
        }

        private ICacheService GetMockCache()
        {
            object expectedReturn = GetCacheDict();
            var mockCache = new Mock<ICacheService>();
            mockCache.Setup(x => x.TryGetValue(CacheKeys.BroadcastLongRunningThreshold, out expectedReturn))
                .Returns(true);
            return mockCache.Object;
        }

        [TestMethod]
        public void DefaultAgencyDoesNotExist_InsertIt()
        {
            var queryDb = new Mock<IMastersQueryable>();
            var cmdDb = new Mock<ICommandDb>();
            var localStore = new Mock<IMasterDataStore>();
            var thresholds = GetThresholds().Where(x => !x.Agency.Equals(Constants.DefaultFontPlaceholder));
            queryDb.Setup(x => x.BroadcastLongRunningThreshold).Returns(thresholds);
            localStore.Setup(x => x.MastersQueryDb).Returns(queryDb.Object);
            localStore.Setup(x => x.MastersCommandDb).Returns(cmdDb.Object);
            var mockCache = new Mock<ICacheService>();

            var sut = new LongRunningService(mockCache.Object, localStore.Object);
            var bcst = new bcstque4
            {
                agency = "blah",
                nxtdstart = new DateTime(2015, 1, 1),
                nxtdend = new DateTime(2015, 8, 31)
            };

            var output = sut.IsLongRunningBroadcast(bcst);

            cmdDb.Verify(x => x.Insert(
                        It.Is<IList<BroadcastLongRunningThreshold>>(
                                recs => recs.Count == 1
                                        && recs.Count(r => r.Agency.Equals(Constants.DefaultFontPlaceholder)) == 1
                            )));
        }

        [TestMethod]
        public void AgencyThresholdExists_NotCached_BroadcastOverLimit_ReturnTrue()
        {
            var sut = new LongRunningService(new CacheService(), _store);
            var bcst = new bcstque4
            {
                agency = "foo",
                nxtdstart = new DateTime(2015, 1, 1),
                nxtdend = new DateTime(2015, 12, 31)
            };

            var output = sut.IsLongRunningBroadcast(bcst);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void AgencyThresholdDoesNotExist_NotCached_BroadcastAtLimit_ReturnTrue()
        {
            var sut = new LongRunningService(new CacheService(), _store);
            var bcst = new bcstque4
            {
                agency = "bar",
                nxtdstart = new DateTime(2015, 1, 1),
                nxtdend = new DateTime(2015, 12, 31)
            };

            var output = sut.IsLongRunningBroadcast(bcst);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void AgencyThresholdExists_Cached_BroadcastOverLimit_ReturnTrue()
        {
            var sut = new LongRunningService(_cache, _store);
            var bcst = new bcstque4
            {
                agency = "bar",
                nxtdstart = new DateTime(2015, 1, 1),
                nxtdend = new DateTime(2015, 11, 30)
            };

            var output = sut.IsLongRunningBroadcast(bcst);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void AgencyThresholdDoesNotExist_Cached_BroadcastOverLimit_ReturnTrue()
        {
            var sut = new LongRunningService(_cache, _store);
            var bcst = new bcstque4
            {
                agency = "blah",
                nxtdstart = new DateTime(2015, 1, 1),
                nxtdend = new DateTime(2015, 8, 31)
            };

            var output = sut.IsLongRunningBroadcast(bcst);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void BroadcastUnderLimit_NotCached_ReturnFalse()
        {
            var sut = new LongRunningService(new CacheService(), _store);
            var bcst = new bcstque4
            {
                agency = "foo",
                nxtdstart = new DateTime(2015, 1, 1),
                nxtdend = new DateTime(2015, 1, 2)
            };

            var output = sut.IsLongRunningBroadcast(bcst);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BroadcastUnderLimit_Cached_ReturnFalse()
        {
            var sut = new LongRunningService(_cache, _store);
            var bcst = new bcstque4
            {
                agency = "bar",
                nxtdstart = new DateTime(2015, 1, 1),
                nxtdend = new DateTime(2015, 1, 2)
            };

            var output = sut.IsLongRunningBroadcast(bcst);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BroadcastRunspcl_RunspclActDataUnderLimitSpclDateOver_Cached_ReturnTrue()
        {
            var sut = new LongRunningService(_cache, _store);
            var bcst = new bcstque4
            {
                agency = "bar",
                nxtdstart = new DateTime(2015, 1, 1),
                nxtdend = new DateTime(2015, 1, 2),
                runspcl = true,
                spclstart = new DateTime(2014, 1, 1),
                spclend = new DateTime(2015, 1, 1),
            };

            var output = sut.IsLongRunningBroadcast(bcst);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void BroadcastRunspcl_RunspclActDataUnderLimitSpclDateUnder_Cached_ReturnFalse()
        {
            var sut = new LongRunningService(_cache, _store);
            var bcst = new bcstque4
            {
                agency = "bar",
                nxtdstart = new DateTime(2015, 1, 1),
                nxtdend = new DateTime(2015, 1, 2),
                runspcl = true,
                spclstart = new DateTime(2015, 1, 1),
                spclend = new DateTime(2015, 1, 2),
            };

            var output = sut.IsLongRunningBroadcast(bcst);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BroadcastRunspcl_RunspclActDateRangeOverPastDateRangeUnder_Cached_ReturnFalse()
        {
            var sut = new LongRunningService(_cache, _store);
            var today = DateTime.Now;
            var spclstart = today.AddDays(-10);
            var spclend = today.AddDays(360);

            var bcst = new bcstque4
            {
                agency = "bar",
                nxtdstart = new DateTime(2015, 1, 1),
                nxtdend = new DateTime(2015, 1, 2),
                runspcl = true,
                spclstart = spclstart,
                spclend = today,
            };

            var output = sut.IsLongRunningBroadcast(bcst);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BroadcastRunspcl_ActDateRangeOverPastDateRangeUnder_Cached_ReturnFalse()
        {
            var sut = new LongRunningService(_cache, _store);
            var today = DateTime.Now;
            var nxtdstart = today.AddDays(-10);
            var nxtdend = today.AddDays(360);

            var bcst = new bcstque4
            {
                agency = "bar",
                nxtdstart = nxtdstart,
                nxtdend = nxtdend
            };

            var output = sut.IsLongRunningBroadcast(bcst);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void BroadcastRunspcl_ActDateRangeOverPastDateRangeOver_Cached_ReturnTrue()
        {
            var sut = new LongRunningService(_cache, _store);
            var today = DateTime.Now;
            var nxtdstart = today.AddMonths(-9);

            var bcst = new bcstque4
            {
                agency = "bar",
                nxtdstart = nxtdstart,
                nxtdend = today
            };

            var output = sut.IsLongRunningBroadcast(bcst);

            Assert.AreEqual(true, output);
        }
    }
}
