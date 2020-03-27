using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using iBank.Services.Implementation.ReportPrograms.TopBottomValidatingCarriers;
using Domain.Models.ReportPrograms.TopBottomValidatingCarriers;


namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.TopBottomValidatingCarriers
{
    [TestClass]
    public class GraphFinalDataTests
    {
        
        [TestMethod]
        public void ConverToGraphFinalData_HomeCtry2DatesSortByTrips_ReturnDescAndData1Data2()
        {
            // Arrange
            GraphDataHelper helper = new GraphDataHelper();
            List<FinalData> final = new List<FinalData>{
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "UNITED AIRLINES", ValCarr = "UA", Amt = 2200.00m, Trips = 10, SubAmt = 3200.00m, Avgcost = 220.00m, Amt2 = 3000.00m, Trips2 = 15, SubAmt2 = 4500.00m, Avgcost2 = 200.00m },
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "AMERICAN AIRLINES", ValCarr = "AA", Amt = 1000.00m, Trips = 5, SubAmt = 3200.00m, Avgcost = 200.00m, Amt2 = 1500.00m, Trips2 = 5, SubAmt2 = 4500.00m, Avgcost2 = 300.00m },
                new FinalData() { HomeCtry = "Canada", Carrdesc = "UNITED AIRLINES", ValCarr = "UA", Amt = 600.00m, Trips = 2, SubAmt = 600.00m, Avgcost = 300.00m, Amt2 = 700.00m, Trips2 = 2, SubAmt2 = 700.00m, Avgcost2 = 350.00m }
            };
            bool isCtryCode = true;
            string orderBy = "Trips";

            List<GraphFinalData> expList = new List<GraphFinalData>
            {
                new GraphFinalData() {CatDesc = "United States of America", Data1 = 15m, Data2 = 20m, RecNumber=2  },
                new GraphFinalData() {CatDesc = "Canada", Data1 = 2m, Data2 = 2m, RecNumber=2  }
            };

            //Act
            var results = helper.ConvertToGraphFinalData(final, isCtryCode, orderBy);

            //Assert
            for (int index = 0; index < expList.Count; index++)
            {
                Assert.AreEqual(expList[index].Data1, results[index].Data1);
                Assert.AreEqual(expList[index].CatDesc, results[index].CatDesc);
            }
        }

        [TestMethod]
        public void ConverToGraphFinalData_Carrier2DatesSortByTrips_ReturnDescAndData1Data2()
        {
            // Arrange
            GraphDataHelper helper = new GraphDataHelper();
            List<FinalData> final = new List<FinalData>{
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "UNITED AIRLINES", ValCarr = "UA", Amt = 2200.00m, Trips = 10, SubAmt = 3200.00m, Avgcost = 220.00m, Amt2 = 3000.00m, Trips2 = 15, SubAmt2 = 4500.00m, Avgcost2 = 200.00m },
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "AMERICAN AIRLINES", ValCarr = "AA", Amt = 1000.00m, Trips = 5, SubAmt = 3200.00m, Avgcost = 200.00m, Amt2 = 1500.00m, Trips2 = 5, SubAmt2 = 4500.00m, Avgcost2 = 300.00m },
                new FinalData() { HomeCtry = "Canada", Carrdesc = "UNITED AIRLINES", ValCarr = "UA", Amt = 600.00m, Trips = 2, SubAmt = 600.00m, Avgcost = 300.00m, Amt2 = 700.00m, Trips2 = 2, SubAmt2 = 700.00m, Avgcost2 = 350.00m }
            };
            bool isCtryCode = false;
            string orderBy = "Trips";

            List<GraphFinalData> expList = new List<GraphFinalData>
            {
                new GraphFinalData() {CatDesc = "UNITED AIRLINES", Data1 = 12m, Data2 = 17m, RecNumber=2  },
                new GraphFinalData() {CatDesc = "AMERICAN AIRLINES", Data1 = 5m, Data2 = 5m, RecNumber=2  }
            };

            //Act
            var results = helper.ConvertToGraphFinalData(final, isCtryCode, orderBy);

            //Assert
            for (int index = 0; index < expList.Count; index++)
            {
                Assert.AreEqual(expList[index].Data1, results[index].Data1);
                Assert.AreEqual(expList[index].CatDesc, results[index].CatDesc);
            }
        }


        [TestMethod]
        public void ConverToGraphFinalData_Carrier2DatesSortByAmt2_ReturnDescAndData1Data2()
        {
            // Arrange
            GraphDataHelper helper = new GraphDataHelper();
            List<FinalData> final = new List<FinalData>{
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "UNITED AIRLINES", ValCarr = "UA", Amt = 2200.00m, Trips = 10, SubAmt = 2800.00m, Avgcost = 220.00m, Amt2 = 3000.00m, Trips2 = 15, SubAmt2 = 3700.00m, Avgcost2 = 200.00m },
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "AMERICAN AIRLINES", ValCarr = "AA", Amt = 1000.00m, Trips = 5, SubAmt = 1000.00m, Avgcost = 200.00m, Amt2 = 1500.00m, Trips2 = 5, SubAmt2 = 1500.00m, Avgcost2 = 300.00m },
                new FinalData() { HomeCtry = "Canada", Carrdesc = "UNITED AIRLINES", ValCarr = "UA", Amt = 600.00m, Trips = 2, SubAmt = 2800.00m, Avgcost = 300.00m, Amt2 = 700.00m, Trips2 = 2, SubAmt2 = 3700.00m, Avgcost2 = 350.00m }
            };
            bool isCtryCode = false;
            string orderBy = "Amt2";

            List<GraphFinalData> expList = new List<GraphFinalData>
            {
                new GraphFinalData() {CatDesc = "UNITED AIRLINES", Data1 = 2800m, Data2 = 3700m, RecNumber=2  },
                new GraphFinalData() {CatDesc = "AMERICAN AIRLINES", Data1 = 1000m, Data2 = 1500m, RecNumber=2  }
            };

            //Act
            var results = helper.ConvertToGraphFinalData(final, isCtryCode, orderBy);

            //Assert
            for (int index = 0; index < expList.Count; index++)
            {
                Assert.AreEqual(expList[index].Data1, results[index].Data1);
                Assert.AreEqual(expList[index].CatDesc, results[index].CatDesc);
            }
        }

        [TestMethod]
        public void ConverToGraphFinalData_HomeCtrySortByTrips_ReturnDescAndData1()
        {
            // Arrange
            GraphDataHelper helper = new GraphDataHelper();
            List<FinalData> final = new List<FinalData>{
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "UNITED AIRLINES", ValCarr = "UA", Amt = 2200.00m, Trips = 10, SubAmt = 3200.00m, Avgcost = 220.00m },
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "AMERICAN AIRLINES", ValCarr = "AA", Amt = 1000.00m, Trips = 5, SubAmt = 3200.00m, Avgcost = 200.00m },
                new FinalData() { HomeCtry = "Canada", Carrdesc = "UNITED AIRLINES", ValCarr = "UA", Amt = 600.00m, Trips = 2, SubAmt = 600.00m, Avgcost = 300.00m }
            };
            bool isCtryCode = true;
            string orderBy = "Trips";

            List<GraphFinalData> expList = new List<GraphFinalData>
            {
                new GraphFinalData() {CatDesc = "United States of America", Data1 = 15m, Data2 = 0m, RecNumber=2  },
                new GraphFinalData() {CatDesc = "Canada", Data1 = 2m, Data2 = 0m, RecNumber=2  }
            };

            //Act
            var results = helper.ConvertToGraphFinalData(final, isCtryCode, orderBy);

            //Assert
            for (int index = 0; index < expList.Count; index++)
            {
                Assert.AreEqual(expList[index].Data1, results[index].Data1);
                Assert.AreEqual(expList[index].CatDesc, results[index].CatDesc);
            }
        }

        [TestMethod]
        public void ConverToGraphFinalData_HomeCtrySortByAmt_ReturnDescAndData1()
        {
            // Arrange
            GraphDataHelper helper = new GraphDataHelper();
            List<FinalData> final = new List<FinalData>{
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "UNITED AIRLINES", ValCarr = "UA", Amt = 2200.00m, Trips = 10, SubAmt = 3200.00m, Avgcost = 220.00m },
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "AMERICAN AIRLINES", ValCarr = "AA", Amt = 1000.00m, Trips = 5, SubAmt = 3200.00m, Avgcost = 200.00m },
                new FinalData() { HomeCtry = "Canada", Carrdesc = "UNITED AIRLINES", ValCarr = "UA", Amt = 600.00m, Trips = 2, SubAmt = 600.00m, Avgcost = 300.00m }
            };
            bool isCtryCode = true;
            string orderBy = "Amt";

            List<GraphFinalData> expList = new List<GraphFinalData>
            {
                new GraphFinalData() {CatDesc = "United States of America", Data1 = 3200.00m, Data2 = 0m, RecNumber=2  },
                new GraphFinalData() {CatDesc = "Canada", Data1 = 600.00m, Data2 = 0m, RecNumber=2  }
            };

            //Act
            var results = helper.ConvertToGraphFinalData(final, isCtryCode, orderBy);

            //Assert
            for (int index = 0; index < expList.Count; index++)
            {
                Assert.AreEqual(expList[index].Data1, results[index].Data1);
                Assert.AreEqual(expList[index].CatDesc, results[index].CatDesc);
            }
        }


        [TestMethod]
        public void GetGraphTitle_SortByTrips_ReturnNoOfTrips()
        {
            // Arrange
            GraphDataHelper helper = new GraphDataHelper();
            DataTypes.SortBy sortBy = DataTypes.SortBy.NO_OF_TRIPS;

            var exp = "# of Trips";
            //Act
            var result = helper.GetGraphTitle(sortBy);
            //Assert
            Assert.AreEqual(exp, result);
        }
        [TestMethod]
        public void GetGraphDataType_SortByTrips_ReturnN()
        {
            // Arrange
            GraphDataHelper helper = new GraphDataHelper();
            DataTypes.SortBy sortBy = DataTypes.SortBy.NO_OF_TRIPS;

            var exp = "N";
            //Act
            var result = helper.GetGraphDataType(sortBy);
            //Assert
            Assert.AreEqual(exp, result);
        }

    }
}
