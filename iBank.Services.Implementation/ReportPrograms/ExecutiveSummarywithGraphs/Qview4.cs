using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.ExecutiveSummarywithGraphsReport;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.CarbonCalculations;

namespace iBank.Services.Implementation.ReportPrograms.ExecutiveSummarywithGraphs
{
    public class Qview4 : ReportRunner<RawData, FinalData>
    {
        private Translations _translations;

        private bool _mileageTable;
        private bool _useMetric;
        private bool _carbonReporting;
        private bool _excludeServiceFee;
        private bool _useServiceFee;
        private bool _railData;
        private bool _orphanServiceFees;
        private bool _useBaseFare;

        private BackofficeOrReservation _airBor;
        private BackofficeOrReservation _carBor;
        private BackofficeOrReservation _hotBor;

        private string _poundsKilos;
        private string _milesKilos;

        private List<CarRawData> _carRawData;
        private List<HotelRawData> _hotelRawData;
        private List<CityPairRawData> _cityPairRawData;
        private List<FeeRawData> _feeRawData;
        private List<RawData> _rawDataListYear;
        private List<CarRawData> _carRawDataYear;
        private List<HotelRawData> _hotelRawDataYear;
        private DateTime _beginDateYear;
        private DateTime _endDateYear;

        //subreport datasets
        public List<AirSumData> AirSumSubreport { get; set; }
        public List<RailSumData> RailSumSubreport { get; set; }
        public List<TopCityData> TopCitySubreport { get; set; }
        public List<CarSumData> CarSumSubreport { get; set; }
        public List<TopCarData> TopCarSubreport { get; set; }
        public List<HotSumData> HotelSumSubreport { get; set; }
        public List<TopHotData> TopHotSubreport { get; set; }

        public List<BarData> AirBarSubreport { get; set; }
        public List<BarData> CarBarSubreport { get; set; }
        public List<BarData> HotelBarSubreport { get; set; }
        public List<BarData> RailBarSubreport { get; set; }

        public List<PieData> AirPieSubreport { get; set; }
        public List<PieData> CarPieSubreport { get; set; }
        public List<PieData> HotelPieSubreport { get; set; }
        public List<PieData> RailPieSubreport { get; set; }

        public List<TopCityData> TopRailSubreport { get; set; }
        
        public Qview4()
        {
            CrystalReportName = "execSumGraphs";

            _carRawData = new List<CarRawData>();
            _carRawDataYear = new List<CarRawData>();
            CarBarSubreport = new List<BarData>();
            CarPieSubreport = new List<PieData>();
            TopCarSubreport = new List<TopCarData>();

            _hotelRawData = new List<HotelRawData>();
            _hotelRawDataYear = new List<HotelRawData>();
            HotelBarSubreport = new List<BarData>();
            HotelPieSubreport = new List<PieData>();
            TopHotSubreport = new List<TopHotData>();

            RailSumSubreport = new List<RailSumData>();
            TopRailSubreport = new List<TopCityData>();
            RailPieSubreport = new List<PieData>();
            RailBarSubreport = new List<BarData>();

            _cityPairRawData = new List<CityPairRawData>();
            _feeRawData = new List<FeeRawData>();
        }

        public override bool InitialChecks()
        {
            _translations = new Translations(Globals);
            SetFlags();
            SetSubtitle();

            if (!IsDateRangeValid()) return false;
            if (!IsUdidNumberSuppliedWithUdidText()) return false;
            if (!IsOnlineReport()) return false;

            if (!HasAccount()) return false;
            
            return true;
        }
        
        public override bool GetRawData()
        {
            var useDate = Globals.ParmValueEquals(WhereCriteria.DATERANGE, "1") ? "DepDate" : "InvDate";
            var udidNumber = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1);

            //create dates for retrieving data for the whole year. 
            var year = Globals.EndDate.Value.Year;
            var month = Globals.EndDate.Value.Month;
            _endDateYear = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            _beginDateYear = _endDateYear.AddYears(-1).AddDays(1);
            
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            var scriptBuilder = new SqlBuilder(getAllMasterAccountsQuery,BuildWhere);
            
            if (!SetTripRawData(scriptBuilder, useDate, udidNumber)) return false;

            //we get either rail or car, not both. 
            if (!_railData) SetCarRawData(scriptBuilder, useDate, udidNumber);

            SetHotelRawData(scriptBuilder, useDate, udidNumber);

            SetCityPairRawData(scriptBuilder, useDate, udidNumber);

            //service fee
            if (_useServiceFee) SetServiceFeeRawData(scriptBuilder, useDate, udidNumber);

            return DoesDataExist(RawDataList, _hotelRawData, _carRawData, _feeRawData, _railData, _orphanServiceFees);
        }
        
        private bool SetTripRawData(SqlBuilder scriptBuilder, string useDate, int udidNumber)
        {
            var airScript = scriptBuilder.BuildTripQuery(useDate, udidNumber > 0);

            RawDataList = RetrieveRawData<RawData>(airScript, GlobalCalc.IsReservationReport(), false).ToList();
            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            PerformCurrencyConversion(RawDataList);

            BuildWhere.SqlParameters[0] = new SqlParameter("t1BeginDate", _beginDateYear);
            BuildWhere.SqlParameters[1] = new SqlParameter("t1EndDate", _endDateYear);

            _rawDataListYear = RetrieveRawData<RawData>(airScript, GlobalCalc.IsReservationReport(), false).ToList();

            PerformCurrencyConversion(_rawDataListYear);

            return true;
        }

        private void SetCarRawData(SqlBuilder scriptBuilder, string useDate, int udidNumber)
        {
            var carScript = scriptBuilder.BuildCarQuery(useDate, udidNumber > 0);

            _carRawData = RetrieveRawData<CarRawData>(carScript, GlobalCalc.IsReservationReport(), false).ToList();
            PerformCurrencyConversion(_carRawData);

            BuildWhere.SqlParameters[0] = new SqlParameter("t1BeginDate", _beginDateYear);
            BuildWhere.SqlParameters[1] = new SqlParameter("t1EndDate", _endDateYear);

            _carRawDataYear = RetrieveRawData<CarRawData>(carScript, GlobalCalc.IsReservationReport(), false).ToList();
            PerformCurrencyConversion(_carRawDataYear);
        }

        private void SetHotelRawData(SqlBuilder scriptBuilder, string useDate, int udidNumber)
        {
            var hotelScript = scriptBuilder.BuildHotelQuery(useDate, udidNumber > 0);

            _hotelRawData = RetrieveRawData<HotelRawData>(hotelScript, isReservationReport: GlobalCalc.IsReservationReport(), addFieldsFromLegsTable: false, 
                includeAllLegs: false, checkForDuplicatesAndRemoveThem: false, handleAdvanceParamsAtReportLevelOnly: false).ToList();
            PerformCurrencyConversion(_hotelRawData);

            BuildWhere.SqlParameters[0] = new SqlParameter("t1BeginDate", _beginDateYear);
            BuildWhere.SqlParameters[1] = new SqlParameter("t1EndDate", _endDateYear);

            _hotelRawDataYear = RetrieveRawData<HotelRawData>(hotelScript, GlobalCalc.IsReservationReport(), false).ToList();
            PerformCurrencyConversion(_hotelRawDataYear);
        }

        private void SetCityPairRawData(SqlBuilder scriptBuilder, string useDate, int udidNumber)
        {
            var cityPairScript = scriptBuilder.BuildCityPairQuery(useDate, udidNumber > 0);

            _cityPairRawData = RetrieveRawData<CityPairRawData>(cityPairScript, GlobalCalc.IsReservationReport(), false).ToList();
            PerformCurrencyConversion(_cityPairRawData);

            _cityPairRawData = GlobalCalc.IsAppliedToLegLevelData() ? BuildWhere.ApplyWhereRoute(_cityPairRawData, true) : BuildWhere.ApplyWhereRoute(_cityPairRawData, false);

            if (Globals.IsParmValueOn(WhereCriteria.MILEAGETABLE)) AirMileageCalculator<CityPairRawData>.CalculateAirMileageFromTable(_cityPairRawData);

            if (Globals.IsParmValueOn(WhereCriteria.SEGFAREMILEAGE)) FareByMileage<CityPairRawData>.CalculateFareByMileage(_cityPairRawData);

            if (Globals.IsParmValueOn(WhereCriteria.CARBONEMISSIONS))
            {
                if (_mileageTable) AirMileageCalculator<CityPairRawData>.CalculateAirMileageFromTable(_cityPairRawData);

                var carbonCalc = new CarbonCalculator();
                carbonCalc.SetAirCarbon(_cityPairRawData, _useMetric, Globals.GetParmValue(WhereCriteria.CARBONCALC));

                if (_useMetric) MetricImperialConverter.ConvertMilesToKilometers(_cityPairRawData);
            }
        }

        private void SetServiceFeeRawData(SqlBuilder scriptBuilder, string useDate, int udidNumber)
        {
            var svcFeeScript = scriptBuilder.BuildSvcFeeQuery(useDate, udidNumber > 0, _orphanServiceFees);

            _feeRawData = RetrieveRawData<FeeRawData>(svcFeeScript, GlobalCalc.IsReservationReport(), false).ToList();
            PerformCurrencyConversion(_feeRawData);
        }

        private bool DoesDataExist(List<RawData> rawData, List<HotelRawData> hotelData, List<CarRawData> carData, List<FeeRawData> feeData, bool railData,
                                   bool orphanServiceFees)
        {
            if (railData)
            {
                if (!rawData.Any() && !hotelData.Any())
                {
                    SetNoDataMessaging();
                    return false;
                }
            }
            else if (orphanServiceFees)
            {
                if (!rawData.Any() && !hotelData.Any() && !carData.Any() && !feeData.Any())
                {
                    SetNoDataMessaging();
                    return false;
                }
            }
            else
            {
                if (!rawData.Any() && !hotelData.Any() && !carData.Any())
                {
                    SetNoDataMessaging();
                    return false;
                }
            }

            return true;
        }

        private void SetNoDataMessaging()
        {
            Globals.ReportInformation.ReturnCode = 2;
            Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.RptMsg_NoData;
        }

        public override bool ProcessData()
        {
            if (_airBor == BackofficeOrReservation.Reservation) DataTransformer.CorrectValidatingCarrierMode(RawDataList, _cityPairRawData);

            var exclusionReasons = Globals.AgencyInformation.ReasonExclude.Split(',').ToList();
            DataTransformer.ProcessRawData(RawDataList, exclusionReasons, _useBaseFare);

            var dataToShow = Globals.GetParmValue(WhereCriteria.RBONGRAPHSSHOW);

            var svcFeeTotal = DataRetrieval.GetServiceFeeTotal(_feeRawData, RawDataList, _airBor, _orphanServiceFees);
            AirSumSubreport = GetAirSummaryData(_carbonReporting, _cityPairRawData, RawDataList, svcFeeTotal, _railData);

            var combine = Globals.ParmValueEquals(WhereCriteria.RBCITYPAIRCOMBINEORNOT, "1") || !Globals.ParmHasValue(WhereCriteria.RBCITYPAIRCOMBINEORNOT);
            TopCitySubreport = SubReportBuilder.BuildTopCity(_cityPairRawData.Where(s => s.Mode.EqualsIgnoreCase("A") || !_railData).ToList(), combine, _railData);
            if (!_railData)
            {
                CarSumSubreport = SubReportBuilder.BuildCarSum(_carRawData);
                TopCarSubreport = SubReportBuilder.BuildTopCar(_carRawData);
            }
            else
            {
                RailSumSubreport = SubReportBuilder.BuildSummaryRail(RawDataList);
                TopRailSubreport = SubReportBuilder.BuildTopCity(_cityPairRawData.Where(s => s.Mode.EqualsIgnoreCase("R")).ToList(), combine, _railData);
            }
            
            HotelSumSubreport = SubReportBuilder.BuildHotelSum(_hotelRawData);
            TopHotSubreport = SubReportBuilder.BuildTopHotel(_hotelRawData);

            var monthList = _translations.xAbbrMthsofYear.Split(',').ToList();
            AirBarSubreport = SubReportBuilder.BuildAirBar(_rawDataListYear.Where(s => s.Mode.EqualsIgnoreCase("A") || !_railData).ToList(), _beginDateYear, dataToShow, _railData, dataToShow.Equals("2") ? _translations.xNbrAirTrans : _translations.xAirCharges, Globals.UserLanguage, monthList);
            AirPieSubreport = SubReportBuilder.BuildAirPie(MasterStore, RawDataList.Where(s => s.ValcarMode.EqualsIgnoreCase("A") || !_railData).ToList(), dataToShow, _railData, !dataToShow.Equals("2") ? _translations.xAirVolBooked : _translations.xNbrAirTrans);


            var dataToShow2 = dataToShow.Equals("1") && Globals.Agency.EqualsIgnoreCase("DESIGN") //Special case for this agency
                ? "D"
                : dataToShow;
            
            if (!_railData)
            {
                var carBartitle = dataToShow.Equals("3") ? _translations.xCarRentalVol : _translations.xNbrDaysRented;
                if (dataToShow2.Equals("D")) carBartitle = _translations.xNbrCarsRented;

                CarBarSubreport = SubReportBuilder.BuildCarBar(_carRawDataYear, _beginDateYear, dataToShow2, carBartitle, Globals.UserLanguage, monthList);
                CarPieSubreport = CarBarSubreport.Count > 0 ? SubReportBuilder.BuildCarPie(_carRawData, dataToShow2, carBartitle) : null;
            }
            else
            {
                RailBarSubreport = SubReportBuilder.BuildAirBar(_rawDataListYear.Where(s => s.Mode.EqualsIgnoreCase("R")).ToList(), _beginDateYear, dataToShow, _railData, dataToShow.Equals("2") ? _translations.xNbrRailTrans : _translations.xRailCharges, Globals.UserLanguage, monthList);
                RailPieSubreport = RailBarSubreport.Count > 0 ? SubReportBuilder.BuildAirPie(MasterStore, RawDataList.Where(s => s.ValcarMode.EqualsIgnoreCase("R")).ToList(), dataToShow, _railData, dataToShow.Equals("2") ? _translations.xNbrRailTrans : _translations.xRailVolBooked) : null;
            }
            var hotelBartitle = dataToShow.Equals("3") ? _translations.xHotelBkgVol : _translations.xNbrOfRoomNights;
            if (dataToShow2.Equals("D")) hotelBartitle = _translations.xNbrHotelBookings;

            HotelBarSubreport = SubReportBuilder.BuildHotelBar(_hotelRawDataYear,_beginDateYear, dataToShow2, hotelBartitle, Globals.UserLanguage, monthList);
            HotelPieSubreport = SubReportBuilder.BuildHotelPie(_hotelRawData, dataToShow2, hotelBartitle);

            UpdateGlobalsToResetAccNameBasedOnPickListNameIfApplicable();

            return true;
        }

        private void UpdateGlobalsToResetAccNameBasedOnPickListNameIfApplicable()
        {
            var inAccts = Globals.GetParmValue(WhereCriteria.INACCT);
            var acct = string.Empty;
            var isItPickList = false;
            if (!string.IsNullOrEmpty(inAccts))
            {
                var accts = inAccts.Split(',').ToList();
                if (accts.Count == 1)
                {
                    acct = accts.First();
                    if (acct.Contains("U-")) isItPickList = true;
                }
            }

            if (!isItPickList) return;

            var pickList = new PickListParms(Globals);
            pickList.ProcessList(acct, string.Empty, "ACCTS");
            Globals.PickListName = string.IsNullOrEmpty(pickList?.PickName)
                ? string.Empty
                : pickList.PickName;

            if (string.IsNullOrEmpty(Globals.PickListName)) return;

            Globals.ReplaceCAcctNameAndAccountInParamsWithPickListName = true;
            if (!Globals.IsListBreakoutEnabled) ReplaceAccountsInPickListWithPickListNameInGlobalWhereText(Globals.PickListName);
        }

        private void ReplaceAccountsInPickListWithPickListNameInGlobalWhereText(string pickListName)
        {
            var parameters = Globals.WhereText.Split(';').ToList();
            Globals.WhereText = string.Empty;
            foreach (var param in parameters)
            {
                Globals.WhereText += param.ContainsIgnoreCase("Account in")
                    ? $"Account in {pickListName}; "
                    : $"{param}; ";
            }
        }

        private List<AirSumData> GetAirSummaryData(bool carbonReporting, List<CityPairRawData> cityPairRawData, List<RawData> rawData, decimal svcFeeTotal, bool railData)
        {
            var miles = carbonReporting ? cityPairRawData.Sum(s => s.Miles) : -1;
            var airCo2 = carbonReporting ? cityPairRawData.Sum(s => s.AirCo2) : -1;

            return SubReportBuilder.BuildSummaryAir(rawData, svcFeeTotal, railData, miles, airCo2);
        }

        public override bool GenerateReport()
        {
            if (_railData) CrystalReportName += "Rail";

            if (Globals.ParmValueEquals(WhereCriteria.DDCOSMETIC, "ENHANCED")) CrystalReportName += "_MK";

            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);
            ReportSource.SetDataSource(new List<FinalData>()); //there is no data on the main report. 
            var airDataYear = _rawDataListYear.Where(s => !_railData || s.Mode.EqualsIgnoreCase("A")).ToList();
            var railDataYear = _rawDataListYear.Where(s => s.Mode.EqualsIgnoreCase("R")).ToList();

            SetSubreports();

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
            SetAdditionalReportParameters(airDataYear, railDataYear);
            CrystalFunctions.CreatePdf(ReportSource, Globals);

            return true;
        }

        private void RaiseCrystalJobLimitException(string rptFilePath)
        {
            for (int i = 0; i < 1000; i++)
            {
                try
                {
                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);
                    ReportSource.SetDataSource(new List<FinalData>()); //there is no data on the main report. 
                    var airDataYear = _rawDataListYear.Where(s => !_railData || s.Mode.EqualsIgnoreCase("A")).ToList();
                    var railDataYear = _rawDataListYear.Where(s => s.Mode.EqualsIgnoreCase("R")).ToList();

                    SetSubreports();

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
                    SetAdditionalReportParameters(airDataYear, railDataYear);
                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                }
                catch (Exception ex2)
                {
                    if (ex2 is CrystalDecisions.Shared.CrystalReportsException)
                    {
                        if (ex2.InnerException.Message.Contains("The maximum report processing jobs limit configured by your system administrator has been reached"))
                        {

                        }
                    }
                }
            }
        }

        private void SetSubreports()
        {
            ReportSource.Subreports["execSumAirSum"].SetDataSource(AirSumSubreport);
            ReportSource.Subreports["execSumTopCity"].SetDataSource(TopCitySubreport);

            ReportSource.Subreports["execSumHotSum"].SetDataSource(HotelSumSubreport);
            ReportSource.Subreports["execSumTopHot"].SetDataSource(TopHotSubreport);
            ReportSource.Subreports["execSumAirBar"].SetDataSource(AirBarSubreport);

            ReportSource.Subreports["execSumHotBar"].SetDataSource(HotelBarSubreport);
            ReportSource.Subreports["execSumAirPie.rpt"].SetDataSource(AirPieSubreport);

            ReportSource.Subreports["execSumHotPie"].SetDataSource(HotelPieSubreport);

            if (_railData)
            {
                ReportSource.Subreports["execSumRailSum"].SetDataSource(RailSumSubreport);
                ReportSource.Subreports["railTopCity"].SetDataSource(TopRailSubreport);
                ReportSource.Subreports["execSumRailBar"].SetDataSource(RailBarSubreport);
                ReportSource.Subreports["execSumRailPie"].SetDataSource(RailPieSubreport);
            }
            else
            {
                ReportSource.Subreports["execSumCarSum"].SetDataSource(CarSumSubreport);
                ReportSource.Subreports["execSumTopCar"].SetDataSource(TopCarSubreport);
                ReportSource.Subreports["execSumCarBar"].SetDataSource(CarBarSubreport);
                ReportSource.Subreports["execSumCarPie"].SetDataSource(CarPieSubreport);
            }
        }

        private void SetAdditionalReportParameters(List<RawData> airDataYear, List<RawData> railDataYear)
        {
            ReportSource.SetParameterValue("airDataExists", airDataYear.Any());
            ReportSource.SetParameterValue("carDataExists", _carRawDataYear.Any());
            ReportSource.SetParameterValue("hotelDataExists", _hotelRawDataYear.Any());

            if (_railData) ReportSource.SetParameterValue("railDataExists", railDataYear.Any());

            var dateDesc = Globals.BuildDateDesc();
            var dateDesc2 = Globals.BuildDateDesc(_beginDateYear, _endDateYear);
            ReportSource.SetParameterValue("cDateDesc", dateDesc);
            ReportSource.SetParameterValue("cDateDesc2", dateDesc2);
            ReportSource.SetParameterValue("cDateDesc3", dateDesc);

            ReportSource.SetParameterValue("lExNegoSvgs", Globals.IsParmValueOn(WhereCriteria.CBEXCLUDENEGOT));
            ReportSource.SetParameterValue("lExSvcFee", Globals.IsParmValueOn(WhereCriteria.CBEXCLUDESVCFEES));
            ReportSource.SetParameterValue("lExSavings", Globals.IsParmValueOn(WhereCriteria.CBEXCLUDESVGS));
            ReportSource.SetParameterValue("lExExcepts", Globals.IsParmValueOn(WhereCriteria.CBEXCLUDEEXCEPTNS));
            ReportSource.SetParameterValue("lUseBaseFare", _useBaseFare);
            ReportSource.SetParameterValue("cMileKilo", _milesKilos);
            ReportSource.SetParameterValue("cPoundsKilos", _poundsKilos);
        }

        private void SetFlags()
        {
            _carbonReporting = Globals.IsParmValueOn(WhereCriteria.CARBONEMISSIONS);
            _mileageTable = Globals.IsParmValueOn(WhereCriteria.MILEAGETABLE);
            _excludeServiceFee = Globals.IsParmValueOn(WhereCriteria.CBEXCLUDESVCFEES);

            if (_excludeServiceFee) Globals.AgencyInformation.UseServiceFees = false;

            _useServiceFee = Globals.AgencyInformation.UseServiceFees;

            _useMetric = Globals.IsParmValueOn(WhereCriteria.METRIC);
            _poundsKilos = _useMetric ? _translations.xKgs : _translations.xLbs;
            _milesKilos = _useMetric ? _translations.xKilometers : _translations.xMiles;
            _railData = Globals.ParmValueEquals(WhereCriteria.RBRPTVERSION,"2");

            _airBor = Globals.ParmValueEquals(WhereCriteria.PREPOSTAIR, "1") ? BackofficeOrReservation.Reservation : BackofficeOrReservation.Backoffice;
            _carBor = Globals.ParmValueEquals(WhereCriteria.PREPOSTCAR, "1") ? BackofficeOrReservation.Reservation : BackofficeOrReservation.Backoffice;
            _hotBor = Globals.ParmValueEquals(WhereCriteria.PREPOSTHOT, "1") ? BackofficeOrReservation.Reservation : BackofficeOrReservation.Backoffice;
            
            _orphanServiceFees = Globals.IsParmValueOn(WhereCriteria.CBINCLSVCFEENOMATCH) && Globals.AgencyInformation.UseServiceFees;
            _useBaseFare = Globals.IsParmValueOn(WhereCriteria.CBUSEBASEFARE);
        }

        private void SetSubtitle()
        {
            //if three of them are the same
            if (_airBor == _hotBor && _airBor == _carBor)
            {
                Globals.HstPrePref = _airBor == BackofficeOrReservation.Reservation ? _translations.xResData : _translations.xBackOffData;
            }
            else if (_airBor == _hotBor)
            {
                Globals.HstPrePref = _translations.xAirHotelRanges + " ";
                Globals.HstPrePref += _airBor == BackofficeOrReservation.Reservation ? _translations.xResData : _translations.xBackOffData;
                Globals.HstPrePref += "; ";
                Globals.HstPrePref += _translations.xCarRange + " ";
                Globals.HstPrePref += _carBor == BackofficeOrReservation.Reservation ? _translations.xResData : _translations.xBackOffData;
            }
            else if (_airBor == _carBor)
            {
                Globals.HstPrePref = _translations.xAirCarRanges + " ";
                Globals.HstPrePref += _airBor == BackofficeOrReservation.Reservation ? _translations.xResData : _translations.xBackOffData;
                Globals.HstPrePref += "; ";
                Globals.HstPrePref += _translations.xHotelRange + " ";
                Globals.HstPrePref += _hotBor == BackofficeOrReservation.Reservation ? _translations.xResData : _translations.xBackOffData;
            }
            else if (_hotBor == _carBor)
            {
                Globals.HstPrePref = _translations.xAirRange + " ";
                Globals.HstPrePref += _airBor == BackofficeOrReservation.Reservation ? _translations.xResData : _translations.xBackOffData;
                Globals.HstPrePref += "; ";
                Globals.HstPrePref += _translations.xCarHotelRange + " ";
                Globals.HstPrePref += _hotBor == BackofficeOrReservation.Reservation ? _translations.xResData : _translations.xBackOffData;
            }
        }
    }
}
