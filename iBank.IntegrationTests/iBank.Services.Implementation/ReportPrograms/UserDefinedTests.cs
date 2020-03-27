namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    //TODO: replace missing class
    //[TestClass]
    //public class UserDefinedTests : BaseUnitTest
    //{
    //    public int ReportKey { get; set; }
    //    [TestMethod]
    //    public void GenerateReportNoData()
    //    {
    //        BuildReportAllTripFields();

    //        GenerateReportHandoffRecords(DateType.InvoiceDate, "DT:2016,6,1", "DT:2016,6,9");
    //        InsertReportHandoff();

    //        //run the report
    //        var rpt = (UserDefined)RunReport();
    //        var rptInfo = rpt.Globals.ReportInformation;

    //        //check for validity
    //        Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
    //        var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_NoData);
    //        Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

    //        ClearReportHandoff();
    //        DeleteReport();
    //    }

    //    [TestMethod]
    //    public void TestTripOnlyFieldsReservationInvoiceDate()
    //    {
    //        BuildReportAllTripFields();

    //        GenerateReportHandoffRecords(DateType.InvoiceDate, "DT:2015,6,1", "DT:2015,6,30");
    //        InsertReportHandoff();

    //        //run the report
    //        var rpt = (UserDefined)RunReport();
    //        var rptInfo = rpt.Globals.ReportInformation;

    //        //check for validity
    //        Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
    //        Assert.AreEqual(rptInfo.ErrorMessage, string.Empty, "Error message failed.");
    //        Assert.AreEqual(rpt.RawDataList.Count,4, "Incorrect count of RawDataList records.");

    //        Assert.AreEqual(rpt.UserDefinedParams.LegDataList.Count, 0, "Incorrect count of LegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.AirLegDataList.Count, 0, "Incorrect count of AirLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.RailLegDataList.Count, 0, "Incorrect count of RailLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.CarDataList.Count, 0, "Incorrect count of CarDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.HotelDataList.Count, 0, "Incorrect count of HotelDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscDataList.Count, 0, "Incorrect count of MiscDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthDataList.Count, 0, "Incorrect count of TravAuthDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthorizerDataList.Count, 0, "Incorrect count of TravAuthorizerDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ChangeLogDataList.Count, 0, "Incorrect count of ChangeLogDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ServiceFeeDataList.Count, 0, "Incorrect count of ServiceFeeDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.UdidDataList.Count, 0, "Incorrect count of UdidDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MarketSegmentDataList.Count, 0, "Incorrect count of MarketSegmentDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegTourDataList.Count, 0, "Incorrect count of MiscSegTourDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegCruiseDataList.Count, 0, "Incorrect count of MiscSegCruiseDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegLimoDataList.Count, 0, "Incorrect count of MiscSegLimoDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegRailTicketDataList.Count, 0, "Incorrect count of MiscSegRailTicketDataList records.");
            

    //        ClearReportHandoff();
    //        DeleteReport();
    //    }

    //    [TestMethod]
    //    public void TestTripOnlyFieldsReservationDepartureDate()
    //    {
    //        BuildReportAllTripFields();

    //        GenerateReportHandoffRecords(DateType.DepartureDate, "DT:2015,6,1", "DT:2015,6,9");
    //        InsertReportHandoff();

    //        //run the report
    //        var rpt = (UserDefined)RunReport();
    //        var rptInfo = rpt.Globals.ReportInformation;

    //        //check for validity
    //        Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
    //        Assert.AreEqual(rptInfo.ErrorMessage, string.Empty, "Error message failed.");
    //        Assert.AreEqual(rpt.RawDataList.Count, 25, "Incorrect count of RawDataList records.");

    //        Assert.AreEqual(rpt.UserDefinedParams.LegDataList.Count, 0, "Incorrect count of LegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.AirLegDataList.Count, 0, "Incorrect count of AirLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.RailLegDataList.Count, 0, "Incorrect count of RailLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.CarDataList.Count, 0, "Incorrect count of CarDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.HotelDataList.Count, 0, "Incorrect count of HotelDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscDataList.Count, 0, "Incorrect count of MiscDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthDataList.Count, 0, "Incorrect count of TravAuthDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthorizerDataList.Count, 0, "Incorrect count of TravAuthorizerDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ChangeLogDataList.Count, 0, "Incorrect count of ChangeLogDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ServiceFeeDataList.Count, 0, "Incorrect count of ServiceFeeDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.UdidDataList.Count, 0, "Incorrect count of UdidDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MarketSegmentDataList.Count, 0, "Incorrect count of MarketSegmentDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegTourDataList.Count, 0, "Incorrect count of MiscSegTourDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegCruiseDataList.Count, 0, "Incorrect count of MiscSegCruiseDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegLimoDataList.Count, 0, "Incorrect count of MiscSegLimoDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegRailTicketDataList.Count, 0, "Incorrect count of MiscSegRailTicketDataList records.");


    //        ClearReportHandoff();
    //        DeleteReport();
    //    }

    //    [TestMethod]
    //    public void TestTripOnlyFieldsReservationOnTheRoad()
    //    {
    //        BuildReportAllTripFields();

    //        GenerateReportHandoffRecords(DateType.OnTheRoadDatesSpecial, "DT:2015,6,1", "DT:2015,6,30");
    //        InsertReportHandoff();

    //        //run the report
    //        var rpt = (UserDefined)RunReport();
    //        var rptInfo = rpt.Globals.ReportInformation;

    //        //check for validity
    //        Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
    //        Assert.AreEqual(rptInfo.ErrorMessage, string.Empty, "Error message failed.");
    //        Assert.AreEqual(rpt.RawDataList.Count, 83, "Incorrect count of RawDataList records.");

    //        Assert.AreEqual(rpt.UserDefinedParams.LegDataList.Count, 0, "Incorrect count of LegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.AirLegDataList.Count, 0, "Incorrect count of AirLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.RailLegDataList.Count, 0, "Incorrect count of RailLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.CarDataList.Count, 0, "Incorrect count of CarDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.HotelDataList.Count, 0, "Incorrect count of HotelDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscDataList.Count, 0, "Incorrect count of MiscDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthDataList.Count, 0, "Incorrect count of TravAuthDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthorizerDataList.Count, 0, "Incorrect count of TravAuthorizerDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ChangeLogDataList.Count, 0, "Incorrect count of ChangeLogDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ServiceFeeDataList.Count, 0, "Incorrect count of ServiceFeeDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.UdidDataList.Count, 0, "Incorrect count of UdidDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MarketSegmentDataList.Count, 0, "Incorrect count of MarketSegmentDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegTourDataList.Count, 0, "Incorrect count of MiscSegTourDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegCruiseDataList.Count, 0, "Incorrect count of MiscSegCruiseDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegLimoDataList.Count, 0, "Incorrect count of MiscSegLimoDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegRailTicketDataList.Count, 0, "Incorrect count of MiscSegRailTicketDataList records.");


    //        ClearReportHandoff();
    //        DeleteReport();
    //    }

    //    [TestMethod]
    //    public void TestTripOnlyFieldsReservationBookedDate()
    //    {
    //        BuildReportAllTripFields();

    //        GenerateReportHandoffRecords(DateType.BookedDate, "DT:2015,1,1", "DT:2015,1,1");
    //        InsertReportHandoff();

    //        //run the report
    //        var rpt = (UserDefined)RunReport();
    //        var rptInfo = rpt.Globals.ReportInformation;

    //        //check for validity
    //        Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
    //        Assert.AreEqual(rptInfo.ErrorMessage, string.Empty, "Error message failed.");
    //        Assert.AreEqual(rpt.RawDataList.Count, 86, "Incorrect count of RawDataList records.");

    //        Assert.AreEqual(rpt.UserDefinedParams.LegDataList.Count, 0, "Incorrect count of LegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.AirLegDataList.Count, 0, "Incorrect count of AirLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.RailLegDataList.Count, 0, "Incorrect count of RailLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.CarDataList.Count, 0, "Incorrect count of CarDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.HotelDataList.Count, 0, "Incorrect count of HotelDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscDataList.Count, 0, "Incorrect count of MiscDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthDataList.Count, 0, "Incorrect count of TravAuthDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthorizerDataList.Count, 0, "Incorrect count of TravAuthorizerDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ChangeLogDataList.Count, 0, "Incorrect count of ChangeLogDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ServiceFeeDataList.Count, 0, "Incorrect count of ServiceFeeDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.UdidDataList.Count, 0, "Incorrect count of UdidDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MarketSegmentDataList.Count, 0, "Incorrect count of MarketSegmentDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegTourDataList.Count, 0, "Incorrect count of MiscSegTourDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegCruiseDataList.Count, 0, "Incorrect count of MiscSegCruiseDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegLimoDataList.Count, 0, "Incorrect count of MiscSegLimoDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegRailTicketDataList.Count, 0, "Incorrect count of MiscSegRailTicketDataList records.");


    //        ClearReportHandoff();
    //        DeleteReport();
    //    }

    //    [TestMethod]
    //    public void TestTripOnlyFieldsBackOffice()
    //    {
    //        BuildReportAllTripFields();

    //        GenerateReportHandoffRecords(DateType.DepartureDate, "DT:2015,6,1", "DT:2015,6,9","2");
    //        InsertReportHandoff();

    //        //run the report
    //        var rpt = (UserDefined)RunReport();
    //        var rptInfo = rpt.Globals.ReportInformation;

    //        //check for validity
    //        Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
    //        Assert.AreEqual(rptInfo.ErrorMessage, string.Empty, "Error message failed.");
    //        Assert.AreEqual(rpt.RawDataList.Count, 75, "Incorrect count of RawDataList records.");

    //        Assert.AreEqual(rpt.UserDefinedParams.LegDataList.Count, 0, "Incorrect count of LegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.AirLegDataList.Count, 0, "Incorrect count of AirLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.RailLegDataList.Count, 0, "Incorrect count of RailLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.CarDataList.Count, 0, "Incorrect count of CarDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.HotelDataList.Count, 0, "Incorrect count of HotelDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscDataList.Count, 0, "Incorrect count of MiscDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthDataList.Count, 0, "Incorrect count of TravAuthDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthorizerDataList.Count, 0, "Incorrect count of TravAuthorizerDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ChangeLogDataList.Count, 0, "Incorrect count of ChangeLogDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ServiceFeeDataList.Count, 0, "Incorrect count of ServiceFeeDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.UdidDataList.Count, 0, "Incorrect count of UdidDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MarketSegmentDataList.Count, 0, "Incorrect count of MarketSegmentDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegTourDataList.Count, 0, "Incorrect count of MiscSegTourDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegCruiseDataList.Count, 0, "Incorrect count of MiscSegCruiseDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegLimoDataList.Count, 0, "Incorrect count of MiscSegLimoDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegRailTicketDataList.Count, 0, "Incorrect count of MiscSegRailTicketDataList records.");


    //        ClearReportHandoff();
    //        DeleteReport();
    //    }

    //    [TestMethod]
    //    public void TestWithCarFields()
    //    {
    //        BuildReportAllTripFields();
    //        AddField("DAYS",4);

    //        GenerateReportHandoffRecords(DateType.DepartureDate, "DT:2015,6,1", "DT:2015,6,9","2");
    //        InsertReportHandoff();

    //        //run the report
    //        var rpt = (UserDefined)RunReport();
    //        var rptInfo = rpt.Globals.ReportInformation;

    //        //check for validity
    //        Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
    //        Assert.AreEqual(rptInfo.ErrorMessage, string.Empty, "Error message failed.");
    //        Assert.AreEqual(rpt.RawDataList.Count, 75, "Incorrect count of RawDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.CarDataList.Count, 20, "Incorrect count of CarDataList records.");

    //        Assert.AreEqual(rpt.UserDefinedParams.AirLegDataList.Count, 0, "Incorrect count of AirLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.RailLegDataList.Count, 0, "Incorrect count of RailLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.LegDataList.Count, 0, "Incorrect count of LegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.HotelDataList.Count, 0, "Incorrect count of HotelDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscDataList.Count, 0, "Incorrect count of MiscDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthDataList.Count, 0, "Incorrect count of TravAuthDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthorizerDataList.Count, 0, "Incorrect count of TravAuthorizerDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ChangeLogDataList.Count, 0, "Incorrect count of ChangeLogDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ServiceFeeDataList.Count, 0, "Incorrect count of ServiceFeeDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.UdidDataList.Count, 0, "Incorrect count of UdidDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MarketSegmentDataList.Count, 0, "Incorrect count of MarketSegmentDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegTourDataList.Count, 0, "Incorrect count of MiscSegTourDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegCruiseDataList.Count, 0, "Incorrect count of MiscSegCruiseDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegLimoDataList.Count, 0, "Incorrect count of MiscSegLimoDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegRailTicketDataList.Count, 0, "Incorrect count of MiscSegRailTicketDataList records.");
    //        ClearReportHandoff();
    //        DeleteReport();
    //    }

    //    [TestMethod]
    //    public void TestWithHotelFields()
    //    {
    //        BuildReportAllTripFields();
    //        AddField("NIGHTS", 4);

    //        GenerateReportHandoffRecords(DateType.DepartureDate, "DT:2015,6,1", "DT:2015,6,9", "2");
    //        InsertReportHandoff();

    //        //run the report
    //        var rpt = (UserDefined)RunReport();
    //        var rptInfo = rpt.Globals.ReportInformation;

    //        //check for validity
    //        Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
    //        Assert.AreEqual(rptInfo.ErrorMessage, string.Empty, "Error message failed.");
    //        Assert.AreEqual(rpt.RawDataList.Count, 75, "Incorrect count of RawDataList records.");

    //        Assert.AreEqual(rpt.UserDefinedParams.LegDataList.Count, 0, "Incorrect count of LegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.AirLegDataList.Count, 0, "Incorrect count of AirLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.RailLegDataList.Count, 0, "Incorrect count of RailLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.CarDataList.Count, 0, "Incorrect count of CarDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.HotelDataList.Count, 20, "Incorrect count of HotelDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscDataList.Count, 0, "Incorrect count of MiscDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthDataList.Count, 0, "Incorrect count of TravAuthDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthorizerDataList.Count, 0, "Incorrect count of TravAuthorizerDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ChangeLogDataList.Count, 0, "Incorrect count of ChangeLogDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ServiceFeeDataList.Count, 0, "Incorrect count of ServiceFeeDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.UdidDataList.Count, 0, "Incorrect count of UdidDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MarketSegmentDataList.Count, 0, "Incorrect count of MarketSegmentDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegTourDataList.Count, 0, "Incorrect count of MiscSegTourDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegCruiseDataList.Count, 0, "Incorrect count of MiscSegCruiseDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegLimoDataList.Count, 0, "Incorrect count of MiscSegLimoDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegRailTicketDataList.Count, 0, "Incorrect count of MiscSegRailTicketDataList records.");

    //        ClearReportHandoff();
    //        DeleteReport();
    //    }

    //    [TestMethod]
    //    public void TestWithLegFields()
    //    {
    //        BuildReportAllTripFields();
    //        AddField("MILES", 4);

    //        GenerateReportHandoffRecords(DateType.DepartureDate, "DT:2015,6,1", "DT:2015,6,9", "2");
    //        InsertReportHandoff();

    //        //run the report
    //        var rpt = (UserDefined)RunReport();
    //        var rptInfo = rpt.Globals.ReportInformation;

    //        //check for validity
    //        Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
    //        Assert.AreEqual(rptInfo.ErrorMessage, string.Empty, "Error message failed.");
    //        Assert.AreEqual(rpt.RawDataList.Count, 75, "Incorrect count of RawDataList records.");

    //        Assert.AreEqual(rpt.UserDefinedParams.LegDataList.Count, 131, "Incorrect count of LegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.AirLegDataList.Count, 131, "Incorrect count of AirLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.RailLegDataList.Count, 0, "Incorrect count of RailLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.CarDataList.Count, 0, "Incorrect count of CarDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.HotelDataList.Count, 0, "Incorrect count of HotelDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscDataList.Count, 0, "Incorrect count of MiscDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthDataList.Count, 0, "Incorrect count of TravAuthDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthorizerDataList.Count, 0, "Incorrect count of TravAuthorizerDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ChangeLogDataList.Count, 0, "Incorrect count of ChangeLogDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ServiceFeeDataList.Count, 0, "Incorrect count of ServiceFeeDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.UdidDataList.Count, 0, "Incorrect count of UdidDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MarketSegmentDataList.Count, 0, "Incorrect count of MarketSegmentDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegTourDataList.Count, 0, "Incorrect count of MiscSegTourDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegCruiseDataList.Count, 0, "Incorrect count of MiscSegCruiseDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegLimoDataList.Count, 0, "Incorrect count of MiscSegLimoDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegRailTicketDataList.Count, 0, "Incorrect count of MiscSegRailTicketDataList records.");


    //        ClearReportHandoff();
    //        DeleteReport();
    //    }

    //    [TestMethod]
    //    public void TestWithMarketSegmentFields()
    //    {
    //        BuildReportAllTripFields();
    //        AddField("CONNECTIME", 4);

    //        GenerateReportHandoffRecords(DateType.DepartureDate, "DT:2015,6,1", "DT:2015,6,9","2");
    //        InsertReportHandoff();

    //        //run the report
    //        var rpt = (UserDefined)RunReport();
    //        var rptInfo = rpt.Globals.ReportInformation;

    //        //check for validity
    //        Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
    //        Assert.AreEqual(rptInfo.ErrorMessage, string.Empty, "Error message failed.");
    //        Assert.AreEqual(rpt.RawDataList.Count, 75, "Incorrect count of RawDataList records.");

    //        Assert.AreEqual(rpt.UserDefinedParams.LegDataList.Count, 0, "Incorrect count of LegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.AirLegDataList.Count, 0, "Incorrect count of AirLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.RailLegDataList.Count, 0, "Incorrect count of RailLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.CarDataList.Count, 0, "Incorrect count of CarDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.HotelDataList.Count, 0, "Incorrect count of HotelDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscDataList.Count, 0, "Incorrect count of MiscDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthDataList.Count, 0, "Incorrect count of TravAuthDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthorizerDataList.Count, 0, "Incorrect count of TravAuthorizerDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ChangeLogDataList.Count, 0, "Incorrect count of ChangeLogDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ServiceFeeDataList.Count, 0, "Incorrect count of ServiceFeeDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.UdidDataList.Count, 0, "Incorrect count of UdidDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MarketSegmentDataList.Count, 131, "Incorrect count of MarketSegmentDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegTourDataList.Count, 0, "Incorrect count of MiscSegTourDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegCruiseDataList.Count, 0, "Incorrect count of MiscSegCruiseDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegLimoDataList.Count, 0, "Incorrect count of MiscSegLimoDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegRailTicketDataList.Count, 0, "Incorrect count of MiscSegRailTicketDataList records.");


    //        ClearReportHandoff();
    //        DeleteReport();
    //    }

    //    //TODO: No data exists in this table for the source DEMOCA01. Checking for zero for now. 
    //    //Same is true for misc, changelog,  types
    //    [TestMethod]
    //    public void TestWithAuthorizerFields()
    //    {
    //        BuildReportAllTripFields();
    //        AddField("AUTHSTATUS", 4);

    //        GenerateReportHandoffRecords(DateType.DepartureDate, "DT:2015,6,1", "DT:2015,6,9", "2");
    //        InsertReportHandoff();

    //        //run the report
    //        var rpt = (UserDefined)RunReport();
    //        var rptInfo = rpt.Globals.ReportInformation;

    //        //check for validity
    //        Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
    //        Assert.AreEqual(rptInfo.ErrorMessage, string.Empty, "Error message failed.");
    //        Assert.AreEqual(rpt.RawDataList.Count, 75, "Incorrect count of RawDataList records.");

    //        Assert.AreEqual(rpt.UserDefinedParams.LegDataList.Count, 0, "Incorrect count of LegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.AirLegDataList.Count, 0, "Incorrect count of AirLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.RailLegDataList.Count, 0, "Incorrect count of RailLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.CarDataList.Count, 0, "Incorrect count of CarDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.HotelDataList.Count, 0, "Incorrect count of HotelDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscDataList.Count, 0, "Incorrect count of MiscDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthDataList.Count, 0, "Incorrect count of TravAuthDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthorizerDataList.Count, 0, "Incorrect count of TravAuthorizerDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ChangeLogDataList.Count, 0, "Incorrect count of ChangeLogDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ServiceFeeDataList.Count, 0, "Incorrect count of ServiceFeeDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.UdidDataList.Count, 0, "Incorrect count of UdidDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MarketSegmentDataList.Count, 0, "Incorrect count of MarketSegmentDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegTourDataList.Count, 0, "Incorrect count of MiscSegTourDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegCruiseDataList.Count, 0, "Incorrect count of MiscSegCruiseDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegLimoDataList.Count, 0, "Incorrect count of MiscSegLimoDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegRailTicketDataList.Count, 0, "Incorrect count of MiscSegRailTicketDataList records.");


    //        ClearReportHandoff();
    //        DeleteReport();
    //    }

    //    [TestMethod]
    //    public void TestWithServiceFeeFields()
    //    {
    //        BuildReportAllTripFields();
    //        AddField("SDESCRIPT", 4);

    //        GenerateReportHandoffRecords(DateType.InvoiceDate, "DT:2015,3,9", "DT:2015,3,9","2");
    //        InsertReportHandoff();

    //        //run the report
    //        var rpt = (UserDefined)RunReport();
    //        var rptInfo = rpt.Globals.ReportInformation;

    //        //check for validity
    //        Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
    //        Assert.AreEqual(rptInfo.ErrorMessage, string.Empty, "Error message failed.");
    //        Assert.AreEqual(rpt.RawDataList.Count, 11, "Incorrect count of RawDataList records.");

    //        Assert.AreEqual(rpt.UserDefinedParams.LegDataList.Count, 0, "Incorrect count of LegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.AirLegDataList.Count, 0, "Incorrect count of AirLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.RailLegDataList.Count, 0, "Incorrect count of RailLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.CarDataList.Count, 0, "Incorrect count of CarDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.HotelDataList.Count, 0, "Incorrect count of HotelDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscDataList.Count, 0, "Incorrect count of MiscDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthDataList.Count, 0, "Incorrect count of TravAuthDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthorizerDataList.Count, 0, "Incorrect count of TravAuthorizerDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ChangeLogDataList.Count, 0, "Incorrect count of ChangeLogDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ServiceFeeDataList.Count, 1, "Incorrect count of ServiceFeeDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.UdidDataList.Count, 0, "Incorrect count of UdidDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MarketSegmentDataList.Count, 0, "Incorrect count of MarketSegmentDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegTourDataList.Count, 0, "Incorrect count of MiscSegTourDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegCruiseDataList.Count, 0, "Incorrect count of MiscSegCruiseDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegLimoDataList.Count, 0, "Incorrect count of MiscSegLimoDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegRailTicketDataList.Count, 0, "Incorrect count of MiscSegRailTicketDataList records.");


    //        ClearReportHandoff();
    //        DeleteReport();
    //    }

    //    [TestMethod]
    //    public void TestWithUdidFields()
    //    {
    //        BuildReportAllTripFields();
    //        AddField("UDID10", 4);

    //        GenerateReportHandoffRecords(DateType.InvoiceDate, "DT:2012,10,29", "DT:2012,10,29", "2");
    //        InsertReportHandoff();

    //        //run the report
    //        var rpt = (UserDefined)RunReport();
    //        var rptInfo = rpt.Globals.ReportInformation;

    //        //check for validity
    //        Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
    //        Assert.AreEqual(rptInfo.ErrorMessage, string.Empty, "Error message failed.");
    //        Assert.AreEqual(rpt.RawDataList.Count, 5, "Incorrect count of RawDataList records.");

    //        Assert.AreEqual(rpt.UserDefinedParams.LegDataList.Count, 0, "Incorrect count of LegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.AirLegDataList.Count, 0, "Incorrect count of AirLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.RailLegDataList.Count, 0, "Incorrect count of RailLegDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.CarDataList.Count, 0, "Incorrect count of CarDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.HotelDataList.Count, 0, "Incorrect count of HotelDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscDataList.Count, 0, "Incorrect count of MiscDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthDataList.Count, 0, "Incorrect count of TravAuthDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.TravAuthorizerDataList.Count, 0, "Incorrect count of TravAuthorizerDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ChangeLogDataList.Count, 0, "Incorrect count of ChangeLogDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.ServiceFeeDataList.Count, 0, "Incorrect count of ServiceFeeDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.UdidDataList.Count, 131, "Incorrect count of UdidDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MarketSegmentDataList.Count, 0, "Incorrect count of MarketSegmentDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegTourDataList.Count, 0, "Incorrect count of MiscSegTourDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegCruiseDataList.Count, 0, "Incorrect count of MiscSegCruiseDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegLimoDataList.Count, 0, "Incorrect count of MiscSegLimoDataList records.");
    //        Assert.AreEqual(rpt.UserDefinedParams.MiscSegRailTicketDataList.Count, 0, "Incorrect count of MiscSegRailTicketDataList records.");


    //        ClearReportHandoff();
    //        DeleteReport();
    //    }



    //    private void GenerateReportHandoffRecords(DateType dateType,string startDate, string endDate, string prePost = "1")
    //    {
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = startDate, ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARBONCALC", ParmValue = "CISCARBON", ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = ((int)dateType).ToString(), ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDSUPPDUPETRIPFLDS", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDTIMEFORMAT", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = endDate, ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "-1", ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PICKRECNUM", ParmValue = "12862", ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = prePost, ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "520", ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibcst500", ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBAPPLYTOLEGORSEG", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3377467", ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = ReportKey.ToString(), ParmInOut = "IN", LangCode = "" });
    //        ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1592", ParmInOut = "IN", LangCode = "" });
    //    }

    //    private void DeleteReport()
    //    {
    //        using (var context = new iBankClientModel())
    //        {
    //            var cols = context.userrpt2.Where(s => s.reportkey == ReportKey);
    //            var rpt = context.userrpts.FirstOrDefault(s => s.reportkey == ReportKey);

    //            if (rpt == null) return;
    //            context.userrpt2.RemoveRange(cols);
    //            context.userrpts.Remove(rpt);

               
    //            context.SaveChanges();

    //        }
    //    }

    //    private void BuildReportAllTripFields()
    //    {
    //        using (var context = new iBankClientQueryable("",""))
    //        {
    //            var usrReport = new userrpts
    //            {
    //                agency = "DEMO",
    //                crname = "UNIT TEST",
    //                UserNumber = 1592,
    //                userid = "UT",
    //                crtype = "COMBDET",
    //                crsubtit = string.Empty,
    //                crtitle = "Trip CO2",
    //                segmentleg = 1,
    //                nodetail = false,
    //                lastused = DateTime.Now,
    //                pgfoottext = string.Empty,
    //                theme = "CLASSIC",
    //                tripsumlvl = 0,

    //            };
    //            context.userrpts.Add(usrReport);

    //            context.SaveChanges();
    //            ReportKey = usrReport.reportkey;

    //            context.userrpt2s.Add(GetColumn(ReportKey, "PAXNAME", 1));
    //            context.userrpt2.Add(GetColumn(ReportKey, "ORIGTICKET", 1));
    //            context.userrpt2.Add(GetColumn(ReportKey, "BASEFARE", 1));

    //            context.SaveChanges();

    //        }
    //    }

    //    private void AddField(string colname,int order)
    //    {
    //        using (var context = new iBankClientModel())
    //        {
               
    //            context.userrpt2.Add(GetColumn(ReportKey, colname, order));
    //            context.SaveChanges();

    //        }
    //    }

    //    private userrpt2 GetColumn(int reportkey,string colName, int order)
    //    {
    //        return new userrpt2
    //        {
    //            reportkey = reportkey,
    //            agency = "DEMO",
    //            crname = "Unit Test",
    //            UserNumber = 1591,
    //            colname = colName,
    //            colorder =(short) order,
    //            sort = 0,
    //            grpbreak = 0,
    //            subtotal = false,
    //            pagebreak = false,
    //            udidhdg1 = "Header1",
    //            udidhdg2 = "Header2",
    //            udidwidth = 20,
    //            udidtype = 1,
    //            horAlign = "0",
    //            goodfld = colName,
    //            goodoper = string.Empty,
    //            goodvalue = string.Empty,
    //            goodhilite = "N",
    //            badfld = colName,
    //            badoper = string.Empty,
    //            badvalue = string.Empty,
    //            badhilite = "N"

    //        };
    //    }
    //}
}
