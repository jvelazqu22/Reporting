using System;
using System.Linq;

using Domain.Helper;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms;
using iBank.Services.Implementation.ReportPrograms.Arrival;
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
           
            SortBy
                Airline Time(Default)
                Passenger Name

       Standard Parameters:
           Date Range: 2015,1,1 - 2015,1,5           

       Report Id: 1a1-323A7C0E-EA48-F868-9C823A0097E35517_62_41466.keystonecf1

       Create a report with the desired criteria through iBank. Generate a set of reporthandoff objects using this sql script:
       select 'ReportHandoff.Add(new ReportHandoffInformation{ParmName = "' + rtrim(ltrim(Parmname)) + '",ParmValue = "'+rtrim(ltrim(parmvalue))+'",ParmInOut = "' + rtrim(ltrim(parminout))+'", LangCode = "' + rtrim(ltrim(Langcode)) + '"});' 
       from reporthandoff where reportid = '<REPORT ID GOES HERE>' and parminout = 'IN' order by parmname
   */


    [TestClass]
    public class ArrivalTests : BaseUnitTest
    {
        private enum SortBy
        {
            ArrivalTime = 1,
            PassengerName = 2
        }

        [TestMethod]
        public void ArrivalBookedDateResvationSortByArrivalTimeAllLegPageBreakByDate()
        {
            //f48-71185304-0867-4D24-5616BFB8A1495894_68_58002.keystonecf1

            GenerateHandoffRecords(DateType.BookedDate, false, SortBy.ArrivalTime, "DT:2015,1,1", "DT:2015,1,3", "1200");
            //add additional            
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBINCLUDEALLLEGS", ParmValue = "ON", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBINCLUDEPGBRKBYDATE", ParmValue = "ON", ParmInOut = "IN", LangCode = "" });

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (Arrival)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            var expectTotalRecCnt = 27;
            var expectTotalPax = 23;
            int expectPageCnt = 21;
            var actTotalRecCnt = rpt.FinalDataList.Count;
            int actualTotalPax = 0;
            int actualPageCnt = rpt.FinalDataList.GroupBy(s => s.CrysPgBrk).ToList().Count;

            var groups = rpt.FinalDataList.GroupBy(s => s.OrgDesc.Trim()).ToList();
            for (var i = 0; i < groups.Count(); i++)
            {
                actualTotalPax = actualTotalPax + rpt.FinalDataList.Where(s => s.Destdesc.Trim() == groups[i].Key).ToList().GroupBy(s => new { s.PassFrst, s.PassLast }).ToList().Count();
            }
            Assert.AreEqual(expectTotalRecCnt, actTotalRecCnt, string.Format("Total Record count = '{0}' actual count = '{1}'", expectTotalRecCnt, actTotalRecCnt));
            Assert.AreEqual(expectTotalPax, actualTotalPax, string.Format("Total Pax count = '{0}' actual count = '{1}'", expectTotalPax, actualTotalPax));
            Assert.AreEqual(expectPageCnt, actualPageCnt, string.Format("Total page count = '{0}' actual count = '{1}'", expectPageCnt, actualPageCnt));


        }

        [TestMethod]
        public void ArrivalBookedDateReservationSortByArrivalTimeAllLegsTest()
        {
            //f48-71185304-0867-4D24-5616BFB8A1495894_68_58002.keystonecf1

            GenerateHandoffRecords(DateType.BookedDate, false, SortBy.ArrivalTime, "DT:2015,1,1", "DT:2015,1,3", "1200");
            //add additional
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBINCLUDEALLLEGS", ParmValue = "ON", ParmInOut = "IN", LangCode = "" });

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (Arrival)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            var expOhareTotal = 4;
            var actOhareTotal = rpt.FinalDataList.Where(s => s.Destdesc.Trim() == "CHI-OHARE,IL").Count();
            Assert.AreEqual(expOhareTotal, actOhareTotal, string.Format("Total Pax count arrive at 'CHI-OHARE,IL' = '{0}' actual count = '{1}'", expOhareTotal, actOhareTotal));

            var expNashTNTotal = 5;
            var actNashTNTotal = rpt.FinalDataList.Where(s => s.Destdesc.Trim() == "NASHVILLE,TN").Count();
            Assert.AreEqual(expNashTNTotal, actNashTNTotal, string.Format("Total Pax count arrive at 'NASHVILLE,TN' = '{0}' actual count = '{1}'", expNashTNTotal, actNashTNTotal));

            var expectTotalRecCnt = 27;
            var expectTotalPax = 23;
            int actualTotalPax = 0;
            var groups = rpt.FinalDataList.GroupBy(s => s.OrgDesc.Trim()).ToList();
            for (var i = 0; i < groups.Count(); i++)
            {
                actualTotalPax = actualTotalPax + rpt.FinalDataList.Where(s => s.Destdesc.Trim() == groups[i].Key).ToList().GroupBy(s => new { s.PassFrst, s.PassLast }).ToList().Count();
            }
            Assert.AreEqual(expectTotalPax, actualTotalPax, string.Format("Total Pax count = '{0}' actual count = '{1}'", expectTotalPax, actualTotalPax));

            var firstRec = rpt.FinalDataList[0];
            var eightRec = rpt.FinalDataList[7];
            var ninth = rpt.FinalDataList[8];
            var lastRec = rpt.FinalDataList[expectTotalRecCnt - 1];

            Assert.AreEqual("JC7R78", firstRec.Recloc.Trim(), "First record recloc = 'JC7R78'");
            Assert.AreEqual("NEANDERTHAL2", firstRec.PassLast.Trim(), "First record last name = 'NEANDERTHAL2'");

            Assert.AreEqual("ZJN6VE", eightRec.Recloc.Trim(), "Eigth record recloc = 'ZJN6VE'");
            Assert.AreEqual("SQUIRREL", eightRec.PassLast.Trim(), "Eigth record last name = 'SQUIRREL'");

            Assert.AreEqual("ZJN6VE", ninth.Recloc.Trim(), "Ninth record recloc = 'ZJN6VE'");
            Assert.AreEqual("SQUIRREL", ninth.PassLast.Trim(), "Ninth record last name = 'SQUIRREL'");

            Assert.AreEqual("W18HNA", lastRec.Recloc.Trim(), "Last record recloc = 'W18HNA'");
            Assert.AreEqual("OPPOSSUM2", lastRec.PassLast.Trim(), "Last record Passenger Name = 'OPPOSSUM2'");

        }
        [TestMethod]
        public void ArrivalBookedDateReservationSortByArrivalTimeTest()
        {
            GenerateHandoffRecords(DateType.BookedDate, false, SortBy.ArrivalTime, "DT:2015,1,1", "DT:2015,1,3", "1200");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (Arrival)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            var expOhareTotal = 4;
            var actOhareTotal = rpt.FinalDataList.Where(s => s.Destdesc.Trim() == "CHI-OHARE,IL").Count();
            Assert.AreEqual(expOhareTotal, actOhareTotal, string.Format("Total Pax count arrive at 'CHI-OHARE,IL' = '{0}' actual count = '{1}'", expOhareTotal, actOhareTotal));

            var expNashTNTotal = 5;
            var actNashTNTotal = rpt.FinalDataList.Where(s => s.Destdesc.Trim() == "NASHVILLE,TN").Count();
            Assert.AreEqual(expNashTNTotal, actNashTNTotal, string.Format("Total Pax count arrive at 'NASHVILLE,TN' = '{0}' actual count = '{1}'", expNashTNTotal, actNashTNTotal));

            var expectTotalPax = 23;
            int actualTotalPax = rpt.FinalDataList.Count();            
            Assert.AreEqual(expectTotalPax, actualTotalPax, string.Format("Total Pax count = '{0}' actual count = '{1}'", expectTotalPax, actualTotalPax));
            
            var firstRec = rpt.FinalDataList[0];
            var eightRec = rpt.FinalDataList[7];
            var ninth = rpt.FinalDataList[8];
            var lastRec = rpt.FinalDataList[expectTotalPax - 1];

            Assert.AreEqual("JC7R78", firstRec.Recloc.Trim(), "First record recloc = 'JC7R78'");
            Assert.AreEqual("NEANDERTHAL2", firstRec.PassLast.Trim(), "First record last name = 'NEANDERTHAL2'");

            Assert.AreEqual("ZJN6VE", eightRec.Recloc.Trim(), "Eigth record recloc = 'ZJN6VE'");
            Assert.AreEqual("SQUIRREL", eightRec.PassLast.Trim(), "Eigth record last name = 'SQUIRREL'");

        }

        [TestMethod]
        public void ArrivalInvoiceDateReservationSortByPaxNameAllLegsTest()
        {
            GenerateHandoffRecords(DateType.InvoiceDate, false, SortBy.PassengerName,"DT:2015,1,1", "DT:2015,1,3", "1200");
            //add additional
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBUSECONNECTLEGS", ParmValue = "ON", ParmInOut = "IN", LangCode = "" });

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (Arrival)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            //Check that there is the right number of FinalData records. 
            var expectTotalRec = 19;
            Assert.AreEqual(expectTotalRec, rpt.FinalDataList.Count, string.Format("Total record count = '{0}'", expectTotalRec));
        }


        [TestMethod]
        public void ArrivalInvoiceDateReservationSortByPaxNameTest()
        {
            GenerateHandoffRecords(DateType.InvoiceDate, false, SortBy.PassengerName, "DT:2015,1,1", "DT:2015,1,3", "1200");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (Arrival)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            //Check that there is the right number of FinalData records. 
            var expectTotalRec = 15;
            Assert.AreEqual(expectTotalRec, rpt.FinalDataList.Count, string.Format("Total record count = '{0}'", expectTotalRec));

            var firstRec = rpt.FinalDataList[0];
            var fifthRec = rpt.FinalDataList[4];
            var sixRec = rpt.FinalDataList[5];
            var lastRec = rpt.FinalDataList[expectTotalRec - 1];

            Assert.AreEqual("JP2B50", firstRec.Recloc.Trim(), "First record recloc = 'JP2B50'");
            Assert.AreEqual("YETI1", firstRec.PassLast.Trim(), "First record last name = 'YETI1'");

            Assert.AreEqual("XH8C2Y", fifthRec.Recloc.Trim(), "Fifth record recloc = 'XH8C2Y'");
            Assert.AreEqual("OPPOSSUM4", fifthRec.PassLast.Trim(), "Fifth record last name = 'OPPOSSUM4'");

            Assert.AreEqual("ZJN6VE", sixRec.Recloc.Trim(), "Sixth record recloc = 'ZJN6VE'");
            Assert.AreEqual("SQUIRREL", sixRec.PassLast.Trim(), "Sixth record last name = 'SQUIRREL'");

            Assert.AreEqual("XR7CBI", lastRec.Recloc.Trim(), "Last record recloc = 'XR7CBI'");
            Assert.AreEqual("JACKAL1", lastRec.PassLast.Trim(), "Last record Passenger Name = 'JACKAL1'");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ArrivalArrvalDateReservationSortByArrivalTimeTest()
        {
            GenerateHandoffRecords(DateType.RoutingArrivalDate, false, SortBy.ArrivalTime, "DT:2015,1,1", "DT:2015,1,5", "1100,1188,1200");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (Arrival)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            //Check that there is the right number of FinalData records. 
            var expectTotalRec = 5;
            Assert.AreEqual(expectTotalRec, rpt.FinalDataList.Count, string.Format("Total record count = '{0}'", expectTotalRec));

            var firstRec = rpt.FinalDataList[0];
            var secondRec = rpt.FinalDataList[1];
            var lastRec = rpt.FinalDataList[expectTotalRec - 1];

            Assert.AreEqual("R03CJE", firstRec.Recloc.Trim(), "First record recloc = 'R03CJE'");
            Assert.AreEqual("KERMIT1", firstRec.PassLast.Trim(), "First record last name = 'KERMIT1'");

            Assert.AreEqual("S2X4Z8", secondRec.Recloc.Trim(), "Second record recloc = 'S2X4Z8'");
            Assert.AreEqual("VICUNA1", secondRec.PassLast.Trim(), "Second record last name = 'VICUNA1'");

            Assert.AreEqual("JC7R78", lastRec.Recloc.Trim(), "Last record recloc = 'JC7R78'");
            Assert.AreEqual("NEANDERTHAL2", lastRec.PassLast.Trim(), "Last record Passenger Name = 'NEANDERTHAL2'");

            var disExptAirlines = 3;
            var disActAirlines = rpt.FinalDataList.Select(s => s.Airline).Distinct().Count();
            Assert.AreEqual(disExptAirlines, disActAirlines, string.Format("Total number of airlines expected {0}, actually is {1} ", disExptAirlines, disActAirlines));

            //default sorting by arrival time
            var firstExpArrtime = "09:01 am";
            var lastExpArrtime = "07:59 pm";
            var firstActArrtime = firstRec.ArrTime;
            var lastActArrtime = lastRec.ArrTime;
            Assert.AreEqual(firstExpArrtime, firstActArrtime, string.Format("First record expected arrival time {0}, actually is {1} ", firstExpArrtime, firstActArrtime));
            Assert.AreEqual(lastExpArrtime, lastActArrtime, string.Format("Last record expected arrival time {0}, actually is {1} ", lastExpArrtime, lastActArrtime));

            ClearReportHandoff();
        }

        [TestMethod]
        public void ArrivalArrvalDateBackOfficeSoryByPaxNameTest()
        {
            GenerateHandoffRecords(DateType.RoutingArrivalDate, true, SortBy.PassengerName, "DT:2015,1,1", "DT:2015,1,5", "");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (Arrival)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            //Check that there is the right number of FinalData records. 
            var expectTotalRec = 13;
            Assert.AreEqual(expectTotalRec, rpt.FinalDataList.Count, string.Format("Total record count = '{0}'", expectTotalRec));

            var firstRec = rpt.FinalDataList[0];
            var fifthRec = rpt.FinalDataList[4];
            var sixRec = rpt.FinalDataList[5];
            var lastRec = rpt.FinalDataList[expectTotalRec - 1];

            Assert.AreEqual("KQQ9ZW", firstRec.Recloc.Trim(), "First record recloc = 'KQQ9ZW'");

            Assert.AreEqual("ELEPHANT14", fifthRec.PassLast.Trim(), "The fithth record last name = 'ELEPHANT14'");
            Assert.AreEqual("XQ5GQM", fifthRec.Recloc.Trim(), "The fithth record recloc = 'XQ5GQM'");

            Assert.AreEqual("MOOSE13", sixRec.PassLast.Trim(), "The sixth record last name = 'MOOSE13'");
            Assert.AreEqual("TZSZTQ", sixRec.Recloc.Trim(), "The sixth record recloc = 'TZSZTQ'");

            Assert.AreEqual("S64PWU", lastRec.Recloc.Trim(), "Last record recloc = 'S64PWU'");

            var disExptAirlines = 3;
            var disActAirlines = rpt.FinalDataList.Select(s => s.Airline).Distinct().Count();
            Assert.AreEqual(disExptAirlines, disActAirlines, string.Format("Total number of airlines expected {0}, actually is {1} ", disExptAirlines, disActAirlines));

            //default sorting by arrival time
            var firstExpArrtime = "11:58 am";
            var lastExpArrtime = "09:24 pm";
            var firstActArrtime = firstRec.ArrTime;
            var lastActArrtime = lastRec.ArrTime;

            Assert.AreEqual(firstExpArrtime, firstActArrtime, string.Format("First record expected arrival time {0}, actually is {1} ", firstExpArrtime, firstActArrtime));
            Assert.AreEqual(lastExpArrtime, lastActArrtime, string.Format("Last record expected arrival time {0}, actually is {1} ", lastExpArrtime, lastActArrtime));

            ClearReportHandoff();
        }

        [TestMethod]
        public void ArrivalArrvalDateBackOfficeSoryByArriveTimeTest()
        {
            GenerateHandoffRecords(DateType.RoutingArrivalDate, true, SortBy.ArrivalTime, "DT:2015,1,1", "DT:2015,1,5", "");

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (Arrival)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            //Check that there is the right number of FinalData records. 
            var expectTotalRec = 13;
            Assert.AreEqual(expectTotalRec, rpt.FinalDataList.Count, string.Format("Total record count = '{0}'", expectTotalRec));

            var firstRec = rpt.FinalDataList[0];
            var fifthRec = rpt.FinalDataList[4];
            var sixRec = rpt.FinalDataList[5];
            var lastRec = rpt.FinalDataList[expectTotalRec - 1];

            Assert.AreEqual("KQQ9ZW", firstRec.Recloc.Trim(), "First record recloc = 'KQQ9ZW'");

            Assert.AreEqual("ELEPHANT14", fifthRec.PassLast.Trim(), "The fithth record last name = 'ELEPHANT14'");
            Assert.AreEqual("XQ5GQM", fifthRec.Recloc.Trim(), "The fithth record recloc = 'XQ5GQM'");

            Assert.AreEqual("SQUIRREL10", sixRec.PassLast.Trim(), "The sixth record last name = 'SQUIRREL10'");
            Assert.AreEqual("VCP48M", sixRec.Recloc.Trim(), "The sixth record recloc = 'VCP48M'");

            Assert.AreEqual("S64PWU", lastRec.Recloc.Trim(), "Last record recloc = 'S64PWU'");

            var disExptFltno = 9;
            var disActFltno = rpt.FinalDataList.Select(s => s.FltNo).Distinct().Count();
            Assert.AreEqual(disExptFltno, disActFltno, string.Format("Total number of flight # expected {0}, actually is {1} ", disExptFltno, disActFltno));

            //default sorting by arrival time
            var firstExpArrtime = "11:58 am";
            var lastExpArrtime = "09:24 pm";
            var firstActArrtime = firstRec.ArrTime;
            var lastActArrtime = lastRec.ArrTime;

            Assert.AreEqual(firstExpArrtime, firstActArrtime, string.Format("First record expected arrival time {0}, actually is {1} ", firstExpArrtime, firstActArrtime));
            Assert.AreEqual(lastExpArrtime, lastActArrtime, string.Format("Last record expected arrival time {0}, actually is {1} ", lastExpArrtime, lastActArrtime));

            ClearReportHandoff();
        }

        [TestMethod]
        public void ArrivalInvoiceDateBackOfficeSoryByArriveTimeTest()
        {
            GenerateHandoffRecords(DateType.InvoiceDate, true);

            //Inserts the records into the database under the Test User id. 
            InsertReportHandoff();

            //Check the results
            var rpt = (Arrival)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(rptInfo.ReturnCode, 1);
            Assert.AreEqual(rptInfo.ErrorMessage, string.Empty);

            //Check that there is the right number of FinalData records. 
            var expectTotalRec = 20;
            Assert.AreEqual(expectTotalRec, rpt.FinalDataList.Count, string.Format("Total record count = '{0}'", expectTotalRec));

            var firstRec = rpt.FinalDataList[0];
            var fifRec = rpt.FinalDataList[5];
            var sixRec = rpt.FinalDataList[6];
            var lastRec = rpt.FinalDataList[expectTotalRec - 1];

            Assert.AreEqual("JW1D8Y", firstRec.Recloc.Trim(), "First record recloc = 'JW1D8Y'");

            Assert.AreEqual("08:18 am", fifRec.ArrTime.Trim(), "First second record arrival time = '08:18 am'");
            Assert.AreEqual("01:18 pm", sixRec.ArrTime.Trim(), "Second record arrival time = '01:18 pm '");

            Assert.AreEqual("WTCL2A", lastRec.Recloc.Trim(), "Last record recloc = 'WTCL2A'");

            var disExptAirline = 5;
            var disActAirline = rpt.FinalDataList.Select(s => s.Airline).Distinct().Count();
            Assert.AreEqual(disExptAirline, disActAirline, string.Format("Total number of airlines expected {0}, actually is {1} ", disExptAirline, disActAirline));

            ClearReportHandoff();
        }

        [TestMethod]
        public void ArrivalGenerateNoDateReport()
        {
            GenerateReportHandoffRecordsNoData();

            InsertReportHandoff();

            //run the report
            var rpt = (Arrival)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_NoData);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ArrivalGenerateOfflineReport()
        {
            GenerateReportHandoffRecordsOfflineReport();

            InsertReportHandoff();

            //run the report
            var rpt = (Arrival)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsOfflineMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
            Assert.AreEqual(true, containsOfflineMsg, "Error message failed.");

            ClearReportHandoff();
        }

        private void GenerateHandoffRecords(DateType dateRange, bool backoffice = false, SortBy sortBy = SortBy.ArrivalTime, string begDate="DT:2015,1,1", string endDate="DT:2015,1,5", string inAcct="")
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = begDate, ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = Convert.ToInt32(dateRange).ToString(), ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = endDate, ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTDEST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTORIGIN", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = inAcct, ParmInOut = "IN", LangCode = "" });
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
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PICKRECNUM", ParmValue = "12400", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = !backoffice ? "1" : "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "4", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibarrival", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3373830", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = Convert.ToInt32(sortBy).ToString(), ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1597", ParmInOut = "IN", LangCode = "" });
        }

        private void GenerateReportHandoffRecordsNoData()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,1,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "5", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,1,5", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTDEST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTORIGIN", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "1200", ParmInOut = "IN", LangCode = "" });
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
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PICKRECNUM", ParmValue = "12400", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "4", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibarrival", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3373828", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1597", ParmInOut = "IN", LangCode = "" });
        }


        private void GenerateReportHandoffRecordsOfflineReport()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2010,1,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,1,5", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTDEST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTORIGIN", ParmValue = "", ParmInOut = "IN", LangCode = "" });
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
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MODE", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PICKRECNUM", ParmValue = "12505", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "4", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibarrival", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3373970", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1597", ParmInOut = "IN", LangCode = "" });
        }
    }        
}