using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserDefinedReports;
using UserDefinedReports.Classes;
using System.Xml.Linq;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport
{
    [TestClass]
    public class CellHightLightManagerTests
    {
        private XNamespace Xmlns = ReportBuilder.Xmlns;

        [TestMethod]
        public void GetCellHighlight_GoodHighlightIsNAndBadHighlightIsY_ReturnIsNotNull()
        {
            //Arrange
            var tester = new CellHighLightManager();
            CustomColumnInformation column = new CustomColumnInformation()
            {
                GoodHighlight = "N",
                BadHighlight = "Y",
                Order = 1
            };

            //Act
            var exp = tester.GetCellHighlight(column);            

            //Assert
            Assert.IsNotNull(exp);
        }

        [TestMethod]
        public void GetCellHighlight_GoodHighlightAndBadHighlightAreN_ReturnNull()
        {
            //Arrange
            var tester = new CellHighLightManager();
            CustomColumnInformation column = new CustomColumnInformation()
            {
                GoodHighlight = "N",
                BadHighlight = "N",
                Order = 1
            };

            //Act
            var exp = tester.GetCellHighlight(column);

            //Assert
            Assert.IsNull(exp);
        }

        [TestMethod]
        public void StyleBoldValue_GoodHighlightIsNAndBadHighlightIsY_ReturnLongIIFText()
        {
            //Arrange
            var tester = new CellHighLightManager();
            CustomColumnInformation column = new CustomColumnInformation()
            {
                GoodHighlight = "N",
                BadHighlight = "Y",
                Order = 1
            };
            var field = string.Format("Fields!Field{0}.Value", column.Order);
            var act = string.Format("=IIF(Right({0}, Len({0}) - InStr({0}, \"-\")) = \"[G]\", \"Bold\", " +
                                  "IIF(Right({0}, Len({0}) - InStr({0}, \"-\")) = \"[R]\", \"Bold\", " +
                                  "IIF(Right({0}, Len({0}) - InStr({0}, \"-\")) = \"[I]\", \"Bold\", " +
                                  "IIF(Right({0}, Len({0}) - InStr({0}, \"-\")) = \"[B]\", \"Bold\", " +
                                  "\"Normal\"))))", field);
            //Act
            var exp = tester.StyleBoldValue(column);

            //Assert
            Assert.AreEqual(exp, act);
        }
        
        [TestMethod]
        public void StyleBoldValue_GoodHighlightAndBadHighlightAreN_ReturnNormal()
        {
            //Arrange
            var tester = new CellHighLightManager();
            CustomColumnInformation column = new CustomColumnInformation()
            {
                GoodHighlight = "N",
                BadHighlight = "N",
                Order = 1
            };
            var act = "Normal";
            //Act
            var exp = tester.StyleBoldValue(column);

            //Assert
            Assert.AreEqual(exp, act);
        }
        
        [TestMethod]
        public void StyleItalicValue_GoodHighlightIsNAndBadHighlightIsN_ReturnLongIIFText()
        {
            //Arrange
            var tester = new CellHighLightManager();
            CustomColumnInformation column = new CustomColumnInformation()
            {
                GoodHighlight = "N",
                BadHighlight = "Y",
                Order = 1
            };
            var field = string.Format("Fields!Field{0}.Value", column.Order);
            var act = string.Format("=IIF(Right({0}, Len({0}) - InStr({0}, \"-\")) = \"[I]\", \"Italic\", " +
                                  "IIF(Right({0}, Len({0}) - InStr({0}, \"-\")) = \"[B]\", \"Italic\", " +
                                  "\"Normal\"))", field);
            //Act
            var exp = tester.StyleItalicValue(column);

            //Assert
            Assert.AreEqual(exp, act);
        }
        
        [TestMethod]
        public void StyleItalicValue_GoodHighlightAndBadHighlightAreN_ReturnNormal()
        {
            //Arrange
            var tester = new CellHighLightManager();
            CustomColumnInformation column = new CustomColumnInformation()
            {
                GoodHighlight = "N",
                BadHighlight = "N",
                Order = 1
            };
            var act = "Normal";
            //Act
            var exp = tester.StyleItalicValue(column);

            //Assert
            Assert.AreEqual(exp, act);
        }
        
        [TestMethod]
        public void GetCellStyle_GoodHighlightAndBadHighlightAreNIsTotal_ResultMatches()
        {
            //Arrange
            var tester = new CellHighLightManager();
            CustomColumnInformation column = new CustomColumnInformation()
            {
                GoodHighlight = "N",
                BadHighlight = "N",
                Order = 1
            };
            var act = new XElement(Xmlns + "FontWeight", "Bold").ToString();

            //Act
            var exp = tester.GetCellStyle(column, 7.5, "Classic").FirstNode.ToString();

            //Assert
            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void GetCellStyle_GoodHighlightAndBadHighlightAreNotNIsNotTotal_ResultAreNotEqual()
        {
            //Arrange
            var tester = new CellHighLightManager();
            CustomColumnInformation column = new CustomColumnInformation()
            {
                GoodHighlight = "B",
                BadHighlight = "G",
                Order = 1
            };
            var act = new XElement(Xmlns + "FontWeight", "Bold");

            //Act
            var exp = tester.GetCellStyle(column, 7.5, "Classic", false).FirstNode;

            //Assert
            Assert.AreNotEqual(exp, act);
        }

        [TestMethod]
        public void GetCellStyle_HighLightisNAndIsDecimalTrueStyleIsGrayScaleAndNotTotal_ResultMatches()
        {
            //Arrange
            var tester = new CellHighLightManager();
            CustomColumnInformation column = new CustomColumnInformation()
            {
                GoodHighlight = "N",
                BadHighlight = "N",
                Order = 1,
                IsDecimal = true
            };
            var act = new XElement(Xmlns + "FontSize", "7.5pt").ToString();

            //Act
            var exp = tester.GetCellStyle(column, 7.5, "GRAYSCALE", false).FirstNode.ToString();

            //Assert
            Assert.AreEqual(exp, act);
        }
        [TestMethod]
        public void GetCellStyle_HighLightisNAndIsDecimalTrueStyleIsGrayScaleAndNotTotalHasItalic_ReturnTrue()
        {
            //Arrange
            var tester = new CellHighLightManager();
            CustomColumnInformation column = new CustomColumnInformation()
            {
                GoodHighlight = "B",
                BadHighlight = "B",
                Order = 1,
                IsDecimal = true
            };
            var field = string.Format("Fields!Field{0}.Value", column.Order);
            var act = string.Format("=IIF(Right({0}, Len({0}) - InStr({0}, \"-\")) = \"[I]\", \"Italic\", " +
                                  "IIF(Right({0}, Len({0}) - InStr({0}, \"-\")) = \"[B]\", \"Italic\", " +
                                  "\"Normal\"))", field);

            //Act
            var exp = tester.GetCellStyle(column, 7.5, "GRAYSCALE", false).ToString();

            //Assert
            Assert.IsTrue(exp.IndexOf(act, StringComparison.Ordinal)>0);
        }

    }
}
