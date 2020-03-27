using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.TopBottomExceptionReasonReport;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Shared;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomExceptionReason
{
    public class TopExceptions : ReportRunner<RawData, FinalData>
    {
        public int TotCount;
        public decimal TotLost;

        public TopExceptions()
        {
            CrystalReportName = "ibTopExceptions";
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

            //if we apply routing in both directions "SpecRouting" is true. 
            BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false,buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true,inMemory: false,isRoutingBidirectional: false,legDit: false,ignoreTravel: false);

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            var udidNumber = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1);
            var sql = SqlBuilder.GetSql(udidNumber > 0, GlobalCalc.IsReservationReport(), BuildWhere.WhereClauseFull,Globals);
            RawDataList = RetrieveRawData<RawData>(sql, GlobalCalc.IsReservationReport(), false).ToList();
            PerformCurrencyConversion(RawDataList);
            return true;
        }

        public override bool ProcessData()
        {
            FinalDataList = new TopBottomExceptionReasonFinalData().GetFinalData(RawDataList, Globals, clientFunctions, ClientStore, MasterStore);

            TotCount = FinalDataList.Sum(s => s.NumOccurs);
            TotLost = FinalDataList.Sum(s => s.LostAmt);

            FinalDataList = TopExceptionHelpers.SortData(FinalDataList, Globals);

            if (!DataExists(FinalDataList)) return false;

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = new TopBottomExceptionReasonData().GetExportFields();
                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, exportFields, Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, exportFields, Globals);
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

            ReportSource.SetParameterValue("nTotCnt", TotCount);
            ReportSource.SetParameterValue("nTotLost", TotLost);
            ReportSource.SetParameterValue("lLogGen1", Globals.IsParmValueOn(WhereCriteria.CBUSEBASEFARE));
            ReportSource.SetParameterValue("cCatDesc", Globals.ProcessKey == 60 ? "Exception Reason" : "Traveler");

            CrystalFunctions.CreatePdf(ReportSource, Globals);
        }
    }
}
