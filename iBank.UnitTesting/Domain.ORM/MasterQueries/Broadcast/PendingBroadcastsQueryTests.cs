using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Domain.Constants;
using Domain.Helper;
using Domain.Orm.iBankMastersQueries.BroadcastQueries;
using Domain.Services;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iBank.UnitTesting.Domain.ORM.MasterQueries.Broadcast
{
    [TestClass]
    public class PendingBroadcastsQueryTests
    {
        private IMasterDataStore _store;
        private ICacheService _cache;

        [TestInitialize]
        public void Init()
        {
            var db = new Mock<IMastersQueryable>();
            db.Setup(x => x.BcstQue4).Returns(GetQueueData().AsQueryable());
            db.Setup(x => x.BroadcastStageAgencies).Returns(GetLoggingAgencies().AsQueryable());
            db.Setup(x => x.BroadcastLongRunningThreshold).Returns(GetLongRunningThresholds().AsQueryable());

            var store = new Mock<IMasterDataStore>();
            store.Setup(x => x.MastersQueryDb).Returns(db.Object);
            _store = store.Object;

            var mock = new Mock<ICacheService>();
            _cache = mock.Object;
        }

        private List<BroadcastLongRunningThreshold> GetLongRunningThresholds()
        {
            return new List<BroadcastLongRunningThreshold>
            {
                new BroadcastLongRunningThreshold()
                {
                    Id= 1,
                    Agency = Constants.DefaultFontPlaceholder,
                    MonthsInRangeThreshold = 12
                },
                new BroadcastLongRunningThreshold()
                {
                    Id= 2,
                    Agency = "LONG",
                    MonthsInRangeThreshold = 3
                }
            };
        }

        private List<broadcast_stage_agencies> GetLoggingAgencies()
        {
            return new List<broadcast_stage_agencies>
            {
                //general logging agency
                new broadcast_stage_agencies
                {
                    id = 1,
                    agency = "LOG",
                    currently_staged = true,
                    staged_batchnumber = null
                },
                //specific batch logging
                new broadcast_stage_agencies
                {
                    id = 1,
                    agency = "SP_LOG",
                    currently_staged = true,
                    staged_batchnumber = "6"
                },
                //logging turned off
                new broadcast_stage_agencies
                {
                    id = 1,
                    agency = "LOG_OFF",
                    currently_staged = false,
                    staged_batchnumber = null
                },
                //multiple specific broadcasts logged - csv
                new broadcast_stage_agencies
                {
                    id = 1,
                    agency = "MULTI_LOG_CSV",
                    currently_staged = true,
                    staged_batchnumber = "10,11"
                },
                //multiple specific broadcasts logged - pipe sep
                new broadcast_stage_agencies
                {
                    id = 1,
                    agency = "MULTI_LOG_PIPE",
                    currently_staged = true,
                    staged_batchnumber = "12|13"
                },
            };
        }

        private List<bcstque4> GetQueueData()
        {
            return new List<bcstque4>
            {
                //regular
                new bcstque4
                {
                    bcstseqno = 1,
                    agency = "FOO",
                    batchnum = 1,
                    svrstatus = BroadcastCriteria.Pending,
                    outputdest = "1",
                    batchname = "PRIMARY",
                    unilangcode = "",
                    nxtdstart = new DateTime(2015, 1, 1),
                    nxtdend = new DateTime(2015, 1, 2),
                    broadcasttype = BroadcastTypes.Primary
                },
                //offline
                new bcstque4
                {
                    bcstseqno = 2,
                    agency = "FOO",
                    batchnum = 2,
                    svrstatus = BroadcastCriteria.Pending,
                    outputdest = "1",
                    batchname = "sysDR:[OFFLINE]",
                    unilangcode = "",
                    nxtdstart = new DateTime(2015, 1, 1),
                    nxtdend = new DateTime(2015, 1, 2),
                    broadcasttype = BroadcastTypes.Offline
                },
                //effects
                new bcstque4
                {
                    bcstseqno = 3,
                    agency = "FOO",
                    batchnum = 3,
                    svrstatus = BroadcastCriteria.Pending,
                    outputdest = "3",
                    batchname = "Effects",
                    unilangcode = "",
                    nxtdstart = new DateTime(2015, 1, 1),
                    nxtdend = new DateTime(2017, 1, 2),
                    broadcasttype = BroadcastTypes.Hot
                },
                //currently running
                new bcstque4
                {
                    bcstseqno = 4,
                    agency = "FOO",
                    batchnum = 4,
                    svrstatus = BroadcastCriteria.Running,
                    outputdest = "1",
                    batchname = "Running",
                    unilangcode = "",
                    nxtdstart = new DateTime(2015, 1, 1),
                    nxtdend = new DateTime(2015, 1, 2),
                    broadcasttype = BroadcastTypes.Primary
                },
                //null batchnumber
                new bcstque4
                {
                    bcstseqno = 5,
                    agency = "FOO",
                    batchnum = null,
                    svrstatus = BroadcastCriteria.Pending,
                    outputdest = "1",
                    batchname = "PRIMARY",
                    unilangcode = "",
                    broadcasttype = BroadcastTypes.Logging
                },
                //agency is logged
                new bcstque4
                {
                    bcstseqno = 6,
                    agency = "LOG",
                    batchnum = 5,
                    svrstatus = BroadcastCriteria.Pending,
                    outputdest = "1",
                    batchname = "LOG",
                    unilangcode = "",
                    nxtdstart = new DateTime(2015, 1, 1),
                    nxtdend = new DateTime(2015, 1, 2),
                    broadcasttype = BroadcastTypes.Logging
                },
                //broadcast is logged
                new bcstque4
                {
                    bcstseqno = 7,
                    agency = "SP_LOG",
                    batchnum = 6,
                    svrstatus = BroadcastCriteria.Pending,
                    outputdest = "1",
                    batchname = "SP_LOG",
                    unilangcode = "",
                    nxtdstart = new DateTime(2015, 1, 1),
                    nxtdend = new DateTime(2015, 1, 2),
                    broadcasttype = BroadcastTypes.Logging
                },
                //agency logged but not turned on
                new bcstque4
                {
                    bcstseqno = 8,
                    agency = "LOG_OFF",
                    batchnum = 7,
                    svrstatus = BroadcastCriteria.Pending,
                    outputdest = "1",
                    batchname = "LOG_OFF",
                    unilangcode = "",
                    nxtdstart = new DateTime(2015, 1, 1),
                    nxtdend = new DateTime(2015, 1, 2),
                    broadcasttype = BroadcastTypes.Primary
                },
                //stage agency
                new bcstque4
                {
                    bcstseqno = 9,
                    agency = "DEMO",
                    batchnum = 8,
                    svrstatus = BroadcastCriteria.Pending,
                    outputdest = "1",
                    batchname = "STAGE",
                    unilangcode = "",
                    nxtdstart = new DateTime(2015, 1, 1),
                    nxtdend = new DateTime(2017, 1, 2),
                    broadcasttype = BroadcastTypes.Stage

                },
                //long running
                new bcstque4
                {
                    bcstseqno = 10,
                    agency = "LONG",
                    batchnum = 9,
                    svrstatus = BroadcastCriteria.Pending,
                    outputdest = "1",
                    batchname = "LONG",
                    unilangcode = "",
                    nxtdstart = new DateTime(2015, 1, 1),
                    nxtdend = new DateTime(2015, 9, 1),
                    broadcasttype = BroadcastTypes.LongRunning
                },
                //multi broadcast is logged (1/2) - CSV
                new bcstque4
                {
                    bcstseqno = 11,
                    agency = "MULTI_LOG_CSV",
                    batchnum = 10,
                    svrstatus = BroadcastCriteria.Pending,
                    outputdest = "1",
                    batchname = "MULTI_LOG_CSV_1",
                    unilangcode = "",
                    nxtdstart = new DateTime(2015, 1, 1),
                    nxtdend = new DateTime(2015, 1, 2),
                    broadcasttype = BroadcastTypes.Logging
                },
                //multi broadcast is logged (2/2) - CSV
                new bcstque4
                {
                    bcstseqno = 12,
                    agency = "MULTI_LOG_CSV",
                    batchnum = 11,
                    svrstatus = BroadcastCriteria.Pending,
                    outputdest = "1",
                    batchname = "MULTI_LOG_CSV_2",
                    unilangcode = "",
                    nxtdstart = new DateTime(2015, 1, 1),
                    nxtdend = new DateTime(2015, 1, 2),
                    broadcasttype = BroadcastTypes.Logging
                },
                //multi broadcast is logged (1/2) - PIPE
                new bcstque4
                {
                    bcstseqno = 13,
                    agency = "MULTI_LOG_PIPE",
                    batchnum = 12,
                    svrstatus = BroadcastCriteria.Pending,
                    outputdest = "1",
                    batchname = "MULTI_LOG_PIPE_1",
                    unilangcode = "",
                    nxtdstart = new DateTime(2015, 1, 1),
                    nxtdend = new DateTime(2015, 1, 2),
                    broadcasttype = BroadcastTypes.Logging
                },
                //multi broadcast is logged (2/2) - PIPE
                new bcstque4
                {
                    bcstseqno = 14,
                    agency = "MULTI_LOG_PIPE",
                    batchnum = 13,
                    svrstatus = BroadcastCriteria.Pending,
                    outputdest = "1",
                    batchname = "MULTI_LOG_PIPE_2",
                    unilangcode = "",
                    nxtdstart = new DateTime(2015, 1, 1),
                    nxtdend = new DateTime(2015, 1, 2),
                    broadcasttype = BroadcastTypes.Logging
                },
                //agency is logged + broadcast qualifies as long running
                new bcstque4
                {
                    bcstseqno = 15,
                    agency = "LOG",
                    batchnum = 14,
                    svrstatus = BroadcastCriteria.Pending,
                    outputdest = "1",
                    batchname = "LOG",
                    unilangcode = "",
                    nxtdstart = new DateTime(2015, 1, 1),
                    nxtdend = new DateTime(2017, 1, 2),
                    broadcasttype =  BroadcastTypes.Logging
                },
            };
        }

        [TestMethod]
        public void PrimaryServer_IgnoreEffects_IgnoreNonPending_IgnoreNoBatchNum_IgnoreStage_IgnoreLogging_IgnoreLongRunning_ReturnRegular_ReturnOffline()
        {
            var sut = new GetPendingBroadcastQuery(_store, "", BroadcastServerFunction.Primary, _cache);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(3, output.Count);
            Assert.AreEqual(true, output.Any(x => x.batchnum.Value == 1));
            Assert.AreEqual(true, output.Any(x => x.batchnum.Value == 2));
            Assert.AreEqual(true, output.Any(x => x.batchnum.Value == 7));
        }

        [TestMethod]
        public void OfflineServer_IgnoreEffects_IgnoreNonPending_IgnoreNoBatchNum_IgnoreStage_IgnoreLogging_IgnoreLongRunning_IgnoreRegular_ReturnOffline()
        {

            var sut = new GetPendingBroadcastQuery(_store, "", BroadcastServerFunction.Offline, _cache);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual(true, output.Any(x => x.batchnum.Value == 2));
        }

        [TestMethod]
        public void HotServer_IgnoreNonPending_IgnoreNoBatchNum_IgnoreStage_IgnoreLogging_IgnoreLongRunning_IgnoreRegular_IgnoreOffline_ReturnEffects()
        {
            var sut = new GetPendingBroadcastQuery(_store, "", BroadcastServerFunction.Hot, _cache);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual(true, output.Any(x => x.batchnum.Value == 3));
        }

        [TestMethod]
        public void StageServer_IgnoreNonPending_IgnoreNoBatchNum_ReturnMatchingAgency()
        {
            var sut = new GetPendingBroadcastQuery(_store, "", BroadcastServerFunction.Stage, _cache);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual(true, output.Any(x => x.batchnum.Value == 8));
        }

        [TestMethod]
        public void LoggingServer_IgnoreNonPending_IgnoreNoBatchNum_IgnoreStage_IgnoreLoggingTurnedOff_ReturnGeneralLoggingAgency_ReturnSpecificBroadcasts_ReturnLongRunningThatMatch()
        {
            var sut = new GetPendingBroadcastQuery(_store, "", BroadcastServerFunction.Logging, _cache);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(7, output.Count);
            Assert.AreEqual(true, output.Any(x => x.batchnum.Value == 5));
            Assert.AreEqual(true, output.Any(x => x.batchnum.Value == 6));
            Assert.AreEqual(true, output.Any(x => x.batchnum.Value == 10));
            Assert.AreEqual(true, output.Any(x => x.batchnum.Value == 11));
            Assert.AreEqual(true, output.Any(x => x.batchnum.Value == 12));
            Assert.AreEqual(true, output.Any(x => x.batchnum.Value == 13));
            Assert.AreEqual(true, output.Any(x => x.batchnum.Value == 14));
        }

        [TestMethod]
        public void LongRunningServer_IgnoreNonPending_IgnoreNoBatchNum_IgnoreStage_IgnoreLogging_ReturnBroadcastsOverThreshold()
        {
            var sut = new GetPendingBroadcastQuery(_store, "", BroadcastServerFunction.LongRunning, _cache);

            var output = sut.ExecuteQuery();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual(true, output.Any(x => x.batchnum.Value == 9));
        }
                
    }
}
