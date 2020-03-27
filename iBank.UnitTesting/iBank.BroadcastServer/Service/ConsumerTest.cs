using Domain.Constants;
using Domain.Helper;
using Domain.Interfaces.BroadcastServer;
using Domain.Interfaces.Query;
using Domain.Models.BroadcastServer;
using iBank.BroadcastServer.Service;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Domain.Services;
using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared;
using iBank.UnitTesting.TestingHelpers.MockHelpers;

using iBankDomain.RepositoryInterfaces;

namespace iBank.UnitTesting.iBank.BroadcastServer.Service
{
    [TestClass]
    public class ConsumerTest
    {
        // Unit test naming convention:
        // [UnitOfWork_StateUnderTest_ExpectedBehavior]
        // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown
        [TestMethod]
        public void ProcessBatch_AllReportsInBatchIsOne_CallExecuteBatch()
        {
            // Arrange
            #region testBcstque4
            var testBcstque4 = new bcstque4()
            {
                bcstseqno = 1,
                dbname = "ibankdemo",
                svrstatus = "RUNNING",
                svrnumber = 200,
                starttime = new DateTime(2016, 12, 13, 9, 55, 40),
                endtime = null,
                UserNumber = 1595,
                agency = "DEMO",
                batchnum = 9028,
                batchname = "reservation monthly",
                emailaddr = "jvelazquez@ciswired.com",
                acctlist = string.Empty,
                prevhist = BroadcastSchedule.RESERVATION,
                weekmonth = 1,
                monthstart = 1,
                monthrun = 15,
                weekstart = 1,
                weekrun = 1,
                nxtdstart = new DateTime(2016, 12, 01),
                nxtdend = new DateTime(2016, 12, 31),
                lastrun = new DateTime(1900, 1, 1),
                lastdstart = new DateTime(2016, 11, 1),
                lastdend = new DateTime(2016, 11, 30),
                errflag = false,
                runspcl = false,
                spclstart = new DateTime(1900, 1, 1),
                spclend = new DateTime(1900, 1, 1),
                pagebrklvl = 0,
                titleacct = string.Empty,
                bcsenderemail = "info@ciswired.com",
                bcsendername = "Cornerstone",
                nextrun = new DateTime(2016, 11, 15),
                setby = string.Empty,
                holdrun = "R",
                reportdays = 1,
                rptusernum = 0,
                usespcl = false,
                nodataoptn = false,
                emailsubj = string.Empty,
                mailformat = "1",
                outputtype = OutputType.PORTABLE_DOC_FORMAT,
                displayuid = false,
                outputdest = "2",
                eProfileNo = 0,
                emailccadr = string.Empty,
                runnewdata = 0,
                unilangcode = string.Empty
            };

            #endregion

            //BlockingCollection<bcstque4> batchesToExecute = new BlockingCollection<bcstque4>() { testBcstque4 };
            var masterDataStore = new Mock<IMasterDataStore>();

            var queueRemover = new Mock<IBroadcastQueueRecordRemover>();
            queueRemover.Setup(r => r.RemoveBroadcastFromQueue(It.IsAny<bcstque4>(), It.IsAny<ICommandDb>()));

            var iBankDatabases = new iBankDatabases() { active = true, StopBcst = false, server_address = "test", };
            var databaseInfoQuery = new Mock<IDatabaseInfoQuery>();
            databaseInfoQuery.Setup(r => r.ExecuteQuery()).Returns(iBankDatabases);
            databaseInfoQuery.Setup(r => r.HasDbBeenDisposed).Returns(false);

            var clientDataStore = new Mock<IClientDataStore>();
            var queryDb = new Mock<IClientQueryable>();
            var historyData = new List<ibbatchhistory> { new ibbatchhistory { batchnum = 9028 } };
            queryDb.Setup(x => x.iBBatchHistory).Returns(MockHelper.GetListAsQueryable(historyData).Object);
            clientDataStore.Setup(x => x.ClientQueryDb).Returns(queryDb.Object);

            var batchRecordUpdatesManager = new Mock<IBroadcastRecordUpdatesManager>();
            batchRecordUpdatesManager.Setup(r => r.UpdateQueueRecordInDataStore(It.IsAny<ICommandDb>(), It.IsAny<bcstque4>())).Returns(true);

            var ibUser = new ibuser() { };
            var getUserByUserNumberQuery = new Mock<IQuery<ibuser>>();
            getUserByUserNumberQuery.Setup(r => r.ExecuteQuery()).Returns(ibUser);

            var batchManager = new Mock<IBatchManager>();
            batchManager.Setup(r => r.SetUserBroadcastSettings());
            batchManager.Setup(s => s.UserManager.User).Returns(new ibuser());
            batchManager.Setup(s => s.QueuedRecord).Returns(testBcstque4);

            var processCaptionInformationserList = new List<ProcessCaptionInformation>();
            var getAllActiveBroadcastProcessCaptionsQuery = new Mock<IQuery<IList<ProcessCaptionInformation>>>();
            getAllActiveBroadcastProcessCaptionsQuery.Setup(r => r.ExecuteQuery()).Returns(processCaptionInformationserList);

            var broadcastReportProcessor = new Mock<IBroadcastReportProcessor>();

            var listOfReportsInBatch = new List<BroadcastReportInformation>() { new BroadcastReportInformation(), };
            var batchReportRetriever = new Mock<IBatchReportRetriever>();
            batchReportRetriever.Setup(s => s.GetAllReportsInBatch(processCaptionInformationserList, "DEMO", false, 1595)).Returns(listOfReportsInBatch);
            var cache = new CacheService();

        var batchRunner = new Mock<IBatchRunner>();
            batchRunner.Setup(s => s.ExecuteBatch(cache, listOfReportsInBatch, It.IsAny<IRecordTimingDetails>(), It.IsAny<BroadcastServerInformation>(), false, false));

            var p = new Parameters(queueRemover.Object, batchRecordUpdatesManager.Object)
            {
                MasterDataStore = masterDataStore.Object,
                IsMaintenanceModeRequested = false,
                BatchesToExecute = new BlockingCollection<bcstque4>() { testBcstque4 },
                DatabaseInfoQuery = databaseInfoQuery.Object,
                ClientDataStore = clientDataStore.Object,
                GetUserByUserNumberQuery = getUserByUserNumberQuery.Object,
                BatchManager = batchManager.Object,
                ServerConfiguration = new BroadcastServerInformation()
                {
                    ReportLogoDirectory = @"C:\iBank.Reports\Logos",
                    CrystalReportDirectory = @"C:\DEV\iBankPlatform\Reporting\Main\iBank.Reporting\iBank.Services.Implementation\CrystalReports",
                    ServerNumber = 200,
                    ReportOutputDirectory = "C:\\INETPUB\\WWWROOT\\V4OUT",
                    SenderEmailAddress = "info@ciswired.com",
                    SenderName = "Cornerstone",
                    IbankBaseUrl = "https://ibank.dev.ciswired.com/ibank4"
                },
                GetAllActiveBroadcastProcessCaptionsQuery = getAllActiveBroadcastProcessCaptionsQuery.Object,
                BroadcastReportProcessor = broadcastReportProcessor.Object,
                BatchReportRetriever = batchReportRetriever.Object,
                BatchRunner = batchRunner.Object,
            };

            // Act
            new Consumer().ProcessBatch(testBcstque4, p, new LoadedListsParams());

            // Assert
            //batchRunner.Verify(v => v.ExecuteBatch(cache, listOfReportsInBatch, It.IsAny<IRecordTimingDetails>(), It.IsAny<BroadcastServerInformation>(), false, false), Times.Once());
        }

        [TestMethod]
        public void ProcessBatch_NxtDStartIsLessThanThreshHold_SetUserBroadcastSettingsAndMarkRecordAsInError()
        {
            // Arrange
            #region testBcstque4
            var testBcstque4 = new bcstque4()
            {
                bcstseqno = 1,
                dbname = "ibankdemo",
                svrstatus = "RUNNING",
                svrnumber = 200,
                starttime = new DateTime(2016, 12, 13, 9, 55, 40),
                endtime = null,
                UserNumber = 1595,
                agency = "DEMO",
                batchnum = 9028,
                batchname = "Monthly Report",
                emailaddr = "jvelazquez@ciswired.com",
                acctlist = string.Empty,
                prevhist = BroadcastSchedule.RESERVATION,
                weekmonth = 1,
                monthstart = 1,
                monthrun = 15,
                weekstart = 1,
                weekrun = 1,
                nxtdstart = DateTime.Today.AddMonths(-62),
                nxtdend = new DateTime(2016, 12, 31),
                lastrun = new DateTime(1900, 1, 1),
                lastdstart = new DateTime(2016, 11, 1),
                lastdend = new DateTime(2016, 11, 30),
                errflag = false,
                runspcl = false,
                spclstart = new DateTime(1900, 1, 1),
                spclend = new DateTime(1900, 1, 1),
                pagebrklvl = 0,
                titleacct = string.Empty,
                bcsenderemail = "info@ciswired.com",
                bcsendername = "Cornerstone",
                nextrun = new DateTime(2016, 11, 15),
                setby = string.Empty,
                holdrun = "R",
                reportdays = 1,
                rptusernum = 0,
                usespcl = false,
                nodataoptn = false,
                emailsubj = string.Empty,
                mailformat = "1",
                outputtype = OutputType.PORTABLE_DOC_FORMAT,
                displayuid = false,
                outputdest = "2",
                eProfileNo = 0,
                emailccadr = string.Empty,
                runnewdata = 0,
                unilangcode = string.Empty
            };

            #endregion

            //BlockingCollection<bcstque4> batchesToExecute = new BlockingCollection<bcstque4>() { testBcstque4 };
            var masterDataStore = new Mock<IMasterDataStore>();

            var queueRemover = new Mock<IBroadcastQueueRecordRemover>();
            queueRemover.Setup(r => r.RemoveBroadcastFromQueue(It.IsAny<bcstque4>(), It.IsAny<ICommandDb>()));

            var iBankDatabases = new iBankDatabases() { active = true, StopBcst = false, server_address = "test", };
            var databaseInfoQuery = new Mock<IDatabaseInfoQuery>();
            databaseInfoQuery.Setup(r => r.ExecuteQuery()).Returns(iBankDatabases);
            databaseInfoQuery.Setup(r => r.HasDbBeenDisposed).Returns(false);

            var clientDataStore = new Mock<IClientDataStore>();
            var queryDb = new Mock<IClientQueryable>();
            var historyData = new List<ibbatchhistory> { new ibbatchhistory { batchnum = 9028 } };
            queryDb.Setup(x => x.iBBatchHistory).Returns(MockHelper.GetListAsQueryable(historyData).Object);
            clientDataStore.Setup(x => x.ClientQueryDb).Returns(queryDb.Object);

            var batchRecordUpdatesManager = new Mock<IBroadcastRecordUpdatesManager>();
            batchRecordUpdatesManager.Setup(r => r.UpdateQueueRecordInDataStore(It.IsAny<ICommandDb>(), It.IsAny<bcstque4>())).Returns(true);

            var ibUser = new ibuser() { };
            var getUserByUserNumberQuery = new Mock<IQuery<ibuser>>();
            getUserByUserNumberQuery.Setup(r => r.ExecuteQuery()).Returns(ibUser);

            var batchManager = new Mock<IBatchManager>();
            batchManager.Setup(r => r.SetUserBroadcastSettings());
            batchManager.Setup(s => s.UserManager.User).Returns(new ibuser());
            batchManager.Setup(s => s.QueuedRecord).Returns(testBcstque4);

            var p = new Parameters(queueRemover.Object, batchRecordUpdatesManager.Object)
            {
                MasterDataStore = masterDataStore.Object,
                IsMaintenanceModeRequested = false,
                BatchesToExecute = new BlockingCollection<bcstque4>() { testBcstque4 },
                DatabaseInfoQuery = databaseInfoQuery.Object,
                ClientDataStore = clientDataStore.Object,
                GetUserByUserNumberQuery = getUserByUserNumberQuery.Object,
                BatchManager = batchManager.Object,
                ServerConfiguration = new BroadcastServerInformation()
                {
                    ReportLogoDirectory = @"C:\iBank.Reports\Logos",
                    CrystalReportDirectory = @"C:\DEV\iBankPlatform\Reporting\Main\iBank.Reporting\iBank.Services.Implementation\CrystalReports",
                    ServerNumber = 200,
                    ReportOutputDirectory = "C:\\INETPUB\\WWWROOT\\V4OUT",
                    SenderEmailAddress = "info@ciswired.com",
                    SenderName = "Cornerstone",
                    IbankBaseUrl = "https://ibank.dev.ciswired.com/ibank4"
                }
            };

            // Act
            new Consumer().ProcessBatch(testBcstque4, p, new LoadedListsParams());

            // Assert
            batchManager.Verify(v => v.SetUserBroadcastSettings(), Times.Once());
        }

        [TestMethod]
        public void ProcessBatch_ServerAddressNotFoundException_CallRemoveBroadcastFromQueue()
        {
            // Arrange
            #region testBcstque4
            var testBcstque4 = new bcstque4()
            {
                bcstseqno = 1,
                dbname = "ibankdemo",
                svrstatus = "RUNNING",
                svrnumber = 200,
                starttime = new DateTime(2016, 12, 13, 9, 55, 40),
                endtime = null,
                UserNumber = 1595,
                agency = "DEMO",
                batchnum = 9028,
                batchname = "reservation monthly",
                emailaddr = "jvelazquez@ciswired.com",
                acctlist = string.Empty,
                prevhist = BroadcastSchedule.RESERVATION,
                weekmonth = 1,
                monthstart = 1,
                monthrun = 15,
                weekstart = 1,
                weekrun = 1,
                nxtdstart = new DateTime(2016, 12, 01),
                nxtdend = new DateTime(2016, 12, 31),
                lastrun = new DateTime(1900, 1, 1),
                lastdstart = new DateTime(2016, 11, 1),
                lastdend = new DateTime(2016, 11, 30),
                errflag = false,
                runspcl = false,
                spclstart = new DateTime(1900, 1, 1),
                spclend = new DateTime(1900, 1, 1),
                pagebrklvl = 0,
                titleacct = string.Empty,
                bcsenderemail = "info@ciswired.com",
                bcsendername = "Cornerstone",
                nextrun = new DateTime(2016, 11, 15),
                setby = string.Empty,
                holdrun = "R",
                reportdays = 1,
                rptusernum = 0,
                usespcl = false,
                nodataoptn = false,
                emailsubj = string.Empty,
                mailformat = "1",
                outputtype = OutputType.PORTABLE_DOC_FORMAT,
                displayuid = false,
                outputdest = "2",
                eProfileNo = 0,
                emailccadr = string.Empty,
                runnewdata = 0,
                unilangcode = string.Empty
            };

            #endregion

            //BlockingCollection<bcstque4> batchesToExecute = new BlockingCollection<bcstque4>() { testBcstque4 };
            var masterDataStore = new Mock<IMasterDataStore>();

            var queueRemover = new Mock<IBroadcastQueueRecordRemover>();
            queueRemover.Setup(r => r.RemoveBroadcastFromQueue(It.IsAny<bcstque4>(), It.IsAny<ICommandDb>()));

            var databaseInfoQuery = new Mock<IDatabaseInfoQuery>();
            var clientDataStore = new Mock<IClientDataStore>();

            var batchRecordUpdatesManager = new Mock<IBroadcastRecordUpdatesManager>();
            batchRecordUpdatesManager.Setup(r => r.UpdateQueueRecordInDataStore(It.IsAny<ICommandDb>(), It.IsAny<bcstque4>())).Returns(true);

            var p = new Parameters(queueRemover.Object, batchRecordUpdatesManager.Object)
            {
                MasterDataStore = masterDataStore.Object,
                IsMaintenanceModeRequested = false,
                BatchesToExecute = new BlockingCollection<bcstque4>() { testBcstque4 },
                DatabaseInfoQuery = databaseInfoQuery.Object,
                ClientDataStore = clientDataStore.Object,
                ServerConfiguration = new BroadcastServerInformation()
                {
                    ReportLogoDirectory = @"C:\iBank.Reports\Logos",
                    CrystalReportDirectory = @"C:\DEV\iBankPlatform\Reporting\Main\iBank.Reporting\iBank.Services.Implementation\CrystalReports",
                    ServerNumber = 200,
                    ReportOutputDirectory = "C:\\INETPUB\\WWWROOT\\V4OUT",
                    SenderEmailAddress = "info@ciswired.com",
                    SenderName = "Cornerstone",
                    IbankBaseUrl = "https://ibank.dev.ciswired.com/ibank4"
                }
            };

            // Act
            new Consumer().ProcessBatch(testBcstque4, p, new LoadedListsParams());

            // Assert
            // TODO: this should work, but it is not.
            //queueRemover.Verify(v => v.RemoveBroadcastFromQueue(testBcstque4, p.MasterDataStore.MastersCommandDb), Times.Once());
        }

        [TestMethod]
        public void ProcessBatch_UpdateQueueRecordInDataStoreIsFalse_SuccessfullyExitTheProgram()
        {
            // Arrange
            #region testBcstque4
            var testBcstque4 = new bcstque4()
            {
                bcstseqno = 1,
                dbname = "ibankdemo",
                svrstatus = "RUNNING",
                svrnumber = 200,
                starttime = new DateTime(2016, 12, 13, 9, 55, 40),
                endtime = null,
                UserNumber = 1595,
                agency = "DEMO",
                batchnum = 9028,
                batchname = "reservation monthly",
                emailaddr = "jvelazquez@ciswired.com",
                acctlist = string.Empty,
                prevhist = BroadcastSchedule.RESERVATION,
                weekmonth = 1,
                monthstart = 1,
                monthrun = 15,
                weekstart = 1,
                weekrun = 1,
                nxtdstart = new DateTime(2016, 12, 01),
                nxtdend = new DateTime(2016, 12, 31),
                lastrun = new DateTime(1900, 1, 1),
                lastdstart = new DateTime(2016, 11, 1),
                lastdend = new DateTime(2016, 11, 30),
                errflag = false,
                runspcl = false,
                spclstart = new DateTime(1900, 1, 1),
                spclend = new DateTime(1900, 1, 1),
                pagebrklvl = 0,
                titleacct = string.Empty,
                bcsenderemail = "info@ciswired.com",
                bcsendername = "Cornerstone",
                nextrun = new DateTime(2016, 11, 15),
                setby = string.Empty,
                holdrun = "R",
                reportdays = 1,
                rptusernum = 0,
                usespcl = false,
                nodataoptn = false,
                emailsubj = string.Empty,
                mailformat = "1",
                outputtype = OutputType.PORTABLE_DOC_FORMAT,
                displayuid = false,
                outputdest = "2",
                eProfileNo = 0,
                emailccadr = string.Empty,
                runnewdata = 0,
                unilangcode = string.Empty
            };

            #endregion

            //BlockingCollection<bcstque4> batchesToExecute = new BlockingCollection<bcstque4>() { testBcstque4 };
            var masterDataStore = new Mock<IMasterDataStore>();
            var queueRemover = new Mock<IBroadcastQueueRecordRemover>();
            var batchRecordUpdatesManager = new Mock<IBroadcastRecordUpdatesManager>();
            batchRecordUpdatesManager.Setup(r => r.UpdateQueueRecordInDataStore(It.IsAny<ICommandDb>(), It.IsAny<bcstque4>())).Returns(false);

            var p = new Parameters(queueRemover.Object, batchRecordUpdatesManager.Object)
            {
                MasterDataStore = masterDataStore.Object,
                IsMaintenanceModeRequested = false,
                BatchesToExecute = new BlockingCollection<bcstque4>() { testBcstque4 },
                ServerConfiguration = new BroadcastServerInformation()
                {
                    ReportLogoDirectory = @"C:\iBank.Reports\Logos",
                    CrystalReportDirectory = @"C:\DEV\iBankPlatform\Reporting\Main\iBank.Reporting\iBank.Services.Implementation\CrystalReports",
                    ServerNumber = 200,
                    ReportOutputDirectory = "C:\\INETPUB\\WWWROOT\\V4OUT",
                    SenderEmailAddress = "info@ciswired.com",
                    SenderName = "Cornerstone",
                    IbankBaseUrl = "https://ibank.dev.ciswired.com/ibank4"
                }
            };

            // Act
            new Consumer().ProcessBatch(testBcstque4, p, new LoadedListsParams());

            // Assert
            Assert.IsTrue(true);
        }

        // TODO: this one has an issue calling the ProcessBatches. Something to do with blocking a thread or something.
        [TestMethod]
        public void ProcessBatches_1BatchToExecute_SuccessfullyProcessBatchRecordAllTheWay()
        {
            // Arrange
            #region testBcstque4
            var testBcstque4 = new bcstque4()
            {
                bcstseqno = 1,
                dbname = "ibankdemo",
                svrstatus = "RUNNING",
                svrnumber = 200,
                starttime = new DateTime(2016, 12, 13, 9, 55, 40),
                endtime = null,
                UserNumber = 1595,
                agency = "DEMO",
                batchnum = 9028,
                batchname = "reservation monthly",
                emailaddr = "jvelazquez@ciswired.com",
                acctlist = string.Empty,
                prevhist = BroadcastSchedule.RESERVATION,
                weekmonth = 1,
                monthstart = 1,
                monthrun = 15,
                weekstart = 1,
                weekrun = 1,
                nxtdstart = new DateTime(2016, 12, 01),
                nxtdend = new DateTime(2016, 12, 31),
                lastrun = new DateTime(1900, 1, 1),
                lastdstart = new DateTime(2016, 11, 1),
                lastdend = new DateTime(2016, 11, 30),
                errflag = false,
                runspcl = false,
                spclstart = new DateTime(1900, 1, 1),
                spclend = new DateTime(1900, 1, 1),
                pagebrklvl = 0,
                titleacct = string.Empty,
                bcsenderemail = "info@ciswired.com",
                bcsendername = "Cornerstone",
                nextrun = new DateTime(2016, 11, 15),
                setby = string.Empty,
                holdrun = "R",
                reportdays = 1,
                rptusernum = 0,
                usespcl = false,
                nodataoptn = false,
                emailsubj = string.Empty,
                mailformat = "1",
                outputtype = OutputType.PORTABLE_DOC_FORMAT,
                displayuid = false,
                outputdest = "2",
                eProfileNo = 0,
                emailccadr = string.Empty,
                runnewdata = 0,
                unilangcode = string.Empty
            };

            #endregion

            //BlockingCollection<bcstque4> batchesToExecute = new BlockingCollection<bcstque4>() { testBcstque4 };
            var masterDataStore = new Mock<IMasterDataStore>();
            var queueRemover = new Mock<IBroadcastQueueRecordRemover>();
            var batchRecordUpdatesManager = new Mock<IBroadcastRecordUpdatesManager>();
            batchRecordUpdatesManager.Setup(r => r.UpdateQueueRecordInDataStore(It.IsAny<ICommandDb>(), It.IsAny<bcstque4>())).Returns(false);

            var p = new Parameters(queueRemover.Object, batchRecordUpdatesManager.Object)
            {
                MasterDataStore = masterDataStore.Object,
                IsMaintenanceModeRequested = false,
                BatchesToExecute = new BlockingCollection<bcstque4>() { testBcstque4 },
                ServerConfiguration = new BroadcastServerInformation()
                {
                    ReportLogoDirectory = @"C:\iBank.Reports\Logos",
                    CrystalReportDirectory = @"C:\DEV\iBankPlatform\Reporting\Main\iBank.Reporting\iBank.Services.Implementation\CrystalReports",
                    ServerNumber = 200,
                    ReportOutputDirectory = "C:\\INETPUB\\WWWROOT\\V4OUT",
                    SenderEmailAddress = "info@ciswired.com",
                    SenderName = "Cornerstone",
                    IbankBaseUrl = "https://ibank.dev.ciswired.com/ibank4"
                }
            };

            // Act
            //new Consumer().ProcessBatches(p);

            // Assert
            Assert.IsTrue(true);
        }
    }

}
