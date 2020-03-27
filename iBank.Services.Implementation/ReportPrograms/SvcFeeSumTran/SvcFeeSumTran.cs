using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.ServiceFeeSummaryByTransactionReport;
using Domain.Orm.iBankClientQueries;
using iBank.Services.Implementation.Shared;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.SvcFeeSumTran
{
    public class SvcFeeSumTran : ReportRunner<RawData, FinalData>
    {
        private readonly SvcFeeSumTranCalculations _calc = new SvcFeeSumTranCalculations();
        private readonly SvcFeeSumTranDataProcessor _processor = new SvcFeeSumTranDataProcessor();
        private readonly SvcFeeSumTranSqlCreator _creator = new SvcFeeSumTranSqlCreator();

        public SvcFeeSumTran() {}

        public override bool InitialChecks()
        {
            if (!IsDateRangeValid()) return false;

            if (!IsOnlineReport()) return false;
            
            return true;
        }

        private void SetProperties()
        {
            CrystalReportName = _calc.GetCrystalReportName();

            Globals.SetParmValue(WhereCriteria.PREPOST, "2");
            Globals.SetParmValue(WhereCriteria.DATERANGE, "8");
        }

        public override bool GetRawData()
        {
            SetProperties();
            
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: false, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            var whereClause = _creator.GetReplacedWhereClause(BuildWhere.WhereClauseFull);
            var sql = _creator.Create(whereClause, Globals.UseHibServices);
            RawDataList = RetrieveRawData<RawData>(sql, false, false).ToList();

            if (!DataExists(RawDataList)) return false;

            //no offline threshold exists for this report

            PerformCurrencyConversion(RawDataList);

            return true;
        }

        public override bool ProcessData()
        {
            FinalDataList = _processor.MapRawToFinalData(RawDataList).ToList();

            if (!DataExists(FinalDataList)) return false;

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                    ExportHelper.ListToXlsx(FinalDataList, _calc.GetExportFields().ToList(), Globals);
                    break;
                case DestinationSwitch.Csv:
                    ExportHelper.ConvertToCsv(FinalDataList, _calc.GetExportFields().ToList(), Globals);
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
