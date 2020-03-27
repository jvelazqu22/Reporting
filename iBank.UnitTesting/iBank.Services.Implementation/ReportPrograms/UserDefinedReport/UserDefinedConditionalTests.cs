using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iBank.Services.Implementation.ReportPrograms.UserDefined;
using iBank.Server.Utilities.Classes;
using Domain.Helper;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport
{
    [TestClass]
    public class UserDefinedConditionalTests
    {
        [TestMethod]
        public void IsCarbonMetricHeadersOn_CarbonCalcHasValueMetricIsOn_ReturnsTrue()
        {
            //Arrage
            var conditional = new UserDefinedConditional();
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.CARBONCALC, "Foo");
            globals.SetParmValue(WhereCriteria.METRIC, "On");

            var userReport = new UserReportInformation();
            userReport.HasTripCarbon = true;

            //Act
            var exp = conditional.IsCarbonMetricHeadersOn(globals, userReport);

            //Assert
            Assert.IsTrue(exp);
        }

        [TestMethod]
        public void IsCarbonMetricHeadersOn_CarbonCalcHasValueHasAirCarbonMetricIsOn_ReturnsTrue()
        {
            //Arrage
            var conditional = new UserDefinedConditional();
            ReportGlobals globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.CARBONCALC, "Foo");
            globals.SetParmValue(WhereCriteria.METRIC, "On");

            var userReport = new UserReportInformation();
            userReport.HasAirCarbon = true;

            //Act
            var exp = conditional.IsCarbonMetricHeadersOn(globals, userReport);

            //Assert
            Assert.IsTrue(exp);
        }

        [TestMethod]
        public void IsCarbonMetricHeadersOn_CarbonCalcHasNoValueHasAirCarbonMetricIsOn_ReturnsFalse()
        {
            //Arrage
            var conditional = new UserDefinedConditional();
            ReportGlobals globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.METRIC, "On");

            var userReport = new UserReportInformation();
            userReport.HasAirCarbon = true;

            //Act
            var exp = conditional.IsCarbonMetricHeadersOn(globals, userReport);

            //Assert
            Assert.IsFalse(exp);
        }

        [TestMethod]
        public void IsCarbonMetricHeadersOn_CarbonCalcHasValueHasAirCarbonMetricIsOff_ReturnsFalse()
        {
            //Arrage
            var conditional = new UserDefinedConditional();
            ReportGlobals globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.CARBONCALC, "Foo");

            var userReport = new UserReportInformation();
            userReport.HasAirCarbon = true;

            //Act
            var exp = conditional.IsCarbonMetricHeadersOn(globals, userReport);

            //Assert
            Assert.IsFalse(exp);
        }
    }
}
