using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.TransactionSummary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class TransactSumTests : BaseUnitTest
    {
        #region Region - Header Info

        /*
            Items to Test:
                Return Data
                Return Data - Show Client Detail

                No data
                Too Much Data

            Standard Params:
                Start Month: March
                Start Year: 2012
                End Month: February 
                End Year: 2016

            No Data Params:
                Start Month: Feb
                Start Year: 2016
                End Month: Feb
                End Year: 2016
                
            Create a report with the desired criteria through iBank. Generate a set of reporthandoff objects using this sql script:

           select 'ReportHandoff.Add(new ReportHandoffInformation{ParmName = "' + rtrim(ltrim(Parmname)) + '",ParmValue = "'+rtrim(ltrim(parmvalue))+'",ParmInOut = "' + rtrim(ltrim(parminout))+'", LangCode = "' + rtrim(ltrim(Langcode)) + '"});' 
           from reporthandoff where reportid = '<REPORT ID GOES HERE>' and parminout = 'IN' order by parmname
        */

        #endregion

        [TestMethod]
        public void GenerateData()
        {
            GenerateReportHandoffRecordsWithData();

            InsertReportHandoff();

            var rpt = (TransactionSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check total records 
            var actualTotalRecs = rpt.FinalDataList.Count;
            var expectedTotalRecs = 2;
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check total trips
            var actualTotalTrips = rpt.FinalDataList.Sum(x => x.BothCount);
            var expectedTotalTrips = 323;
            Assert.AreEqual(expectedTotalTrips, actualTotalTrips, "Total trips failed.");

            //check total change management
            var actualTotalChgMgmt = rpt.FinalDataList.Sum(x => x.ChgMgmtCnt);
            var expectedTotalChgMgmt = 0;
            Assert.AreEqual(expectedTotalChgMgmt, actualTotalChgMgmt, "Total change management failed.");
            
            //check first month
            var actualFirstMonth = rpt.FinalDataList[0].Month;
            var expectedFirstMonth = 12;
            Assert.AreEqual(expectedFirstMonth, actualFirstMonth, "First month failed.");

            //check last month
            var actualLastMonth = rpt.FinalDataList.Last().Month;
            var expectedLastMonth = 1;
            Assert.AreEqual(expectedLastMonth, actualLastMonth, "Last month failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateDataShowClientDetail()
        {
            GenerateReportHandoffRecordsWithData(true);

            InsertReportHandoff();

            var rpt = (TransactionSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var actualTotalRecs = rpt.FinalDataList.Count;
            var expectedTotalRecs = 7;
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check total months
            var actualTotalMonths = rpt.FinalDataList.Select(x => x.Month).Distinct().Count();
            var expectedTotalMonths = 2;
            Assert.AreEqual(expectedTotalMonths, actualTotalMonths, "Total months failed.");

            //check total records in Dec
            var actualDecRecs = rpt.FinalDataList.Count(x => x.Month == 12);
            var expectedDecRecs = 3;
            Assert.AreEqual(expectedDecRecs, actualDecRecs, "Total Dec recs failed.");

            //check total trips
            var actualTotalTrips = rpt.FinalDataList.Sum(x => x.BothCount);
            var expectedTotalTrips = 323;
            Assert.AreEqual(expectedTotalTrips, actualTotalTrips, "Total trips failed.");
            
            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateNoData()
        {
            GenerateReportHandoffRecordsNoData();

            InsertReportHandoff();

            var rpt = (TransactionSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_NoData);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");


            ClearReportHandoff();
        }

        private void GenerateReportHandoffRecordsWithData(bool showClientDetail = false)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBBREAKBYSOURCE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBSHOWCLIENTDETAIL", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDMONTH", ParmValue = "February", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDYEAR", ParmValue = "2016", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "138", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTransactSum", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3374554", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "STARTMONTH", ParmValue = "March", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "STARTYEAR", ParmValue = "2012", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });

            if (showClientDetail) ManipulateReportHandoffRecords("ON", "CBSHOWCLIENTDETAIL");
        }
        
        private void GenerateReportHandoffRecordsNoData()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBBREAKBYSOURCE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBSHOWCLIENTDETAIL", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDMONTH", ParmValue = "February", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDYEAR", ParmValue = "2016", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "138", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTransactSum", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3374555", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "STARTMONTH", ParmValue = "February", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "STARTYEAR", ParmValue = "2016", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });
        }
    }
}
