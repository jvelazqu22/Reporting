using System.Linq;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms;
using iBank.Services.Implementation.ReportPrograms.CCRecon;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    /*
        Back Office Only Report

        Items to test:
            SortBy
                Airline Number/Ticket Number
                Airline Number/Traveler
                Airline Number/Transaction Date
                Ticket Number
                Transaction Date
                Transaction Date/Airline
                Transaction Date/Airline/Traveler
                Transaction Date/Traveler/Airline
                Traveler
                Traveler/Airline
                Traveler/Airline/Transaction Date

            No data found
            Push to offline
                
        Standard Parameters:
            Dates: 12/1/15 - 12/2/15
            Accts: 1100, 1188, 1200

        No data parameters: Report Id: d84-CA440F96-02DB-D36F-4D70CD28B7FF6E79_55_42387.keystonecf1
            Date: 2/1/16 - 2/24/16
            Account: 1100

        Offline report parameters: Report Id: 344-CA57DDA6-DC63-ED96-A5DC18366AC8AE7D_55_42517.keystonecf1
            Date: 2/1/16 - 2/24/16
            Account: not set

        Report Id: 848-9EDB7B36-DDAC-B4D3-2586388464A68570_54_55964.keystonecf1
            
        Create a report with the desired criteria through iBank. Generate a set of reporthandoff objects using this sql script:

        select 'ReportHandoff.Add(new ReportHandoffInformation{ParmName = "' + rtrim(ltrim(Parmname)) + '",ParmValue = "'+rtrim(ltrim(parmvalue))+'",ParmInOut = "' + rtrim(ltrim(parminout))+'", LangCode = "' + rtrim(ltrim(Langcode)) + '"});' 
        from reporthandoff where reportid = '<REPORT ID GOES HERE>' and parminout = 'IN' order by parmname
    */

    [TestClass]
    public class CCReconTests : BaseUnitTest
    {
        private enum SortBy
        {
            AirlineNumber_TicketNumber = 1,
            AirlineNumber_Traveler = 2,
            AirlineNumber_TransactionDate = 3,
            TicketNumber = 4,
            TransactionDate = 5,
            TransactionDate_Airline = 6,
            TransactionDate_Airline_Traveler = 7,
            TransactionDate_Traveler_Airline = 11,
            Traveler = 8,
            Traveler_Airline = 9,
            Traveler_Airline_TransactionDate = 10
        }

        [TestMethod]
        public void GenerateReportSortByAirlineNumberTicketNumber()
        {
            GenerateReportHandoffRecords(SortBy.AirlineNumber_TicketNumber);

            InsertReportHandoff();

            //run the report
            var rpt = (CcRecon)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for right number of records
            Assert.AreEqual(83, rpt.FinalDataList.Count, "Number of records failed.");

            //check for order
            var expectedFirstInvoice = "33419099";
            var actualFirstInvoice = rpt.FinalDataList[0].Invoice.Trim();
            Assert.AreEqual(expectedFirstInvoice, actualFirstInvoice, "Ordering failed.");

            var expectedLastInvoice = "33419089";
            var actualLastInvoice = rpt.FinalDataList.Last().Invoice.Trim();
            Assert.AreEqual(expectedLastInvoice, actualLastInvoice, "Ordering failed.");

            //check for total credit cards
            var expectedTotalCreditCards = 26;
            var actualTotalCreditCards = rpt.FinalDataList.Select(x => x.Cardnum).Distinct().Count();
            Assert.AreEqual(expectedTotalCreditCards, actualTotalCreditCards, "Distinct credit cards failed.");

            //check for total credit card charges
            var expectedTotalCharges = 62855.52M;
            var actualTotalCharges = rpt.FinalDataList.Sum(x => x.Cctransamt);
            Assert.AreEqual(expectedTotalCharges, actualTotalCharges, "Total charges failed.");
            
            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportSortByAirlineNumberTraveler()
        {
            GenerateReportHandoffRecords(SortBy.AirlineNumber_Traveler);

            InsertReportHandoff();

            //run the report
            var rpt = (CcRecon)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for right number of records
            Assert.AreEqual(83, rpt.FinalDataList.Count, "Number of records failed.");

            //check for order
            var expectedFirstInvoice = "33419099";
            var actualFirstInvoice = rpt.FinalDataList[0].Invoice.Trim();
            Assert.AreEqual(expectedFirstInvoice, actualFirstInvoice, "Ordering failed.");

            var expectedLastInvoice = "33419089";
            var actualLastInvoice = rpt.FinalDataList.Last().Invoice.Trim();
            Assert.AreEqual(expectedLastInvoice, actualLastInvoice, "Ordering failed.");

            //check for total credit cards
            var expectedTotalCreditCards = 26;
            var actualTotalCreditCards = rpt.FinalDataList.Select(x => x.Cardnum).Distinct().Count();
            Assert.AreEqual(expectedTotalCreditCards, actualTotalCreditCards, "Distinct credit cards failed.");

            //check for total credit card charges
            var expectedTotalCharges = 62855.52M;
            var actualTotalCharges = rpt.FinalDataList.Sum(x => x.Cctransamt);
            Assert.AreEqual(expectedTotalCharges, actualTotalCharges, "Total charges failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportSortByAirlineNumberTransactionDate()
        {
            GenerateReportHandoffRecords(SortBy.AirlineNumber_TransactionDate);

            InsertReportHandoff();

            //run the report
            var rpt = (CcRecon)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for right number of records
            Assert.AreEqual(83, rpt.FinalDataList.Count, "Number of records failed.");

            //check for order
            var expectedFirstInvoice = "33419099";
            var actualFirstInvoice = rpt.FinalDataList[0].Invoice.Trim();
            Assert.AreEqual(expectedFirstInvoice, actualFirstInvoice, "Ordering failed.");

            var expectedLastInvoice = "33419089";
            var actualLastInvoice = rpt.FinalDataList.Last().Invoice.Trim();
            Assert.AreEqual(expectedLastInvoice, actualLastInvoice, "Ordering failed.");

            //check for total credit cards
            var expectedTotalCreditCards = 26;
            var actualTotalCreditCards = rpt.FinalDataList.Select(x => x.Cardnum).Distinct().Count();
            Assert.AreEqual(expectedTotalCreditCards, actualTotalCreditCards, "Distinct credit cards failed.");

            //check for total credit card charges
            var expectedTotalCharges = 62855.52M;
            var actualTotalCharges = rpt.FinalDataList.Sum(x => x.Cctransamt);
            Assert.AreEqual(expectedTotalCharges, actualTotalCharges, "Total charges failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportSortByTicketNumber()
        {
            GenerateReportHandoffRecords(SortBy.TicketNumber);

            InsertReportHandoff();

            //run the report
            var rpt = (CcRecon)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for right number of records
            Assert.AreEqual(83, rpt.FinalDataList.Count, "Number of records failed.");

            //check for order
            var expectedFirstInvoice = "33419056";
            var actualFirstInvoice = rpt.FinalDataList[0].Invoice.Trim();
            Assert.AreEqual(expectedFirstInvoice, actualFirstInvoice, "Ordering failed.");

            var expectedLastInvoice = "33419089";
            var actualLastInvoice = rpt.FinalDataList.Last().Invoice.Trim();
            Assert.AreEqual(expectedLastInvoice, actualLastInvoice, "Ordering failed.");

            //check for total credit cards
            var expectedTotalCreditCards = 26;
            var actualTotalCreditCards = rpt.FinalDataList.Select(x => x.Cardnum).Distinct().Count();
            Assert.AreEqual(expectedTotalCreditCards, actualTotalCreditCards, "Distinct credit cards failed.");

            //check for total credit card charges
            var expectedTotalCharges = 62855.52M;
            var actualTotalCharges = rpt.FinalDataList.Sum(x => x.Cctransamt);
            Assert.AreEqual(expectedTotalCharges, actualTotalCharges, "Total charges failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportSortByTransactionDate()
        {
            GenerateReportHandoffRecords(SortBy.TransactionDate);

            InsertReportHandoff();

            //run the report
            var rpt = (CcRecon)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for right number of records
            Assert.AreEqual(83, rpt.FinalDataList.Count, "Number of records failed.");

            //check for order
            var expectedFirstInvoice = "33418694";
            var actualFirstInvoice = rpt.FinalDataList[0].Invoice.Trim();
            Assert.AreEqual(expectedFirstInvoice, actualFirstInvoice, "Ordering failed.");

            var expectedLastInvoice = "33419089";
            var actualLastInvoice = rpt.FinalDataList.Last().Invoice.Trim();
            Assert.AreEqual(expectedLastInvoice, actualLastInvoice, "Ordering failed.");

            //check for total credit cards
            var expectedTotalCreditCards = 26;
            var actualTotalCreditCards = rpt.FinalDataList.Select(x => x.Cardnum).Distinct().Count();
            Assert.AreEqual(expectedTotalCreditCards, actualTotalCreditCards, "Distinct credit cards failed.");

            //check for total credit card charges
            var expectedTotalCharges = 62855.52M;
            var actualTotalCharges = rpt.FinalDataList.Sum(x => x.Cctransamt);
            Assert.AreEqual(expectedTotalCharges, actualTotalCharges, "Total charges failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportSortByTransactionDateAirline()
        {
            GenerateReportHandoffRecords(SortBy.TransactionDate_Airline);

            InsertReportHandoff();

            //run the report
            var rpt = (CcRecon)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for right number of records
            Assert.AreEqual(83, rpt.FinalDataList.Count, "Number of records failed.");

            //check for order
            var expectedFirstInvoice = "33418694";
            var actualFirstInvoice = rpt.FinalDataList[0].Invoice.Trim();
            Assert.AreEqual(expectedFirstInvoice, actualFirstInvoice, "Ordering failed.");

            var expectedLastInvoice = "33419089";
            var actualLastInvoice = rpt.FinalDataList.Last().Invoice.Trim();
            Assert.AreEqual(expectedLastInvoice, actualLastInvoice, "Ordering failed.");

            //check for total credit cards
            var expectedTotalCreditCards = 26;
            var actualTotalCreditCards = rpt.FinalDataList.Select(x => x.Cardnum).Distinct().Count();
            Assert.AreEqual(expectedTotalCreditCards, actualTotalCreditCards, "Distinct credit cards failed.");

            //check for total credit card charges
            var expectedTotalCharges = 62855.52M;
            var actualTotalCharges = rpt.FinalDataList.Sum(x => x.Cctransamt);
            Assert.AreEqual(expectedTotalCharges, actualTotalCharges, "Total charges failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportSortByTransactionDateAirlineTraveler()
        {
            GenerateReportHandoffRecords(SortBy.TransactionDate_Airline_Traveler);

            InsertReportHandoff();

            //run the report
            var rpt = (CcRecon)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for right number of records
            Assert.AreEqual(83, rpt.FinalDataList.Count, "Number of records failed.");

            //check for order
            var expectedFirstInvoice = "33418694";
            var actualFirstInvoice = rpt.FinalDataList[0].Invoice.Trim();
            Assert.AreEqual(expectedFirstInvoice, actualFirstInvoice, "Ordering failed.");

            var expectedLastInvoice = "33419089";
            var actualLastInvoice = rpt.FinalDataList.Last().Invoice.Trim();
            Assert.AreEqual(expectedLastInvoice, actualLastInvoice, "Ordering failed.");

            //check for total credit cards
            var expectedTotalCreditCards = 26;
            var actualTotalCreditCards = rpt.FinalDataList.Select(x => x.Cardnum).Distinct().Count();
            Assert.AreEqual(expectedTotalCreditCards, actualTotalCreditCards, "Distinct credit cards failed.");

            //check for total credit card charges
            var expectedTotalCharges = 62855.52M;
            var actualTotalCharges = rpt.FinalDataList.Sum(x => x.Cctransamt);
            Assert.AreEqual(expectedTotalCharges, actualTotalCharges, "Total charges failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportSortByTransactionDateTravelerAirline()
        {
            GenerateReportHandoffRecords(SortBy.TransactionDate_Traveler_Airline);

            InsertReportHandoff();

            //run the report
            var rpt = (CcRecon)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for right number of records
            Assert.AreEqual(83, rpt.FinalDataList.Count, "Number of records failed.");

            //check for order
            var expectedFirstInvoice = "33418694";
            var actualFirstInvoice = rpt.FinalDataList[0].Invoice.Trim();
            Assert.AreEqual(expectedFirstInvoice, actualFirstInvoice, "Ordering failed.");

            var expectedLastInvoice = "33419089";
            var actualLastInvoice = rpt.FinalDataList.Last().Invoice.Trim();
            Assert.AreEqual(expectedLastInvoice, actualLastInvoice, "Ordering failed.");

            //check for total credit cards
            var expectedTotalCreditCards = 26;
            var actualTotalCreditCards = rpt.FinalDataList.Select(x => x.Cardnum).Distinct().Count();
            Assert.AreEqual(expectedTotalCreditCards, actualTotalCreditCards, "Distinct credit cards failed.");

            //check for total credit card charges
            var expectedTotalCharges = 62855.52M;
            var actualTotalCharges = rpt.FinalDataList.Sum(x => x.Cctransamt);
            Assert.AreEqual(expectedTotalCharges, actualTotalCharges, "Total charges failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportSortByTraveler()
        {
            GenerateReportHandoffRecords(SortBy.Traveler);

            InsertReportHandoff();

            //run the report
            var rpt = (CcRecon)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for right number of records
            Assert.AreEqual(83, rpt.FinalDataList.Count, "Number of records failed.");

            //check for order
            var expectedFirstInvoice = "33419418";
            var actualFirstInvoice = rpt.FinalDataList[0].Invoice.Trim();
            Assert.AreEqual(expectedFirstInvoice, actualFirstInvoice, "Ordering failed.");

            var expectedLastInvoice = "33419089";
            var actualLastInvoice = rpt.FinalDataList.Last().Invoice.Trim();
            Assert.AreEqual(expectedLastInvoice, actualLastInvoice, "Ordering failed.");

            //check for total credit cards
            var expectedTotalCreditCards = 26;
            var actualTotalCreditCards = rpt.FinalDataList.Select(x => x.Cardnum).Distinct().Count();
            Assert.AreEqual(expectedTotalCreditCards, actualTotalCreditCards, "Distinct credit cards failed.");

            //check for total credit card charges
            var expectedTotalCharges = 62855.52M;
            var actualTotalCharges = rpt.FinalDataList.Sum(x => x.Cctransamt);
            Assert.AreEqual(expectedTotalCharges, actualTotalCharges, "Total charges failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportSortByTravelerAirline()
        {
            GenerateReportHandoffRecords(SortBy.Traveler_Airline);

            InsertReportHandoff();

            //run the report
            var rpt = (CcRecon)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for right number of records
            Assert.AreEqual(83, rpt.FinalDataList.Count, "Number of records failed.");

            //check for order
            var expectedFirstInvoice = "33419418";
            var actualFirstInvoice = rpt.FinalDataList[0].Invoice.Trim();
            Assert.AreEqual(expectedFirstInvoice, actualFirstInvoice, "Ordering failed.");

            var expectedLastInvoice = "33419089";
            var actualLastInvoice = rpt.FinalDataList.Last().Invoice.Trim();
            Assert.AreEqual(expectedLastInvoice, actualLastInvoice, "Ordering failed.");

            //check for total credit cards
            var expectedTotalCreditCards = 26;
            var actualTotalCreditCards = rpt.FinalDataList.Select(x => x.Cardnum).Distinct().Count();
            Assert.AreEqual(expectedTotalCreditCards, actualTotalCreditCards, "Distinct credit cards failed.");

            //check for total credit card charges
            var expectedTotalCharges = 62855.52M;
            var actualTotalCharges = rpt.FinalDataList.Sum(x => x.Cctransamt);
            Assert.AreEqual(expectedTotalCharges, actualTotalCharges, "Total charges failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportSortByTravelerAirlineTransactionDate()
        {
            GenerateReportHandoffRecords(SortBy.Traveler_Airline_TransactionDate);

            InsertReportHandoff();

            //run the report
            var rpt = (CcRecon)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for right number of records
            Assert.AreEqual(83, rpt.FinalDataList.Count, "Number of records failed.");

            //check for order
            var expectedFirstInvoice = "33419418";
            var actualFirstInvoice = rpt.FinalDataList[0].Invoice.Trim();
            Assert.AreEqual(expectedFirstInvoice, actualFirstInvoice, "Ordering failed.");

            var expectedLastInvoice = "33419089";
            var actualLastInvoice = rpt.FinalDataList.Last().Invoice.Trim();
            Assert.AreEqual(expectedLastInvoice, actualLastInvoice, "Ordering failed.");

            //check for total credit cards
            var expectedTotalCreditCards = 26;
            var actualTotalCreditCards = rpt.FinalDataList.Select(x => x.Cardnum).Distinct().Count();
            Assert.AreEqual(expectedTotalCreditCards, actualTotalCreditCards, "Distinct credit cards failed.");

            //check for total credit card charges
            var expectedTotalCharges = 62855.52M;
            var actualTotalCharges = rpt.FinalDataList.Sum(x => x.Cctransamt);
            Assert.AreEqual(expectedTotalCharges, actualTotalCharges, "Total charges failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateNoDateReport()
        {
            GenerateReportHandoffRecordsNoData();

            InsertReportHandoff();

            //run the report
            var rpt = (CcRecon)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_NoData);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateOfflineReport()
        {
            GenerateReportHandoffRecordsOfflineReport();

            InsertReportHandoff();

            //run the report
            var rpt = (CcRecon)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsOfflineMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
            Assert.AreEqual(true, containsOfflineMsg, "Error message failed.");

            ClearReportHandoff();
        }

        private void GenerateReportHandoffRecords(SortBy sortBy)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,12,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBINCLVOIDS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDCREDCARDCOMP", ParmValue = "AX", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,12,2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "1100,1188,1200", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVCRED", ParmValue = "ALL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "GBP", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTLANGUAGE", ParmValue = "EN", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "146", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibCCRecon", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3373269", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TXTCCNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)sortBy).ToString(), "SORTBY");
        }

        private void GenerateReportHandoffRecordsNoData()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,2,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBINCLVOIDS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDCREDCARDCOMP", ParmValue = "AX", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,2,24", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "1100", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVCRED", ParmValue = "ALL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "GBP", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTLANGUAGE", ParmValue = "EN", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "146", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibCCRecon", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3373336", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TXTCCNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });
        }

        private void GenerateReportHandoffRecordsOfflineReport()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,2,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBINCLVOIDS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDCREDCARDCOMP", ParmValue = "AX", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,2,24", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVCRED", ParmValue = "ALL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "GBP", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTLANGUAGE", ParmValue = "EN", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "146", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibCCRecon", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3373337", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TXTCCNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });
        }
    }
}
