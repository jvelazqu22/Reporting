using System;
using System.Linq;
using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.TopBottomCars;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class TopCarTests : BaseUnitTest
    {

        [TestMethod]
        public void GenerateReportBackOfficeRentalDate()
        {
            GenerateReportHandoff(DateType.CarRentalDate);

            InsertReportHandoff();

            //run the report
            var rpt = (TopCars)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(7, rpt.FinalDataList.Count);

            //verify first row 
            var firstRow = rpt.FinalDataList.FirstOrDefault();
            Assert.AreEqual(113, firstRow.Rentals);
            Assert.AreEqual(333, firstRow.Days);
            Assert.AreEqual(44.38m, Math.Round(firstRow.Avgbook, 2));
            Assert.AreEqual(9851.00m, Math.Round(firstRow.Carcost, 2));

            //verify totals
            Assert.AreEqual(156, rpt.FinalDataList.Sum(s => s.Rentals));
            Assert.AreEqual(449, rpt.FinalDataList.Sum(s => s.Days));
            Assert.AreEqual(12777.20m, rpt.FinalDataList.Sum(s => s.Carcost));

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportBackOfficeInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);

            InsertReportHandoff();

            //run the report
            var rpt = (TopCars)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(5, rpt.FinalDataList.Count);

            //verify first row 
            var firstRow = rpt.FinalDataList.FirstOrDefault();
            Assert.AreEqual(81, firstRow.Rentals);
            Assert.AreEqual(232, firstRow.Days);
            Assert.AreEqual(44.79m, Math.Round(firstRow.Avgbook, 2));
            Assert.AreEqual(6777.67m, Math.Round(firstRow.Carcost, 2));

            //verify totals
            Assert.AreEqual(117, rpt.FinalDataList.Sum(s => s.Rentals));
            Assert.AreEqual(324, rpt.FinalDataList.Sum(s => s.Days));
            Assert.AreEqual(9358.06m, rpt.FinalDataList.Sum(s => s.Carcost));

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportBackOfficeRentalDateComparison()
        {
            GenerateReportHandoff(DateType.CarRentalDate);
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,9,12", "ENDDATE");
            ManipulateReportHandoffRecords("DT:2016,1,1", "BEGDATE2");
            ManipulateReportHandoffRecords("DT:2016,9,12", "ENDDATE2");
            InsertReportHandoff();

            //run the report
            var rpt = (TopCars)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(8, rpt.FinalDataList.Count);

            //verify first row 
            var firstRow = rpt.FinalDataList.FirstOrDefault();
            Assert.AreEqual(329, firstRow.Rentals);
            Assert.AreEqual(792, firstRow.Days);
            Assert.AreEqual(21839.70m, Math.Round(firstRow.Carcost, 2));

            Assert.AreEqual(113, firstRow.Rentals2);
            Assert.AreEqual(333, firstRow.Days2);
            Assert.AreEqual(9851.00m, Math.Round(firstRow.Carcost2, 2));

            //verify totals
            Assert.AreEqual(409, rpt.FinalDataList.Sum(s => s.Rentals));
            Assert.AreEqual(1032, rpt.FinalDataList.Sum(s => s.Days));
            Assert.AreEqual(27384.50m, rpt.FinalDataList.Sum(s => s.Carcost));

            Assert.AreEqual(156, rpt.FinalDataList.Sum(s => s.Rentals2));
            Assert.AreEqual(449, rpt.FinalDataList.Sum(s => s.Days2));
            Assert.AreEqual(12777.20m, rpt.FinalDataList.Sum(s => s.Carcost2));

            Assert.AreEqual(409, rpt.TotCnt);
            Assert.AreEqual(1032, rpt.TotDays);
            Assert.AreEqual(27384.50m, rpt.TotCost);

            Assert.AreEqual(156, rpt.TotCnt2);
            Assert.AreEqual(449, rpt.TotDays2);
            Assert.AreEqual(12777.20m, rpt.TotCost2);

            ClearReportHandoff();
        }
        [TestMethod]
        public void GenerateReportReservationRentalDate()
        {
            GenerateReportHandoff(DateType.CarRentalDate);
            ManipulateReportHandoffRecords("1","PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,9,12", "ENDDATE");
            InsertReportHandoff();

            //run the report
            var rpt = (TopCars)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count);

            //verify first row 
            var firstRow = rpt.FinalDataList.FirstOrDefault();
            Assert.AreEqual(2348, firstRow.Rentals);
            Assert.AreEqual(6965, firstRow.Days);
            Assert.AreEqual(56.16m, Math.Round(firstRow.Avgbook, 2));
            Assert.AreEqual(775151.90m, Math.Round(firstRow.Carcost, 2));

            //verify totals
            Assert.AreEqual(6130, rpt.FinalDataList.Sum(s => s.Rentals));
            Assert.AreEqual(18885, rpt.FinalDataList.Sum(s => s.Days));
            Assert.AreEqual(1784454.49m, rpt.FinalDataList.Sum(s => s.Carcost));

            Assert.AreEqual(6140,rpt.TotCnt);
            Assert.AreEqual(18923, rpt.TotDays);
            Assert.AreEqual(1785888.19m, rpt.TotCost);

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportReservationInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("12", "HOWMANY");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,9,12", "ENDDATE");
            InsertReportHandoff();

            //run the report
            var rpt = (TopCars)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(12, rpt.FinalDataList.Count);

            //verify first row 
            var firstRow = rpt.FinalDataList.FirstOrDefault();
            Assert.AreEqual(2217, firstRow.Rentals);
            Assert.AreEqual(6609, firstRow.Days);
            Assert.AreEqual(56.43m, Math.Round(firstRow.Avgbook, 2));
            Assert.AreEqual(760355.88m, Math.Round(firstRow.Carcost, 2));

            //verify totals
            Assert.AreEqual(5870, rpt.FinalDataList.Sum(s => s.Rentals));
            Assert.AreEqual(17989, rpt.FinalDataList.Sum(s => s.Days));
            Assert.AreEqual(1752629.27m, rpt.FinalDataList.Sum(s => s.Carcost));

            Assert.AreEqual(5875, rpt.TotCnt);
            Assert.AreEqual(18004, rpt.TotDays);
            Assert.AreEqual(1753081.95m, rpt.TotCost);

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportReservationBookedDate()
        {
            GenerateReportHandoff(DateType.BookedDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,9,12", "ENDDATE");      
            ManipulateReportHandoffRecords("2", "RBSORTDESCASC");
            InsertReportHandoff();

            //run the report
            var rpt = (TopCars)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count);

            //verify first row 
            var firstRow = rpt.FinalDataList.FirstOrDefault();
            Assert.AreEqual(1, firstRow.Rentals);
            Assert.AreEqual(2, firstRow.Days);
            Assert.AreEqual(25.55m, Math.Round(firstRow.Avgbook, 2));
            Assert.AreEqual(51.10m, Math.Round(firstRow.Carcost, 2));

            //verify totals
            Assert.AreEqual(161, rpt.FinalDataList.Sum(s => s.Rentals));
            Assert.AreEqual(479, rpt.FinalDataList.Sum(s => s.Days));
            Assert.AreEqual(17117.82m, rpt.FinalDataList.Sum(s => s.Carcost));

            Assert.AreEqual(6146, rpt.TotCnt);
            Assert.AreEqual(18948, rpt.TotDays);
            Assert.AreEqual(1786863.08m, rpt.TotCost);

            ClearReportHandoff();
        }

        public void GenerateReportHandoff(DateType dateType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,1,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "6", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,9,12", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "GROUPBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "HOWMANY", ParmValue = "10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "46", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTopCars", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBSORTDESCASC", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3392716", ParmInOut = "IN", LangCode = "" });
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
