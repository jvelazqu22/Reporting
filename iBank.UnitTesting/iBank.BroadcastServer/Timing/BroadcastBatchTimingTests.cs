using iBank.BroadcastServer.Timing;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;

namespace iBank.UnitTesting.iBank.BroadcastServer.Timing
{
    [TestClass]
    public class BroadcastBatchTimingTests
    {
        [TestMethod]
        public void MapToQueueRecord()
        {
            var expectedQueueRecord = new bcstque4
            {
                prevhist = 1,
                weekmonth = 2,
                monthrun = 3,
                weekrun = 4, 
                weekstart = 5,
                monthstart = 6,
                nxtdstart = new DateTime(2000, 3, 1),
                nxtdend = new DateTime(2000, 3, 2),
                lastdstart = new DateTime(2000, 2, 1),
                lastdend = new DateTime(2000, 2, 2),
                runnewdata = 1,
                reportdays = 1,
                nextrun = new DateTime(1999, 1, 1),
                lastrun = new DateTime(1998, 1, 1)
            };

            var batchTiming = new RecordTimingDetails(expectedQueueRecord);
            
            var newQueueRecord = batchTiming.MapToQueueRecord(new bcstque4());

            Assert.AreEqual(expectedQueueRecord.prevhist, newQueueRecord.prevhist);
            Assert.AreEqual(expectedQueueRecord.weekmonth, newQueueRecord.weekmonth);
            Assert.AreEqual(expectedQueueRecord.monthrun, newQueueRecord.monthrun);
            Assert.AreEqual(expectedQueueRecord.weekrun, newQueueRecord.weekrun);
            Assert.AreEqual(expectedQueueRecord.weekstart, newQueueRecord.weekstart);
            Assert.AreEqual(expectedQueueRecord.monthstart, newQueueRecord.monthstart);
            Assert.AreEqual(expectedQueueRecord.nxtdstart, newQueueRecord.nxtdstart);
            Assert.AreEqual(expectedQueueRecord.nxtdend, newQueueRecord.nxtdend);
            Assert.AreEqual(expectedQueueRecord.lastdstart, newQueueRecord.lastdstart);
            Assert.AreEqual(expectedQueueRecord.lastdend, newQueueRecord.lastdend);
            Assert.AreEqual(expectedQueueRecord.runnewdata, newQueueRecord.runnewdata);
            Assert.AreEqual(expectedQueueRecord.reportdays, newQueueRecord.reportdays);
            Assert.AreEqual(expectedQueueRecord.nextrun, newQueueRecord.nextrun);
            Assert.AreEqual(expectedQueueRecord.lastrun, newQueueRecord.lastrun);

        }

        [TestMethod]
        public void MapToBatchRecord_NotRunSpecial()
        {
            var expectedBatchRecord = new ibbatch
            {
                prevhist = 1,
                weekmonth = 2,
                monthrun = 3,
                weekrun = 4,
                weekstart = 5,
                monthstart = 6,
                nxtdstart = new DateTime(2000, 3, 1),
                nxtdend = new DateTime(2000, 3, 2),
                lastdstart = new DateTime(2000, 2, 1),
                lastdend = new DateTime(2000, 2, 2),
                RunNewData = 1,
                reportdays = 1,
                nextrun = new DateTime(1999, 1, 1),
                lastrun = new DateTime(1998, 1, 1)
            };

            var batchTiming = new RecordTimingDetails(expectedBatchRecord);

            var newBatchRecord = batchTiming.MapToBatchRecord(new ibbatch(), false);

            Assert.AreEqual(expectedBatchRecord.prevhist, newBatchRecord.prevhist);
            Assert.AreEqual(expectedBatchRecord.weekmonth, newBatchRecord.weekmonth);
            Assert.AreEqual(expectedBatchRecord.monthrun, newBatchRecord.monthrun);
            Assert.AreEqual(expectedBatchRecord.weekrun, newBatchRecord.weekrun);
            Assert.AreEqual(expectedBatchRecord.weekstart, newBatchRecord.weekstart);
            Assert.AreEqual(expectedBatchRecord.monthstart, newBatchRecord.monthstart);
            Assert.AreEqual(expectedBatchRecord.nxtdstart, newBatchRecord.nxtdstart);
            Assert.AreEqual(expectedBatchRecord.nxtdend, newBatchRecord.nxtdend);
            Assert.AreEqual(expectedBatchRecord.lastdstart, newBatchRecord.lastdstart);
            Assert.AreEqual(expectedBatchRecord.lastdend, newBatchRecord.lastdend);
            Assert.AreEqual(expectedBatchRecord.RunNewData, newBatchRecord.RunNewData);
            Assert.AreEqual(expectedBatchRecord.reportdays, newBatchRecord.reportdays);
            Assert.AreEqual(expectedBatchRecord.nextrun, newBatchRecord.nextrun);
            Assert.AreEqual(expectedBatchRecord.lastrun, newBatchRecord.lastrun);

        }
    }
}
