using System;
using System.Text;
using System.Collections.Generic;

using iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryYearToYear;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryYearToYear
{
    /// <summary>
    /// Summary description for ExecSummaryYrToYrDateCalculatorTests
    /// </summary>
    [TestClass]
    public class ExecSummaryYrToYrDateCalculatorTests
    {
        private readonly ExecSummaryYrToYrDateCalculator _calc = new ExecSummaryYrToYrDateCalculator();

        [TestMethod]
        public void GetBeginDate_FiscalYearGreaterThanStartMonth()
        {
            var fiscalYearStartMonth = 2;
            var startMonth = 1;
            var startYear = 2016;

            var output = _calc.GetBeginDate(fiscalYearStartMonth, startMonth, startYear);

            Assert.AreEqual(new DateTime(2014, 2, 1), output);
        }

        [TestMethod]
        public void GetBeginDate_FiscalYearLessThanStartMonth()
        {
            var fiscalYearStartMonth = 1;
            var startMonth = 2;
            var startYear = 2016;

            var output = _calc.GetBeginDate(fiscalYearStartMonth, startMonth, startYear);

            Assert.AreEqual(new DateTime(2015, 1, 1), output);
        }
        [TestMethod]
        public void GetBeginDate_FiscalYearEqualToStartMonth()
        {
            var fiscalYearStartMonth = 2;
            var startMonth = 2;
            var startYear = 2016;

            var output = _calc.GetBeginDate(fiscalYearStartMonth, startMonth, startYear);

            Assert.AreEqual(new DateTime(2015, 2, 1), output);
        }

        [TestMethod]
        public void GetOneYearPrior()
        {
            var testDate = new DateTime(2016, 5, 1);

            var output = _calc.GetOneYearPrior(testDate);

            Assert.AreEqual(new DateTime(2015, 5, 1), output);
        }

        [TestMethod]
        public void GetBeginDate2_FiscalYearStartGreaterThanStartMonth()
        {
            var fiscalYearStartMonth = 2;
            var startMonth = 1;
            var startYear = 2016;

            var output = _calc.GetBeginDate2(fiscalYearStartMonth, startMonth, startYear);

            Assert.AreEqual(new DateTime(2015, 2, 1), output);
        }

        [TestMethod]
        public void GetBeginDate2_FiscalYearStartLessThanStartMonth()
        {
            var fiscalYearStartMonth = 1;
            var startMonth = 2;
            var startYear = 2016;

            var output = _calc.GetBeginDate2(fiscalYearStartMonth, startMonth, startYear);

            Assert.AreEqual(new DateTime(2016, 1, 1), output);
        }

        [TestMethod]
        public void GetBeginDate2_FiscalYearStartEqualToStartMonth()
        {
            var fiscalYearStartMonth = 2;
            var startMonth = 2;
            var startYear = 2016;

            var output = _calc.GetBeginDate2(fiscalYearStartMonth, startMonth, startYear);

            Assert.AreEqual(new DateTime(2016, 2, 1), output);
        }

        [TestMethod]
        public void GetBeginMonth2_NotQuarterToQuarterOption()
        {
            var output = _calc.GetBeginMonth2(2016, 1, false, 2);

            Assert.AreEqual(new DateTime(2016, 1, 1), output);
        }
    }
}
