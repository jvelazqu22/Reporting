using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models.ReportPrograms.AirActivityReport;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using iBank.Services.Implementation.Shared.CarbonCalculations;
using iBank.Services.Implementation.Shared.SpecifyUdid;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.AirActivity
{

    public class CustAct2 : ReportRunner<RawData, FinalData>
    {
        private static readonly com.ciswired.libraries.CISLogger.ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private bool ExcludeServiceFees { get; set; }
        private bool IsReservationReport { get; set; }
        private bool IsCarbonReportingReport { get; set; }
        private string CarbonCalculator { get; set; }
        private bool IsAlternateEmissionsCarbonReportingReport { get; set; }
        private bool IsDateSort { get; set; }
        private int SortBy { get; set; }
        private string FlightSegments { get; set; }
        private bool ApplyToSegment { get; set; }
        private bool UseMetric { get; set; }
        private UserBreaks UserBreaks { get; set; }
        private string UnitOfMeasurement { get; set; }
        private List<int> UdidNumber { get; set; }
        private List<string> UdidLabel { get; set; }

        private readonly AirActivityCalculations _calc = new AirActivityCalculations();

        private readonly AirActivityDataProcessor _processor;

        private readonly ReportChecker _checker = new ReportChecker();
        
        public CustAct2()
        {
             _processor = new AirActivityDataProcessor(clientFunctions);
        }

        public override bool InitialChecks()
        {
            if (!IsGoodCombo()) return false;

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            SetPropertiesFromWhereCriteria();

            CrystalReportName = _calc.GetCrystalReportName(IsReservationReport, IsCarbonReportingReport, IsAlternateEmissionsCarbonReportingReport);

            return true;
        }
        
        public override bool GetRawData()
        {
            if (!string.IsNullOrEmpty(FlightSegments)) ClearOutWhereCriteria();
        
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            //Note that we are not building the Route clause here. We'll need the in-memory version later 
            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true,isRoutingBidirectional: false, legDit: false, ignoreTravel: true)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            var udidNumber = Globals.GetParmValue(WhereCriteria.UDIDNBR);
            var udid = _calc.GetUdid(udidNumber);
            var sqlCreator = new AirActivityRawDataSqlCreator();
            var sql = sqlCreator.CreateScript(BuildWhere.WhereClauseFull, udid, IsReservationReport, BuildWhere.WhereClauseUdid, BuildWhere);
            
            RawDataList = RetrieveRawData<RawData>(sql: sql, isReservationReport: IsReservationReport, addFieldsFromLegsTable: true, includeAllLegs: true).ToList();

            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold(RawDataList)) return false;
            
            if (ApplyToSegment)
            {
                //collapse to seg level 
                var segData = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);
                if (!string.IsNullOrEmpty(FlightSegments)) segData = new FlightSegmentFilter<RawData>().FilterOnFlightSegment(segData, FlightSegments, GlobalCalc.IsBothWays()).ToList();

                if (BuildWhere.HasFirstOrigin || BuildWhere.HasFirstDestination || BuildWhere.HasRoutingCriteria) segData = BuildWhere.ApplyWhereRoute(segData, false, returnAllLegs: false);

                RawDataList = GetLegDataFromFilteredSegData(RawDataList, segData);
            }
            else
            {

                if (BuildWhere.HasFirstOrigin || BuildWhere.HasFirstDestination || BuildWhere.HasRoutingCriteria) RawDataList = BuildWhere.ApplyWhereRoute(RawDataList, true, returnAllLegs: true);
            }

            if (!Globals.GetParmValue(WhereCriteria.MODE).IsNullOrWhiteSpace())
            {
                var helper = new ModeHelper(Globals, BuildWhere.Parameters, BuildWhere.WhereClauseFull, IsReservationReport, sql.FromClause);
                RawDataList = helper.ApplyFilter(RawDataList);
                Globals.WhereText = helper.ModeText + " " + Globals.WhereText;
            }

            if (!string.IsNullOrEmpty(FlightSegments)) Globals.WhereText += _calc.GetFlightSegmentsWhereText(FlightSegments);

            var moneyType = Globals.GetParmValue(WhereCriteria.MONEYTYPE);
            PerformCurrencyConversion(RawDataList);
            
            CarbonCalculations();

            HandleServiceFees(moneyType, udid);

            return true;
        }
        
        public override bool ProcessData()
        {
            if (IsDateSort) Globals.SetParmValue(WhereCriteria.SORTBY, SortBy.ToString());
                  
            var dateRange = Globals.GetParmValue(WhereCriteria.DATERANGE);

            //udids
            var udids = new List<UdidRecord>();

            //udid number could be up to 10, we only need to retrieve udids is when at less one is configured
            if (UdidNumber.Where(x => x > 0).ToList().Count > 0)
            {
                var retriever = new UdidDataRetriever();
                udids = retriever.GetUdids(UdidNumber, BuildWhere.WhereClauseFull, Globals, BuildWhere, IsReservationReport).ToList();
            }

            FinalDataList = _processor.ConvertRawDataToFinalData(RawDataList, udids, UdidNumber, Globals, UserBreaks, IsDateSort, dateRange, ClientStore.ClientQueryDb, MasterStore).ToList();
            
            if (!DataExists(FinalDataList)) return false;

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFieldList = _processor.GetExportFields(ExcludeServiceFees, IsCarbonReportingReport,
                        IsAlternateEmissionsCarbonReportingReport, UseMetric, UserBreaks, Globals.User.AccountBreak, IsReservationReport, Globals.User, UdidNumber, UdidLabel).ToList();
                    
                    var zeroFields = _processor.GetZeroOutFields(ExcludeServiceFees).ToList();

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        FinalDataList = ZeroOut<FinalData>.Process(FinalDataList, zeroFields);
                        ExportHelper.ConvertToCsv(FinalDataList, exportFieldList, Globals);

                    }
                    else
                    {
                        FinalDataList = ZeroOut<FinalData>.Process(FinalDataList, zeroFields);
                        ExportHelper.ListToXlsx(FinalDataList, exportFieldList, Globals);
                    }
                    break;
                default:
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;
                    var totalAirCharge = _calc.GetTotalAirCharge(FinalDataList);

                    ReportSource = SetupPdfReportSource(rptFilePath, totalAirCharge, FinalDataList);
                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }
            
            return true;
        }

        private void SetPropertiesFromWhereCriteria()
        {
            ExcludeServiceFees = Globals.IsParmValueOn(WhereCriteria.CBEXCLUDESVCFEES);
            CarbonCalculator = Globals.GetParmValue(WhereCriteria.CARBONCALC);
            IsCarbonReportingReport = Globals.IsParmValueOn(WhereCriteria.CARBONEMISSIONS) && !string.IsNullOrEmpty(CarbonCalculator);

            if (!IsCarbonReportingReport) CarbonCalculator = "";

            IsAlternateEmissionsCarbonReportingReport = IsCarbonReportingReport && Globals.IsParmValueOn(WhereCriteria.ALTERNATEEMISSNS);
            IsReservationReport = Globals.ParmValueEquals(WhereCriteria.PREPOST, "1");
            IsDateSort = Globals.IsParmValueOn(WhereCriteria.CBINCLBREAKBYDATE);
            SortBy = Convert.ToInt32(Globals.GetParmValue(WhereCriteria.DATERANGE));
            FlightSegments = Globals.GetParmValue(WhereCriteria.TXTFLTSEGMENTS);
            
            ApplyToSegment = _checker.IsAppliedToSegment(Globals);

            UserBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);

            UseMetric = Globals.IsParmValueOn(WhereCriteria.METRIC);

            var udidHandler = new UdidHandler(Globals);
            udidHandler.SetUdidOnReportProperties();
            UdidNumber = udidHandler.UdidNo;
            UdidLabel = udidHandler.UdidLabel;
        }

        private void ClearOutWhereCriteria()
        {
            Globals.SetParmValue(WhereCriteria.ORIGIN, string.Empty);
            Globals.SetParmValue(WhereCriteria.INORGS, string.Empty);
            Globals.SetParmValue(WhereCriteria.DESTINAT, string.Empty);
            Globals.SetParmValue(WhereCriteria.INDESTS, string.Empty);
            Globals.SetParmValue(WhereCriteria.ORIGCOUNTRY, string.Empty);
            Globals.SetParmValue(WhereCriteria.INORIGCOUNTRY, string.Empty);
            Globals.SetParmValue(WhereCriteria.DESTCOUNTRY, string.Empty);
            Globals.SetParmValue(WhereCriteria.INDESTCOUNTRY, string.Empty);
            Globals.SetParmValue(WhereCriteria.INORIGREGION, string.Empty);
            Globals.SetParmValue(WhereCriteria.NOTINORIGREGION, string.Empty);
            Globals.SetParmValue(WhereCriteria.DESTREGION, string.Empty);
            Globals.SetParmValue(WhereCriteria.INDESTREGION, string.Empty);
            Globals.SetParmValue(WhereCriteria.METROORG, string.Empty);
            Globals.SetParmValue(WhereCriteria.INMETROORGS, string.Empty);
            Globals.SetParmValue(WhereCriteria.METRODEST, string.Empty);
            Globals.SetParmValue(WhereCriteria.INMETRODESTS, string.Empty);
        }

        private void HandleServiceFees(string moneyType, int udid)
        {
            if (!IsReservationReport)
            {
                //when we are getting service fee data we want to ensure we don't try currency conversion
                Globals.SetParmValue(WhereCriteria.MONEYTYPE, string.Empty);

                var svcFeeHandler = new ServiceFeeHandler(ClientStore.ClientQueryDb);
                var svcFees = svcFeeHandler.RetrieveServiceFeeData(Globals, BuildWhere, udid, IsReservationReport);

                //and now reinstate the original moneytype value
                Globals.SetParmValue(WhereCriteria.MONEYTYPE, moneyType);

                PerformCurrencyConversion(svcFees.ToList());

                RawDataList = svcFeeHandler.CombineServiceFeesWithRawData(svcFees, RawDataList).ToList();
            }
        }

        private void CarbonCalculations()
        {
            var carbonCalculator = Globals.GetParmValue(WhereCriteria.CARBONCALC);
            
            UnitOfMeasurement = _calc.GetUnitOfWeightType(UseMetric, IsCarbonReportingReport, carbonCalculator);
            
            if (IsCarbonReportingReport)
            {
                var useMileageTable = Globals.IsParmValueOn(WhereCriteria.MILEAGETABLE);

                if (useMileageTable) AirMileageCalculator<RawData>.CalculateAirMileageFromTable(RawDataList);

                RawDataList.ForEach(s => s.Miles = s.Plusmin * Math.Abs(s.Miles));

                var carbonCalc = new CarbonCalculator();
                carbonCalc.SetAirCarbon(RawDataList, UseMetric, carbonCalculator);

                if (UseMetric) MetricImperialConverter.ConvertMilesToKilometers(RawDataList);
            }
        }

        private ReportDocument SetupPdfReportSource(string rptFilePath, decimal totalAirCharge, IList<FinalData> dataSource)
        {
            var reportSource = new ReportDocument();

            reportSource.Load(rptFilePath);

            reportSource.SetDataSource(dataSource);

            CrystalFunctions.SetupCrystalReport(reportSource, Globals);
    
            reportSource.SetParameterValue("cUdidLbl1", Globals.GetParmValue(WhereCriteria.UDIDLBL1) != "" ? Globals.GetParmValue(WhereCriteria.UDIDLBL1) + ":" : "Udid # " + Globals.GetParmValue(WhereCriteria.UDIDONRPT1) + " text:");
            reportSource.SetParameterValue("cUdidLbl2", Globals.GetParmValue(WhereCriteria.UDIDLBL2) != "" ? Globals.GetParmValue(WhereCriteria.UDIDLBL2) + ":" : "Udid # " + Globals.GetParmValue(WhereCriteria.UDIDONRPT2) + " text:");
            reportSource.SetParameterValue("cUdidLbl3", Globals.GetParmValue(WhereCriteria.UDIDLBL3) != "" ? Globals.GetParmValue(WhereCriteria.UDIDLBL3) + ":" : "Udid # " + Globals.GetParmValue(WhereCriteria.UDIDONRPT3) + " text:");
            reportSource.SetParameterValue("cUdidLbl4", Globals.GetParmValue(WhereCriteria.UDIDLBL4) != "" ? Globals.GetParmValue(WhereCriteria.UDIDLBL4) + ":" : "Udid # " + Globals.GetParmValue(WhereCriteria.UDIDONRPT4) + " text:");
            reportSource.SetParameterValue("cUdidLbl5", Globals.GetParmValue(WhereCriteria.UDIDLBL5) != "" ? Globals.GetParmValue(WhereCriteria.UDIDLBL5) + ":" : "Udid # " + Globals.GetParmValue(WhereCriteria.UDIDONRPT5) + " text:");
            reportSource.SetParameterValue("cUdidLbl6", Globals.GetParmValue(WhereCriteria.UDIDLBL6) != "" ? Globals.GetParmValue(WhereCriteria.UDIDLBL6) + ":" : "Udid # " + Globals.GetParmValue(WhereCriteria.UDIDONRPT6) + " text:");
            reportSource.SetParameterValue("cUdidLbl7", Globals.GetParmValue(WhereCriteria.UDIDLBL7) != "" ? Globals.GetParmValue(WhereCriteria.UDIDLBL7) + ":" : "Udid # " + Globals.GetParmValue(WhereCriteria.UDIDONRPT7) + " text:");
            reportSource.SetParameterValue("cUdidLbl8", Globals.GetParmValue(WhereCriteria.UDIDLBL8) != "" ? Globals.GetParmValue(WhereCriteria.UDIDLBL8) + ":" : "Udid # " + Globals.GetParmValue(WhereCriteria.UDIDONRPT8) + " text:");
            reportSource.SetParameterValue("cUdidLbl9", Globals.GetParmValue(WhereCriteria.UDIDLBL9) != "" ? Globals.GetParmValue(WhereCriteria.UDIDLBL9) + ":" : "Udid # " + Globals.GetParmValue(WhereCriteria.UDIDONRPT9) + " text:");
            reportSource.SetParameterValue("cUdidLbl10", Globals.GetParmValue(WhereCriteria.UDIDLBL10) != "" ? Globals.GetParmValue(WhereCriteria.UDIDLBL10) + ":" : "Udid # " + Globals.GetParmValue(WhereCriteria.UDIDONRPT10) + " text:");

            var airActivityCalculations = new AirActivityCalculations();
            var totalVoidedTickets = airActivityCalculations.GetTotalVoidedTickets(FinalDataList);
            var totalVoidedTicketsValues = airActivityCalculations.GetTotalValuedOfVoidedTickets(FinalDataList);
            //only ibCustAct.rpt, ibCustActCarb1.rpt and ibCustActCarb2.rpt need these two calulated values
            if (CrystalReportName.Equals("ibCustAct") || CrystalReportName.Equals("ibCustActCarb1") || CrystalReportName.Equals("ibCustActCarb2"))
            {
                reportSource.SetParameterValue("TotalVoidedTickets", totalVoidedTickets);
                reportSource.SetParameterValue("TotalVoidedTicketValues", totalVoidedTicketsValues);
            }
            var columnHeader = _calc.GetColumnHeader(IsDateSort, SortBy, Globals.LanguageVariables);

            var includeVoids = Globals.IsParmValueOn(WhereCriteria.CBINCLVOIDS);
            var parameters = new ReportDocumentParameters(columnHeader, totalAirCharge, UnitOfMeasurement, includeVoids, ExcludeServiceFees);
            reportSource = _processor.SetParameterValues(reportSource, parameters, IsCarbonReportingReport, IsReservationReport);

            return reportSource;
        }
    }
}
