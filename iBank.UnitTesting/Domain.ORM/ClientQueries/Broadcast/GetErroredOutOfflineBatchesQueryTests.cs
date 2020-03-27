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
    public class GetErroredOutOfflineBatchesQueryTests
    {
        private IList<ibbatch> _data;
        [TestInitialize]
        public void Init()
        {
            _data = new List<ibbatch>
            {
                new ibbatch
                    {
                        batchname = BroadcastCriteria.OfflineRecord + "foo][DONE]",
                        lastrun = new DateTime(2016, 8, 1),
                        batchnum = 1
                    },
                new ibbatch
                    {
                        batchname = BroadcastCriteria.OfflineRecord + "foo][ERROR]",
                        lastrun = new DateTime(2016, 8, 1),
                        batchnum = 2
                    },
                new ibbatch
                    {
                        batchname = BroadcastCriteria.OfflineRecord + "foo][ERROR]",
                        lastrun = new DateTime(2016, 10, 1),
                        batchnum = 3
                    },
            };
        }

        [TestMethod]
        public void ExecuteQuery()
        {
            var mockSet = new Mock<IQueryable<ibbatch>>();
            mockSet.SetupIQueryable(_data.AsQueryable());

            var mockDb = new Mock<IClientQueryable>();
            mockDb.Setup(x => x.iBBatch).Returns(mockSet.Object);

            var threshold = new DateTime(2016, 9, 1);
            var query = new GetErroredOutOfflineBatchesQuery(mockDb.Object, threshold);
            var output = query.ExecuteQuery();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual(2, output[0].batchnum);
        }
    }
}
