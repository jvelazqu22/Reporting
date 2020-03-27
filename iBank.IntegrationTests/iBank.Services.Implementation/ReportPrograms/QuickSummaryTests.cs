using System.Linq;

using Domain.Helper;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.QuickSummary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class QuickSummaryTests : BaseUnitTest
    {

        #region standard report
        [TestMethod]
        public void GenerateReportTooMuchData()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,6,23", "ENDDATE");
            ManipulateReportHandoffRecords(string.Empty, "INSOURCEABBR");
            InsertReportHandoff();

            //run the report
            var rpt = (Qview2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportBackOfficeDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            InsertReportHandoff();

            //run the report
            var rpt = (Qview2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 1); //trips
            Assert.AreEqual("117", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 2); //Air charges
            Assert.AreEqual("$77,981.29", testRow.Tots);
            Assert.AreEqual("$666.51", testRow.Avgs);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 3); //Savings
            Assert.AreEqual("$36,027.32", testRow.Tots);
            Assert.AreEqual("$307.93", testRow.Avgs);
            Assert.AreEqual("31.60 %", testRow.Svgs);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 4); //trips
            Assert.AreEqual("11", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 5); //Lost Savings
            Assert.AreEqual("$1,445.36", testRow.Tots);
            Assert.AreEqual("$12.35", testRow.Avgs);
            Assert.AreEqual("1.85 %", testRow.Svgs);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 6); //Negotiated Savings
            Assert.AreEqual("$0.00", testRow.Tots);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 8); //Rail charges
            Assert.IsNull(testRow);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 15); //Cars rented
            Assert.AreEqual("38", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 16); //Days rented
            Assert.AreEqual("111", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 17); //Car charges
            Assert.AreEqual("$2,669.98", testRow.Tots);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 21); //Bookings
            Assert.AreEqual("62", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 22); //Nights rented
            Assert.AreEqual("137", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 23); //Hotel charges
            Assert.AreEqual("$12,537.52", testRow.Tots);
            


            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 27); //Total charges
            Assert.AreEqual("$93,188.79", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 28); //exceptions
            Assert.AreEqual("73", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 29); //exception lost amount
            Assert.AreEqual("$13,982.88", testRow.Tots);


            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportBackOfficeDepartureDateSeparateRail()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,6,23", "ENDDATE");
            ManipulateReportHandoffRecords("ON", "CBSEPARATERAIL");
            InsertReportHandoff();

            //run the report
            var rpt = (Qview2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 1); //trips
            Assert.AreEqual("1124", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 2); //Air charges
            Assert.AreEqual("$798,095.86", testRow.Tots);
            Assert.AreEqual("$710.05", testRow.Avgs);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 3); //Savings
            Assert.AreEqual("$542,330.17", testRow.Tots);
            Assert.AreEqual("$482.50", testRow.Avgs);
            Assert.AreEqual("40.46 %", testRow.Svgs);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 4); //trips
            Assert.AreEqual("31", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 5); //Lost Savings
            Assert.AreEqual("$6,957.33", testRow.Tots);
            Assert.AreEqual("$6.19", testRow.Avgs);
            Assert.AreEqual("0.87 %", testRow.Svgs);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 6); //Negotiated Savings
            Assert.AreEqual("$-1,116.43", testRow.Tots);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 7); // railtrips
            Assert.AreEqual("18", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 8); //Rail charges
            Assert.AreEqual("$3,155.36", testRow.Tots);
            Assert.AreEqual("$175.30", testRow.Avgs);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 9); //Savings
            Assert.AreEqual("$0.00", testRow.Tots);
            Assert.AreEqual("$0.00", testRow.Avgs);
            Assert.AreEqual("0.00 %", testRow.Svgs);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 10); //trips
            Assert.AreEqual("0", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 11); //Lost Savings
            Assert.AreEqual("$0.00", testRow.Tots);
            Assert.AreEqual("$0.00", testRow.Avgs);
            Assert.AreEqual("0.00 %", testRow.Svgs);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 12); //Negotiated Savings
            Assert.AreEqual("$0.00", testRow.Tots);


            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 15); //Cars rented
            Assert.AreEqual("289", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 16); //Days rented
            Assert.AreEqual("727", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 17); //Car charges
            Assert.AreEqual("$19,756.42", testRow.Tots);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 21); //Bookings
            Assert.AreEqual("453", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 22); //Nights rented
            Assert.AreEqual("866", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 23); //Hotel charges
            Assert.AreEqual("$89,197.26", testRow.Tots);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 27); //Total charges
            Assert.AreEqual("$910,204.90", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 28); //exceptions
            Assert.AreEqual("496", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 29); //exception lost amount
            Assert.AreEqual("$96,558.83", testRow.Tots);


            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportBackOfficeInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            InsertReportHandoff();

            //run the report
            var rpt = (Qview2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 1); //trips
            Assert.AreEqual("107", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 2); //Air charges
            Assert.AreEqual("$77,901.14", testRow.Tots);
            Assert.AreEqual("$728.05", testRow.Avgs);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 3); //Savings
            Assert.AreEqual("$31,200.22", testRow.Tots);
            Assert.AreEqual("$291.59", testRow.Avgs);
            Assert.AreEqual("28.60 %", testRow.Svgs);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 4); //trips
            Assert.AreEqual("8", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 5); //Lost Savings
            Assert.AreEqual("$654.57", testRow.Tots);
            Assert.AreEqual("$6.12", testRow.Avgs);
            Assert.AreEqual("0.84 %", testRow.Svgs);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 6); //Negotiated Savings
            Assert.AreEqual("$0.00", testRow.Tots);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 8); //Rail charges
            Assert.IsNull(testRow);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 15); //Cars rented
            Assert.AreEqual("38", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 16); //Days rented
            Assert.AreEqual("101", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 17); //Car charges
            Assert.AreEqual("$2,608.60", testRow.Tots);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 21); //Bookings
            Assert.AreEqual("50", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 22); //Nights rented
            Assert.AreEqual("94", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 23); //Hotel charges
            Assert.AreEqual("$8,596.38", testRow.Tots);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 27); //Total charges
            Assert.AreEqual("$89,106.12", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 28); //exceptions
            Assert.AreEqual("58", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 29); //exception lost amount
            Assert.AreEqual("$9,250.95", testRow.Tots);

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportReservationDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1","PREPOST");
            ManipulateReportHandoffRecords("DT:2015,7,1", "BEGDATE");
            InsertReportHandoff();

            //run the report
            var rpt = (Qview2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 1); //trips
            Assert.AreEqual("25", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 2); //Air charges
            Assert.AreEqual("$44,854.25", testRow.Tots);
            Assert.AreEqual("$1,794.17", testRow.Avgs);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 3); //Savings
            Assert.AreEqual("$10,250.39", testRow.Tots);
            Assert.AreEqual("$410.02", testRow.Avgs);
            Assert.AreEqual("18.60 %", testRow.Svgs);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 4); //trips
            Assert.AreEqual("9", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 5); //Lost Savings
            Assert.AreEqual("$12,628.56", testRow.Tots);
            Assert.AreEqual("$505.14", testRow.Avgs);
            Assert.AreEqual("28.15 %", testRow.Svgs);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 6); //Negotiated Savings
            Assert.AreEqual("$0.00", testRow.Tots);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 8); //Rail charges
            Assert.IsNull(testRow);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 15); //Cars rented
            Assert.AreEqual("1", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 16); //Days rented
            Assert.AreEqual("12", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 17); //Car charges
            Assert.AreEqual("$721.32", testRow.Tots);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 21); //Bookings
            Assert.AreEqual("12", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 22); //Nights rented
            Assert.AreEqual("31", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 23); //Hotel charges
            Assert.AreEqual("$6,901.18", testRow.Tots);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 27); //Total charges
            Assert.AreEqual("$52,476.75", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 28); //exceptions
            Assert.AreEqual("22", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 29); //exception lost amount
            Assert.AreEqual("$20,251.06", testRow.Tots);


            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportReservationInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2016,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2016,9,23", "ENDDATE");
            InsertReportHandoff();

            //run the report
            var rpt = (Qview2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 1); //trips
            Assert.AreEqual("2", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 2); //Air charges
            Assert.AreEqual("$268.20", testRow.Tots);
            Assert.AreEqual("$134.10", testRow.Avgs);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 3); //Savings
            Assert.AreEqual("$518.00", testRow.Tots);
            Assert.AreEqual("$259.00", testRow.Avgs);
            Assert.AreEqual("65.89 %", testRow.Svgs);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 4); //trips
            Assert.AreEqual("0", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 5); //Lost Savings
            Assert.AreEqual("$0.00", testRow.Tots);
            Assert.AreEqual("$0.00", testRow.Avgs);
            Assert.AreEqual("0.00 %", testRow.Svgs);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 6); //Negotiated Savings
            Assert.AreEqual("$0.00", testRow.Tots);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 8); //Rail charges
            Assert.IsNull(testRow);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 15); //Cars rented
            Assert.AreEqual("3", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 16); //Days rented
            Assert.AreEqual("9", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 17); //Car charges
            Assert.AreEqual("$513.58", testRow.Tots);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 21); //Bookings
            Assert.AreEqual("5", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 22); //Nights rented
            Assert.AreEqual("15", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 23); //Hotel charges
            Assert.AreEqual("$3,957.10", testRow.Tots);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 27); //Total charges
            Assert.AreEqual("$4,738.88", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 28); //exceptions
            Assert.AreEqual("8", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 29); //exception lost amount
            Assert.AreEqual("$4,470.68", testRow.Tots);

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportReservationBookedDate()
        {
            GenerateReportHandoff(DateType.BookedDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,15", "ENDDATE");
            InsertReportHandoff();

            //run the report
            var rpt = (Qview2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 1); //trips
            Assert.AreEqual("8501", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 2); //Air charges
            Assert.AreEqual("$5,279,181.21", testRow.Tots);
            Assert.AreEqual("$621.01", testRow.Avgs);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 3); //Savings
            Assert.AreEqual("$3,400,047.46", testRow.Tots);
            Assert.AreEqual("$399.96", testRow.Avgs);
            Assert.AreEqual("39.17 %", testRow.Svgs);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 4); //trips
            Assert.AreEqual("2826", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 5); //Lost Savings
            Assert.AreEqual("$975,522.45", testRow.Tots);
            Assert.AreEqual("$114.75", testRow.Avgs);
            Assert.AreEqual("18.48 %", testRow.Svgs);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 6); //Negotiated Savings
            Assert.AreEqual("$259,190.83", testRow.Tots);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 8); //Rail charges
            Assert.IsNull(testRow);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 15); //Cars rented
            Assert.AreEqual("3042", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 16); //Days rented
            Assert.AreEqual("9377", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 17); //Car charges
            Assert.AreEqual("$920,651.76", testRow.Tots);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 21); //Bookings
            Assert.AreEqual("4944", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 22); //Nights rented
            Assert.AreEqual("11037", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 23); //Hotel charges
            Assert.AreEqual("$1,503,549.49", testRow.Tots);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 27); //Total charges
            Assert.AreEqual("$7,703,382.46", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 28); //exceptions
            Assert.AreEqual("10812", testRow.Tots);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 29); //exception lost amount
            Assert.AreEqual("$3,399,723.70", testRow.Tots);

            ClearReportHandoff();
        }

        #endregion

        #region Domestic/International breakdown
        [TestMethod]
        public void GenerateReportBackOfficeDepartureDateDit()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("ON", "CBSHOWBRKBYDOMINTL");
            InsertReportHandoff();

            //run the report
            var rpt = (Qview2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 1); //trips
            Assert.AreEqual("59", testRow.Tots1);
            Assert.AreEqual("58", testRow.Tots2);
            Assert.AreEqual("117", testRow.Tots3);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 2); //Air charges
            Assert.AreEqual("$27,529.57", testRow.Tots1);
            Assert.AreEqual("$50,451.72", testRow.Tots2);
            Assert.AreEqual("$77,981.29", testRow.Tots3);

            Assert.AreEqual("$466.60", testRow.Avgs1);
            Assert.AreEqual("$869.86", testRow.Avgs2);
            Assert.AreEqual("$666.51", testRow.Avgs3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 3); //Savings
            Assert.AreEqual("$23,107.67", testRow.Tots1);
            Assert.AreEqual("$12,919.65", testRow.Tots2);
            Assert.AreEqual("$36,027.32", testRow.Tots3);

            Assert.AreEqual("$391.66", testRow.Avgs1);
            Assert.AreEqual("$222.75", testRow.Avgs2);
            Assert.AreEqual("$307.93", testRow.Avgs3);

            Assert.AreEqual("45.63 %", testRow.Svgs1);
            Assert.AreEqual("20.39 %", testRow.Svgs2);
            Assert.AreEqual("31.60 %", testRow.Svgs3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 4); //trips
            Assert.AreEqual("9", testRow.Tots1);
            Assert.AreEqual("2", testRow.Tots2);
            Assert.AreEqual("11", testRow.Tots3);


            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 5); //Lost Savings
            Assert.AreEqual("$1,028.49", testRow.Tots1);
            Assert.AreEqual("$416.87", testRow.Tots2);
            Assert.AreEqual("$1,445.36", testRow.Tots3);

            Assert.AreEqual("$17.43", testRow.Avgs1);
            Assert.AreEqual("$7.19", testRow.Avgs2);
            Assert.AreEqual("$12.35", testRow.Avgs3);

            Assert.AreEqual("3.74 %", testRow.Svgs1);
            Assert.AreEqual("0.83 %", testRow.Svgs2);
            Assert.AreEqual("1.85 %", testRow.Svgs3);


            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 6); //Negotiated Savings
            Assert.AreEqual("$0.00", testRow.Tots1);
            Assert.AreEqual("$0.00", testRow.Tots2);
            Assert.AreEqual("$0.00", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 15); //Cars rented
            Assert.AreEqual("17", testRow.Tots1);
            Assert.AreEqual("21", testRow.Tots2);
            Assert.AreEqual("38", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 16); //Days rented
            Assert.AreEqual("45", testRow.Tots1);
            Assert.AreEqual("66", testRow.Tots2);
            Assert.AreEqual("111", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 17); //Car charges
            Assert.AreEqual("$0.00", testRow.Tots1);
            Assert.AreEqual("$2,669.98", testRow.Tots2);
            Assert.AreEqual("$2,669.98", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 21); //Bookings
            Assert.AreEqual("23", testRow.Tots1);
            Assert.AreEqual("39", testRow.Tots2);
            Assert.AreEqual("62", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 22); //Nights rented
            Assert.AreEqual("40", testRow.Tots1);
            Assert.AreEqual("97", testRow.Tots2);
            Assert.AreEqual("137", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 23); //Hotel charges
            Assert.AreEqual("$163.54", testRow.Tots1);
            Assert.AreEqual("$12,373.98", testRow.Tots2);
            Assert.AreEqual("$12,537.52", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 27); //Total charges
            Assert.AreEqual("$27,693.11", testRow.Tots1);
            Assert.AreEqual("$65,495.68", testRow.Tots2);
            Assert.AreEqual("$93,188.79", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 28); //exceptions
            Assert.AreEqual("32", testRow.Tots1);
            Assert.AreEqual("41", testRow.Tots2);
            Assert.AreEqual("73", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 29); //exception lost amount
            Assert.AreEqual("$1,192.03", testRow.Tots1);
            Assert.AreEqual("$12,790.85", testRow.Tots2);
            Assert.AreEqual("$13,982.88", testRow.Tots3);

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportBackOfficeDepartureDateDitSeparateRail()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("ON", "CBSHOWBRKBYDOMINTL");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,6,23", "ENDDATE");
            ManipulateReportHandoffRecords("ON", "CBSEPARATERAIL");
            InsertReportHandoff();

            //run the report
            var rpt = (Qview2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 1); //trips
            Assert.AreEqual("513", testRow.Tots1);
            Assert.AreEqual("611", testRow.Tots2);
            Assert.AreEqual("1124", testRow.Tots3);
            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 2); //Air charges
            Assert.AreEqual("$236,639.77", testRow.Tots1);
            Assert.AreEqual("$561,456.09", testRow.Tots2);
            Assert.AreEqual("$798,095.86", testRow.Tots3);

            Assert.AreEqual("$461.29", testRow.Avgs1);
            Assert.AreEqual("$918.91", testRow.Avgs2);
            Assert.AreEqual("$710.05", testRow.Avgs3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 3); //Savings
            Assert.AreEqual("$222,947.27", testRow.Tots1);
            Assert.AreEqual("$319,382.90", testRow.Tots2);
            Assert.AreEqual("$542,330.17", testRow.Tots3);

            Assert.AreEqual("$434.60", testRow.Avgs1);
            Assert.AreEqual("$522.72", testRow.Avgs2);
            Assert.AreEqual("$482.50", testRow.Avgs3);

            Assert.AreEqual("48.51 %", testRow.Svgs1);
            Assert.AreEqual("36.26 %", testRow.Svgs2);
            Assert.AreEqual("40.46 %", testRow.Svgs3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 4); //trips
            Assert.AreEqual("10", testRow.Tots1);
            Assert.AreEqual("21", testRow.Tots2);
            Assert.AreEqual("31", testRow.Tots3);


            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 5); //Lost Savings
            Assert.AreEqual("$1,517.83", testRow.Tots1);
            Assert.AreEqual("$5,439.50", testRow.Tots2);
            Assert.AreEqual("$6,957.33", testRow.Tots3);

            Assert.AreEqual("$2.96", testRow.Avgs1);
            Assert.AreEqual("$8.90", testRow.Avgs2);
            Assert.AreEqual("$6.19", testRow.Avgs3);

            Assert.AreEqual("0.64 %", testRow.Svgs1);
            Assert.AreEqual("0.97 %", testRow.Svgs2);
            Assert.AreEqual("0.87 %", testRow.Svgs3);


            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 6); //Negotiated Savings
            Assert.AreEqual("$0.00", testRow.Tots1);
            Assert.AreEqual("$-1,116.43", testRow.Tots2);
            Assert.AreEqual("$-1,116.43", testRow.Tots3);


            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 7); // Rail trips
            Assert.AreEqual("18", testRow.Tots1);
            Assert.AreEqual("0", testRow.Tots2);
            Assert.AreEqual("18", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 8); //Rail charges
            Assert.AreEqual("$3,155.36", testRow.Tots1);
            Assert.AreEqual("$0.00", testRow.Tots2);
            Assert.AreEqual("$3,155.36", testRow.Tots3);

            Assert.AreEqual("$175.30", testRow.Avgs1);
            Assert.AreEqual("$0.00", testRow.Avgs2);
            Assert.AreEqual("$175.30", testRow.Avgs3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 9); //Savings
            Assert.AreEqual("$0.00", testRow.Tots1);
            Assert.AreEqual("$0.00", testRow.Tots2);
            Assert.AreEqual("$0.00", testRow.Tots3);

            Assert.AreEqual("$0.00", testRow.Avgs1);
            Assert.AreEqual("$0.00", testRow.Avgs2);
            Assert.AreEqual("$0.00", testRow.Avgs3);

            Assert.AreEqual("0.00 %", testRow.Svgs1);
            Assert.AreEqual("0.00 %", testRow.Svgs2);
            Assert.AreEqual("0.00 %", testRow.Svgs3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 10); //exceptions
            Assert.AreEqual("0", testRow.Tots1);
            Assert.AreEqual("0", testRow.Tots2);
            Assert.AreEqual("0", testRow.Tots3);


            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 11); //Lost Savings
            Assert.AreEqual("$0.00", testRow.Tots1);
            Assert.AreEqual("$0.00", testRow.Tots2);
            Assert.AreEqual("$0.00", testRow.Tots3);

            Assert.AreEqual("$0.00", testRow.Avgs1);
            Assert.AreEqual("$0.00", testRow.Avgs2);
            Assert.AreEqual("$0.00", testRow.Avgs3);

            Assert.AreEqual("0.00 %", testRow.Svgs1);
            Assert.AreEqual("0.00 %", testRow.Svgs2);
            Assert.AreEqual("0.00 %", testRow.Svgs3);


            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 12); //Negotiated Savings
            Assert.AreEqual("$0.00", testRow.Tots1);
            Assert.AreEqual("$0.00", testRow.Tots2);
            Assert.AreEqual("$0.00", testRow.Tots3);



            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 15); //Cars rented
            Assert.AreEqual("169", testRow.Tots1);
            Assert.AreEqual("120", testRow.Tots2);
            Assert.AreEqual("289", testRow.Tots3);


            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 16); //Days rented
            Assert.AreEqual("355", testRow.Tots1);
            Assert.AreEqual("372", testRow.Tots2);
            Assert.AreEqual("727", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 17); //Car charges
            Assert.AreEqual("$2,568.06", testRow.Tots1);
            Assert.AreEqual("$17,188.36", testRow.Tots2);
            Assert.AreEqual("$19,756.42", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 21); //Bookings
            Assert.AreEqual("229", testRow.Tots1);
            Assert.AreEqual("224", testRow.Tots2);
            Assert.AreEqual("453", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 22); //Nights rented
            Assert.AreEqual("331", testRow.Tots1);
            Assert.AreEqual("535", testRow.Tots2);
            Assert.AreEqual("866", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 23); //Hotel charges
            Assert.AreEqual("$6,514.82", testRow.Tots1);
            Assert.AreEqual("$82,682.44", testRow.Tots2);
            Assert.AreEqual("$89,197.26", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 27); //Total charges
            Assert.AreEqual("$248,878.01", testRow.Tots1);
            Assert.AreEqual("$661,326.89", testRow.Tots2);
            Assert.AreEqual("$910,204.90", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 28); //exceptions
            Assert.AreEqual("246", testRow.Tots1);
            Assert.AreEqual("250", testRow.Tots2);
            Assert.AreEqual("496", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 29); //exception lost amount
            Assert.AreEqual("$8,073.03", testRow.Tots1);
            Assert.AreEqual("$88,485.80", testRow.Tots2);
            Assert.AreEqual("$96,558.83", testRow.Tots3);

            ClearReportHandoff();
        }
        #endregion

        #region Comparison dates
        [TestMethod]
        public void GenerateReportBackOfficeDepartureDateComp()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,2,28", "ENDDATE");
            ManipulateReportHandoffRecords("DT:2015,3,1", "BEGDATE2");
            ManipulateReportHandoffRecords("DT:2015,6,23", "ENDDATE2");
            InsertReportHandoff();

            //run the report
            var rpt = (Qview2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 1); //trips
            Assert.AreEqual("380", testRow.Tots);
            Assert.AreEqual("762", testRow.Tots2);
            Assert.AreEqual("382", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 2); //Air charges
            Assert.AreEqual("$254,462.29", testRow.Tots);
            Assert.AreEqual("$546,788.93", testRow.Tots2);
            Assert.AreEqual("$292,326.64", testRow.Tots3);

            Assert.AreEqual("$669.64", testRow.Avgs);
            Assert.AreEqual("$717.57", testRow.Avgs2);
            Assert.AreEqual("$47.93", testRow.Avgs3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 3); //Savings
            Assert.AreEqual("$196,074.06", testRow.Tots);
            Assert.AreEqual("$346,256.11", testRow.Tots2);
            Assert.AreEqual("$150,182.05", testRow.Tots3);

            Assert.AreEqual("$515.98", testRow.Avgs);
            Assert.AreEqual("$454.40", testRow.Avgs2);
            Assert.AreEqual("$-61.58", testRow.Avgs3);

            Assert.AreEqual("43.52 %", testRow.Svgs);
            Assert.AreEqual("38.77 %", testRow.Svgs2);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 4); //trips
            Assert.AreEqual("5", testRow.Tots);
            Assert.AreEqual("26", testRow.Tots2);
            Assert.AreEqual("21", testRow.Tots3);


            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 5); //Lost Savings
            Assert.AreEqual("$1,556.54", testRow.Tots);
            Assert.AreEqual("$5,400.79", testRow.Tots2);
            Assert.AreEqual("$3,844.25", testRow.Tots3);

            Assert.AreEqual("$4.10", testRow.Avgs);
            Assert.AreEqual("$7.09", testRow.Avgs2);
            Assert.AreEqual("$2.99", testRow.Avgs3);

            Assert.AreEqual("0.61 %", testRow.Svgs);
            Assert.AreEqual("0.99 %", testRow.Svgs2);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 6); //Negotiated Savings
            Assert.AreEqual("$0.00", testRow.Tots);
            Assert.AreEqual("$-1,116.43", testRow.Tots2);
            Assert.AreEqual("$-1,116.43", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 15); //Cars rented
            Assert.AreEqual("102", testRow.Tots);
            Assert.AreEqual("187", testRow.Tots2);
            Assert.AreEqual("85", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 16); //Days rented
            Assert.AreEqual("249", testRow.Tots);
            Assert.AreEqual("478", testRow.Tots2);
            Assert.AreEqual("229", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 17); //Car charges
            Assert.AreEqual("$7,789.45", testRow.Tots);
            Assert.AreEqual("$11,966.97", testRow.Tots2);
            Assert.AreEqual("$4,177.52", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 21); //Bookings
            Assert.AreEqual("136", testRow.Tots);
            Assert.AreEqual("317", testRow.Tots2);
            Assert.AreEqual("181", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 22); //Nights rented
            Assert.AreEqual("263", testRow.Tots);
            Assert.AreEqual("603", testRow.Tots2);
            Assert.AreEqual("340", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 23); //Hotel charges
            Assert.AreEqual("$30,780.11", testRow.Tots);
            Assert.AreEqual("$58,417.15", testRow.Tots2);
            Assert.AreEqual("$27,637.04", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 27); //Total charges
            Assert.AreEqual("$293,031.85", testRow.Tots);
            Assert.AreEqual("$617,173.05", testRow.Tots2);
            Assert.AreEqual("$324,141.20", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 28); //exceptions
            Assert.AreEqual("153", testRow.Tots);
            Assert.AreEqual("343", testRow.Tots2);
            Assert.AreEqual("190", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 29); //exception lost amount
            Assert.AreEqual("$32,740.89", testRow.Tots);
            Assert.AreEqual("$63,817.94", testRow.Tots2);
            Assert.AreEqual("$31,077.05", testRow.Tots3);

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportBackOfficeDepartureDateCompSeparateRail()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,2,28", "ENDDATE");
            ManipulateReportHandoffRecords("DT:2015,3,1", "BEGDATE2");
            ManipulateReportHandoffRecords("DT:2015,6,23", "ENDDATE2");
            ManipulateReportHandoffRecords("ON", "CBSEPARATERAIL");
            InsertReportHandoff();

            //run the report
            var rpt = (Qview2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 1); //trips
            Assert.AreEqual("369", testRow.Tots);
            Assert.AreEqual("755", testRow.Tots2);
            Assert.AreEqual("386", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 2); //Air charges
            Assert.AreEqual("$253,130.35", testRow.Tots);
            Assert.AreEqual("$544,965.51", testRow.Tots2);
            Assert.AreEqual("$291,835.16", testRow.Tots3);

            Assert.AreEqual("$685.99", testRow.Avgs);
            Assert.AreEqual("$721.81", testRow.Avgs2);
            Assert.AreEqual("$35.82", testRow.Avgs3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 3); //Savings
            Assert.AreEqual("$196,074.06", testRow.Tots);
            Assert.AreEqual("$346,256.11", testRow.Tots2);
            Assert.AreEqual("$150,182.05", testRow.Tots3);

            Assert.AreEqual("$531.37", testRow.Avgs);
            Assert.AreEqual("$458.62", testRow.Avgs2);
            Assert.AreEqual("$-72.75", testRow.Avgs3);

            Assert.AreEqual("43.65 %", testRow.Svgs);
            Assert.AreEqual("38.85 %", testRow.Svgs2);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 4); //trips
            Assert.AreEqual("5", testRow.Tots);
            Assert.AreEqual("26", testRow.Tots2);
            Assert.AreEqual("21", testRow.Tots3);


            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 5); //Lost Savings
            Assert.AreEqual("$1,556.54", testRow.Tots);
            Assert.AreEqual("$5,400.79", testRow.Tots2);
            Assert.AreEqual("$3,844.25", testRow.Tots3);

            Assert.AreEqual("$4.22", testRow.Avgs);
            Assert.AreEqual("$7.15", testRow.Avgs2);
            Assert.AreEqual("$2.94", testRow.Avgs3);

            Assert.AreEqual("0.61 %", testRow.Svgs);
            Assert.AreEqual("0.99 %", testRow.Svgs2);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 6); //Negotiated Savings
            Assert.AreEqual("$0.00", testRow.Tots);
            Assert.AreEqual("$-1,116.43", testRow.Tots2);
            Assert.AreEqual("$-1,116.43", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 7); //trips
            Assert.AreEqual("11", testRow.Tots);
            Assert.AreEqual("7", testRow.Tots2);
            Assert.AreEqual("-4", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 8); //Air charges
            Assert.AreEqual("$1,331.94", testRow.Tots);
            Assert.AreEqual("$1,823.42", testRow.Tots2);
            Assert.AreEqual("$491.48", testRow.Tots3);

            Assert.AreEqual("$121.09", testRow.Avgs);
            Assert.AreEqual("$260.49", testRow.Avgs2);
            Assert.AreEqual("$139.40", testRow.Avgs3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 9); //Savings
            Assert.AreEqual("$0.00", testRow.Tots);
            Assert.AreEqual("$0.00", testRow.Tots2);
            Assert.AreEqual("$0.00", testRow.Tots3);

            Assert.AreEqual("$0.00", testRow.Avgs);
            Assert.AreEqual("$0.00", testRow.Avgs2);
            Assert.AreEqual("$0.00", testRow.Avgs3);

            Assert.AreEqual("0.00 %", testRow.Svgs);
            Assert.AreEqual("0.00 %", testRow.Svgs2);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 10); //trips
            Assert.AreEqual("0", testRow.Tots);
            Assert.AreEqual("0", testRow.Tots2);
            Assert.AreEqual("0", testRow.Tots3);


            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 11); //Lost Savings
            Assert.AreEqual("$0.00", testRow.Tots);
            Assert.AreEqual("$0.00", testRow.Tots2);
            Assert.AreEqual("$0.00", testRow.Tots3);

            Assert.AreEqual("$0.00", testRow.Avgs);
            Assert.AreEqual("$0.00", testRow.Avgs2);
            Assert.AreEqual("$0.00", testRow.Avgs3);

            Assert.AreEqual("0.00 %", testRow.Svgs);
            Assert.AreEqual("0.00 %", testRow.Svgs2);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 12); //Negotiated Savings
            Assert.AreEqual("$0.00", testRow.Tots);
            Assert.AreEqual("$0.00", testRow.Tots2);
            Assert.AreEqual("$0.00", testRow.Tots3);


            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 15); //Cars rented
            Assert.AreEqual("102", testRow.Tots);
            Assert.AreEqual("187", testRow.Tots2);
            Assert.AreEqual("85", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 16); //Days rented
            Assert.AreEqual("249", testRow.Tots);
            Assert.AreEqual("478", testRow.Tots2);
            Assert.AreEqual("229", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 17); //Car charges
            Assert.AreEqual("$7,789.45", testRow.Tots);
            Assert.AreEqual("$11,966.97", testRow.Tots2);
            Assert.AreEqual("$4,177.52", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 21); //Bookings
            Assert.AreEqual("136", testRow.Tots);
            Assert.AreEqual("317", testRow.Tots2);
            Assert.AreEqual("181", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 22); //Nights rented
            Assert.AreEqual("263", testRow.Tots);
            Assert.AreEqual("603", testRow.Tots2);
            Assert.AreEqual("340", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 23); //Hotel charges
            Assert.AreEqual("$30,780.11", testRow.Tots);
            Assert.AreEqual("$58,417.15", testRow.Tots2);
            Assert.AreEqual("$27,637.04", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 27); //Total charges
            Assert.AreEqual("$293,031.85", testRow.Tots);
            Assert.AreEqual("$617,173.05", testRow.Tots2);
            Assert.AreEqual("$324,141.20", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 28); //exceptions
            Assert.AreEqual("153", testRow.Tots);
            Assert.AreEqual("343", testRow.Tots2);
            Assert.AreEqual("190", testRow.Tots3);

            testRow = rpt.FinalDataList.FirstOrDefault(s => s.Rownum == 29); //exception lost amount
            Assert.AreEqual("$32,740.89", testRow.Tots);
            Assert.AreEqual("$63,817.94", testRow.Tots2);
            Assert.AreEqual("$31,077.05", testRow.Tots3);

            ClearReportHandoff();
        }
        #endregion

        public void GenerateReportHandoff(DateType dateType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,8,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,8,23", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTLANGUAGE", ParmValue = "EN", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "32", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibqview2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3383486", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");

        }
    }
}
