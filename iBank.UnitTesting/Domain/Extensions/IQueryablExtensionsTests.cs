using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Constants;
using Domain.Extensions;
using Domain.Helper;
using iBank.Entities.MasterEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.Domain.Extensions
{
    [TestClass]
    public class IQueryablExtensionsTests
    {
        [TestMethod]
        public void AreFinishedBroadcasts_ReturnNonPendingAndNonRunningBroadcasts()
        {
            var queue = new List<bcstque4>
            {
                //running
                new bcstque4
                {
                    bcstseqno = 1,
                    svrstatus = BroadcastCriteria.Running
                },
                //pending
                new bcstque4
                {
                    bcstseqno = 2,
                    svrstatus = BroadcastCriteria.Pending
                },
                //done
                new bcstque4
                {
                    bcstseqno = 3,
                    svrstatus = BroadcastCriteria.Done
                }
            }.AsQueryable();

            var output = queue.AreFinishedBroadcasts();

            Assert.AreEqual(1, output.Count());
            Assert.AreEqual(true, output.Any(x => x.bcstseqno == 3));
        }

        [TestMethod]
        public void AreErroredBroadcasts_ReturnBroadcastsInError()
        {
            var queue = new List<bcstque4>
            {
                new bcstque4
                {
                    bcstseqno = 1,
                    errflag = true
                },
                //control
                new bcstque4
                {
                    bcstseqno = 2,
                    errflag = false
                }
            }.AsQueryable();

            var output = queue.AreErroredBroadcasts();

            Assert.AreEqual(1, output.Count());
            Assert.AreEqual(true, output.Any(x => x.bcstseqno == 1));
        }

        [TestMethod]
        public void AreRunningBroadcasts_ReturnRunningBroadcasts()
        {
            var queue = new List<bcstque4>
            {
                new bcstque4
                {
                    bcstseqno = 1,
                    svrstatus = BroadcastCriteria.Running
                },
                //control
                new bcstque4
                {
                    bcstseqno = 2,
                    svrstatus = BroadcastCriteria.Pending
                }
            }.AsQueryable();

            var output = queue.AreRunningBroadcasts();

            Assert.AreEqual(1, output.Count());
            Assert.AreEqual(true, output.Any(x => x.bcstseqno == 1));
        }

        [TestMethod]
        public void ArePendingBroadcasts_ReturnPendingBroadcasts()
        {
            var queue = new List<bcstque4>
            {
                new bcstque4
                {
                    bcstseqno = 1,
                    svrstatus = BroadcastCriteria.Pending
                },
                //control
                new bcstque4
                {
                    bcstseqno = 2,
                    svrstatus = BroadcastCriteria.Running
                }
            }.AsQueryable();

            var output = queue.ArePendingBroadcasts();

            Assert.AreEqual(1, output.Count());
            Assert.AreEqual(true, output.Any(x => x.bcstseqno == 1));
        }

        [TestMethod]
        public void HaveRunningTimeOverThreshold_ReturnRecordsWhereStartTimeIsPriorToThreshold_ExclusiveComparison()
        {
            var threshold = new DateTime(2016, 1, 1, 3, 0, 0);
            var queue = new List<bcstque4>
            {
                //prior to
                new bcstque4
                {
                    bcstseqno = 1,
                    starttime = threshold.AddHours(-1)
                },
                //same as
                new bcstque4
                {
                    bcstseqno = 2,
                    starttime = threshold
                },
                //after
                new bcstque4
                {
                    bcstseqno = 3,
                    starttime = threshold.AddHours(1)
                }
            }.AsQueryable();

            var output = queue.HaveRunningTimeOverThreshold(threshold);

            Assert.AreEqual(1, output.Count());
            Assert.AreEqual(true, output.Any(x => x.bcstseqno == 1));
        }

        [TestMethod]
        public void AreOnServer_ReturnWhereServerNumberMatches()
        {
            var serverNumber = 1;
            var queue = new List<bcstque4>
            {
                new bcstque4
                {
                    bcstseqno = 1,
                    svrnumber = serverNumber
                },
                //control
                new bcstque4
                {
                    bcstseqno = 2,
                    svrnumber = serverNumber + 1
                }
            }.AsQueryable();

            var output = queue.AreOnServer(serverNumber);

            Assert.AreEqual(1, output.Count());
            Assert.AreEqual(true, output.Any(x => x.bcstseqno == 1));
        }

        [TestMethod]
        public void AreEffectsBroadcast_ReturnWhereOutputDestinationMatchesEffectsOutputDestination()
        {
            var queue = new List<bcstque4>
            {
                new bcstque4
                {
                    bcstseqno = 1,
                    outputdest = BroadcastCriteria.EffectsOutputDest
                },
                //control
                new bcstque4
                {
                    bcstseqno = 2,
                    outputdest = BroadcastCriteria.EffectsOutputDest + "1"
                }
            }.AsQueryable();

            var output = queue.AreEffectsBroadcasts();

            Assert.AreEqual(1, output.Count());
            Assert.AreEqual(true, output.Any(x => x.bcstseqno == 1));
        }
        
        [TestMethod]
        public void AreNotEffectsBroadcasts_ReturnWhereOutputDestinationDoesNotEqualEffectsOutputDestination()
        {
            var queue = new List<bcstque4>
            {
                new bcstque4
                {
                    bcstseqno = 1,
                    outputdest = BroadcastCriteria.EffectsOutputDest + "1"
                },
                //control
                new bcstque4
                {
                    bcstseqno = 2,
                    outputdest = BroadcastCriteria.EffectsOutputDest
                }
            }.AsQueryable();

            var output = queue.AreNotEffectsBroadcasts();

            Assert.AreEqual(1, output.Count());
            Assert.AreEqual(true, output.Any(x => x.bcstseqno == 1));
        }

        [TestMethod]
        public void HaveBatchNumber_ReturnWhereBatchnumberIsNotNull()
        {
            var queue = new List<bcstque4>
            {
                new bcstque4
                {
                    bcstseqno = 1,
                    batchnum = 1
                },
                //control
                new bcstque4
                {
                    bcstseqno = 2,
                    batchnum = null
                }
            }.AsQueryable();

            var output = queue.HaveBatchNumber();

            Assert.AreEqual(1, output.Count());
            Assert.AreEqual(true, output.Any(x => x.bcstseqno == 1));
        }

        [TestMethod]
        public void AreStageAgencyBroadcasts_ReturnBroadcastsBelongingToTheDesignatedStageAgency()
        {
            var queue = new List<bcstque4>
            {
                new bcstque4
                {
                    bcstseqno = 1,
                    agency = Broadcasts.StageAgency
                },
                //control
                new bcstque4
                {
                    bcstseqno = 2,
                    agency = Broadcasts.StageAgency + "1"
                }
            }.AsQueryable();

            var output = queue.AreStageAgencyBroadcasts(Broadcasts.StageAgency);

            Assert.AreEqual(1, output.Count());
            Assert.AreEqual(true, output.Any(x => x.bcstseqno == 1));
        }

        [TestMethod]
        public void AreNotLoggingAgencyBroadcasts_IgnoreBroadcastsWhereAgencyIsTurnedOnAndHasNoSpecificBroadcastNumber()
        {
            var logAgency = "log";
            var logTurnedOff = "notLog";
            var specificLogging = "specificLog";

            var loggingAgencies = new List<broadcast_stage_agencies>
            {
               new broadcast_stage_agencies
               {
                   agency = logAgency,
                   currently_staged = true,
                   staged_batchnumber = null
               },
                new broadcast_stage_agencies
                {
                    agency = logTurnedOff,
                    currently_staged = false,
                    staged_batchnumber = null
                },
                new broadcast_stage_agencies
                {
                    agency = specificLogging,
                    currently_staged = true,
                    staged_batchnumber = "1"
                }
            }.AsQueryable();

            var queue = new List<bcstque4>
            {
                new bcstque4
                {
                    bcstseqno = 1,
                    agency = logAgency
                },
                new bcstque4
                {
                    bcstseqno = 2,
                    agency = logTurnedOff
                },
                new bcstque4
                {
                    bcstseqno = 3,
                    agency = specificLogging
                }
            }.AsQueryable();

            var output = queue.AreNotLoggingAgencyBroadcasts(loggingAgencies);

            Assert.AreEqual(2, output.Count());
            Assert.AreEqual(false, output.Any(x => x.bcstseqno == 1));
            Assert.AreEqual(true, output.Any(x => x.bcstseqno == 2));
            Assert.AreEqual(true, output.Any(x => x.bcstseqno == 3));
        }
        [TestMethod]
        public void AreNotLoggingAgencyBroadcasts_ReturnBroadcastsWhereAgencyIsTurnedOnAndHasNoSpecificBroadcastNumber()
        {
            var logAgency = "log";
            var logTurnedOff = "notLog";
            var specificLogging = "specificLog";

            var loggingAgencies = new List<broadcast_stage_agencies>
            {
                new broadcast_stage_agencies
                {
                    agency = logAgency,
                    currently_staged = true,
                    staged_batchnumber = null
                },
                new broadcast_stage_agencies
                {
                    agency = logTurnedOff,
                    currently_staged = false,
                    staged_batchnumber = null
                },
                new broadcast_stage_agencies
                {
                    agency = specificLogging,
                    currently_staged = true,
                    staged_batchnumber = "1"
                }
            }.AsQueryable();

            var queue = new List<bcstque4>
            {
                new bcstque4
                {
                    bcstseqno = 1,
                    agency = logAgency
                },
                new bcstque4
                {
                    bcstseqno = 2,
                    agency = logTurnedOff
                },
                new bcstque4
                {
                    bcstseqno = 3,
                    agency = specificLogging
                }
            }.AsQueryable();

            var output = queue.AreLoggingAgencyBroadcasts(loggingAgencies);

            Assert.AreEqual(1, output.Count());
            Assert.AreEqual(true, output.Any(x => x.bcstseqno == 1));
            Assert.AreEqual(false, output.Any(x => x.bcstseqno == 2));
            Assert.AreEqual(false, output.Any(x => x.bcstseqno == 3));
        }

        [TestMethod]
        public void AreOfflineBroadcasts_ReturnOfflineRecords()
        {
            var queue = new List<bcstque4>
            {
                new bcstque4
                {
                    bcstseqno = 1,
                    batchname = $"{BroadcastCriteria.OfflineRecord}offline]"
                },
                //control
                new bcstque4
                {
                    bcstseqno = 2,
                    batchname = "foo"
                }
            }.AsQueryable();

            var output = queue.AreOfflineBroadcasts();

            Assert.AreEqual(1, output.Count());
            Assert.AreEqual(true, output.Any(x => x.bcstseqno == 1));
        }
    }
}
