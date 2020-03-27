using Domain.Helper;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport.Managers   
{
    [TestClass]
    public class TwoValuesCompareManagerTest
    {
        [TestMethod]
        public void TwoValuesCompareManager_PassTwoMatchingStringTestEqual_ReturnTrue()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("ABC", "ABC", Operator.Equal, "string");

            //Assert
            Assert.IsTrue(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassABCInFirstStringTestEmpty_ReturnFalse()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("ABC", "", Operator.Empty, "");

            //Assert
            Assert.IsFalse(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassJan2ndTestBetweenJan1And3rd_ReturnTrue()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("2016-01-02", "2016-01-01", Operator.Between, "DATE",  "2016-01-03");

            //Assert
            Assert.IsTrue(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassDateJan2ndTestNotBetweenJan1And3rd_ReturnFalse()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("2016-01-02", "2016-01-01",Operator.NotBetween, "DATE", "2016-01-03");

            //Assert
            Assert.IsFalse(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassCurrency123TestBetween100And125_ReturnTrue()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("123.00", "100.00", Operator.Between, "CURRENCY", "125");

            //Assert
            Assert.IsTrue(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassCurrency123TestNotBetween100And125_ReturnFalse()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("123.00", "100.00", Operator.NotBetween, "CURRENCY", "125");

            //Assert
            Assert.IsFalse(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassNumber123TestBetween100And125_ReturnTrue()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("123", "100", Operator.Between, "Number", "125");

            //Assert
            Assert.IsTrue(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassNumber123TestNotBetween100And125_ReturnFalse()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("123", "100", Operator.NotBetween, "Number", "125");

            //Assert
            Assert.IsFalse(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassTwoNotMatchingStringTestEqual_ReturnFalse()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("ABC", "ABCS", Operator.Equal, "string");

            //Assert
            Assert.IsFalse(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassTwoStringABCDandABCTestGreaterOrEqual_ReturnTrule()
        {
            //Arrange

            //Act
            var exp = TwoValuesCompareManager.Compare("ABCD", "ABC", Operator.GreaterOrEqual, "TEXT");

            //Assert
            Assert.IsTrue(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassTwoStringABCDandABCTestLessthan_ReturnFalse()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("ABCD", "ABC", Operator.Lessthan, "TEXT");

            //Assert
            Assert.IsFalse(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassTwoEqualDatesTestDateTestGreaterThanOrEqual_ReturnTrue()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("5/2/2016", "5/2/2016", Operator.GreaterOrEqual, "DATE");

            //Assert
            Assert.IsTrue(exp);
        }


        [TestMethod]
        public void TwoValuesCompareManager_PassTwoEqualDatesTestDateTestGreaterThan_ReturnFalse()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("5/2/2016", "5/2/2016", Operator.GreaterThan, "DATE");

            //Assert
            Assert.IsFalse(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassTwoEqualDatesTestDateTestLessThan_ReturnFalse()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("5/2/2016", "5/2/2016", Operator.Lessthan, "DATE");

            //Assert
            Assert.IsFalse(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassTwoNumbers123And120TextGreaterThanOrEqual_ReturnTrue()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("123", "120", Operator.GreaterOrEqual, "NUMERIC");

            //Assert
            Assert.IsTrue(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassTwoNumbers120And120TextGreaterThan_ReturnFalse()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("120", "120", Operator.GreaterThan, "NUMERIC");

            //Assert
            Assert.IsFalse(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassTwoNumbers123And120TextLessThan_ReturnFalse()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("123", "120", Operator.Lessthan, "NUMERIC");

            //Assert
            Assert.IsFalse(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassTwoCurrency123And120TestGreaterThanOrEqual_ReturnTrue()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("123.00", "120.00", Operator.GreaterOrEqual, "CURRENCY");

            //Assert
            Assert.IsTrue(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassTwoCurrency120And120TestGreaterThan_ReturnFalse()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("120.00", "120.00", Operator.GreaterThan, "CURRENCY");

            //Assert
            Assert.IsFalse(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassTwoCurrency123And120TestLessThan_ReturnFalse()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("123.00", "120.00", Operator.Lessthan, "CURRENCY");

            //Assert
            Assert.IsFalse(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassTwoDateTimesTestGreaterOrEqual_RetureTrue()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("2016-10-01 12:23:00", "2016-10-01 12:00:00", Operator.GreaterOrEqual, "DATETIME");

            //Assert
            Assert.IsTrue(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassTwoDateTimesTestLessThan_ReturnFalse()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("2016-10-01 12:23:00", "2016-10-01 12:00:00", Operator.Lessthan, "DATETIME");

            //Assert
            Assert.IsFalse(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassStringABCAndListContainsABCandBCDTestInList_ReturnTrue()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("ABC", "ABC,BCD", Operator.InList, "string");

            //Assert
            Assert.IsTrue(exp);
        }

        [TestMethod]
        public void TwoValuesCompareManager_PassStringABCAndListContainsABCandBCDTestNotInList_ReturnFalse()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("ABC", "ABC,BCD", Operator.NotInList, "string");

            //Assert
            Assert.IsFalse(exp);
        }


        [TestMethod]
        public void TwoValuesCompareManager_PassStringBCAndABCDTestLike_ReturnTrue()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("BC", "ABCD", Operator.Like, "string");

            //Assert
            Assert.IsTrue(exp);
        }
        [TestMethod]
        public void TwoValuesCompareManager_PassStringBCAndABCDTestNotLike_ReturnFalse()
        {
            //Act
            var exp = TwoValuesCompareManager.Compare("BC", "ABCD", Operator.NotLike, "string");

            //Assert
            Assert.IsFalse(exp);
        }

    }
}
