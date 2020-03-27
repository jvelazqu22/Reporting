using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Constants;
using Domain.Extensions;
using Domain.Helper;
using Domain.Orm.Classes;
using iBank.Entities.MasterEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.Domain.Extensions
{
    [TestClass]
    public class IEnumerableExtensionsTests
    {
        [TestMethod]
        public void GetCorrespondingServerNumbers_MatchServersWithFunction()
        {
            var configs = new List<BroadcastServerConfiguration>
            {
                new BroadcastServerConfiguration
                {
                    ServerNumber = 1,
                    Function = BroadcastServerFunction.Primary
                },
                new BroadcastServerConfiguration
                {
                    ServerNumber = 2,
                    Function = BroadcastServerFunction.Primary
                },
                new BroadcastServerConfiguration
                {
                    ServerNumber = 3,
                    Function = BroadcastServerFunction.Hot
                }
            };

            var output = configs.GetCorrespondingServerNumbers(BroadcastServerFunction.Primary).ToList();

            Assert.AreEqual(2, output.Count());
            Assert.AreEqual(true, output.Any(x => x == 1));
            Assert.AreEqual(true, output.Any(x => x == 2));
        }
        

        [TestMethod]
        public void AreNotSpecificBatchNumberLoggingBroadcasts_ReturnBroadcastsWithAgencyLevelLoggingOrNoLogging()
        {
            var generalLog = "log";
            var logTurnedOff = "notLog";
            var csvLog = "csvLog";
            var pipeLog = "pipeLog";
            var singleLog = "singleLog";
            var notLogged = "foo";

            var loggingAgencies = new List<broadcast_stage_agencies>
            {
                new broadcast_stage_agencies
                {
                    agency = generalLog,
                    currently_staged = true,
                    staged_batchnumber = null
                },
                new broadcast_stage_agencies
                {
                    agency = logTurnedOff,
                    currently_staged = false,
                    staged_batchnumber = null
                },
                //comma separated
                new broadcast_stage_agencies
                {
                    agency = csvLog,
                    currently_staged = true,
                    staged_batchnumber = "1,2"
                },
                //pipe separated
                new broadcast_stage_agencies
                {
                    agency = pipeLog,
                    currently_staged = true,
                    staged_batchnumber = "1|2"
                },
                //one batch logged
                new broadcast_stage_agencies
                {
                    agency = singleLog,
                    currently_staged = true,
                    staged_batchnumber = "1"
                },
            };

            var queue = new List<bcstque4>
            {
                new bcstque4
                {
                    bcstseqno = 1,
                    agency = generalLog,
                    batchnum = 1
                },
                new bcstque4
                {
                    bcstseqno = 2,
                    agency = logTurnedOff,
                    batchnum = 1
                },
                new bcstque4
                {
                    bcstseqno = 3,
                    agency = csvLog,
                    batchnum = 1
                },
                new bcstque4
                {
                    bcstseqno = 4,
                    agency = csvLog,
                    batchnum = 2
                },
                new bcstque4
                {
                    bcstseqno = 5,
                    agency = pipeLog,
                    batchnum = 1
                },
                new bcstque4
                {
                    bcstseqno = 6,
                    agency = pipeLog,
                    batchnum = 2
                },
                new bcstque4
                {
                    bcstseqno = 7,
                    agency = singleLog,
                    batchnum = 1
                },
                new bcstque4
                {
                    bcstseqno = 8,
                    agency = notLogged,
                    batchnum = 1
                }
            };

            var output = queue.AreNotSpecificBatchNumberLoggingBroadcasts(loggingAgencies).ToList();

            Assert.AreEqual(3, output.Count());
            Assert.AreEqual(true, output.Any(x => x.agency.Equals(generalLog)));
            Assert.AreEqual(true, output.Any(x => x.agency.Equals(logTurnedOff)));
            Assert.AreEqual(true, output.Any(x => x.agency.Equals(notLogged)));
        }

        [TestMethod]
        public void AreSpecificBatchNumberLoggingBroadcasts_ReturnBroadcastsWhereThatBroadcastIsBeingLogged()
        {
            var generalLog = "log";
            var logTurnedOff = "notLog";
            var csvLog = "csvLog";
            var pipeLog = "pipeLog";
            var singleLog = "singleLog";
            var notLogged = "foo";

            var loggingAgencies = new List<broadcast_stage_agencies>
            {
                new broadcast_stage_agencies
                {
                    agency = generalLog,
                    currently_staged = true,
                    staged_batchnumber = null
                },
                new broadcast_stage_agencies
                {
                    agency = logTurnedOff,
                    currently_staged = false,
                    staged_batchnumber = null
                },
                //comma separated
                new broadcast_stage_agencies
                {
                    agency = csvLog,
                    currently_staged = true,
                    staged_batchnumber = "1,2"
                },
                //pipe separated
                new broadcast_stage_agencies
                {
                    agency = pipeLog,
                    currently_staged = true,
                    staged_batchnumber = "1|2"
                },
                //one batch logged
                new broadcast_stage_agencies
                {
                    agency = singleLog,
                    currently_staged = true,
                    staged_batchnumber = "1"
                },
            };

            var queue = new List<bcstque4>
            {
                new bcstque4
                {
                    bcstseqno = 1,
                    agency = generalLog,
                    batchnum = 1
                },
                new bcstque4
                {
                    bcstseqno = 2,
                    agency = logTurnedOff,
                    batchnum = 1
                },
                new bcstque4
                {
                    bcstseqno = 3,
                    agency = csvLog,
                    batchnum = 1
                },
                new bcstque4
                {
                    bcstseqno = 4,
                    agency = csvLog,
                    batchnum = 2
                },
                new bcstque4
                {
                    bcstseqno = 5,
                    agency = pipeLog,
                    batchnum = 1
                },
                new bcstque4
                {
                    bcstseqno = 6,
                    agency = pipeLog,
                    batchnum = 2
                },
                new bcstque4
                {
                    bcstseqno = 7,
                    agency = singleLog,
                    batchnum = 1
                },
                new bcstque4
                {
                    bcstseqno = 8,
                    agency = notLogged,
                    batchnum = 1
                }
            };

            var output = queue.AreSpecificBatchNumberLoggingBroadcasts(loggingAgencies).ToList();

            Assert.AreEqual(5, output.Count());
            Assert.AreEqual(2, output.Count(x => x.agency.Equals(pipeLog)));
            Assert.AreEqual(2, output.Count(x => x.agency.Equals(csvLog)));
            Assert.AreEqual(1, output.Count(x => x.agency.Equals(singleLog)));
        }

        [TestMethod]
        public void AreNotLongRunningBroadcasts_ReturnNonLongRunningBroadcasts()
        {
            var longRunning = new List<bcstque4>
            {
                new bcstque4
                {
                    bcstseqno = 1,
                    batchnum = 1,
                    agency = "foo"
                }
            };

            var queue = new List<bcstque4>
            {
                new bcstque4
                {
                    bcstseqno = 1,
                    batchnum = 1,
                    agency = "foo"
                },
                new bcstque4
                {
                    bcstseqno = 2,
                    batchnum = 2,
                    agency = "foo"
                },
                new bcstque4
                {
                    bcstseqno = 3,
                    batchnum = 3,
                    agency = "bar"
                }
            };

            var output = queue.AreNotLongRunningBroadcasts(longRunning).ToList();

            Assert.AreEqual(2, output.Count());
            Assert.AreEqual(true, output.Any(x => x.bcstseqno == 2));
            Assert.AreEqual(true, output.Any(x => x.bcstseqno == 3));
        }

        [TestMethod]
        public void AreStageAgencyBroadcasts_ReturnBroadcastsForTheDesignatedStageAgency()
        {
            var queue = new List<bcstque4>
            {
                new bcstque4
                {
                    bcstseqno = 1,
                    batchnum = 1,
                    agency = Broadcasts.StageAgency
                },
                new bcstque4
                {
                    bcstseqno = 1,
                    batchnum = 2,
                    agency = "foo"
                }
            };

            var output = queue.AreStageAgencyBroadcasts(Broadcasts.StageAgency);

            Assert.AreEqual(1, output.Count());
            Assert.AreEqual(true, output.Any(x => x.bcstseqno == 1));
        }
    }
}
