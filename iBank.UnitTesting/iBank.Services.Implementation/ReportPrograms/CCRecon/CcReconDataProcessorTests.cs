using System;
using iBank.Services.Implementation.ReportPrograms.CCRecon;
using Domain.Models.ReportPrograms.CCReconReport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.CCRecon
{
    [TestClass]
    public class CcReconDataProcessorTests
    {
        [TestMethod]
        public void SortFinalData_OrderBy1_()
        {
            //Arrange
            var tester = new CcReconSortHandler();
            var finalDataList = new List<FinalData> {
                new FinalData{ Acct ="b", Break1 = "b1", Break2="b2", Break3="b3", Cardnum="c", Airlinenbr="0" },
                new FinalData { Acct = "a", Break1 = "b1", Break2 = "b2", Break3 = "b3", Cardnum = "c", Airlinenbr = "8" }
            };

            var expected = new List<FinalData>
            {
                new FinalData{ Acct ="a", Break1 = "b1", Break2="b2", Break3="b3", Cardnum="c", Airlinenbr="8" },
                new FinalData { Acct = "b", Break1 = "b1", Break2 = "b2", Break3 = "b3", Cardnum = "c", Airlinenbr = "0" }
            };

            //Act
            var output = tester.SortFinalData(finalDataList, "1",true, false);

            //Assert
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].Acct, output[i].Acct);
            }
        }

        [TestMethod]
        public void SortFinalData_OrderBy1_Airlinenbr()
        {
            //Arrange
            var tester = new CcReconSortHandler();
            var finalDataList = new List<FinalData> {
                new FinalData{ Acct ="a", Break1 = "b1", Break2="b2", Break3="b3", Cardnum="c", Airlinenbr="890", Ticket = "0718048641" },
                new FinalData { Acct = "a", Break1 = "b1", Break2 = "b2", Break3 = "b3", Cardnum = "c", Airlinenbr = "006", Ticket="7009899734"}
            };

            var expected = new List<FinalData>
            {
                new FinalData{ Acct ="a", Break1 = "b1", Break2="b2", Break3="b3", Cardnum="c", Airlinenbr="006", Ticket="7009899734" },
                new FinalData { Acct = "a", Break1 = "b1", Break2 = "b2", Break3 = "b3", Cardnum = "c", Airlinenbr = "890", Ticket = "0718048641" }
            };

            //Act
            var output = tester.SortFinalData(finalDataList, "1",true, false);

            //Assert
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].Airlinenbr, output[i].Airlinenbr);
                Assert.AreEqual(expected[i].Ticket, output[i].Ticket);
                Assert.AreNotEqual(expected[i].Ticket, finalDataList[i].Ticket);
            }
        }

        [TestMethod]
        public void SortFinalData_OrderBy1_Cardnum()
        {
            //Arrange
            var tester = new CcReconSortHandler();
            var finalDataList = new List<FinalData> {
                new FinalData { Acct = "a", Break1 = "b1", Break2 = "b2", Break3 = "b3", Cardnum = "c2", Airlinenbr = "006", Ticket="7009899734"},
                new FinalData{ Acct ="a", Break1 = "b1", Break2="b2", Break3="b3", Cardnum="c1", Airlinenbr="890", Ticket = "0718048641" }
            };

            var expected = new List<FinalData>
            {
                new FinalData { Acct = "a", Break1 = "b1", Break2 = "b2", Break3 = "b3", Cardnum = "c1", Airlinenbr = "890", Ticket = "0718048641" },
                new FinalData{ Acct ="a", Break1 = "b1", Break2="b2", Break3="b3", Cardnum="c2", Airlinenbr="006", Ticket="7009899734" }
            };

            //Act
            var output = tester.SortFinalData(finalDataList, "1", true, false);

            //Assert
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].Airlinenbr, output[i].Airlinenbr);
                Assert.AreEqual(expected[i].Ticket, output[i].Ticket);
                Assert.AreEqual(expected[i].Cardnum, output[i].Cardnum);
            }
        }

        [TestMethod]
        public void SortFinalData_OrderBy1_OnlyDisplayBreaksPerUserSettingIsTrue_Cardnum()
        {
            //Arrange
            var tester = new CcReconSortHandler();
            var finalDataList = new List<FinalData> {
                new FinalData { Acct = "ab", Break1 = "b1", Break2 = "b2", Break3 = "b3", Cardnum = "c2", Airlinenbr = "006", Ticket="7009899734"},
                new FinalData{ Acct ="ax", Break1 = "b1", Break2="b2", Break3="b3", Cardnum="c1", Airlinenbr="890", Ticket = "0718048641" }
            };

            var expected = new List<FinalData>
            {
                new FinalData { Acct = "ax", Break1 = "b1", Break2 = "b2", Break3 = "b3", Cardnum = "c1", Airlinenbr = "890", Ticket = "0718048641" },
                new FinalData{ Acct ="ab", Break1 = "b1", Break2="b2", Break3="b3", Cardnum="c2", Airlinenbr="006", Ticket="7009899734" }
            };

            //Act
            var output = tester.SortFinalData(finalDataList, "1", true, true);

            //Assert
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].Airlinenbr, output[i].Airlinenbr);
                Assert.AreEqual(expected[i].Ticket, output[i].Ticket);
                Assert.AreEqual(expected[i].Cardnum, output[i].Cardnum);
            }
        }

        [TestMethod]
        public void SortFinalData_SortByTransactionDate_UseAcctBrksIsFalse_IncludeBreaks()
        {
            //Arrange
            var tester = new CcReconSortHandler();
            var finalDataList = new List<FinalData> {
                new FinalData { Acct = "a", Break1 = "b1", Break2 = "b2", Break3 = "b3", Trandate = new DateTime(2018,10,31), Airlinenbr = "006", Ticket="7009899734"},
                new FinalData{ Acct ="a", Break1 = "b1", Break2="b2", Break3="b3", Trandate = new DateTime(2018,10,31), Airlinenbr="890", Ticket = "0718048641" }
            };

            var expected = new List<FinalData>
            {
                new FinalData { Acct = "a", Break1 = "b1", Break2 = "b2", Break3 = "b3", Trandate = new DateTime(2018,10,31), Airlinenbr = "890", Ticket = "0718048641" },
                new FinalData{ Acct ="a", Break1 = "b1", Break2="b2", Break3="b3", Trandate = new DateTime(2018,10,31), Airlinenbr="006", Ticket="7009899734" }
            };

            //Act
            var output = tester.SortFinalData(finalDataList, "5", false, false);

            //Assert
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].Acct, output[i].Acct);
                Assert.AreEqual(expected[i].Ticket, output[i].Ticket);
                Assert.AreEqual(expected[i].Trandate, output[i].Trandate);
            }
        }

        [TestMethod]
        public void SortFinalData_SortByTransactionDate_OnlyDisplayBreaksPerUserSettingIsTrue_IncludeBreaks()
        {
            //Arrange
            var tester = new CcReconSortHandler();
            var finalDataList = new List<FinalData> {
                new FinalData { Acct = "ax", Break1 = "b1", Break2 = "bx2", Break3 = "b3", Trandate = new DateTime(2018,9,30), Airlinenbr = "006", Ticket="7009899734"},
                new FinalData{ Acct ="ab", Break1 = "b1", Break2="ba2", Break3="b3", Trandate = new DateTime(2018,10,31), Airlinenbr="890", Ticket = "9718048641" },
                new FinalData{ Acct ="ab", Break1 = "b1", Break2="ba2", Break3="b3", Trandate = new DateTime(2018,3,31), Airlinenbr="890", Ticket = "0718048641" }
            };

            var expected = new List<FinalData>
            {
                new FinalData{ Acct ="ab", Break1 = "b1", Break2="ba2", Break3="b3", Trandate = new DateTime(2018,3,31), Airlinenbr="890", Ticket = "0718048641" },
                new FinalData { Acct = "ax", Break1 = "b1", Break2 = "bx2", Break3 = "b3", Trandate = new DateTime(2018,9,30), Airlinenbr = "006", Ticket="7009899734"},
                new FinalData{ Acct ="ab", Break1 = "b1", Break2="ba2", Break3="b3", Trandate = new DateTime(2018,10,31), Airlinenbr="890", Ticket = "9718048641" }
            };

            //Act
            var output = tester.SortFinalData(finalDataList, "5", false, true);

            //Assert
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].Airlinenbr, output[i].Airlinenbr);
                Assert.AreEqual(expected[i].Ticket, output[i].Ticket);
                Assert.AreEqual(expected[i].Trandate, output[i].Trandate);
            }
        }

    }
}
