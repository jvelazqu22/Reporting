using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Orm.iBankClientQueries.BroadcastQueries;

using iBank.Entities.ClientEntities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.Domain.ORM.ClientQueries.Broadcast
{
    [TestClass]
    public class GetAllClientHotBatchQueryTests
    {
        private IList<ibbatch> Data { get; set; }
        private DateTime CycleTimeZone { get; set; }
        private readonly IList<string> _agencies = new List<string> { "DEMO" };

        [TestInitialize]
        public void InitializeData()
        {
            Data = new List<ibbatch>
                       {
                           new ibbatch
                               {
                                   batchnum = 0,
                                   errflag = false,
                                   outputdest = "3",
                                   runspcl = true,
                                   nextrun = DateTime.Now,
                                   holdrun = "R",
                                   batchname = "run_special_batch",
                                       agency = "DEMO"
                               },
                           new ibbatch
                               {
                                   batchnum = 1,
                                   errflag = false,
                                   outputdest = "3",
                                   runspcl = true,
                                   nextrun = new DateTime(2000, 01, 01),
                                   holdrun = "R",
                                   batchname = "next_run_prior_to_cycle_time_zone",
                                       agency = "DEMO"
                               },
                           new ibbatch
                               {
                                   batchnum = 2,
                                   errflag = false,
                                   outputdest = "3",
                                   runspcl = true,
                                   nextrun = new DateTime(2017, 01, 01),
                                   holdrun = "H",
                                   batchname = "next_run_prior_to_cycle_time_zone_on_hold",
                                       agency = "DEMO"
                               },
                           new ibbatch
                               {
                                   batchnum = 3,
                                   errflag = false,
                                   outputdest = "3",
                                   runspcl = true,
                                   nextrun = DateTime.Now,
                                   holdrun = "R",
                                   batchname = "sysdr:[hot_offline",
                                       agency = "DEMO"
                               },
                           new ibbatch
                               {
                                   batchnum = 4,
                                   errflag = true,
                                   outputdest = "3",
                                   runspcl = false,
                                   nextrun = DateTime.Now,
                                   holdrun = "H",
                                   batchname = "in_error",
                                       agency = "DEMO"
                               },
                           new ibbatch
                               {
                                   batchnum = 5,
                                   errflag = false,
                                   outputdest = "1",
                                   runspcl = false,
                                   nextrun = DateTime.Now,
                                   holdrun = "H",
                                   batchname = "wrong_output_dest",
                                       agency = "DEMO"
                               }
                       };
            CycleTimeZone = DateTime.Now;
        }

        [TestMethod]
        public void GetRunSpecialHotRecordFromGroup()
        {
            var mockData = Data.Where(x => x.batchnum == 0);
            var mockDb = MockBroadcastBatchRecords.GetMockDbOfQueue(mockData.ToList());

            var query = new GetAllClientHotBatchQuery(mockDb.Object, CycleTimeZone, _agencies);
            var actual = query.ExecuteQuery();

            Assert.AreEqual("run_special_batch", actual[0].batchname);
        }

        [TestMethod]
        public void GetNextRunPriorToCycleTimeZone()
        {
            var mockData = Data.Where(x => x.batchnum == 1);
            var mockDb = MockBroadcastBatchRecords.GetMockDbOfQueue(mockData.ToList());

            var query = new GetAllClientHotBatchQuery(mockDb.Object, CycleTimeZone, _agencies);
            var actual = query.ExecuteQuery();

            Assert.AreEqual("next_run_prior_to_cycle_time_zone", actual[0].batchname);
        }

        [TestMethod]
        public void GetNextRunAfterCycleTimeZoneOnHold()
        {
            var mockData = Data.Where(x => x.batchnum == 2);
            var mockDb = MockBroadcastBatchRecords.GetMockDbOfQueue(mockData.ToList());

            var query = new GetAllClientHotBatchQuery(mockDb.Object, CycleTimeZone, _agencies);
            var actual = query.ExecuteQuery();

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void GetHotOffline()
        {
            var mockData = Data.Where(x => x.batchnum == 3);
            var mockDb = MockBroadcastBatchRecords.GetMockDbOfQueue(mockData.ToList());

            var query = new GetAllClientHotBatchQuery(mockDb.Object, CycleTimeZone, _agencies);
            var actual = query.ExecuteQuery();

            Assert.AreEqual("sysdr:[hot_offline", actual[0].batchname);
        }

        [TestMethod]
        public void GetInError()
        {
            var mockData = Data.Where(x => x.batchnum == 4);
            var mockDb = MockBroadcastBatchRecords.GetMockDbOfQueue(mockData.ToList());

            var query = new GetAllClientHotBatchQuery(mockDb.Object, CycleTimeZone, _agencies);
            var actual = query.ExecuteQuery();

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void GetWrongOutputDestination()
        {
            var mockData = Data.Where(x => x.batchnum == 5);
            var mockDb = MockBroadcastBatchRecords.GetMockDbOfQueue(mockData.ToList());

            var query = new GetAllClientHotBatchQuery(mockDb.Object, CycleTimeZone, _agencies);
            var actual = query.ExecuteQuery();

            Assert.AreEqual(0, actual.Count);
        }
    }
}
