using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using UserDefinedReports.Classes;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport.Managers
{
    /// <summary>
    /// Summary description for BorderStyleTests
    /// </summary>
    [TestClass]
    public class StyleManagerTests
    {
        private StyleManager _manager;
        public StyleManagerTests()
        {
            _manager = new StyleManager();
        }
        
        [TestMethod]
        public void BuildCellBorderStyle_TopBorderStyleSolidColorBlack_ReturnEqual()
        {
            //Arrange
             var act = new XElement(_manager.Xmlns + "TopBorder",
             new XElement(_manager.Xmlns + "Color", "Black"),
             new XElement(_manager.Xmlns + "Style", "Solid"),
             new XElement(_manager.Xmlns + "Width", "1pt"));

            //Act
            var exp = _manager.BuildCellBorderStyle("TopBorder", "Solid", "Black");

            //Assert
            Assert.AreEqual(exp.ToString(), act.ToString());
        }

        [TestMethod]
        public void SubtotalCellBorderStyle_SummaryPassFalse_ReturnEqual()
        {
            //Arrange
            var act = new XElement(_manager.Xmlns + "Style",
                 new XElement(_manager.Xmlns + "BackgroundColor", "White"),
                 new XElement(_manager.Xmlns + "Border",
                     new XElement(_manager.Xmlns + "Style", "None")),
                 new XElement(_manager.Xmlns + "LeftBorder",
                     new XElement(_manager.Xmlns + "Color", "White"),
                     new XElement(_manager.Xmlns + "Style", "Solid"),
                     new XElement(_manager.Xmlns + "Width", "1pt")),
                 new XElement(_manager.Xmlns + "RightBorder",
                     new XElement(_manager.Xmlns + "Color", "White"),
                     new XElement(_manager.Xmlns + "Style", "Solid"),
                     new XElement(_manager.Xmlns + "Width", "1pt")),
                 new XElement(_manager.Xmlns + "PaddingLeft", "1pt"),
                 new XElement(_manager.Xmlns + "PaddingRight", "1pt"),
                 new XElement(_manager.Xmlns + "PaddingBottom", "1pt"),
                 new XElement(_manager.Xmlns + "PaddingTop", "1pt"));

            //Act
            var exp = _manager.SubtotalCellBorderStyle(false);

            //Assert
            Assert.AreEqual(exp.ToString(), act.ToString());
        }

        [TestMethod]
        public void SubtotalCellBorderStyle_SummaryPassTrue_ReturnEqual()
        {
            //Arrange
            var act = new XElement(_manager.Xmlns + "Style",
                 new XElement(_manager.Xmlns + "BackgroundColor", "White"),
                 new XElement(_manager.Xmlns + "Border",
                     new XElement(_manager.Xmlns + "Style", "None")),
                 new XElement(_manager.Xmlns + "TopBorder",
                     new XElement(_manager.Xmlns + "Color", "Black"),
                     new XElement(_manager.Xmlns + "Style", "Solid"),
                     new XElement(_manager.Xmlns + "Width", "1pt")),
                 new XElement(_manager.Xmlns + "LeftBorder",
                     new XElement(_manager.Xmlns + "Color", "White"),
                     new XElement(_manager.Xmlns + "Style", "Solid"),
                     new XElement(_manager.Xmlns + "Width", "1pt")),
                 new XElement(_manager.Xmlns + "RightBorder",
                     new XElement(_manager.Xmlns + "Color", "White"),
                     new XElement(_manager.Xmlns + "Style", "Solid"),
                     new XElement(_manager.Xmlns + "Width", "1pt")),
                 new XElement(_manager.Xmlns + "PaddingLeft", "1pt"),
                 new XElement(_manager.Xmlns + "PaddingRight", "1pt"),
                 new XElement(_manager.Xmlns + "PaddingBottom", "1pt"),
                 new XElement(_manager.Xmlns + "PaddingTop", "1pt"));

            //Act
            var exp = _manager.SubtotalCellBorderStyle(true);

            //Assert
            Assert.AreEqual(exp.ToString(), act.ToString());
        }

        [TestMethod]
        public void TotalCellBorderStyle_SummaryColumnPassTrue_ReturnEqual()
        {
            //Arrange
            var act = new XElement(_manager.Xmlns + "Style",
                 new XElement(_manager.Xmlns + "BackgroundColor", "White"),
                 new XElement(_manager.Xmlns + "Border",
                     new XElement(_manager.Xmlns + "Style", "None")),
                 new XElement(_manager.Xmlns + "TopBorder",
                     new XElement(_manager.Xmlns + "Color", "Black"),
                     new XElement(_manager.Xmlns + "Style", "Solid"),
                     new XElement(_manager.Xmlns + "Width", "1pt")),
                 new XElement(_manager.Xmlns + "BottomBorder",
                     new XElement(_manager.Xmlns + "Color", "Black"),
                     new XElement(_manager.Xmlns + "Style", "Solid"),
                     new XElement(_manager.Xmlns + "Width", "2pt")),
                 new XElement(_manager.Xmlns + "LeftBorder",
                     new XElement(_manager.Xmlns + "Color", "White"),
                     new XElement(_manager.Xmlns + "Style", "Solid"),
                     new XElement(_manager.Xmlns + "Width", "1pt")),
                 new XElement(_manager.Xmlns + "RightBorder",
                     new XElement(_manager.Xmlns + "Color", "White"),
                     new XElement(_manager.Xmlns + "Style", "Solid"),
                     new XElement(_manager.Xmlns + "Width", "1pt")),
                 new XElement(_manager.Xmlns + "PaddingLeft", "1pt"),
                 new XElement(_manager.Xmlns + "PaddingRight", "1pt"),
                 new XElement(_manager.Xmlns + "PaddingBottom", "1pt"),
                 new XElement(_manager.Xmlns + "PaddingTop", "1pt"));

            //Act
            var exp = _manager.TotalCellBorderStyle(true);

            //Assert
            Assert.AreEqual(exp.ToString(), act.ToString());

        }

        [TestMethod]
        public void TotalCellBorderStyle_SummaryColumnPassFalse_ReturnEqual()
        {
            //Arrange
            var act = new XElement(_manager.Xmlns + "Style",
                 new XElement(_manager.Xmlns + "BackgroundColor", "White"),
                 new XElement(_manager.Xmlns + "Border",
                     new XElement(_manager.Xmlns + "Style", "None")),
                 new XElement(_manager.Xmlns + "LeftBorder",
                     new XElement(_manager.Xmlns + "Color", "White"),
                     new XElement(_manager.Xmlns + "Style", "Solid"),
                     new XElement(_manager.Xmlns + "Width", "1pt")),
                 new XElement(_manager.Xmlns + "RightBorder",
                     new XElement(_manager.Xmlns + "Color", "White"),
                     new XElement(_manager.Xmlns + "Style", "Solid"),
                     new XElement(_manager.Xmlns + "Width", "1pt")),
                 new XElement(_manager.Xmlns + "PaddingLeft", "1pt"),
                 new XElement(_manager.Xmlns + "PaddingRight", "1pt"),
                 new XElement(_manager.Xmlns + "PaddingBottom", "1pt"),
                 new XElement(_manager.Xmlns + "PaddingTop", "1pt"));

            //Act
            var exp = _manager.TotalCellBorderStyle(false);

            //Assert
            Assert.AreEqual(exp.ToString(), act.ToString());

        }

        [TestMethod]
        public void HeaderCellBorderStyle_Build_MatchesExpected()
        {
            //Arrange
            var act = new XElement(_manager.Xmlns + "Style",
                 new XElement(_manager.Xmlns + "BackgroundColor", "White"),
                 new XElement(_manager.Xmlns + "Border",
                     new XElement(_manager.Xmlns + "Style", "None")),
                 new XElement(_manager.Xmlns + "LeftBorder",
                     new XElement(_manager.Xmlns + "Color", "White"),
                     new XElement(_manager.Xmlns + "Style", "Solid"),
                     new XElement(_manager.Xmlns + "Width", "1pt")),
                 new XElement(_manager.Xmlns + "RightBorder",
                     new XElement(_manager.Xmlns + "Color", "White"),
                     new XElement(_manager.Xmlns + "Style", "Solid"),
                     new XElement(_manager.Xmlns + "Width", "1pt")),
                 new XElement(_manager.Xmlns + "BottomBorder",
                     new XElement(_manager.Xmlns + "Color", "Black"),
                     new XElement(_manager.Xmlns + "Style", "Solid"),
                     new XElement(_manager.Xmlns + "Width", "1pt")),
                 new XElement(_manager.Xmlns + "PaddingLeft", "1pt"),
                 new XElement(_manager.Xmlns + "PaddingRight", "1pt"),
                 new XElement(_manager.Xmlns + "PaddingBottom", "1pt"),
                 new XElement(_manager.Xmlns + "PaddingTop", "1pt"));

            //Act
            var exp = _manager.HeaderCellBorderStyle();

            //Assert
            Assert.AreEqual(exp.ToString(), act.ToString());

        }

        [TestMethod]
        public void DetailCellBorderStyle_UseAlternativeAndClassic_ReturnExpected()
        {
            //Arrange
            var act = new XElement(_manager.Xmlns + "BackgroundColor", "White");

            //Act
            var exp = _manager.DetailCellBorderStyle(false, "CLASSIC");

            //Assert
            Assert.AreEqual(exp.FirstNode.ToString(), act.ToString());

        }
        
        [TestMethod]
        public void DetailCellBorderStyle_UseAlternativeTrueGreyStyle_MatchExpected()
        {
            //Arrange   
            var act = new XElement(_manager.Xmlns + "BackgroundColor", "=IIf(RowNumber(Nothing) Mod 2 = 1, \"#F1F2F2\", \"White\")");

            //Act
            var exp = _manager.DetailCellBorderStyle(true, "GREY");

            //Assert
            Assert.AreEqual(exp.FirstNode.ToString(), act.ToString());

        }

        [TestMethod]
        public void DetailCellBorderStyle_UseAlternativeTrueFRESHStyle_MatchExpected()
        {
            //Arrange
            var act = new XElement(_manager.Xmlns + "BackgroundColor", "=IIf(RowNumber(Nothing) Mod 2 = 1, \"#F1F2F2\", \"White\")");

            //Act
            var exp = _manager.DetailCellBorderStyle(true, "FRESH");

            //Assert
            Assert.AreEqual(exp.FirstNode.ToString(), act.ToString());

        }

        [TestMethod]
        public void DetailCellBorderStyle_UseAlternativeBoldStyle_MatchExpected()
        {
            //Arrange
            var act = new XElement(_manager.Xmlns + "BackgroundColor", "=IIf(RowNumber(Nothing) Mod 2 = 1, \"#504D4C\", \"White\")");

            //Act
            var exp = _manager.DetailCellBorderStyle(true, "BOLD");

            //Assert
            Assert.AreEqual(exp.FirstNode.ToString(), act.ToString());

        }

    }
}
