using System.Collections.Generic;

using iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth.Calculations;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth.Calculations
{
    [TestClass]
    public class CarDataCalculationsTests
    {
        [TestMethod]
        public void CalculateCarExcepts_EmptyReasCoda_Return0()
        {
            var calc = new CarDataCalculations();
            var reasCoda = "";
            var reasExclude = new List<string>();
            var cplusMin = 0;

            var output = calc.CalculateCarExcepts(reasCoda, reasExclude, cplusMin);

            Assert.AreEqual(0, output);
        }

        [TestMethod]
        public void CalculateCarExcepts_NonEmptyReasCodaContainedInReasExclude_Return0()
        {
            var calc = new CarDataCalculations();
            var reasCoda = "foo";
            var reasExclude = new List<string> { "foo" };
            var cplusMin = 0;

            var output = calc.CalculateCarExcepts(reasCoda, reasExclude, cplusMin);

            Assert.AreEqual(0, output);
        }

        [TestMethod]
        public void CalculateCarExcepts_NonEmptyReasCodaNotContainedInReasExclude_ReturnCPlusMin()
        {
            var calc = new CarDataCalculations();
            var reasCoda = "foo";
            var reasExclude = new List<string> { "bar" };
            var cplusMin = 1;

            var output = calc.CalculateCarExcepts(reasCoda, reasExclude, cplusMin);

            Assert.AreEqual(1, output);
        }

        [TestMethod]
        public void CalculateCarLost_EmptyReasCoda_Return0()
        {
            var calc = new CarDataCalculations();
            var reasCoda = "";
            var reasExclude = new List<string>();
            var abookrat = 1M;
            var aExcprat = 2M;
            var days = 3;

            var output = calc.CalculateCarLost(reasCoda, reasExclude, abookrat, aExcprat, days);

            Assert.AreEqual(0M, output);
        }

        [TestMethod]
        public void CalculateCarLost_NonEmptyReasCodaContainedInReasExclude_Return0()
        {
            var calc = new CarDataCalculations();
            var reasCoda = "foo";
            var reasExclude = new List<string> { "foo" };
            var abookrat = 1M;
            var aExcprat = 2M;
            var days = 3;

            var output = calc.CalculateCarLost(reasCoda, reasExclude, abookrat, aExcprat, days);

            Assert.AreEqual(0M, output);
        }

        [TestMethod]
        public void CalculateCarLost_NonEmptyReasCodaNotContainedInReasExclude_ReturnProductOfABookRatMinusAExcpratThenTimesDays()
        {
            var calc = new CarDataCalculations();
            var reasCoda = "foo";
            var reasExclude = new List<string> { "bar" };
            var abookrat = 2M;
            var aExcprat = 3M;
            var days = 4;

            var exp = (abookrat - aExcprat) * days;
            var output = calc.CalculateCarLost(reasCoda, reasExclude, abookrat, aExcprat, days);

            Assert.AreEqual(exp, output);
        }
    }
}
