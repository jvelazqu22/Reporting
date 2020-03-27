using System.Linq;
using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.AgentSummary;
using Domain.Orm.iBankClientQueries;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.AgentSummary
{
    public class AgentSummary : ReportRunner<RawData, FinalData>
    {
        public AgentSummary()
        {
            CrystalReportName = "ibAgentSummary";
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
            Globals.SetParmValue(WhereCriteria.PREPOST, "2");
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            RawDataList = RetrieveRawData<RawData>(SqlBuilder.GetSql(Globals, BuildWhere.WhereClauseFull),GlobalCalc.IsReservationReport(), false).ToList();
            RawDataList = PerformCurrencyConversion(RawDataList);

            return true;
        }

        public override bool ProcessData()
        {
            FinalDataList = new AgentSummaryProcessor().GetFinalDataList(RawDataList, Globals);

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
                        ExportHelper.ConvertToCsv(FinalDataList, Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, Globals);
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
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(FinalDataList);

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
            ReportSource.SetParameterValue("nTotChg", FinalDataList.Sum(s => s.Netvolume));
            ReportSource.SetParameterValue("xAgentID", "Agent ID");
            ReportSource.SetParameterValue("xNumOfTrans1", "# of");
            ReportSource.SetParameterValue("xNumOfTrans2", "Transactions");
            ReportSource.SetParameterValue("xNumOfTikts1", "# of");
            ReportSource.SetParameterValue("xNumOfTikts2", "Tickets");
            ReportSource.SetParameterValue("xNumgOfRfunds1", "# of");
            ReportSource.SetParameterValue("xNumgOfRfunds2", "Refunds");
            ReportSource.SetParameterValue("xNumOrNetTrips1", "Net # of");
            ReportSource.SetParameterValue("xNumOrNetTrips2", "Trips");
            ReportSource.SetParameterValue("xTotCommissions1", "Total");
            ReportSource.SetParameterValue("xTotCommissions2", "Commission");
            ReportSource.SetParameterValue("xAvgComRatePerTrip1", "Ave Commission");
            ReportSource.SetParameterValue("xAvgComRatePerTrip2", "Rate per Trip");
            ReportSource.SetParameterValue("xInvoiceAmt", "Invoice Amt");
            ReportSource.SetParameterValue("xCreditAmt", "Credit Amt");
            ReportSource.SetParameterValue("xNetVol", "Net Volume");
            ReportSource.SetParameterValue("xPerctOfTotal", "% of Total");
            CrystalFunctions.CreatePdf(ReportSource, Globals);
        }
    }
}
