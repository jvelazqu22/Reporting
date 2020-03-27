using System;
using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.AgentProductivity;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.AgentProductivity
{
    public class AgentProductivity : ReportRunner<RawData,FinalData>
    {
        private List<Tuple<string,string>> _carrierList;
        private readonly AgentProductivityProcessor _agentProductivityProcessor = new AgentProductivityProcessor();

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

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: false,isRoutingBidirectional: false, legDit: false, ignoreTravel: false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            RawDataList =
                RetrieveRawData<RawData>(SqlBuilder.GetSql(Globals, GlobalCalc.IsReservationReport(), BuildWhere.WhereClauseFull),
                    GlobalCalc.IsReservationReport(), false).ToList();
            RawDataList = PerformCurrencyConversion(RawDataList);
            return true;
        }

        public override bool ProcessData()
        {
            var countTrips = Globals.ParmValueEquals(WhereCriteria.DDSHOWCOUNTSORFARE, "1");

            RawDataList = _agentProductivityProcessor.GetFilterRawData(RawDataList, Globals);

            var maxCarriers = Globals.OutputFormat == DestinationSwitch.Xls ? 14 : 8;
            _carrierList = new List<Tuple<string, string>>();

            _carrierList = _agentProductivityProcessor.GetCarrierList(RawDataList, MasterStore, countTrips, maxCarriers);
            _carrierList.Add(new Tuple<string, string>(string.Empty, "OTHER"));

            //make sure we have at least MAXCARRIER rows to avoid null errors
            for (int i = 1; i <= maxCarriers + 1; i++)
            {
                if (_carrierList.Count < i) _carrierList.Add(new Tuple<string, string>(string.Empty, string.Empty));
            }

            FinalDataList = _agentProductivityProcessor.GetFinalData(RawDataList, countTrips, _carrierList, Globals);

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFieldList = AgentProductivityHelpers.GetExportFields(_carrierList);

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
                    CreateReport();
                    break;
            }

            return true;
        }

        private void CreateReport()
        {
            CrystalReportName = Globals.ParmValueEquals(WhereCriteria.DDSHOWCOUNTSORFARE, "1") ? "ibAgentProd1" : "ibAgentProd2";
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(FinalDataList);

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
            //ReportSource.SetParameterValue("cCharGen1", AgentProductivityHelpers.GetAgentColumnHeader(Globals.GetParmValue(WhereCriteria.DDAGENTTYPE)));
            var carrCounter = 1;
            foreach (var carrier in _carrierList)
            {
                ReportSource.SetParameterValue("cCarrName" + carrCounter++, carrier.Item2);
            }
            if (CrystalReportName.EqualsIgnoreCase("ibAgentProd1"))
            {
                ReportSource.SetParameterValue("nTotCnt", FinalDataList.Sum(s => s.Tottrips));
            }
            else
            {
                ReportSource.SetParameterValue("nCarrTotal", "Total Carriers");
            }
            CrystalFunctions.CreatePdf(ReportSource, Globals);
        }
    }
}
