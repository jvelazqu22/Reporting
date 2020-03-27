using System.Linq;
using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.ValidatingCarrierReport;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.ValidatingCarrier
{
    public class AgcyValCarr : ReportRunner<RawData, FinalData>
    {
        public AgcyValCarr()
        {
            CrystalReportName = "ibAgcyValCarr";
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
            BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false);

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
            var sqlScript = SqlBuilder.GetSql(hasUdid, BuildWhere.WhereClauseFull);

            RawDataList = RetrieveRawData<RawData>(sqlScript, GlobalCalc.IsReservationReport(), false).ToList();
            PerformCurrencyConversion(RawDataList);
            return true;
        }

        public override bool ProcessData()
        {
            foreach (var row in RawDataList)
            {
                if (!row.ValcarMode.EqualsIgnoreCase("R"))
                {
                    row.ValcarMode = "A";
                }
            }
            var allRrQuery = new GetAllRailroadOperatorsQuery(MasterStore.MastersQueryDb);
            var airportQuery = new GetAllAirlinesQuery(MasterStore.MastersQueryDb);
            
            var tempQuery = RawDataList.GroupBy(s => new {s.Valcarr, s.ValcarMode, s.Plusmin}, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new 
                {
                    key.Valcarr,
                    Carrdesc = LookupFunctions.LookupAline(MasterStore, key.Valcarr, key.ValcarMode),
                    key.ValcarMode,
                    key.Plusmin,
                    Transacts = reclist.Count,
                    Commission = reclist.Sum(s => s.Acommisn),
                    Airchg = reclist.Sum(s => s.Airchg)
                    
                };
            })
            .GroupBy(s => new {s.Valcarr,s.Carrdesc}, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new FinalData
                {
                    Valcarr = key.Valcarr,
                    Carrdesc = key.Carrdesc,
                    Transacts = reclist.Sum(s => s.Transacts),
                    Tickets = reclist.Sum(s => s.Plusmin == 1 ? s.Transacts:0),
                    Refunds = reclist.Sum(s => s.Plusmin == 1 ? 0 : s.Transacts),
                    Net_trips = reclist.Sum(s => s.Plusmin == 1 ? s.Transacts : 0-s.Transacts),
                    Commission = reclist.Sum(s => s.Commission),
                    Invoiceamt = reclist.Sum(s => s.Plusmin == 1? s.Airchg: 0),
                    Creditamt = reclist.Sum(s => s.Plusmin == 1 ? 0: 0 - s.Airchg),
                    Netvolume = reclist.Sum(s => s.Airchg)
                };
            });

            switch (Globals.GetParmValue(WhereCriteria.SORTBY))
            {
                //matches FoxPro sort (FoxPro uses grouped data by ValCarr, Carrdesc...)
                case "2":
                    FinalDataList = tempQuery.OrderByDescending(s => s.Net_trips).ThenBy(x => x.Valcarr).ThenBy(x => x.Carrdesc).ToList();
                    break;
                case "3":
                    FinalDataList = tempQuery.OrderByDescending(s => s.Netvolume).ThenBy(x => x.Valcarr).ThenBy(x=>x.Carrdesc).ToList();
                    break;
                default:
                    FinalDataList = tempQuery.OrderBy(s => s.Carrdesc).ToList();
                    break;
            }
            
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
