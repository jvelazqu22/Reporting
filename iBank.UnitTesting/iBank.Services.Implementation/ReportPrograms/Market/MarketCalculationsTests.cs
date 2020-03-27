using System;

using Domain.Constants;
using Domain.Helper;
using Domain.Models.ReportPrograms.MarketReport;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.Market;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.Market
{
    [TestClass]
    public class MarketCalculationsTests
    {
        private readonly MarketCalculations _calc = new MarketCalculations();
        [TestMethod]
        public void GetCrystalReportName_UseAirportCodesOneCarrier()
        {
            var numberOfCarriers = 1;
            var useAirportCodes = true;

            var output = _calc.GetCrystalReportName(numberOfCarriers, useAirportCodes);

            Assert.AreEqual(ReportNames.MARKET_RPT_1A, output);
        }

        [TestMethod]
        public void GetCrystalReportName_UseAirportCodesTwoCarriers()
        {
            var numberOfCarriers = 2;
            var useAirportCodes = true;

            var output = _calc.GetCrystalReportName(numberOfCarriers, useAirportCodes);

            Assert.AreEqual(ReportNames.MARKET_RPT_2A, output);
        }

        [TestMethod]
        public void GetCrystalReportName_UseAirportCodesThreeCarriers()
        {
            var numberOfCarriers = 3;
            var useAirportCodes = true;

            var output = _calc.GetCrystalReportName(numberOfCarriers, useAirportCodes);

            Assert.AreEqual(ReportNames.MARKET_RPT_3A, output);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetCrystalReportName_UseAirportCodesFourCarrier_ThrowException()
        {
            var numberOfCarriers = 4;
            var useAirportCodes = true;

            var output = _calc.GetCrystalReportName(numberOfCarriers, useAirportCodes);
        }

        [TestMethod]
        public void GetCrystalReportName_DontUseAirportCodesOneCarrier()
        {
            var numberOfCarriers = 1;
            var useAirportCodes = false;

            var output = _calc.GetCrystalReportName(numberOfCarriers, useAirportCodes);

            Assert.AreEqual(ReportNames.MARKET_RPT_1, output);
        }

        [TestMethod]
        public void GetCrystalReportName_DontUseAirportCodesTwoCarriers()
        {
            var numberOfCarriers = 2;
            var useAirportCodes = false;

            var output = _calc.GetCrystalReportName(numberOfCarriers, useAirportCodes);

            Assert.AreEqual(ReportNames.MARKET_RPT_2, output);
        }

        [TestMethod]
        public void GetCrystalReportName_DontUseAirportCodesThreeCarriers()
        {
            var numberOfCarriers = 3;
            var useAirportCodes = false;

            var output = _calc.GetCrystalReportName(numberOfCarriers, useAirportCodes);

            Assert.AreEqual(ReportNames.MARKET_RPT_3, output);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetCrystalReportName_DontUseAirportCodesFourCarrier_ThrowException()
        {
            var numberOfCarriers = 4;
            var useAirportCodes = false;

            var output = _calc.GetCrystalReportName(numberOfCarriers, useAirportCodes);
        }

        [TestMethod]
        public void UseAirportCodes_IsOn_ReturnTrue()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.CBUSEAIRPORTCODES, "ON");

            var output = _calc.UseAirportCodes(globals);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void UseAirportCodes_IsOff_ReturnFalse()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.CBUSEAIRPORTCODES, "OFF");

            var output = _calc.UseAirportCodes(globals);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void GetNumberCarriers_CarriersAreAllNull_Return0()
        {
            Carrier carrier1 = null;
            Carrier carrier2 = null;
            Carrier carrier3 = null;

            var output = _calc.GetNumberCarriers(carrier1, carrier2, carrier3);

            Assert.AreEqual(0, output);
        }

        [TestMethod]
        public void GetNumberCarriers_Carrier1HasCountOf1_Return1()
        {
            Carrier carrier1 = null;
            Carrier carrier2 = null;
            Carrier carrier3 = null;

            var output = _calc.GetNumberCarriers(carrier1, carrier2, carrier3);

            Assert.AreEqual(0, output);
        }
    }
}
