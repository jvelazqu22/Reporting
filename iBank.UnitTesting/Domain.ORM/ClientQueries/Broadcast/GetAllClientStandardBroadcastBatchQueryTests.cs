using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper;
using Domain.Orm.iBankClientQueries.BroadcastQueries;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.UnitTesting.TestingHelpers.MockHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace iBank.UnitTesting.Domain.ORM.ClientQueries.Broadcast
{
    [TestClass]
    public class GetAllClientStandardBroadcastBatchQueryTests
    {
        private List<ibbatch> InitializeData()
        {
            return new List<ibbatch>
                           {
                               //valid
                               new ibbatch
                                   {
                                       batchnum = 1,
                                       batchname = "valid",
                                       errflag = false,
                                       holdrun = "R",
                                       outputdest = "1",
                                       nextrun = new DateTime(2016, 1, 1, 1, 00, 00), 
                                       agency = "DEMO"
                                   },
                               //valid but not ready to run yet
                               new ibbatch
                                   {
                                       batchnum = 2,
                                       batchname = "valid but not ready to run yet",
                                       errflag = false,
                                       holdrun = "R",
                                       outputdest = "1",
                                       nextrun = new DateTime(2200, 6, 16, 10, 45, 00),
                                       agency = "DEMO"
                                   },
                               //in error
                               new ibbatch
                                   {
                                       batchnum = 3,
                                       batchname = "error",
                                       errflag = true,
                                       holdrun = "R",
                                       outputdest = "1",
                                       nextrun = new DateTime(2016, 6, 16, 10, 45, 00),
                                       agency = "DEMO"
                                   },
                               //on hold
                               new ibbatch
                                   {
                                       batchnum = 4,
                                       batchname = "hold",
                                       errflag = false,
                                       holdrun = "H",
                                       outputdest = "1",
                                       nextrun = new DateTime(2016, 6, 16, 10, 45, 00),
                                       agency = "DEMO"
                                   },
                               //effects output
                               new ibbatch
                                   {
                                       batchnum = 5,
                                       batchname = "effects",
                                       errflag = false,
                                       holdrun = "R",
                                       outputdest = BroadcastCriteria.EffectsOutputDest,
                                       nextrun = new DateTime(2016, 6, 16, 10, 45, 00),
                                       agency = "DEMO"
                                   },
                               //offline 
                               new ibbatch
                                   {
                                       batchnum = 6,
                                       batchname = BroadcastCriteria.OfflineRecord + "offline bcst",
                                       errflag = false,
                                       holdrun = "R",
                                       outputdest = "1",
                                       nextrun = new DateTime(2016, 6, 16, 10, 45, 00),
                                       agency = "DEMO"
                                   },
                               //travet offline
                               new ibbatch
                                   {
                                       batchnum = 7,
                                       batchname = BroadcastCriteria.TravetRecord + "travet bcst",
                                       errflag = false,
                                       holdrun = "R",
                                       outputdest = "1",
                                       nextrun = new DateTime(2016, 6, 16, 10, 45, 00),
                                       agency = "DEMO"
                                   }
                           };
        }

        private readonly IList<string> _agencies = new List<string> { "DEMO" };
        [TestMethod]
        public void Valid()
        {
            var mockDb = GetMockDb();

            var query = new GetAllClientStandardBroadcastBatchQuery(mockDb.Object, DateTime.Now, _agencies);
            var actual = query.ExecuteQuery();

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual[0].batchnum);
        }

        [TestMethod]
        public void ValidNotTimeToRunYet()
        {
            var mockDb = GetMockDb(2);

            var query = new GetAllClientStandardBroadcastBatchQuery(mockDb.Object, DateTime.Now, _agencies);
            var actual = query.ExecuteQuery();

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void InError()
        {
            var mockDb = GetMockDb(3);

            var query = new GetAllClientStandardBroadcastBatchQuery(mockDb.Object, DateTime.Now, _agencies);
            var actual = query.ExecuteQuery();

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void OnHold()
        {
            var mockDb = GetMockDb(4);

            var query = new GetAllClientStandardBroadcastBatchQuery(mockDb.Object, DateTime.Now, _agencies);
            var actual = query.ExecuteQuery();

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void EffectsOutput()
        {
            var mockDb = GetMockDb(5);

            var query = new GetAllClientStandardBroadcastBatchQuery(mockDb.Object, DateTime.Now, _agencies);
            var actual = query.ExecuteQuery();

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void Offline()
        {
            var mockDb = GetMockDb(6);

            var query = new GetAllClientStandardBroadcastBatchQuery(mockDb.Object, DateTime.Now, _agencies);
            var actual = query.ExecuteQuery();

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TravetOffline()
        {
            var mockDb = GetMockDb(7);

            var query = new GetAllClientStandardBroadcastBatchQuery(mockDb.Object, DateTime.Now, _agencies);
            var actual = query.ExecuteQuery();

            Assert.AreEqual(0, actual.Count);
        }

        private Mock<IClientQueryable> GetMockDb(int batchnumToRunAgainst = 0)
        {
            var data = InitializeData();
            if (batchnumToRunAgainst > 0)
            {
                data = data.Where(x => x.batchnum == batchnumToRunAgainst).ToList();
            }
            
            var mockSet = new Mock<IQueryable<ibbatch>>();
            mockSet.SetupIQueryable(data.AsQueryable());

            var mockDb = new Mock<IClientQueryable>();
            mockDb.Setup(x => x.iBBatch).Returns(mockSet.Object);

            return mockDb;
        }
    }
}
