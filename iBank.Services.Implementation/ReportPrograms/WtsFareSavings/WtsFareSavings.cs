using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.WtsFareSavings;
using Domain.Orm.iBankClientQueries;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.WtsFareSavings
{
    public class WtsFareSavings : ReportRunner<RawData, FinalData>
    {
        private List<UdidData> _udidData;
        public List<SummaryData> SummaryData;

        public override bool InitialChecks()
        {
            if (!IsGoodCombo()) return false;
            if (!IsDateRangeValid()) return false;
            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            //Globals.SetParmValue(WhereCriteria.PREPOST, "2");
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }
            BuildWhere.AddSecurityChecks();

            RawDataList = RetrieveRawData<RawData>(SqlBuilder.GetSql(GlobalCalc.IsReservationReport(), BuildWhere.WhereClauseFull), GlobalCalc.IsReservationReport(), false).ToList();
            RawDataList = PerformCurrencyConversion(RawDataList);
            
            _udidData = RetrieveRawData<UdidData>(SqlBuilder.GetSqlUdids(GlobalCalc.IsReservationReport(), BuildWhere.WhereClauseFull), GlobalCalc.IsReservationReport(), false).ToList();

            return true;
        }

        public override bool ProcessData()
        {
            var helper = new FinalDataProcessor(MasterStore, ClientStore, clientFunctions, Globals, RawDataList, _udidData);

            FinalDataList = helper.GetFinalData();

            SummaryData = helper.GetSummary();

            return true;
        }
        
        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    FinalDataList = ZeroOut<FinalData>.Process(FinalDataList, new List<string> {"airchg","stndchg","offrdchg","savings","lostamt"});
                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, WtsFareSavingsHelper.GetExportFields(Globals.User), Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, WtsFareSavingsHelper.GetExportFields(Globals.User), Globals);
                    }
                    break;
                default:
                    CreateReport();
                    break;
            }

            return true;
        }

        private void CreateReport()
        {
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + WtsFareSavingsHelper.GetReportName(Globals) + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            if (!Globals.IsParmValueOn(WhereCriteria.CBSUMPAGEONLY))
            {
                ReportSource.SetDataSource(FinalDataList);
                ReportSource.Subreports[0].SetDataSource(SummaryData);
            }
            else
            {
                ReportSource.SetDataSource(SummaryData);
            }

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

            CrystalFunctions.CreatePdf(ReportSource, Globals);
        }

    }
}
