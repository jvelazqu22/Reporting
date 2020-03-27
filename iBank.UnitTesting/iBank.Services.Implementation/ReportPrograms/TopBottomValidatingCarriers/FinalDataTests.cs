using Domain.Models.ReportPrograms.TopBottomValidatingCarriers;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.TopBottomValidatingCarriers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.TopBottomValidatingCarriers
{
    /// <summary>
    /// Summary description for FinalDataTests
    /// </summary>
    [TestClass]
    public class FinalDataTests
    {
        [TestMethod]
        public void GroupFinalData_FinalDataNotGraphGroupBy_RecordsOrderByTripsKeepAllRecords()
        {
            // Arrange
            FinalDataHelper helper = new FinalDataHelper(new ReportGlobals(), "Trips", DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER, DataTypes.SortBy.NO_OF_TRIPS);
            List<FinalData> final = new List<FinalData>{
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 2", ValCarr = "A2", Amt = 1800.00m, Trips = 20},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 7", ValCarr = "A7", Amt = 1400.00m, Trips = 9},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "DELTA", ValCarr = "DL", Amt = 2100m, Trips = 40},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 1", ValCarr = "A1", Amt = 1900.00m, Trips = 30 },
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 3", ValCarr = "A3", Amt = 1700.00m, Trips = 19},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "AMERICAN AIRLINES", ValCarr = "AA", Amt = 2200.00m, Trips = 50},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 5", ValCarr = "A5", Amt = 1600.00m, Trips = 18},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 8", ValCarr = "A8", Amt = 1300.00m, Trips = 8},
                new FinalData() { HomeCtry = "Canada", Carrdesc = "Airline 4", ValCarr = "A4", Amt = 1200.00m, Trips = 1},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 9", ValCarr = "A9", Amt = 1100.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 6", ValCarr = "A6", Amt = 1500.00m, Trips = 17},
            };
            bool isGraph = false;
            DataTypes.Sort sort = DataTypes.Sort.Descending;
            var nHowMany = 0;


            List<FinalData> exp = new List<FinalData>{
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "AMERICAN AIRLINES", ValCarr = "AA", Amt = 2200.00m, Trips = 50},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "DELTA", ValCarr = "DL", Amt = 2100m, Trips = 40},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 1", ValCarr = "A1", Amt = 1900.00m, Trips = 30 },
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 2", ValCarr = "A2", Amt = 1800.00m, Trips = 20},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 3", ValCarr = "A3", Amt = 1700.00m, Trips = 19},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 5", ValCarr = "A5", Amt = 1600.00m, Trips = 18},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 6", ValCarr = "A6", Amt = 1500.00m, Trips = 17},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 7", ValCarr = "A7", Amt = 1400.00m, Trips = 9},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 8", ValCarr = "A8", Amt = 1300.00m, Trips = 8},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 9", ValCarr = "A9", Amt = 1100.00m, Trips = 2},
                new FinalData() { HomeCtry = "Canada", Carrdesc = "Airline 4", ValCarr = "A4", Amt = 1200.00m, Trips = 1}
            };
            //Act
            var results = helper.GroupFinalData(final, isGraph, sort, nHowMany);

            //Assert
            for (int index = 0; index < results.Count; index++)
            {
                Assert.AreEqual(exp[index].HomeCtry, results[index].HomeCtry);
                Assert.AreEqual(exp[index].Amt, results[index].Amt);
                Assert.AreEqual(exp[index].Trips, results[index].Trips);
                Assert.AreEqual(exp[index].Avgcost, results[index].Avgcost);
                Assert.AreEqual(exp[index].SubAmt, results[index].SubAmt);
                Assert.AreEqual(exp[index].SubTrips, results[index].SubTrips);
                Assert.AreEqual(exp[index].Amt2, results[index].Amt2);
                Assert.AreEqual(exp[index].Trips2, results[index].Trips2);
                Assert.AreEqual(exp[index].Avgcost2, results[index].Avgcost2);
                Assert.AreEqual(exp[index].SubAmt2, results[index].SubAmt2);
                Assert.AreEqual(exp[index].SubTrips2, results[index].SubTrips2);
            }
        }

        [TestMethod]
        public void GroupFinalData_FinalDataNotGraphGroupBy_OnlyKeep10RecordsOrderByAmt()
        {
            // Arrange
            FinalDataHelper helper = new FinalDataHelper(new ReportGlobals(), "Amt", DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER, DataTypes.SortBy.VOLUME_BOOKED);
            List<FinalData> final = new List<FinalData>{
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 2", ValCarr = "A2", Amt = 1800.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "DELTA", ValCarr = "DL", Amt = 2100m, Trips = 3},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 1", ValCarr = "A1", Amt = 1900.00m, Trips = 2 },
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 7", ValCarr = "A7", Amt = 1400.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 3", ValCarr = "A3", Amt = 1700.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "AMERICAN AIRLINES", ValCarr = "AA", Amt = 2200.00m, Trips = 10},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 4", ValCarr = "A4", Amt = 1600.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 8", ValCarr = "A8", Amt = 1300.00m, Trips = 2},
                new FinalData() { HomeCtry = "Canada", Carrdesc = "Airline 4", ValCarr = "A4", Amt = 1200.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 9", ValCarr = "A9", Amt = 1100.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 6", ValCarr = "A6", Amt = 1500.00m, Trips = 2}
            };
            bool isGraph = false;
            DataTypes.Sort sort = DataTypes.Sort.Descending;
            var nHowMany = 10;


            List<FinalData> exp = new List<FinalData>{
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "AMERICAN AIRLINES", ValCarr = "AA", Amt = 2200.00m, Trips = 10},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "DELTA", ValCarr = "DL", Amt = 2100m, Trips = 3},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 1", ValCarr = "A1", Amt = 1900.00m, Trips = 2 },
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 2", ValCarr = "A2", Amt = 1800.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 3", ValCarr = "A3", Amt = 1700.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 4", ValCarr = "A4", Amt = 1600.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 6", ValCarr = "A6", Amt = 1500.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 7", ValCarr = "A7", Amt = 1400.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 8", ValCarr = "A8", Amt = 1300.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 9", ValCarr = "A9", Amt = 1100.00m, Trips = 2},
                new FinalData() { HomeCtry = "Canada", Carrdesc = "Airline 4", ValCarr = "A4", Amt = 1200.00m, Trips = 2}
            };
            //Act
            var results = helper.GroupFinalData(final, isGraph, sort, nHowMany);

            //Assert
            for (int index = 0; index < results.Count; index++)
            {
                Assert.AreEqual(exp[index].HomeCtry, results[index].HomeCtry);
                Assert.AreEqual(exp[index].Amt, results[index].Amt);
                Assert.AreEqual(exp[index].Trips, results[index].Trips);
                Assert.AreEqual(exp[index].Avgcost, results[index].Avgcost);
                Assert.AreEqual(exp[index].SubAmt, results[index].SubAmt);
                Assert.AreEqual(exp[index].SubTrips, results[index].SubTrips);
                Assert.AreEqual(exp[index].Amt2, results[index].Amt2);
                Assert.AreEqual(exp[index].Trips2, results[index].Trips2);
                Assert.AreEqual(exp[index].Avgcost2, results[index].Avgcost2);
                Assert.AreEqual(exp[index].SubAmt2, results[index].SubAmt2);
                Assert.AreEqual(exp[index].SubTrips2, results[index].SubTrips2);
            }
        }


        [TestMethod]
        public void GroupFinalData_FinalDataNotGraphGroupBySortCarrierHomeAsc_OrderByCarrdesc()
        {
            // Arrange
            FinalDataHelper helper = new FinalDataHelper(new ReportGlobals(), "Carrdesc", DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER, DataTypes.SortBy.CARRIER_HOME_COUNTRY);
            List<FinalData> final = new List<FinalData>{
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 2", ValCarr = "A2", Amt = 1800.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 1", ValCarr = "A1", Amt = 1900.00m, Trips = 2 },
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 7", ValCarr = "A7", Amt = 1400.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 3", ValCarr = "A3", Amt = 1700.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 4", ValCarr = "A4", Amt = 1600.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 8", ValCarr = "A8", Amt = 1300.00m, Trips = 2},
                new FinalData() { HomeCtry = "Canada", Carrdesc = "Airline 4", ValCarr = "A4", Amt = 1200.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 9", ValCarr = "A9", Amt = 1100.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 6", ValCarr = "A6", Amt = 1500.00m, Trips = 2}
            };
            bool isGraph = false;
            DataTypes.Sort sort = DataTypes.Sort.Ascending;
            var nHowMany = 0;


            List<FinalData> exp = new List<FinalData>{
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 1", ValCarr = "A1", Amt = 1900.00m, Trips = 2 },
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 2", ValCarr = "A2", Amt = 1800.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 3", ValCarr = "A3", Amt = 1700.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 4", ValCarr = "A4", Amt = 1600.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 6", ValCarr = "A6", Amt = 1500.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 7", ValCarr = "A7", Amt = 1400.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 8", ValCarr = "A8", Amt = 1300.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 9", ValCarr = "A9", Amt = 1100.00m, Trips = 2},
                new FinalData() { HomeCtry = "Canada", Carrdesc = "Airline 4", ValCarr = "A4", Amt = 1200.00m, Trips = 2}
            };
            //Act
            var results = helper.GroupFinalData(final, isGraph, sort, nHowMany);

            //Assert
            for (int index = 0; index < results.Count; index++)
            {
                Assert.AreEqual(exp[index].HomeCtry, results[index].HomeCtry);
                Assert.AreEqual(exp[index].Amt, results[index].Amt);
                Assert.AreEqual(exp[index].Trips, results[index].Trips);
                Assert.AreEqual(exp[index].Avgcost, results[index].Avgcost);
                Assert.AreEqual(exp[index].SubAmt, results[index].SubAmt);
                Assert.AreEqual(exp[index].SubTrips, results[index].SubTrips);
                Assert.AreEqual(exp[index].Amt2, results[index].Amt2);
                Assert.AreEqual(exp[index].Trips2, results[index].Trips2);
                Assert.AreEqual(exp[index].Avgcost2, results[index].Avgcost2);
                Assert.AreEqual(exp[index].SubAmt2, results[index].SubAmt2);
                Assert.AreEqual(exp[index].SubTrips2, results[index].SubTrips2);
            }
        }

        [TestMethod]
        public void GroupFinalData_GroupByHomeCtryValCarrSortByCarrierHomeCtryDesc_HomeDescCarrierDesc()
        {
            // Arrange
            FinalDataHelper helper = new FinalDataHelper(new ReportGlobals(), "Carrdesc", DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER, DataTypes.SortBy.CARRIER_HOME_COUNTRY);
            List<FinalData> final = new List<FinalData>{
                new FinalData() { HomeCtry = "Canada", Carrdesc = "Airline 4", ValCarr = "A4", Amt = 1200.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 2", ValCarr = "A2", Amt = 1800.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 1", ValCarr = "A1", Amt = 1900.00m, Trips = 2 },
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 7", ValCarr = "A7", Amt = 1400.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 3", ValCarr = "A3", Amt = 1700.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 4", ValCarr = "A4", Amt = 1600.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 8", ValCarr = "A8", Amt = 1300.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 9", ValCarr = "A9", Amt = 1100.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 6", ValCarr = "A6", Amt = 1500.00m, Trips = 2}
            };
            bool isGraph = false;
            DataTypes.Sort sort = DataTypes.Sort.Descending;
            var nHowMany = 0;


            List<FinalData> exp = new List<FinalData>{
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 9", ValCarr = "A9", Amt = 1100.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 8", ValCarr = "A8", Amt = 1300.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 7", ValCarr = "A7", Amt = 1400.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 6", ValCarr = "A6", Amt = 1500.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 4", ValCarr = "A4", Amt = 1600.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 3", ValCarr = "A3", Amt = 1700.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 2", ValCarr = "A2", Amt = 1800.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 1", ValCarr = "A1", Amt = 1900.00m, Trips = 2 },
                new FinalData() { HomeCtry = "Canada", Carrdesc = "Airline 4", ValCarr = "A4", Amt = 1200.00m, Trips = 2},
            };
            //Act
            var results = helper.GroupFinalData(final, isGraph, sort, nHowMany);

            //Assert
            for (int index = 0; index < results.Count; index++)
            {
                Assert.AreEqual(exp[index].HomeCtry, results[index].HomeCtry);
                Assert.AreEqual(exp[index].Amt, results[index].Amt);
                Assert.AreEqual(exp[index].Trips, results[index].Trips);
                Assert.AreEqual(exp[index].Avgcost, results[index].Avgcost);
                Assert.AreEqual(exp[index].SubAmt, results[index].SubAmt);
                Assert.AreEqual(exp[index].SubTrips, results[index].SubTrips);
                Assert.AreEqual(exp[index].Amt2, results[index].Amt2);
                Assert.AreEqual(exp[index].Trips2, results[index].Trips2);
                Assert.AreEqual(exp[index].Avgcost2, results[index].Avgcost2);
                Assert.AreEqual(exp[index].SubAmt2, results[index].SubAmt2);
                Assert.AreEqual(exp[index].SubTrips2, results[index].SubTrips2);
            }
        }



        [TestMethod]
        public void GroupFinalData_GroupByValCarrSortByCarrierHomeCtryDesc_OrderByCarrdesc()
        {
            // Arrange
            FinalDataHelper helper = new FinalDataHelper(new ReportGlobals(), "Carrdesc", DataTypes.GroupBy.VALIDATING_CARRIER_ONLY, DataTypes.SortBy.CARRIER_HOME_COUNTRY);
            List<FinalData> final = new List<FinalData>{
                new FinalData() { HomeCtry = "NA", Carrdesc = "Airline 2", ValCarr = "A2", Amt = 1800.00m, Trips = 2},
                new FinalData() { HomeCtry = "NA", Carrdesc = "Airline 1", ValCarr = "A1", Amt = 1900.00m, Trips = 2 },
                new FinalData() { HomeCtry = "NA", Carrdesc = "Airline 7", ValCarr = "A7", Amt = 1400.00m, Trips = 2},
                new FinalData() { HomeCtry = "NA", Carrdesc = "Airline 3", ValCarr = "A3", Amt = 1700.00m, Trips = 2},
                new FinalData() { HomeCtry = "NA", Carrdesc = "Airline 4", ValCarr = "A4", Amt = 1600.00m, Trips = 2},
                new FinalData() { HomeCtry = "NA", Carrdesc = "Airline 8", ValCarr = "A8", Amt = 1300.00m, Trips = 2},
                new FinalData() { HomeCtry = "NA", Carrdesc = "Airline 9", ValCarr = "A9", Amt = 1100.00m, Trips = 2},
                new FinalData() { HomeCtry = "NA", Carrdesc = "Airline 6", ValCarr = "A6", Amt = 1500.00m, Trips = 2}
            };
            bool isGraph = false;
            DataTypes.Sort sort = DataTypes.Sort.Descending;
            var nHowMany = 0;


            List<FinalData> exp = new List<FinalData>{
                new FinalData() { HomeCtry = "NA", Carrdesc = "Airline 9", ValCarr = "A9", Amt = 1100.00m, Trips = 2},
                new FinalData() { HomeCtry = "NA", Carrdesc = "Airline 8", ValCarr = "A8", Amt = 1300.00m, Trips = 2},
                new FinalData() { HomeCtry = "NA", Carrdesc = "Airline 7", ValCarr = "A7", Amt = 1400.00m, Trips = 2},
                new FinalData() { HomeCtry = "NA", Carrdesc = "Airline 6", ValCarr = "A6", Amt = 1500.00m, Trips = 2},
                new FinalData() { HomeCtry = "NA", Carrdesc = "Airline 4", ValCarr = "A4", Amt = 1600.00m, Trips = 2},
                new FinalData() { HomeCtry = "NA", Carrdesc = "Airline 3", ValCarr = "A3", Amt = 1700.00m, Trips = 2},
                new FinalData() { HomeCtry = "NA", Carrdesc = "Airline 2", ValCarr = "A2", Amt = 1800.00m, Trips = 2},
                new FinalData() { HomeCtry = "NA", Carrdesc = "Airline 1", ValCarr = "A1", Amt = 1900.00m, Trips = 2 }
            };
            //Act
            var results = helper.GroupFinalData(final, isGraph, sort, nHowMany);

            //Assert
            for (int index = 0; index < results.Count; index++)
            {
                Assert.AreEqual(exp[index].HomeCtry, results[index].HomeCtry);
                Assert.AreEqual(exp[index].Amt, results[index].Amt);
                Assert.AreEqual(exp[index].Trips, results[index].Trips);
                Assert.AreEqual(exp[index].Avgcost, results[index].Avgcost);
                Assert.AreEqual(exp[index].SubAmt, results[index].SubAmt);
                Assert.AreEqual(exp[index].SubTrips, results[index].SubTrips);
                Assert.AreEqual(exp[index].Amt2, results[index].Amt2);
                Assert.AreEqual(exp[index].Trips2, results[index].Trips2);
                Assert.AreEqual(exp[index].Avgcost2, results[index].Avgcost2);
                Assert.AreEqual(exp[index].SubAmt2, results[index].SubAmt2);
                Assert.AreEqual(exp[index].SubTrips2, results[index].SubTrips2);
            }
        }


        [TestMethod]
        public void GroupFinalData_GroupByValCarrHomeCtry_OnlyKeep10GroupsOrderByAmt()
        {
            // Arrange
            FinalDataHelper helper = new FinalDataHelper(new ReportGlobals(), "Amt", DataTypes.GroupBy.VALIDATING_CARRIER_HOME_COUNTRY, DataTypes.SortBy.VOLUME_BOOKED);
            List<FinalData> final = new List<FinalData>{
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 2", ValCarr = "A2", Amt = 1800.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "DELTA", ValCarr = "DL", Amt = 2100m, Trips = 3},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 1", ValCarr = "A1", Amt = 1900.00m, Trips = 2 },
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 7", ValCarr = "A7", Amt = 1400.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 3", ValCarr = "A3", Amt = 1700.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "AMERICAN AIRLINES", ValCarr = "AA", Amt = 2200.00m, Trips = 10},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 4", ValCarr = "A4", Amt = 1600.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 8", ValCarr = "A8", Amt = 1300.00m, Trips = 2},
                new FinalData() { HomeCtry = "Canada", Carrdesc = "Airline 4", ValCarr = "A4", Amt = 1200.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 9", ValCarr = "A9", Amt = 1100.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 6", ValCarr = "A6", Amt = 1500.00m, Trips = 2},
            };
            bool isGraph = false;
            DataTypes.Sort sort = DataTypes.Sort.Descending;
            var nHowMany = 10;


            List<FinalData> exp = new List<FinalData>{
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "AMERICAN AIRLINES", ValCarr = "AA", Amt = 2200.00m, Trips = 10},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "DELTA", ValCarr = "DL", Amt = 2100m, Trips = 3},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 1", ValCarr = "A1", Amt = 1900.00m, Trips = 2 },
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 2", ValCarr = "A2", Amt = 1800.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 3", ValCarr = "A3", Amt = 1700.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 4", ValCarr = "A4", Amt = 1600.00m, Trips = 2},
                new FinalData() { HomeCtry = "Canada", Carrdesc = "Airline 4", ValCarr = "A4", Amt = 1200.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 6", ValCarr = "A6", Amt = 1500.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 7", ValCarr = "A7", Amt = 1400.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 8", ValCarr = "A8", Amt = 1300.00m, Trips = 2},
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "Airline 9", ValCarr = "A9", Amt = 1100.00m, Trips = 2},
            };
            //Act
            var results = helper.GroupFinalData(final, isGraph, sort, nHowMany);

            //Assert
            for (int index = 0; index < results.Count; index++)
            {
                Assert.AreEqual(exp[index].HomeCtry, results[index].HomeCtry);
                Assert.AreEqual(exp[index].Amt, results[index].Amt);
                Assert.AreEqual(exp[index].Trips, results[index].Trips);
                Assert.AreEqual(exp[index].Avgcost, results[index].Avgcost);
                Assert.AreEqual(exp[index].SubAmt, results[index].SubAmt);
                Assert.AreEqual(exp[index].SubTrips, results[index].SubTrips);
                Assert.AreEqual(exp[index].Amt2, results[index].Amt2);
                Assert.AreEqual(exp[index].Trips2, results[index].Trips2);
                Assert.AreEqual(exp[index].Avgcost2, results[index].Avgcost2);
                Assert.AreEqual(exp[index].SubAmt2, results[index].SubAmt2);
                Assert.AreEqual(exp[index].SubTrips2, results[index].SubTrips2);
            }
        }

        [TestMethod]
        public void GroupFinalData_FinalDataNotGraphGroupBy_ReturnListOrderByAmt()
        {
            // Arrange
            FinalDataHelper helper = new FinalDataHelper(new ReportGlobals(), "Amt", DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER, DataTypes.SortBy.VOLUME_BOOKED);
            List<FinalData> final = new List<FinalData>{
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "DELTA", ValCarr = "DL", Amt = 2100m, Trips = 3, SubAmt = 2100m, Avgcost = 700m, SubTrips=15, Amt2 = 1500.00m, Trips2 = 5,  SubAmt2 = 300.00m, SubTrips2=10, Avgcost2 = 300.00m },
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "AMERICAN AIRLINES", ValCarr = "AA", Amt = 2200.00m, Trips = 10, SubAmt = 2200.00m, Avgcost = 220.00m, SubTrips=15,Amt2 = 1500.00m, Trips2 = 5, SubAmt2 = 1500.00m, SubTrips2=10,Avgcost2 = 300.00m },
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "UNITED AIRLINES", ValCarr = "UA", Amt = 600.00m, Trips = 2, SubAmt = 600.00m, Avgcost = 300.00m, SubTrips=15,Amt2 = 0m, Trips2 = 0, SubAmt2 = 0m, SubTrips2=10,Avgcost2 = 0m }
            };
            bool isGraph = false;
            DataTypes.Sort sort = DataTypes.Sort.Descending;
            var nHowMany = 10;

            List<FinalData> exp = new List<FinalData>{
               new FinalData() { HomeCtry = "United States of America", Carrdesc = "AMERICAN AIRLINES", ValCarr = "AA", Amt = 2200.00m, Trips = 10, SubAmt = 2200.00m, Avgcost = 220.00m, SubTrips=15,Amt2 = 1500.00m, Trips2 = 5, SubAmt2 = 1500.00m, SubTrips2=10,Avgcost2 = 300.00m },
               new FinalData() { HomeCtry = "United States of America", Carrdesc = "DELTA", ValCarr = "DL", Amt = 2100m, Trips = 3, SubAmt = 2100m, Avgcost = 700m, SubTrips=15,Amt2 = 1500.00m, Trips2 = 5, SubAmt2 = 300.00m, SubTrips2=10,Avgcost2 = 300.00m },
               new FinalData() { HomeCtry = "United States of America", Carrdesc = "UNITED AIRLINES", ValCarr = "UA", Amt = 600.00m, Trips = 2, SubAmt = 600.00m, Avgcost = 300.00m, SubTrips=15,Amt2 = 0m, Trips2 = 0, SubAmt2 = 0m, SubTrips2=10,Avgcost2 = 0m}
            };

            //Act
            var results = helper.GroupFinalData(final, isGraph, sort, nHowMany);

            //Assert
            for (int index = 0; index < results.Count; index++)
            {
                Assert.AreEqual(exp[index].HomeCtry, results[index].HomeCtry);
                Assert.AreEqual(exp[index].Amt, results[index].Amt);
                Assert.AreEqual(exp[index].Trips, results[index].Trips);
                Assert.AreEqual(exp[index].Avgcost, results[index].Avgcost);
                Assert.AreEqual(exp[index].SubAmt, results[index].SubAmt);
                Assert.AreEqual(exp[index].SubTrips, results[index].SubTrips);
                Assert.AreEqual(exp[index].Amt2, results[index].Amt2);
                Assert.AreEqual(exp[index].Trips2, results[index].Trips2);
                Assert.AreEqual(exp[index].Avgcost2, results[index].Avgcost2);
                Assert.AreEqual(exp[index].SubAmt2, results[index].SubAmt2);
                Assert.AreEqual(exp[index].SubTrips2, results[index].SubTrips2);
            }
        }

        [TestMethod]
        public void GroupFinalData_FinalListGroupByPageBread_ReturnListGroupByHomeCtry()
        {
            // Arrange
            FinalDataHelper helper = new FinalDataHelper(new ReportGlobals(), "Amt", DataTypes.GroupBy.HOME_COUNTRY_VALIDATING_CARRIER,DataTypes.SortBy.VOLUME_BOOKED);
            List<FinalData> final = new List<FinalData>{
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "DELTA", ValCarr = "DL", Amt = 2100m, Trips = 3, SubAmt = 2100m, Avgcost = 700m, SubTrips=15, Amt2 = 1500.00m, Trips2 = 5,  SubAmt2 = 300.00m, SubTrips2=10, Avgcost2 = 300.00m },
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "AMERICAN AIRLINES", ValCarr = "AA", Amt = 2200.00m, Trips = 10, SubAmt = 2200.00m, Avgcost = 220.00m, SubTrips=15,Amt2 = 1500.00m, Trips2 = 5, SubAmt2 = 1500.00m, SubTrips2=10,Avgcost2 = 300.00m },
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "UNITED AIRLINES", ValCarr = "UA", Amt = 600.00m, Trips = 2, SubAmt = 600.00m, Avgcost = 300.00m, SubTrips=15,Amt2 = 0m, Trips2 = 0, SubAmt2 = 0m, SubTrips2=10,Avgcost2 = 0m }
            };
            bool isGraph = false;
            DataTypes.Sort sort = DataTypes.Sort.Descending;
            var nHowMany = 10;

            List<FinalData> exp = new List<FinalData>{
               new FinalData() { HomeCtry = "United States of America", Carrdesc = "AMERICAN AIRLINES", ValCarr = "AA", Amt = 2200.00m, Trips = 10, SubAmt = 2200.00m, Avgcost = 220.00m, SubTrips=15,Amt2 = 1500.00m, Trips2 = 5, SubAmt2 = 1500.00m, SubTrips2=10,Avgcost2 = 300.00m },
               new FinalData() { HomeCtry = "United States of America", Carrdesc = "DELTA", ValCarr = "DL", Amt = 2100m, Trips = 3, SubAmt = 2100m, Avgcost = 700m, SubTrips=15,Amt2 = 1500.00m, Trips2 = 5, SubAmt2 = 300.00m, SubTrips2=10,Avgcost2 = 300.00m },
               new FinalData() { HomeCtry = "United States of America", Carrdesc = "UNITED AIRLINES", ValCarr = "UA", Amt = 600.00m, Trips = 2, SubAmt = 600.00m, Avgcost = 300.00m, SubTrips=15,Amt2 = 0m, Trips2 = 0, SubAmt2 = 0m, SubTrips2=10,Avgcost2 = 0m}
            };
            
            //Act
            var results = helper.GroupFinalData(final, isGraph, sort, nHowMany);

            //Assert
            for (int index = 0; index < results.Count; index++)
            {
                Assert.AreEqual(exp[index].HomeCtry, results[index].HomeCtry);
                Assert.AreEqual(exp[index].Amt, results[index].Amt);
                Assert.AreEqual(exp[index].Trips, results[index].Trips);
                Assert.AreEqual(exp[index].Avgcost, results[index].Avgcost);
                Assert.AreEqual(exp[index].SubAmt, results[index].SubAmt);
                Assert.AreEqual(exp[index].SubTrips, results[index].SubTrips);
                Assert.AreEqual(exp[index].Amt2, results[index].Amt2);
                Assert.AreEqual(exp[index].Trips2, results[index].Trips2);
                Assert.AreEqual(exp[index].Avgcost2, results[index].Avgcost2);
                Assert.AreEqual(exp[index].SubAmt2, results[index].SubAmt2);
                Assert.AreEqual(exp[index].SubTrips2, results[index].SubTrips2);
            }
        }
        
        //[TestMethod]
        //public void GroupFinalData_GroupByValCarrHomeCtrySortByCarrierHomeDescending_RetureValidatingThenHomeCountry

    }
}
