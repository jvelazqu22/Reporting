using Domain.Models.ReportPrograms.TopBottomValidatingCarriers;
using iBank.Services.Implementation.ReportPrograms.TopBottomValidatingCarriers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.TopBottomValidatingCarriers
{
    [TestClass]
    public class SubReportDataTests
    {
        [TestMethod]
        public void SetSubReportData_TakeFinalData_ReturnCtrySubReportDataInCtrys()
        {// Arrange
            SubReportDataHelper helper = new SubReportDataHelper();

            List<FinalData> final = new List<FinalData>{
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "UNITED AIRLINES", ValCarr = "UA", Amt = 2200.00m, Trips = 10, SubAmt = 3200.00m, Avgcost = 220.00m, Amt2 = 3000.00m, Trips2 = 15, SubAmt2 = 4500.00m, Avgcost2 = 200.00m },
                new FinalData() { HomeCtry = "United States of America", Carrdesc = "AMERICAN AIRLINES", ValCarr = "AA", Amt = 1100.00m, Trips = 5, SubAmt = 3200.00m, Avgcost = 200.00m, Amt2 = 1500.00m, Trips2 = 5, SubAmt2 = 4500.00m, Avgcost2 = 300.00m },
                new FinalData() { HomeCtry = "Canada", Carrdesc = "UNITED AIRLINES", ValCarr = "UA", Amt = 600.00m, Trips = 2, SubAmt = 600.00m, Avgcost = 300.00m, Amt2 = 700.00m, Trips2 = 2, SubAmt2 = 700.00m, Avgcost2 = 350.00m }
            };

            List<SubReportData> exp = new List<SubReportData>
            {
               new SubReportData() { HomeCtry = "United States of America",  Amt = 3300.00m, Trips = 15, Avgcost = 220.00m, Amt2 = 4500.00m, Trips2 = 20,  Avgcost2 = 225.00m },
               new SubReportData() { HomeCtry = "Canada", Amt = 600.00m, Trips = 2, Avgcost = 300.00m, Amt2 = 700.00m, Trips2 = 2,Avgcost2 = 350.00m }
            };
            //Act
            var results = helper.SetSubReportData(final);

            //Assert
            for (int index = 0; index < results.Count; index++)
            {
                Assert.AreEqual(exp[index].HomeCtry, results[index].HomeCtry);
                Assert.AreEqual(exp[index].Amt, results[index].Amt);
                Assert.AreEqual(exp[index].Trips, results[index].Trips);
                Assert.AreEqual(exp[index].Avgcost, results[index].Avgcost);
                Assert.AreEqual(exp[index].Amt2, results[index].Amt2);
                Assert.AreEqual(exp[index].Trips2, results[index].Trips2);
                Assert.AreEqual(exp[index].Avgcost2, results[index].Avgcost2);
            }
        }
    }
}
