using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iBank.Services.Implementation.ReportPrograms.TopBottomValidatingCarriers;
using Domain.Models.ReportPrograms.TopBottomValidatingCarriers;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.TopBottomValidatingCarriers
{
    [TestClass]
    public class DataHelperTests
    {
        [TestMethod]
        public void GetExportFields_HomeCtryValidatingSecondRange_RetureFullList()
        {
            //Arrange
            DataHelper helper = new DataHelper();

            bool isCtryCode = true;
            bool isValCarr = true;
            DataTypes.GroupBy groupBy = DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER;
            bool isSecondRange = false;

            var exp = new List<string>{ "ctrycode", "homectry", "valcarr",
                "carrdesc", "trips", "amt","avgcost", "trips2", "amt2","avgcost2" };
            //Act
            var result = helper.GetExportFields(groupBy, isCtryCode, isValCarr, isSecondRange);

            //Assert
            for (int index = 0; index < result.Count; index++)
            {
                Assert.AreEqual(exp[index], result[index]);
            }
        }

        [TestMethod]
        public void GetExportFields_ValidatingHomeCtry_RetureListValcarrFirst()
        {
            //Arrange
            DataHelper helper = new DataHelper();

            bool isCtryCode = true;
            bool isValCarr = true;
            DataTypes.GroupBy groupBy = DataTypes.GroupBy.VALIDATING_CARRIER_HOME_COUNTRY;
            bool isSecondRange = false;

            var exp = new List<string>{ "valcarr", "carrdesc", "ctrycode", "homectry", "trips", "amt","avgcost"};
            //Act
            var result = helper.GetExportFields(groupBy, isCtryCode, isValCarr, isSecondRange);
            
            //Assert
            for (int index = 0; index < result.Count; index++)
            {
                Assert.AreEqual(exp[index], result[index]);
            }
        }
        [TestMethod]
        public void GetExportFields_HomeCtryValidatingNoSecondRange_RetureListNoSecondRange()
        {
            //Arrange
            DataHelper helper = new DataHelper();

            bool isCtryCode = true;
            bool isValCarr = true;
            DataTypes.GroupBy groupBy = DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER;
            bool isSecondRange = false;

            var exp = new List<string> { "ctrycode", "homectry", "valcarr", "carrdesc", "trips", "amt", "avgcost" };
            //Act
            var result = helper.GetExportFields(groupBy, isCtryCode, isValCarr, isSecondRange);

            //Assert
            for (int index = 0; index < result.Count; index++)
            {
                Assert.AreEqual(exp[index], result[index]);
            }
        }
        [TestMethod]
        public void GetExtraWhereText_BothAirAndRailInHomeCtryCanada_ReturnHomeCountryInCan()
        {
            //Arrange
            DataHelper helper = new DataHelper();
            int mode = 0; //both air and rail.
            string inHomeCtry = "CAN";
            string notInCtry = "";

            string exp = "Home Country in CAN";

            //Act
            var result = helper.GetExtraWhereText(mode, inHomeCtry, notInCtry);

            //Assert
            Assert.AreEqual(exp, result);
        }

        [TestMethod]
        public void GetExtraWhereText_RailInHomeCtryCanada_ReturnHomeCountryInCan()
        {
            //Arrange
            DataHelper helper = new DataHelper();
            int mode = 2; //rail.
            string inHomeCtry = "CAN";
            string notInCtry = "";

            string exp = "Rail Only Home Country in CAN";

            //Act
            var result = helper.GetExtraWhereText(mode, inHomeCtry, notInCtry);

            //Assert
            Assert.AreEqual(exp, result);

        }
        [TestMethod]
        public void AdjustDate2Values_cbIncludeYTDTotalIsOnTxtFYstartMonthJan_AdjustDates()
        {
            // Arrange
            DataHelper helper = new DataHelper();
            bool includeYTDTotals = true;
            DateTime begDate = new DateTime(2016, 7, 1);
            DateTime endDate = new DateTime(2016, 7, 3);
            DateTime? begDate2 = new DateTime(2016, 7, 1);
            DateTime? endDate2 = new DateTime(2016, 7, 2);
            string month = "January";

            DateTime? expBeg = new DateTime(2016, 1, 1);
            DateTime? expEnd = endDate;

            //Act
            helper.AdjustDate2Values(includeYTDTotals, begDate, endDate, ref begDate2, ref endDate2, month);

            //Assert
            Assert.AreEqual(expBeg, begDate2);
            Assert.AreEqual(expEnd, endDate2);
        }

        [TestMethod]
        public void AdjustDate2Values_cbIncludeYTDTotalIsOffTxtFYstartMonthJan_NotAdjustDates()
        {
            // Arrange
            DataHelper helper = new DataHelper();
            bool includeYTDTotals = false;
            DateTime begDate = new DateTime(2016, 7, 1);
            DateTime endDate = new DateTime(2016, 7, 3);
            DateTime? begDate2 = new DateTime(2016, 7, 1);
            DateTime? endDate2 = new DateTime(2016, 7, 2);
            string month = "January";

            DateTime? expBeg = begDate2;
            DateTime? expEnd = endDate2;

            //Act
            helper.AdjustDate2Values(includeYTDTotals, begDate, endDate, ref begDate2, ref endDate2, month);

            //Assert
            Assert.AreEqual(expBeg, begDate2);
            Assert.AreEqual(expEnd, endDate2);
        }


        [TestMethod]
        public void GetOrderBy_YTDSortIsOnSortByVolumeBooked_ReturnAmt2()
        {
            // Arrange
            DataHelper helper = new DataHelper();
            bool bYTDSort = true;
            DataTypes.SortBy sortBy = DataTypes.SortBy.VOLUME_BOOKED;

            var exp = "Amt2";
            //Act
            var result = helper.GetOrderBy(bYTDSort, sortBy);
            //Assert
            Assert.AreEqual(exp, result);
        }

        [TestMethod]
        public void GetOrderBy_YTDSortIsOffSortByAvgCostPerTrip_ReturnAvgCost()
        {
            // Arrange
            DataHelper helper = new DataHelper();
            bool bYTDSort = false;
            DataTypes.SortBy sortBy = DataTypes.SortBy.AVG_COST_PER_TRIP;

            var exp = "Avgcost";
            //Act
            var result = helper.GetOrderBy(bYTDSort, sortBy);
            //Assert
            Assert.AreEqual(exp, result);
        }

        [TestMethod]
        public void GetOrderBy_YTDSortIsOffSortByAvgCostPerTrip_ReturnCarrdesc()
        {
            // Arrange
            DataHelper helper = new DataHelper();
            bool bYTDSort = false;
            DataTypes.SortBy sortBy = DataTypes.SortBy.CARRIER_HOME_COUNTRY;

            var exp = "Carrdesc";
            //Act
            var result = helper.GetOrderBy(bYTDSort, sortBy);
            //Assert
            Assert.AreEqual(exp, result);
        }

        [TestMethod]
        public void HowManyRecords_GraphOutputRecordsIfSortISsNotCarrierHomeCountry_Return10()
        {
            // Arrange
            DataHelper helper = new DataHelper();
            var howMany = "0";
            var isGraph = true;
            DataTypes.SortBy sortBy = DataTypes.SortBy.AVG_COST_PER_TRIP;
            DataTypes.GroupBy groupBy = DataTypes.GroupBy.VALIDATING_CARRIER_HOME_COUNTRY;

            var exp = Convert.ToInt32(howMany);
            //Act
            var result = helper.HowManyRecords(howMany, isGraph, groupBy, sortBy);
            //Assert
            Assert.AreEqual(exp, result);
        }

        [TestMethod]
        public void HowManyRecords_NotGraphOutput10RecordsIsCarrierHomeCountry_Return0()
        {
            // Arrange
            DataHelper helper = new DataHelper();
            var howMany = "10";
            var isGraph = false;
            DataTypes.SortBy sortBy = DataTypes.SortBy.CARRIER_HOME_COUNTRY;
            DataTypes.GroupBy groupBy = DataTypes.GroupBy.VALIDATING_CARRIER_HOME_COUNTRY;

            var exp = 0;
            //Act
            var result = helper.HowManyRecords(howMany, isGraph, groupBy, sortBy);
            //Assert
            Assert.AreEqual(exp, result);
        }

        [TestMethod]
        public void HowManyRecords_GroupByHomeCountryValidatingCarrier_Return0()
        {
            // Arrange
            DataHelper helper = new DataHelper();
            var howMany = "10";
            var isGraph = false;
            DataTypes.SortBy sortBy = DataTypes.SortBy.CARRIER_HOME_COUNTRY;
            DataTypes.GroupBy groupBy = DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER;

            var exp = 0;
            //Act
            var result = helper.HowManyRecords(howMany, isGraph, groupBy, sortBy);
            //Assert
            Assert.AreEqual(exp, result);
        }

        [TestMethod]
        public void GetCrystalReportName_GroupByHomeCtryValCarrierHas2Dates_Return2B()
        {
            // Arrange
            DataHelper helper = new DataHelper();
            DataTypes.GroupBy groupBy = DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER;

            var exp = "ibTopValCarr2B";
            //Act
            var result = helper.GetCrystalReportName(groupBy, true, false);
            //Assert
            Assert.AreEqual(exp, result);
        }
        [TestMethod]
        public void GetCrystalReportName_GroupByHomeCtryValCarrierNo2Dates_Return2B()
        {
            // Arrange
            DataHelper helper = new DataHelper();
            DataTypes.GroupBy groupBy = DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER;

            var exp = "ibTopValCarr1B";
            //Act
            var result = helper.GetCrystalReportName(groupBy, false, false);
            //Assert
            Assert.AreEqual(exp, result);
        }

        [TestMethod]
        public void GetCrystalReportName_GroupByHomeCtryValCarrierNo2DatesIsGraph_ReturnGraph1()
        {
            // Arrange
            DataHelper helper = new DataHelper();
            DataTypes.GroupBy groupBy = DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER;

            var exp = "ibGraph1";
            //Act
            var result = helper.GetCrystalReportName(groupBy, false, true);
            //Assert
            Assert.AreEqual(exp, result);
        }
    }
}
