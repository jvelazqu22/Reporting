using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Extensions;
using iBank.BroadcastServer.QueueManager.BuildQueue;
using iBank.Entities.MasterEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.Domain.Extensions
{
    [TestClass]
    public class BroadcastTypeExtensionsTests
    {

        private string generalLog = "log";
        private string logTurnedOff = "notLog";
        private string csvLog = "csvLog";
        private string pipeLog = "pipeLog";
        private string singleLog = "singleLog";
        private string demoLog = "demo";
        private string effects = "3";
        private string noEffects = "2";

        private List<broadcast_stage_agencies> loggingAgencies;
        private Dictionary<string, int> longRunningThresholds = new Dictionary<string, int>();
        private int defaultThreshold = 12;

        private void init()
        {
            loggingAgencies = new List<broadcast_stage_agencies>
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
                new broadcast_stage_agencies
                {
                    agency = demoLog,
                    currently_staged = false,
                    staged_batchnumber = null
                },
            };

        }

        [TestMethod]
        public void GetBroadcastType_pipeLog_ReturnLogging()
        {
            init();
            var exp = BroadcastTypes.Logging;

            var bcst = new bcstque4
            {
                bcstseqno = 1,
                agency = pipeLog,
                batchnum = 1,
                svrstatus = "PENDING",
                outputdest = effects
            };

            var output = BroadcastTypeIdentifier.GetBroadcastType(bcst, loggingAgencies, longRunningThresholds, defaultThreshold);

            Assert.AreEqual(exp, output);
        }

        [TestMethod]
        public void GetBroadcastType_csvLog_ReturnLogging()
        {
            init();
            var exp = BroadcastTypes.Logging;

            var bcst = new bcstque4
            {
                bcstseqno = 1,
                agency = csvLog,
                batchnum = 1,
                svrstatus = "PENDING",
                outputdest = effects
            };

            var output = BroadcastTypeIdentifier.GetBroadcastType(bcst, loggingAgencies, longRunningThresholds, defaultThreshold); ;

            Assert.AreEqual(exp, output);
        }


        [TestMethod]
        public void GetBroadcastType_singleLog_ReturnLogging()
        {
            init();
            var exp = BroadcastTypes.Logging;

            var bcst = new bcstque4
            {
                bcstseqno = 1,
                agency = singleLog,
                batchnum = 1,
                svrstatus = "PENDING",
                outputdest = effects
            };

            var output = BroadcastTypeIdentifier.GetBroadcastType(bcst, loggingAgencies, longRunningThresholds, defaultThreshold); ;

            Assert.AreEqual(exp, output);
        }

        [TestMethod]
        public void GetBroadcastType_generalLog_ReturnLogging()
        {
            init();
            var exp = BroadcastTypes.Logging;

            var bcst = new bcstque4
            {
                bcstseqno = 1,
                agency = generalLog,
                batchnum = 3,
                svrstatus = "PENDING",
                outputdest = effects
            };

            var output = BroadcastTypeIdentifier.GetBroadcastType(bcst, loggingAgencies, longRunningThresholds, defaultThreshold); ;

            Assert.AreEqual(exp, output);
        }

        [TestMethod]
        public void GetBroadcastType_logTurnedOffHasEffects_ReturnHot()
        {
            init();
            var exp = BroadcastTypes.Hot;

            var bcst = new bcstque4
            {
                bcstseqno = 1,
                agency = logTurnedOff,
                batchnum = 3,
                svrstatus = "PENDING",
                outputdest = effects
            };

            var output = BroadcastTypeIdentifier.GetBroadcastType(bcst, loggingAgencies, longRunningThresholds, defaultThreshold); ;

            Assert.AreEqual(exp, output);
        }


        [TestMethod]
        public void GetBroadcastType_logTurnedOffNoEffects_ReturnPrimary()
        {
            init();
            var exp = BroadcastTypes.Primary;

            var bcst = new bcstque4
            {
                bcstseqno = 1,
                agency = logTurnedOff,
                batchnum = 3,
                svrstatus = "PENDING",
                outputdest = noEffects,
                batchname = "testing"
            };

            var output = BroadcastTypeIdentifier.GetBroadcastType(bcst, loggingAgencies, longRunningThresholds, defaultThreshold); ;

            Assert.AreEqual(exp, output);
        }

        [TestMethod]
        public void GetBroadcastType_demoLog_ReturnStage()
        {
            init();
            var exp = BroadcastTypes.Stage;

            var bcst = new bcstque4
            {
                bcstseqno = 1,
                agency = demoLog,
                batchnum = 3,
                svrstatus = "PENDING",
                outputdest = noEffects
            };

            var output = BroadcastTypeIdentifier.GetBroadcastType(bcst, loggingAgencies, longRunningThresholds, defaultThreshold); ;

            Assert.AreEqual(exp, output);
        }


        [TestMethod]
        public void GetBroadcastType_logTurnedOffSYSDRName_ReturnOffline()
        {
            init();
            var exp = BroadcastTypes.Offline;

            var bcst = new bcstque4
            {
                bcstseqno = 1,
                agency = logTurnedOff,
                batchnum = 3,
                svrstatus = "PENDING",
                outputdest = noEffects,
                batchname = "sysDR:[18263]"
            };

            var output = BroadcastTypeIdentifier.GetBroadcastType(bcst, loggingAgencies, longRunningThresholds, defaultThreshold); ;

            Assert.AreEqual(exp, output);
        }


        [TestMethod]
        public void GetBroadcastType_logTurnedOffSYSDRNameHasEffect_ReturnHot()
        {
            init();
            var exp = BroadcastTypes.Hot;

            var bcst = new bcstque4
            {
                bcstseqno = 1,
                agency = logTurnedOff,
                batchnum = 3,
                svrstatus = "PENDING",
                outputdest = effects,
                batchname = "sysDR:[18263]"
            };

            var output = BroadcastTypeIdentifier.GetBroadcastType(bcst, loggingAgencies, longRunningThresholds, defaultThreshold); ;

            Assert.AreEqual(exp, output);
        }
    }
}
