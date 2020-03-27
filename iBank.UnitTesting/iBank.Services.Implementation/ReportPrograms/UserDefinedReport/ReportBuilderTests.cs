using System;
using System.Collections.Generic;
using System.Globalization;

using Domain.Models.ReportPrograms.UserDefinedReport;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserDefinedReports;
using UserDefinedReports.Classes;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport
{
    [TestClass]
    public class ReportBuilderTests
    {
        [TestMethod]
        public void GetValueAsString_PassPnrcrdtgmtDateTimeReturnDateTime_ResultMatch()
        {
            //Arrange
            var rawData = new RawData
            {
                Recloc = "ABCDEF",
                Agency = "DEMO",
                Airchg = 23.23m,
                Pnrcrdtgmt = new DateTime(2016, 12, 29, 2,22,22)
            };
            var act = "12/29/2016 02:22:22";

            //Act
            var exp = ReportBuilder.GetValueAsString(rawData, "PNRCRDTGMT");
            //Assert
            Assert.AreEqual(exp,act);
        }

        [TestMethod]
        public void GetValueAsString_PassStringReturnString_ResultMatch()
        {
            //Arrange
            var rawData = new RawData
            {
                Recloc = "ABCDEF",
                Agency = "DEMO",
                Airchg = 23.23m,
                Arrdate = new DateTime(2016, 12, 29, 2, 22, 22)

            };
            var act = "ABCDEF";

            //Act
            var exp = ReportBuilder.GetValueAsString(rawData, "Recloc");
            //Assert
            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void GetValueAsString_PassDecimalReturnDecimalString_ResultMatch()
        {
            //Arrange
            var rawData = new RawData
            {
                Recloc = "ABCDEF",
                Agency = "DEMO",
                Airchg = 23.23m,
                Arrdate = new DateTime(2016, 12, 29, 2, 22, 22)

            };
            var act = "23.23";

            //Act
            var exp = ReportBuilder.GetValueAsString(rawData, "Airchg");
            //Assert
            Assert.AreEqual(exp, act);
        }
        
        [TestMethod]
        public void GetValueAsString_PassNullableDateTimeReturnNullDateTimeString_ResultMatch()
        {
            //Arrange
            var rawData = new RawData
            {
                Recloc = "ABCDEF",
                Agency = "DEMO",
                Airchg = 23.23m

            };
            var act = "01/01/0001 00:00:00";

            //Act
            var exp = ReportBuilder.GetValueAsString(rawData, "Arrdate");
            //Assert
            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void GetValueAsString_PassNullableDecimalReturnDedaultDecimal_ResultMatch()
        {
            //Arrange
            var rawData = new RawData
            {
                Recloc = "ABCDEF",
                Agency = "DEMO",
                Airchg = 23.23m

            };
            var act = "0.00";

            //Act
            var exp = ReportBuilder.GetValueAsString(rawData, "Svcfee");
            //Assert
            Assert.AreEqual(exp, act);
        }



        [TestMethod]
        public void GetValueAsString_PassNullableIntergeReturnDefaultIntergeZero_ResultMatch()
        {
            //Arrange
            var rawData = new RawData
            {
                Recloc = "ABCDEF",
                Agency = "DEMO",
                Airchg = 23.23m,
                Arrdate = new DateTime(2016, 12, 29, 2, 22, 22)

            };
            var act = "0";

            //Act
            var exp = ReportBuilder.GetValueAsString(rawData, "TrpAuxSeq");
            //Assert
            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void GetValueAsString_PassRawDataAndNullableIntergeHasValueReturnStringValue_ResultMatch()
        {
            //Arrange
            var rawData = new RawData
            {
                Recloc = "ABCDEF",
                Agency = "DEMO",
                Airchg = 23.23m,
                TrpAuxSeq = 2,
                Arrdate = new DateTime(2016, 12, 29, 2, 22, 22)

            };
            var act = "2";

            //Act
            var exp = ReportBuilder.GetValueAsString(rawData, "TrpAuxSeq");
            //Assert
            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void GetValueAsString_PassNullableDateTimeWithValueReturnStringWithFormatDisplay_ResultMatch()
        {
            //Arrange
            var rawData = new RawData
            {
                Recloc = "ABCDEF",
                Agency = "DEMO",
                Airchg = 23.23m,
                TrpAuxSeq = 2,
                Arrdate = new DateTime(2016, 12, 29, 2, 22, 22)

            };
            var act = "12/29/2016 02:22:22 AM";
            var format = "MM/dd/yyyy hh:mm:ss tt";
            //Act
            var exp = ReportBuilder.GetValueAsString(rawData, "Arrdate");
            var expFormat = Convert.ToDateTime(exp);
            //Assert
            Assert.AreEqual(expFormat.ToString(format, CultureInfo.InvariantCulture), act);
        }

        [TestMethod]
        public void GetValueAsString_PassIntergeValueReturnIntergerString_ResultMatch()
        {
            //Arrange
            var rawData = new FinalData()
            {
                RecKey = 12345,
            };
            var act = "12345";

            //Act
            var exp = ReportBuilder.GetValueAsString(rawData, "RecKey");
            //Assert
            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void DetermineFontAndPageSize_ColumnsWidthLess7Inch_MatchFirstSetting()
        {
            //Arrange
            var reportBuilder = new ReportBuilder
            {
                Columns = new List<CustomColumnInformation>
                {
                    new CustomColumnInformation {Width = 10},
                    new CustomColumnInformation {Width = 10}
                }
            };
            
            var actFont = GetPageParameter(0);

            //Act
            reportBuilder.SetPageParameter();

            //Assert
            Assert.AreEqual(reportBuilder.PageParameter.FontSize, actFont.FontSize);
            Assert.AreEqual(reportBuilder.PageParameter.FontConverter, actFont.FontConverter);
            Assert.AreEqual(reportBuilder.PageParameter.CellHeight, actFont.CellHeight);
            Assert.AreEqual(reportBuilder.PageParameter.PageWidth, actFont.PageWidth);
            Assert.AreEqual(reportBuilder.PageParameter.PageHeight, actFont.PageHeight);
            Assert.AreEqual(reportBuilder.PageParameter.MaxContentWidth, actFont.MaxContentWidth);
        }

        [TestMethod]
        public void DetermineFontAndPageSize_ColumnsWidth100_MatchSecondetting()
        {
            //Arrange
            var reportBuilder = new ReportBuilder
            {
                Columns = new List<CustomColumnInformation>
                {
                    new CustomColumnInformation {Width = 100}
                }
            };

            var actFont = GetPageParameter(1);

            //Act
            reportBuilder.SetPageParameter();

            //Assert
            Assert.AreEqual(reportBuilder.PageParameter.FontSize, actFont.FontSize);
            Assert.AreEqual(reportBuilder.PageParameter.FontConverter, actFont.FontConverter);
            Assert.AreEqual(reportBuilder.PageParameter.CellHeight, actFont.CellHeight);
            Assert.AreEqual(reportBuilder.PageParameter.PageWidth, actFont.PageWidth);
            Assert.AreEqual(reportBuilder.PageParameter.PageHeight, actFont.PageHeight);
            Assert.AreEqual(reportBuilder.PageParameter.MaxContentWidth, actFont.MaxContentWidth);
        }

        [TestMethod]
        public void DetermineFontAndPageSize_ColumnsWidth200_MatchLastSetting()
        {
            //Arrange
            var reportBuilder = new ReportBuilder
            {
                Columns = new List<CustomColumnInformation>
                {
                    new CustomColumnInformation {Width = 100},
                    new CustomColumnInformation {Width = 100}
                }
            };

            var actFont = GetPageParameter(2);

            //Act
            reportBuilder.SetPageParameter();

            //Assert
            Assert.AreEqual(reportBuilder.PageParameter.FontSize, actFont.FontSize);
            Assert.AreEqual(reportBuilder.PageParameter.FontConverter, actFont.FontConverter);
            Assert.AreEqual(reportBuilder.PageParameter.CellHeight, actFont.CellHeight);
            Assert.AreEqual(reportBuilder.PageParameter.PageWidth, actFont.PageWidth);
            Assert.AreEqual(reportBuilder.PageParameter.PageHeight, actFont.PageHeight);
            Assert.AreEqual(reportBuilder.PageParameter.MaxContentWidth, actFont.MaxContentWidth);
        }


        [TestMethod]
        public void DetermineFontAndPageSize_ColumnsWidth300_MatchLastSetting()
        {
            //Arrange
            var reportBuilder = new ReportBuilder
            {
                Columns = new List<CustomColumnInformation>
                {
                    new CustomColumnInformation {Width = 100},
                    new CustomColumnInformation {Width = 100},
                    new CustomColumnInformation {Width = 100}
                }
            };

            var actFont = GetPageParameter(2);

            //Act
            reportBuilder.SetPageParameter();

            //Assert
            Assert.AreEqual(reportBuilder.PageParameter.FontSize, actFont.FontSize);
            Assert.AreEqual(reportBuilder.PageParameter.FontConverter, actFont.FontConverter);
            Assert.AreEqual(reportBuilder.PageParameter.CellHeight, actFont.CellHeight);
            Assert.AreEqual(reportBuilder.PageParameter.PageWidth, actFont.PageWidth);
            Assert.AreEqual(reportBuilder.PageParameter.PageHeight, actFont.PageHeight);
            Assert.AreEqual(reportBuilder.PageParameter.MaxContentWidth, actFont.MaxContentWidth);
        }

        [TestMethod]
        public void OrderEachColumn_()
        {
            //Arrange
            var reportBuilder = new ReportBuilder
            {
                Columns = new List<CustomColumnInformation>
                {
                    new CustomColumnInformation {Width = 1},
                    new CustomColumnInformation {Width = 6},
                    new CustomColumnInformation {Width = 10}
                }
            };
            var actColumns = new List<CustomColumnInformation>
            {
                new CustomColumnInformation {Width = 4, Order = 1},
                new CustomColumnInformation {Width = 6, Order = 2},
                new CustomColumnInformation {Width = 10, Order = 3}
            };

            //Act
            reportBuilder.OrderEachColumn();

            //Assert
            for (var i=0; i< actColumns.Count; i++)
            {
                Assert.AreEqual(reportBuilder.Columns[i].Width, actColumns[i].Width);
                Assert.AreEqual(reportBuilder.Columns[i].Order, actColumns[i].Order);
            }
        }

        private PageParameter GetPageParameter(int idx)
        {
            var fontConversions = new List<PageParameter>
            {
                new PageParameter(8.75, 12.5, 0.15, 8.5, 11, 7.0),
                new PageParameter(8.10, 14, 0.12, 11, 8.5, 10),
                new PageParameter(5.4, 18.0, 0.10, 14, 8.5, 13 )
            };
            return fontConversions[idx];
        }
    }
}
