using System.Collections.Generic;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Services.Implementation.ReportPrograms.UserDefined;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport
{
    [TestClass]
    public class RowCounterTests
    {
        private UserDefinedParameters _params;

        private SwitchManager _switch;
        private UserReportInformation _userReport;

        [TestInitialize]
        public void Init()
        {
            var hotel = new List<HotelRawData> { new HotelRawData { RecKey = 1 } };
            var car = new List<CarRawData> { new CarRawData { RecKey = 1 } };
            var leg = new List<LegRawData> { new LegRawData { RecKey = 1 } };
            var miscSegTour = new List<MiscSegSharedRawData> { new MiscSegSharedRawData { RecKey = 1 } };
            var miscSegCruise = new List<MiscSegSharedRawData> { new MiscSegSharedRawData { RecKey = 1 } };
            var miscSegLimo = new List<MiscSegSharedRawData> { new MiscSegSharedRawData { RecKey = 1 } };
            var miscSegRail = new List<MiscSegSharedRawData> { new MiscSegSharedRawData { RecKey = 1 } };
            var svcFee = new List<ServiceFeeData> { new ServiceFeeData { RecKey = 1 } };
            var changeLog = new List<ChangeLogData> { new ChangeLogData { RecKey = 1 } };
            var marketSeg = new List<MarketSegmentRawData> { new MarketSegmentRawData { RecKey = 1 } };

            _switch = new SwitchManager(null, null);

            _params = new UserDefinedParameters();
            _params.CarDataList = car;
            _params.HotelDataList = hotel;
            _params.LegDataList = leg;
            _params.MiscSegTourDataList = miscSegTour;
            _params.MiscSegCruiseDataList = miscSegCruise;
            _params.MiscSegLimoDataList = miscSegLimo;
            _params.MiscSegRailTicketDataList = miscSegRail;
            _params.ServiceFeeDataList = svcFee;
            _params.ChangeLogDataList = changeLog;
            _params.MarketSegmentDataList = marketSeg;
        }

        [TestMethod]
        public void GetTotalRowsNeeded_NoMatchingReckey()
        {
            _switch.LegSwitch = true;
            _switch.MarketSegmentsSwitch = true;
            var userReport = new UserReportInformation() { AreAllColumnsInTripTables = false };
            var output = RowCounter.GetTotalRowsNeeded(0, _params, _switch, userReport);

            Assert.AreEqual(0, output);
        }

        [TestMethod]
        public void GetTotalRowsNeeded_MatchingReckey_AllSameValue()
        {
            _switch.LegSwitch = true;
            _switch.MarketSegmentsSwitch = true;
            var userReport = new UserReportInformation() { AreAllColumnsInTripTables = false };
            var output = RowCounter.GetTotalRowsNeeded(1, _params, _switch, userReport);

            Assert.AreEqual(1, output);
        }

        [TestMethod]
        public void GetTotalRowsNeeded_MatchingReckey_HotelHoldsMax()
        {
            _params.HotelDataList.Add(new HotelRawData { RecKey =  1 });
            _switch.LegSwitch = true;
            _switch.MarketSegmentsSwitch = true;
            var userReport = new UserReportInformation() { AreAllColumnsInTripTables = false };
            var output = RowCounter.GetTotalRowsNeeded(1, _params, _switch, userReport);

            Assert.AreEqual(2, output);
        }
    }
}
