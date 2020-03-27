using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models.ReportPrograms.TravelDetail;
using Domain.Orm.Classes;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities;
using iBankDomain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TravelDetail
{
    public class TopTravAll : ReportRunner<TravDetRawData, TopTravAllFinalData>
    {
        private readonly TravDetShared _travDetShared;
        private TopTravAllFinalDataCalculator _topTravAllFinalDataCalculator;
        private TopTravAllCarbonCalc _topTravAllCarbonCalc;
        private readonly ReportChecker _checker = new ReportChecker();
        private IQuery<IList<MasterAccountInformation>> _getAllMasterAccountsQuery;
        private int _totalTripCount, _totalRailTripCount, _totalCarDays, _totalHotelNights, _totalDaysOnTheRoad, _totalAirCo2, _totalCarCo2, _totalHotelCo2, _totalCo2;
        private decimal _totalAirChange, _totalRailChange, _totalCarCosts, _totalHotelCosts, _totalTripCost;
        private bool _useCarbonEmissions, _useMetric, _homeCountry;
        private string _carbonCalculator = string.Empty, _exceptions = string.Empty;
        private UserBreaks UserBreaks { get; set; }

        public TopTravAll()
        {
            _travDetShared = new TravDetShared();
        }

        public override bool InitialChecks()
        {
            if (!IsGoodCombo()) return false;
            if (!IsDateRangeValid()) return false;
            if (!IsUdidNumberSuppliedWithUdidText()) return false;
            if (!IsOnlineReport()) return false;
            // Initialize variables used in the report
            SetReportVariables();
            return true;
        }

        public override bool GetRawData()
        {
            // Get report title
            CrystalReportName = _topTravAllFinalDataCalculator.GetCrystalReportName(_useCarbonEmissions, _homeCountry);

            // Get raw data (Trip, car, and hotel)
            if (!_travDetShared.GetRawData(BuildWhere)) return false;
            ConvertCurrencies();
            RawDataList = _travDetShared.RawDataList;

            return true;
        }

        public override bool ProcessData()
        {
            // Apply leg or segment filters
            RawDataList = TopTravAllRawDataCalculator.GetRawData(_travDetShared, _checker.IsAppliedToSegment(Globals), BuildWhere, this);

            _getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            var useMileageTable = Globals.IsParmValueOn(WhereCriteria.MILEAGETABLE);

            // Apply Air/Rail filter if present
            if (!Globals.GetParmValue(WhereCriteria.MODE).IsNullOrWhiteSpace())
            {
                var mode = (Mode)Globals.GetParmValue(WhereCriteria.MODE).TryIntParse(0);
                RawDataList = ApplyModeFilter(RawDataList, mode);
            }

            // Transform raw data to final data
            var routing = BuildWhere.HasFirstDestination || BuildWhere.HasFirstOrigin || BuildWhere.HasRoutingCriteria;
            FinalDataList = _topTravAllFinalDataCalculator.GetFinalDataFromRawData(RawDataList, _travDetShared.Cars, _travDetShared.Hotels, _getAllMasterAccountsQuery, Globals, _homeCountry, _useCarbonEmissions, routing);

            // Apply exceptions to final data list
            FinalDataList = TopTravAllExceptions.ApplyExceptionsToFinalDataList(FinalDataList, _exceptions);
            if (!DataExists(FinalDataList)) return false;

            //Add CO2 emmissions if needed
            if (_useCarbonEmissions)
                _topTravAllCarbonCalc.AddCarbonEmissionsToFinalData(FinalDataList, _travDetShared, useMileageTable, _useMetric, _carbonCalculator, GlobalCalc.IsReservationReport());

            // Group data by passenger
            FinalDataList = TopTravAllFinalGroupData.GetGroupFinalData(FinalDataList);

            // Calculate report totals
            CalculateTotals();

            // Sort Final list
            FinalDataList = TopTravAllData.SortList(FinalDataList, Globals.GetParmValue(WhereCriteria.SORTBY), Globals.GetParmValue(WhereCriteria.RBSORTDESCASC), Globals.GetParmValue(WhereCriteria.HOWMANY));
            return true;
        }

        public override bool GenerateReport()
        {
            var exportFields = TopTravAllData.GetExportFields(UserBreaks, Globals.User.AccountBreak, Globals.User, _useCarbonEmissions);
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                    ExportHelper.ListToXlsx(FinalDataList, exportFields, Globals);
                    break;
                case DestinationSwitch.Csv:
                    ExportHelper.ConvertToCsv(FinalDataList, exportFields, Globals);
                    break;
                default:
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);

                    ReportSource.SetParameterValue("LBFREPORTTOTALS", "Report totals:");
                    ReportSource.SetParameterValue("NTOTCHG", _totalAirChange);
                    ReportSource.SetParameterValue("NTOTCNT", _totalTripCount);
                    ReportSource.SetParameterValue("NTOTDAYS", _totalCarDays);
                    ReportSource.SetParameterValue("NTOTCARCOST", _totalCarCosts);
                    ReportSource.SetParameterValue("NTOTNITES", _totalHotelNights);
                    ReportSource.SetParameterValue("NTOTHOTCOST", _totalHotelCosts);
                    ReportSource.SetParameterValue("NTOTCOST", _totalTripCost);
                    ReportSource.SetParameterValue("NTOTOTR", _totalDaysOnTheRoad);
                    if (_useCarbonEmissions)
                    {
                        ReportSource.SetParameterValue("NAIRCO2", _totalAirCo2);
                        ReportSource.SetParameterValue("NCARCO2", _totalCarCo2);
                        ReportSource.SetParameterValue("NHOTELCO2", _totalHotelCo2);
                        ReportSource.SetParameterValue("NTOTCO2", _totalCo2);
                        ReportSource.SetParameterValue("CCARBCALCRPTFTR", "");
                        ReportSource.SetParameterValue("CPOUNDSKILOS", _topTravAllFinalDataCalculator.GetMetric(_useMetric));
                    }
                    else
                    {
                        ReportSource.SetParameterValue("NTOTCNT2", _totalRailTripCount);
                        ReportSource.SetParameterValue("NTOTCHG2", _totalRailChange);
                    }

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }

        // Filters data list based on mode
        private List<TravDetRawData> ApplyModeFilter(List<TravDetRawData> rawdata, Mode mode)
        {
            if (mode == Mode.RAIL)
            {
                Globals.WhereText = string.IsNullOrEmpty(Globals.WhereText) ? "Rail Only;" : $"{Globals.WhereText} Rail Only";
                _exceptions = _exceptions.IsNullOrWhiteSpace() ? "Rail Only" : _exceptions + " Rail Only";
                return rawdata.Where(r => r.ValCarr.Trim().Length.Equals(4) || r.ValCarMode.Equals("R")).ToList();
            }
            else if(mode == Mode.AIR)
            {
                Globals.WhereText = string.IsNullOrEmpty(Globals.WhereText) ? "Air Only;" : $"{Globals.WhereText} Air Only";
                _exceptions = _exceptions.IsNullOrWhiteSpace() ? "Air Only" : _exceptions + " Air Only";
                return rawdata.Where(r => r.ValCarMode.Equals("A")).ToList();
            }
            return rawdata;
        }

        private void ConvertCurrencies()
        {
            if (_travDetShared.RawDataList.Any()) _travDetShared.RawDataList = PerformCurrencyConversion(_travDetShared.RawDataList);
            if (_travDetShared.Cars.Any()) _travDetShared.Cars = PerformCurrencyConversion(_travDetShared.Cars);
            if (_travDetShared.Hotels.Any()) _travDetShared.Hotels = PerformCurrencyConversion(_travDetShared.Hotels);
            if (_travDetShared.Legs.Any()) _travDetShared.Legs = PerformCurrencyConversion(_travDetShared.Legs);
            if (_travDetShared.Segments.Any()) _travDetShared.Segments = PerformCurrencyConversion(_travDetShared.Segments);
            if (_travDetShared.ServiceFees.Any()) _travDetShared.ServiceFees = PerformCurrencyConversion(_travDetShared.ServiceFees);
        }

        // Initialize report variables
        private void SetReportVariables()
        {
            _topTravAllFinalDataCalculator = new TopTravAllFinalDataCalculator(clientFunctions);
            _topTravAllCarbonCalc = new TopTravAllCarbonCalc();
            _useCarbonEmissions = Globals.IsParmValueOn(WhereCriteria.CARBONEMISSIONS);
            _carbonCalculator = Globals.GetParmValue(WhereCriteria.CARBONCALC);
            _useMetric = Globals.IsParmValueOn(WhereCriteria.METRIC);
            _homeCountry = Globals.IsParmValueOn(WhereCriteria.CBDISLPAYHOMECTRY);
            _exceptions = Globals.GetParmValue(WhereCriteria.DDAIRRAILCARHOTELOPTIONS);
            _travDetShared.IsReservation = Globals.ParmValueEquals(WhereCriteria.PREPOST, "1");

            UserBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);

        }

        // Calculate report totals
        private void CalculateTotals()
        {
            _totalTripCount = FinalDataList.Sum(s => s.tripcount);
            _totalAirChange = FinalDataList.Sum(s => s.airchg);

            _totalCarDays = FinalDataList.Sum(s => s.cardays);
            _totalCarCosts = FinalDataList.Sum(s => s.carcost);

            _totalHotelNights = FinalDataList.Sum(s => s.hotnights);
            _totalHotelCosts = FinalDataList.Sum(s => s.hotelcost);

            _totalTripCost = FinalDataList.Sum(s => s.tripcost);
            _totalRailTripCount = FinalDataList.Sum(s => s.railcount);
            _totalRailChange = FinalDataList.Sum(s => s.railchg);
            _totalDaysOnTheRoad = FinalDataList.Sum(s => s.daysonroad);

            _totalAirCo2 = Convert.ToInt32(FinalDataList.Sum(s => s.Airco2));
            _totalCarCo2 = Convert.ToInt32(FinalDataList.Sum(s => s.Carco2));
            _totalHotelCo2 = Convert.ToInt32(FinalDataList.Sum(s => s.HotelCo2));
            _totalCo2 = _totalAirCo2 + _totalCarCo2 + _totalHotelCo2;
        }
    }
}
