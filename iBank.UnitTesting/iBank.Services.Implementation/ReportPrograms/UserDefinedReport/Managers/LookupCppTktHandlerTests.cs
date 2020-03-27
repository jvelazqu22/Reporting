using System;
using System.Collections.Generic;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.LookupFieldHandlers;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Shared.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport.Managers
{
    [TestClass]
    public class LookupCppTktHandlerTests
    {
        private ReportLookups _reportLookups;
        private CppTicketIndicatorHelper _helper;

        [TestInitialize]
        public void Init()
        {
            var columns = new List<UserReportColumnInformation>();
            var buildWhere = new BuildWhere(new ClientFunctions());
            var globals = new ReportGlobals();

            _reportLookups = new ReportLookups(columns, buildWhere, globals);

            _helper = new CppTicketIndicatorHelper();
        }

        [TestMethod]
        public void LookupCppTicketIndicator_MarketSegmentDataListIsNotEmpty_ReturnN()
        {
            //arrange
            var marketSegmentDataList = new List<MarketSegmentRawData>();

            //act
            var sut = _helper.FindIndicator(1, _reportLookups, marketSegmentDataList);

            //assert
            Assert.AreEqual("N", sut);

        }

        [TestMethod]
        public void LookupCppTicketIndicator_MarketSegmentDataListHasAtLeastOneQualifies_ReturnY()
        {
            //arrange
            var marketSegmentDataList = new List<MarketSegmentRawData>() {
                new MarketSegmentRawData { RecKey = 1, Prdfbase = "YCA" },
                new MarketSegmentRawData { RecKey = 3, Prdfbase = "VCA" },
                new MarketSegmentRawData { RecKey = 3, Prdfbase = "SDG" },
                new MarketSegmentRawData { RecKey = 4, Prdfbase = "CA" }
            };

            //act
            var sut1 = _helper.FindIndicator(1, _reportLookups, marketSegmentDataList);
            var sut3 = _helper.FindIndicator(3, _reportLookups, marketSegmentDataList);

            //assert
            Assert.AreEqual("Y", sut1);
            Assert.AreEqual("Y", sut3);

        }

        [TestMethod]
        public void LookupCppTicketIndicator_xCBTranslatedCodeExists_ReturnY()
        {
            //arrange
            var marketSegmentDataList = new List<MarketSegmentRawData>() {
                new MarketSegmentRawData { RecKey = 1, Prdfbase = "xCB" }
            };

            //act
            var sut = _helper.FindIndicator(1, _reportLookups, marketSegmentDataList);

            //assert
            Assert.AreEqual("Y", sut);

        }


        [TestMethod]
        public void LookupCppTicketIndicator_xCATranslatedCodeExists_ReturnY()
        {
            //arrange
            var marketSegmentDataList = new List<MarketSegmentRawData>() {
                new MarketSegmentRawData { RecKey = 1, Prdfbase = "xCA" }
            };

            //act
            var sut = _helper.FindIndicator(1, _reportLookups, marketSegmentDataList);

            //assert
            Assert.AreEqual("Y", sut);

        }


        [TestMethod]
        public void LookupCppTicketIndicator_ReckeyDoesNotExistsinMarketSegmentDataList_ReturnN()
        {
            //arrange
            var marketSegmentDataList = new List<MarketSegmentRawData>() {
                new MarketSegmentRawData { RecKey = 1, Prdfbase = "YCA" },
                new MarketSegmentRawData { RecKey = 3, Prdfbase = "VCA" },
                new MarketSegmentRawData { RecKey = 3, Prdfbase = "SDG" },
                new MarketSegmentRawData { RecKey = 4, Prdfbase = "CA" },
                new MarketSegmentRawData { RecKey = 4, Prdfbase = "B" }
            };

            //act
            var sut = _helper.FindIndicator(5, _reportLookups, marketSegmentDataList);

            //assert
            Assert.AreEqual("N", sut);

        }
    }
}
