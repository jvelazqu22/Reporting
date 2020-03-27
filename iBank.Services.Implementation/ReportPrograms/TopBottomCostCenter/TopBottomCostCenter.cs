using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.Graphs;
using Domain.Models.ReportPrograms.TopBottomCostCenter;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomCostCenter
{
    public class TopBottomCostCenter : ReportRunner<RawData, FinalData>
    {
        public decimal TotChg;
        public decimal TotLost;
        public int TotCnt;
        public int TotCnt2;
        public int TotCnt3; //IF nTotcnt3 IS 0, THEN WE USE THAT AS THE INDICATOR IN CRYSTAL TO SUPPRESS "ALL OTHERS" ON THE RPT. 
        public int TotStays;
        public int TotNites;
        public decimal TotHotCost;
        public int TotRents;
        public int TotDays;
        public decimal TotCarCost;
        public decimal TotCost;

        public List<CarRawData> CarRawDataList;
        public List<HotelRawData> HotelRawDataList;
        private string _groupColumn;

        public TopBottomCostCenter()
        {
            CrystalReportName = "ibTopCostCtr";
        }

        public override bool InitialChecks()
        {

            if (!IsGoodCombo()) return false;

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            var server = Globals.AgencyInformation.ServerName;
            var db = Globals.AgencyInformation.DatabaseName;
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(new iBankClientQueryable(server, db), Globals.Agency);

            BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false,buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true, isRoutingBidirectional: false, legDit: false, ignoreTravel: true);

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            _groupColumn = "Break1";
            switch (Globals.GetParmValue(WhereCriteria.GROUPBY))
            {
                case "2":
                    _groupColumn = "Break2";
                    break;
                case "3":
                    _groupColumn = "Break3";
                    break;
            }

            var udidNumber = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1);

            if (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstDestination || BuildWhere.HasFirstOrigin)
            {
                var airSql = SqlBuilder.GetSqlWithRouting(udidNumber > 0, GlobalCalc.IsReservationReport(), BuildWhere, _groupColumn);
                RawDataList = RetrieveRawData<RawData>(airSql, GlobalCalc.IsReservationReport()).ToList();
                if (!IsUnderOfflineThreshold(RawDataList)) return false;

                if (GlobalCalc.IsAppliedToLegLevelData())
                {
                    RawDataList = BuildWhere.ApplyWhereRoute(RawDataList, true);
                }
                else
                {
                    var segmentData = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);
                    segmentData = BuildWhere.ApplyWhereRoute(segmentData, false);
                    RawDataList = GetLegDataFromFilteredSegData(RawDataList, segmentData);
                }

                RawDataList = RawDataList.GroupBy(s => s.RecKey, (key, recs) =>
                {
                    var firstRec = recs.FirstOrDefault() ?? new RawData();

                    return new RawData
                    {
                        RecKey = key,
                        GrpCol = firstRec.GrpCol,
                        AirChg = firstRec.AirChg,
                        OffrdChg = firstRec.OffrdChg,
                        Plusmin = firstRec.Plusmin,
                        NoHotel = 1
                    };
                }).ToList();
            }
            else
            {
                var airSql = SqlBuilder.GetSqlNoRouting(udidNumber > 0, GlobalCalc.IsReservationReport(), BuildWhere, _groupColumn);
                RawDataList = RetrieveRawData<RawData>(airSql, GlobalCalc.IsReservationReport(), false).ToList();
            }

            // remove duplicate records
            var rwDataListGroup = RawDataList.GroupBy(g => new { g.RecKey, g.AirChg, g.OffrdChg, g.DitCode, g.DomIntl, g.Miles, g.Origin, g.Destinat, g.ActFare, g.MiscAmt, g.Connect, g.Mode, g.DepTime, g.ArrTime, g.Airline, g.fltno, g.ClassCode, g.Seg_Cntr, g.SeqNo, g.RDepDate, g.RArrDate, g.BookDate, g.InvDate, g.AirCurrTyp, g.GrpCol, g.NoHotel, g.Plusmin, g.Class, g.Classcat, g.Farebase, g.Seat, g.Stops, g.Segstatus, g.Tktdesig, g.Rplusmin, g.OrigOrigin, g.OrigDest, g.OrigCarr, g.AirCo2 });
            RawDataList = rwDataListGroup.Select(g => g.First()).ToList();

   //         if (!DataExists(RawDataList)) return false;
            PerformCurrencyConversion(RawDataList);

            var carSql = SqlBuilder.GetSqlCar(udidNumber > 0, GlobalCalc.IsReservationReport(), BuildWhere, _groupColumn);
            CarRawDataList = RetrieveRawData<CarRawData>(carSql, GlobalCalc.IsReservationReport(), false).ToList();
            // remove duplicate records
            var carRawDataistGroup = CarRawDataList.GroupBy(g => new { g.RecKey, g.GrpCol, g.ABookRat, g.Days, g.CPlusMin, g.CarExchangeDate, g.BookDate, g.InvDate, g.CarCurrTyp });
            CarRawDataList = carRawDataistGroup.Select(g => g.First()).ToList();

            PerformCurrencyConversion(CarRawDataList);

            var hotelSql = SqlBuilder.GetSqlHotel(udidNumber > 0, GlobalCalc.IsReservationReport(), BuildWhere, _groupColumn);
            HotelRawDataList = RetrieveRawData<HotelRawData>(hotelSql, GlobalCalc.IsReservationReport(), false).ToList();
            // remove duplicate records
            var hotelRawDataistGroup = HotelRawDataList.GroupBy(g => new { g.RecKey, g.GrpCol, g.BookRate, g.Rooms, g.Nights, g.HPlusMin, g.HotelExchangeDate, g.BookDate, g.InvDate, g.HotCurrTyp });
            HotelRawDataList = hotelRawDataistGroup.Select(g => g.First()).ToList();

            PerformCurrencyConversion(HotelRawDataList);

            return true;
        }

        public override bool ProcessData()
        {
            var topBottomCostCenterFinalData = new TopBottomCostCenterFinalData();
            FinalDataList = topBottomCostCenterFinalData.GetFinalList(RawDataList, BuildWhere, HotelRawDataList, CarRawDataList, Globals);

            topBottomCostCenterFinalData.CalculateAndUpdateTotalParameterValues(ref TotChg, ref TotLost, ref TotCnt, ref TotCnt2, ref TotCnt3, ref TotStays,
            ref TotNites, ref TotHotCost, ref TotRents, ref TotDays, ref TotCarCost, ref TotCost, FinalDataList);

            FinalDataList = TopBottomCostCenterHelpers.SortData(FinalDataList, Globals);
            if (!DataExists(FinalDataList)) return false;

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    //var UserBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);
                    var exportFieldList = new TopBottomCostCenterData().GetExportFields(_groupColumn);
                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                        ExportHelper.ConvertToCsv(FinalDataList, exportFieldList, Globals);
                    else
                        ExportHelper.ListToXlsx(FinalDataList, exportFieldList, Globals);
                    break;
                default:

                    var outputType = Globals.GetParmValue(WhereCriteria.OUTPUTTYPE);
                    if ("4,6,TG,XG".Contains(outputType)) return GenerateGraph();

                    CreatePdf();
                    break;
            }
            return true;
        }

        private void CreatePdf()
        {
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(FinalDataList);

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

            ReportSource.SetParameterValue("nTotCost", TotCost);
            ReportSource.SetParameterValue("nTotDays", TotDays);
            ReportSource.SetParameterValue("nTotRents", TotRents);
            ReportSource.SetParameterValue("nTotHotCost", TotHotCost);
            ReportSource.SetParameterValue("nTotCarCost", TotCarCost);
            ReportSource.SetParameterValue("nTotCnt", TotCnt);
            ReportSource.SetParameterValue("nTotCnt2", TotCnt2);
            ReportSource.SetParameterValue("nTotChg", TotChg);
            ReportSource.SetParameterValue("nTotLost", TotLost);
            ReportSource.SetParameterValue("nTotStays", TotStays);
            ReportSource.SetParameterValue("nTotNites", TotNites);
            ReportSource.SetParameterValue("cBrkColHdr", TopBottomCostCenterHelpers.GetBreakName(Globals));

            CrystalFunctions.CreatePdf(ReportSource, Globals);
        }

        private bool GenerateGraph()
        {
            var sortBy = Globals.GetParmValue(WhereCriteria.SORTBY);
            var graphTitle = "Volume Booked";
            switch (sortBy)
            {
                case "1":
                    graphTitle = "Total Spend Volume";
                    break;
                case "2":
                    graphTitle = "# of Trips";
                    break;
            }

            var graphData = FinalDataList.Select(s => new Graph1FinalData
            {
                CatDesc = s.GrpCol,
                Data1 = sortBy.Equals("2") ? s.Numtrips : s.Totalcost
            }).ToList();

            CrystalReportName = "ibGraph1";

            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(graphData);

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
            ReportSource.SetParameterValue("cGrDataType", "2,3".Contains(sortBy) ? "N" : "C");
            ReportSource.SetParameterValue("cCatDesc", _groupColumn);
            ReportSource.SetParameterValue("cSubTitle", graphTitle);
            ReportSource.SetParameterValue("cGrColHdr1", Globals.BeginDate.Value.ToShortDateString() + " - " + Globals.EndDate.Value.ToShortDateString());
           
            CrystalFunctions.CreatePdf(ReportSource, Globals);
            return true;
        }
    }
}
