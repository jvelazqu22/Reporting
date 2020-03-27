using System;
using System.Collections.Generic;
using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.TripChangesAir;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesAir
{
    public class TripChange : ReportRunner<RawData, FinalData>
    {
        public DateTime? BeginDate2 { get; set; }
        public DateTime? EndDate2 { get; set; }
        public bool LogGen1 { get; set; }
        public bool ConsolidateChanges { get; set; }
        //public List<RawData> CancelledRawDataList { get; set; }
        //public List<RawData> RoutingRawDataList { get; set; }
        public int RowCount { get; set; }
        public bool IncludeCancelledTrips { get; set; }
        public List<string> ExportFields { get; set; }

        private RawDataParams _rawDataParams;
        private TripChangeDataProcessor dataProcessor;
        public TripChange()
        {
            CrystalReportName = "ibTripChange";
            ExportFields = new List<string>();
        }

        public override bool InitialChecks()
        {
            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsDateRangeValid()) return false;

            BeginDate2 = Globals.GetParmValue(WhereCriteria.BEGDATE2).ToDateFromiBankFormattedString();
            EndDate2 = Globals.GetParmValue(WhereCriteria.ENDDATE2).ToDateFromiBankFormattedString();

            //* 12/22/2014 - V5 uses "CHANGESTAMP, CHANGESTAMP2" for  begdate3, enddate3
            if (!BeginDate2.HasValue)
            {
                if (!string.IsNullOrEmpty(Globals.GetParmValue(WhereCriteria.CHANGESTAMP)))
                    BeginDate2 = Globals.GetParmValue(WhereCriteria.CHANGESTAMP).ToDateFromiBankFormattedString();
                if (!string.IsNullOrEmpty(Globals.GetParmValue(WhereCriteria.CHANGESTAMP2)))
                    EndDate2 = Globals.GetParmValue(WhereCriteria.CHANGESTAMP2).ToDateFromiBankFormattedString();
            }

            if (BeginDate2.HasValue || EndDate2.HasValue)
            {
                if (BeginDate2.HasValue && EndDate2.HasValue && BeginDate2 > EndDate2)
                {
                    Globals.ReportInformation.ReturnCode = 2;
                    Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.RptMsg_DateRange;
                    return false;
                }
            }

            if (!Globals.BeginDate.HasValue && Globals.EndDate.HasValue) Globals.BeginDate = Globals.EndDate;

            if (Globals.BeginDate.HasValue && !Globals.EndDate.HasValue) Globals.EndDate = Globals.BeginDate;

            if (!BeginDate2.HasValue && EndDate2.HasValue) BeginDate2 = EndDate2;

            if (BeginDate2.HasValue && !EndDate2.HasValue) EndDate2 = BeginDate2;

            if (!Globals.EndDate.HasValue || !Globals.BeginDate.HasValue ||
                (Globals.BeginDate.Value > Globals.EndDate.Value))
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.RptMsg_DateRange;
                return false;
            }

            if (BeginDate2.HasValue && EndDate2.HasValue && (BeginDate2.Value > EndDate2.Value))
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.BadCompareTripChanges;
                return false;
            }

            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            SetReportParameters();

            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);
            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true, isRoutingBidirectional: false, legDit: true, ignoreTravel: false)) return false;

            //Build the where changes clause
            if (!BuildWhere.AddBuildWhereChanges())
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.RptMsg_InvalidChangeCodes;
                return false;
            }

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }
            BuildWhere.AddSecurityChecks();

            int udid;
            bool goodUdid = int.TryParse(Globals.GetParmValue(WhereCriteria.UDIDNBR).Trim(), out udid);
            int udidNumber = udid;

            IncludeCancelledTrips = string.IsNullOrEmpty(BuildWhere.WhereClauseChanges) || BuildWhere.IncludeCancelled;

            dataProcessor = new TripChangeDataProcessor(Globals, BuildWhere, goodUdid, udidNumber);
            RawDataList = dataProcessor.GetRawData(IncludeCancelledTrips);

            _rawDataParams = new RawDataHandler().SetUpRawDataLists(Globals, RawDataList, MasterStore, IncludeCancelledTrips, dataProcessor);
            RawDataList = _rawDataParams.RawDataList;

            if (!DataExists(RawDataList)) return false;

            PerformCurrencyConversion(RawDataList);
            PerformCurrencyConversion(_rawDataParams.RoutingRawDataList);

            return true;
        }

        public override bool ProcessData()
        {
            FinalDataList = new FinalDataHandler().GetFinalDataLists(Globals, RawDataList, MasterStore, _rawDataParams, BuildWhere, GlobalCalc, clientFunctions, ConsolidateChanges);

            if (!DataExists(FinalDataList)) return false;

            if (!IsUnderOfflineThreshold(FinalDataList)) return false;

            return true;
        }
        
        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    //var exportDataList = SetupFinalReportAndSetExportFields();
                    var exportDataList = new FinalDataExportHandler().SetupFinalReportAndSetExportFields(FinalDataList, ConsolidateChanges, ExportFields);
                    if (Globals.OutputFormat == DestinationSwitch.Xls)
                    {
                        ExportHelper.ListToXlsx(exportDataList, ExportFields, Globals);
                    }
                    else
                    {
                        ExportHelper.ConvertToCsv(exportDataList, ExportFields, Globals);
                    }
                    break;
                default:
                    PrintReport();
                    break;
            }

            return true;
        }

        private void PrintReport()
        {
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            //Create the ReportDocument object and load the .RPT File. 
            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);
            ReportSource.SetDataSource(FinalDataList);
            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
            CrystalFunctions.CreatePdf(ReportSource, Globals);
        }

        /// <summary>
        /// Set the common report parameters
        /// </summary>
        private void SetReportParameters()
        {
            LogGen1 = Globals.IsParmValueOn(WhereCriteria.CBINCLSUBTOTSBYFLT);
            ConsolidateChanges = Globals.IsParmValueOn(WhereCriteria.CBCONSOLIDATECHNGES);
            Globals.SetParmValue(WhereCriteria.PREPOST, "1");
        }
    }
}
