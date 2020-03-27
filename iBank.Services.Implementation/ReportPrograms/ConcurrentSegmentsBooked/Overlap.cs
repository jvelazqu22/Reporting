using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.ConcurrentSegmentsBookedReport;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;
using System;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.ConcurrentSegmentsBooked
{
    public class Overlap : ReportRunner<RawData, FinalData>
    {
        public Overlap()
        {
            CrystalReportName = "ibOverlap";
        }

        public override bool InitialChecks()
        {
            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true, isRoutingBidirectional: false, legDit: false, ignoreTravel: true)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();
            
            var creator = new OverlapSqlCreator();
            var sql = creator.Create(BuildWhere.WhereClauseFull, GlobalCalc.GetUdidNumber());
            RawDataList = RetrieveRawData<RawData>(sql, true, false).ToList();

            if (!IsUnderOfflineThreshold(RawDataList)) return false;
            if (!DataExists(RawDataList)) return false;

            RawDataList = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);

            if (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstDestination || BuildWhere.HasFirstOrigin)
            {
                RawDataList = GlobalCalc.IsAppliedToLegLevelData() ? BuildWhere.ApplyWhereRoute(RawDataList, true) : BuildWhere.ApplyWhereRoute(RawDataList, false);
            }

            PerformCurrencyConversion(RawDataList);

            return true;
        }

        public override bool ProcessData()
        {
            FinalDataList = new OverlapFinalDataProcessor().GetFinalData(RawDataList, Globals, clientFunctions, MasterStore);

            if (!DataExists(FinalDataList)) return false;

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFieldList = new OverlapHelper().GetExportFields(Globals);

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, exportFieldList, Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, exportFieldList, Globals);
                    }
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
