using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models.ReportPrograms.HotelActivity;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using System.Linq;
using com.ciswired.libraries.CISLogger;
using Domain;
using iBank.Services.Implementation.Utilities.ClientData;


namespace iBank.Services.Implementation.ReportPrograms.HotelActivity
{
    public class HotelActivity : ReportRunner<RawData, FinalData>
    {
        private static readonly com.ciswired.libraries.CISLogger.ILogger LOG = new LogManagerWrapper().GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public UserBreaks UserBreaks { get; set; }
        public bool AccountBreak { get; set; }
        HotelActivityCalculator _calculator = new HotelActivityCalculator();

        public HotelActivity()
        {
            CrystalReportName = "ibHotelActivity";
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
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: true, buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false)) return false;

            BuildWhere.BuildAdvancedClauses();
            BuildWhere.AddSecurityChecks();

            int udid = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(0);
            var isReservation = Globals.ParmValueEquals(WhereCriteria.PREPOST, "1");

            CrystalReportName = _calculator.GetCrystalReportNameAndSetIsReservationVariable(Globals.ProcessKey.ToString(), ref isReservation);

            var sql = new HotelActivitySqlCreator().CreateScript(BuildWhere, udid, isReservation);
            var retriever = new DataRetriever(ClientStore.ClientQueryDb);
            RawDataList = retriever.GetData<RawData>(sql, BuildWhere, false, udid > 0, isReservation, includeAllLegs: false, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();

            if (!DataExists(RawDataList)) return false;

            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            if (!DataExists(RawDataList)) return false;

            PerformCurrencyConversion(RawDataList);

            return true;
        }

        public override bool ProcessData()
        {
            UserBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            FinalDataList = _calculator.GetFinalDataFromRawData(RawDataList, Globals, UserBreaks, clientFunctions, getAllMasterAccountsQuery);
            FinalDataList = new HotelActivityData().SortFinalData(Globals.ProcessKey.ToString(), FinalDataList);

            if (!DataExists(FinalDataList)) return false;

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = new HotelActivityData().GetExportFields(Globals, UserBreaks);

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                        ExportHelper.ConvertToCsv(FinalDataList, exportFields, Globals);
                    else
                        ExportHelper.ListToXlsx(FinalDataList, exportFields, Globals);
                    break;
                default:
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }
            return true;
        }
    }
}
