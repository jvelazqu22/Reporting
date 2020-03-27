using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Constants;
using Domain.Helper;
using Domain.Models.ReportPrograms.TopBottomTravelersCar;
using Domain.Orm.Classes;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Shared;
using iBankDomain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomTravelerCar
{
    public class TopTravCar : ReportRunner<RawData, FinalData>
    {
        decimal _nTotRateFinal;
        decimal _nTotBookCntFinal;
        string _groupBy = string.Empty;
        IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery;
        IQuery<IList<MasterAccountInformation>> getAllParentAccountsQuery;

        public TopTravCar()
        {
            CrystalReportName = ReportNames.CAR_TOP_BOTTOM_TRAVELER_RPT;
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
            getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(new iBankClientQueryable(server, db), Globals.Agency);
            getAllParentAccountsQuery = new GetAllParentAccountsQuery(new iBankClientQueryable(server, db));

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: true, buildHotelWhere: false, buildUdidWhere: true,buildDateWhere: true,inMemory: false,isRoutingBidirectional: false,legDit: false,ignoreTravel: false)) return false;

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

            var sql = new TopBottomTravelerCarSqlCreator().CreateScript(BuildWhere.WhereClauseFull, udid, isReservation);
            RawDataList = RetrieveRawData<RawData>(sql, isReservation, false).ToList();

            if (!DataExists(RawDataList)) return false;

            // TODO: dbo.ibprocess.reclimit = -1 for processkey 52
            //if (!IsUnderOfflineThreshold(RawDataList)) return false;

            PerformCurrencyConversion(RawDataList);
            _groupBy = Globals.GetParmValue(WhereCriteria.GROUPBY);

            RawDataList = isReservation
                ? new CarTopBottomTravelerRawDataCalculator().GetSummaryReservationRawData(RawDataList)
                : new CarTopBottomTravelerRawDataCalculator().GetSummaryBakcOfficeRawData(RawDataList);

            return true;
        }

        public override bool ProcessData()
        {
            var calc = new CarTopBottomTravelerFinalDataCalculator();
            FinalDataList = calc.GetFinalDataFromRawData(RawDataList);
            _nTotRateFinal = calc.GetFinalListTotalRate(FinalDataList);
            _nTotBookCntFinal = calc.GetFinalListTotalBookCount(FinalDataList);
            FinalDataList = new CarTopBottomTravelerData().SortList(FinalDataList, Globals.GetParmValue(WhereCriteria.SORTBY), Globals.GetParmValue(WhereCriteria.RBSORTDESCASC), Globals.GetParmValue(WhereCriteria.HOWMANY));
            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = new CarTopBottomTravelerData().GetExportFields();

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

                    var rawDatacalculator = new CarTopBottomTravelerRawDataCalculator();

                    var nTotCnt = rawDatacalculator.GetRawListTotalRentals(RawDataList);
                    var nTotDays = rawDatacalculator.GetRawListTotalDays(RawDataList);
                    var nTotCost = rawDatacalculator.GetRawListTotalCarCosts(RawDataList);
                    ReportSource.SetParameterValue("nTotCnt", nTotCnt);
                    ReportSource.SetParameterValue("nTotDays", nTotDays);
                    ReportSource.SetParameterValue("nTotCost", nTotCost);
                    ReportSource.SetParameterValue("nTotRate", _nTotRateFinal);
                    ReportSource.SetParameterValue("nTotBookCnt", _nTotBookCntFinal);

                    var missing = CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }
    }
}
