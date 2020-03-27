using System;
using System.Linq;

using Domain.Helper;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.TravelManagementSummary;
using iBank.Services.Implementation.Shared;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class TravelManagementTests : BaseUnitTest
    {
        #region Region - Header Info
        /* 
           Reservation & Back Office Report

           Items to test:
               Date Range Type
                    Departure Date
                    Invoice Date
                    On the road dates
                    Booked Date - Reservation Report Only

               Number of passengers
               No data
               Too Much Data

           Standard Parameters Back Office:
               Dates: 1/1/16 - 2/27/16
               Accounts: All

           Standard Parameters Reservation:
               Accounts: All

               Departure Date: 7/1/15 - 2/27/16
               Invoice Date: 4/1/15 - 2/27/16
               On The Road Dates: 6/1/15 - 2/27/16
               Booked Date: 1/1/15 - 1/31/15 
                            Account: 1100, 1188, 1200
                            
           No Data Params:
                Dates: 2/25/16 - 2/26/16
                Date Range Type: Departure Date
                Accounts: 1100
                Back Office

           Too Much Data Params:
                Dates: 2/25/12 - 2/26/16
                Date Range Type: Departure Date
                Accounts: all
                Back Office

           Report Id: 

           Create a report with the desired criteria through iBank. Generate a set of reporthandoff objects using this sql script:

           select 'ReportHandoff.Add(new ReportHandoffInformation{ParmName = "' + rtrim(ltrim(Parmname)) + '",ParmValue = "'+rtrim(ltrim(parmvalue))+'",ParmInOut = "' + rtrim(ltrim(parminout))+'", LangCode = "' + rtrim(ltrim(Langcode)) + '"});' 
           from reporthandoff where reportid = '<REPORT ID GOES HERE>' and parminout = 'IN' order by parmname
       */
        #endregion

        #region Region - General

        [TestMethod]
        public void GenerateReportNoData()
        {
            GenerateHandoffRecords(DateType.DepartureDate, "2015","1","2");
            ManipulateReportHandoffRecords("1200", "INACCT");
            InsertReportHandoff();

            //run the report
            var rpt = (TravelManagementSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_NoData);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportTooMuchData()
        {
            GenerateHandoffRecords(DateType.DepartureDate, "2015", "1", "2");
            
            InsertReportHandoff();

            //run the report
            var rpt = (TravelManagementSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        #endregion

        #region Region - Reservation

        [TestMethod]
        public void ReservationReportDepartureDate()
        {
            GenerateHandoffRecords(DateType.DepartureDate, "2015", "1", "1");
            ManipulateReportHandoffRecords("2600", "INACCT");
            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (TravelManagementSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(40, rpt.FinalDataList.Count, "Final data records failed.");

            //grab the total row and check some values
            var totalRow = rpt.FinalDataList.Last();
            Assert.AreEqual(52517, Math.Round(totalRow.Ytd));

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportInvoiceDate()
        {
            GenerateHandoffRecords(DateType.InvoiceDate, "2015", "1", "1");
            ManipulateReportHandoffRecords("2600", "INACCT");
            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (TravelManagementSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(40, rpt.FinalDataList.Count, "Final data records failed.");

            //grab the total row and check some values
            var totalRow = rpt.FinalDataList.Last();
            Assert.AreEqual(49852, Math.Round(totalRow.Ytd));

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportBookedDate()
        {
            GenerateHandoffRecords(DateType.BookedDate, "2015", "1", "1");
            ManipulateReportHandoffRecords("2600", "INACCT");
            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (TravelManagementSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(40, rpt.FinalDataList.Count, "Final data records failed.");

            //grab the total row and check some values
            var totalRow = rpt.FinalDataList.Last();
            Assert.AreEqual(52517, Math.Round(totalRow.Ytd));

            ClearReportHandoff();
        }


        private void GenerateHandoffRecords(DateType dateType, string year, string month, string prePost)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARBONCALC", ParmValue = "CISCARBON", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTLANGUAGE", ParmValue = "EN", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "38", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTravelMgmt", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3379890", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "STARTYEAR", ParmValue = "2015", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TXTFYSTARTMTH", ParmValue = "January", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");
            ManipulateReportHandoffRecords(prePost, "PREPOST");
            ManipulateReportHandoffRecords(month, "TXTFYSTARTMTH");
            ManipulateReportHandoffRecords(year, "STARTYEAR");

        }


        #endregion

        #region Region - Back Office
        [TestMethod]
        public void BackOfficeReportDepartureDate()
        {
            GenerateHandoffRecords(DateType.DepartureDate, "2015", "1", "2");
            ManipulateReportHandoffRecords("2600", "INACCT");
            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (TravelManagementSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(53, rpt.FinalDataList.Count, "Final data records failed.");

             //grab the total row and check some values
            var totalRow = rpt.FinalDataList.Last();
            Assert.AreEqual(1408499,Math.Round(totalRow.Ytd));
            var milesRow = rpt.FinalDataList.FirstOrDefault(s => s.RowNum == 90);
            Assert.AreEqual(milesRow.Ytd, 3994237);

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeReportDepartureDateMetric()
        {
            GenerateHandoffRecords(DateType.DepartureDate, "2015", "1", "2");
            ManipulateReportHandoffRecords("2600", "INACCT");
            ManipulateReportHandoffRecords("ON", "METRIC");
            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (TravelManagementSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(53, rpt.FinalDataList.Count, "Final data records failed.");

            //grab the total row and check some values
            var totalRow = rpt.FinalDataList.Last();
            Assert.AreEqual(1408499, Math.Round(totalRow.Ytd));
            var milesRow = rpt.FinalDataList.FirstOrDefault(s => s.RowNum == 90);
            Assert.AreEqual(milesRow.Ytd, 6428517);

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeReportDepartureDateSeparateRail()
        {
            GenerateHandoffRecords(DateType.DepartureDate, "2015", "1", "2");
            ManipulateReportHandoffRecords("2600", "INACCT");
            ManipulateReportHandoffRecords("ON", "CBSEPARATERAIL"); 
            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (TravelManagementSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(73, rpt.FinalDataList.Count, "Final data records failed.");

            //grab the total row and check some values
            var totalRow = rpt.FinalDataList.Last();
            Assert.AreEqual(1408499, Math.Round(totalRow.Ytd));


            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeReportDepartureDateCarbonEmissions()
        {
            GenerateHandoffRecords(DateType.DepartureDate, "2015", "1", "2");
            ManipulateReportHandoffRecords("2600", "INACCT");
            ManipulateReportHandoffRecords("ON", "CARBONEMISSIONS"); 
            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (TravelManagementSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(59, rpt.FinalDataList.Count, "Final data records failed.");

            //grab the total row and check some values
            var totalRow = rpt.FinalDataList.Last();
            Assert.AreEqual(1408499, Math.Round(totalRow.Ytd));


            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeReportDepartureDateExclusions()
        {
            GenerateHandoffRecords(DateType.DepartureDate, "2015", "1", "2");
            ManipulateReportHandoffRecords("2600", "INACCT");
            ManipulateReportHandoffRecords("ON", "CBEXCLEXCHINFO");
            ManipulateReportHandoffRecords("ON", "CBEXCLUDEEXCEPTNS");
            ManipulateReportHandoffRecords("ON", "CBEXCLUDEONLINEADOPT");
            ManipulateReportHandoffRecords("ON", "CBEXCLUDESVCFEES");
            ManipulateReportHandoffRecords("ON", "CBEXCLUDESVGS");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (TravelManagementSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(39, rpt.FinalDataList.Count, "Final data records failed.");

            //grab the total row and check some values
            var totalRow = rpt.FinalDataList.Last();
            Assert.AreEqual(1408499, Math.Round(totalRow.Ytd));
            var milesRow = rpt.FinalDataList.FirstOrDefault(s => s.RowNum == 90);
            Assert.AreEqual(milesRow.Ytd, 3994237);

            ClearReportHandoff();
        }


        [TestMethod]
        public void BackOfficeReportInvoiceDate()
        {
            GenerateHandoffRecords(DateType.InvoiceDate, "2015", "1", "2");
            ManipulateReportHandoffRecords("2600", "INACCT");
            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (TravelManagementSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(53, rpt.FinalDataList.Count, "Final data records failed.");

            //grab the total row and check some values
            var totalRow = rpt.FinalDataList.Last();
            Assert.AreEqual(1394963, Math.Round(totalRow.Ytd));

            ClearReportHandoff();
        }

      
        #endregion
    }
}
