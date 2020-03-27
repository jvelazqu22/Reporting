using System;
using System.Linq;

using Domain.Helper;
using Domain.Models.ReportPrograms.TripDurationReport;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms;
using iBank.Services.Implementation.ReportPrograms.TripDuration;
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
                On the road Date
           Filters
                Data source: DEMOCA01              
           Currency: USD, GBP           

       Create a report with the desired criteria through iBank. Generate a set of reporthandoff objects using this sql script:
       select 'ReportHandoff.Add(new ReportHandoffInformation{ParmName = "' + rtrim(ltrim(Parmname)) + '",ParmValue = "'+rtrim(ltrim(parmvalue))+'",ParmInOut = "' + rtrim(ltrim(parminout))+'", LangCode = "' + rtrim(ltrim(Langcode)) + '"});' 
       from reporthandoff where reportid = '<REPORT ID GOES HERE>' and parminout = 'IN' order by parmname
   */

    [TestClass]
    public class TripDurationTests : BaseUnitTest
    {
        [TestMethod]
        public void TripDurationNoData()
        {
            //e9-B52E68D0-D9A3-18EC-BA99BFBEAF1290F7_134_57831.keystonecf1 no data
            GenerateHandoffRecords(DateType.DepartureDate, "USD", true);
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,5,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,5,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "2600", ParmInOut = "IN", LangCode = "" });

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (TripDuration)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 2);
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_NoData);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void TripDurationBigData()
        {
            //4a-B4E170FF-C1E5-14B0-C395318AC49ED55B_134_57326.keystonecf1
            GenerateHandoffRecords(DateType.DepartureDate, "USD", true);
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2012,1,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,5,16", ParmInOut = "IN", LangCode = "" });

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (TripDuration)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 2);
            var containsBigDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
            Assert.AreEqual(true, containsBigDataMsg, "Error message failed.");

            ClearReportHandoff();
        }
        [TestMethod]
        public void TripDurationBackOffDepartureDate()
        {
            //a7-1ADBB33F-AD43-0C47-B1D3A14D45919901_131_57906.keystonecf1 total record count 11
            GenerateHandoffRecords(DateType.DepartureDate, "USD", true);
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,4,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,5,10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "NBRDAYSTRIPDUR", ParmValue = "0", ParmInOut = "IN", LangCode = "" });

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (TripDuration)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            //Check that there is the right number of FinalData records. 
            var totalCount = 11;
            Assert.AreEqual(totalCount, rpt.FinalDataList.Count, "Total record count = '11'");

            var firstRec = rpt.FinalDataList.First<FinalData>();
            var lastRec = rpt.FinalDataList.Last<FinalData>();

            Assert.AreEqual(Convert.ToDecimal(199.59), Convert.ToDecimal(firstRec.Airchg));

            var firstInvoice = firstRec.Invoice.Trim();
            Assert.AreEqual("260002474", firstInvoice);

            var lastRecloc = lastRec.Recloc;
            Assert.AreEqual("NMW1SU", lastRecloc.Trim());

            var group2600Nonetotal = rpt.FinalDataList.Where<FinalData>(b => b.Break1.Trim() == "NONE" && b.Break2.Trim() == "NONE" && b.Break3.Trim() == "NONE" && b.Acct.Trim() == "2600")
                                                       .Sum(s => s.Airchg);
            var group9001212Nonetotal = rpt.FinalDataList.Where<FinalData>(b => b.Break1.Trim() == "NONE" && b.Break2.Trim() == "NONE" && b.Break3.Trim() == "NONE" && b.Acct.Trim() == "9001212")
                                                       .Sum(s => s.Airchg);
            var total = rpt.FinalDataList.Sum(s => s.Airchg);

            Assert.AreEqual(Convert.ToDecimal(551.51), Convert.ToDecimal(group2600Nonetotal), "Acct 2600 Break 1/Break2 are NONE Subtotal = '551.51'");
            Assert.AreEqual(Convert.ToDecimal(0), Convert.ToDecimal(group9001212Nonetotal), "Acct 9001212 Break 1/Break2 are NONE Subtotal = '0'");
            Assert.AreEqual(Convert.ToDecimal(3538.40), total, "total = '3,538.40'");

            ClearReportHandoff();
        }
        [TestMethod]
        public void TripDurationBackOffDepartureDateCurrency()
        {
            //a7-1ADBB33F-AD43-0C47-B1D3A14D45919901_131_57906.keystonecf1 total record count 11
            GenerateHandoffRecords(DateType.DepartureDate, "GBP", true);
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,4,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,5,10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "NBRDAYSTRIPDUR", ParmValue = "0", ParmInOut = "IN", LangCode = "" });

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (TripDuration)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            //Check that there is the right number of FinalData records. 
            var totalCount = 11;
            Assert.AreEqual(totalCount, rpt.FinalDataList.Count, "Total record count = '11'");

            var firstRec = rpt.FinalDataList.First<FinalData>();
            var lastRec = rpt.FinalDataList.Last<FinalData>();

            Assert.AreEqual(Convert.ToDecimal(140.93), Convert.ToDecimal(firstRec.Airchg));

            var firstInvoice = firstRec.Invoice.Trim();
            Assert.AreEqual("260002474", firstInvoice);

            var lastRecloc = lastRec.Recloc;
            Assert.AreEqual("NMW1SU", lastRecloc.Trim());

            var group2600Nonetotal = rpt.FinalDataList.Where<FinalData>(b => b.Break1.Trim() == "NONE" && b.Break2.Trim() == "NONE" && b.Break3.Trim() == "NONE" && b.Acct.Trim() == "2600")
                                                       .Sum(s => s.Airchg);
            var group9001212Nonetotal = rpt.FinalDataList.Where<FinalData>(b => b.Break1.Trim() == "NONE" && b.Break2.Trim() == "NONE" && b.Break3.Trim() == "NONE" && b.Acct.Trim() == "9001212")
                                                       .Sum(s => s.Airchg);
            var total = rpt.FinalDataList.Sum(s => s.Airchg);

            Assert.AreEqual(Convert.ToDecimal(389.42), group2600Nonetotal, "Acct 2600 Break 1/Break2 are NONE Subtotal = '389.42'");
            Assert.AreEqual(Convert.ToDecimal(0), group9001212Nonetotal, "Acct 9001212 Break 1/Break2 are NONE Subtotal = '0'");
            Assert.AreEqual(Convert.ToDecimal(2498.46), total, "total = '2,498.46'");

            ClearReportHandoff();
        }

        [TestMethod]
        public void TripDurationBackOfficeDepartureDateOver4Days()
        {
            //2a-B55486C8-99A5-2F3D-84C9E3BB880CB815_134_58081.keystonecf1 total record count 4
            GenerateHandoffRecords(DateType.DepartureDate, "USD", true);
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,4,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,5,10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "NBRDAYSTRIPDUR", ParmValue = "4", ParmInOut = "IN", LangCode = "" });
            InsertReportHandoff();

            //Check the results
            var rpt = (TripDuration)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            //Check that there is the right number of FinalData records. 
            var totalCount = 4;
            Assert.AreEqual(totalCount, rpt.FinalDataList.Count, "Total record count = '4'");

            var firstRec = rpt.FinalDataList.First<FinalData>();
            var lastRec = rpt.FinalDataList.Last<FinalData>();

            var firstRecloc = firstRec.Recloc;
            Assert.AreEqual("JP4NJ2", firstRecloc.Trim());

            var lastRecloc = lastRec.Recloc;
            Assert.AreEqual("NMW1SU", lastRecloc.Trim());

            var groupSecondtotal = rpt.FinalDataList.Where<FinalData>(b => b.Break1.Trim() == "PET-NA" && b.Break2.Trim() == "MARKETING" && b.Break3.Trim() == "121 DEMO" && b.Acct.Trim() == "2600")
                                                       .Sum(s => s.Airchg);

            var total = rpt.FinalDataList.Sum(s => s.Airchg);

            Assert.AreEqual(Convert.ToDecimal(530.96), Convert.ToDecimal(groupSecondtotal), "Acct 2600 break 3 121 DEMO Subtotal = '530.96'");
            Assert.AreEqual(Convert.ToDecimal(1426.78), Convert.ToDecimal(total), "total = '1,426.78'");

            ClearReportHandoff();
        }

        [TestMethod]
        public void TripDuritionBackOfficeOntheRoad()
        {
            //9d-28EE67B7-0295-229B-1E2CADCB772F0414_136_79215.keystonecf1
            GenerateHandoffRecords(DateType.OnTheRoadDatesSpecial, "USD", true);
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,4,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,5,10", ParmInOut = "IN", LangCode = "" });
            InsertReportHandoff();

            //Check the results
            var rpt = (TripDuration)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            //Check that there is the right number of FinalData records. 
            var totalCount = 13;
            Assert.AreEqual(totalCount, rpt.FinalDataList.Count, "Total record count = '13'");

            var firstRec = rpt.FinalDataList.First<FinalData>();
            var lastRec = rpt.FinalDataList.Last<FinalData>();

            var firstRecloc = firstRec.Recloc;
            Assert.AreEqual("RT7SWC", firstRecloc.Trim());

            var lastRecloc = lastRec.Recloc;
            Assert.AreEqual("NMW1SU", lastRecloc.Trim());

            var total = rpt.FinalDataList.Sum(s => s.Airchg);
            Assert.AreEqual(Convert.ToDecimal(6152.03), Convert.ToDecimal(total), "total = '6,152.03'");

            ClearReportHandoff();
        }

        public void TripDuritionBackOfficeInvoiceDate()
        {
            //c1-41839027-C35B-1EA2-23E150CFBCE2A4F5_137_34055.keystonecf1
            GenerateHandoffRecords(DateType.InvoiceDate, "USD", true);
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,1,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,1,5", ParmInOut = "IN", LangCode = "" });
            InsertReportHandoff();

            //Check the results
            var rpt = (TripDuration)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            //Check that there is the right number of FinalData records. 
            var totalCount = 28;
            Assert.AreEqual(totalCount, rpt.FinalDataList.Count, "Total record count = '28'");

            var firstRec = rpt.FinalDataList.First<FinalData>();
            var lastRec = rpt.FinalDataList.Last<FinalData>();

            var firstRecloc = firstRec.Recloc;
            Assert.AreEqual("MR0WJI", firstRecloc.Trim());

            var lastRecloc = lastRec.Recloc;
            Assert.AreEqual("NRB266", lastRecloc.Trim());

            var total = rpt.FinalDataList.Sum(s => s.Airchg);
            Assert.AreEqual(Convert.ToDecimal(10340.32), Convert.ToDecimal(total), "total = '10,340.32'");

            ClearReportHandoff();
        }

        [TestMethod]
        public void TripDurationReservationDepartureDate()
        {
            //59-1B240E83-C0C7-1D91-6A5304B68D54FAB4_131_58380.keystonecf1 total record count 13
            GenerateHandoffRecords(DateType.DepartureDate, "USD");
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,11,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,5,10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "NBRDAYSTRIPDUR", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            InsertReportHandoff();

            //Check the results
            var rpt = (TripDuration)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            //Check that there is the right number of FinalData records. 
            var totalCount = 13;
            Assert.AreEqual(totalCount, rpt.FinalDataList.Count, "Total record count = '13'");

            var firstRec = rpt.FinalDataList.First<FinalData>();
            var lastRec = rpt.FinalDataList.Last<FinalData>();

            var firstRecloc = firstRec.Recloc;
            Assert.AreEqual("W3NM9E", firstRecloc.Trim());

            var lastRecloc = lastRec.Recloc;
            Assert.AreEqual("R461482", lastRecloc.Trim());

            var groupSecondtotal = rpt.FinalDataList.Where<FinalData>(b => b.Break1.Trim() == "NONE" && b.Break2.Trim() == "NONE" && b.Break3.Trim() == "NONE" && b.Acct.Trim() == "30032")
                                                       .Sum(s => s.Airchg);

            var total = rpt.FinalDataList.Sum(s => s.Airchg);

            Assert.AreEqual(Convert.ToDecimal(5899.70), Convert.ToDecimal(groupSecondtotal), "Acct 30032, breaks NONE Subtotal = '5899.70'");
            Assert.AreEqual(Convert.ToDecimal(6988.34), Convert.ToDecimal(total), "total = '6988.34'");

            ClearReportHandoff();
        }

        private void GenerateHandoffRecords(DateType dateRange, string currency = "USD", bool backoffice = false)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = Convert.ToInt32(dateRange).ToString(), ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTDEST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTORIGIN", ParmValue = "", ParmInOut = "IN", LangCode = "" });
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
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MODE", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = currency, ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = !backoffice ? "1" : "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "21", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTripDuration", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBAPPLYTOLEGORSEG", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBTRIPDURATION", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3376611", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1597", ParmInOut = "IN", LangCode = "" });
        }
    }
}
