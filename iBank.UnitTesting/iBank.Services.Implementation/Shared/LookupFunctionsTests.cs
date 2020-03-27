using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iBank.Services.Implementation.Shared;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared
{
    [TestClass]
    public class LookupFunctionsTests
    {
        [TestMethod]
        public void LookupTwoDateGroup_PassDatesTwodaysApart_ReturnMatch()
        {
            //Arrange
            DateTime date1 = new DateTime(2017, 5, 1);
            DateTime date2 = date1.AddDays(2);

            var exp = "0-2 Days";
            //Act
            var sut = LookupFunctions.LookupTwoDateGroup(date1, date2);

            //Assert
            Assert.AreEqual(sut, exp);
        }


        [TestMethod]
        public void LookupTwoDateGroup_PassDatesThreedaysApart_ReturnMatch()
        {
            //Arrange
            DateTime date1 = new DateTime(2017, 2, 28);
            DateTime date2 = date1.AddDays(3);

            var exp = "3-6 Days";
            //Act
            var sut = LookupFunctions.LookupTwoDateGroup(date1, date2);

            //Assert
            Assert.AreEqual(sut, exp);
        }


        [TestMethod]
        public void LookupTwoDateGroup_PassDates7daysApart_ReturnMatch()
        {
            //Arrange
            DateTime date1 = new DateTime(2017, 2, 28);
            DateTime date2 = date1.AddDays(7);

            var exp = "7-13 Days";
            //Act
            var sut = LookupFunctions.LookupTwoDateGroup(date1, date2);

            //Assert
            Assert.AreEqual(sut, exp);
        }

        [TestMethod]
        public void LookupTwoDateGroup_PassDates14daysApart_ReturnMatch()
        {
            //Arrange
            DateTime date1 = new DateTime(2017, 2, 28);
            DateTime date2 = date1.AddDays(14);

            var exp = "14-20 Days";
            //Act
            var sut = LookupFunctions.LookupTwoDateGroup(date1, date2);

            //Assert
            Assert.AreEqual(sut, exp);
        }


        [TestMethod]
        public void LookupTwoDateGroup_PassDates21daysApart_ReturnMatch()
        {
            //Arrange
            DateTime date1 = new DateTime(2017, 2, 28);
            DateTime date2 = date1.AddDays(21);

            var exp = "21+ Days";
            //Act
            var sut = LookupFunctions.LookupTwoDateGroup(date1, date2);

            //Assert
            Assert.AreEqual(sut, exp);
        }

        //[TestMethod]
        //public void LookupTicketIssued_PassEReturn0_ResultMatch()
        //{
        //    var sut = "E";
        //    var exp = "0";

        //    var act = LookupFunctions.LookupTicketIssued(sut);

        //    Assert.AreEqual(exp, act);
        //}

    }
}
