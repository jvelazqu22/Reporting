using System.Linq;

using Domain.Helper;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.XmlExtract;
using iBank.Services.Implementation.Shared;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class XmlExtractTests : BaseUnitTest
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
            GenerateReportHandoffRecordsNoData();

            InsertReportHandoff();

            //run the report
            var rpt = (XmlExtract)RunReport();
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
            GenerateReportHandoffRecordsTooMuchData();

            InsertReportHandoff();

            //run the report
            var rpt = (XmlExtract)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        private void GenerateReportHandoffRecordsNoData()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,6,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDPASSENGERXMLRECORD", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,6,30", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "9", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "581", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibxmlexport", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3377132", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "-4", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });
        }

        private void GenerateReportHandoffRecordsTooMuchData()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,6,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDPASSENGERXMLRECORD", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,6,30", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "9", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "581", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibxmlexport", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3377132", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "-4", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });
        }

        #endregion

        #region Standard exports BackOffice data

        [TestMethod]
        public void BackOfficeReportDepartureDate()
        {
            var beginDate = "DT:2015,6,1";
            var endDate = "DT:2015,6,1";
            GenerateHandoffRecords(DateType.DepartureDate, beginDate, endDate, "2");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (XmlExtract)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of RawData records. XML does not use the FinalData class. 
            Assert.AreEqual(22, rpt.RawDataList.Count, "Raw data records failed.");

            //check for the correct # of iBank Travelitinerary nodes
            var tiNodes = rpt.XDoc.Descendants().Where(s => s.Name.LocalName.Equals("iBank_TravelItinerary")).ToList();
            Assert.AreEqual(21, tiNodes.Count, "Node count failed.");


            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeReportInvoiceDate()
        {
            var beginDate = "DT:2015,6,1";
            var endDate = "DT:2015,6,1";
            GenerateHandoffRecords(DateType.InvoiceDate, beginDate, endDate,"2");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (XmlExtract)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of RawData records. XML does not use the FinalData class. 
            Assert.AreEqual(5, rpt.RawDataList.Count, "Raw data records failed.");

            //check for the correct # of iBank Travelitinerary nodes
            var tiNodes = rpt.XDoc.Descendants().Where(s => s.Name.LocalName.Equals("iBank_TravelItinerary")).ToList();
            Assert.AreEqual(5, tiNodes.Count, "Node count failed.");


            ClearReportHandoff();
        }

       
        private void GenerateHandoffRecords(DateType dateType, string beginDate, string endDate, string prePost, string inAcct = "")
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,6,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDPASSENGERXMLRECORD", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,6,30", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "9", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "581", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibxmlexport", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3377132", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "-4", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");
            ManipulateReportHandoffRecords(beginDate, "BEGDATE");
            ManipulateReportHandoffRecords(endDate, "ENDDATE");
            ManipulateReportHandoffRecords(prePost, "PREPOST");
            if (inAcct != "")
            {
                ManipulateReportHandoffRecords(inAcct, "INACCT");
            }
        }

        #endregion

        [TestMethod]
        public void ReservationReportLastUpdateDate()
        {
            var beginDate = "DT:2015,6,1";
            var endDate = "DT:2015,6,2";
            GenerateHandoffRecords(DateType.LastUpdate, beginDate, endDate, "1");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (XmlExtract)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of RawData records. XML does not use the FinalData class. 
           // Assert.AreEqual(2, rpt.RawDataList.Count, "Raw data records failed.");

            //check for the correct # of iBank Travelitinerary nodes
            var tiNodes = rpt.XDoc.Descendants().Where(s => s.Name.LocalName.Equals("iBank_TravelItinerary")).ToList();
            Assert.AreEqual(2, tiNodes.Count, "Node count failed.");


            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportDepartureDate()
        {
            var beginDate = "DT:2015,6,1";
            var endDate = "DT:2015,6,2";
            GenerateHandoffRecords(DateType.DepartureDate, beginDate, endDate, "1");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (XmlExtract)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of RawData records. XML does not use the FinalData class. 
            Assert.AreEqual(5, rpt.RawDataList.Count, "Raw data records failed.");

            //check for the correct # of iBank Travelitinerary nodes
            var tiNodes = rpt.XDoc.Descendants().Where(s => s.Name.LocalName.Equals("iBank_TravelItinerary")).ToList();
            Assert.AreEqual(5, tiNodes.Count, "Node count failed.");


            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportInvoiceDate()
        {
            var beginDate = "DT:2015,6,1";
            var endDate = "DT:2015,6,30";
            GenerateHandoffRecords(DateType.InvoiceDate, beginDate, endDate, "1");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (XmlExtract)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of RawData records. XML does not use the FinalData class. 
            Assert.AreEqual(4, rpt.RawDataList.Count, "Raw data records failed.");

            //check for the correct # of iBank Travelitinerary nodes
            var tiNodes = rpt.XDoc.Descendants().Where(s => s.Name.LocalName.Equals("iBank_TravelItinerary")).ToList();
            Assert.AreEqual(4, tiNodes.Count, "Node count failed.");


            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportBookedDate()
        {
            var beginDate = "DT:2015,1,1";
            var endDate = "DT:2015,1,1";
            GenerateHandoffRecords(DateType.BookedDate, beginDate, endDate, "1","1200");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (XmlExtract)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of RawData records. XML does not use the FinalData class. 
            Assert.AreEqual(2, rpt.RawDataList.Count, "Raw data records failed.");

            //check for the correct # of iBank Travelitinerary nodes
            var tiNodes = rpt.XDoc.Descendants().Where(s => s.Name.LocalName.Equals("iBank_TravelItinerary")).ToList();
            Assert.AreEqual(2, tiNodes.Count, "Node count failed.");


            ClearReportHandoff();
        }

    }
}
