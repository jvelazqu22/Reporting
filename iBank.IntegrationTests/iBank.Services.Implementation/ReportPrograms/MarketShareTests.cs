using System.Collections.Generic;
using System.Linq;

using Domain.Helper;
using Domain.Models.MarketReport;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms;
using iBank.Services.Implementation.ReportPrograms.Market;
using iBank.Services.Implementation.Shared;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class MarketShareTests : BaseUnitTest
    {
        #region Region - Header Info

        /*
            Reservation & Back Office Report

            Items to Test:
                Date Range Type
                    Departure Date
                    Invoice Date
                    Booked Date (reservation only)

                Sort By
                    Flight Market = default
                    Total # Segments
                    Total Revenue

                Use Mileage From Industry Table
                Use Airport Codes, not City Names
                Calculate Seg Fare from Mileage
                Treat Markets:
                    Bi-directional -- default
                    One Way
                Too Much Data
                No Data
                
            Standard Params Reservation:
                Dates: 3/1/15 - 3/1/16
                Accounts: 1100,1188,1200
                Carrier: American Airlines (AA)

            Standard Params Back Office:
                Dates: 1/1/16 - 3/2/16
                Accounts: All
                Carrier: Delta Air Lines (DL)

            No Data Params:
                Dates: 3/1/16 - 3/1/16
                Accounts: 1100

            Too Much Data Params:
                Dates: 3/1/11 - 3/1/16
                Accounts: all

            Create a report with the desired criteria through iBank. Generate a set of reporthandoff objects using this sql script:

            select 'ReportHandoff.Add(new ReportHandoffInformation{ParmName = "' + rtrim(ltrim(Parmname)) + '",ParmValue = "'+rtrim(ltrim(parmvalue))+'",ParmInOut = "' + rtrim(ltrim(parminout))+'", LangCode = "' + rtrim(ltrim(Langcode)) + '"});' 
            from reporthandoff where reportid = '<REPORT ID GOES HERE>' and parminout = 'IN' order by parmname
        */

        #endregion

        #region Region - Enums

        private enum SortBy
        {
            FlightMarket = 1, 
            TotalNumberSegments = 2,
            TotalRevenue = 3
        }


        #endregion

        #region Region - General

        [TestMethod]
        public void NoDataReport()
        {
            GenerateNoDataHandoffRecords();

            InsertReportHandoff();

            var rpt = (Market)RunReport();
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

            var rpt = (Market)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        private void GenerateNoDataHandoffRecords()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,3,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,3,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "1100", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INAIRLINE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETRODESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETROORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVCRED", ParmValue = "ALL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MODE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "GBP", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibMarket", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBFLTMKTONEWAYBOTHWAYS", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3373803", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SEGCARR1", ParmValue = "XX", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SEGCARR2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SEGCARR3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TXTFLTSEGMENTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });
        }

        private void GenerateTooMuchDataHandoffRecords()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2011,3,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,3,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INAIRLINE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETRODESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETROORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVCRED", ParmValue = "ALL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MODE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "GBP", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibMarket", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBFLTMKTONEWAYBOTHWAYS", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3373804", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SEGCARR1", ParmValue = "XX", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SEGCARR2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SEGCARR3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TXTFLTSEGMENTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });
        }
        
        #endregion

        #region Region - Reservation

        [TestMethod]
        public void ReservationReportDepartureDate()
        {
            GenerateReservationHandoffRecords();

            InsertReportHandoff();

            var rpt = (Market)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total markets
            var expectedTotalRecs = 112;
            var actualTotalRecs = CountDistinctOriginDest(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check for total segments
            var expectedTotalSegs = 273;
            var actualTotalSegs = rpt.FinalDataList.Sum(x => x.Segments);
            Assert.AreEqual(expectedTotalSegs, actualTotalSegs, "Total Segments failed.");

            //check total revenue
            var expectedTotalRevenue = 150697.45M;
            var actualTotalRevenue = rpt.FinalDataList.Sum(x => x.Fare);
            Assert.AreEqual(expectedTotalRevenue, actualTotalRevenue, "Total revenue failed.");

            //check carrier total segments
            var expectedCarrierSegs = 12;
            var actualCarrierSegs = rpt.FinalDataList.Sum(x => x.Carr1Segs);
            Assert.AreEqual(expectedCarrierSegs, actualCarrierSegs, "Carrier segments total failed.");

            //check carrier total revenue
            var expectedCarrierRevenue = 2277.81M;
            var actualCarrierRevenue = rpt.FinalDataList.Sum(x => x.Carr1Fare);
            Assert.AreEqual(expectedCarrierRevenue, actualCarrierRevenue, "Carrier revenue failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportInvoiceDate()
        {
            GenerateReservationHandoffRecords(DateType.InvoiceDate);

            InsertReportHandoff();

            var rpt = (Market)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var expectedTotalRecs = 40;
            var actualTotalRecs = CountDistinctOriginDest(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check for total segments
            var expectedTotalSegs = 72;
            var actualTotalSegs = rpt.FinalDataList.Sum(x => x.Segments);
            Assert.AreEqual(expectedTotalSegs, actualTotalSegs, "Total Segments failed.");

            //check total revenue
            var expectedTotalRevenue = 61212.07M;
            var actualTotalRevenue = rpt.FinalDataList.Sum(x => x.Fare);
            Assert.AreEqual(expectedTotalRevenue, actualTotalRevenue, "Total revenue failed.");

            //check carrier total segments
            var expectedCarrierSegs = 0;
            var actualCarrierSegs = rpt.FinalDataList.Sum(x => x.Carr1Segs);
            Assert.AreEqual(expectedCarrierSegs, actualCarrierSegs, "Carrier segments total failed.");

            //check carrier total revenue
            var expectedCarrierRevenue = 0M;
            var actualCarrierRevenue = rpt.FinalDataList.Sum(x => x.Carr1Fare);
            Assert.AreEqual(expectedCarrierRevenue, actualCarrierRevenue, "Carrier revenue failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportBookedDate()
        {
            GenerateReservationBookedDateHandoffRecords();

            InsertReportHandoff();

            var rpt = (Market)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var expectedTotalRecs = 40;
            var actualTotalRecs = CountDistinctOriginDest(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check for total segments
            var expectedTotalSegs = 72;
            var actualTotalSegs = rpt.FinalDataList.Sum(x => x.Segments);
            Assert.AreEqual(expectedTotalSegs, actualTotalSegs, "Total Segments failed.");

            //check total revenue
            var expectedTotalRevenue = 12327.54M;
            var actualTotalRevenue = rpt.FinalDataList.Sum(x => x.Fare);
            Assert.AreEqual(expectedTotalRevenue, actualTotalRevenue, "Total revenue failed.");

            //check carrier total segments
            var expectedCarrierSegs = 9;
            var actualCarrierSegs = rpt.FinalDataList.Sum(x => x.Carr1Segs);
            Assert.AreEqual(expectedCarrierSegs, actualCarrierSegs, "Carrier segments total failed.");

            //check carrier total revenue
            var expectedCarrierRevenue = 1067.44M;
            var actualCarrierRevenue = rpt.FinalDataList.Sum(x => x.Carr1Fare);
            Assert.AreEqual(expectedCarrierRevenue, actualCarrierRevenue, "Carrier revenue failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportSortByFlightMarket()
        {
            GenerateReservationHandoffRecords();

            InsertReportHandoff();

            var rpt = (Market)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var expectedTotalRecs = 112;
            var actualTotalRecs = CountDistinctOriginDest(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check first flight market
            var expectedFirstFlightMarket = "ABU DHABI,TC";
            var actualFirstFlightMarket = rpt.FinalDataList.OrderBy(x => x.OrgDesc).First().OrgDesc.Trim();
            Assert.AreEqual(expectedFirstFlightMarket, actualFirstFlightMarket, "First flight market failed.");

            //check last flight market
            var expectedLastFlightMarket = "WAS-NATIONAL,DC";
            var actualLastFlightMarket = rpt.FinalDataList.OrderBy(x => x.OrgDesc).Last().OrgDesc.Trim();
            Assert.AreEqual(expectedLastFlightMarket, actualLastFlightMarket, "Last flight market failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportSortByTotalNmbrSegs()
        {
            GenerateReservationHandoffRecords(DateType.DepartureDate, SortBy.TotalNumberSegments);

            InsertReportHandoff();

            var rpt = (Market)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var expectedTotalRecs = 112;
            var actualTotalRecs = CountDistinctOriginDest(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check first flight market
            var expectedFirstFlightMarket = "NEWARK,NJ";
            var actualFirstFlightMarket = rpt.FinalDataList.OrderBy(x => x.OrgDesc).First().OrgDesc.Trim();
            Assert.AreEqual(expectedFirstFlightMarket, actualFirstFlightMarket, "First flight market failed.");

            //check last flight market
            var expectedLastFlightMarket = "WAS-NATIONAL,DC";
            var actualLastFlightMarket = rpt.FinalDataList.OrderBy(x => x.OrgDesc).Last().OrgDesc.Trim();
            Assert.AreEqual(expectedLastFlightMarket, actualLastFlightMarket, "Last flight market failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportSortByTotalRevenue()
        {
            GenerateReservationHandoffRecords(DateType.DepartureDate, SortBy.TotalRevenue);

            InsertReportHandoff();

            var rpt = (Market)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var expectedTotalRecs = 112;
            var actualTotalRecs = CountDistinctOriginDest(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check first flight market
            var expectedFirstFlightMarket = "LONDON-HEATHROW,UK";
            var actualFirstFlightMarket = rpt.FinalDataList.OrderBy(x => x.OrgDesc).First().OrgDesc.Trim();
            Assert.AreEqual(expectedFirstFlightMarket, actualFirstFlightMarket, "First flight market failed.");

            //check last flight market
            var expectedLastFlightMarket = "ORLANDO,FL";
            var actualLastFlightMarket = rpt.FinalDataList.OrderBy(x => x.OrgDesc).Last().OrgDesc.Trim();
            Assert.AreEqual(expectedLastFlightMarket, actualLastFlightMarket, "Last flight market failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportUseAirportCodes()
        {
            GenerateReservationHandoffRecords(DateType.DepartureDate, SortBy.FlightMarket, true);

            InsertReportHandoff();

            var rpt = (Market)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var expectedTotalRecs = 112;
            var actualTotalRecs = CountDistinctOriginDest(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check first flight market
            var expectedFirstFlightMarket = "ABY";
            var actualFirstFlightMarket = rpt.FinalDataList.OrderBy(x => x.Origin).First().Origin.Trim();
            Assert.AreEqual(expectedFirstFlightMarket, actualFirstFlightMarket, "First flight market failed.");

            //check last flight market
            var expectedLastFlightMarket = "PHL";
            var actualLastFlightMarket = rpt.FinalDataList.OrderBy(x => x.Origin).Last().Origin.Trim();
            Assert.AreEqual(expectedLastFlightMarket, actualLastFlightMarket, "Last flight market failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportCalcSegFareFromMileage()
        {
            GenerateReservationHandoffRecords(DateType.DepartureDate, SortBy.FlightMarket, false, true);

            InsertReportHandoff();

            var rpt = (Market)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check total revenue
            var expectedTotalRevenue = 136999.21M;
            var actualTotalRevenue = rpt.FinalDataList.Sum(x => x.Fare);
            Assert.AreEqual(expectedTotalRevenue, actualTotalRevenue, "Total revenue failed.");

            //check carrier total revenue
            var expectedCarrierRevenue = 1967.13M;
            var actualCarrierRevenue = rpt.FinalDataList.Sum(x => x.Carr1Fare);
            Assert.AreEqual(expectedCarrierRevenue, actualCarrierRevenue, "Carrier revenue failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportUseMileageFromIndustryTable()
        {
            GenerateReservationHandoffRecords(DateType.DepartureDate, SortBy.FlightMarket, false, true, true);

            InsertReportHandoff();

            var rpt = (Market)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check total revenue
            var expectedTotalRevenue = 136999.21M;
            var actualTotalRevenue = rpt.FinalDataList.Sum(x => x.Fare);
            Assert.AreEqual(expectedTotalRevenue, actualTotalRevenue, "Total revenue failed.");

            //check carrier total revenue
            var expectedCarrierRevenue = 1967.37M;
            var actualCarrierRevenue = rpt.FinalDataList.Sum(x => x.Carr1Fare);
            Assert.AreEqual(expectedCarrierRevenue, actualCarrierRevenue, "Carrier revenue failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportTreatMarketsOneWay()
        {
            GenerateReservationHandoffRecords(DateType.DepartureDate, SortBy.FlightMarket, false, false, false, true);

            InsertReportHandoff();

            var rpt = (Market)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var expectedTotalRecs = 169;
            var actualTotalRecs = rpt.FinalDataList.Count;
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check for total segments
            var expectedTotalSegs = 273;
            var actualTotalSegs = rpt.FinalDataList.Sum(x => x.Segments);
            Assert.AreEqual(expectedTotalSegs, actualTotalSegs, "Total Segments failed.");

            //check total revenue
            var expectedTotalRevenue = 150697.45M;
            var actualTotalRevenue = rpt.FinalDataList.Sum(x => x.Fare);
            Assert.AreEqual(expectedTotalRevenue, actualTotalRevenue, "Total revenue failed.");

            //check carrier total segments
            var expectedCarrierSegs = 12;
            var actualCarrierSegs = rpt.FinalDataList.Sum(x => x.Carr1Segs);
            Assert.AreEqual(expectedCarrierSegs, actualCarrierSegs, "Carrier segments total failed.");

            //check carrier total revenue
            var expectedCarrierRevenue = 2277.81M;
            var actualCarrierRevenue = rpt.FinalDataList.Sum(x => x.Carr1Fare);
            Assert.AreEqual(expectedCarrierRevenue, actualCarrierRevenue, "Carrier revenue failed.");

            //check first flight market
            var expectedFirstFlightMarket = "ABU DHABI,TC";
            var actualFirstFlightMarket = rpt.FinalDataList.OrderBy(x => x.OrgDesc).First().OrgDesc.Trim();
            Assert.AreEqual(expectedFirstFlightMarket, actualFirstFlightMarket, "First flight market failed.");

            //check last flight market
            var expectedLastFlightMarket = "WAS-NATIONAL,DC";
            var actualLastFlightMarket = rpt.FinalDataList.OrderBy(x => x.OrgDesc).Last().OrgDesc.Trim();
            Assert.AreEqual(expectedLastFlightMarket, actualLastFlightMarket, "Last flight market failed.");

            ClearReportHandoff();
        }
        
        private void GenerateReservationHandoffRecords(DateType dateType = DateType.DepartureDate, SortBy sortBy = SortBy.FlightMarket,bool useAirportCodes = false,
                                                       bool calcSegFareFromMileage = false, bool useMileageFromIndustryTable = false, bool treatMarketsOneWay = false)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,3,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,3,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "1100,1188,1200", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INAIRLINE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETRODESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETROORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVCRED", ParmValue = "ALL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MODE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "GBP", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibMarket", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBFLTMKTONEWAYBOTHWAYS", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3373810", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SEGCARR1", ParmValue = "AA", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SEGCARR2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SEGCARR3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TXTFLTSEGMENTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });

            if (dateType != DateType.DepartureDate) ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");
            if (sortBy != SortBy.FlightMarket) ManipulateReportHandoffRecords(((int)sortBy).ToString(), "SORTBY");
            if (useAirportCodes) ManipulateReportHandoffRecords("ON", "CBUSEAIRPORTCODES");
            if (calcSegFareFromMileage) ManipulateReportHandoffRecords("ON", "SEGFAREMILEAGE");
            if (useMileageFromIndustryTable) ManipulateReportHandoffRecords("ON", "MILEAGETABLE");
            if (treatMarketsOneWay) ManipulateReportHandoffRecords("RBFLTMKTONEWAYBOTHWAYS", "2");
        }

        private void GenerateReservationBookedDateHandoffRecords()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,1,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,1,4", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "1100,1188,1200", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INAIRLINE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETRODESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETROORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MODE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "GBP", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibMarket", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBFLTMKTONEWAYBOTHWAYS", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3373890", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SEGCARR1", ParmValue = "AA", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SEGCARR2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SEGCARR3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TXTFLTSEGMENTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });
        }

        #endregion

        #region Region - Back Office

        [TestMethod]
        public void BackOfficeReportDepartureDate()
        {
            GenerateBackOfficeHandoffRecords();

            InsertReportHandoff();

            var rpt = (Market)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var expectedTotalRecs = 150;
            var actualTotalRecs = CountDistinctOriginDest(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check for total segments
            var expectedTotalSegs = 964;
            var actualTotalSegs = rpt.FinalDataList.Sum(x => x.Segments);
            Assert.AreEqual(expectedTotalSegs, actualTotalSegs, "Total Segments failed.");

            //check total revenue
            var expectedTotalRevenue = 172198.65M;
            var actualTotalRevenue = rpt.FinalDataList.Sum(x => x.Fare);
            Assert.AreEqual(expectedTotalRevenue, actualTotalRevenue, "Total revenue failed.");

            //check carrier total segments
            var expectedCarrierSegs = 25;
            var actualCarrierSegs = rpt.FinalDataList.Sum(x => x.Carr1Segs);
            Assert.AreEqual(expectedCarrierSegs, actualCarrierSegs, "Carrier segments total failed.");

            //check carrier total revenue
            var expectedCarrierRevenue = 2762.46M;
            var actualCarrierRevenue = rpt.FinalDataList.Sum(x => x.Carr1Fare);
            Assert.AreEqual(expectedCarrierRevenue, actualCarrierRevenue, "Carrier revenue failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeReportInvoiceDate()
        {
            GenerateBackOfficeHandoffRecords(DateType.InvoiceDate);

            InsertReportHandoff();

            var rpt = (Market)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var expectedTotalRecs = 141;
            var actualTotalRecs = CountDistinctOriginDest(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check for total segments
            var expectedTotalSegs = 758;
            var actualTotalSegs = rpt.FinalDataList.Sum(x => x.Segments);
            Assert.AreEqual(expectedTotalSegs, actualTotalSegs, "Total Segments failed.");

            //check total revenue
            var expectedTotalRevenue = 137959.38M;
            var actualTotalRevenue = rpt.FinalDataList.Sum(x => x.Fare);
            Assert.AreEqual(expectedTotalRevenue, actualTotalRevenue, "Total revenue failed.");

            //check carrier total segments
            var expectedCarrierSegs = 31;
            var actualCarrierSegs = rpt.FinalDataList.Sum(x => x.Carr1Segs);
            Assert.AreEqual(expectedCarrierSegs, actualCarrierSegs, "Carrier segments total failed.");

            //check carrier total revenue
            var expectedCarrierRevenue = 3196.18M;
            var actualCarrierRevenue = rpt.FinalDataList.Sum(x => x.Carr1Fare);
            Assert.AreEqual(expectedCarrierRevenue, actualCarrierRevenue, "Carrier revenue failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeReportSortByFlightMarket()
        {
            GenerateBackOfficeHandoffRecords();

            InsertReportHandoff();

            var rpt = (Market)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var expectedTotalRecs = 150;
            var actualTotalRecs = CountDistinctOriginDest(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check first flight market
            var expectedFirstFlightMarket = "AMSTERDAM,NL";
            var actualFirstFlightMarket = rpt.FinalDataList.OrderBy(x => x.OrgDesc).First().OrgDesc.Trim();
            Assert.AreEqual(expectedFirstFlightMarket, actualFirstFlightMarket, "First flight market failed.");

            //check last flight market
            var expectedLastFlightMarket = "WINNIPEG MB,CA";
            var actualLastFlightMarket = rpt.FinalDataList.OrderBy(x => x.OrgDesc).Last().OrgDesc.Trim();
            Assert.AreEqual(expectedLastFlightMarket, actualLastFlightMarket, "Last flight market failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeReportSortByTotalNumberSegs()
        {
            GenerateBackOfficeHandoffRecords(DateType.DepartureDate, SortBy.TotalNumberSegments);

            InsertReportHandoff();

            var rpt = (Market)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var expectedTotalRecs = 150;
            var actualTotalRecs = CountDistinctOriginDest(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check first flight market
            var expectedFirstFlightMarket = "VANCOUVER BC,CA";
            var actualFirstFlightMarket = rpt.FinalDataList.OrderBy(x => x.OrgDesc).First().OrgDesc.Trim();
            Assert.AreEqual(expectedFirstFlightMarket, actualFirstFlightMarket, "First flight market failed.");

            //check last flight market
            var expectedLastFlightMarket = "NEWARK,NJ";
            var actualLastFlightMarket = rpt.FinalDataList.OrderBy(x => x.OrgDesc).Last().OrgDesc.Trim();
            Assert.AreEqual(expectedLastFlightMarket, actualLastFlightMarket, "Last flight market failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeReportSortByTotalRevenue()
        {
            GenerateBackOfficeHandoffRecords(DateType.DepartureDate, SortBy.TotalRevenue);

            InsertReportHandoff();

            var rpt = (Market)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var expectedTotalRecs = 150;
            var actualTotalRecs = CountDistinctOriginDest(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check first flight market
            var expectedFirstFlightMarket = "VANCOUVER BC,CA";
            var actualFirstFlightMarket = rpt.FinalDataList.OrderBy(x => x.OrgDesc).First().OrgDesc.Trim();
            Assert.AreEqual(expectedFirstFlightMarket, actualFirstFlightMarket, "First flight market failed.");

            //check last flight market
            var expectedLastFlightMarket = "NEWARK,NJ";
            var actualLastFlightMarket = rpt.FinalDataList.OrderBy(x => x.OrgDesc).Last().OrgDesc.Trim();
            Assert.AreEqual(expectedLastFlightMarket, actualLastFlightMarket, "Last flight market failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeReportUseAirportCodes()
        {
            GenerateBackOfficeHandoffRecords(DateType.DepartureDate, SortBy.FlightMarket, true);

            InsertReportHandoff();

            var rpt = (Market)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var expectedTotalRecs = 150;
            var actualTotalRecs = CountDistinctOriginDest(rpt.FinalDataList);
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check first flight market
            var expectedFirstFlightMarket = "AKL";
            var actualFirstFlightMarket = rpt.FinalDataList.OrderBy(x => x.Origin).First().Origin.Trim();
            Assert.AreEqual(expectedFirstFlightMarket, actualFirstFlightMarket, "First flight market failed.");

            //check last flight market
            var expectedLastFlightMarket = "YYZ";
            var actualLastFlightMarket = rpt.FinalDataList.OrderBy(x => x.Origin).Last().Origin.Trim();
            Assert.AreEqual(expectedLastFlightMarket, actualLastFlightMarket, "Last flight market failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeReportCalcSegFareFromMileage()
        {
            GenerateBackOfficeHandoffRecords(DateType.DepartureDate, SortBy.FlightMarket, false, true);

            InsertReportHandoff();

            var rpt = (Market)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check total revenue
            var expectedTotalRevenue = 184215.34M;
            var actualTotalRevenue = rpt.FinalDataList.Sum(x => x.Fare);
            Assert.AreEqual(expectedTotalRevenue, actualTotalRevenue, "Total revenue failed.");

            //check carrier total revenue
            var expectedCarrierRevenue = 3098.89M;
            var actualCarrierRevenue = rpt.FinalDataList.Sum(x => x.Carr1Fare);
            Assert.AreEqual(expectedCarrierRevenue, actualCarrierRevenue, "Carrier revenue failed.");

            ClearReportHandoff();
        }
        
        [TestMethod]
        public void BackOfficeReportUseMileageFromIndustryTable()
        {
            GenerateBackOfficeHandoffRecords(DateType.DepartureDate, SortBy.FlightMarket, false, true, true);

            InsertReportHandoff();

            var rpt = (Market)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check total revenue
            var expectedTotalRevenue = 184215.34M;
            var actualTotalRevenue = rpt.FinalDataList.Sum(x => x.Fare);
            Assert.AreEqual(expectedTotalRevenue, actualTotalRevenue, "Total revenue failed.");

            //check carrier total revenue
            var expectedCarrierRevenue = 3094.69M;
            var actualCarrierRevenue = rpt.FinalDataList.Sum(x => x.Carr1Fare);
            Assert.AreEqual(expectedCarrierRevenue, actualCarrierRevenue, "Carrier revenue failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeTreatMarketsOneWay()
        {
            GenerateBackOfficeHandoffRecords(DateType.DepartureDate, SortBy.FlightMarket, false, false, false, true);

            InsertReportHandoff();

            var rpt = (Market)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for total records
            var expectedTotalRecs = 247;
            var actualTotalRecs = rpt.FinalDataList.Count;
            Assert.AreEqual(expectedTotalRecs, actualTotalRecs, "Total records failed.");

            //check for total segments
            var expectedTotalSegs = 964;
            var actualTotalSegs = rpt.FinalDataList.Sum(x => x.Segments);
            Assert.AreEqual(expectedTotalSegs, actualTotalSegs, "Total Segments failed.");

            //check total revenue
            var expectedTotalRevenue = 172198.65M;
            var actualTotalRevenue = rpt.FinalDataList.Sum(x => x.Fare);
            Assert.AreEqual(expectedTotalRevenue, actualTotalRevenue, "Total revenue failed.");

            //check carrier total segments
            var expectedCarrierSegs = 25;
            var actualCarrierSegs = rpt.FinalDataList.Sum(x => x.Carr1Segs);
            Assert.AreEqual(expectedCarrierSegs, actualCarrierSegs, "Carrier segments total failed.");

            //check carrier total revenue
            var expectedCarrierRevenue = 2762.46M;
            var actualCarrierRevenue = rpt.FinalDataList.Sum(x => x.Carr1Fare);
            Assert.AreEqual(expectedCarrierRevenue, actualCarrierRevenue, "Carrier revenue failed.");

            //check first flight market
            var expectedFirstFlightMarket = "AMSTERDAM,NL";
            var actualFirstFlightMarket = rpt.FinalDataList.OrderBy(x => x.OrgDesc).First().OrgDesc.Trim();
            Assert.AreEqual(expectedFirstFlightMarket, actualFirstFlightMarket, "First flight market failed.");

            //check last flight market
            var expectedLastFlightMarket = "ZURICH,SZ";
            var actualLastFlightMarket = rpt.FinalDataList.OrderBy(x => x.OrgDesc).Last().OrgDesc.Trim();
            Assert.AreEqual(expectedLastFlightMarket, actualLastFlightMarket, "Last flight market failed.");

            ClearReportHandoff();
        }

        private void GenerateBackOfficeHandoffRecords(DateType dateType = DateType.DepartureDate, SortBy sortBy = SortBy.FlightMarket, bool useAirportCodes = false,
                                                       bool calcSegFareFromMileage = false, bool useMileageFromIndustryTable = false, bool treatMarketsOneWay = false)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,1,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,3,2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INAIRLINE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETRODESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETROORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MODE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "GBP", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibMarket", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBFLTMKTONEWAYBOTHWAYS", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3373840", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SEGCARR1", ParmValue = "DL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SEGCARR2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SEGCARR3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TXTFLTSEGMENTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });

            if (dateType != DateType.DepartureDate) ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");
            if (sortBy != SortBy.FlightMarket) ManipulateReportHandoffRecords(((int)sortBy).ToString(), "SORTBY");
            if (useAirportCodes) ManipulateReportHandoffRecords("ON", "CBUSEAIRPORTCODES");
            if (calcSegFareFromMileage) ManipulateReportHandoffRecords("ON", "SEGFAREMILEAGE");
            if (useMileageFromIndustryTable) ManipulateReportHandoffRecords("ON", "MILEAGETABLE");
            if (treatMarketsOneWay) ManipulateReportHandoffRecords("RBFLTMKTONEWAYBOTHWAYS", "2");
        }

        #endregion

        private int CountDistinctOriginDest(List<FinalData> finalDataList)
        {
            var dict = new Dictionary<string, int>();

            foreach (var item in finalDataList)
            {
                var mkt = item.OrgDesc.Trim() + "-" + item.DestDesc.Trim();

                if (dict.ContainsKey(mkt)) dict[mkt]++;
                else
                {
                    dict.Add(mkt, 1);
                }
            }

            return dict.Keys.Count;
        }
    }
}
