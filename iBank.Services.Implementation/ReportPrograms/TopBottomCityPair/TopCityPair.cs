using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models;
using Domain.Orm.iBankClientQueries;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.TopBottomCityPairReport;
using iBank.Services.Implementation.Shared.CarbonCalculations;
using iBank.Server.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomCityPair
{
    public class TopCityPair : ReportRunner<RawData, FinalData>
    {
        private TopCityPairHelper topCityPairBasic { get; set; }
        private readonly DataHelper dataHelper = new DataHelper();

        private bool _isSegFareMileage { get; set; }
        private bool _llMileageTable { get; set; }
        private bool _isExcludeMileage { get; set; }
        private string _cPoundsKilos { get; set; }
        private string _cCarbCalcRptFtr { get; set; }
        private bool _llCarbonRptg;
        private bool _llCityPairByMetro;
        private bool _llUseTickCnt;
        private bool _llOnlineAdopt;
        private string _lcCarbCalc;
        private string _cSubTitle;
        private string _cMileKilo;
        private List<RawData> _uncollapseRawData = new List<RawData>();
        private bool _isMetric;
        private bool _isRouteBidirectional;
        private int nHowMany;

        public decimal nTotCnt { get; set; }
        public decimal nTotChg { get; set; }
        public DataTypes.Sort Sort { get; set; }
        public DataTypes.SortBy SortBy { get; set; }
        public string OrderBy;

        private bool IsReservationReport { get; set; }
        private string UdidNumber { get; set; }

        public override bool InitialChecks()
        {
            Globals.BeginDate = DateConverter.ReplaceEmptyDate(Globals.BeginDate, Globals.EndDate);

            //if the end date is empty and the begin date is not set the end date to the begin date value
            Globals.EndDate = DateConverter.ReplaceEmptyDate(Globals.EndDate, Globals.BeginDate);

            if (!IsDateRangeValid()) return false;

            if (!IsDataRangeUnderThreeMonths()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            topCityPairBasic = new TopCityPairHelper();

            ReformatRouteParameters();

            SetProperties();

            return true;
        }

        //Could add oring/destination country region etc too when need. same logic.
        public void ReformatRouteParameters()
        {
            var org = Globals.AdvancedParameters.Parameters.Where(x => x.FieldName == "ORIGIN").ToList();
            var dest = Globals.AdvancedParameters.Parameters.Where(x => x.FieldName == "DESTINAT").ToList();
            if (org.Count > 0 || dest.Count > 0)
            {
                Globals.ConvertAdvancedRouteParmToReportParm("ORIGIN", WhereCriteria.INORGS, WhereCriteria.NOTINORIGIN);
                Globals.ConvertAdvancedRouteParmToReportParm("DESTINAT", WhereCriteria.INDESTS, WhereCriteria.NOTINDESTS);
            }            
        }

        public override bool GetRawData()
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false,buildHotelWhere: false,buildUdidWhere: true, buildDateWhere: true, inMemory: true, isRoutingBidirectional: _isRouteBidirectional, legDit: true, ignoreTravel: false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            var whereClause = BuildWhere.WhereClauseFull;

            var sqlBuilder = new SqlBuilder();
            var specialFields = sqlBuilder.GetSpecialList(_llCityPairByMetro);
            var fieldList = sqlBuilder.GetFieldList(IsReservationReport);
            var sql = new SqlScript
            {
                FieldList = fieldList + "," + specialFields,
                FromClause = sqlBuilder.GetFromClause(IsReservationReport, UdidNumber),
                WhereClause = sqlBuilder.GetWherelause(whereClause, UdidNumber),
                KeyWhereClause = sqlBuilder.GetKeyWhereClause(UdidNumber),
                OrderBy = "order by T1.Reckey, T2.SeqNo",
                GroupBy = ""
            };
            RawDataList = RetrieveRawData<RawData>(sql, IsReservationReport, false, true).ToList();

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

            if (!DataExists(RawDataList)) return false;

            if (_llCarbonRptg) CarbonCalculations(_lcCarbCalc);
            
            //use to compute total mileage on leg/seg level
            _uncollapseRawData = RawDataList;
            RawDataList = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both, _llMileageTable, _isSegFareMileage);

            if (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstOrigin || BuildWhere.HasFirstDestination )
            {
                RawDataList = BuildWhere.ApplyWhereRoute(RawDataList, false, false);
            }

            PerformCurrencyConversion(RawDataList);

            return true;
        }

        public override bool ProcessData()
        {

            if (_llUseTickCnt || _llOnlineAdopt) RawDataList = dataHelper.CalculateAdopt(RawDataList);

            if (_isSegFareMileage) RawDataList = dataHelper.AllocateAirCharge(_uncollapseRawData, RawDataList);               

            if (_isRouteBidirectional) RawDataList = dataHelper.CalculateOneWayBothWay(RawDataList);

            if (_llCityPairByMetro) RawDataList = dataHelper.ApplyCityPair(RawDataList);

            //this should be the only place to do metric convertion
            if (_isMetric) MetricImperialConverter.ConvertMilesToKilometers(RawDataList, false);

            if (!DataExists(RawDataList)) return false;

            var semiDataHalper = new SemiDataProducer(RawDataList);
            var SemiFinalDataList = semiDataHalper.ProduceSemiData();
            
            var finalDataProducer = new FinalDataProducer();
            FinalDataList = finalDataProducer.GetFirstDraftData(MasterStore, _llCityPairByMetro, SemiFinalDataList);

            nTotCnt = finalDataProducer.CalculateTotalCount(FinalDataList, _llUseTickCnt);
        
            FinalDataList = finalDataProducer.GetTopOrgDestGroup(FinalDataList, SortBy, Sort, _llUseTickCnt, nHowMany);
            
            if (Globals.OutputFormat == DestinationSwitch.Xls || Globals.OutputFormat == DestinationSwitch.Csv)
            {
                FinalDataList = finalDataProducer.CalculateExcelCost(FinalDataList, nTotCnt, _isExcludeMileage);
            }
            
            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = dataHelper.GetExportFields(_isMetric, _llCarbonRptg);

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, exportFields.ToList(), Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, exportFields.ToList(), Globals);
                    }
                    break;
                default:
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;
                    ReportSource = SetPdfReportSource(rptFilePath);

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }


        private void SetProperties()
        {
            _isSegFareMileage = Globals.ParmValueEquals(WhereCriteria.SEGFAREMILEAGE, "ON");
            _llOnlineAdopt = Globals.ParmValueEquals(WhereCriteria.CBRPTVERSION, "ON");

            if (_llOnlineAdopt)
            {
                //ALL CARBON AND MILEAGE OPTIONS WILL BE IGNORED
                Globals.SetParmValue(WhereCriteria.MILEAGETABLE, "OFF");
            }

            _isExcludeMileage = Globals.ParmValueEquals(WhereCriteria.CBEXCLUDEMILEAGE, "ON") || _llOnlineAdopt;
            _lcCarbCalc = Globals.GetParmValue(WhereCriteria.CARBONCALC);
            _llCarbonRptg = topCityPairBasic.GetCarbonReporting(Globals.ParmValueEquals(WhereCriteria.CARBONEMISSIONS, "ON"), _isExcludeMileage, _lcCarbCalc);

            _isMetric = Globals.ParmValueEquals(WhereCriteria.METRIC, "ON");
            _cPoundsKilos = _isMetric ? "Kgs" : "Lbs.";
            _cMileKilo = _isMetric ? "Km" : "Mile";

            //more
            _cCarbCalcRptFtr = "";
            //more
            _llMileageTable = Globals.ParmValueEquals(WhereCriteria.MILEAGETABLE, "ON");
            _llCityPairByMetro = Globals.ParmValueEquals(WhereCriteria.CBCITYPAIRBYMETRO, "ON");
            _isRouteBidirectional = Globals.ParmValueEquals(WhereCriteria.RBFLTMKTONEWAYBOTHWAYS, "1");
            _llUseTickCnt = Globals.ParmValueEquals(WhereCriteria.CBCOUNTTKTSNOTSEGS, "ON");

            _cSubTitle = dataHelper.GetSubTitle(_isRouteBidirectional);

            SortBy = (DataTypes.SortBy)Convert.ToInt32(Globals.GetParmValue(WhereCriteria.SORTBY));
            Sort = (DataTypes.Sort)Convert.ToInt32(Globals.GetParmValue(WhereCriteria.RBSORTDESCASC));
            OrderBy = dataHelper.GetOrderBy(SortBy, _llUseTickCnt);

            var howMany = Globals.GetParmValue(WhereCriteria.HOWMANY) == "" ? "0" : Globals.GetParmValue(WhereCriteria.HOWMANY);
            nHowMany = topCityPairBasic.HowManyRecords(howMany, SortBy);

            CrystalReportName = topCityPairBasic.GetCrystalReportName(_llOnlineAdopt, _isExcludeMileage, _llUseTickCnt, _llCarbonRptg);

            IsReservationReport = GlobalCalc.IsReservationReport();

            UdidNumber = Globals.GetParmValue(WhereCriteria.UDIDNBR);
        }

        private void CarbonCalculations(string carbCalc)
        {
            RawDataList.ForEach(s => s.Miles = s.Plusmin * Math.Abs(s.Miles));    
            //will do the metric in the end, ignore here  
            var calcCarbon = new CarbonCalculator();
            calcCarbon.SetAirCarbon(RawDataList, false, carbCalc);
        }

        
        private ReportDocument SetPdfReportSource(string rptFilePath)
        {
            var reportSource = new ReportDocument();
            reportSource.Load(rptFilePath);

            reportSource.SetDataSource(FinalDataList);

            var missing = CrystalFunctions.SetupCrystalReport(reportSource, Globals);

            SetReportParameters(CrystalReportName, reportSource);
            return reportSource;
        }


        private void SetReportParameters(string report, ReportDocument ReportSource)
        {
            switch (report)
            {
                case "ibTopCityPairCarb":
                    ReportSource.SetParameterValue("cSubtitle", _cSubTitle);
                    ReportSource.SetParameterValue("nTotCnt", nTotCnt);
                    ReportSource.SetParameterValue("cMileKilo", _cMileKilo);
                    ReportSource.SetParameterValue("cPoundsKilos", _cPoundsKilos);
                break;
                case "ibTopCityPair2":
                case "ibTopCityPairOnlineAdopt":
                case "ibTopCityPairOnlineAdoptA":
                    ReportSource.SetParameterValue("cSubtitle", _cSubTitle);
                    ReportSource.SetParameterValue("nTotCnt", nTotCnt);
                    break;
                default:
                    ReportSource.SetParameterValue("cSubtitle", _cSubTitle);
                    ReportSource.SetParameterValue("nTotCnt", nTotCnt);
                    ReportSource.SetParameterValue("cMileKilo", _cMileKilo);
                    break;
            }
        }       
 
    }
}
