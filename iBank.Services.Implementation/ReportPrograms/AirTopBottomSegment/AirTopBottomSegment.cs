using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models.ReportPrograms.Graphs;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using System.Linq;
using Domain.Models.ReportPrograms.AirTopBottomSegment;
using Domain.Orm.iBankClientQueries;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.AirTopBottomSegment
{
    public class AirTopBottomSegment : ReportRunner<RawData, FinalData>
    {
        private string _sortBy;
        private UserBreaks _userBreaks { get; set; }
        private AirTopBottomData _data = new AirTopBottomData();
        private bool _accountBreak { get; set; }
        int _totalTrips;
        decimal _totalCharges;


        public AirTopBottomSegment()
        {
            CrystalReportName = "ibTopSegCarr";
        }

        public override bool InitialChecks()
        {
            _sortBy = Globals.GetParmValue(WhereCriteria.SORTBY);
            if (!IsGoodCombo()) return false;

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true, isRoutingBidirectional: false, legDit: true, ignoreTravel: false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            int udid = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(0);
            var isReservation = Globals.ParmValueEquals(WhereCriteria.PREPOST, "1");

            var sqlCreator = new AirTopBottomSegmentSqlCreator();
            var sql = sqlCreator.CreateScript(BuildWhere.WhereClauseFull, udid, isReservation, BuildWhere.WhereClauseUdid);
            RawDataList = RetrieveRawData<RawData>(sql, isReservation, true).ToList();

            if (!DataExists(RawDataList)) return false;

            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            RawDataList = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);

            RawDataList = GlobalCalc.IsAppliedToLegLevelData() ? BuildWhere.ApplyWhereRoute(RawDataList,true) : BuildWhere.ApplyWhereRoute(RawDataList, false);

            if (!DataExists(RawDataList)) return false;

            PerformCurrencyConversion(RawDataList);

            return true;
        }

        public override bool ProcessData()
        {
            _userBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);
            _accountBreak = Globals.User.AccountBreak;

            var calculator = new AirTopBottomSegmentCalculations();
            FinalDataList = calculator.GetInitializeFinalDataFromRawData(RawDataList, MasterStore);
            FinalDataList = calculator.GetCalculatedValues(FinalDataList);
            calculator.UpdateTotalTripsAndTotalCharges(FinalDataList, ref _totalTrips, ref _totalCharges);

            if (!DataExists(FinalDataList)) return false;

            FinalDataList = new AirTopBottomData().SortList(FinalDataList, _sortBy, Globals.GetParmValue(WhereCriteria.RBSORTDESCASC), Globals.GetParmValue(WhereCriteria.HOWMANY));

            return true;
        }

        public override bool GenerateReport()
        {
            var outputType = Globals.GetParmValue(WhereCriteria.OUTPUTTYPE);
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = _data.GetExportFields();

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                        ExportHelper.ConvertToCsv(FinalDataList, exportFields, Globals);
                    else
                        ExportHelper.ListToXlsx(FinalDataList, exportFields, Globals);
                    break;
                default:
                    if ("4,6,RG,XG".Contains(outputType))
                    {
                        return GenerateGraph();
                    }

                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);

                    ReportSource.SetParameterValue("nTotCnt", _totalTrips);
                    ReportSource.SetParameterValue("nTotChg", _totalCharges);

                    var missing = CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }

        private bool GenerateGraph()
        {
            string graphTitle;
            switch (_sortBy)
            {
                case "1":
                    graphTitle = "Volume Booked";
                    break;
                case "2":
                    graphTitle = "Avg Cost per Segment";
                    break;
                case "3":
                    graphTitle = "# of Segments";
                    break;
                case "4":
                    graphTitle = "Commissions";
                    break;
                default:
                    graphTitle = "Volume Booked";
                    break;
            }

            var graphData = FinalDataList.Select(s => new Graph1FinalData
            {
                CatDesc = s.Carrdesc.Left(30),
                Data1 = AirTopBottomData.GetGraphData1(s, _sortBy)
            }).ToList();

            CrystalReportName = "ibGraph1";

            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(graphData);

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
            ReportSource.SetParameterValue("cGrDataType", _sortBy.Equals("3") ? "N" : "C");
            ReportSource.SetParameterValue("cCatDesc", "Carrier");
            ReportSource.SetParameterValue("cSubTitle", graphTitle);
            ReportSource.SetParameterValue("cGrColHdr1", Globals.BeginDate.Value.ToShortDateString() + " - " + Globals.EndDate.Value.ToShortDateString());

            CrystalFunctions.CreatePdf(ReportSource, Globals);
            return true;
        }
    }
}
