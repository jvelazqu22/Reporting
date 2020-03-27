using System;
using System.Linq;

using Domain.Helper;
using Domain.Models.ReportPrograms.ExceptHotelReport;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms;
using iBank.Services.Implementation.ReportPrograms.ExceptHotel;
using iBank.Services.Implementation.Shared;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    /*
       Reservation and Back Offce Reports

       Items to test:
           Date Range Type
                Departure Date
                Invoice Date
                Book Date
           Filters
                Data source: DEMOCA01              
           Currency: USD, GBP           

       Standard Parameters:
           Start Date: 2015,12,1 
           End Date: 2015,12,31 or 2015,12,3(By Invoice Date) 

       Report Id: 3a4-C7EF94C5-0394-C3AF-43E67A51090C25C7_60_35948.keystonecf1

       Create a report with the desired criteria through iBank. Generate a set of reporthandoff objects using this sql script:
       select 'ReportHandoff.Add(new ReportHandoffInformation{ParmName = "' + rtrim(ltrim(Parmname)) + '",ParmValue = "'+rtrim(ltrim(parmvalue))+'",ParmInOut = "' + rtrim(ltrim(parminout))+'", LangCode = "' + rtrim(ltrim(Langcode)) + '"});' 
       from reporthandoff where reportid = '<REPORT ID GOES HERE>' and parminout = 'IN' order by parmname
   */

    [TestClass]
    public class ExceptHotelTests : BaseUnitTest
    {

        [TestMethod]
        public void ExceptHotelBookDateReservationTest()
        {
            GenerateHandoffRecords(DateType.BookedDate, "USD");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (ExceptHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            //Check that there is the right number of FinalData records. 
            var totalCount = 13;
            Assert.AreEqual(totalCount, rpt.FinalDataList.Count, "Total record count = '13'");

            var firstRoomType = rpt.FinalDataList[0].Typedesc;
            Assert.AreEqual("DXL SGL", firstRoomType.Trim(), "First record room type = 'DXL SGL'");

            var firstAmt = rpt.FinalDataList.First<FinalData>();
            Assert.AreEqual(Convert.ToDecimal(116.25), Convert.ToDecimal(firstAmt.Bookrate * firstAmt.Nights), "First record amount paid = '$98.15'");

            var totalKermit2ffered = rpt.FinalDataList.Where(s => s.Break2.Trim() == "MARKETING" && s.Break3.Trim() == "239 DEMO")
                                                .Sum(s => s.Bookrate * s.Nights);
            Assert.AreEqual(Convert.ToDecimal(260.25), totalKermit2ffered, "Break 3 239 DEMO Subtotal paid = '$260.25'");

            var totalOffered = rpt.FinalDataList.Sum(s => s.Bookrate * s.Nights);
            Assert.AreEqual(Convert.ToDecimal(4566.77), totalOffered, "Total amount paid = '$4566.77'");

            var lastRoomType = rpt.FinalDataList[totalCount - 1].Typedesc;
            Assert.AreEqual("SUP SGL", lastRoomType.Trim(), "Last record room type = 'SUP SGL '");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ExceptHotelInvoiceReservationTest()
        {
            GenerateHandoffRecords(DateType.InvoiceDate, "GBP");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (ExceptHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            //Check that there is the right number of FinalData records.
            var expectTotalRec = 1;
            Assert.AreEqual(expectTotalRec, rpt.FinalDataList.Count, "Total record count = '1'");

            var firstAmt = rpt.FinalDataList.First<FinalData>();
            Assert.AreEqual(Convert.ToDecimal(357.99), Convert.ToDecimal(firstAmt.Bookrate * firstAmt.Nights), "First record amount paid = '￡357.99'");

            var totalOffered = rpt.FinalDataList.Sum(s => s.Bookrate * s.Nights);
            Assert.AreEqual(Convert.ToDecimal(357.99), totalOffered, "Total amount paid = '￡357.99'");

            var lastRoomType = rpt.FinalDataList[expectTotalRec - 1].Typedesc;
            Assert.AreEqual("SQN", lastRoomType.Trim(), "Last record room type = 'SQN'");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ExceptHotelCurrencyReservationTest()
        {
            GenerateHandoffRecords(DateType.DepartureDate, "GBP");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (ExceptHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            //Check that there is the right number of FinalData records. 
            var expectTotalRec = 5;
            Assert.AreEqual(expectTotalRec, rpt.FinalDataList.Count);

            //first amount
            var firstRec = rpt.FinalDataList.First<FinalData>();
            Assert.AreEqual(Convert.ToDecimal(506.98), Convert.ToDecimal(firstRec.Bookrate * firstRec.Nights), "First record amount paid = '￡506.98'");
            Assert.AreEqual("1200", firstRec.Acct.Trim(), "First account = '1200'");
            Assert.AreEqual("HY0055315655", firstRec.Confirmno.Trim(), "First conf = 'HY0055315655'");

            var lastRec = rpt.FinalDataList[expectTotalRec - 1];
            Assert.AreEqual(Convert.ToDecimal(62.84), Convert.ToDecimal(lastRec.Bookrate * lastRec.Nights), "Last record amount paid ='￡62.84'");
            Assert.AreEqual("ZFW", lastRec.Typedesc.Trim(), "Last record room type ='ZFW'");

            var totalOffered = rpt.FinalDataList.Sum(s => s.Bookrate * s.Nights);
            Assert.AreEqual(Convert.ToDecimal(1665.69), totalOffered, "Total amount paid = '￡1665.69'");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ExceptHotelDepartureReservationTest()
        {
            GenerateHandoffRecords(DateType.DepartureDate, "USD");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (ExceptHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            //Check that there is the right number of FinalData records. 
            var expectTotalRec = 5;
            Assert.AreEqual(expectTotalRec, rpt.FinalDataList.Count);

            //first amount
            var firstAmt = rpt.FinalDataList.First<FinalData>();
            Assert.AreEqual(Convert.ToDecimal(718.00), Convert.ToDecimal(firstAmt.Bookrate * firstAmt.Nights), "First record amount paid = '718.00'");

            var firstAccount = rpt.FinalDataList[0].Acct;
            Assert.AreEqual("1200", firstAccount.Trim(), "First account = '1200'");

            var lastRoomType = rpt.FinalDataList[expectTotalRec - 1].Typedesc;
            Assert.AreEqual("ZFW", lastRoomType.Trim(), "Last record room type ='DXL SGL'");


            var totalOffered = rpt.FinalDataList.Sum(s => s.Bookrate * s.Nights);
            Assert.AreEqual(Convert.ToDecimal(2359.00), totalOffered, "Total amount paid = '2359.00'");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ExceptHotelInvoiceBackOfficeTest()
        {
            GenerateHandoffRecords(DateType.InvoiceDate, "USD", true);

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (ExceptHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            //Check that there is the right number of FinalData records. 
            var expectTotalRec = 5;
            Assert.AreEqual(expectTotalRec, rpt.FinalDataList.Count);

            var group1total = rpt.FinalDataList.Where<FinalData>(b => b.Break1.Trim() == "NONE" && b.Break2.Trim() == "NONE")
                                                       .Sum(s => s.Bookrate * s.Nights);
            var group2total = rpt.FinalDataList.Where<FinalData>(b => b.Break1.Trim() == "UNKNOWN CHRIS ID" && b.Break2.Trim() == "UNKNOWN CHRIS ID")
                                                       .Sum(s => s.Bookrate * s.Nights);
            var group3total = rpt.FinalDataList.Where<FinalData>(b => b.Break1.Trim() == "UNKNOWN DIV CODE" && b.Break2.Trim() == "COMMERCIAL")
                                                       .Sum(s => s.Bookrate * s.Nights);

            Assert.AreEqual(Convert.ToDecimal(78.50), group1total, "Break 1 NONE Subtotal = '78.50'");
            Assert.AreEqual(Convert.ToDecimal(0), group2total, "Break 1 UNKNOWN CHRIS ID Subtotal = '0'");
            Assert.AreEqual(Convert.ToDecimal(768.44), group3total, "Break 1 UNKNOWN DIV CODE Subtotal '768.44'");

            var totalOffered = rpt.FinalDataList.Sum(s => s.Bookrate * s.Nights);
            Assert.AreEqual(Convert.ToDecimal(846.94), totalOffered, "Total doesn't match");
            Assert.AreEqual(totalOffered, Convert.ToDecimal(group1total + group2total + group3total), "All groups total doesn't match");


            ClearReportHandoff();
        }

        [TestMethod]
        public void ExceptHotelDepartureBackofficeTest()
        {
            GenerateHandoffRecords(DateType.DepartureDate, "USD", true);

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (ExceptHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            //Check that there is the right number of FinalData records. 
            var expectTotalRec = 3;
            Assert.AreEqual(expectTotalRec, rpt.FinalDataList.Count);

            //first amount
            var firstAmt = rpt.FinalDataList.First<FinalData>();
            Assert.AreEqual(Convert.ToDecimal(77.12), Convert.ToDecimal(firstAmt.Bookrate * firstAmt.Nights), "First record amount paid = '77.12'");

            var firstConf = rpt.FinalDataList[0].Confirmno;
            Assert.AreEqual("263137694", firstConf.Trim(), "First record confirmation = '263137694'");

            var lastRoomType = rpt.FinalDataList[expectTotalRec - 1].Typedesc;
            Assert.AreEqual("SUP SGL", lastRoomType.Trim(), "Last record room type = 'DXL SGL'");

            var totalOffered = rpt.FinalDataList.Sum(s => s.Bookrate * s.Nights);
            Assert.AreEqual(Convert.ToDecimal(77.12), totalOffered, "Total amount paid = '77.12'");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ExceptHotelInvoiceCurrencyBackOfficeTest()
        {
            GenerateHandoffRecords(DateType.InvoiceDate, "GBP", true);

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (ExceptHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            //Check that there is the right number of FinalData records. 
            var expectTotalRec = 5;
            Assert.AreEqual(expectTotalRec, rpt.FinalDataList.Count);

            //first amount
            var firstAmt = rpt.FinalDataList.First<FinalData>();
            Assert.AreEqual(Convert.ToDecimal(55.43), Convert.ToDecimal(firstAmt.Bookrate * firstAmt.Nights));

            var firstConf = rpt.FinalDataList[0].Confirmno;
            Assert.AreEqual("339190142", firstConf.Trim());

            var lastRoolType = rpt.FinalDataList[expectTotalRec - 1].Typedesc;
            Assert.AreEqual("CORPORAT", lastRoolType.Trim());

            var group1total = rpt.FinalDataList.Where<FinalData>(b => b.Break1.Trim() == "NONE" && b.Break2.Trim() == "NONE")
                                                       .Sum(s => s.Bookrate * s.Nights);
            var group2total = rpt.FinalDataList.Where<FinalData>(b => b.Break1.Trim() == "UNKNOWN CHRIS ID" && b.Break2.Trim() == "UNKNOWN CHRIS ID")
                                                       .Sum(s => s.Bookrate * s.Nights);
            var group3total = rpt.FinalDataList.Where<FinalData>(b => b.Break1.Trim() == "UNKNOWN DIV CODE" && b.Break2.Trim() == "COMMERCIAL")
                                                       .Sum(s => s.Bookrate * s.Nights);

            Assert.AreEqual(Convert.ToDecimal(55.43), group1total, "Break 1 NONE Subtotal = '55.43'");
            Assert.AreEqual(Convert.ToDecimal(0), group2total, "Break 1 UNKNOWN CHRIS ID Subtotal = '0'");
            Assert.AreEqual(Convert.ToDecimal(542.58), group3total, "Break 1 UNKNOWN DIV CODE Subtotal '542.58'");

            var totalOffered = rpt.FinalDataList.Sum(s => s.Bookrate * s.Nights);
            Assert.AreEqual(Convert.ToDecimal(598.01), totalOffered, "Total doesn't match '598.01'");
            Assert.AreEqual(totalOffered, Convert.ToDecimal(group1total + group2total + group3total), "All groups total doesn't match");


            ClearReportHandoff();
        }

        [TestMethod]
        public void ExceptHotelInvoiceOnlyBackOfficeTest()
        {
            GenerateHandoffRecords(DateType.InvoiceDate, "USD", true);

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (ExceptHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            //Check that there is the right number of FinalData records. 
            var expectTotalRec = 5;
            Assert.AreEqual(expectTotalRec, rpt.FinalDataList.Count, "Total record count = '5'");

            //first amount
            var firstAmt = rpt.FinalDataList.First<FinalData>();
            Assert.AreEqual(Convert.ToDecimal(78.50), Convert.ToDecimal(firstAmt.Bookrate * firstAmt.Nights), "First record amount paid ='78.5'");

            var firstConf = rpt.FinalDataList[0].Confirmno;
            Assert.AreEqual("339190142", firstConf.Trim(), "First record confirmation = '339190142'");

            var lastRoomType = rpt.FinalDataList[expectTotalRec - 1].Typedesc;
            Assert.AreEqual("CORPORAT", lastRoomType.Trim(), "Last record room type = 'CORPORAT'");

            ClearReportHandoff();
        }

        private void GenerateHandoffRecords(DateType dateRange, string currency = "USD", bool backoffice = false)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = dateRange == DateType.BookedDate ? "DT:2015,1,30" : "DT:2015,12,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = Convert.ToInt32(dateRange).ToString(), ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = dateRange == DateType.InvoiceDate && backoffice ? "DT:2015,12,3" : "DT:2015,12,31", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = dateRange == DateType.DepartureDate && backoffice ? "9000933,32006" : dateRange == DateType.BookedDate ? "1100,1188" : "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = currency, ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = !backoffice ? "1" : "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "104", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibExceptHotel", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3373568", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1597", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
        }
    }
}
