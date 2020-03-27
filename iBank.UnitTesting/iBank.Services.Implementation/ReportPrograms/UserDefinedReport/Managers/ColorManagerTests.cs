using Microsoft.VisualStudio.TestTools.UnitTesting;
using iBank.Services.Implementation.ReportPrograms.UserDefined;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport.Managers
{
    /// <summary>
    /// Summary description for ColorManagerTests
    /// </summary>
    [TestClass]
    public class ColorManagerTests
    {
        private ColorManager _manager;
        public ColorManagerTests()
        {
            _manager = new ColorManager();
        }

        [TestMethod]
        public void TranslateColorCode_PassClassicG_ReturnGAsGreenBold()
        {
            //Arrange
            var code = "G";
            var theme = "CLASSIC";
            var act = "G";

            //Act
            var exp = _manager.TranslateColorCode(code, theme);

            //Assert
            Assert.AreEqual(exp,act);
        }
        
        [TestMethod]
        public void TranslateColorCode_PassClassicB_ReturnRAsRedBold()
        { 
            //Arrange
            var code = "B";
            var theme = "CLASSIC";
            var act = "R";

            //Act
            var exp = _manager.TranslateColorCode(code, theme);

            //Assert
            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void TranslateColorCode_PassBoldG_ReturnGAsGreenBold()
        {
            //Arrange
            var code = "G";
            var theme = "CLASSIC";
            var act = "G";

            //Act
            var exp = _manager.TranslateColorCode(code, theme);

            //Assert
            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void TranslateColorCode_PassBoldB_ReturnRAsRedBold()
        {
            //Arrange
            var code = "B";
            var theme = "CLASSIC";
            var act = "R";

            //Act
            var exp = _manager.TranslateColorCode(code, theme);

            //Assert
            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void TranslateColorCode_PassGreyScaleG_ReturnBAsBlackBoldItalic()
        {
            //Arrange
            var code = "G";
            var theme = "GRAYSCALE";
            var act = "B";

            //Act
            var exp = _manager.TranslateColorCode(code, theme);

            //Assert
            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void TranslateColorCode_PassGrayScaleB_ReturnRAsRedBold()
        {
            //Arrange
            var code = "B";
            var theme = "GRAYSCALE";
            var act = "R";

            //Act
            var exp = _manager.TranslateColorCode(code, theme);

            //Assert
            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void TranslateColorCode_PassFreshG_ReturnIAsRedBoldItalic()
        {
            //Arrange
            var code = "G";
            var theme = "FRESH";
            var act = "I";

            //Act
            var exp = _manager.TranslateColorCode(code, theme);

            //Assert
            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void TranslateColorCode_PassFreshB_ReturnGAsGreenBold()
        {
            //Arrange
            var code = "B";
            var theme = "FRESH";
            var act = "G";

            //Act
            var exp = _manager.TranslateColorCode(code, theme);

            //Assert
            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void GetColor_PassColumnWithNoGoodOrBad_ReturnBlackString()
        {
            //Arrange
            var column = new UserReportColumnInformation();
            column.GoodHilite = "N";
            column.BadHilite = "N";
            var cellValue = "test";
            var theme = "FRESH";
            var act = "";

            //Act
            var exp = _manager.GetColor(column, cellValue, theme);

            //Assert
            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void GetColor_PassColumnGoodBBadNOperEqualThemeAsFresh_ReturnGAsGreenBold()
        {
            //Arrange
            var column = new UserReportColumnInformation();
            column.GoodHilite = "B";
            column.BadHilite = "N";
            column.GoodValue = "test";
            column.BadValue = "";
            column.GoodFieldType = "TEXT";
            column.GoodOperator = "=";
            column.BadOperator = "";
            var cellValue = "test";
            var theme = "FRESH";
            var act = "G";

            //Act
            var exp = _manager.GetColor(column, cellValue, theme);

            //Assert
            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void GetColor_PassColumnGoodNBadGOperEqualThemeAsFresh_ReturnIAsRedBoldItalic()
        {
            //Arrange
            var column = new UserReportColumnInformation();
            column.GoodHilite = "N";
            column.BadHilite = "G";
            column.GoodValue = "";
            column.BadValue = "test";
            column.BadFieldType = "TEXT";
            column.GoodOperator = "";
            column.BadOperator = "=";
            var cellValue = "test";
            var theme = "FRESH";
            var act = "I";

            //Act
            var exp = _manager.GetColor(column, cellValue, theme);

            //Assert
            Assert.AreEqual(exp, act);
        }
    }
}
