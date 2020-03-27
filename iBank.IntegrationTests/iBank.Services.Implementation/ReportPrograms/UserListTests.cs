using System.Linq;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.UserList;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class UserListTests: BaseUnitTest
    {

        [TestMethod]
        public void NoCriteria()
        {
            GenerateReportHandoff();
            InsertReportHandoff();

            var rpt = (UserList)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(181, rpt.FinalDataList.Count);

            ClearReportHandoff();
        }

        [TestMethod]
        public void PasswordSuppress()
        {
            GenerateReportHandoff();
            ManipulateReportHandoffRecords("ON", "CBSUPPPASSWORDS");
            InsertReportHandoff();

            var rpt = (UserList)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(181, rpt.FinalDataList.Count);

            Assert.IsTrue(rpt.FinalDataList.All(s => s.Password.Equals("xxxxxxxx")));
            ClearReportHandoff();
        }

        [TestMethod]
        public void AnalyticsOn()
        {
            GenerateReportHandoff();
            ManipulateReportHandoffRecords("1", "DDANALYTICS");
            InsertReportHandoff();

            var rpt = (UserList)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(16, rpt.FinalDataList.Count);

            ClearReportHandoff();
        }

        [TestMethod]
        public void AnalyticsOff()
        {
            GenerateReportHandoff();
            ManipulateReportHandoffRecords("2", "DDANALYTICS");
            InsertReportHandoff();

            var rpt = (UserList)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(165, rpt.FinalDataList.Count);

            ClearReportHandoff();
        }

        [TestMethod]
        public void TravetsOn()
        {
            GenerateReportHandoff();
            ManipulateReportHandoffRecords("1", "DDTRAVETSYESNO");
            InsertReportHandoff();

            var rpt = (UserList)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(173, rpt.FinalDataList.Count);

            ClearReportHandoff();
        }

        [TestMethod]
        public void TravetsOff()
        {
            GenerateReportHandoff();
            ManipulateReportHandoffRecords("2", "DDTRAVETSYESNO");
            InsertReportHandoff();

            var rpt = (UserList)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(8, rpt.FinalDataList.Count);

            ClearReportHandoff();
        }

        [TestMethod]
        public void EmailFilterOn()
        {
            GenerateReportHandoff();
            ManipulateReportHandoffRecords("1", "DDEMAILFILTERCHECKED");
            InsertReportHandoff();

            var rpt = (UserList)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(135, rpt.FinalDataList.Count);

            ClearReportHandoff();
        }

        [TestMethod]
        public void EmailFilterOff()
        {
            GenerateReportHandoff();
            ManipulateReportHandoffRecords("2", "DDEMAILFILTERCHECKED");
            InsertReportHandoff();

            var rpt = (UserList)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(46, rpt.FinalDataList.Count);

            ClearReportHandoff();
        }

        public void GenerateReportHandoff()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBSUPPPASSWORDS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDANALYTICS", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDEMAILFILTERCHECKED", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDTRAVETSYESNO", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "124", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibUserList", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3400999", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });

        }
    }
}
