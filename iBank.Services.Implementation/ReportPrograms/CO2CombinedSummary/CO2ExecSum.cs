using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.CO2CombinedSummaryReport;
using Domain.Orm.iBankClientQueries;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.CarbonCalculations;

namespace iBank.Services.Implementation.ReportPrograms.CO2CombinedSummary
{
    public class Co2ExecSum : ReportRunner<RawData, FinalData>
    {
        private string _carbonCalculator;
        private string _poundsKilos;
        private string _milesKilos;
        private bool _useMetric;
        private DateTime _beginDateYear;
        private DateTime _endDateYear;

        private List<CarRawData> _carRawDataList;
        private List<HotelRawData> _hotelRawDataList;

        private List<RawData> _rawDataListYear;
        private List<CarRawData> _carRawDataListYear;
        private List<HotelRawData> _hotelRawDataListYear;

        public List<AirSumData> AirSummary;
        public List<CityPairData> CityPair;
        public List<CarSumData> CarSummary;
        public List<TopCarData> TopCar;
        public List<HotelSumData> HotelSummary;
        public List<TopHotelData> TopHotel;
        public List<AirGraphData> Co2Graph;
        public List<AirGraphData> AltCo2Graph;
        public List<CarGraphData> CarGraph;
        public List<HotelGraphData> HotelGraph;

        public Co2ExecSum()
        {
            CrystalReportName = "ibco2execsum";
        }

        public override bool InitialChecks()
        {
            if (!IsDateRangeValid()) return false;
            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            _carbonCalculator = Globals.GetParmValue(WhereCriteria.CARBONCALC);

            if (string.IsNullOrEmpty(_carbonCalculator))
            {
                //WHEN RUN FROM BCST, GET THE USER'S DEFAULT CALCULATOR
                if (Globals.IsOfflineServer)
                {
                    _carbonCalculator = new GetUserDefaultCarbCalculatorQuery(ClientStore.ClientQueryDb, Globals.UserNumber).ExecuteQuery();
                    if (string.IsNullOrEmpty(_carbonCalculator))
                    {
                        Globals.ReportInformation.ReturnCode = 2;
                        Globals.ReportInformation.ErrorMessage = "Carbon calculator not specified; user default calculator not found.";
                        return false;
                    }
                }
                else
                {
                    Globals.ReportInformation.ReturnCode = 2;
                    Globals.ReportInformation.ErrorMessage = "For this report, you must choose a carbon calculator.";
                    return false;
                }
            }

            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            SetMetric();
            //create dates for retrieving data for the whole year. 
            var year = Globals.EndDate.Value.Year;
            var month = Globals.EndDate.Value.Month;
            _endDateYear = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            _beginDateYear = _endDateYear.AddYears(-1).AddDays(1);

            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);
            BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false,
                buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false,
                legDit: false, ignoreTravel: false);

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            var useDate = Globals.ParmValueEquals(WhereCriteria.DATERANGE, "2") ? "invdate" : "depdate";
            var udidNumber = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1);
            var hasUdid = udidNumber > 0;
            var airSql = SqlBuilder.GetSqlTripsAndLegs(hasUdid, GlobalCalc.IsReservationReport(), BuildWhere.WhereClauseFull, useDate);
            RawDataList = RetrieveRawData<RawData>(airSql, GlobalCalc.IsReservationReport()).ToList();

            if (!IsUnderOfflineThreshold(RawDataList)) return false;
            PerformCurrencyConversion(RawDataList);

            var carSql = SqlBuilder.GetSqlCar(hasUdid, GlobalCalc.IsReservationReport(), BuildWhere.WhereClauseFull, useDate);
            _carRawDataList = RetrieveRawData<CarRawData>(carSql, GlobalCalc.IsReservationReport(), false).ToList();
            PerformCurrencyConversion(_carRawDataList);

            var hotSql = SqlBuilder.GetSqlHotel(hasUdid, GlobalCalc.IsReservationReport(), BuildWhere.WhereClauseFull, useDate);
            _hotelRawDataList = RetrieveRawData<HotelRawData>(hotSql, GlobalCalc.IsReservationReport(), false).ToList();
            PerformCurrencyConversion(_hotelRawDataList);

            BuildWhere.SqlParameters[0] = new SqlParameter("t1BeginDate", _beginDateYear);
            BuildWhere.SqlParameters[1] = new SqlParameter("t1EndDate", _endDateYear);

            _rawDataListYear = RetrieveRawData<RawData>(airSql, GlobalCalc.IsReservationReport(), false).ToList();

            _carRawDataListYear = RetrieveRawData<CarRawData>(carSql, GlobalCalc.IsReservationReport(), false).ToList();

            _hotelRawDataListYear = RetrieveRawData<HotelRawData>(hotSql, GlobalCalc.IsReservationReport(), false).ToList();

            return true;
        }

        public override bool ProcessData()
        {
            //RawDataList = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);

            var useMileageTable = Globals.IsParmValueOn(WhereCriteria.MILEAGETABLE);
            var isPreview = GlobalCalc.IsReservationReport();

            if (useMileageTable)
            {
                AirMileageCalculator<RawData>.CalculateAirMileageFromTable(RawDataList);
                AirMileageCalculator<RawData>.CalculateAirMileageFromTable(_rawDataListYear);
            }

            var carbonCalc = new CarbonCalculator();
            carbonCalc.SetAirCarbon(RawDataList, _useMetric, _carbonCalculator);
            carbonCalc.SetCarCarbon(_carRawDataList, _useMetric, isPreview);
            carbonCalc.SetHotelCarbon(_hotelRawDataList, _useMetric, isPreview);

            carbonCalc.SetAirCarbon(_rawDataListYear, _useMetric, _carbonCalculator);
            carbonCalc.SetCarCarbon(_carRawDataListYear, _useMetric, isPreview);
            carbonCalc.SetHotelCarbon(_hotelRawDataListYear, _useMetric, isPreview);

            if (_useMetric)
            {
                MetricImperialConverter.ConvertMilesToKilometers(RawDataList);
                MetricImperialConverter.ConvertMilesToKilometers(_rawDataListYear);
            }

            //build subreport data
            var combine = Globals.ParmValueEquals(WhereCriteria.RBCITYPAIRCOMBINEORNOT, "1") ||
                          !Globals.ParmHasValue(WhereCriteria.RBCITYPAIRCOMBINEORNOT);

            AirSummary = SubReportBuilder.BuildSummaryAir(RawDataList);
            CityPair = SubReportBuilder.BuildCityPair(RawDataList, combine);
            CarSummary = SubReportBuilder.BuildCarSummary(_carRawDataList);
            TopCar = SubReportBuilder.BuildTopCar(_carRawDataList);
            HotelSummary = SubReportBuilder.BuildHotelSum(_hotelRawDataList);
            TopHotel = SubReportBuilder.BuildTopHotel(_hotelRawDataList);
            Co2Graph = SubReportBuilder.BuildAirCo2(_rawDataListYear, _beginDateYear);
            AltCo2Graph = SubReportBuilder.BuildAltCo2(_rawDataListYear, _beginDateYear);
            CarGraph = SubReportBuilder.BuildCarGraph(_carRawDataListYear, _beginDateYear);
            HotelGraph = SubReportBuilder.BuildHotelGraph(_hotelRawDataListYear, _beginDateYear);

            return true;
        }

        public override bool GenerateReport()
        {
            ReportSource = new ReportDocument();

            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(new List<FinalData>());//there is no data on the main report. 

            ReportSource.Subreports["CO2ExecSumAirSum"].SetDataSource(AirSummary);
            ReportSource.Subreports["ibCO2ExecSumCityPair"].SetDataSource(CityPair);
            ReportSource.Subreports["CO2ExecSumCarSum"].SetDataSource(CarSummary);
            ReportSource.Subreports["COExecSumTopCar"].SetDataSource(TopCar);
            ReportSource.Subreports["CO2ExecSumHotelSum"].SetDataSource(HotelSummary);
            ReportSource.Subreports["COExecSumTopHotel"].SetDataSource(TopHotel);
            ReportSource.Subreports["CO2ExecSumAirGraph1"].SetDataSource(Co2Graph);
            ReportSource.Subreports["CO2ExecSumAirGraph2"].SetDataSource(AltCo2Graph);
            ReportSource.Subreports["CO2ExecSumCarGraph"].SetDataSource(CarGraph);
            ReportSource.Subreports["CO2ExecSumHotelGraph"].SetDataSource(HotelGraph);

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

            ReportSource.SetParameterValue("cMileKilo", _milesKilos);
            ReportSource.SetParameterValue("cPoundsKilos", _poundsKilos);
            var dateDesc = Globals.ParmValueEquals(WhereCriteria.DATERANGE, "1")
               ? "Trip Departure Dates from {0} to {1}"
               : "Invoice Dates from {0} to {1}";
            ReportSource.SetParameterValue("cDateDesc2", string.Format(dateDesc, _beginDateYear.ToShortDateString(), _endDateYear.ToShortDateString()));

            CrystalFunctions.CreatePdf(ReportSource, Globals);
            return true;
        }

        private void SetMetric()
        {
            _useMetric = Globals.IsParmValueOn(WhereCriteria.METRIC);
            if (_useMetric)
            {
                _poundsKilos = "Kgs";
                _milesKilos = "Kms";
            }
            else
            {
                _poundsKilos = "Lbs.";
                _milesKilos = "Miles";
            }
        }
    }
}
