using System;

using Domain.Helper;

using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesSendOff;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesSendOff
{
    [TestClass]
    public class SendOffCalculationsTests
    {
        private ReportGlobals _globals;

        private SendOffCalculations _calc;

        [TestInitialize]
        public void Init()
        {
            _globals = new ReportGlobals();
            _calc = new SendOffCalculations();
        }

        [TestMethod]
        public void GetCrystalReportName_SuppressChangeDetails()
        {
            var output = _calc.GetCrystalReportName(true);

            Assert.AreEqual("ibSendOff2", output);
        }
        [TestMethod]
        public void GetCrystalReportName_DontSuppressChangeDetails()
        {
            var output = _calc.GetCrystalReportName(false);

            Assert.AreEqual("ibSendOff", output);
        }

        
       

        [TestMethod]
        public void IsDepartureDateRange_IsFour_ReturnTrue()
        {
            _globals.SetParmValue(WhereCriteria.DATERANGE, "4");

            var output = _calc.IsDepartureDateRange(_globals);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsDepartureDateRange_IsNotFour_ReturnFalse()
        {
            _globals.SetParmValue(WhereCriteria.DATERANGE, "1");

            var output = _calc.IsDepartureDateRange(_globals);

            Assert.AreEqual(false, output);
        }
        
        [TestMethod]
        public void IncludeCancelledTrips_WhereClauseChangesNotEmptyBuildWhereIncludeCancelledFalseCancelCodeEqualsY_ReturnTrue()
        {
            _globals.SetParmValue(WhereCriteria.CANCELCODE, "Y");
            var whereClauseChanges = "foo";
            var buildWhereIncludeCancelled = false;

            var output = _calc.IncludeCancelledTrips(_globals, whereClauseChanges, buildWhereIncludeCancelled);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IncludeCancelledTrips_WhereClauseChangesEmptyBuildWhereIncludeCancelledFalseCancelCodeEqualsY_ReturnFalse()
        {
            _globals.SetParmValue(WhereCriteria.CANCELCODE, "Y");
            var whereClauseChanges = "";
            var buildWhereIncludeCancelled = false;

            var output = _calc.IncludeCancelledTrips(_globals, whereClauseChanges, buildWhereIncludeCancelled);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IncludeCancelledTrips_WhereClauseChangesNotEmptyBuildWhereIncludeCancelledTrueCancelCodeEqualsN_ReturnFalse()
        {
            _globals.SetParmValue(WhereCriteria.CANCELCODE, "N");
            var whereClauseChanges = "foo";
            var buildWhereIncludeCancelled = true;

            var output = _calc.IncludeCancelledTrips(_globals, whereClauseChanges, buildWhereIncludeCancelled);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void IncludeCancelledTrips_WhereClauseChangesNotEmptyBuildWhereIncludeCancelledTrueCancelCodeEmpty_ReturnFalse()
        {
            _globals.SetParmValue(WhereCriteria.CANCELCODE, "");
            var whereClauseChanges = "foo";
            var buildWhereIncludeCancelled = true;

            var output = _calc.IncludeCancelledTrips(_globals, whereClauseChanges, buildWhereIncludeCancelled);

            Assert.AreEqual(true, output);
        }
    }
}
