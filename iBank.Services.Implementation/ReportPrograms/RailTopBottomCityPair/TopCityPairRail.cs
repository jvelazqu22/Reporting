using System;
using System.Linq;
using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.TopBottomCityPairRail;
using Domain.Orm.iBankClientQueries;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.RailTopBottomCityPair
{
    public class TopCityPairRail : ReportRunner<RawData, FinalData>
    {

        private int _totCnt;
        private bool _excludeMileage;
        public TopCityPairRail()
        {
            _totCnt = 0;
            _excludeMileage = false;
            CrystalReportName = "ibtopcitypairrail";
        }

        public override bool InitialChecks()
        {
            if (!IsGoodCombo()) return false;
            if (!IsDateRangeValid()) return false;
            if (!IsUdidNumberSuppliedWithUdidText()) return false;
            if (!IsOnlineReport()) return false;
            
            //if ((Globals.EndDate.Value - Globals.BeginDate.Value).Days > 193)
            //{
            //    Globals.ReportInformation.ReturnCode = 2;
            //    Globals.ReportInformation.ErrorMessage = "Due to the large volume of data selected for this report, it must be run offline.";
            //    return false;
            //}

            return true;
        }

        public override bool GetRawData()
        {
            var oneWayBothWays = Globals.GetParmValue(WhereCriteria.RBFLTMKTONEWAYBOTHWAYS);
            
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);
            BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true,buildDateWhere: true, inMemory: true, isRoutingBidirectional: oneWayBothWays.Equals("1"), legDit: false, ignoreTravel: false);

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            var hasUdid = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1) > 0;
            var sqlScript = SqlBuilder.GetSql(hasUdid, GlobalCalc.IsReservationReport(), BuildWhere.WhereClauseFull);

            RawDataList = RetrieveRawData<RawData>(sqlScript, GlobalCalc.IsReservationReport()).ToList();
            RawDataList = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);

            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            PerformCurrencyConversion(RawDataList);
            return true;
        }

        public override bool ProcessData()
        {
            var useTicketCount = Globals.IsParmValueOn(WhereCriteria.CBCOUNTTKTSNOTSEGS);

            FinalDataList = new TopCityPairRailProcessDataHandler().GetFinalData(Globals, RawDataList, BuildWhere, GlobalCalc, MasterStore, useTicketCount);
            
            var sortBy = Globals.GetParmValue(WhereCriteria.SORTBY);

            var sortDescending = sortBy != "4" && Globals.ParmValueEquals(WhereCriteria.RBSORTDESCASC, "1");

            if (!DataExists(FinalDataList)) return false;

            _totCnt = useTicketCount
                ? (int) Math.Round(FinalDataList.Sum(s => s.Cpnumticks))
                : FinalDataList.Sum(s => s.Cpsegs);

            _excludeMileage = Globals.IsParmValueOn(WhereCriteria.CBEXCLUDEMILEAGE);

            TopCityPairRailHelpers.XlsCalculations(FinalDataList, Globals, _totCnt, _excludeMileage);
           
            FinalDataList = TopCityPairRailHelpers.SortFinalData(FinalDataList,sortBy, sortDescending, useTicketCount);

            var howMany = sortBy.Equals("4")? 0 : Globals.GetParmValue(WhereCriteria.HOWMANY).TryIntParse(0);

            if (howMany > 0) FinalDataList = FinalDataList.Take(howMany).ToList();

            if (_excludeMileage) CrystalReportName += "2";

            if (useTicketCount) CrystalReportName += "A";

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, TopCityPairRailHelpers.GetExportFields(_excludeMileage), Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, TopCityPairRailHelpers.GetExportFields(_excludeMileage), Globals);
                    }
                    break;
                default:
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
            var subTitle = Globals.ParmValueEquals(WhereCriteria.RBFLTMKTONEWAYBOTHWAYS, "1")
                ? "City Pairs Represent Trips in Both Directions"
                : "City Pairs Represent Trips in Each Direction Separately";

            ReportSource.SetParameterValue("nTotCnt", _totCnt);
            ReportSource.SetParameterValue("cSubTitle", subTitle);

            CrystalFunctions.CreatePdf(ReportSource, Globals);
        }
    }
}
