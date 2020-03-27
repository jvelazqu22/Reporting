using System;
using System.Collections.Generic;
using iBank.Services.Implementation.ReportPrograms.UserDefined;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserDefinedReports.Classes;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport
{
    [TestClass]
    public class CustomColumnMapperTests
    {
        [TestMethod]
        public void BuildCustomColumn_PassColumnTypeCurrency_ReturnIsDecimalIsTrue()
        {
            //Arrange
            var column = new UserReportColumnInformation
            {
                Name = "N",
                Order = 1,
                Sort = 1,
                GroupBreak = 0,
                SubTotal = false,
                TotalThisField = false,
                PageBreak = false,
                TableName = "TRIP",
                Header1 = "H1",
                Header2 = "H2",
                Width = 4,
                UdidType = 0,
                HorizontalAlignment ="1",
                GoodField = "N",
                GoodFieldType = "",
                GoodHilite = "",
                GoodOperator = "=",
                GoodValue = "",
                BadHilite = "N",
                BadValue = "",
                BadOperator = "",
                ColumnType = "Currency"
            };
            var exp = new CustomColumnInformation {Header = "H1" + Environment.NewLine + "H2", IsDecimal = true,};
            
            //Act
            var mapper = new CustomColumnMapper();
            var act = mapper.BuildCustomColumn(column, "CLASSIC");

            //Assert
            Assert.AreEqual(exp.Header, act.Header);
            Assert.AreEqual(exp.IsDecimal, act.IsDecimal);
        }

        [TestMethod]
        public void BuildCustomColumn_PassColumnTypeNumber_ReturnIsDecimalIsFalse()
        {
            //Arrange
            var column = new UserReportColumnInformation
            {
                Name = "N",
                Order = 1,
                Sort = 1,
                GroupBreak = 0,
                SubTotal = false,
                TotalThisField = false,
                PageBreak = false,
                TableName = "TRIP",
                Header1 = "H1",
                Header2 = "H2",
                Width = 4,
                UdidType = 0,
                HorizontalAlignment = "1",
                GoodField = "N",
                GoodFieldType = "",
                GoodHilite = "",
                GoodOperator = "=",
                GoodValue = "",
                BadHilite = "N",
                BadValue = "",
                BadOperator = "",
                ColumnType = "number"
            };
            var exp = new CustomColumnInformation { Header = "H1" + Environment.NewLine + "H2", IsDecimal = false, };

            //Act
            var mapper = new CustomColumnMapper();
            var act = mapper.BuildCustomColumn(column, "CLASSIC");

            //Assert
            Assert.AreEqual(exp.Header, act.Header);
            Assert.AreEqual(exp.IsDecimal, act.IsDecimal);
        }

        [TestMethod]
        public void BuildCustomColumn_PassColumnTypeNumber_ReturnIsIntegerIsTrue()
        {
            //Arrange
            var column = new UserReportColumnInformation
            {
                Name = "N",
                Order = 1,
                Sort = 1,
                GroupBreak = 0,
                SubTotal = false,
                TotalThisField = false,
                PageBreak = false,
                TableName = "TRIP",
                Header1 = "H1",
                Header2 = "H2",
                Width = 4,
                UdidType = 0,
                HorizontalAlignment = "1",
                GoodField = "N",
                GoodFieldType = "",
                GoodHilite = "",
                GoodOperator = "=",
                GoodValue = "",
                BadHilite = "N",
                BadValue = "",
                BadOperator = "",
                ColumnType = "number"
            };
            var exp = new CustomColumnInformation { Header = "H1" + Environment.NewLine + "H2", IsInteger = true, };

            //Act
            var mapper = new CustomColumnMapper();
            var act = mapper.BuildCustomColumn(column, "CLASSIC");

            //Assert
            Assert.AreEqual(exp.Header, act.Header);
            Assert.AreEqual(exp.IsDecimal, act.IsDecimal);
        }

        [TestMethod]
        public void BuildCustomColumn_PassTableNameHotel_ReturnIsTripFieldIsFalse()
        {
            //Arrange
            var column = new UserReportColumnInformation
            {
                Name = "N",
                Order = 1,
                Sort = 1,
                GroupBreak = 0,
                SubTotal = false,
                TotalThisField = false,
                PageBreak = false,
                TableName = "Hotel",
                Header1 = "H1",
                Header2 = "H2",
                Width = 4,
                UdidType = 0,
                HorizontalAlignment = "1",
                GoodField = "N",
                GoodFieldType = "",
                GoodHilite = "",
                GoodOperator = "=",
                GoodValue = "",
                BadHilite = "N",
                BadValue = "",
                BadOperator = "",
                ColumnType = "number"
            };
            var exp = new CustomColumnInformation { Header = "H1" + Environment.NewLine + "H2", IsTripField = false, };

            //Act
            var mapper = new CustomColumnMapper();
            var act = mapper.BuildCustomColumn(column, "CLASSIC");

            //Assert
            Assert.AreEqual(exp.Header, act.Header);
            Assert.AreEqual(exp.IsTripField, act.IsTripField);
        }
    }
}
