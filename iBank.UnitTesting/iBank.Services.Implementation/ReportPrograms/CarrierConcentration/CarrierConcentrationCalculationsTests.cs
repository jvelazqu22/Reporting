using System;

using Domain.Constants;
using Domain.Helper;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.CarrierConcentration;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.CarrierConcentration
{
    [TestClass]
    public class CarrierConcentrationCalculationsTests
    {
        [TestMethod]
        public void GetCrystalReportName_NoOptions_ReturnDefaultName()
        {
            var suppressAvgDiff = false;    
            var excludeSavings = false;
            var useAirportCodes = false;
            var sut = new CarrierConcentrationCalculations();

            var output = sut.GetCrystalReportName(suppressAvgDiff, excludeSavings, useAirportCodes);

            Assert.AreEqual(ReportNames.CARRIER_CONCENTRATION, output);
        }

        [TestMethod]
        public void GetCrystalReportName_SuppressAvgDiffIsFalse_ExcludeSavingsIsFalse_UseAirportCodesTrue_ReturnAReport()
        {
            var suppressAvgDiff = false;
            var excludeSavings = false;
            var useAirportCodes = true;
            var sut = new CarrierConcentrationCalculations();

            var output = sut.GetCrystalReportName(suppressAvgDiff, excludeSavings, useAirportCodes);

            Assert.AreEqual(ReportNames.CARRIER_CONCENTRATION_A, output);
        }

        [TestMethod]
        public void GetCrystalReportName_SuppressAvgDiffIsTrue_ExcludeSavingsIsFalse_UseAirportCodesTrue_ReturnAReport()
        {
            var suppressAvgDiff = true;
            var excludeSavings = false;
            var useAirportCodes = true;
            var sut = new CarrierConcentrationCalculations();

            var output = sut.GetCrystalReportName(suppressAvgDiff, excludeSavings, useAirportCodes);

            Assert.AreEqual(ReportNames.CARRIER_CONCENTRATION_A, output);
        }

        [TestMethod]
        public void GetCrystalReportName_SuppressAvgDiffIsFalse_ExcludeSavingsIsTrue_UseAirportCodesTrue_ReturnAReport()
        {
            var suppressAvgDiff = false;
            var excludeSavings = true;
            var useAirportCodes = true;
            var sut = new CarrierConcentrationCalculations();

            var output = sut.GetCrystalReportName(suppressAvgDiff, excludeSavings, useAirportCodes);

            Assert.AreEqual(ReportNames.CARRIER_CONCENTRATION_A, output);
        }

        [TestMethod]
        public void GetCrystalReportName_SuppressAvgDiffIsTrue_ExcludeSavingsIsTrue_UseAirportCodesTrue_Return2AReport()
        {
            var suppressAvgDiff = true;
            var excludeSavings = true;
            var useAirportCodes = true;
            var sut = new CarrierConcentrationCalculations();

            var output = sut.GetCrystalReportName(suppressAvgDiff, excludeSavings, useAirportCodes);

            Assert.AreEqual(ReportNames.CARRIER_CONCENTRATION_2A, output);
        }

        [TestMethod]
        public void GetCrystalReportName_SuppressAvgDiffIsTrue_ExcludeSavingsIsTrue_UseAirportCodesFalse_Return2Report()
        {
            var suppressAvgDiff = true;
            var excludeSavings = true;
            var useAirportCodes = false;
            var sut = new CarrierConcentrationCalculations();

            var output = sut.GetCrystalReportName(suppressAvgDiff, excludeSavings, useAirportCodes);

            Assert.AreEqual(ReportNames.CARRIER_CONCENTRATION_2, output);
        }

        [TestMethod]
        public void SwapOriginsAndDestinations()
        {
            var sut = new CarrierConcentrationCalculations();
            var globals = new ReportGlobals();
            //origins
            globals.SetParmValue(WhereCriteria.ORIGIN, "ORIGIN");
            globals.SetParmValue(WhereCriteria.INORGS, "INORGS");
            globals.SetParmValue(WhereCriteria.METROORG, "METRORG");
            globals.SetParmValue(WhereCriteria.INMETROORGS, "INMETROORGS");
            globals.SetParmValue(WhereCriteria.ORIGCOUNTRY, "ORIGCOUNTRY");
            globals.SetParmValue(WhereCriteria.INORIGCOUNTRY, "INORIGCOUNTRY");
            globals.SetParmValue(WhereCriteria.ORIGREGION, "ORIGREGION");
            globals.SetParmValue(WhereCriteria.INORIGREGION, "INORIGREGION");
            //destinations
            globals.SetParmValue(WhereCriteria.DESTINAT, "DESTINAT");
            globals.SetParmValue(WhereCriteria.INDESTS, "INDESTS");
            globals.SetParmValue(WhereCriteria.METRODEST, "METRODEST");
            globals.SetParmValue(WhereCriteria.INMETRODESTS, "INMETRODESTS");
            globals.SetParmValue(WhereCriteria.DESTCOUNTRY, "DESTCOUNTRY");
            globals.SetParmValue(WhereCriteria.INDESTCOUNTRY, "INDESTCOUNTRY");
            globals.SetParmValue(WhereCriteria.DESTREGION, "DESTREGION");
            globals.SetParmValue(WhereCriteria.INDESTREGION, "INDESTREGION");

            sut.SwapOriginsAndDestinations(globals);

            //origin
            Assert.AreEqual("DESTINAT", globals.GetParmValue(WhereCriteria.ORIGIN));
            Assert.AreEqual("INDESTS", globals.GetParmValue(WhereCriteria.INORGS));
            Assert.AreEqual("METRODEST", globals.GetParmValue(WhereCriteria.METROORG));
            Assert.AreEqual("INMETRODESTS", globals.GetParmValue(WhereCriteria.INMETROORGS));
            Assert.AreEqual("DESTCOUNTRY", globals.GetParmValue(WhereCriteria.ORIGCOUNTRY));
            Assert.AreEqual("INDESTCOUNTRY", globals.GetParmValue(WhereCriteria.INORIGCOUNTRY));
            Assert.AreEqual("DESTREGION", globals.GetParmValue(WhereCriteria.ORIGREGION));
            Assert.AreEqual("INDESTREGION", globals.GetParmValue(WhereCriteria.INORIGREGION));

            //destination
            Assert.AreEqual("ORIGIN", globals.GetParmValue(WhereCriteria.DESTINAT));
            Assert.AreEqual("INORGS", globals.GetParmValue(WhereCriteria.INDESTS));
            Assert.AreEqual("METRORG", globals.GetParmValue(WhereCriteria.METRODEST));
            Assert.AreEqual("INMETROORGS", globals.GetParmValue(WhereCriteria.INMETRODESTS));
            Assert.AreEqual("ORIGCOUNTRY", globals.GetParmValue(WhereCriteria.DESTCOUNTRY));
            Assert.AreEqual("INORIGCOUNTRY", globals.GetParmValue(WhereCriteria.INDESTCOUNTRY));
            Assert.AreEqual("ORIGREGION", globals.GetParmValue(WhereCriteria.DESTREGION));
            Assert.AreEqual("INORIGREGION", globals.GetParmValue(WhereCriteria.INDESTREGION));
        }
    }
}
