using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain.Models.ReportPrograms.TopBottomTravelersCar;
using System.Collections.Generic;

using iBank.Services.Implementation.ReportPrograms.TopBottomTravelerCar;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.TopBottomTravelerCar
{
    // Unit test naming convention:
    // [UnitOfWork_StateUnderTest_ExpectedBehavior]
    // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown

    [TestClass]
    public class CarTopBottomTravelerFinalDataCalculatorTest
    {
        [TestMethod]
        public void GetFinalDataFromRawData_RawDataList_GetFinalListGroupByAcct()
        {
            // Arrange
            var result = new List<FinalData>();
            var finalData = new FinalData();

            var rawDataList = new List<RawData>()
            {
                new RawData() { Passlast = "Velazquez", Passfrst = "Jorge", ABookRat = 1, Rentals = 2, Days = 3, CarCost = 4, sumbkrate = 5},
                new RawData() { Passlast = "Velazquez", Passfrst = "Jorge", ABookRat = 1, Rentals = 2, Days = 3, CarCost = 4, sumbkrate = 5},
                new RawData() { Passlast = "Velazquez", Passfrst = "Jorge", ABookRat = 1, Rentals = 2, Days = 3, CarCost = 4, sumbkrate = 5},
                new RawData() { Passlast = "Velazquez", Passfrst = "Jorge", ABookRat = 1, Rentals = 2, Days = 3, CarCost = 4, sumbkrate = 5},
            };

            // Act 
            result = new CarTopBottomTravelerFinalDataCalculator().GetFinalDataFromRawData(rawDataList);

            // Assert
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void GetFinalListTotalRate_FinalListWithSumBkRate_ReturnsSumOfAllSuBkRates()
        {
            // Arrange
            decimal result = 0;
            List<FinalData> finalData = new List<FinalData>()
            {
                new FinalData() { Bookrate = 1 },
                new FinalData() { Bookrate = 1 },
                new FinalData() { Bookrate = 1 },
                new FinalData() { Bookrate = 1 },
            };

            // Act 
            result = new CarTopBottomTravelerFinalDataCalculator().GetFinalListTotalRate(finalData);

            // Assert
            Assert.AreEqual(4, result);
        }

        [TestMethod]
        public void GetFinalListTotalBookCount_FinalListBookCnt_ReturnsSumOfAllBookCnt()
        {
            // Arrange
            decimal result = 0;
            List<FinalData> finalData = new List<FinalData>()
            {
                new FinalData() { Bookcnt = 1 },
                new FinalData() { Bookcnt = 1 },
                new FinalData() { Bookcnt = 1 },
                new FinalData() { Bookcnt = 1 },
            };

            // Act 
            result = new CarTopBottomTravelerFinalDataCalculator().GetFinalListTotalBookCount(finalData);

            // Assert
            Assert.AreEqual(4, result);
        }
    }
}
