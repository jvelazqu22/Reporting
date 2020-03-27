using Domain.Constants;

using iBank.Services.Implementation.ReportPrograms.AirActivity;
using iBank.Services.Implementation.Shared;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.AirActivity
{
    [TestClass]
    public class AirActivityCalculationsTests
    {
        [TestMethod]
        public void GetCrystalReportName_ReservationReportCarbonEmissionsReportWithAlternateEmissions_ReturnAirActivityRpt2AltEmissions()
        {
            var calc = new AirActivityCalculations();

            var output = calc.GetCrystalReportName(true, true, true);

            Assert.AreEqual(ReportNames.AIR_ACTIVITY_RPT_2_ALT_EMISSIONS, output);
        }

        [TestMethod]
        public void GetCrystalReportName_ReservationReportCarbonReportNoAltEmissions_ReturnAirActivityWithSvcFeesAltEmissions()
        {
            var calc = new AirActivityCalculations();

            var output = calc.GetCrystalReportName(true, true, false);

            Assert.AreEqual(ReportNames.AIR_ACTIVITY_RPT_2_NO_ALT_EMISSIONS, output);
        }

        [TestMethod]
        public void GetCrystalReportName_ReservationReportNoCarbonReport_ReturnAirActivityWithSvcFeesNoAltEmissions()
        {
            var calc = new AirActivityCalculations();

            var output = calc.GetCrystalReportName(true, false, false);

            Assert.AreEqual(ReportNames.AIR_ACTIVITY_RPT_2, output);
        }

        [TestMethod]
        public void GetCrystalReportName_HistoryReportCarbonReportWithAltEmissions_ReturnAirActivityNoSvcFeesAltEmissions()
        {
            var calc = new AirActivityCalculations();

            var output = calc.GetCrystalReportName(false, true, true);

            Assert.AreEqual(ReportNames.AIR_ACTIVITY_RPT_ALT_EMISSIONS, output);
        }

        [TestMethod]
        public void GetCrystalReportName_HistoryReportCarbonReportNoAltEmissions_ReturnAirActivityNoSvcFeesNoAltEmissions()
        {
            var calc = new AirActivityCalculations();

            var output = calc.GetCrystalReportName(false, true, false);

            Assert.AreEqual(ReportNames.AIR_ACTIVITY_RPT_NO_ALT_EMISSIONS, output);
        }

        [TestMethod]
        public void GetCrystalReportName_HistoryReportNoCarbonReport_ReturnAirActivityNoSvcFees()
        {
            var calc = new AirActivityCalculations();

            var output = calc.GetCrystalReportName(false, false, false);

            Assert.AreEqual(ReportNames.AIR_ACTIVITY_RPT, output);
        }
    }
}
