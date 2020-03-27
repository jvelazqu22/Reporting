using CODE.Framework.Core.Utilities;
using Domain.Helper;
using Domain.Models.ReportPrograms.QuickSummaryByMonthReport;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth.Calculations;
using iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth.Factories;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.QuickSummaryByMonthReport;
using Domain.Orm.iBankClientQueries;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth
{
    public class QView1 : ReportRunner<AirRawData, ReportFinalData>
    {
        /*
         *  Quick Summary by Month report
         *  process key: 30
         */
         
        private string _defaultMonthAbbreviations = "Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec";

        private readonly QuickSummaryMonthCalculations _calculations = new QuickSummaryMonthCalculations();
        
        public IList<HotelRawData> HotelRawDataList = new List<HotelRawData>();

        public IList<CarRawData> CarRawDataList = new List<CarRawData>();
        
        public bool ExcludeExceptions { get; set; }

        private bool IsPreviewReport { get; set; }
        private bool IsGraphReportOutput { get; set; }

        public QView1() { }
        
        public override bool InitialChecks()
        {
            if (!IsGoodCombo()) return false;
            
            //if the begin date is empty and the end date is not set the begin date to the end date value
            Globals.BeginDate = DateConverter.ReplaceEmptyDate(Globals.BeginDate, Globals.EndDate);

            //if the end date is empty and the begin date is not set the end date to the begin date value
            Globals.EndDate = DateConverter.ReplaceEmptyDate(Globals.EndDate, Globals.BeginDate);

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            IsPreviewReport = Globals.ParmValueEquals(WhereCriteria.PREPOST, "1");

            ExcludeExceptions = Globals.IsParmValueOn(WhereCriteria.CBEXCLUDEEXCEPTNS);

            var outputType = Globals.GetParmValue(WhereCriteria.OUTPUTTYPE);
            IsGraphReportOutput = _calculations.IsGraphReportOutput(outputType);

            CrystalReportName = _calculations.GetCrystalReportName(ExcludeExceptions, IsGraphReportOutput);

            return true;
        }
        
        public override bool GetRawData()
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            var dateRangeType = GetDateComparison(Globals.GetParmValue(WhereCriteria.DATERANGE));
            var possibleUdidNumber = Globals.GetParmValue(WhereCriteria.UDIDNBR);
            
            RawDataList = GetAirRawData(dateRangeType, possibleUdidNumber).ToList();

            if (IsGraphReportOutput)
            {
                if (!DataExists(RawDataList)) return false;

                if (!IsUnderOfflineThreshold(RawDataList)) return false;

                PerformCurrencyConversion(RawDataList);

                return true;
            }
            
            CarRawDataList = GetCarRawData(dateRangeType, possibleUdidNumber);

            HotelRawDataList = GetHotelRawData(dateRangeType, possibleUdidNumber);

            if (!DataExists(RawDataList) 
                && !DataExists(CarRawDataList.ToList()) 
                && !DataExists(HotelRawDataList.ToList())) return false;

            var totalRecordCount = RawDataList.Count + CarRawDataList.Count + HotelRawDataList.Count;
            if (!IsUnderOfflineThreshold(totalRecordCount)) return false;

            PerformCurrencyConversion(RawDataList);
            PerformCurrencyConversion(CarRawDataList.ToList());
            PerformCurrencyConversion(HotelRawDataList.ToList());

            return true;
        }
        
        public override bool ProcessData()
        {
            var monthTranslationKey = "lt_AbbrMthsofYear";
            var monthAbbreviations = GetMonthAbbreviations(monthTranslationKey, Globals).ToList();
            
            var reasExclude = Globals.AgencyInformation.ReasonExclude.Split(',').ToList();

            // TODO: test to delete
            var test = RawDataList.Where(w => w.Depdate > new DateTime(2017, 2, 28)).ToList();
            var reportData = MapAirDataToReportFinalData(RawDataList, monthAbbreviations, reasExclude).ToList();

            if (!IsGraphReportOutput)
            {
                if (CarRawDataList.Any())
                {
                    var carReportData = MapCarDataToReportFinalData(CarRawDataList, reasExclude, monthAbbreviations);
                    reportData.AddRange(carReportData);
                }

                if (HotelRawDataList.Any())
                {
                    var hotelReportData = MapHotelDataToReportFinalData(HotelRawDataList, monthAbbreviations, reasExclude);
                    reportData.AddRange(hotelReportData);
                }
            }

            var processor = new FinalDataProcessor();
            FinalDataList = processor.FinalizeReportData(reportData).ToList();
            
            if (!DataExists(reportData)) return false;

            return true;
        }
        
        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = GetExportFields();

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
                    
                    if (IsGraphReportOutput) SetGraphReportSource(rptFilePath);
                    else SetPdfReportSource(rptFilePath);
                    
                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }

        private IList<AirRawData> GetAirRawData(string dateRangeType, string possibleUdidNumber)
        {
            var airRawDataFactory = new RawDataFactory<AirRawData>(DataTypes.DataType.Air, dateRangeType, possibleUdidNumber, IsPreviewReport,
                BuildWhere, Globals, ClientStore.ClientQueryDb);
            return airRawDataFactory.Build();
        }

        private IList<CarRawData> GetCarRawData(string dateRangeType, string possibleUdidNumber)
        {
            var carRawDataFactory = new RawDataFactory<CarRawData>(DataTypes.DataType.Car, dateRangeType, possibleUdidNumber, IsPreviewReport,
                    BuildWhere, Globals, ClientStore.ClientQueryDb);
            return carRawDataFactory.Build();
        }

        private IList<HotelRawData> GetHotelRawData(string dateRangeType, string possibleUdidNumber)
        {
            var hotelRawDataFactory = new RawDataFactory<HotelRawData>(DataTypes.DataType.Hotel, dateRangeType, possibleUdidNumber, IsPreviewReport,
                    BuildWhere, Globals, ClientStore.ClientQueryDb);
            return hotelRawDataFactory.Build();
        }

        private IList<string> GetExportFields()
        {
            var fieldList = new List<string>();

            fieldList.Add("rptyear");
            fieldList.Add("rptmonth");
            fieldList.Add("rptmthtext");
            fieldList.Add("airtrips");
            fieldList.Add("airvolume");
            fieldList.Add("carrents");
            fieldList.Add("cardays");
            fieldList.Add("carvolume");
            fieldList.Add("hotstays");
            fieldList.Add("hotnights");
            fieldList.Add("hotvolume");

            if (ExcludeExceptions) return fieldList;

            fieldList.Add("airsvgs");
            fieldList.Add("airexcepts");
            fieldList.Add("airlost");
            fieldList.Add("carexcepts");
            fieldList.Add("carlost");
            fieldList.Add("hotexcepts");
            fieldList.Add("hotlost");

            return fieldList;
        }

        private void SetGraphReportSource(string reportFilePath)
        {
            ReportSource.Load(reportFilePath);

            var finalDataProcessor = new FinalDataProcessor();
            var graphData = finalDataProcessor.ConvertToGraphData(FinalDataList);

            ReportSource.SetDataSource(graphData);

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

            //set the parameters
            var toTranslationKey = "ll_To";
            var toTranslation = LookupFunctions.LookupLanguageTranslation(toTranslationKey, "to", Globals.LanguageVariables);

            ReportSource.SetParameterValue("cCatDesc", "Month");
            ReportSource.SetParameterValue("cGrColHdr1", "Air Volume");
            ReportSource.SetParameterValue("cGrColHdr2", "Savings");
            ReportSource.SetParameterValue("cGrDataType", "C");

            var subtitle = string.Format("{0} {1} {2}", FinalDataList[0].RptMthText, toTranslation, FinalDataList.Last().RptMthText);
            ReportSource.SetParameterValue("cSubtitle", subtitle);
        }

        private void SetPdfReportSource(string reportFilePath)
        {
            ReportSource.Load(reportFilePath);
            ReportSource.SetDataSource(FinalDataList);

            var missing = CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
        }

        private static string GetDateComparison(string dateRangeType)
        {
            switch (dateRangeType)
            {
                case "3":
                    return "bookdate";
                case "2":
                    return "invdate";
                default:
                    return "depdate";
            }
        }

        private IEnumerable<string> GetMonthAbbreviations(string monthTranslationKey, ReportGlobals globals)
        {
            var months = LookupFunctions.LookupLanguageTranslation(monthTranslationKey, _defaultMonthAbbreviations, globals.LanguageVariables);

            return months.Split(',');
        }

        private static IList<ReportFinalData> MapAirDataToReportFinalData(IList<AirRawData> airRawData, IList<string> monthAbbreviations, IList<string> reasExclude)
        {
            var finalDataProcessor = new FinalDataProcessor();

            var airFinalData = finalDataProcessor.ConvertRawAirToFinalAir(airRawData, monthAbbreviations);
            airFinalData = finalDataProcessor.UpdateFinalAirData(airFinalData, reasExclude).ToList();

            return finalDataProcessor.CombineAirDataIntoReportData(airFinalData);
        }

        private static IList<ReportFinalData> MapCarDataToReportFinalData(IList<CarRawData> carRawData, IList<string> reasExclude, IList<string> monthAbbreviations)
        {
            var finalDataProcessor = new FinalDataProcessor();

            var carFinalData = finalDataProcessor.ConvertRawCarToFinalCarData(carRawData, monthAbbreviations, reasExclude);
            return finalDataProcessor.CombineCarDataIntoReportData(carFinalData);
        }

        private static IList<ReportFinalData> MapHotelDataToReportFinalData(IList<HotelRawData> hotelRawData, IList<string> monthAbbreviations, IList<string> reasExclude)
        {
            var finalDataProcessor = new FinalDataProcessor();

            var hotelData = finalDataProcessor.ConvertRawHotelToFinalHotelData(hotelRawData, monthAbbreviations, reasExclude);
            return finalDataProcessor.CombineHotelDataIntoReportData(hotelData);
        }
    }
}
