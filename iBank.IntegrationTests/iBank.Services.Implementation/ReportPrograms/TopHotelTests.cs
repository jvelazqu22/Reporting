using System;
using System.Linq;

using Domain.Helper;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.TopBottomHotels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class TopHotelTests : BaseUnitTest
    {

        [TestMethod]
        public void GenerateReportReservationCheckinDate()
        {
            GenerateReportHandoff(DateType.HotelCheckInDate);
            InsertReportHandoff();

            //run the report
            var rpt = (TopHotels)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count);

            //verify first row 
            var firstRow = rpt.FinalDataList.FirstOrDefault();
            Assert.AreEqual(6,firstRow.Stays);
            Assert.AreEqual(19, firstRow.Nights);
            Assert.AreEqual(232.33m, Math.Round(firstRow.Avgbook,2));
            Assert.AreEqual(4421.00m, Math.Round(firstRow.Hotelcost, 2));

            //verify totals
            Assert.AreEqual(26,rpt.FinalDataList.Sum(s => s.Stays));
            Assert.AreEqual(90, rpt.FinalDataList.Sum(s => s.Nznights));
            Assert.AreEqual(19704.94m, rpt.FinalDataList.Sum(s => s.Hotelcost));
            
            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportReservationInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            InsertReportHandoff();

            //run the report
            var rpt = (TopHotels)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(3, rpt.FinalDataList.Count);

            //verify first row 
            var firstRow = rpt.FinalDataList.FirstOrDefault();
            Assert.AreEqual(1, firstRow.Stays);
            Assert.AreEqual(4, firstRow.Nights);
            Assert.AreEqual(239.00m, Math.Round(firstRow.Avgbook, 2));
            Assert.AreEqual(956.00m, Math.Round(firstRow.Hotelcost, 2));

            //verify totals
            Assert.AreEqual(3, rpt.FinalDataList.Sum(s => s.Stays));
            Assert.AreEqual(9, rpt.FinalDataList.Sum(s => s.Nznights));
            Assert.AreEqual(2181.00m, rpt.FinalDataList.Sum(s => s.Hotelcost));
            
            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportReservationBookedDate()
        {
            GenerateReportHandoff(DateType.BookedDate);
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,8,1", "ENDDATE");
            InsertReportHandoff();

            //run the report
            var rpt = (TopHotels)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count);

            //verify first row 
            var firstRow = rpt.FinalDataList.FirstOrDefault();
            Assert.AreEqual(266, firstRow.Stays);
            Assert.AreEqual(619, firstRow.Nights);
            Assert.AreEqual(112.37m, Math.Round(firstRow.Avgbook, 2));
            Assert.AreEqual(67209.06m, Math.Round(firstRow.Hotelcost, 2));

            //verify totals
            Assert.AreEqual(1105, rpt.FinalDataList.Sum(s => s.Stays));
            Assert.AreEqual(3028, rpt.FinalDataList.Sum(s => s.Nights));
            Assert.AreEqual(403709.33m, rpt.FinalDataList.Sum(s => s.Hotelcost));

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportBackOfficeCheckinDate()
        {
            GenerateReportHandoff(DateType.HotelCheckInDate);
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,8,1", "ENDDATE");
            ManipulateReportHandoffRecords("2", "PREPOST");

            InsertReportHandoff();

            //run the report
            var rpt = (TopHotels)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count);

            //verify first row 
            var firstRow = rpt.FinalDataList.FirstOrDefault();
            Assert.AreEqual(14, firstRow.Stays);
            Assert.AreEqual(37, firstRow.Nights);
            Assert.AreEqual(152.97m, Math.Round(firstRow.Avgbook, 2));
            Assert.AreEqual(5590.10m, Math.Round(firstRow.Hotelcost, 2));

            //verify totals
            Assert.AreEqual(82, rpt.FinalDataList.Sum(s => s.Stays));
            Assert.AreEqual(218, rpt.FinalDataList.Sum(s => s.Nights));
            Assert.AreEqual(35918.76m, rpt.FinalDataList.Sum(s => s.Hotelcost));

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportBackOfficeInvoiceDateLimit8()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,8,1", "ENDDATE");
            ManipulateReportHandoffRecords("2", "PREPOST");
            ManipulateReportHandoffRecords("8", "HOWMANY");
            InsertReportHandoff();

            //run the report
            var rpt = (TopHotels)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(8, rpt.FinalDataList.Count);

            //verify first row 
            var firstRow = rpt.FinalDataList.FirstOrDefault();
            Assert.AreEqual(15, firstRow.Stays);
            Assert.AreEqual(40, firstRow.Nights);
            Assert.AreEqual(152.97m, Math.Round(firstRow.Avgbook, 2));
            Assert.AreEqual(6049.13m, Math.Round(firstRow.Hotelcost, 2));

            //verify totals
            Assert.AreEqual(77, rpt.FinalDataList.Sum(s => s.Stays));
            Assert.AreEqual(218, rpt.FinalDataList.Sum(s => s.Nights));
            Assert.AreEqual(33807.51m, rpt.FinalDataList.Sum(s => s.Hotelcost));

            ClearReportHandoff();
        }

        public void GenerateReportHandoff(DateType dateType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,8,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "7", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,12,19", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "GROUPBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "HOWMANY", ParmValue = "10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "48", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTopHotels", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBSORTDESCASC", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3383083", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");

        }
    }
}
