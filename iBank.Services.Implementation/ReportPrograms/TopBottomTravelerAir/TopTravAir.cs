using System.Linq;
using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Models.ReportPrograms.TopBottomTravelerAir;
using iBank.Services.Implementation.Utilities.ClientData;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomTravelerAir
{

    public class TopTravAir : ReportRunner<RawData, FinalData>
    {
        public int TotCount;
        public decimal TotCharge;
        public decimal TotBkDays;
        public decimal TotLost;

        private bool _useHomeCountry;

        private readonly TopTravAirDataProcessor _data = new TopTravAirDataProcessor();

        public TopTravAir() { }

        public override bool InitialChecks()
        {
            if (!IsGoodCombo()) return false;

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            _useHomeCountry = Globals.IsParmValueOn(WhereCriteria.CBDISLPAYHOMECTRY);

            CrystalReportName = TopTravAirHelpers.GetCrystalReportName(_useHomeCountry);

            return true;
        }

        public override bool GetRawData()
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            //if we apply routing in both directions "SpecRouting" is true. 
            BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false,buildHotelWhere: false,buildUdidWhere: true, buildDateWhere: true,inMemory: false,isRoutingBidirectional: false,legDit: false,ignoreTravel: false);

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

            var retriever = new DataRetriever(ClientStore.ClientQueryDb);
            //Build SQL, get raw dat
            var airSql = SqlBuilder.GetSql(udidNumber > 0, GlobalCalc.IsReservationReport(), BuildWhere);
            RawDataList = retriever.GetData<RawData>(airSql, BuildWhere, false, false, GlobalCalc.IsReservationReport(), includeAllLegs: false, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();

            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            //Process Routing Criteria
            if (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstDestination || BuildWhere.HasFirstOrigin)
            {

                if (GlobalCalc.IsAppliedToLegLevelData())
                {
                    RawDataList = BuildWhere.ApplyWhereRoute(RawDataList, true, false);
                }
                else
                {
                    var segmentData = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);
                    segmentData = BuildWhere.ApplyWhereRoute(segmentData, false, false);
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

            //Group raw data, ignores unnecessary fields
            RawDataList = RawDataList.GroupBy(s => new
            {
                s.RecKey,
                s.Passlast,
                s.Passfrst,
                s.Airchg,
                s.Offrdchg,
                s.Invdate,
                s.Bookdate,
                s.SourceAbbr,
                s.Depdate,
                s.Plusmin,
                s.AirCurrTyp
            }, (key, recs) => new RawData
            {
                RecKey = key.RecKey,
                Passlast = key.Passlast,
                Passfrst = key.Passfrst,
                Airchg = key.Airchg,
                Offrdchg = key.Offrdchg,
                Invdate = key.Invdate,
                Bookdate = key.Bookdate,
                SourceAbbr = key.SourceAbbr,
                Depdate = key.Depdate,
                Plusmin = key.Plusmin,
                AirCurrTyp = key.AirCurrTyp
            }).ToList();

            PerformCurrencyConversion(RawDataList);
            return true;
        }

        public override bool ProcessData()
        {
            FinalDataList = _data.ConvertRawDataToFinalData(RawDataList, Globals, MasterStore, _useHomeCountry, GlobalCalc.IsReservationReport()).ToList();

            //Calculate totals before trimming final data
            TotCount = FinalDataList.Sum(s => s.Trips);
            TotCharge = FinalDataList.Sum(s => s.Amt);
            TotBkDays = FinalDataList.Sum(s => s.Totbkdays);
            TotLost = FinalDataList.Sum(s => s.Lostamt);

            FinalDataList = TopTravAirHelpers.SortData(FinalDataList, Globals);

            //Check for data
            if (!DataExists(FinalDataList)) return false;

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFieldList = TopTravAirHelpers.GetExportFields(_useHomeCountry, Globals.IsParmValueOn(WhereCriteria.CBINCLUDELOSTSVGS));

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        if (Globals.OutputLanguage == null) Globals.OutputLanguage = Globals.UserLanguage;
                        ExportHelper.ConvertToCsv(FinalDataList, exportFieldList, Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, exportFieldList, Globals);
                    }
                    break;
                default:
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;
                    var ReportSource = _data.SetUpReportSource(rptFilePath, FinalDataList, TotCount, TotCharge, TotBkDays, TotLost, Globals.IsParmValueOn(WhereCriteria.CBINCLUDELOSTSVGS), SetReportTitle());
                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }
            return true;
        }

        //Changes the Report title
        private string SetReportTitle()
        {
            switch (Globals.GetParmValue(WhereCriteria.MODE))
            {
                case "0":
                    return Globals.ReportTitle = Globals.ReportTitle.Replace("Air", "Air/Rail");
                case Constants.RailMode:
                    return Globals.ReportTitle = Globals.ReportTitle.Replace("Air", "Rail");
                default:
                    return Globals.ReportTitle;
            }
        }

    }
}
