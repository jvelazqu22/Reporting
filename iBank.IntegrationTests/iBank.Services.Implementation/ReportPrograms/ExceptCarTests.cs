using System;
using System.Linq;

using Domain.Helper;
using Domain.Models.ReportPrograms.ExceptCarReport;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms;
using iBank.Services.Implementation.ReportPrograms.ExceptCar;
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
               
    Standard Parameters:
            Dates: '2015-05-01' - '2015-12-31'
            Accts: 1100, 1188, 1200

    Report Id: 792-D075493C-08B0-E4C3-E153303019EFC5B8_60_50245.keystonecf1 (Reservation Data)
    Note: report id e28-93E8CBCD-98B5-BBE0-2E3E9356146D7737_54_37598.keystonecf1 has issue with currency conversion, tested separately.
            
    Create a report with the desired criteria through iBank. Generate a set of reporthandoff objects using this sql script:
    select 'ReportHandoff.Add(new ReportHandoffInformation{ParmName = "' + rtrim(ltrim(Parmname)) + '",ParmValue = "'+rtrim(ltrim(parmvalue))+'",ParmInOut = "' + rtrim(ltrim(parminout))+'", LangCode = "' + rtrim(ltrim(Langcode)) + '"});' 
    from reporthandoff where reportid = '<REPORT ID GOES HERE>' and parminout = 'IN' order by parmname
    */

    [TestClass]
    public class ExceptCarTests : BaseUnitTest
    {
        [TestMethod]
        public void ExceptCarReservationInvoiceTest()
        {
            GenerateHandoffRecords(DateType.InvoiceDate);

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (ExceptCar)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            var totalExpectRecCnt = 2;
            Assert.AreEqual(totalExpectRecCnt, rpt.FinalDataList.Count);

            var firstRec = rpt.FinalDataList[0];
            Assert.AreEqual(Convert.ToDecimal(96.00), Convert.ToDecimal(firstRec.Aexcprat * firstRec.Days));
            Assert.AreEqual("30825", firstRec.Acct.Trim(), "First account '30825'");

            var lastRec = rpt.FinalDataList[totalExpectRecCnt - 1];
            Assert.AreEqual(Convert.ToDecimal(690.20), Convert.ToDecimal(lastRec.Aexcprat * lastRec.Days));
            Assert.AreEqual("STD CAR 4DR AUTO AC", lastRec.Ctypedesc.Trim(), "Last record car type = 'STD CAR 4DR AUTO AC'");

            var totalOffered = rpt.FinalDataList.Sum(s => Convert.ToDecimal(s.Aexcprat * s.Days));
            Assert.AreEqual(Convert.ToDecimal(786.20), totalOffered, "Total '786.02'");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ExceptCarReservationDepartureDateTest()
        {
            GenerateHandoffRecords(DateType.DepartureDate);

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (ExceptCar)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            var expTotalCnt = 3;
            Assert.AreEqual(expTotalCnt, rpt.FinalDataList.Count);

            var firstRec = rpt.FinalDataList[0];
            Assert.AreEqual("INTER CAR AUTO AC", firstRec.Ctypedesc.Trim(), "First record car type = 'INTER CAR AUTO AC'");
            Assert.AreEqual(Convert.ToDecimal(154.05), Convert.ToDecimal(firstRec.Aexcprat * firstRec.Days), "First record amount paid '154.05'");

            var lastRec = rpt.FinalDataList[expTotalCnt - 1];
            Assert.AreEqual("INTER CAR AUTO AC", lastRec.Ctypedesc.Trim(), "Last record car type = 'INTER CAR AUTO AC'");
            Assert.AreEqual(Convert.ToDecimal(154.05), Convert.ToDecimal(lastRec.Aexcprat * lastRec.Days), "Last record amount paid '154.05'");

            var totalOffered = rpt.FinalDataList.Sum(s => Convert.ToDecimal(s.Aexcprat * s.Days));
            Assert.AreEqual(Convert.ToDecimal(465.15), totalOffered, "Total amount paid = '$465.15'");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ExceptCarReservationBookDateTest()
        {
            GenerateHandoffRecords(DateType.BookedDate);

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (ExceptCar)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            var expTotalCnt = 6;
            Assert.AreEqual(expTotalCnt, rpt.FinalDataList.Count);

            var firstRec = rpt.FinalDataList[0];
            Assert.AreEqual("INTER CAR AUTO AC", firstRec.Ctypedesc.Trim(), "First record car type = 'INTER CAR AUTO AC'");
            Assert.AreEqual(Convert.ToDecimal(36.35), Convert.ToDecimal(firstRec.Aexcprat * firstRec.Days), "First record amount paid '36.35'");

            var lastRec = rpt.FinalDataList[expTotalCnt - 1];
            Assert.AreEqual("FULL SIZE AUTO AC", lastRec.Ctypedesc.Trim(), "Last record car type = 'FULL SIZE AUTO AC'");
            Assert.AreEqual(Convert.ToDecimal(106.60), Convert.ToDecimal(lastRec.Aexcprat * lastRec.Days), "Last record amount paid '106.60'");

            var totalOffered = rpt.FinalDataList.Sum(s => Convert.ToDecimal(s.Aexcprat * s.Days));
            Assert.AreEqual(Convert.ToDecimal(852.05), totalOffered, "Total amount paid = '$852.05'");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ExceptCarBackofficeInvoiceDateTest()
        {
            GenerateHandoffRecords(DateType.InvoiceDate, "USD", true);

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (ExceptCar)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            var expTotalCnt = 12;
            Assert.AreEqual(expTotalCnt, rpt.FinalDataList.Count);

            var firstRec = rpt.FinalDataList[0];
            Assert.AreEqual("INTER CAR AUTO AC", firstRec.Ctypedesc.Trim(), "First record car type = 'INTER CAR AUTO AC'");
            Assert.AreEqual(Convert.ToDecimal(89.73), Convert.ToDecimal(firstRec.Aexcprat * firstRec.Days), "First record amount paid '89.73'");

            var lastRec = rpt.FinalDataList[expTotalCnt - 1];
            Assert.AreEqual("STD CAR 4WD AUTO AC", lastRec.Ctypedesc.Trim(), "Last record car type = 'STD CAR 4WD AUTO AC'");
            Assert.AreEqual(Convert.ToDecimal(0), Convert.ToDecimal(lastRec.Aexcprat * lastRec.Days), "Last record amount paid '0'");

            var totalOffered = rpt.FinalDataList.Sum(s => Convert.ToDecimal(s.Aexcprat * s.Days));
            Assert.AreEqual(Convert.ToDecimal(404.24), totalOffered, "Total amount paid = '$404.24'");

            ClearReportHandoff();
        }
        [TestMethod]
        public void ExceptCarCurrencyBackofficeTest()
        {
            GenerateHandoffRecords(DateType.DepartureDate, "GBP", true);

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (ExceptCar)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            var expCount = 12;
            Assert.AreEqual(expCount, rpt.FinalDataList.Count);

            var firstRec = rpt.FinalDataList.First<FinalData>();
            Assert.AreEqual("2600", firstRec.Acct.Trim(), "First account = '2600'");
            Assert.AreEqual("INTER CAR AUTO AC", firstRec.Ctypedesc.Trim(), "First car type ='INTER CAR AUTO AC'");
            Assert.AreEqual(Convert.ToDecimal(59.28), Convert.ToDecimal(firstRec.Aexcprat * firstRec.Days), "First record amount paid = '59.28'");

            var lastRec = rpt.FinalDataList[expCount - 1];
            Assert.AreEqual("31187", lastRec.Acct.Trim(), "Last account = '31187'");
            Assert.AreEqual("STD CAR 4WD AUTO AC", lastRec.Ctypedesc.Trim(), "Last record car type = 'STD CAR 4WD AUTO AC'");
            Assert.AreEqual(Convert.ToDecimal(0), Convert.ToDecimal(lastRec.Aexcprat * lastRec.Days), "Last record amount paid = '0'");

            var totalOffered = rpt.FinalDataList.Sum(s => Convert.ToDecimal(s.Aexcprat * s.Days));
            Assert.AreEqual(Convert.ToDecimal(267.20), Convert.ToDecimal(totalOffered), "Total amount paid ='￡267.20'");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ExceptCarBackOfficeDepartureTest()
        {
            GenerateHandoffRecords(DateType.DepartureDate, "USD", true);

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (ExceptCar)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            var expTotalCnt = 12;
            Assert.AreEqual(expTotalCnt, rpt.FinalDataList.Count);

            var firstRec = rpt.FinalDataList[0];
            Assert.AreEqual("INTER CAR AUTO AC", firstRec.Ctypedesc.Trim(), "First record car type = 'INTER CAR AUTO AC'");
            Assert.AreEqual(Convert.ToDecimal(89.73), Convert.ToDecimal(firstRec.Aexcprat * firstRec.Days), "First record amount paid '89.73'");

            var lastRec = rpt.FinalDataList[expTotalCnt - 1];
            Assert.AreEqual("STD CAR 4WD AUTO AC", lastRec.Ctypedesc.Trim(), "Last record car type = 'STD CAR 4WD AUTO AC'");
            Assert.AreEqual(Convert.ToDecimal(0), Convert.ToDecimal(lastRec.Aexcprat * lastRec.Days), "Last record amount paid '0'");

            var totalOffered = rpt.FinalDataList.Sum(s => Convert.ToDecimal(s.Aexcprat * s.Days));
            Assert.AreEqual(Convert.ToDecimal(404.24), totalOffered, "Total amount paid = '$404.24'");


            ClearReportHandoff();
        }

        //TODO: Create all ReportHandoff records for the report test. 
        private void GenerateHandoffRecords(DateType dateRange, string currency = "USD", bool backoffice = false)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = (dateRange == DateType.DepartureDate && backoffice) ? "DT:2013,1,1" : (dateRange == DateType.InvoiceDate && backoffice) ? "DT:2015,1,1" : dateRange == DateType.BookedDate ? "DT:2015,1,30" : "DT:2015,5,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = Convert.ToInt32(dateRange).ToString(), ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,12,31", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = dateRange == DateType.DepartureDate && !backoffice ? "1200" : dateRange == DateType.BookedDate ? "1120,1188" : "", ParmInOut = "IN", LangCode = "" });
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
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "102", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibExceptCar", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3373623", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1597", ParmInOut = "IN", LangCode = "" });

        }

    }
}
