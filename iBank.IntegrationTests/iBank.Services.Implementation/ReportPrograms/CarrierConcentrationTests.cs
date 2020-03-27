using System.Linq;

using Domain.Helper;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms;
using iBank.Services.Implementation.ReportPrograms.CarrierConcentration;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class CarrierConcentrationTests : BaseUnitTest
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
            var rpt = (CarrierConcentration)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_NoData);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        //TODO: Can't find any criteria that return too much data. 
        //[TestMethod]
        //public void GenerateReportTooMuchData()
        //{
        //    GenerateReportHandoffRecordsTooMuchData();

        //    InsertReportHandoff();

        //    //run the report
        //    var rpt = (CarrierConcentration)RunReport();
        //    var rptInfo = rpt.Globals.ReportInformation;

        //    //check for validity
        //    Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
        //    var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
        //    Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

        //    ClearReportHandoff();
        //}

        private void GenerateReportHandoffRecordsNoData()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,6,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,6,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "HOWMANY", ParmValue = "10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "29", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibCarrierConc", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBFLTMKTONEWAYBOTHWAYS", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBSORTDESCASC", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3378078", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SEGCARR1", ParmValue = "DL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });
        }

        //TODO: Remove if "too much data" test can't be built. 

        private void GenerateReportHandoffRecordsTooMuchData()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,6,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,6,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "HOWMANY", ParmValue = "10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "29", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibCarrierConc", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBFLTMKTONEWAYBOTHWAYS", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBSORTDESCASC", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3378078", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SEGCARR1", ParmValue = "DL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });
        }

        #endregion

        #region Region - Reservation

        [TestMethod]
        public void ReservationReportDepartureDate()
        {
            GenerateHandoffRecords(DateType.DepartureDate, "DT:2015,6,1", "DT:2015,6,30", "1");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (CarrierConcentration)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");


            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(6, rpt.FinalDataList.Count, "Final data records failed.");

            var carrierVolume = rpt.FinalDataList.Sum(s => s.Carrvolume);
            Assert.AreEqual(1610.27m, carrierVolume, "Final data records failed.");
            var otherVolume = rpt.FinalDataList.Sum(s => s.Totvolume);
            Assert.AreEqual(1610.27m, otherVolume, "Final data records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportInvoiceDate()
        {
            GenerateHandoffRecords(DateType.InvoiceDate, "DT:2015,8,1", "DT:2015,8,30", "1");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (CarrierConcentration)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(2, rpt.FinalDataList.Count, "Final data records failed.");

            var carrierVolume = rpt.FinalDataList.Sum(s => s.Carrvolume);
            Assert.AreEqual(6758.90m, carrierVolume, "Final data records failed.");
            var otherVolume = rpt.FinalDataList.Sum(s => s.Totvolume);
            Assert.AreEqual(6758.90m, otherVolume, "Final data records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportBookedDate()
        {
            GenerateHandoffRecords(DateType.BookedDate, "DT:2015,1,1", "DT:2015,1,1", "1");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (CarrierConcentration)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");


            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(10, rpt.FinalDataList.Count, "Final data records failed.");

            var carrierVolume = rpt.FinalDataList.Sum(s => s.Carrvolume);
            Assert.AreEqual(3786.71m, carrierVolume, "Final data records failed.");
            var otherVolume = rpt.FinalDataList.Sum(s => s.Totvolume);
            Assert.AreEqual(4681.32m, otherVolume, "Final data records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportBookedDateNoLimit()
        {
            GenerateHandoffRecords(DateType.BookedDate, "DT:2015,1,1", "DT:2015,1,1", "1", "10","1");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (CarrierConcentration)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");


            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(14, rpt.FinalDataList.Count, "Final data records failed.");

            var carrierVolume = rpt.FinalDataList.Sum(s => s.Carrvolume);
            Assert.AreEqual(5262.61m, carrierVolume, "Final data records failed.");
            var otherVolume = rpt.FinalDataList.Sum(s => s.Totvolume);
            Assert.AreEqual(6157.22m, otherVolume, "Final data records failed.");

            ClearReportHandoff();
        }



        private void GenerateHandoffRecords(DateType dateType, string beginDate, string endDate, string prePost, string howMany = "10", string sortBy = "2")
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = beginDate, ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = endDate, ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "HOWMANY", ParmValue = "10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "29", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibCarrierConc", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBFLTMKTONEWAYBOTHWAYS", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBSORTDESCASC", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3378078", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SEGCARR1", ParmValue = "DL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");
            ManipulateReportHandoffRecords(prePost, "PREPOST");
            ManipulateReportHandoffRecords(howMany, "HOWMANY");
            ManipulateReportHandoffRecords(sortBy, "SORTBY");
        }


        #endregion

        #region Region - Back Office
        [TestMethod]
        public void BackOfficeReportDepartureDate()
        {
            GenerateHandoffRecords(DateType.DepartureDate, "DT:2015,1,1", "DT:2015,2,1", "2");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (CarrierConcentration)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(5, rpt.FinalDataList.Count, "Final data records failed.");

            var carrierVolume = rpt.FinalDataList.Sum(s => s.Carrvolume);
            Assert.AreEqual(2889.87m, carrierVolume, "Final data records failed.");
            var otherVolume = rpt.FinalDataList.Sum(s => s.Totvolume);
            Assert.AreEqual(13623.99m, otherVolume, "Final data records failed.");

            ClearReportHandoff();
        }

       

        [TestMethod]
        public void BackOfficeReportInvoiceDate()
        {
            GenerateHandoffRecords(DateType.InvoiceDate, "DT:2015,1,1", "DT:2015,2,1", "2");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //run the report
            var rpt = (CarrierConcentration)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(8, rpt.FinalDataList.Count, "Final data records failed.");

            var carrierVolume = rpt.FinalDataList.Sum(s => s.Carrvolume);
            Assert.AreEqual(4000.22m, carrierVolume, "Final data records failed.");
            var otherVolume = rpt.FinalDataList.Sum(s => s.Totvolume);
            Assert.AreEqual(20153.03m, otherVolume, "Final data records failed.");

            ClearReportHandoff();
        }

      
        #endregion
    }
}
