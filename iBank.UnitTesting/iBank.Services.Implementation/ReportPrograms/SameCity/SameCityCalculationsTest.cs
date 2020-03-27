using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Helper;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.SameCity;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.SameCity
{
    [TestClass]
    public class SameCityCalculationsTest
    {
        private readonly SameCityCalculations _calc = new SameCityCalculations();

        [TestMethod]
        public void GetCrystalReportName()
        {
            var output = _calc.GetCrystalReportName();

            Assert.AreEqual("ibSameCity", output);
        }

        [TestMethod]
        public void GetNumberOfTravelers_NotANumber_Returns5()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.HOWMANY, "foo");

            var output = _calc.GetNumberOfTravelers(globals);

            Assert.AreEqual(5, output);
        }

        [TestMethod]
        public void GetNumberOfTravelers_NoTravelers_Returns5()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.HOWMANY, "0");

            var output = _calc.GetNumberOfTravelers(globals);

            Assert.AreEqual(5, output);
        }

        [TestMethod]
        public void GetNumberOfTravelers_GreaterThanOne_ReturnsTheNumber()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.HOWMANY, "2");

            var output = _calc.GetNumberOfTravelers(globals);

            Assert.AreEqual(2, output);
        }
    }
}
