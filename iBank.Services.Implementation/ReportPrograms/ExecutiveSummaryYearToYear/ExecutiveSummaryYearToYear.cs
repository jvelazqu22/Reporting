using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;

using iBank.Server.Utilities;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Domain.Models.ReportPrograms.ExecutiveSummaryYearToYearReport;
using Domain.Orm.iBankClientQueries;

using iBank.Services.Implementation.Shared.CarbonCalculations;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryYearToYear
{
    //ibQview3.PRG in FoxPro
    public class ExecutiveSummaryYearToYear : ReportRunner<RawData, FinalData>
    {
        private readonly ExecutiveSummaryYearToYearCalculations _calc = new ExecutiveSummaryYearToYearCalculations();

        private readonly ExecutiveSummaryYearToYearSqlCreator _creator = new ExecutiveSummaryYearToYearSqlCreator();

        //current year raw data
        //private List<RawData> _airRawDataCy;
        private List<LegRawData> _legRawDataCurrYr;
        private List<CarRawData> _carRawDataCurrYr;
        private List<HotelRawData> _hotelRawDataCurrYr;
        private List<FeeRawData> _feeRawDataCurrYr;

        //prior year raw data
        private List<RawData> _airRawDataPriorYr;
        private List<LegRawData> _legRawDataPriorYr;
        private List<CarRawData> _carRawDataPriorYr;
        private List<HotelRawData> _hotelRawDataPriorYr;
        private List<FeeRawData> _feeRawDataPriorYr;

        private bool IsReservationReport => false;

        private bool ExcludeServiceFee { get; set; }
        private bool UseServiceFees { get; set; }
        private bool OrphanServiceFees { get; set; }
        private bool ExcludeSavings { get; set; }

        private bool CarbonReporting { get; set; }
        private bool SplitRail { get; set; }
        private string ReportOption { get; set; }
        private bool IsQuarterToQuarterOption { get; set; }
        

        private DateTime _beginDate;
        private DateTime _beginDate2;
        private DateTime _endDate;
        private DateTime _endDate2;
        private DateTime _begMonth;
        private DateTime _begMonth2;
        
        private bool UseMetric { get; set; }
        private string WeightMeasurement { get; set; }
        private string DistanceMeasurement { get; set; }
        private string StartMonthName { get; set; }
        private int StartMonthNumber { get; set; }
        private int StartYear { get; set; }
        private string FiscalMonthName { get; set; }
        private int FiscalYearStartMonthNumber { get; set; }

        public ExecutiveSummaryYearToYear()
        {
            _beginDate = DateTime.MinValue;
            _beginDate2 = DateTime.MinValue;
            _endDate = DateTime.MinValue;
            _endDate2 = DateTime.MinValue;

            _legRawDataCurrYr = new List<LegRawData>();
            _carRawDataCurrYr = new List<CarRawData>();
            _hotelRawDataCurrYr = new List<HotelRawData>();
            _feeRawDataCurrYr = new List<FeeRawData>();

            _airRawDataPriorYr = new List<RawData>();
            _legRawDataPriorYr = new List<LegRawData>();
            _carRawDataPriorYr = new List<CarRawData>();
            _hotelRawDataPriorYr = new List<HotelRawData>();
            _feeRawDataPriorYr = new List<FeeRawData>();
        }

        private void SetProperties()
        {
            Globals.SetParmValue(WhereCriteria.PREPOST, "2");
            ExcludeServiceFee = _calc.ExcludeServiceFee(IsReservationReport, Globals);
            UseServiceFees = _calc.UseServiceFees(ExcludeServiceFee, Globals);
            OrphanServiceFees = _calc.OrphanServiceFees(UseServiceFees, Globals);

            CarbonReporting = Globals.IsParmValueOn(WhereCriteria.CARBONEMISSIONS);
            ExcludeSavings = Globals.IsParmValueOn(WhereCriteria.CBEXCLUDESVGS);

            SplitRail = Globals.IsParmValueOn(WhereCriteria.CBSEPARATERAIL);
            ReportOption = _calc.GetReportOption(Globals);

            UseMetric = _calc.UseMetric(Globals);
            WeightMeasurement = _calc.GetWeightMeasurement(UseMetric);
            DistanceMeasurement = _calc.GetDistanceMeasurement(UseMetric);
            IsQuarterToQuarterOption = _calc.IsQuarterToQuarterOption(ReportOption);
            
            CrystalReportName = _calc.GetCrystalReportName(ReportOption);
        }

        public override bool InitialChecks()
        {
            SetProperties();

            if (!IsGoodCombo()) return false;

            StartMonthName = _calc.GetStartMonthName(Globals);
            StartMonthNumber = StartMonthName.MonthNumberFromName();
            StartYear = _calc.GetStartYear(Globals);
            FiscalMonthName = _calc.GetFiscalMonthName(Globals);
            FiscalYearStartMonthNumber = SharedProcedures.GetMonthNum(FiscalMonthName);
            if (FiscalYearStartMonthNumber < 1) FiscalYearStartMonthNumber = 1;

            if (!StartMonthNumber.IsBetween(1, 12) || !StartYear.IsBetween(1998, 2020))
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = "You need to specify a month and year.";
                return false;
            }

            if (!IsOnlineReport()) return false;

            if (IsQuarterToQuarterOption)
            {
                if ((FiscalYearStartMonthNumber == 1 && StartMonthNumber.IsBetween(11, 12)) ||
                    (FiscalYearStartMonthNumber == 2 && (StartMonthNumber == 1 || StartMonthNumber == 12)) ||
                    (StartMonthNumber == FiscalYearStartMonthNumber - 1 || StartMonthNumber == FiscalYearStartMonthNumber - 2))
                {
                    Globals.ReportInformation.ReturnCode = 2;
                    Globals.ReportInformation.ErrorMessage = "For the Qtr-to-Qtr option, your Start Mth cannot be 1 or 2 months prior to your Fiscal Start month.";
                    return false;
                }
            }

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!HasAccount()) return false;

            return true;
        }

        private void SetDates()
        {
            var dateCalc = new ExecSummaryYrToYrDateCalculator();
            _begMonth2 = dateCalc.GetBeginMonth2(StartYear, StartMonthNumber, IsQuarterToQuarterOption, FiscalYearStartMonthNumber);
            _endDate2 = dateCalc.GetEndDate2(_begMonth2, IsQuarterToQuarterOption);

            _beginDate2 = dateCalc.GetBeginDate2(FiscalYearStartMonthNumber, StartMonthNumber, StartYear);

            _begMonth = dateCalc.GetOneYearPrior(_begMonth2);
            _endDate = dateCalc.GetOneYearPrior(_endDate2);
            _beginDate = dateCalc.GetBeginDate(FiscalYearStartMonthNumber, StartMonthNumber, StartYear);

            Globals.BeginDate = _beginDate;
            Globals.EndDate = _endDate;
        }
        
        public override bool GetRawData()
        {
            SetDates();

            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);
            
            //previous year data
            BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: false,
                buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false);

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();
            
            var wherePriorYear = BuildWhere.WhereClauseFull;
            var useDate = _calc.GetUseDate(Globals);

            var udidNumber = GlobalCalc.GetUdidNumber();
            
            var priorYearAirSql = _creator.GetAirSql(useDate, udidNumber, wherePriorYear);
            _airRawDataPriorYr = RetrieveRawData<RawData>(priorYearAirSql, IsReservationReport, false).ToList();
            
            if (!IsUnderOfflineThreshold(_airRawDataPriorYr)) return false;
            PerformCurrencyConversion(_airRawDataPriorYr);

            GetPreviousYearData(useDate, udidNumber, wherePriorYear);
            
            //current year data
            var originalBeginDate = Globals.BeginDate;
            var originalEndDate = Globals.EndDate;

            Globals.BeginDate = _beginDate2;
            Globals.EndDate = _endDate2;
            
            BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: false,
                buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false);
            BuildWhere.AddAdvancedClauses();
            BuildWhere.AddSecurityChecks();
            
            Globals.BeginDate = originalBeginDate;
            Globals.EndDate = originalEndDate;

            var currentYearWhereClause = BuildWhere.WhereClauseFull;

            GetCurrentYearData(useDate, udidNumber, currentYearWhereClause);
            
            Globals.WhereText = _calc.GetFiscalYearWhereText(FiscalMonthName) + Globals.WhereText;
            if (OrphanServiceFees)
            {
                Globals.WhereText = string.IsNullOrEmpty(Globals.WhereText)
                                        ? _calc.GetOrphanServiceFeeWhereText()
                                        : Globals.WhereText + "; " + _calc.GetOrphanServiceFeeWhereText();
            }
            
            var calc = new ExecSummaryYrToYrDateCalculator();
            if (!calc.DataExists(RawDataList, _airRawDataPriorYr, _legRawDataCurrYr, _legRawDataPriorYr, _carRawDataCurrYr, _carRawDataPriorYr, _hotelRawDataCurrYr, _hotelRawDataPriorYr))
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.RptMsg_NoData;
                return false;
            }

            return true;
        }

        private void GetPreviousYearData(string useDate, int udidNumber, string wherePriorYear)
        {
            var previousYearDataTasks = new List<Task>();

            //leg
            previousYearDataTasks.Add(Task.Factory.StartNew(() =>
                {
                    var priorYearLegSql = _creator.GetLegSql(useDate, udidNumber, wherePriorYear);
                    _legRawDataPriorYr = RetrieveRawData<LegRawData>(priorYearLegSql, IsReservationReport, true).ToList();
                    //no leg currency data
                }));

            //car 
            previousYearDataTasks.Add(Task.Factory.StartNew(() =>
                {
                    var priorYearCarSql = _creator.GetCarSql(useDate, udidNumber, wherePriorYear);
                    _carRawDataPriorYr = RetrieveRawData<CarRawData>(priorYearCarSql, IsReservationReport, false).ToList();
                    PerformCurrencyConversion(_carRawDataPriorYr);
                }));

            //hotel
            previousYearDataTasks.Add(Task.Factory.StartNew(() =>
                {
                    var priorYearHotelSql = _creator.GetHotelSql(useDate, udidNumber, wherePriorYear);
                    _hotelRawDataPriorYr = RetrieveRawData<HotelRawData>(priorYearHotelSql, IsReservationReport, false).ToList();
                    PerformCurrencyConversion(_hotelRawDataPriorYr);
                }));

            //service fees
            previousYearDataTasks.Add(Task.Factory.StartNew(() =>
                {
                    if (UseServiceFees)
                    {
                        var priorYearFeeSql = _creator.GetSvcFeeSql(useDate, udidNumber, wherePriorYear, OrphanServiceFees);
                        _feeRawDataPriorYr = RetrieveRawData<FeeRawData>(priorYearFeeSql, IsReservationReport, false).ToList();
                        PerformCurrencyConversion(_feeRawDataPriorYr);
                    }
                }));

            Task.WaitAll(previousYearDataTasks.ToArray());
        }

        private void GetCurrentYearData(string useDate, int udidNumber, string currentYearWhereClause)
        {
            var currentYearDataTasks = new List<Task>();

            //air
            currentYearDataTasks.Add(Task.Factory.StartNew(() =>
                {
                    var currentYearAirSql = _creator.GetAirSql(useDate, udidNumber, currentYearWhereClause);
                    RawDataList = RetrieveRawData<RawData>(currentYearAirSql, IsReservationReport, false).ToList();
                    PerformCurrencyConversion(RawDataList);
                }));

            //leg
            currentYearDataTasks.Add(Task.Factory.StartNew(() =>
                {
                    var currentYearLegSql = _creator.GetLegSql(useDate, udidNumber, currentYearWhereClause);
                    _legRawDataCurrYr = RetrieveRawData<LegRawData>(currentYearLegSql, IsReservationReport, true).ToList();
                    //no leg data currency conversion
                }));

            //car
            currentYearDataTasks.Add(Task.Factory.StartNew(() =>
                {
                    var currentYearCarSql = _creator.GetCarSql(useDate, udidNumber, currentYearWhereClause);
                    _carRawDataCurrYr = RetrieveRawData<CarRawData>(currentYearCarSql, IsReservationReport, false, false).ToList();
                    PerformCurrencyConversion(_carRawDataCurrYr);
                }));

            //hotel
            currentYearDataTasks.Add(Task.Factory.StartNew(() =>
                {
                    var currentYearHotelSql = _creator.GetHotelSql(useDate, udidNumber, currentYearWhereClause);
                    _hotelRawDataCurrYr = RetrieveRawData<HotelRawData>(currentYearHotelSql, IsReservationReport, false, false).ToList();
                    PerformCurrencyConversion(_hotelRawDataCurrYr);
                }));

            //fee
            currentYearDataTasks.Add(Task.Factory.StartNew(() =>
                {
                    if (UseServiceFees)
                    {
                        var currentYearFeeSql = _creator.GetSvcFeeSql(useDate, udidNumber, currentYearWhereClause, OrphanServiceFees);
                        _feeRawDataCurrYr = RetrieveRawData<FeeRawData>(currentYearFeeSql, IsReservationReport, false, false).ToList();
                        PerformCurrencyConversion(_feeRawDataCurrYr);
                    }
                }));

            Task.WaitAll(currentYearDataTasks.ToArray());
        }

        public override bool ProcessData()
        {
            var useBaseFare = GlobalCalc.UseBaseFare();
            var reasExclude = Globals.AgencyInformation.ReasonExclude.Split(',').ToList();
            var execSummaryYrToYrDateCalculator = new ExecSummaryYrToYrDateCalculator();

            execSummaryYrToYrDateCalculator.ProcessTripData(useBaseFare, _airRawDataPriorYr, reasExclude);
            execSummaryYrToYrDateCalculator.ProcessTripData(useBaseFare, RawDataList, reasExclude);
            
            var useMileageTable = Globals.IsParmValueOn(WhereCriteria.MILEAGETABLE);
            var carbonCalculator = Globals.GetParmValue(WhereCriteria.CARBONCALC);

            if (useMileageTable)
            {
                AirMileageCalculator<LegRawData>.CalculateAirMileageFromTable(_legRawDataPriorYr);
                AirMileageCalculator<LegRawData>.CalculateAirMileageFromTable(_legRawDataCurrYr);
            }

            if (CarbonReporting)
            {
                var carbonCalc = new CarbonCalculator();
                carbonCalc.SetAirCarbon(_legRawDataPriorYr, UseMetric, carbonCalculator);
                carbonCalc.SetAirCarbon(_legRawDataCurrYr, UseMetric, carbonCalculator);

                carbonCalc.SetCarCarbon(_carRawDataPriorYr, UseMetric, false);
                carbonCalc.SetCarCarbon(_carRawDataCurrYr, UseMetric, false);

                carbonCalc.SetHotelCarbon(_hotelRawDataPriorYr, UseMetric, false);
                carbonCalc.SetHotelCarbon(_hotelRawDataCurrYr, UseMetric, false);
            }

            if (UseMetric)
            {
                MetricImperialConverter.ConvertMilesToKilometers(_legRawDataPriorYr);
                MetricImperialConverter.ConvertMilesToKilometers(_legRawDataCurrYr);
            }

            var rowBuilder = BuildRows();
            FinalDataList = rowBuilder.Rows.OrderBy(x => x.RowNum).ToList();

            var calc = new ExecSummaryYrToYrDateCalculator();
            calc.AssignCurrency(Globals.GetParmValue(WhereCriteria.MONEYTYPE), FinalDataList);

            return true;
        }

        public override bool GenerateReport()
        {
            var startMonthName = IsQuarterToQuarterOption
                        ? StartMonthName.Left(3) + " - " +
                          Globals.EndDate.Value.Month.GetMonthAbbreviationFromNumber(null)
                        : StartMonthName;

            var endMonthAbbreviation = Globals.EndDate.Value.Month.GetMonthAbbreviationFromNumber(null);

            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFieldList = _calc.GetExportFields(StartMonthName, endMonthAbbreviation, ReportOption, StartYear).ToList();

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

                    var subTitle = Globals.ParmValueEquals(WhereCriteria.DATERANGE, "2")
                         ? "Based on Invoice Date"
                         : "Based on Trip Departure Date";

                    ReportSource.SetParameterValue("nYear", StartYear);
                    ReportSource.SetParameterValue("nFiscMonth", FiscalYearStartMonthNumber);
                    ReportSource.SetParameterValue("lLogGen1", GlobalCalc.UseBaseFare());
                    ReportSource.SetParameterValue("cMthName", startMonthName);
                    ReportSource.SetParameterValue("cDateDesc", subTitle);


                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }
        
        private RowBuilder BuildRows()
        {
            var excludeExceptions = Globals.IsParmValueOn(WhereCriteria.CBEXCLUDEEXCEPTNS);
            var excludeMileage = Globals.IsParmValueOn(WhereCriteria.CBEXCLUDEMILEAGE);
            var excludeNegotiatedSavings = Globals.IsParmValueOn(WhereCriteria.CBEXCLUDENEGOT);

            var rowBuilder = new RowBuilder(_begMonth, _begMonth2, _endDate, _endDate2, WeightMeasurement, DistanceMeasurement, CarbonReporting, SplitRail,
                excludeExceptions, excludeMileage, ExcludeServiceFee, excludeNegotiatedSavings, ExcludeSavings);

            rowBuilder.AddTripRows(1, "  Air Summary", _airRawDataPriorYr, RawDataList, _feeRawDataPriorYr,_feeRawDataCurrYr,_legRawDataPriorYr,_legRawDataCurrYr);

            if (SplitRail) rowBuilder.AddTripRows(2, " Rail Summary", _airRawDataPriorYr, RawDataList, _feeRawDataPriorYr, _feeRawDataCurrYr, _legRawDataPriorYr, _legRawDataCurrYr);

            rowBuilder.AddCarRows(_carRawDataPriorYr,_carRawDataCurrYr);
            rowBuilder.AddHotelRows(_hotelRawDataPriorYr, _hotelRawDataCurrYr);
            rowBuilder.AddSummaryRows();

            return rowBuilder;
        }
    }
}

