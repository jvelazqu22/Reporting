using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.CarrierConcentrationReport;
using Domain.Orm.iBankClientQueries;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.CarrierConcentration
{
    public class CarrierConcentration : ReportRunner<RawData, FinalData>
    {
        private readonly CarrierConcentrationSqlCreator _creator = new CarrierConcentrationSqlCreator();
        private readonly CarrierConcentrationDataProcessor _processor = new CarrierConcentrationDataProcessor();
        private readonly CarrierConcentrationCalculations _calc = new CarrierConcentrationCalculations();
        private string _segmentCarrier;
        private string _txtFlightSegments;
        private bool _useAirportCodes;
        private bool _bidirectional;

        public CarrierConcentration() { }

        private void SetProperties()
        {
            var suppressAvgDiff = Globals.IsParmValueOn(WhereCriteria.CBSUPPRESSAVGDIFF);
            var excludeSavings = Globals.IsParmValueOn(WhereCriteria.CBEXCLUDESVGS);
            _useAirportCodes = Globals.IsParmValueOn(WhereCriteria.CBUSEAIRPORTCODES);
            _txtFlightSegments = Globals.GetParmValue(WhereCriteria.TXTFLTSEGMENTS);
            _bidirectional = Globals.ParmValueEquals(WhereCriteria.RBFLTMKTONEWAYBOTHWAYS, "1");

            CrystalReportName = _calc.GetCrystalReportName(suppressAvgDiff, excludeSavings, _useAirportCodes);

            _segmentCarrier = Globals.GetParmValue(WhereCriteria.SEGCARR1);
        }

        public override bool InitialChecks()
        {
            SetProperties();

            if (!IsGoodCombo()) return false;

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;
            
            if (string.IsNullOrEmpty(_segmentCarrier))
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = "You must provide a Segment Carrier for the Carrier Concentration Report.";
                return false;
            }
            else
            {
                if (_segmentCarrier.Contains(","))
                {
                    Globals.ReportInformation.ReturnCode = 2;
                    Globals.ReportInformation.ErrorMessage = "You can only select 1 Segment Carrier for the Carrier Concentration Report.";
                    return false;
                }
            }
            return true;
        }

        public override bool GetRawData()
        {
            if (!string.IsNullOrEmpty(_txtFlightSegments)) RoutingCriteriaUtility.ClearRouteCriteria(BuildWhere.ReportGlobals);

            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true, isRoutingBidirectional: _bidirectional, legDit: true, ignoreTravel: false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            if (_bidirectional)
            {
                Globals.SetParmValue(WhereCriteria.FIRSTORIGIN, string.Empty);
                Globals.SetParmValue(WhereCriteria.FIRSTDEST, string.Empty);
            }

            var isReservationReport = GlobalCalc.IsReservationReport();
            var udidNumber = GlobalCalc.GetUdidNumber();
            
            var sql = _creator.Create(BuildWhere.WhereClauseFull, udidNumber, isReservationReport);
            RawDataList = RetrieveRawData<RawData>(sql, isReservationReport, true).ToList();

            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold(RawDataList)) return false;
            
            PerformCurrencyConversion(RawDataList);

            if (Globals.IsParmValueOn(WhereCriteria.MILEAGETABLE)) AirMileageCalculator<RawData>.CalculateAirMileageFromTable(RawDataList); 

            var segFareMileage = Globals.IsParmValueOn(WhereCriteria.SEGFAREMILEAGE);
            if (segFareMileage) FareByMileage<RawData>.CalculateFareByMileage(RawDataList);

            RawDataList = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);

            if (!segFareMileage) _processor.AllocateAirCharge(RawDataList);

            if (_bidirectional) _processor.SetConsistentCityPairs(RawDataList);

            _processor.SetFlightMarkets(RawDataList);

            if (BuildWhere.HasFirstOrigin || BuildWhere.HasFirstDestination)
            {
                RawDataList = GlobalCalc.IsAppliedToLegLevelData() ? BuildWhere.ApplyWhereRoute(RawDataList, true) : BuildWhere.ApplyWhereRoute(RawDataList, false);
            }
            else
            {
                //**04 / 11 / 2008 - IF FLIGHT MARKETS TREATED AS BI - DIRECTIONAL, THIS GETS MESSY. * *
                //**THE OLD METHOD DOESN'T WORK IF COUNTRIES AND/OR REGIONS ARE USED AS THE    **
                //* *CRITERIA.I.E., IF THE USER SELECTED USA AND INDIA, WITH BI-DIRECTIONAL,  **
                //**HE WOULD GET ALL DOMESTIC FLIGHTS IN THE USA AND IN INDIA, AS WELL AS**
                //** FLIGHTS BACK AND FORTH BETWEEN USA AND INDIA.                              * *
                //**SO WE NEED TO BASICALLY DO 2 PASSES-- ONE FOR EACH DIRECTION.             **
                //**DON'T HAVE TO WORRY ABOUT THIS IN THE ABOVE CONDITION, BECAUSE cWhe1stDest **
                //* *AND cWhe1stOrig WILL BE EMPTY IF BI-DIRECTIONAL.                           * *
                if (_bidirectional && BuildWhere.HasRoutingCriteria)
                {
                    //we really only want the record numbers here, and we don't want to change the original list. 
                    var firstPass = new List<RawData>(RawDataList);
                    var recnosFirstPass = GlobalCalc.IsAppliedToLegLevelData()
                        ? BuildWhere.ApplyWhereRoute(firstPass, true).Select(s => s.RecordNo)
                        : BuildWhere.ApplyWhereRoute(firstPass, false).Select(s => s.RecordNo);

                    _calc.SwapOriginsAndDestinations(Globals);

                    var recnosSecondPass = GlobalCalc.IsAppliedToLegLevelData()
                        ? BuildWhere.ApplyWhereRoute(firstPass, true).Select(s => s.RecordNo)
                        : BuildWhere.ApplyWhereRoute(firstPass, false).Select(s => s.RecordNo);

                    RawDataList = RawDataList.Where(s => recnosFirstPass.Contains(s.RecordNo) || recnosSecondPass.Contains(s.RecordNo)).ToList();

                }
                else
                {
                    RawDataList = GlobalCalc.IsAppliedToLegLevelData() ? BuildWhere.ApplyWhereRoute(RawDataList, true) : BuildWhere.ApplyWhereRoute(RawDataList, false);
                }
            }
            
            return true;
        }

        public override bool ProcessData()
        {
            var intermediaryData = RawDataList.ToIntermediaryData(_useAirportCodes, _segmentCarrier, Globals);
            
            //*WE DO NOT WANT TO SEE ROWS WHERE TOTAL SEGMENTS IS A NEGATIVE
            //*04 / 14 / 00 - BEFORE, WE DELETED ROWS WHERE TOTAL SEGMENTS < 1.
            //* THIS CAUSED A PROBLEM WITH THE REPORT TOTALS NOT MATCHING THOSE
            //*OF OTHER REPORTS BECAUSE DELETING THE ROWS REMOVED REPORT RECORDS
            //* WHERE THE NET EFFECT OF CREDITS CAUSED THE SEGMENTS COUNT TO BE NEGATIVE.
            //*NOW, JUST SET THE NUMBER OF SEGMENTS TO ZERO, AS WAS ALWAYS DONE
            //* FOR THE CARRIER SEGMENTS.


            //* *MARKET SEGMENTS CRITERIA APPLIED TO FLT_MKT**
            //** COLUMN -REQUIRES DIFFERENT PROCESSING.     **
            if (!string.IsNullOrEmpty(_txtFlightSegments))
            {
                intermediaryData = _processor.GetDataFilteredOnFlightSegments(intermediaryData, _txtFlightSegments, _bidirectional);
                Globals.WhereText += "Flight Markets = " + _txtFlightSegments;
            }

            intermediaryData = _processor.GetDataWithCarrierOneSegments(intermediaryData);

            var carr1Desc = LookupFunctions.LookupAline(MasterStore, _segmentCarrier, "A");

            var sortAscending = _calc.IsSortByAscending(Globals);
            var sortBy = Globals.GetParmValue(WhereCriteria.SORTBY);
            var useRecordLimit = _calc.UseRecordLimit(sortBy);
            var isExcelOrCsv = _calc.IsExcelOrCsvOutput(Globals);
            var howMany = Globals.GetParmValue(WhereCriteria.HOWMANY).TryIntParse(intermediaryData.Count);

            var sortedData = intermediaryData.ToSortedData(sortAscending, sortBy);
            
            FinalDataList = sortedData.ToFinalData(howMany, useRecordLimit, isExcelOrCsv, carr1Desc);

            if (!DataExists(FinalDataList)) return false;

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
                    var suppressAvgDiff = Globals.IsParmValueOn(WhereCriteria.CBSUPPRESSAVGDIFF);
                    var excludeSavings = Globals.IsParmValueOn(WhereCriteria.CBEXCLUDESVGS);
                    var sortInAscOrder = Globals.ParmValueEquals(WhereCriteria.RBSORTDESCASC, "2");

                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
                    ReportSource.SetParameterValue("lLogGen1", suppressAvgDiff);
                    ReportSource.SetParameterValue("lLogGen2", excludeSavings);
                    ReportSource.SetParameterValue("lLogGen3", sortInAscOrder);

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }
            return true;
        }
    }
}
