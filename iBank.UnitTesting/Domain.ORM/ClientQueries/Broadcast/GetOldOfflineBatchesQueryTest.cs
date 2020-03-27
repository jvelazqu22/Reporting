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
    public class GetOldOfflineBatchesQueryTest
    {
        private List<ibbatch> Data { get; set; }
        
        public void InitializeData()
        {
            var data = new List<ibbatch>();
            //should be found when looking for old
            data.Add(new ibbatch
                         {
                             batchname = BroadcastCriteria.OfflineRecord + "foo[DONE]",
                             lastrun = new DateTime(2016, 01, 01),
                             batchnum = 1
                         });
            //should not be found - not old
            data.Add(new ibbatch
            {
                batchname = BroadcastCriteria.OfflineRecord + "foo[DONE]",
                lastrun = new DateTime(2100, 01, 01),
                batchnum = 2
            });
            //should not be found - not done or pending
            data.Add(new ibbatch
                         {
                             batchname = BroadcastCriteria.OfflineRecord + "foo[RUN]",
                             lastrun = new DateTime(2016, 01, 01),
                             batchnum = 3
                         });

            Data = data;
        }

        [TestMethod]
        public void GetOldBatches_OneDoneRec()
        {
            InitializeData();

            var mockSet = new Mock<IQueryable<ibbatch>>();
            mockSet.SetupIQueryable(Data.AsQueryable());

            var mockDb = new Mock<IClientQueryable>();
            mockDb.Setup(x => x.iBBatch).Returns(mockSet.Object);

            var query = new GetOldOfflineBatchesQuery(mockDb.Object, DateTime.Now);
            var actual = query.ExecuteQuery();

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual[0].batchnum);
        }

        [TestMethod]
        public void GetOldBatches_NullLastRun()
        {
            var mockData = new List<ibbatch>
                               {
                                   new ibbatch
                                       {
                                            batchname = BroadcastCriteria.OfflineRecord + "foo[RUN]",
                                            lastrun = null
                                       }
                               };

            var mockSet = new Mock<IQueryable<ibbatch>>();
            mockSet.SetupIQueryable(mockData.AsQueryable());

            var mockDb = new Mock<IClientQueryable>();
            mockDb.Setup(x => x.iBBatch).Returns(mockSet.Object);

            var query = new GetOldOfflineBatchesQuery(mockDb.Object, DateTime.Now);
            var actual = query.ExecuteQuery();

            Assert.AreEqual(0, actual.Count);
        }
    }
}
