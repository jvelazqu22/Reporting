using Domain.Helper;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryYearToYear;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryYearToYear
{
    /// <summary>
    /// Summary description for ExecutiveSummaryYearToYearSqlCreatorTests
    /// </summary>
    [TestClass]
    public class ExecutiveSummaryYearToYearCalculationsTests
    {
        private readonly ExecutiveSummaryYearToYearCalculations _calc = new ExecutiveSummaryYearToYearCalculations();

        [TestMethod]
        public void GetCrystalReportName_ReportOptionNotEqualToThree_ReturnDefault()
        {
            var reportName = _calc.GetCrystalReportName("1");

            Assert.AreEqual("ibQView3", reportName);
        }
        [TestMethod]
        public void GetCrystalReportName_ReportOptionNull_ReturnDefault()
        {
            var reportName = _calc.GetCrystalReportName(null);

            Assert.AreEqual("ibQView3", reportName);
        }

        [TestMethod]
        public void GetCrystalReportName_ReportOptionEqualToThree_ReturnAlternateName()
        {
            var reportName = _calc.GetCrystalReportName("3");

            Assert.AreEqual("ibQView3A", reportName);
        }

        [TestMethod]
        public void ExcludeServiceFee_NotReservationReportAndExcludeServiceFeesOn_ReturnTrue()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.CBEXCLUDESVCFEES, "ON");

            var output = _calc.ExcludeServiceFee(false, globals);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void ExcludeServiceFee_NotReservationReportAndExcludeServiceFeesOff_ReturnFalse()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.CBEXCLUDESVCFEES, "OFF");

            var output = _calc.ExcludeServiceFee(false, globals);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void ExcludeServiceFee_IsReservationReportAndExcludeServiceFeesOn_ReturnFalse()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.CBEXCLUDESVCFEES, "ON");

            var output = _calc.ExcludeServiceFee(true, globals);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void ExcludeServiceFee_IsReservationReportAndExcludeServiceFeesOff_ReturnFalse()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.CBEXCLUDESVCFEES, "OFF");

            var output = _calc.ExcludeServiceFee(true, globals);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void GetUseDate_DateRangeEqualsTwo_ReturnInvDate()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.DATERANGE, "2");
            var expected = "invdate";

            var output = _calc.GetUseDate(globals);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void GetUseDate_DateRangeEqualsThree_ReturnDepDate()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.DATERANGE, "3");
            var expected = "depdate";

            var output = _calc.GetUseDate(globals);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void IsQuarterToQuarterOption_OptionEqualsTwo_ReturnTrue()
        {
            var output = _calc.IsQuarterToQuarterOption("2");

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsQuarterToQuarterOption_OptionNotEqualToTwo_ReturnFalse()
        {
            var output = _calc.IsQuarterToQuarterOption("3");

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void OrphanServiceFees_IncludeOrphanFeesOnUseServiceFeesTrue_ReturnTrue()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.CBINCLSVCFEENOMATCH, "ON");
            var useServicesFees = true;

            var output = _calc.OrphanServiceFees(useServicesFees, globals);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void OrphanServiceFees_IncludeOrphanFeesOffUseServiceFeesTrue_ReturnFalse()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.CBINCLSVCFEENOMATCH, "OFF");
            var useServicesFees = true;

            var output = _calc.OrphanServiceFees(useServicesFees, globals);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void OrphanServiceFees_IncludeOrphanFeesOnUseServiceFeesFalse_ReturnFalse()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.CBINCLSVCFEENOMATCH, "ON");
            var useServicesFees = false;

            var output = _calc.OrphanServiceFees(useServicesFees, globals);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void GetWeightMeasurement_UseMetricTrue_ReturnKgs()
        {
            var output = _calc.GetWeightMeasurement(true);

            Assert.AreEqual("Kgs", output);
        }

        [TestMethod]
        public void GetWeightMeasurement_UseMetricFalse_ReturnLbs()
        {
            var output = _calc.GetWeightMeasurement(false);

            Assert.AreEqual("Lbs.", output);
        }

        [TestMethod]
        public void GetDistanceMeasurement_UseMetricTrue_ReturnKm()
        {
            var output = _calc.GetDistanceMeasurement(true);

            Assert.AreEqual("Km", output);
        }

        [TestMethod]
        public void GetDistanceMeasurement_UseMetricFalse_ReturnMile()
        {
            var output = _calc.GetDistanceMeasurement(false);

            Assert.AreEqual("Mile", output);
        }
    }
}
