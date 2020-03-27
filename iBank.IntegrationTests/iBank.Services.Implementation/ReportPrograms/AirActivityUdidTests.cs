using System.Collections.Generic;
using System.Linq;

using Domain.Helper;
using Domain.Models.ReportPrograms.AirActivityByUdidReport;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms;
using iBank.Services.Implementation.ReportPrograms.AirActivityByUdid;
using iBank.Services.Implementation.Shared;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class AirActivityUdidTests : BaseUnitTest
    {
        #region Region - Header

        /*
            Reservation & Back Office Report

            Items to Test:
                Date Range Type
                    Departure Date
                    Invoice Date
                    On the road dates
                    Booked Date (reservation only)

                Udid text w/o Udid #
                Too Much Data
                No Data
                
            Standard Params Reservation:
                Dates: 7/15/15 - 8/30/15
                Accounts: All
                Udid #: 1
                Currency: USD

                Alt Booked Date: Dates: 1/15/15 - 1/15/15
                                 Accounts: 1200
                                 Udid #: 1
                                 Currency: USD
                
            Standard Params Back Office:
                Dates: 8/1/15 - 8/3/15
                Accounts: All
                Udid #: 1
                Currency: USD
                
            No Data Params:
                Dates: 8/30/15 - 8/30/15
                Date Range Type: Departure Date
                Accounts: All
                Udid #: 1
               
            Too Much Data Params:
                Dates: 8/1/11 - 8/30/15
                Date Range Type: Departure Date
                Accounts: All
                Udid #: 1
                
            Create a report with the desired criteria through iBank. Generate a set of reporthandoff objects using this sql script:

            select 'ReportHandoff.Add(new ReportHandoffInformation{ParmName = "' + rtrim(ltrim(Parmname)) + '",ParmValue = "'+rtrim(ltrim(parmvalue))+'",ParmInOut = "' + rtrim(ltrim(parminout))+'", LangCode = "' + rtrim(ltrim(Langcode)) + '"});' 
            from reporthandoff where reportid = '<REPORT ID GOES HERE>' and parminout = 'IN' order by parmname
        */


        #endregion

        #region Region - General

        [TestMethod]
        public void NoDataReport()
        {
            GenerateNoDataHandoffRecords();

            InsertReportHandoff();

            var rpt = (AirActivityUdid)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_NoData);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void TooMuchDataReport()
        {
            GenerateTooMuchDataHandoffRecords();

            InsertReportHandoff();

            var rpt = (AirActivityUdid)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsTooMuchDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
            Assert.AreEqual(true, containsTooMuchDataMsg, "Error message failed.");

            ClearReportHandoff();

        }

        [TestMethod]
        public void UdidTextWithoutUdidNumber()
        {
            GenerateNoUdidNumberHandoffRecords();

            InsertReportHandoff();

            var rpt = (AirActivityUdid)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_UDIDNbrReqd);
            Assert.AreEqual(true, containsMsg, "Error message failed.");

            ClearReportHandoff();
        }

        private void GenerateNoDataHandoffRecords()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,8,30", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,8,30", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "GBP", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PICKRECNUM", ParmValue = "12553", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "16", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibAirUDID", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3374112", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidLbl1", ParmValue = "udid number 1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDIDNBR", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDIDTEXT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });
        }

        private void GenerateTooMuchDataHandoffRecords()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2011,8,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,8,30", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "GBP", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PICKRECNUM", ParmValue = "12553", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "16", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibAirUDID", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3374111", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidLbl1", ParmValue = "udid number 1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDIDNBR", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDIDTEXT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });
        }

        private void GenerateNoUdidNumberHandoffRecords()
        {
            GenerateTooMuchDataHandoffRecords();

            //set the udid number to blank
            var rhUdid = ReportHandoff.First(x => x.ParmName == "UDIDNBR");
            if (rhUdid != null)
            {
                ReportHandoff.RemoveAll(x => x.ParmName == "UDIDNBR");

                rhUdid.ParmValue = "";
                ReportHandoff.Add(rhUdid);
            }
            else
            {
                ReportHandoff.Add(new ReportHandoffInformation
                                      {
                                          ParmName = "UDIDNBR",
                                          ParmValue = "",
                                          ParmInOut = "IN",
                                          LangCode = ""
                                      });
            }

            //make sure the udidtext has a value
            var rh = ReportHandoff.First(x => x.ParmName == "UDIDTEXT");
            if (rh != null)
            {
                ReportHandoff.RemoveAll(x => x.ParmName == "UDIDTEXT");

                rh.ParmValue = "foo";
                ReportHandoff.Add(rh);
            }
            else
            {
                ReportHandoff.Add(new ReportHandoffInformation
                                      {
                                          ParmName = "UDIDTEXT",
                                          ParmValue = "foo",
                                          ParmInOut = "IN",
                                          LangCode = ""
                                      });
            }
            

            

            
        }


        #endregion

        #region Region - Reservation

        [TestMethod]
        public void ReservationReportDepartureDate()
        {
            GenerateReservationReportHandoffRecords();

            InsertReportHandoff();

            var rpt = (AirActivityUdid)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var expectedTotalRecs = 24;
            var actualTotalRecs = rpt.FinalDataList.Count;
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");
            
            //check total fare paid
            var expectedTotalFare = 21041.9M;
            var actualTotalFare = GetTotalActualFare(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalFare, actualTotalFare, "Total fare failed.");

            //check total tickets
            var expectedTotalTix = 8;
            var actualTotalTix = GetNetTicketCount(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalTix, actualTotalTix, "Total tickets failed.");

            //check first flt no
            var expectedFirstFltNo = "6309";
            var actualFirstFltNo = rpt.FinalDataList[0].Fltno.Trim();
            Assert.AreEqual(expectedFirstFltNo, actualFirstFltNo, "First flt no failed.");
            
            //check last flt no
            var expectedLastFltNo = "905";
            var actualLastFltNo = rpt.FinalDataList.Last().Fltno.Trim();
            Assert.AreEqual(expectedLastFltNo, actualLastFltNo, "Last flt no failed.");
            
            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportInvoiceDate()
        {
            GenerateReservationReportHandoffRecords(DateType.InvoiceDate);

            InsertReportHandoff();

            var rpt = (AirActivityUdid)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var expectedTotalRecs = 16;
            var actualTotalRecs = rpt.FinalDataList.Count;
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check total fare paid
            var expectedTotalFare = 12271.90M;
            var actualTotalFare = GetTotalActualFare(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalFare, actualTotalFare, "Total fare failed.");

            //check total tickets
            var expectedTotalTix = 3;
            var actualTotalTix = GetNetTicketCount(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalTix, actualTotalTix, "Total tickets failed.");

            //check first flt no
            var expectedFirstFltNo = "6518";
            var actualFirstFltNo = rpt.FinalDataList[0].Fltno.Trim();
            Assert.AreEqual(expectedFirstFltNo, actualFirstFltNo, "First flt no failed.");

            //check last flt no
            var expectedLastFltNo = "5812";
            var actualLastFltNo = rpt.FinalDataList.Last().Fltno.Trim();
            Assert.AreEqual(expectedLastFltNo, actualLastFltNo, "Last flt no failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportBookedDate()
        {
            GenerateReservationReprotHandoffRecordsBookedDate();

            InsertReportHandoff();

            var rpt = (AirActivityUdid)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var expectedTotalRecs = 54;
            var actualTotalRecs = rpt.FinalDataList.Count;
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check total fare paid
            var expectedTotalFare = 44937.68M;
            var actualTotalFare = GetTotalActualFare(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalFare, actualTotalFare, "Total fare failed.");

            //check total tickets
            var expectedTotalTix = 20;
            var actualTotalTix = GetNetTicketCount(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalTix, actualTotalTix, "Total tickets failed.");

            //check first flt no
            var expectedFirstFltNo = "1226";
            var actualFirstFltNo = rpt.FinalDataList[0].Fltno.Trim();
            Assert.AreEqual(expectedFirstFltNo, actualFirstFltNo, "First flt no failed.");

            //check last flt no
            var expectedLastFltNo = "49";
            var actualLastFltNo = rpt.FinalDataList.Last().Fltno.Trim();
            Assert.AreEqual(expectedLastFltNo, actualLastFltNo, "Last flt no failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportOnTheRoadDates()
        {
            GenerateReservationReportHandoffRecords(DateType.OnTheRoadDatesSpecial);

            InsertReportHandoff();

            var rpt = (AirActivityUdid)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var expectedTotalRecs = 66;
            var actualTotalRecs = rpt.FinalDataList.Count;
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check total fare paid
            var expectedTotalFare = 44918.29M;
            var actualTotalFare = GetTotalActualFare(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalFare, actualTotalFare, "Total fare failed.");

            //check total tickets
            var expectedTotalTix = 21;
            var actualTotalTix = GetNetTicketCount(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalTix, actualTotalTix, "Total tickets failed.");

            //check first flt no
            var expectedFirstFltNo = "238";
            var actualFirstFltNo = rpt.FinalDataList[0].Fltno.Trim();
            Assert.AreEqual(expectedFirstFltNo, actualFirstFltNo, "First flt no failed.");

            //check last flt no
            var expectedLastFltNo = "905";
            var actualLastFltNo = rpt.FinalDataList.Last().Fltno.Trim();
            Assert.AreEqual(expectedLastFltNo, actualLastFltNo, "Last flt no failed.");

            ClearReportHandoff();
        }

        private void GenerateReservationReportHandoffRecords(DateType dateType = DateType.DepartureDate)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,7,15", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,8,30", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PICKRECNUM", ParmValue = "12553", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "16", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibAirUDID", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3374117", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidLbl1", ParmValue = "udid number 1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDIDNBR", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDIDTEXT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });

            if (dateType != DateType.DepartureDate) ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");
        }

        private void GenerateReservationReprotHandoffRecordsBookedDate()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,1,15", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,1,15", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "1200", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVCRED", ParmValue = "ALL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PICKRECNUM", ParmValue = "12553", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "16", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibAirUDID", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3374213", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidLbl1", ParmValue = "udid number 1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDIDNBR", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDIDTEXT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });
        }

        #endregion

        #region Region - Back Office

        [TestMethod]
        public void BackOfficeReportDepartureDate()
        {
            GenerateBackOfficeReportHandoffRecords(DateType.DepartureDate);

            InsertReportHandoff();

            var rpt = (AirActivityUdid)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var expectedTotalRecs = 17;
            var actualTotalRecs = rpt.FinalDataList.Count;
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check total fare paid
            var expectedTotalFare = 5465.20M;
            var actualTotalFare = GetTotalActualFare(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalFare, actualTotalFare, "Total fare failed.");

            //check total tickets
            var expectedTotalTix = 8;
            var actualTotalTix = GetNetTicketCount(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalTix, actualTotalTix, "Total tickets failed.");

            //check first flt no
            var expectedFirstFltNo = "7476";
            var actualFirstFltNo = rpt.FinalDataList[0].Fltno.Trim();
            Assert.AreEqual(expectedFirstFltNo, actualFirstFltNo, "First flt no failed.");

            //check last flt no
            var expectedLastFltNo = "6079";
            var actualLastFltNo = rpt.FinalDataList.Last().Fltno.Trim();
            Assert.AreEqual(expectedLastFltNo, actualLastFltNo, "Last flt no failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeReportInvoiceDate()
        {
            GenerateBackOfficeReportHandoffRecords(DateType.InvoiceDate);

            InsertReportHandoff();

            var rpt = (AirActivityUdid)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var expectedTotalRecs = 7;
            var actualTotalRecs = rpt.FinalDataList.Count;
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check total fare paid
            var expectedTotalFare = 1138.78M;
            var actualTotalFare = GetTotalActualFare(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalFare, actualTotalFare, "Total fare failed.");

            //check total tickets
            var expectedTotalTix = 2;
            var actualTotalTix = GetNetTicketCount(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalTix, actualTotalTix, "Total tickets failed.");

            //check first flt no
            var expectedFirstFltNo = "1847";
            var actualFirstFltNo = rpt.FinalDataList[0].Fltno.Trim();
            Assert.AreEqual(expectedFirstFltNo, actualFirstFltNo, "First flt no failed.");

            //check last flt no
            var expectedLastFltNo = "3085";
            var actualLastFltNo = rpt.FinalDataList.Last().Fltno.Trim();
            Assert.AreEqual(expectedLastFltNo, actualLastFltNo, "Last flt no failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeReportOnTheRoadDates()
        {
            GenerateBackOfficeReportHandoffRecords(DateType.OnTheRoadDatesSpecial);

            InsertReportHandoff();

            var rpt = (AirActivityUdid)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var expectedTotalRecs = 22;
            var actualTotalRecs = rpt.FinalDataList.Count;
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check total fare paid
            var expectedTotalFare = 11995.59M;
            var actualTotalFare = GetTotalActualFare(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalFare, actualTotalFare, "Total fare failed.");

            //check total tickets
            var expectedTotalTix = 10;
            var actualTotalTix = GetNetTicketCount(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalTix, actualTotalTix, "Total tickets failed.");

            //check first flt no
            var expectedFirstFltNo = "7476";
            var actualFirstFltNo = rpt.FinalDataList[0].Fltno.Trim();
            Assert.AreEqual(expectedFirstFltNo, actualFirstFltNo, "First flt no failed.");

            //check last flt no
            var expectedLastFltNo = "6079";
            var actualLastFltNo = rpt.FinalDataList.Last().Fltno.Trim();
            Assert.AreEqual(expectedLastFltNo, actualLastFltNo, "Last flt no failed.");

            ClearReportHandoff();
        }

        private void GenerateBackOfficeReportHandoffRecords(DateType dateType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,8,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,8,3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVCRED", ParmValue = "ALL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PICKRECNUM", ParmValue = "12553", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "16", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibAirUDID", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3374223", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidLbl1", ParmValue = "udid number 1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDIDNBR", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDIDTEXT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });

            if (dateType != DateType.DepartureDate) ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");
        }

        #endregion

        #region Region - Helpers

        private decimal GetTotalActualFare(List<FinalData> finalData)
        {
            var dict = new Dictionary<string, decimal>();
            foreach (var item in finalData)
            {
                var temp = item.Ticket + item.Reckey + item.Passfrst + item.Passlast;

                if(!dict.ContainsKey(temp)) dict.Add(temp, item.Airchg);
            }
            return dict.Keys.Sum(item => dict[item]);
        }

        private int GetNetTicketCount(List<FinalData> finalData)
        {
            var dict = new Dictionary<string, decimal>();
            foreach (var item in finalData)
            {
                var temp = item.Ticket + item.Reckey + item.Passfrst + item.Passlast;

                if (!dict.ContainsKey(temp)) dict.Add(temp, item.Airchg);
            }
            return dict.Keys.Count;
        }

        #endregion
    }
}
