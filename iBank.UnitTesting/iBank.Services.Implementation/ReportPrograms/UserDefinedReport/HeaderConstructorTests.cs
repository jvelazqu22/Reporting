using Domain.Helper;

using iBank.Entities.MasterEntities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport
{
    [TestClass]
    public class HeaderConstructorTests
    {
        [TestMethod]
        public void GetHeadersByColumn_BreakOne_ReturnHeaderOneBlank_HeaderTwoWithBreakOneName()
        {
            var column = new collist2 { colname = "BREAK1" };
            var globals = new ReportGlobals { User = new UserInformation { Break1Name = "foo" } };
            var sut = new HeaderConstructor();

            var output = sut.GetHeadersByColumn(column, globals);

            Assert.AreEqual("", output.HeaderOne);
            Assert.AreEqual("foo", output.HeaderTwo);
        }

        [TestMethod]
        public void GetHeadersByColumn_TBreakOne_ReturnHeaderOneBlank_HeaderTwoWithBreakOneName()
        {
            var column = new collist2 { colname = "TBREAK1" };
            var globals = new ReportGlobals { User = new UserInformation { Break1Name = "foo" } };
            var sut = new HeaderConstructor();

            var output = sut.GetHeadersByColumn(column, globals);

            Assert.AreEqual("", output.HeaderOne);
            Assert.AreEqual("foo", output.HeaderTwo);
        }

        [TestMethod]
        public void GetHeadersByColumn_BreakTwo_ReturnHeaderOneBlank_HeaderTwoWithBreakTwoName()
        {
            var column = new collist2 { colname = "BREAK2" };
            var globals = new ReportGlobals { User = new UserInformation { Break2Name = "foo" } };
            var sut = new HeaderConstructor();

            var output = sut.GetHeadersByColumn(column, globals);

            Assert.AreEqual("", output.HeaderOne);
            Assert.AreEqual("foo", output.HeaderTwo);
        }

        [TestMethod]
        public void GetHeadersByColumn_TBreakTwo_ReturnHeaderOneBlank_HeaderTwoWithTBreakTwoName()
        {
            var column = new collist2 { colname = "TBREAK2" };
            var globals = new ReportGlobals { User = new UserInformation { Break2Name = "foo" } };
            var sut = new HeaderConstructor();

            var output = sut.GetHeadersByColumn(column, globals);

            Assert.AreEqual("", output.HeaderOne);
            Assert.AreEqual("foo", output.HeaderTwo);
        }

        [TestMethod]
        public void GetHeadersByColumn_BreakThree_ReturnHeaderOneBlank_HeaderTwoWithBreakThreeName()
        {
            var column = new collist2 { colname = "BREAK3" };
            var globals = new ReportGlobals { User = new UserInformation { Break3Name = "foo" } };
            var sut = new HeaderConstructor();

            var output = sut.GetHeadersByColumn(column, globals);

            Assert.AreEqual("", output.HeaderOne);
            Assert.AreEqual("foo", output.HeaderTwo);
        }

        [TestMethod]
        public void GetHeadersByColumn_TBreakThree_ReturnHeaderOneBlank_HeaderTwoWithTBreakThreeName()
        {
            var column = new collist2 { colname = "TBREAK3" };
            var globals = new ReportGlobals { User = new UserInformation { Break3Name = "foo" } };
            var sut = new HeaderConstructor();

            var output = sut.GetHeadersByColumn(column, globals);

            Assert.AreEqual("", output.HeaderOne);
            Assert.AreEqual("foo", output.HeaderTwo);
        }

        [TestMethod]
        public void GetHeadersByColumn_Tax1_TaxNameHasNoSpaces_ReturnHeaderOneBlank_HeaderTwoWithTaxName()
        {
            var column = new collist2 { colname = "TAX1" };
            var globals = new ReportGlobals { User = new UserInformation { Tax1Name = "foo" } };
            var sut = new HeaderConstructor();

            var output = sut.GetHeadersByColumn(column, globals);

            Assert.AreEqual("", output.HeaderOne);
            Assert.AreEqual("foo", output.HeaderTwo);
        }

        [TestMethod]
        public void GetHeadersByColumn_Tax1_TaxNameHasSpaces_ReturnHeaderOneFirstWordInTaxName_HeaderTwoIsSecondWordInTaxName()
        {
            var column = new collist2 { colname = "TAX1" };
            var globals = new ReportGlobals { User = new UserInformation { Tax1Name = "foo bar" } };
            var sut = new HeaderConstructor();

            var output = sut.GetHeadersByColumn(column, globals);

            Assert.AreEqual("foo", output.HeaderOne);
            Assert.AreEqual("bar", output.HeaderTwo);
        }

        [TestMethod]
        public void GetHeadersByColumn_STax1_TaxNameHasNoSpaces_ReturnHeaderOneBlank_HeaderTwoWithTaxName()
        {
            var column = new collist2 { colname = "STAX1" };
            var globals = new ReportGlobals { User = new UserInformation { Tax1Name = "foo" } };
            var sut = new HeaderConstructor();

            var output = sut.GetHeadersByColumn(column, globals);

            Assert.AreEqual("", output.HeaderOne);
            Assert.AreEqual("foo", output.HeaderTwo);
        }

        [TestMethod]
        public void GetHeadersByColumn_STax1_TaxNameHasSpaces_ReturnHeaderOneFirstWordInTaxName_HeaderTwoIsSecondWordInTaxName()
        {
            var column = new collist2 { colname = "STAX1" };
            var globals = new ReportGlobals { User = new UserInformation { Tax1Name = "foo bar" } };
            var sut = new HeaderConstructor();

            var output = sut.GetHeadersByColumn(column, globals);

            Assert.AreEqual("foo", output.HeaderOne);
            Assert.AreEqual("bar", output.HeaderTwo);
        }

        [TestMethod]
        public void GetHeadersByColumn_Tax2_TaxNameHasNoSpaces_ReturnHeaderOneBlank_HeaderTwoWithTaxName()
        {
            var column = new collist2 { colname = "TAX2" };
            var globals = new ReportGlobals { User = new UserInformation { Tax2Name = "foo" } };
            var sut = new HeaderConstructor();

            var output = sut.GetHeadersByColumn(column, globals);

            Assert.AreEqual("", output.HeaderOne);
            Assert.AreEqual("foo", output.HeaderTwo);
        }

        [TestMethod]
        public void GetHeadersByColumn_Tax2_TaxNameHasSpaces_ReturnHeaderOneFirstWordInTaxName_HeaderTwoIsSecondWordInTaxName()
        {
            var column = new collist2 { colname = "TAX2" };
            var globals = new ReportGlobals { User = new UserInformation { Tax2Name = "foo bar" } };
            var sut = new HeaderConstructor();

            var output = sut.GetHeadersByColumn(column, globals);

            Assert.AreEqual("foo", output.HeaderOne);
            Assert.AreEqual("bar", output.HeaderTwo);
        }

        [TestMethod]
        public void GetHeadersByColumn_STax2_TaxNameHasNoSpaces_ReturnHeaderOneBlank_HeaderTwoWithTaxName()
        {
            var column = new collist2 { colname = "STAX2" };
            var globals = new ReportGlobals { User = new UserInformation { Tax2Name = "foo" } };
            var sut = new HeaderConstructor();

            var output = sut.GetHeadersByColumn(column, globals);

            Assert.AreEqual("", output.HeaderOne);
            Assert.AreEqual("foo", output.HeaderTwo);
        }

        [TestMethod]
        public void GetHeadersByColumn_STax2_TaxNameHasSpaces_ReturnHeaderOneFirstWordInTaxName_HeaderTwoIsSecondWordInTaxName()
        {
            var column = new collist2 { colname = "STAX2" };
            var globals = new ReportGlobals { User = new UserInformation { Tax2Name = "foo bar" } };
            var sut = new HeaderConstructor();

            var output = sut.GetHeadersByColumn(column, globals);

            Assert.AreEqual("foo", output.HeaderOne);
            Assert.AreEqual("bar", output.HeaderTwo);
        }
    }
}
