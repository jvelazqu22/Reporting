using System;
using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.AgentAirActivity;
using Domain.Orm.iBankClientQueries;

using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.AgentAirActivity
{
    public class AgentAirActivity : ReportRunner<RawData, FinalData>
    {
        private List<SvcFeeData> _svcFeeDataList;

        public AgentAirActivity()
        {
            CrystalReportName = "ibAgentAirActivity";
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

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false,
                buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true, isRoutingBidirectional: false,
                legDit: false, ignoreTravel: false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            var rawDataSql = SqlBuilder.GetSql(Globals, GlobalCalc.IsReservationReport(), BuildWhere.WhereClauseFull);
            RawDataList = RetrieveRawData<RawData>(rawDataSql, GlobalCalc.IsReservationReport()).ToList();

            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            if (Globals.AgencyInformation.UseServiceFees)
            {
                var svcFeeSql = SqlBuilder.GetSqlSvcFees(Globals, BuildWhere.WhereClauseFull, BuildWhere.WhereClauseServices);
                _svcFeeDataList = RetrieveRawData<SvcFeeData>(svcFeeSql, GlobalCalc.IsReservationReport(), false).ToList();

                _svcFeeDataList = _svcFeeDataList.GroupBy(s => s.RecKey, (key, recs) => new SvcFeeData
                {
                    RecKey = key,
                    SvcFee = recs.Sum(s => s.SvcFee)
                }).ToList();

                foreach (var row in RawDataList)
                {
                    var svcFee = _svcFeeDataList.FirstOrDefault(s => s.RecKey == row.RecKey);
                    if (svcFee != null)
                    {
                        row.Svcfee = svcFee.SvcFee;
                    }
                }
            }
            RawDataList = PerformCurrencyConversion(RawDataList);
            if (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstOrigin || BuildWhere.HasFirstDestination)
            {
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
            }
            else
            {
                //Filter by mode if no routing criteria
                if (!Globals.GetParmValue(WhereCriteria.MODE).IsNullOrWhiteSpace())
                {
                    var mode = (Mode)(Globals.GetParmValue(WhereCriteria.MODE).TryIntParse(0));

                    if (mode == Mode.RAIL)
                    {
                        Globals.WhereText = string.IsNullOrEmpty(Globals.WhereText) ? "Rail Only;" : $"{Globals.WhereText} Rail Only";
                        RawDataList = RawDataList.Where(x => x.Mode.EqualsIgnoreCase("R")).ToList();
                    }
                    else if (mode == Mode.AIR)
                    {
                        Globals.WhereText = string.IsNullOrEmpty(Globals.WhereText) ? "Air Only;" : $"{Globals.WhereText} Air Only";
                        RawDataList = RawDataList.Where(x => x.Mode.EqualsIgnoreCase("A")).ToList();
                    }
                }
            }
            return true;
        }

        public override bool ProcessData()
        {
            FinalDataList =
                RawDataList.OrderBy(s => s.Agentid)
                    .ThenBy(s => s.Passlast)
                    .ThenBy(s => s.Passfrst)
                    .ThenBy(s => s.RecKey)
                    .ThenBy(s => s.SeqNo)
                    .Select(s => 
                    {
                        var newRow = new FinalData();
                        Mapper.Map(s, newRow);
                        if (s.RDepDate != null) newRow.Rdepdate = (DateTime) s.RDepDate;
                        newRow.Seqno = s.SeqNo;
                        newRow.Passfrst = s.Passfrst;
                        newRow.Passlast = s.Passlast;
                        newRow.Orgdesc = AportLookup.LookupAport(MasterStore, s.Origin, s.Mode, Globals.Agency);
                        newRow.Destdesc = AportLookup.LookupAport(MasterStore, s.Destinat, s.Mode, Globals.Agency);
                        newRow.Airline = !string.IsNullOrEmpty(s.OrigCarr.Trim()) ? s.OrigCarr.Trim() : s.Airline.Trim();
                        return newRow;
                    }).ToList();

            FinalDataList = ZeroOut<FinalData>.Process(FinalDataList, new List<string> { "airchg", "svcfee", "acommisn" });
            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    FinalDataList = ZeroOut<FinalData>.Process(FinalDataList, new List<string> { "airchg", "svcfee" });
                    var exportFieldList = GetExportFields();

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

        private static List<string> GetExportFields()
        {
            return new List<string>
            {
                "agentid",
                "reckey",
                "invoice",
                "recloc",
                "ticket",
                "passlast",
                "passfrst",
                "cardnum",
                "seqno",
                "origin",
                "orgdesc",
                "destinat",
                "destdesc",
                "depdate",
                "airline",
                "fltno",
                "airchg",
                "svcfee",
                "acommisn"
            };
        }
    }
}
