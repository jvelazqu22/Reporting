using System;
using System.Linq;

using Domain.Helper;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms;
using iBank.Services.Implementation.ReportPrograms.ClassOfService;
using iBank.Services.Implementation.Shared;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{

    [TestClass]
    public class ClassofSvcTests : BaseUnitTest
    {

        #region Region - Header Info
        /* 
           Reservation & Back Office Report

           Items to test:
               Date Range Type
                    Departure Date
                    Invoice Date
                    

           Standard Parameters Back Office:
               Dates: 5/1/16 - 6/3/16
               Date Range Type: Departure Date

           Standard Parameters Reservation:
               Departure Date: 12/1/2015 - 6/3/2016
               Invoice Date: 2/23/2016 - 6/3/2016 
               Booked Date: 1/1/2015 - 1/2/2015
                            
           No Data Params:
                Dates: 6/1/16 - 6/3/16
                Date Range Type: Departure Date
                Accounts: 1200
                Back Office

           Too Much Data Params:
                Dates: 
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
            var rpt = (ClassofSvc)RunReport();
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
            var rpt = (ClassofSvc)RunReport();
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
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,6,3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTDEST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTORIGIN", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "GROUPBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "1200", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INAIRLINE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INHOMECTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETRODESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETROORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVCRED", ParmValue = "ALL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MILEAGETABLE", ParmValue = "ON", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MODE", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PICKRECNUM", ParmValue = "12999", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "28", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibClassofSvc", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3378608", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SEGFAREMILEAGE", ParmValue = "ON", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1597", ParmInOut = "IN", LangCode = "" });
        }

        private void GenerateReportHandoffRecordsTooMuchData()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AOCANDOR", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AOCFLD01", ParmValue = "FLTNO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AOCOPER01", ParmValue = "=", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AOCSELECT01", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AOCVALUE01", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AOCVALUEA01", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2011,1,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,1,10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTDEST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTORIGIN", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "GROUPBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INAIRLINE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INHOMECTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETRODESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETROORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MODE", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PICKRECNUM", ParmValue = "13076", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "28", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibClassofSvc", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3378634", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1597", ParmInOut = "IN", LangCode = "" });
        }

        #endregion

        #region Regin - Reservation
        [TestMethod]
        public void ClassofSvcReservationReportDepartureDate()
        {
            GenerateHandoffRecords(DateType.DepartureDate, "DT:2015,12,1", "DT:2016,6,3", "", false);

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (ClassofSvc)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(9, rpt.FinalDataList.Count, "Final data records failed.");

            var totalExp = 6988.34m;
            var totalAct = rpt.FinalDataList.Sum(x => x.Segcost);
            Assert.AreEqual(totalExp.ToString("#0.00"), totalAct.ToString("#0.00"), "Expected total seg cost failed");

            var segExp = 16;
            var segAct = rpt.FinalDataList.Sum(x => x.Segs);
            Assert.AreEqual(segExp, segAct, "Avg Segment Cost failed");

            ClearReportHandoff();
        }
        
        [TestMethod]
        public void ClassofSvcReservationReportInvoiceDate()
        {

            GenerateHandoffRecords(DateType.InvoiceDate, "DT:2015,1,1", "DT:2015,1,2", "1188", false);
            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (ClassofSvc)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(24, rpt.FinalDataList.Count, "Final data records failed.");

            var totalExp = 9765.00m;
            var totalAct = rpt.FinalDataList.Sum(x => x.Segcost);
            Assert.AreEqual(totalExp.ToString("#0.00"), totalAct.ToString("#0.00"), "Expected total seg cost failed");

            var segExp = 41;
            var segAct = rpt.FinalDataList.Sum(x => x.Segs);
            Assert.AreEqual(segExp, segAct, "Avg Segment Cost failed");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ClassofSvcReservationReportBookDate()
        {
            GenerateHandoffRecords(DateType.BookedDate, "DT:2015,1,1", "DT:2015,1,2", "1188", false);

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (ClassofSvc)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(26, rpt.FinalDataList.Count, "Final data records failed.");

            var totalExp = 10254.28m;
            var totalAct = rpt.FinalDataList.Sum(x => x.Segcost);
            Assert.AreEqual(totalExp.ToString("#0.00"), totalAct.ToString("#0.00"), "Expected total seg cost failed");

            var segExp = 43;
            var segAct = rpt.FinalDataList.Sum(x => x.Segs);
            Assert.AreEqual(segExp, segAct, "Avg Segment Cost failed");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ClassofSvcReservationSegFareAirMileageReportBookDate()
        {
            GenerateHandoffRecords(DateType.BookedDate, "DT:2015,1,1", "DT:2015,1,2", "1188", false);
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MILEAGETABLE", ParmValue = "ON", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SEGFAREMILEAGE", ParmValue = "ON", ParmInOut = "IN", LangCode = "" });

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (ClassofSvc)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(26, rpt.FinalDataList.Count, "Final data records failed.");

            var totalExp = 8887.76m;
            var totalAct = rpt.FinalDataList.Sum(x => x.Segcost);
            Assert.AreEqual(totalExp.ToString("#0.00"), totalAct.ToString("#0.00"), "Expected total seg cost failed");

            var segExp = 43;
            var segAct = rpt.FinalDataList.Sum(x => x.Segs);
            Assert.AreEqual(segExp, segAct, "Avg Segment Cost failed");

            ClearReportHandoff();
        }
        #endregion

        #region Region - Back Office


        [TestMethod]
        public void ClassofSvcBackOfficeReportInvoiceDate()
        {
            GenerateHandoffRecords(DateType.InvoiceDate, "DT:2016,2,23", "DT:2016,6,3", "");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (ClassofSvc)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(11, rpt.FinalDataList.Count, "Final data records failed.");

            var totalExp = 0m;
            var totalAct = rpt.FinalDataList.Sum(x => x.Segcost);
            Assert.AreEqual(totalExp.ToString("#0.00"), totalAct.ToString("#0.00"), "Expected total seg cost failed");

            var segExp = 28;
            var segAct = rpt.FinalDataList.Sum(x => x.Segs);
            Assert.AreEqual(segExp, segAct, "Avg Segment Cost failed");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ClassofSvcBackOffceReportDepartureDate()
        {
            GenerateHandoffRecords(DateType.DepartureDate, "DT:2016,5,1", "DT:2016,6,3", "");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (ClassofSvc)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");
            
            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(3, rpt.FinalDataList.Count, "Final data records failed.");

            var totalExp = 806.53m;
            var totalAct = rpt.FinalDataList.Sum(x => x.Segcost);
            Assert.AreEqual(totalExp.ToString("#0.00"), totalAct.ToString("#0.00"), "Expected total seg cost failed");

            var segExp = 4;
            var segAct = rpt.FinalDataList.Sum(x => x.Segs);
            Assert.AreEqual(segExp, segAct, "Avg Segment Cost failed");

            ClearReportHandoff();
        }


        private void GenerateHandoffRecords(DateType dateRange, string begDate, string endDate, string account, bool backoffice=true)
        {
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "AGENCY",ParmValue = "DEMO",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = begDate, ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "BRANCH",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "CARDNUM",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "country",ParmValue = "United States",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = Convert.ToInt32(dateRange).ToString(), ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "DOMINTL",ParmValue = "1",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "ENDDATE",ParmValue = endDate,ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "FIRSTDEST",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "FIRSTORIGIN",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "GROUPBY",ParmValue = "1",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "INACCT",ParmValue = account,ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "INAIRLINE",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "INBREAK1",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "INBREAK2",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "INBREAK3",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "INDESTCOUNTRY",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "INDESTREGION",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "INDESTS",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "INMETRODESTS",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "INMETROORGS",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "INORGS",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "INORIGCOUNTRY",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "INORIGREGION",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "INSOURCEABBR",ParmValue = "DEMOCA01",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "INVALCARR",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "INVCRED",ParmValue = "ALL",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "INVOICE",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "MILEAGETABLE",ParmValue = "ON",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "MODE",ParmValue = "0",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "MONEYTYPE",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "OUTPUTDEST",ParmValue = "1",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "OUTPUTTYPE",ParmValue = "3",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "PASSFIRST",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "PASSLAST",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "PICKRECNUM",ParmValue = "12999",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "PREPOST",ParmValue = backoffice ? "2" : "1",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "PROCESSID",ParmValue = "28",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "PROGRAM",ParmValue = "ibClassofSvc",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "PSEUDOCITY",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "PSEUDOCITY",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "RECLOC",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "REPORTINGTRAVET",ParmValue = "YES",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "REPORTLOGKEY",ParmValue = "3378608",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "REPORTSTATUS",ParmValue = "DONE",ParmInOut = "IN", LangCode = "EN"});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "RPTTITLE2",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "TICKET",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "TITLEACCT2",ParmValue = "",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "UDRKEY",ParmValue = "0",ParmInOut = "IN", LangCode = ""});
            ReportHandoff.Add(new ReportHandoffInformation{ParmName = "USERNBR",ParmValue = "1597",ParmInOut = "IN", LangCode = ""});

        }

        #endregion
    }
}
