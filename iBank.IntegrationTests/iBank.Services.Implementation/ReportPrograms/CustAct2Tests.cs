using System;
using System.Linq;

using Domain.Helper;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.AirActivity;
using iBank.Services.Implementation.Shared;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    #region General info
    /*
      Reservation and Back Offce Reports

      Items to test:
          Date Range Type
               Departure Date
               Invoice Date
               On-the-Road Dates
               Book Date
          Filters
               Data source: DEMOCA01 

          Apply Origin/Dest Criteria at:
               Leg Level
               Segment Level

      Standard Parameters:
          Date Range: 2016,3,1 - 2016,3,5           

      Report Id: c68-994C3C89-EEB5-77E2-71DEF1268127D573_69_39047.keystonecf1
      Return 19 records

      Create a report with the desired criteria through iBank. Generate a set of reporthandoff objects using this sql script:
      select 'ReportHandoff.Add(new ReportHandoffInformation{ParmName = "' + rtrim(ltrim(Parmname)) + '",ParmValue = "'+rtrim(ltrim(parmvalue))+'",ParmInOut = "' + rtrim(ltrim(parminout))+'", LangCode = "' + rtrim(ltrim(Langcode)) + '"});' 
      from reporthandoff where reportid = '<REPORT ID GOES HERE>' and parminout = 'IN' order by parmname
   */
    #endregion

    [TestClass]
    public class CustAct2Tests : BaseUnitTest
    {
        [TestMethod]
        public void AirActivityBackOfficeDepartDate()
        {
            GenerateHandoffRecords(DateType.DepartureDate, "DT:2016,3,1", "DT:2016,3,5", "2600");
            
            InsertReportHandoff();

            //Check the results
            var rpt = (CustAct2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            var expectTotalRecords = 19;
            int actualTotalRecords = rpt.RawDataList.Count();
            Assert.AreEqual(expectTotalRecords, actualTotalRecords, "Total record count failed");

            var firstRec = rpt.RawDataList[0];
            var lastRec = rpt.RawDataList[expectTotalRecords - 1];
            Assert.AreEqual("260002376", firstRec.Invoice.Trim(), "First record invoice failed.");
            Assert.AreEqual("1612315345", firstRec.Ticket.Trim(), "First record ticket failed.");

            Assert.AreEqual("260002396", lastRec.Invoice.Trim(), "Last record invoice failed.");
            Assert.AreEqual("1612315372", lastRec.Ticket.Trim(), "Last record ticket failed.");

            var expTotalFare = 2643.84m;
            var expNetNoTickets = 10;
            var expAvgTicketPrice = 264.38m;

            var distinctTicket = rpt.RawDataList.Select(s => new { invoice = s.Invoice, break1 = s.Break1, break2 = s.Break2, break3 = s.Break3, airchg = s.Airchg, ticket = s.Ticket }).Distinct();
            var actTotalFare = distinctTicket.Sum(s => s.airchg);
            var actNetNoTickets = distinctTicket.Count();
            var actAvgTicketPrice = (actNetNoTickets != 0) ? actTotalFare / actNetNoTickets : 0m;
            Assert.AreEqual(expNetNoTickets, actNetNoTickets, "Net # tickets failed.");            
            Assert.AreEqual(expTotalFare.ToString("#0.00"), actTotalFare.ToString("#0.00"), "Total fare failed.");
            Assert.AreEqual(expAvgTicketPrice.ToString("#0.00"), actAvgTicketPrice.ToString("#0.00"), "Avg ticket price failed.");
            
            //test on break1:CHOC-NA, break2:Sales, breaks:Demo
            var expSalesDemoTotFare = 297.23m;
            var actSalesDemoTotFare = distinctTicket.Where(s=> s.break1.Trim() =="CHOC-NA" && s.break2.Trim() == "SALES" && s.break3.Trim() == "121 DEMO")
                                                     .Sum(s => s.airchg);
            var expSalesPercent = 11.24m;
            var actSalesPercent = (actTotalFare != 0)? actSalesDemoTotFare / actTotalFare * 100 : 0m;
            Assert.AreEqual(expSalesDemoTotFare.ToString("#0.00"), actSalesDemoTotFare.ToString("#0.00"), "CHOC-NA, Sales and Demo total fare failed.");
            Assert.AreEqual(expSalesPercent.ToString("#0.00"), actSalesPercent.ToString("#0.00"), "Expected Sales fare percentage failed.");
            
            //test on break1:CHOC-NA
            var expCHOCNATotFare = 489.55m;
            var actCHOCNATotFare = distinctTicket.Where(s => s.break1.Trim() == "CHOC-NA")
                                                     .Sum(s => s.airchg);
            Assert.AreEqual(expCHOCNATotFare.ToString("#0.00"), actCHOCNATotFare.ToString("#0.00"), "CHOC-NA total fare failed.");

            var expCHOCNAPercent = 18.52m;
            var actCHOCNAPercent = (actTotalFare != 0) ? actCHOCNATotFare / actTotalFare * 100 : 0m; 
            Assert.AreEqual(expCHOCNAPercent.ToString("#0.00"), actCHOCNAPercent.ToString("#0.00"), "CHOC-NA fare percentage failed.");

        }

        [TestMethod]
        public void AirActivityBackOfficeDepartDateAltCarRailCurrency()
        {
            GenerateHandoffRecords(DateType.DepartureDate, "DT:2016,3,1", "DT:2016,3,5", "2600");
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ALTERNATEEMISSNS", ParmValue = "ON", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARBONCALC", ParmValue = "CISCARBON", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARBONEMISSIONS", ParmValue = "ON", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "GBP", ParmInOut = "IN", LangCode = "" });

            InsertReportHandoff();
            
            //Check the results
            var rpt = (CustAct2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            var expectTotalRecords = 19;
            int actualTotalRecords = rpt.RawDataList.Count();
            Assert.AreEqual(expectTotalRecords, actualTotalRecords, string.Format("Total record count = '{0}' actual count = '{1}'", expectTotalRecords, actualTotalRecords));

            var firstRec = rpt.RawDataList[0];
            var actfirstFare = firstRec.Airchg;
            var lastRec = rpt.RawDataList[expectTotalRecords - 1];
            var actlastFare = lastRec.Airchg;
            string.Format("First record AirCO2 = '190' actual amount = '{0}'", firstRec.AirCo2.ToString("#0"));
            Assert.AreEqual("190", firstRec.AirCo2.ToString("#0"), string.Format("First record AirCO2 = '190' actual amount = '{0}'", firstRec.AirCo2.ToString("#0")));
            Assert.AreEqual("14", firstRec.AltRailCo2.ToString("#0"), string.Format("First record AltRailCo2 = '14' actual amount = '{0}'", firstRec.AltRailCo2.ToString("#0")));
            Assert.AreEqual("147", firstRec.AltCarCo2.ToString("#0"), string.Format("First record AltCarCo2 = '14' actual amount = '{0}'",firstRec.AltCarCo2.ToString("#0")));
            Assert.AreEqual("135.80", actfirstFare.ToString("#0.00"), string.Format("First record Fare = '135.80' actual amount = '{0}'", actfirstFare));

            Assert.AreEqual("286", lastRec.AirCo2.ToString("#0"), string.Format("Last record AirCO2 = '286' actual count = '{0}'",lastRec.AirCo2.ToString("#0")));
            Assert.AreEqual("36", lastRec.AltRailCo2.ToString("#0"), string.Format("Last record AltRailCo2 = '36' actual amount = '{0}'",lastRec.AltRailCo2.ToString("#0")));
            Assert.AreEqual("394", lastRec.AltCarCo2.ToString("#0"), string.Format("Last record AltCarCo2 = '394' actual amount = '{0}'",lastRec.AltCarCo2.ToString("#0")));
            Assert.AreEqual("57.47", actlastFare.ToString("#0.00"), string.Format("Last record Fare = '57.47' actual amount = '{0}'",lastRec.Airchg.ToString("#0.00")));

            var expAirCo2 = "7367";
            var expAltRailCo2 = "938";
            var expAltCarCo2 = "8700";
            var expTotalFare = "1866.81";
            var actAirCo2 = rpt.RawDataList.Sum(s => s.AirCo2).ToString("#0");    
            var actAltRailCo2 = rpt.RawDataList.Sum(s => s.AltRailCo2).ToString("#0");
            var actAltCarCo2 = rpt.RawDataList.Sum(s => s.AltCarCo2).ToString("#0");
            var actTotalFare = rpt.RawDataList.Select(s => new { ticket = s.Ticket, airchg = s.Airchg }).Distinct().Sum(s=>s.airchg);
            var actFirstPct = actfirstFare / actTotalFare * 100;
            Assert.AreEqual(expAirCo2, actAirCo2, string.Format("Total AirCO2 = '{0}' actual amount = '{1}'", expAirCo2, actAirCo2));
            Assert.AreEqual(expAltRailCo2, actAltRailCo2, string.Format("Total AltRailCO2 = '{0}' actual amount = '{1}'", expAltRailCo2, actAltRailCo2));
            Assert.AreEqual(expAltCarCo2, actAltCarCo2, string.Format("Total AirCO2 = '{0}' actual amount = '{1}'", expAltCarCo2, actAltCarCo2));
            Assert.AreEqual(expTotalFare, actTotalFare.ToString("#0.00"), string.Format("Total Fare = '{0}' actual amount = '{1}'", expTotalFare, actTotalFare));
            
            Assert.AreEqual("7.27", actFirstPct.ToString("#0.00"), string.Format("First record percent = '7.27' actual amount = '{0}'", actFirstPct.ToString("#0.00")));
        }

        [TestMethod]
        public void AirActivityBackOfficeDepartDateSortBySegment()
        {
            GenerateHandoffRecords(DateType.DepartureDate, "DT:2016,3,1", "DT:2016,3,5", "2600");
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBINCLBREAKBYDATE", ParmValue = "ON", ParmInOut = "IN", LangCode = "" });

            InsertReportHandoff();

            //Check the results
            var rpt = (CustAct2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            var expectTotalRecords = 19;
            int actualTotalRecords = rpt.RawDataList.Count();
            Assert.AreEqual(expectTotalRecords, actualTotalRecords, "Total record count failed");

            var distinctTicket = rpt.RawDataList.Select(s => new { invoice = s.Invoice, break1 = s.Break1, break2 = s.Break2, break3 = s.Break3, airchg = s.Airchg, ticket = s.Ticket }).Distinct();
            var actTotalFare = distinctTicket.Sum(s => s.airchg);
            var actNetNoTickets = distinctTicket.Count();

            //test on break1:CHOC-NA, break2:Sales, breaks:Demo
            var expSalesDemoTotFare = 297.23m;
            var actSalesDemoTotFare = distinctTicket.Where(s => s.break1.Trim() == "CHOC-NA" && s.break2.Trim() == "SALES" && s.break3.Trim() == "121 DEMO")
                                                     .Sum(s => s.airchg);
            var expSalesPercent = 11.24m;
            var actSalesPercent = (actTotalFare != 0) ? actSalesDemoTotFare / actTotalFare * 100 : 0m;
            Assert.AreEqual(expSalesDemoTotFare.ToString("#0.00"), actSalesDemoTotFare.ToString("#0.00"), "CHOC-NA, Sales and Demo total fare failed.");
            Assert.AreEqual(expSalesPercent.ToString("#0.00"), actSalesPercent.ToString("#0.00"), "Expected Sales fare percentage failed.");

            //test on break1:CHOC-NA
            var expCHOCNATotFare = 489.55m;
            var actCHOCNATotFare = distinctTicket.Where(s => s.break1.Trim() == "CHOC-NA")
                                                     .Sum(s => s.airchg);
            Assert.AreEqual(expCHOCNATotFare.ToString("#0.00"), actCHOCNATotFare.ToString("#0.00"), "Expected CHOC-NA total fare failed.");

            var expCHOCNAPercent = 18.52m;
            var actCHOCNAPercent = (actTotalFare != 0) ? actCHOCNATotFare / actTotalFare * 100 : 0m;
            Assert.AreEqual(expCHOCNAPercent.ToString("#0.00"), actCHOCNAPercent.ToString("#0.00"), "Expected CHOC-NA fare percentage failed.");

        }

        [TestMethod]
        public void AirActivityBackOfficeInvoiceDate()
        {
            //c85-A6A6C993-F818-7F57-63CDF1A880686B29_69_61450.keystonecf1
            GenerateHandoffRecords(DateType.InvoiceDate, "DT:2016,2,19", "DT:2016,2,20", "2600");

            InsertReportHandoff();

            //Check the results
            var rpt = (CustAct2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);
            var expectTotalRecords = 19;
            int actualTotalRecords = rpt.RawDataList.Count();
            Assert.AreEqual(expectTotalRecords, actualTotalRecords, "Total record count failed");

            var firstRec = rpt.RawDataList[0];
            var lastRec = rpt.RawDataList[expectTotalRecords - 1];
            Assert.AreEqual("33501157", firstRec.Invoice.Trim(), "First record invoice failed");
            Assert.AreEqual("1612634695", firstRec.Ticket.Trim(), "First record ticket failed");
            Assert.AreEqual("441.62", firstRec.Airchg.ToString("#0.00"), "First record fare failed");

            Assert.AreEqual("33501152", lastRec.Invoice.Trim(), "Last record failed");
            Assert.AreEqual("1612634687", lastRec.Ticket.Trim(), "Last record ticket failed");
            Assert.AreEqual("411.70", lastRec.Airchg.ToString("#0.00"), "Last record fare failed");

            var expTotalFare = 4377.99m;
            var expNetNoTickets = 10;
            var expAvgTicketPrice = 437.80m;

            var distinctTicket = rpt.RawDataList.Select(s => new { invoice = s.Invoice, airchg = s.Airchg, ticket = s.Ticket }).Distinct();
            var actTotalFare = distinctTicket.Sum(s => s.airchg);
            var actNetNoTickets = distinctTicket.Count();
            var actAvgTicketPrice = (actNetNoTickets != 0) ? actTotalFare / actNetNoTickets : 0m;
            Assert.AreEqual(expNetNoTickets, actNetNoTickets, "Expected net # tickets failed");
            Assert.AreEqual(expTotalFare.ToString("#0.00"), actTotalFare.ToString("#0.00"), "Expected total fare failed");
            Assert.AreEqual(expAvgTicketPrice.ToString("#0.00"), actAvgTicketPrice.ToString("#0.00"), "Expected avg ticket price failed");
  
        }

        [TestMethod]

        public void AirActivityBackOfficeOnTheRoadDomesticOnly()
        {
            //7c0-CF53B9A9-DB35-6DE9-D41E6D420318B338_70_43288.keystonecf1
            //Set domestic only, value="2"
            GenerateHandoffRecords(DateType.OnTheRoadDatesSpecial, "DT:2016,2,19", "DT:2016,2,19", "2600");

            InsertReportHandoff();

            //Check the results
            var rpt = (CustAct2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);
            var expectTotalRecords = 16;
            int actualTotalRecords = rpt.RawDataList.Count();
            Assert.AreEqual(expectTotalRecords, actualTotalRecords, "Total record count failed");

            var firstRec = rpt.RawDataList[0];
            var lastRec = rpt.RawDataList[expectTotalRecords - 1];
            Assert.AreEqual("260002277", firstRec.Invoice.Trim(), "First record invoice failed");
            Assert.AreEqual("1612315245", firstRec.Ticket.Trim(), "First record ticket failed");
            Assert.AreEqual("602.61", firstRec.Airchg.ToString("#0.00"), "First record fare failed");

            Assert.AreEqual("260002383", lastRec.Invoice.Trim(), "Last record invoice failed");
            Assert.AreEqual("1612315352", lastRec.Ticket.Trim(), "Last record ticket failed");
            Assert.AreEqual("462.05", lastRec.Airchg.ToString("#0.00"), "Last record fare failed");

            var expTotalFare = 3485.23m;
            var expNetNoTickets = 8;
            var expAvgTicketPrice = 435.65m;

            var distinctTicket = rpt.RawDataList.Select(s => new { invoice = s.Invoice, airchg = s.Airchg, ticket = s.Ticket }).Distinct();
            var actTotalFare = distinctTicket.Sum(s => s.airchg);
            var actNetNoTickets = distinctTicket.Count();
            var actAvgTicketPrice = (actNetNoTickets != 0) ? actTotalFare / actNetNoTickets : 0m;
            Assert.AreEqual(expNetNoTickets, actNetNoTickets, "Expected net # tickets failed");
            Assert.AreEqual(expTotalFare.ToString("#0.00"), actTotalFare.ToString("#0.00"), "Expected total fare failed");
            Assert.AreEqual(expAvgTicketPrice.ToString("#0.00"), actAvgTicketPrice.ToString("#0.00"), "Expected avg ticket price failed");

        }
        [TestMethod]
        public void AirActivityBackOfficeInvoiceDateCurrencyBreakFINANCE()
        {
            //3b4-CA3B835B-926E-ABAE-6E4AD794840150FF_70_34741.keystonecf1
            GenerateHandoffRecords(DateType.InvoiceDate, "DT:2016,1,19", "DT:2016,1,19", "2600");
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "GBP", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "FINANCE", ParmInOut = "IN", LangCode = "" });

            InsertReportHandoff();

            //Check the results
            var rpt = (CustAct2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);
            var expectTotalRecords = 8;
            int actualTotalRecords = rpt.RawDataList.Count();
            Assert.AreEqual(expectTotalRecords, actualTotalRecords, "Total record count failed");

            var firstRec = rpt.RawDataList[0];
            var lastRec = rpt.RawDataList[expectTotalRecords - 1];
            Assert.AreEqual("260002389", firstRec.Invoice.Trim(), "First record invoice failed");
            Assert.AreEqual("1612315362", firstRec.Ticket.Trim(), "First record ticket failed");
            Assert.AreEqual("506.44", firstRec.Airchg.ToString("#0.00"), "First record fare failed");

            Assert.AreEqual("260002403", lastRec.Invoice.Trim(), "Last record invoice failed");
            Assert.AreEqual("1612315380", lastRec.Ticket.Trim(), "Last record ticket failed");
            Assert.AreEqual("423.26", lastRec.Airchg.ToString("#0.00"), "Last record fare failed");

            var expTotalFare = 1655.90m;
            var expNetNoTickets = 5;
            var expAvgTicketPrice = 331.18m;

            var distinctTicket = rpt.RawDataList.Select(s => new { invoice = s.Invoice, airchg = s.Airchg, ticket = s.Ticket }).Distinct();
            var actTotalFare = distinctTicket.Sum(s => s.airchg);
            var actNetNoTickets = distinctTicket.Count();
            var actAvgTicketPrice = (actNetNoTickets != 0) ? actTotalFare / actNetNoTickets : 0m;
            Assert.AreEqual(expNetNoTickets, actNetNoTickets, "Expected net # tickets failed");
            Assert.AreEqual(expTotalFare.ToString("#0.00"), actTotalFare.ToString("#0.00"), "Expected total fare failed");
            Assert.AreEqual(expAvgTicketPrice.ToString("#0.00"), actAvgTicketPrice.ToString("#0.00"), "Expected avg ticket price failed");

        }


        [TestMethod]
        public void AirActivityBackOfficeInvoiceDateNoData()
        {
            GenerateHandoffRecords(DateType.InvoiceDate, "DT:2016,3,1", "DT:2016,3,5", "2600");

            InsertReportHandoff();

            //Check the results
            var rpt = (CustAct2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_NoData);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void AirActivityBackOfficeInvoiceDateOffline()
        {
            GenerateHandoffRecords(DateType.InvoiceDate, "DT:2014,3,1", "DT:2016,3,5", "2600");

            InsertReportHandoff();

            //run the report
            var rpt = (CustAct2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsOfflineMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
            Assert.AreEqual(true, containsOfflineMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void AirActivityReservationDepartDate()
        {
            GenerateHandoffRecords(DateType.DepartureDate, "DT:2015,12,1", "DT:2016,3,10", "", false);

            InsertReportHandoff();

            //Check the results
            var rpt = (CustAct2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            var expectTotalRecords = 16;
            int actualTotalRecords = rpt.RawDataList.Count();
            Assert.AreEqual(expectTotalRecords, actualTotalRecords, "Total record count failed");

            var firstRec = rpt.RawDataList[0];
            var lastRec = rpt.RawDataList[expectTotalRecords - 1];
            Assert.AreEqual("VQ1HTQ", firstRec.Recloc.Trim(), "First record locator failed");
            Assert.AreEqual("981.45", firstRec.Airchg.ToString("#0.00"), "First record fare failed");

            Assert.AreEqual("LF7GRY", lastRec.Recloc.Trim(), "Last record locator failed");
            Assert.AreEqual("0.00", lastRec.Airchg.ToString("#0.00"), "Last record fare failed");

            var expTotalFare = 6988.34m;
            var expNetNoTickets = 11;
            var expAvgTicketPrice = 635.30m;

            var distinctTicket = rpt.RawDataList.Select(s => new { recloc = s.Recloc, break1 = s.Break1, break2 = s.Break2, break3 = s.Break3, airchg = s.Airchg, ticket = s.Ticket }).Distinct();
            var actTotalFare = distinctTicket.Sum(s => s.airchg);
            var actNetNoTickets = distinctTicket.Count();
            var actAvgTicketPrice = (actNetNoTickets != 0) ? actTotalFare / actNetNoTickets : 0m;
            Assert.AreEqual(expNetNoTickets, actNetNoTickets, "Expected net # tickets failed");
            Assert.AreEqual(expTotalFare.ToString("#0.00"), actTotalFare.ToString("#0.00"), "Expected total fare failed");
            Assert.AreEqual(expAvgTicketPrice.ToString("#0.00"), actAvgTicketPrice.ToString("#0.00"), "Expected avg ticket price failed");

            //test on break1:NONE, break2:NONE, breaks:NONE
            var expNoneTotFare = 5899.70m;
            var actNoneTotFare = distinctTicket.Where(s => s.break1.Trim() == "NONE" && s.break2.Trim() == "NONE" && s.break3.Trim() == "NONE")
                                                     .Sum(s => s.airchg);
            var expNonePercent = 84.42m;
            var actNonePercent = (actTotalFare != 0) ? actNoneTotFare / actTotalFare * 100 : 0m;
            Assert.AreEqual(expNoneTotFare.ToString("#0.00"), actNoneTotFare.ToString("#0.00"), "Expected 3 break all to be NONE failed");
            Assert.AreEqual(expNonePercent.ToString("#0.00"), actNonePercent.ToString("#0.00"), "Expected Sales fare percentage failed");

        }

        [TestMethod]
        public void AirActivityReservationInvoiceDate()
        {
            //55f-D6672983-0F63-809A-FB71F6FA8E9BCC9D_70_55158.keystonecf1
            GenerateHandoffRecords(DateType.InvoiceDate, "DT:2015,12,1", "DT:2016,3,10", "", false);

            InsertReportHandoff();

            //Check the results
            var rpt = (CustAct2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            var expectTotalRecords = 6;
            int actualTotalRecords = rpt.RawDataList.Count();
            Assert.AreEqual(expectTotalRecords, actualTotalRecords, "Total record count failed");

            var firstRec = rpt.RawDataList[0];
            var lastRec = rpt.RawDataList[expectTotalRecords - 1];
            Assert.AreEqual("ML2GS8", firstRec.Recloc.Trim(), "First record locator failed");
            Assert.AreEqual("268.20", firstRec.Airchg.ToString("#0.00"), "First record fare failed");

            Assert.AreEqual("RX5TWK", lastRec.Recloc.Trim(), "Last record locator failed");
            Assert.AreEqual("0.00", lastRec.Airchg.ToString("#0.00"), "Last record fare failed");

            var expTotalFare = 735.30m;
            var expNetNoTickets = 3;
            var expAvgTicketPrice = 245.10m;

            var distinctTicket = rpt.RawDataList.Select(s => new { invoice = s.Invoice, break1 = s.Break1, break2 = s.Break2, break3 = s.Break3, airchg = s.Airchg, ticket = s.Ticket }).Distinct();
            var actTotalFare = distinctTicket.Sum(s => s.airchg);
            var actNetNoTickets = distinctTicket.Count();
            var actAvgTicketPrice = (actNetNoTickets != 0) ? actTotalFare / actNetNoTickets : 0m;
            Assert.AreEqual(expNetNoTickets, actNetNoTickets, "Expected net # tickets failed");
            Assert.AreEqual(expTotalFare.ToString("#0.00"), actTotalFare.ToString("#0.00"), "Expected total fare failed");
            Assert.AreEqual(expAvgTicketPrice.ToString("#0.00"), actAvgTicketPrice.ToString("#0.00"), "Expected avg ticket price failed");

            //test on break1:UNKNOWN DIV CODE, break2:FINANCE, breaks:118 DEMO
            var expNoneTotFare = 467.10m;
            var actNoneTotFare = distinctTicket.Where(s => s.break1.Trim() == "UNKNOWN DIV CODE" && s.break2.Trim() == "FINANCE" && s.break3.Trim() == "118 DEMO")
                                                     .Sum(s => s.airchg);
            var expNonePercent = 63.53m;
            var actNonePercent = (actTotalFare != 0) ? actNoneTotFare / actTotalFare * 100 : 0m;
            Assert.AreEqual(expNoneTotFare.ToString("#0.00"), actNoneTotFare.ToString("#0.00"), "Expected 3 breaks total fare failed");
            Assert.AreEqual(expNonePercent.ToString("#0.00"), actNonePercent.ToString("#0.00"), "Expected fare percentage failed");

        }

        [TestMethod]
        public void AirActivityReservationOnTheRoadDate()
        {
            //b8d-D7E99FFA-0B55-C552-A1751CA8A437CBB9_70_57691.keystonecf1
            GenerateHandoffRecords(DateType.OnTheRoadDatesSpecial, "DT:2015,12,1", "DT:2016,3,10", "", false);

            InsertReportHandoff();

            //Check the results
            var rpt = (CustAct2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            var expectTotalRecords = 8;
            int actualTotalRecords = rpt.RawDataList.Count();
            Assert.AreEqual(expectTotalRecords, actualTotalRecords, "Total record count failed");

            var firstRec = rpt.RawDataList[0];
            var lastRec = rpt.RawDataList[expectTotalRecords - 1];
            Assert.AreEqual("ZL081W", firstRec.Recloc.Trim(), "First record locator failed");
            Assert.AreEqual("621.54", firstRec.Airchg.ToString("#0.00"), "First record fare failed");

            Assert.AreEqual("LF7GRY", lastRec.Recloc.Trim(), "Last record locator failed");
            Assert.AreEqual("0.00", lastRec.Airchg.ToString("#0.00"), "Last record fare failed");

            var expTotalFare = 1088.64m;
            var expNetNoTickets = 3;
            var expAvgTicketPrice = 362.88m;

            var distinctTicket = rpt.RawDataList.Select(s => new { invoice = s.Invoice, break1 = s.Break1, break2 = s.Break2, break3 = s.Break3, airchg = s.Airchg, ticket = s.Ticket }).Distinct();
            var actTotalFare = distinctTicket.Sum(s => s.airchg);
            var actNetNoTickets = distinctTicket.Count();
            var actAvgTicketPrice = (actNetNoTickets != 0) ? actTotalFare / actNetNoTickets : 0m;
            Assert.AreEqual(expNetNoTickets, actNetNoTickets, "Expected net # tickets failed");
            Assert.AreEqual(expTotalFare.ToString("#0.00"), actTotalFare.ToString("#0.00"), "Expected total fare failed");
            Assert.AreEqual(expAvgTicketPrice.ToString("#0.00"), actAvgTicketPrice.ToString("#0.00"), "Expected avg ticket price failed");

            //test on break1:UNKNOWN DIV CODE, break2:CORPORATE AFFAIRS, breaks:118 DEMO
            var expNoneTotFare = 621.54m;
            var actNoneTotFare = distinctTicket.Where(s => s.break1.Trim() == "UNKNOWN DIV CODE" && s.break2.Trim() == "CORPORATE AFFAIRS" && s.break3.Trim() == "118 DEMO")
                                                     .Sum(s => s.airchg);
            var expNonePercent = 57.09m;
            var actNonePercent = (actTotalFare != 0) ? actNoneTotFare / actTotalFare * 100 : 0m;
            Assert.AreEqual(expNoneTotFare.ToString("#0.00"), actNoneTotFare.ToString("#0.00"), "Expected 3 breaks total fare failed");
            Assert.AreEqual(expNonePercent.ToString("#0.00"), actNonePercent.ToString("#0.00"), "Expected fare percentage failed");

        }

        [TestMethod]
        public void AirActivityReservationBookDate()
        {
            //96c-D85F168A-DBA6-A9C6-E9505D6CED27302D_70_58461.keystonecf1
            GenerateHandoffRecords(DateType.BookedDate, "DT:2015,1,1", "DT:2015,1,2", "1188", false);

            InsertReportHandoff();

            //Check the results
            var rpt = (CustAct2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            var expectTotalRecords = 60;
            int actualTotalRecords = rpt.RawDataList.Count();
            Assert.AreEqual(expectTotalRecords, actualTotalRecords, "Total record count failed");

            var firstRec = rpt.RawDataList[0];
            var lastRec = rpt.RawDataList[expectTotalRecords - 1];
            Assert.AreEqual("XRC988", firstRec.Recloc.Trim(), "First record locator failed");
            Assert.AreEqual("737.25", firstRec.Airchg.ToString("#0.00"), "First record fare failed");

            Assert.AreEqual("Z1X5CA", lastRec.Recloc.Trim(), "Last record locator failed");
            Assert.AreEqual("477.80", lastRec.Airchg.ToString("#0.00"), "Last record fare failed");

            var expTotalFare = 10254.23m;
            var expNetNoTickets = 22;
            var expAvgTicketPrice = 466.10m;

            var distinctTicket = rpt.RawDataList.Select(s => new { invoice = s.Invoice, break1 = s.Break1, break2 = s.Break2, break3 = s.Break3, airchg = s.Airchg, ticket = s.Ticket }).Distinct();
            var actTotalFare = distinctTicket.Sum(s => s.airchg);
            var actNetNoTickets = distinctTicket.Count();
            var actAvgTicketPrice = (actNetNoTickets != 0) ? actTotalFare / actNetNoTickets : 0m;
            Assert.AreEqual(expNetNoTickets, actNetNoTickets, "Expected net # tickets failed");
            Assert.AreEqual(expTotalFare.ToString("#0.00"), actTotalFare.ToString("#0.00"), "Expected total fare failed");
            Assert.AreEqual(expAvgTicketPrice.ToString("#0.00"), actAvgTicketPrice.ToString("#0.00"), "Expected avg ticket price failed");

            //test on break1:CHOC-NA, break2:SUPPLY, breaks: 239 DEMO
            var expNoneTotFare = 555.74m;
            var actNoneTotFare = distinctTicket.Where(s => s.break1.Trim() == "CHOC-NA" && s.break2.Trim() == "SUPPLY" && s.break3.Trim() == "239 DEMO")
                                                     .Sum(s => s.airchg);
            var expNonePercent = 5.42m;
            var actNonePercent = (actTotalFare != 0) ? actNoneTotFare / actTotalFare * 100 : 0m;
            Assert.AreEqual(expNoneTotFare.ToString("#0.00"), actNoneTotFare.ToString("#0.00"), "Expected 3 breaks total fare failed");
            Assert.AreEqual(expNonePercent.ToString("#0.00"), actNonePercent.ToString("#0.00"), "Expected fare percentage failed");

        }

        private void GenerateHandoffRecords(DateType dateRange, string begDate, string endDate, string inAcct,  bool backOffice = true )
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = begDate, ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBEXCLUDESVCFEES", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBINCLVOIDS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBTRANDATEWITHINRANGE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = Convert.ToInt32(dateRange).ToString(), ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = (dateRange == DateType.OnTheRoadDatesSpecial) ? "2":"1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = endDate, ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTDEST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTORIGIN", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = inAcct, ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INAIRLINE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
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
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MODE", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTLANGUAGE", ParmValue = "EN", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = backOffice? "2":"1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibcustact", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBAPPLYTOLEGORSEG", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3374098", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TXTFLTSEGMENTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1597", ParmInOut = "IN", LangCode = "" });
        }


    }
}
