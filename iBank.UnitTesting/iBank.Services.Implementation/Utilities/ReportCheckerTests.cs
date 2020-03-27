using Domain.Helper;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities
{
    [TestClass]
    public class ReportCheckerTests
    {
        [TestMethod]
        public void IsOriginCriteriaPopulated_NoCriteria_ReturnFalse()
        {
            var globals = new ReportGlobals();
            var checker = new ReportChecker();

            var output = checker.IsOriginCriteriaPopulated(globals);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void IsOriginCriteriaPopulated_EmptyCriteria_ReturnFalse()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.INMETROORGS, "");
            globals.SetParmValue(WhereCriteria.INORIGCOUNTRY, "");
            globals.SetParmValue(WhereCriteria.INORIGREGION, "");
            globals.SetParmValue(WhereCriteria.INORGS, "");
            var checker = new ReportChecker();

            var output = checker.IsOriginCriteriaPopulated(globals);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void IsOriginCriteriaPopulated_MetroPopulated_ReturnTrue()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.INMETROORGS, "foo");
            var checker = new ReportChecker();

            var output = checker.IsOriginCriteriaPopulated(globals);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsOriginCriteriaPopulated_CountryPopulated_ReturnTrue()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.INORIGCOUNTRY, "foo");
            var checker = new ReportChecker();

            var output = checker.IsOriginCriteriaPopulated(globals);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsOriginCriteriaPopulated_RegionPopulated_ReturnTrue()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.INORIGREGION, "foo");
            var checker = new ReportChecker();

            var output = checker.IsOriginCriteriaPopulated(globals);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsOriginCriteriaPopulated_AirportCodePopulated_ReturnTrue()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.INORGS, "foo");
            var checker = new ReportChecker();

            var output = checker.IsOriginCriteriaPopulated(globals);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsDestinationCriteriaPopulated_NoCriteria_ReturnFalse()
        {
            var globals = new ReportGlobals();
            var checker = new ReportChecker();

            var output = checker.IsDestinationCriteriaPopulated(globals);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void IsDestinationCriteriaPopulated_EmptyCriteria_ReturnFalse()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.INMETRODESTS, "");
            globals.SetParmValue(WhereCriteria.INDESTCOUNTRY, "");
            globals.SetParmValue(WhereCriteria.INDESTREGION, "");
            globals.SetParmValue(WhereCriteria.INDESTS, "");
            var checker = new ReportChecker();

            var output = checker.IsDestinationCriteriaPopulated(globals);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void IsDestinationCriteriaPopulated_MetroPopulated_ReturnTrue()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.INMETRODESTS, "foo");
            var checker = new ReportChecker();

            var output = checker.IsDestinationCriteriaPopulated(globals);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsDestinationCriteriaPopulated_CountryPopulated_ReturnTrue()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.INDESTCOUNTRY, "foo");
            var checker = new ReportChecker();

            var output = checker.IsDestinationCriteriaPopulated(globals);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsDestinationCriteriaPopulated_RegionPopulated_ReturnTrue()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.INDESTREGION, "foo");
            var checker = new ReportChecker();

            var output = checker.IsDestinationCriteriaPopulated(globals);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsDestinationCriteriaPopulated_AirportCodePopulated_ReturnTrue()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.INDESTS, "foo");
            var checker = new ReportChecker();

            var output = checker.IsDestinationCriteriaPopulated(globals);

            Assert.AreEqual(true, output);
        }
    }
}
