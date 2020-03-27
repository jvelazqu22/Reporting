using Domain.Helper;
using Domain.Interfaces.BroadcastServer;
using iBank.BroadcastServer.BroadcastBatch;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;

namespace iBank.UnitTesting.iBank.BroadcastServer.BroadcastQueue
{
    [TestClass]
    public class BroadcastValueManagerTests
    {
        [TestMethod]
        public void ChangeBatchToRunning()
        {
            var broadcast = new bcstque4 { svrstatus = BroadcastCriteria.Pending, svrnumber = 0, starttime = new DateTime(1900, 1, 1) };
            var now = new DateTime(2016, 1, 1, 01, 02, 03);
            var newServerNumber = 1;
            var sut = new BroadcastRecordUpdatesManager();

            sut.ChangeBatchToRunning(broadcast, newServerNumber, now);

            Assert.AreEqual(BroadcastCriteria.Running, broadcast.svrstatus);
            Assert.AreEqual(newServerNumber, broadcast.svrnumber);
            Assert.AreEqual(now, broadcast.starttime);
        }

        [TestMethod]
        public void UpdateOfflineBatch_BatchIsOkay()
        {
            var broadcast = new ibbatch { lastrun = new DateTime(1900, 1, 1), errflag = true, batchname = "sysDR:[foo][RUN]" };
            var now = new DateTime(2016, 1, 1, 1, 2, 3);
            var batchOk = true;
            var sut = new BroadcastRecordUpdatesManager();

            sut.UpdateOfflineBatch(broadcast, batchOk, now);

            Assert.AreEqual("sysDR:[foo][DONE]", broadcast.batchname);
            Assert.AreEqual(now, broadcast.lastrun);
            Assert.AreEqual(false, broadcast.errflag);
        }

        [TestMethod]
        public void UpdateOfflineBatch_BatchIsNotOkay()
        {
            var broadcast = new ibbatch { lastrun = new DateTime(1900, 1, 1), errflag = true, batchname = "sysDR:[foo][RUN]" };
            var now = new DateTime(2016, 1, 1, 1, 2, 3);
            var batchOk = false;
            var sut = new BroadcastRecordUpdatesManager();

            sut.UpdateOfflineBatch(broadcast, batchOk, now);

            Assert.AreEqual("sysDR:[foo][ERROR]", broadcast.batchname);
            Assert.AreEqual(now, broadcast.lastrun);
            Assert.AreEqual(true, broadcast.errflag);
        }

        [TestMethod]
        public void UpdateNonOfflineBatch_BatchOkay()
        {
            var broadcast = new ibbatch
                                {
                                    lastrun = new DateTime(1900, 1, 1),
                                    errflag = true,
                                    lastdend = new DateTime(1900, 1, 1),
                                    lastdstart = new DateTime(1900, 1, 1)
                                };
            var batchOk = true;
            var now = new DateTime(2016, 2, 2, 1, 2, 3);
            var nextReportPeriodStart = new DateTime(2016, 1, 1);
            var nextReportPeriodEnd = new DateTime(2016, 2, 2);
            var lastReportPeriodStart = new DateTime(2015, 1, 1);
            var lastReportPeriodEnd = new DateTime(2015, 2, 2);
            var timing = new Mock<IRecordTimingDetails>();
            timing.Setup(x => x.NextReportPeriodStart).Returns(nextReportPeriodStart);
            timing.Setup(x => x.NextReportPeriodEnd).Returns(nextReportPeriodEnd);
            timing.Setup(x => x.LastReportPeriodStart).Returns(lastReportPeriodStart);
            timing.Setup(x => x.LastReportPeriodEnd).Returns(lastReportPeriodEnd);
            var sut = new BroadcastRecordUpdatesManager();

            sut.UpdateNonOfflineBatch(broadcast, batchOk, timing.Object, now);

            Assert.AreEqual(now, broadcast.lastrun);
            Assert.AreEqual(false, broadcast.errflag);
            Assert.AreEqual(nextReportPeriodEnd, broadcast.lastdend);
            Assert.AreEqual(nextReportPeriodStart, broadcast.lastdstart);
        }

        [TestMethod]
        public void UpdateNonOfflineBatch_BatchNotOkay()
        {
            var broadcast = new ibbatch
            {
                lastrun = new DateTime(1900, 1, 1),
                errflag = true,
                lastdend = new DateTime(1900, 1, 1),
                lastdstart = new DateTime(1900, 1, 1)
            };
            var batchOk = false;
            var now = new DateTime(2016, 2, 2, 1, 2, 3);
            var nextReportPeriodStart = new DateTime(2016, 1, 1);
            var nextReportPeriodEnd = new DateTime(2016, 2, 2);
            var lastReportPeriodStart = new DateTime(2015, 1, 1);
            var lastReportPeriodEnd = new DateTime(2015, 2, 2);
            var timing = new Mock<IRecordTimingDetails>();
            timing.Setup(x => x.NextReportPeriodStart).Returns(nextReportPeriodStart);
            timing.Setup(x => x.NextReportPeriodEnd).Returns(nextReportPeriodEnd);
            timing.Setup(x => x.LastReportPeriodStart).Returns(lastReportPeriodStart);
            timing.Setup(x => x.LastReportPeriodEnd).Returns(lastReportPeriodEnd);
            var sut = new BroadcastRecordUpdatesManager();

            sut.UpdateNonOfflineBatch(broadcast, batchOk, timing.Object, now);

            Assert.AreEqual(now, broadcast.lastrun);
            Assert.AreEqual(true, broadcast.errflag);
            Assert.AreEqual(lastReportPeriodEnd, broadcast.lastdend);
            Assert.AreEqual(lastReportPeriodStart, broadcast.lastdstart);
        }
    }
}
