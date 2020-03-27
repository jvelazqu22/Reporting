using Domain;
using Domain.Helper;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Utilities;
using iBank.UnitTesting.iBank.Services.Implementation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Server.Utilities.Helpers
{
    [TestClass]
    public class ReportConvertedAndAgencyEnabledTests
    {
        /*
         *  report not enabled - return false
         *  agency not enabled - return false
         *  report enabled report not staged agency enabled - return true
         *  report enabled report staged agency enabled agency staged - return true
         *  report enabled report staged agency enabled agency staged but not currently turned on - return false
         *  report enabled report staged agency enabled agency belongs to database that is staged - return true
         * */
        private readonly CanReportRunContext _context = new CanReportRunContext();

        [TestMethod]
        public void ViaReportId_ReportNotEnabled_ReturnFalse()
        {
            var reportId = "1";
            var userNumber = 10;
            var sut = new ReportValidation(_context.MasterDataStore);

            var output = sut.IsReportConvertedAndAgencyEnabled(reportId, userNumber);

            Assert.AreEqual(false, output);
        }

        //[TestMethod]
        //public void DirectCheck_ReportNotEnabled_ReturnFalse()
        //{
        //    var processKey = 1;
        //    var userNumber = 10;
        //    var sut = new ReportValidation(_context.MasterDataStore);
        //    var settings = new AgencyDotNetSettings("FOO", true, false);

        //    var output = sut.IsReportConvertedAndAgencyEnabled(processKey, settings, userNumber);

        //    Assert.AreEqual(false, output);
        //}

        [TestMethod]
        public void ViaReportId_AgencyNotEnabled_ReturnFalse()
        {
            var reportId = "4";
            var userNumber = 10;
            var sut = new ReportValidation(_context.MasterDataStore);

            var output = sut.IsReportConvertedAndAgencyEnabled(reportId, userNumber);

            Assert.AreEqual(false, output);
        }

        //[TestMethod]
        //public void DirectCheck_AgencyNotEnabled_ReturnFalse()
        //{
        //    var processKey = 2;
        //    var userNumber = 10;
        //    var sut = new ReportValidation(_context.MasterDataStore);
        //    var settings = new AgencyDotNetSettings("BAR", false, false);

        //    var output = sut.IsReportConvertedAndAgencyEnabled(processKey, settings, userNumber);

        //    Assert.AreEqual(false, output);
        //}

        //[TestMethod]
        //public void DirectCheck_AgencyNotEnabled_UserAllowToTestDotNetReport_ReturnTrue()
        //{
        //    var processKey = 2;
        //    var userNumber = 20;
        //    var sut = new ReportValidation(_context.MasterDataStore);
        //    var settings = new AgencyDotNetSettings("BAR", false, false);

        //    var output = sut.IsReportConvertedAndAgencyEnabled(processKey, settings, userNumber);

        //    Assert.AreEqual(true, output);
        //}

        [TestMethod]
        public void ViaReportId_ReportEnabledReportNotStaged_AgencyEnabled_ReturnTrue()
        {
            var reportId = "2";
            var userNumber = 10;
            var sut = new ReportValidation(_context.MasterDataStore);

            var output = sut.IsReportConvertedAndAgencyEnabled(reportId, userNumber);

            Assert.AreEqual(true, output);
        }

        //[TestMethod]
        //public void DirectCheck_ReportEnabledReportNotStaged_AgencyEnabled_ReturnTrue()
        //{
        //    var processKey = 2;
        //    var userNumber = 10;
        //    var sut = new ReportValidation(_context.MasterDataStore);
        //    var settings = new AgencyDotNetSettings("FOO", true, false);

        //    var output = sut.IsReportConvertedAndAgencyEnabled(processKey, settings, userNumber);

        //    Assert.AreEqual(true, output);
        //}

        [TestMethod]
        public void ViaReportId_ReportEnabledReportStaged_AgencyEnabledAndStaged_ReturnTrue()
        {
            var reportId = "3";
            var userNumber = 10;
            var sut = new ReportValidation(_context.MasterDataStore);

            var output = sut.IsReportConvertedAndAgencyEnabled(reportId, userNumber);

            Assert.AreEqual(true, output);
        }

        //[TestMethod]
        //public void DirectCheck_ReportEnabledReportStaged_AgencyEnabledAndStaged_ReturnTrue()
        //{
        //    var processKey = 3;
        //    var userNumber = 10;
        //    var sut = new ReportValidation(_context.MasterDataStore);
        //    var agencySettings = new AgencyDotNetSettings("FOO", true, false);

        //    var output = sut.IsReportConvertedAndAgencyEnabled(processKey, agencySettings, userNumber);

        //    Assert.AreEqual(true, output);
        //}

        [TestMethod]
        public void ViaReportId_ReportEnabledReportStaged_AgencyEnabledAndStagedButNotCurrentlyTurnedOn_ReturnFalse()
        {
            var reportId = "6";
            var userNumber = 10;
            var sut = new ReportValidation(_context.MasterDataStore);

            var output = sut.IsReportConvertedAndAgencyEnabled(reportId, userNumber);

            Assert.AreEqual(false, output);
        }

        //[TestMethod]
        //public void DirectCheck_ReportEnabledReportStaged_AgencyEnabledAndStagedButNotCurrentlyTurnedOn_ReturnFalse()
        //{
        //    var processKey = 3;
        //    var userNumber = 10;
        //    var sut = new ReportValidation(_context.MasterDataStore);
        //    var settings = new AgencyDotNetSettings("FOOBAR", true, false);

        //    var output = sut.IsReportConvertedAndAgencyEnabled(processKey, settings, userNumber);

        //    Assert.AreEqual(false, output);
        //}

        [TestMethod]
        public void ViaReportId_ReportEnabledReportStaged_AgencyEnabledAndBelongsToStagedDB_ReturnTrue()
        {
            var reportId = "5";
            var sut = new ReportValidation(_context.MasterDataStore);
            var userNumber = 10;

            var output = sut.IsReportConvertedAndAgencyEnabled(reportId, userNumber);

            Assert.AreEqual(true, output);
        }

        //[TestMethod]
        //public void DirectCheck_ReportEnabledReportStaged_AgencyEnabledAndBelongsToStagedDB_ReturnTrue()
        //{
        //    var processKey = 3;
        //    var userNumber = 10;
        //    var sut = new ReportValidation(_context.MasterDataStore);
        //    var settings = new AgencyDotNetSettings("STAGED", true, false);

        //    var output = sut.IsReportConvertedAndAgencyEnabled(processKey, settings, userNumber);

        //    Assert.AreEqual(true, output);
        //}
    }
}
