using Domain.Interfaces.BroadcastServer;
using iBank.BroadcastServer.Timing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

using Domain.Exceptions;

using iBank.BroadcastServer.BroadcastBatch;

namespace iBank.UnitTesting.iBank.BroadcastServer.Timing
{
    [TestClass]
    public class PreRunTimingCalculatorTests
    {
        [TestMethod]
        public void GetInitialReportPeriodStartAndEnd_DontRunNewData_ReturnOriginalPeriod()
        {
            var calc = new PreRunTimingCalculator();
            var lastRun = new DateTime(2001, 1, 1);
            var reportPeriodStart = new DateTime(2001, 1, 2);
            var reportPeriodEnd = new DateTime(2001, 1, 3);
            var runNewData = false;
            var broadcastScheduleData = 1;
            var now = new DateTime(2016, 1, 1, 3, 00, 00);
            var timing = new Mock<IRecordTimingDetails>();
            timing.Setup(x => x.RunNewData).Returns(runNewData);
            timing.Setup(x => x.BroadcastScheduleData).Returns(broadcastScheduleData);
            timing.Setup(x => x.LastRun).Returns(lastRun);
            timing.Setup(x => x.NextReportPeriodStart).Returns(reportPeriodStart);
            timing.Setup(x => x.NextReportPeriodEnd).Returns(reportPeriodEnd);
            timing.Setup(x => x.Now).Returns(now);

            calc.SetInitialRecordNextReportPeriodStartAndEnd(timing.Object);

            Assert.AreEqual(reportPeriodStart, timing.Object.NextReportPeriodStart);
            Assert.AreEqual(reportPeriodEnd, timing.Object.NextReportPeriodEnd);
        }

        [TestMethod]
        public void GetInitialReportPeriodStartAndEnd_RunNewDataHistoryBroadcast_ReturnOriginalPeriod()
        {
            var calc = new PreRunTimingCalculator();
            var lastRun = new DateTime(2001, 1, 1);
            var reportPeriodStart = new DateTime(2001, 1, 2);
            var reportPeriodEnd = new DateTime(2001, 1, 3);
            var runNewData = true;
            var broadcastScheduleData = 2;
            var now = new DateTime(2016, 1, 1, 3, 00, 00);
            var timing = new Mock<IRecordTimingDetails>();
            timing.Setup(x => x.RunNewData).Returns(runNewData);
            timing.Setup(x => x.BroadcastScheduleData).Returns(broadcastScheduleData);
            timing.Setup(x => x.LastRun).Returns(lastRun);
            timing.Setup(x => x.NextReportPeriodStart).Returns(reportPeriodStart);
            timing.Setup(x => x.NextReportPeriodEnd).Returns(reportPeriodEnd);
            timing.Setup(x => x.Now).Returns(now);

            calc.SetInitialRecordNextReportPeriodStartAndEnd(timing.Object);

            Assert.AreEqual(reportPeriodStart, timing.Object.NextReportPeriodStart);
            Assert.AreEqual(reportPeriodEnd, timing.Object.NextReportPeriodEnd);
        }

        [TestMethod]
        public void GetInitialReportPeriodStartAndEnd_RunNewDataReservationBroadcastLastRunNoValue_ReturnOriginalStartSetEndToNow()
        {
            var calc = new PreRunTimingCalculator();
            var reportPeriodStart = new DateTime(2001, 1, 2);
            var reportPeriodEnd = new DateTime(2001, 1, 3);
            var runNewData = true;
            var broadcastScheduleData = 1;
            var now = new DateTime(2016, 1, 1, 3, 00, 00);
            DateTime? lastRun = null;
            var timing = new Mock<IRecordTimingDetails>();
            timing.Setup(x => x.RunNewData).Returns(runNewData);
            timing.Setup(x => x.BroadcastScheduleData).Returns(broadcastScheduleData);
            timing.Setup(x => x.LastRun).Returns(lastRun);
            timing.Setup(x => x.NextReportPeriodStart).Returns(reportPeriodStart);
            timing.Setup(x => x.NextReportPeriodEnd).Returns(reportPeriodEnd);
            timing.Setup(x => x.Now).Returns(now);

            calc.SetInitialRecordNextReportPeriodStartAndEnd(timing.Object);

            Assert.AreEqual(reportPeriodStart, timing.Object.NextReportPeriodStart);
            Assert.AreNotEqual(now, timing.Object.NextReportPeriodEnd);
        }

        [TestMethod]
        public void GetInitialReportPeriodStartAndEnd_RunNewDataReservationBroadcastLastRunHasValue_ReturnStartSetToLastRuntSetEndToNow()
        {
            var calc = new PreRunTimingCalculator();
            var reportPeriodStart = new DateTime(2001, 1, 2);
            var reportPeriodEnd = new DateTime(2001, 1, 3);
            var runNewData = true;
            var broadcastScheduleData = 1;
            var now = new DateTime(2016, 1, 1, 3, 00, 00);
            DateTime? lastRun = new DateTime(2016, 1, 2);
            var timing = new Mock<IRecordTimingDetails>();
            timing.Setup(x => x.RunNewData).Returns(runNewData);
            timing.Setup(x => x.BroadcastScheduleData).Returns(broadcastScheduleData);
            timing.Setup(x => x.LastRun).Returns(lastRun);
            timing.Setup(x => x.NextReportPeriodStart).Returns(reportPeriodStart);
            timing.Setup(x => x.NextReportPeriodEnd).Returns(reportPeriodEnd);
            timing.Setup(x => x.Now).Returns(now);

            calc.SetInitialRecordNextReportPeriodStartAndEnd(timing.Object);

            Assert.AreEqual(reportPeriodStart, timing.Object.NextReportPeriodStart);
            Assert.AreNotEqual(now, timing.Object.NextReportPeriodEnd);
        }

        [TestMethod]
        [ExpectedException(typeof(NotTimeToRunBroadcastException))]
        public void SetRunDailyPriorBusinessDayStartAndEndRange_TodayIsWeekend_ThrowNotTimeToRunException()
        {
            var sut = new PreRunTimingCalculator();
            var timing = new Mock<IRecordTimingDetails>();
            timing.Setup(x => x.Today).Returns(new DateTime(2016, 12, 25)); //this is a sunday
            var batchManager = new Mock<IBatchManager>();

            sut.SetRunDailyPriorBusinessDayStartAndEndRange(timing.Object, batchManager.Object);

            batchManager.Verify(x => x.IncrementOriginalBatchNextRun(new DateTime()), Times.Once);
        }

        [TestMethod]
        public void SetRunDailyPriorBusinessDayStartAndEndRange_TodayIsMonday_CoverTheWeekendWithTheRange()
        {
            var sut = new PreRunTimingCalculator();
            var timing = new RecordTimingDetails();
            timing.Today = new DateTime(2016, 12, 26); // this is a monday
            var batchManager = new Mock<IBatchManager>();
            
            sut.SetRunDailyPriorBusinessDayStartAndEndRange(timing, batchManager.Object);

            Assert.AreEqual(new DateTime(2016, 12, 23), timing.NextReportPeriodStart);
            Assert.AreEqual(new DateTime(2016, 12, 25), timing.NextReportPeriodEnd);
        }

        [TestMethod]
        public void SetRunDailyPriorBusinessDay_TodayIsBusinessDayNotMonday_NextReportPeriodIsPreviousDay()
        {
            var sut = new PreRunTimingCalculator();
            var timing = new RecordTimingDetails();
            timing.Today = new DateTime(2016, 12, 27); //this is a tuesday
            var batchManager = new Mock<IBatchManager>();

            sut.SetRunDailyPriorBusinessDayStartAndEndRange(timing, batchManager.Object);

            Assert.AreEqual(new DateTime(2016, 12, 26), timing.NextReportPeriodStart);
            Assert.AreEqual(new DateTime(2016, 12, 26), timing.NextReportPeriodEnd);
        }

        [TestMethod]
        [ExpectedException(typeof(NotTimeToRunBroadcastException))]
        public void SetRunDailyNextBusinessDayStartAndEndRange_TodayIsFriday_ThrowException()
        {
            var sut = new PreRunTimingCalculator();
            var timing = new RecordTimingDetails();
            timing.Today = new DateTime(2016, 12, 23);// this is friday
            var batchManager = new Mock<IBatchManager>();

            sut.SetRunDailyNextBusinessDayStartAndEndRange(timing, batchManager.Object);

            batchManager.Verify(x => x.IncrementOriginalBatchNextRun(new DateTime()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(NotTimeToRunBroadcastException))]
        public void SetRunDailyNextBusinessDayStartAndEndRange_TodayIsSaturday_ThrowException()
        {
            var sut = new PreRunTimingCalculator();
            var timing = new RecordTimingDetails();
            timing.Today = new DateTime(2016, 12, 24);// this is saturday
            var batchManager = new Mock<IBatchManager>();

            sut.SetRunDailyNextBusinessDayStartAndEndRange(timing, batchManager.Object);

            batchManager.Verify(x => x.IncrementOriginalBatchNextRun(new DateTime()), Times.Once);
        }

        [TestMethod]
        public void SetRunDailyNextBusinessDayStartAndEndRange_TodayIsNotFridayOrSaturday_RangeIsNextDay()
        {
            var sut = new PreRunTimingCalculator();
            var timing = new RecordTimingDetails();
            timing.Today = new DateTime(2016, 12, 22);// this is thursday
            var batchManager = new Mock<IBatchManager>();

            sut.SetRunDailyNextBusinessDayStartAndEndRange(timing, batchManager.Object);

            Assert.AreEqual(new DateTime(2016, 12, 23), timing.NextReportPeriodStart);
            Assert.AreEqual(new DateTime(2016, 12, 23), timing.NextReportPeriodEnd);
        }

        [TestMethod]
        public void SetReportRangeToSpecialDates()
        {
            var sut = new PreRunTimingCalculator();
            var timing = new RecordTimingDetails();
            timing.NextReportPeriodStart = new DateTime(2016, 12, 21);
            timing.NextReportPeriodEnd = new DateTime(2016, 12, 22);
            timing.SpecialReportPeriodStart = new DateTime(2015, 11, 1);
            timing.SpecialReportPeriodEnd = new DateTime(2015, 11, 2);

            sut.SetReportRangeToSpecialDates(timing);

            Assert.AreEqual(timing.SpecialReportPeriodStart, timing.NextReportPeriodStart);
            Assert.AreEqual(timing.SpecialReportPeriodEnd, timing.NextReportPeriodEnd);

        }
    }
}
