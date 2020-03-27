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
using iBank.Server.Utilities;
//NOTE: sharing the classes from the combined report. 
using iBank.Services.Implementation.ReportPrograms.CO2CombinedSummary;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.CarbonCalculations;

namespace iBank.Services.Implementation.ReportPrograms.CO2AirSummary
{
    public class Co2AirSum : ReportRunner<RawData, FinalData>
    {
        private string _carbonCalculator;
        private string _poundsKilos;
        private string _milesKilos;
        private bool _useMetric;
        private DateTime _beginDateYear;
        private DateTime _endDateYear;
        private string _groupField;

        private List<RawData> _rawDataListYear;
        private List<RawData> _rawDataCityPair;

        public List<AirSumData> AirSummary;
        public List<CityPairData> CityPair;
        public List<TopCarrierData> TopCarriers;
        public List<ServiceClassData> ServiceClass;
        public List<AirGraphData> Co2Graph;
        public List<AirGraphData> AltCo2Graph;
        public List<AccountBarData> AccountBarGraph;
        public List<TopGroupData> TopGroup;

        public Co2AirSum()
        {
            CrystalReportName = "ibco2airsum";
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

            if (!HasAccount()) return false;

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

            var useDate = Globals.ParmValueEquals(WhereCriteria.DATERANGE, "2") ? "invdate" : "depdate";
            var udidNumber = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1);

            _groupField = Globals.GetParmValue(WhereCriteria.DDTOPBRKCAT).ToUpper();

            //If it's Parent Account, we'll look it up later using Acct.
            if (string.IsNullOrEmpty(_groupField) || _groupField.EqualsIgnoreCase("PARENTACCT")) _groupField = "ACCT";

            var airSql = SqlBuilder.GetSql(udidNumber > 0, GlobalCalc.IsReservationReport(), BuildWhere.WhereClauseFull, useDate, _groupField);
            RawDataList = RetrieveRawData<RawData>(airSql, GlobalCalc.IsReservationReport()).ToList();
           
            if (!IsUnderOfflineThreshold(RawDataList)) return false;
            PerformCurrencyConversion(RawDataList);
            
            _rawDataCityPair = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);

            //get the data for the whole year
            BuildWhere.SqlParameters[0] = new SqlParameter("t1BeginDate", _beginDateYear);
            BuildWhere.SqlParameters[1] = new SqlParameter("t1EndDate", _endDateYear);

            _rawDataListYear = RetrieveRawData<RawData>(airSql, GlobalCalc.IsReservationReport(), false).ToList();
           

            return true;
        }

        public override bool ProcessData()
        {
            var useMileageTable = Globals.IsParmValueOn(WhereCriteria.MILEAGETABLE);

            if (useMileageTable)
            {
                AirMileageCalculator<RawData>.CalculateAirMileageFromTable(RawDataList);
                AirMileageCalculator<RawData>.CalculateAirMileageFromTable(_rawDataListYear);
                AirMileageCalculator<RawData>.CalculateAirMileageFromTable(_rawDataCityPair);
            }

            var carbonCalc = new CarbonCalculator();
            carbonCalc.SetAirCarbon(RawDataList, _useMetric, _carbonCalculator);
            carbonCalc.SetAirCarbon(_rawDataListYear, _useMetric, _carbonCalculator);
            carbonCalc.SetAirCarbon(_rawDataCityPair, _useMetric, _carbonCalculator);

            if (_useMetric)
            {
                MetricImperialConverter.ConvertMilesToKilometers(RawDataList);
                MetricImperialConverter.ConvertMilesToKilometers(_rawDataListYear);
                MetricImperialConverter.ConvertMilesToKilometers(_rawDataCityPair);
            }
            
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (_groupField.EqualsIgnoreCase("ACCT") || string.IsNullOrEmpty(_groupField))
            {
                var accts = RawDataList.Select(s => s.GroupField).Distinct();

                //lookup each acct name found, so we don't have to do it for each record. 
                var pairs = new List<Tuple<string, string>>();
                foreach (var acct in accts)
                {
                    var acctName = clientFunctions.LookupCname(getAllMasterAccountsQuery, acct, Globals);
                    pairs.Add(new Tuple<string, string>(acct,acctName));
                }

                foreach (var row in RawDataList)
                {
                    var acctPair = pairs.FirstOrDefault(s => s.Item1.EqualsIgnoreCase(row.GroupField));
                    if (acctPair != null) row.GroupField = acctPair.Item2;
                }
            }
            else if (_groupField.EqualsIgnoreCase("PARENTACCT"))
            {
                var getAllParentAccountsQuery = new GetAllParentAccountsQuery(ClientStore.ClientQueryDb);

                var accts = RawDataList.Select(s => s.GroupField).Distinct().ToList();
                foreach (var acct in accts)
                {
                    var parentAcct = clientFunctions.LookupParent(getAllMasterAccountsQuery,acct, getAllParentAccountsQuery);
                    foreach (var row in RawDataList.Where(s => s.GroupField.EqualsIgnoreCase(acct)))
                    {
                        row.GroupField = parentAcct.AccountDescription;
                    }
                }
            }

            //preprocess data
            foreach (var row in RawDataList)
            {
                if (string.IsNullOrEmpty(row.ClassCat)) row.ClassCat = string.Empty;
            }

            //build subreport data
            var combine = Globals.ParmValueEquals(WhereCriteria.RBCITYPAIRCOMBINEORNOT, "1") ||
                          !Globals.ParmHasValue(WhereCriteria.RBCITYPAIRCOMBINEORNOT);

            AirSummary = SubReportBuilder.BuildSummaryAir(RawDataList);
            TopGroup = SubReportBuilderAir.BuildTop5GroupField(RawDataList);
            CityPair = SubReportBuilder.BuildCityPair(_rawDataCityPair, combine);
            TopCarriers = SubReportBuilderAir.BuildTopCarrier(MasterStore, RawDataList);
            ServiceClass = SubReportBuilderAir.BuildClassOfService(RawDataList);
            Co2Graph = SubReportBuilder.BuildAirCo2(_rawDataListYear, _beginDateYear);
            AltCo2Graph = SubReportBuilder.BuildAltCo2(_rawDataListYear, _beginDateYear);
            AccountBarGraph = SubReportBuilderAir.BuildAccountBarGraph(RawDataList);
            return true;
        }

        public override bool GenerateReport()
        {
            ReportSource = new ReportDocument();

            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(new List<FinalData>());//there is no data on the main report. 

            ReportSource.Subreports["CO2ExecSumAirSum"].SetDataSource(AirSummary);
            ReportSource.Subreports["CO2AirSumTopGrps"].SetDataSource(TopGroup);
            ReportSource.Subreports["ibCO2ExecSumCityPair"].SetDataSource(CityPair);
            ReportSource.Subreports["CO2AirSumTopCarriers"].SetDataSource(TopCarriers);
            ReportSource.Subreports["CO2AirSumSvcClass"].SetDataSource(ServiceClass);
            ReportSource.Subreports["CO2ExecSumAirGraph1"].SetDataSource(Co2Graph);
            ReportSource.Subreports["CO2ExecSumAirGraph2"].SetDataSource(AltCo2Graph);
            ReportSource.Subreports["CO2AirSumCostCtrGraph"].SetDataSource(AccountBarGraph);


            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

            ReportSource.SetParameterValue("cMileKilo", _milesKilos);
            ReportSource.SetParameterValue("cPoundsKilos", _poundsKilos);
            ReportSource.SetParameterValue("cCatDesc", GetCategoryDescription());
            ReportSource.SetParameterValue("cCharGen1", "TODO");
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

        private string GetCategoryDescription()
        {
            switch (_groupField)
            {
                case "PARENTACCT":
                    return "Parent Account";
                case "SOURCEABBR":
                    return "Data Source";
                case "BREAK1":
                    return Globals.User.Break1Name;
                case "BREAK2":
                    return Globals.User.Break2Name;
                case "BREAK3":
                    return Globals.User.Break3Name;
                default:
                    return "Account";
            }
        }
    }
}
