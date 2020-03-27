using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Constants;

using iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth.Calculations;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth.Calculations
{
    [TestClass]
    public class QuickSummaryMonthCalculationsTests
    {
        [TestMethod]
        public void GetCrystalReportName_NotGraphOutputDontExcludeExceptions_ReturnReport1()
        {
            var calc = new QuickSummaryMonthCalculations();

            var output = calc.GetCrystalReportName(false, false);

            Assert.AreEqual(ReportNames.QUICK_SUMMARY_BY_MONTH_RPT_1, output);
        }

        [TestMethod]
        public void GetCrystalReportName_GraphOutputDontExcludeExceptions_ReturnReportGraph2()
        {
            var calc = new QuickSummaryMonthCalculations();

            var output = calc.GetCrystalReportName(false, true);

            Assert.AreEqual(ReportNames.GRAPH_2, output);
        }

        [TestMethod]
        public void GetCrystalReportName_NotGraphOutputExcludeExceptions_ReturnReport1A()
        {
            var calc = new QuickSummaryMonthCalculations();

            var output = calc.GetCrystalReportName(true, false);

            Assert.AreEqual(ReportNames.QUICK_SUMMARY_BY_MONTH_RPT_1A, output);
        }

        [TestMethod]
        public void IsGraphOutput_OutputType4_ReturnTrue()
        {
            var calc = new QuickSummaryMonthCalculations();

            var output = calc.IsGraphReportOutput("4");

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsGraphOutput_OutputType6_ReturnTrue()
        {
            var calc = new QuickSummaryMonthCalculations();

            var output = calc.IsGraphReportOutput("6");

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsGraphOutput_OutputType8_ReturnTrue()
        {
            var calc = new QuickSummaryMonthCalculations();

            var output = calc.IsGraphReportOutput("8");

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsGraphOutput_OutputTypeRG_ReturnTrue()
        {
            var calc = new QuickSummaryMonthCalculations();

            var output = calc.IsGraphReportOutput("RG");

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsGraphOutput_OutputTypeXG_ReturnTrue()
        {
            var calc = new QuickSummaryMonthCalculations();

            var output = calc.IsGraphReportOutput("XG");

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsGraphOutput_OutputType1_ReturnFalse()
        {
            var calc = new QuickSummaryMonthCalculations();

            var output = calc.IsGraphReportOutput("1");

            Assert.AreEqual(false, output);
        }
    }
}
