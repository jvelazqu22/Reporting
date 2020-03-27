using System.Linq;
using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Constants;
using Domain.Helper;
using Domain.Models.ReportPrograms.MarketReport;
using Domain.Orm.iBankClientQueries;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.Market
{
    /// <summary>
    ///     From iBankMasters.ibProces (ProcKey 10):
    ///     Breaks the city pairs down by legs or segments. Allows for comparison of up to
    ///     three different validating carriers with volume and number of trips.
    /// </summary>
    public class Market : ReportRunner<RawData, FinalData>
    {
        private readonly MarketCalculations _calc = new MarketCalculations();

        private readonly MarketSqlCreator _creator = new MarketSqlCreator();

        private readonly MarketDataProcessor _processor = new MarketDataProcessor();

        public Market()
        {
            UseAirportCodes = false;
            FltSegments = string.Empty;
            TreatMarketsAsOneWay = false;
            UseFltMkts = false;
            UseIndustryMileageTable = false;
            UserMode = string.Empty;
            UseSegFareMileage = false;
        }
        
        private bool UseAirportCodes { get; set; }

        private Carrier Carrier1 { get; set; }

        private Carrier Carrier2 { get; set; }

        private Carrier Carrier3 { get; set; }

        private string FltSegments { get; set; }

        private bool TreatMarketsAsOneWay { get; set; }
        private bool TreatMarketsAsBidirectional { get; set; }

        private bool UseFltMkts { get; set; }

        private bool UseIndustryMileageTable { get; set; }

        private string UserMode { get; set; }

        private bool UseSegFareMileage { get; set; }

        private int NumberCarriers { get; set; }

        public override bool InitialChecks()
        {
            SetProperties();

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            if (NumberCarriers == 0)
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage =
                    "You must provide at least 1 carrier for comparison to run the Market Share Analysis.";
                return false;
            }

            return true;
        }

        public override bool GetRawData()
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            var specRouting = TreatMarketsAsBidirectional; // This is mostly for clarity tracing code back to the original FoxPro
            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true, isRoutingBidirectional: specRouting, legDit: true, ignoreTravel: false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();
            
            var isReservationReport = GlobalCalc.IsReservationReport();
            var sql = _creator.Create(BuildWhere.WhereClauseFull, GlobalCalc.GetUdidNumber(), isReservationReport);

            RawDataList = RetrieveRawData<RawData>(sql, isReservationReport, true).ToList();
           
            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            if (!DataExists(RawDataList)) return false;

            PerformCurrencyConversion(RawDataList);

            if (UseIndustryMileageTable)
            {
                AirMileageCalculator<RawData>.CalculateAirMileageFromTable(RawDataList); 
            }

            RawDataList = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);

            if (UseSegFareMileage)
            {
                var groups = _processor.GroupRawData(RawDataList);
                RawDataList = _calc.AssignAccountFare(RawDataList, groups).ToList();
            }

            if (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstOrigin || BuildWhere.HasFirstDestination)
            {
                RawDataList = BuildWhere.ApplyWhereRoute(RawDataList, GlobalCalc.IsAppliedToLegLevelData(), false);
            }
             
            //If user selected to treat segments as bi-directional, meaning 
            //CHI -OHARE,IL - ABU DHABI,TC = ABU DHABI, TC - CHI-OHARE,IL, 
            //then swap the origin and destination so that they are in alphabetical order, if they aren't already. 
            //This partly for ordering and so that grouping will collapse segments into one record per segment.
            if (TreatMarketsAsBidirectional)
            {
                RawDataList = _processor.FlipOriginAndDestination(RawDataList).ToList();
            }

            return true;
        }

        private TotalSegsAndFares _totals;
        public override bool ProcessData()
        {
            //Need to group by segment ends regardless of direction. So, for example CHI-OHARE,IL - ABU DHABI,TC = ABU DHABI, TC - CHI-OHARE,IL
            FinalDataList = _processor.MapRawToFinalData(RawDataList, Carrier1, Carrier2, Carrier3, UseAirportCodes, UserMode, Globals).ToList();
            _totals = _calc.CalculateTotalSegmentsAndFares(FinalDataList);

            if (!string.IsNullOrEmpty(FltSegments))
            {
                FinalDataList = _processor.FilterFlightSegments(FinalDataList, FltSegments);
                SetWhereTextWithFlightMarkets();
            }

            FinalDataList = _processor.OrderFinalData(FinalDataList, _calc.GetSortBy(Globals), UseAirportCodes).ToList();

            if (!DataExists(FinalDataList)) return false;
            return true;
        }

        private void SetWhereTextWithFlightMarkets()
        {
            var fltWhereText = "Flight Markets ";
            if (Globals.IsParmValueOn(WhereCriteria.CBALLEXCEPTFLTSEGS))
            {
                fltWhereText += $" not {FltSegments}";
            }
            else
            {
                fltWhereText += $" = {FltSegments}";
            }

            Globals.WhereText = $"{fltWhereText}; {Globals.WhereText}";
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
                    SetReportParameters(ReportSource);

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }
            return true;
        }

        private void SetProperties()
        {
            UseAirportCodes = _calc.UseAirportCodes(Globals);

            //don't want to default mode to AirMode when it's blank, it means both A and R, US 8182 - Defect 00040034
            UserMode = Globals.GetParmValue(WhereCriteria.MODE);

            //use AirCode by default if for both A and R, 
            var mode = string.IsNullOrWhiteSpace(UserMode) || UserMode == "0" ?
                Constants.AirCode :
                UserMode;

            Carrier1 = _calc.GetCarrier(Globals, MarketCalculations.CarrierNumber.Carrier1, mode, MasterStore);
            Carrier2 = _calc.GetCarrier(Globals, MarketCalculations.CarrierNumber.Carrier2, mode, MasterStore);
            Carrier3 = _calc.GetCarrier(Globals, MarketCalculations.CarrierNumber.Carrier3, mode, MasterStore);

            NumberCarriers = _calc.GetNumberCarriers(Carrier1, Carrier2, Carrier3);

            CrystalReportName = _calc.GetCrystalReportName(NumberCarriers, UseAirportCodes);

            UseSegFareMileage = Globals.IsParmValueOn(WhereCriteria.SEGFAREMILEAGE);
            UseIndustryMileageTable = Globals.IsParmValueOn(WhereCriteria.MILEAGETABLE);
            UseAirportCodes = Globals.IsParmValueOn(WhereCriteria.CBUSEAIRPORTCODES);
            TreatMarketsAsOneWay = Globals.GetParmValue(WhereCriteria.RBFLTMKTONEWAYBOTHWAYS).Equals("2");
            TreatMarketsAsBidirectional = Globals.GetParmValue(WhereCriteria.RBFLTMKTONEWAYBOTHWAYS).Equals("1");
            FltSegments = Globals.GetParmValue(WhereCriteria.TXTFLTSEGMENTS);

            if (!string.IsNullOrEmpty(FltSegments))
            {
                UseFltMkts = true;
                Globals.SetParmValue(WhereCriteria.ORIGIN, "");
                Globals.SetParmValue(WhereCriteria.INORGS, "");
                Globals.SetParmValue(WhereCriteria.DESTINAT, "");
                Globals.SetParmValue(WhereCriteria.INDESTS, "");
            }
        }

        private void SetReportParameters(ReportDocument reportDocument)
        {
            reportDocument.SetParameterValue("lFltMkts", UseFltMkts);

            reportDocument.SetParameterValue("nTotSegs", _totals.TotalSegs);
            reportDocument.SetParameterValue("nTotFare", _totals.TotalFare);

            reportDocument.SetParameterValue("nTotC1Segs", _totals.TotalCarrier1Segs);
            reportDocument.SetParameterValue("nTotC1Fare", _totals.TotalCarrier1Fare);

            if (CrystalReportName == ReportNames.MARKET_RPT_1 || CrystalReportName == ReportNames.MARKET_RPT_1A)
            {
                reportDocument.SetParameterValue("cCarrDesc1", Carrier1.Description);
            }
            else
            {
                //parameter named differently in market2 and market3
                reportDocument.SetParameterValue("cCarr1Desc", Carrier1.Description);

                reportDocument.SetParameterValue("nTotC2Segs", _totals.TotalCarrier2Segs);
                reportDocument.SetParameterValue("nTotC2Fare", _totals.TotalCarrier2Fare);
                reportDocument.SetParameterValue("cCarr2Desc", Carrier2.Description);
            }

            if (CrystalReportName == ReportNames.MARKET_RPT_3 || CrystalReportName == ReportNames.MARKET_RPT_3A)
            {
                reportDocument.SetParameterValue("nTotC3Segs", _totals.TotalCarrier3Segs);
                reportDocument.SetParameterValue("nTotC3Fare", _totals.TotalCarrier3Fare);
                reportDocument.SetParameterValue("cCarr3Desc", Carrier3.Description);
            }

            reportDocument.SetParameterValue("cPageFoot",
                "* Flight Markets treated as " + (TreatMarketsAsOneWay ? "one-way" : "bi-directional"));
        }
    }
}
