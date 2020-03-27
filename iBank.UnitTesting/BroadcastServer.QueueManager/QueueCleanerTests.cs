using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Helper;
using Domain.Orm.Classes;
using Domain.Services;
using iBank.BroadcastServer.QueueManager.Cleaner;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace iBank.UnitTesting.BroadcastServer.QueueManager
{
    [TestClass]
    public class QueueCleanerTests
    {
        //not long running
        private static readonly DateTime _thresholdToExpire = new DateTime(2016, 1, 1, 12, 0, 0);
        private static readonly DateTime _expiredStartTime = new DateTime(2016, 1, 1, 10, 0, 0);
        private static readonly DateTime _notExpiredStartTime = new DateTime(2016, 1, 1, 13, 0, 0); 
                   
        //long running
        private static readonly DateTime _longRunThresholdToExpire = new DateTime(2016, 1, 1, 9, 0, 0);
        private static readonly DateTime _longNotExpiredStartTime = new DateTime(2016, 1, 1, 10, 0, 0);
        private static readonly DateTime _longExpiredStartTime = new DateTime(2016, 1, 1, 8, 0, 0);

        private static readonly DateTime _startDate = new DateTime(2015, 1, 1);
        private static readonly DateTime _endDate = new DateTime(2015, 1, 2);
        private static readonly DateTime _longEndDate = _endDate.AddMonths(14);
        
        [TestMethod]
        public void AllTypesOnQueue_RemoveOrphaned_RemoveMinimallyExpiredHotLoggingStage_RemoveMaximallyExpiredLongRunning_RetainMinimallyExpiredLongRunning_RemoveMinimallyExpiredPrimaryAndOffline()
        {
            #region Testing Explanation
            /*
              Testing Explanation
                - our queue is composed of regular, offline, hot, stage, and logging broadcasts
                - we have a minimal expiration and a maximal expiration 
                - any pending broadcasts should be retained
                - any done or errored broadcasts should be removed
                - any minimally expired hot, stage, or logging broadcasts should be removed
                - any minimally expired, but not maximally expired, long running broadcasts should be retained
                - any maximally expired long running broadcasts should be removed
                - any non-long running regular or offline broadcasts that are minimally expired should be removed
            */
            #endregion

            var queue = GetDoneBroadcasts();
            queue = queue.Concat(GetPendingBroadcasts());
            queue = queue.Concat(GetErroredBroadcasts());
            queue = queue.Concat(GetStageBroadcasts());
            queue = queue.Concat(GetHotBroadcasts());
            queue = queue.Concat(GetLoggingBroadcasts());
            queue = queue.Concat(GetLongRunningBroadcasts());
            queue = queue.Concat(GetRegularBroadcasts());
            queue = queue.Concat(GetOfflineBroadcasts());

            var mocks = GetRepoMocks(queue);
            var sut = new QueueCleaner(mocks.Store.Object);

            sut.RemoveRecords(_thresholdToExpire, _longRunThresholdToExpire, GetServerConfigs());

            mocks.CmdDb.Verify(x => x.RemoveLetClientWin(
                It.Is<List<bcstque4>>(
                    recsToRemove => recsToRemove.Count == 10
                                    && recsToRemove.Count(rec => rec.batchnum.Value == 100) == 1
                                    && recsToRemove.Count(rec => rec.batchnum.Value == 200) == 1
                                    && recsToRemove.Count(rec => rec.batchnum.Value == 300) == 1
                                    && recsToRemove.Count(rec => rec.batchnum.Value == 302) == 1
                                    && recsToRemove.Count(rec => rec.batchnum.Value == 400) == 1
                                    && recsToRemove.Count(rec => rec.batchnum.Value == 500) == 1
                                    && recsToRemove.Count(rec => rec.batchnum.Value == 600) == 1
                                    && recsToRemove.Count(rec => rec.batchnum.Value == 700) == 1
                                    && recsToRemove.Count(rec => rec.batchnum.Value == 800) == 1
                                    && recsToRemove.Count(rec => rec.batchnum.Value == 801) == 1
                )));
        }

        private IEnumerable<bcstque4> GetPendingBroadcasts()
        {
            return new List<bcstque4>
            {
                new bcstque4
                {
                    agency = "foo",
                    batchname = "pending",
                    batchnum = 1,
                    outputdest = "1",
                    nxtdstart = _startDate,
                    nxtdend = _endDate,
                    starttime = null,
                    errflag = false,
                    svrstatus = BroadcastCriteria.Pending,
                    svrnumber = 20
                }
            };
        }

        private IEnumerable<bcstque4> GetRegularBroadcasts()
        {
            return new List<bcstque4>
            {
                new bcstque4
                {
                    agency = "foo",
                    batchname = "regular_expired",
                    batchnum = 100,
                    outputdest = "1",
                    nxtdstart = _startDate,
                    nxtdend = _endDate,
                    starttime = _expiredStartTime,
                    errflag = false,
                    svrstatus = BroadcastCriteria.Running,
                    svrnumber = 20
                },
                new bcstque4
                {
                    agency = "foo",
                    batchname = "regular_control",
                    batchnum = 101,
                    outputdest = "1",
                    nxtdstart = _startDate,
                    nxtdend = _endDate,
                    starttime = _notExpiredStartTime,
                    errflag = false,
                    svrstatus = BroadcastCriteria.Running,
                    svrnumber = 20
                }
            };
        }

        private IEnumerable<bcstque4> GetOfflineBroadcasts()
        {
            return new List<bcstque4>
            {
                new bcstque4
                {
                    agency = "foo",
                    batchname = "sysDR:[offline_expired]",
                    batchnum = 200,
                    outputdest = "1",
                    nxtdstart = _startDate,
                    nxtdend = _endDate,
                    starttime = _expiredStartTime,
                    svrstatus = BroadcastCriteria.Running,
                    svrnumber = 25
                },
                new bcstque4
                {
                    agency = "foo",
                    batchname = "sysDR:[offline_control]",
                    batchnum = 201,
                    outputdest = "1",
                    nxtdstart = _startDate,
                    nxtdend = _endDate,
                    starttime = _notExpiredStartTime,
                    svrstatus = BroadcastCriteria.Running,
                    svrnumber = 25
                }
            };
        }

        private IEnumerable<bcstque4> GetLongRunningBroadcasts()
        {
            return new List<bcstque4>
            {
                new bcstque4
                {
                    agency = "LONG",
                    batchname = "long_regular_expired",
                    batchnum = 300,
                    outputdest = "1",
                    nxtdstart = _startDate,
                    nxtdend = _longEndDate,
                    starttime = _longExpiredStartTime,
                    errflag = false,
                    svrstatus = BroadcastCriteria.Running,
                    svrnumber = 30
                },
                new bcstque4
                {
                    agency = "LONG",
                    batchname = "long_regular_control",
                    batchnum = 301,
                    outputdest = "1",
                    nxtdstart = _startDate,
                    nxtdend = _longEndDate,
                    starttime = _longNotExpiredStartTime,
                    errflag = false,
                    svrstatus = BroadcastCriteria.Running,
                    svrnumber = 30
                },
                new bcstque4
                {
                    agency = "foo",
                    batchname = "sysDR:[offline_long_expired]",
                    batchnum = 302,
                    outputdest = "1",
                    nxtdstart = _startDate,
                    nxtdend = _longEndDate,
                    starttime = _longExpiredStartTime,
                    svrstatus = BroadcastCriteria.Running,
                    svrnumber = 30
                },
                new bcstque4
                {
                    agency = "foo",
                    batchname = "sysDR:[offline_long_control]",
                    batchnum = 303,
                    outputdest = "1",
                    nxtdstart = _startDate,
                    nxtdend = _longEndDate,
                    starttime = _longNotExpiredStartTime,
                    svrstatus = BroadcastCriteria.Running,
                    svrnumber = 30
                },
            };
        }

        private IEnumerable<bcstque4> GetErroredBroadcasts()
        {
            return new List<bcstque4>
            {
                new bcstque4
                {
                    agency = "foo",
                    batchname = "errored",
                    batchnum = 400,
                    outputdest = "1",
                    nxtdstart = _startDate,
                    nxtdend = _endDate,
                    starttime = _notExpiredStartTime,
                    errflag = true,
                    svrstatus = BroadcastCriteria.Pending,
                    svrnumber = 20
                }
            };
        }

        private IEnumerable<bcstque4> GetDoneBroadcasts()
        {
            return new List<bcstque4>
            {
                new bcstque4
                {
                    agency = "foo",
                    batchname = "done",
                    batchnum = 500,
                    outputdest = "1",
                    nxtdstart = _startDate,
                    nxtdend = _endDate,
                    starttime = _notExpiredStartTime,
                    errflag = false,
                    svrstatus = BroadcastCriteria.Done,
                    svrnumber = 20
                }
            };
        }

        private IEnumerable<bcstque4> GetStageBroadcasts()
        {
            return new List<bcstque4>
            {
                new bcstque4
                {
                    agency = Broadcasts.StageAgency,
                    batchname = "stage",
                    batchnum = 600,
                    outputdest = "1",
                    nxtdstart = _startDate,
                    nxtdend = _endDate,
                    starttime = _expiredStartTime,
                    errflag = false,
                    svrstatus = BroadcastCriteria.Running,
                    svrnumber = 200
                },
                new bcstque4
                {
                    agency = Broadcasts.StageAgency,
                    batchname = "stage_control",
                    batchnum = 601,
                    outputdest = "1",
                    nxtdstart = _startDate,
                    nxtdend = _endDate,
                    starttime = _notExpiredStartTime,
                    errflag = false,
                    svrstatus = BroadcastCriteria.Running,
                    svrnumber = 200
                }
            };
        }

        private IEnumerable<bcstque4> GetHotBroadcasts()
        {
            return new List<bcstque4>
            {
                new bcstque4
                {
                    agency = "foo",
                    batchname = "hot",
                    batchnum = 700,
                    outputdest = "3",
                    nxtdstart = _startDate,
                    nxtdend = _endDate,
                    starttime = _expiredStartTime,
                    errflag = false,
                    svrstatus = BroadcastCriteria.Running,
                    svrnumber = 26
                },
                new bcstque4
                {
                    agency = "foo",
                    batchname = "hot_control",
                    batchnum = 701,
                    outputdest = "3",
                    nxtdstart = _startDate,
                    nxtdend = _endDate,
                    starttime = _notExpiredStartTime,
                    errflag = false,
                    svrstatus = BroadcastCriteria.Running,
                    svrnumber = 26
                }
            };
        }

        private IEnumerable<bcstque4> GetLoggingBroadcasts()
        {
            return new List<bcstque4>
            {
                //general agency logging
                new bcstque4
                {
                    agency = "LOG",
                    batchname = "general_log",
                    batchnum = 800,
                    outputdest = "1",
                    nxtdstart = _startDate,
                    nxtdend = _endDate,
                    starttime = _expiredStartTime,
                    errflag = false,
                    svrstatus = BroadcastCriteria.Running,
                    svrnumber = 201
                },
                //specific broadcast logging
                new bcstque4
                {
                    agency = "SP_LOG",
                    batchname = "specific_bcst_log",
                    batchnum = 801,
                    outputdest = "1",
                    nxtdstart = _startDate,
                    nxtdend = _endDate,
                    starttime = _expiredStartTime,
                    errflag = false,
                    svrstatus = BroadcastCriteria.Running,
                    svrnumber = 201
                },
                //control
                new bcstque4
                {
                    agency = "LOG",
                    batchname = "log_control",
                    batchnum = 802,
                    outputdest = "1",
                    nxtdstart = _startDate,
                    nxtdend = _endDate,
                    starttime = _notExpiredStartTime,
                    errflag = false,
                    svrstatus = BroadcastCriteria.Running,
                    svrnumber = 201
                }
            };
        }

        private IList<BroadcastServerConfiguration> GetServerConfigs()
        {
            return new List<BroadcastServerConfiguration>
            {
                new BroadcastServerConfiguration
                {
                    ServerNumber = 20,
                    Function = BroadcastServerFunction.Primary
                },
                new BroadcastServerConfiguration
                {
                    ServerNumber = 25,
                    Function = BroadcastServerFunction.Offline
                },
                new BroadcastServerConfiguration
                {
                    ServerNumber = 26,
                    Function = BroadcastServerFunction.Hot
                },
                new BroadcastServerConfiguration
                {
                    ServerNumber = 30,
                    Function = BroadcastServerFunction.LongRunning
                },
                new BroadcastServerConfiguration
                {
                    ServerNumber = 200,
                    Function = BroadcastServerFunction.Stage
                },
                new BroadcastServerConfiguration
                {
                    ServerNumber = 201,
                    Function = BroadcastServerFunction.Offline
                }
            };

        }

        private Mocks GetRepoMocks(IEnumerable<bcstque4> broadcasts)
        {
            var db = new Mock<IMastersQueryable>();
            db.Setup(x => x.BroadcastLongRunningThreshold).Returns(GetLongRunningThreshold);
            db.Setup(x => x.BroadcastStageAgencies).Returns(GetLoggingAgencies);
            db.Setup(x => x.BcstQue4).Returns(broadcasts.AsQueryable);

            var cmdDb = new Mock<ICommandDb>();

            var store = new Mock<IMasterDataStore>();
            store.Setup(x => x.MastersQueryDb).Returns(db.Object);
            store.Setup(x => x.MastersCommandDb).Returns(cmdDb.Object);

            var mocks = new Mocks(store, cmdDb);
            return mocks;
        }

        private struct Mocks
        {
            public Mocks(Mock<IMasterDataStore> store, Mock<ICommandDb> cmd)
            {
                CmdDb = cmd;
                Store = store;
            }

            public Mock<ICommandDb> CmdDb { get; set; }

            public Mock<IMasterDataStore> Store { get; set; }
        }

        private IQueryable<BroadcastLongRunningThreshold> GetLongRunningThreshold()
        {
            return new List<BroadcastLongRunningThreshold>
            {
                new BroadcastLongRunningThreshold()
                {
                    Id = 1,
                    Agency = "[DEFAULT]",
                    MonthsInRangeThreshold = 6
                },
                new BroadcastLongRunningThreshold()
                {
                    Id = 1,
                    Agency = "LONG",
                    MonthsInRangeThreshold = 6
                }
            }.AsQueryable();
        }

        private IQueryable<broadcast_stage_agencies> GetLoggingAgencies()
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
                    staged_batchnumber = "501"
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
            }.AsQueryable();
        }
    }
}
