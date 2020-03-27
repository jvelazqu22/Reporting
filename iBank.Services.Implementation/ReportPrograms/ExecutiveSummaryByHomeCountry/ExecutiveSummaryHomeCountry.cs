using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;

using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;

using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.ExecutiveSummaryByHomeCountryReport;
using Domain.Orm.iBankClientQueries;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryByHomeCountry
{
    public class ExecutiveSummaryHomeCountry: ReportRunner<AirRawData, FinalData>
    {
        private readonly ExecutiveSummaryByHomeCountryCalculations _calc = new ExecutiveSummaryByHomeCountryCalculations();
        private ExecutiveSummaryByHomeCountryDataProcessor _dataProcessor = new ExecutiveSummaryByHomeCountryDataProcessor();
        private readonly ExecutiveSummaryByHomeCountrySqlCreator _creator = new ExecutiveSummaryByHomeCountrySqlCreator();
        private List<HotelRawData> _hotelRawDataList;
        private List<CarRawData> _carRawDataList;
        private List<SvcFeeRawData> _svcRawDataList;

        public ExecutiveSummaryHomeCountry()
        {
            CrystalReportName = "ibHomeCtrySum";
            _hotelRawDataList = new List<HotelRawData>();
            _carRawDataList = new List<CarRawData>();
            _svcRawDataList = new List<SvcFeeRawData>();
        }

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

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere:false, buildUdidWhere: true, buildDateWhere:true, inMemory:true, isRoutingBidirectional:false, legDit:false, ignoreTravel:false)) return false;

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
            var udidNumber = GlobalCalc.GetUdidNumber();

            //air data
            var airSql = _creator.CreateAirRawDataSql(BuildWhere.WhereClauseFull, isReservationReport, udidNumber);
            RawDataList = RetrieveRawData<AirRawData>(airSql, isReservationReport, false).ToList();

            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            PerformCurrencyConversion(RawDataList);
            
            //car data
            var carSql = _creator.CreateCarSql(BuildWhere.WhereClauseFull, isReservationReport, udidNumber);
            _carRawDataList = RetrieveRawData<CarRawData>(carSql, isReservationReport, false).ToList();
            PerformCurrencyConversion(_carRawDataList);

            //hotel data
            var hotelSql = _creator.CreateHotelSql(BuildWhere.WhereClauseFull, isReservationReport, udidNumber);
            _hotelRawDataList = RetrieveRawData<HotelRawData>(hotelSql, isReservationReport, false).ToList();
            PerformCurrencyConversion(_hotelRawDataList);

            //service fee
            if (!isReservationReport)
            {
                var svcFeeExistingWhereClause = _creator.GetReplacedServiceFeeWhereClause(BuildWhere.WhereClauseFull);
                var includeServiceFeeNoMatch = _calc.IncludeServiceFeeNoMatch(Globals);
                var svcFeeSql = _creator.CreateSvcFeeSql(svcFeeExistingWhereClause, udidNumber, includeServiceFeeNoMatch);
                _svcRawDataList = RetrieveRawData<SvcFeeRawData>(svcFeeSql, isReservationReport, false).ToList();
                PerformCurrencyConversion(_svcRawDataList);
            }

            //leg data
            //**08 / 24 / 2011-- WE NEED A VALID VALUE FOR valcarMode.WE WILL DEPEND ON **
            //**HISTORY DATA TO HAVE A VALID VALUE, BUT NOT RESERVATION DATA.SO, IF**
            //** lnResBOAir = "1", WE'RE GOING TO GET INFO FROM THE ibLegs TABLE.       **
            if (isReservationReport)
            {
                var legSql = _creator.CreateLegSql(BuildWhere.WhereClauseFull, udidNumber);
                var legData = RetrieveRawData<LegRawData>(legSql, isReservationReport, false);

                foreach (var row in RawDataList)
                {
                    var leg = legData.FirstOrDefault(s => s.RecKey == row.RecKey && s.Airline.EqualsIgnoreCase(row.ValCarr));
                    if (leg != null) row.Valcarmode = leg.Mode;
                    else
                    {
                        leg = legData.FirstOrDefault(s => s.RecKey == row.RecKey);
                        if (leg != null) row.Valcarmode = leg.Mode;
                    }
                }
            }
            
            return true;
        }

        public override bool ProcessData()
        {
            var homeCountry = Globals.GetParmValue(WhereCriteria.HOMECTRY);
            var inHomeCountry = Globals.GetParmValue(WhereCriteria.INHOMECTRY);
            var notIn = Globals.IsParmValueOn(WhereCriteria.NOTINHOMECTRY);
            inHomeCountry = inHomeCountry + homeCountry;//there will be only one
            if (!string.IsNullOrEmpty(homeCountry) || !string.IsNullOrEmpty(inHomeCountry))
            {
                
                var notInText = notIn ? " NOT " : string.Empty;
                Globals.WhereText += notIn
                    ? " Home Country " + notInText + " = " + inHomeCountry
                    : " Home Country " + " = " + inHomeCountry;
            }
            
            var homeCountries = inHomeCountry.Split(',');
            //get the source abbreviations 
            var sourceAbbrs = new List<string>();
            foreach (var country in homeCountries)
            {
                if (!string.IsNullOrEmpty(country))
                {
                    var homeCountrySourceAbbr = LookupFunctions.LookupHomeCountry(country, Globals, MasterStore);
                    if (!string.IsNullOrEmpty(homeCountrySourceAbbr))
                        sourceAbbrs.Add(homeCountrySourceAbbr);
                    else
                        sourceAbbrs.Add(country);
                }
            }

            _dataProcessor.AddAirAndRailData(sourceAbbrs, RawDataList, Globals, FinalDataList);

            _dataProcessor.AddHotelData(sourceAbbrs, _hotelRawDataList, Globals, FinalDataList);

            _dataProcessor.AddCarData(sourceAbbrs, _carRawDataList, Globals, FinalDataList);

            if (!Globals.ParmValueEquals(WhereCriteria.PREPOST, "1")) _dataProcessor.AddServiceFees(_svcRawDataList, Globals, FinalDataList);

            if (!DataExists(FinalDataList)) return false;

            FinalDataList = FinalDataList.OrderBy(s => s.HomeCtry).ThenBy(s => s.RowType).ToList();

            if (Globals.IsParmValueOn(WhereCriteria.CBINCLUDERPTTOTALS)) _calc.AddTotals(FinalDataList);
            
            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = _calc.GetExportFields().ToList();

                    if (Globals.OutputFormat == DestinationSwitch.Xls)
                    {
                        ExportHelper.ListToXlsx(FinalDataList, exportFields, Globals);
                    }
                    else
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, exportFields, Globals);
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
