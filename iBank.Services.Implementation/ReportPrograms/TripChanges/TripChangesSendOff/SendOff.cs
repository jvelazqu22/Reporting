using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.TripChangesSendOffReport;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities;
using iBank.Services.Implementation.ReportPrograms.TripChanges.SharedClasses;
using iBank.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesSendOff
{
    public class SendOff : ReportRunner<RawData, FinalData>
    {
        public DateTime? BeginDate2 { get; set; }
        public DateTime? EndDate2 { get; set; }
        public bool IncludeSubTotalsByFlight { get; set; }
        public bool SuppressChangeDetails { get; set; }
        public bool ConsolidateChanges { get; set; }
        public bool UseAirportCodes { get; set; }
        public bool IncludeEmailAddress { get; set; }
        public int UdidNumber1 { get; set; }
        public int UdidNumber2 { get; set; }
        public string UdidLabel1 { get; set; }
        public string UdidLabel2 { get; set; }
        public List<UdidData> UdidOneData { get; set; }
        public List<UdidData> UdidTwoData { get; set; }
        public List<RawData> CancelledRawDataList { get; set; }
        public List<FinalData> CancelledFinalDataList { get; set; }
        public bool IncludeCancelledTrips { get; set; }
        public string ChangeStampWhereOnBeginEndDate { get; set; }
        public string SpecWhere { get; set; }
        public string DepartureDateTripWhere { get; set; }
        public string ChangeStampWhere { get; set; }
        public List<string> ExportFields = new List<string>();

        private readonly SendOffCalculations _calc = new SendOffCalculations();
        private readonly SendOffSqlCreator _creator = new SendOffSqlCreator();
        private readonly SendOffDataProcessor _processor = new SendOffDataProcessor();

        private readonly TripChangesCalculations _sharedCalc = new TripChangesCalculations();

        public SendOff(){}

        public override bool InitialChecks()
        {
            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsDateRangeValid()) return false;

            BeginDate2 = _sharedCalc.GetBeginDate2(Globals);
            EndDate2 = _sharedCalc.GetEndDate2(Globals);

            if (!_sharedCalc.IsDateRangeValid(BeginDate2, EndDate2, Globals)) return false;

            Globals.BeginDate = _sharedCalc.ReassignDate(Globals.BeginDate, Globals.EndDate);

            Globals.EndDate = _sharedCalc.ReassignDate(Globals.EndDate, Globals.BeginDate);

            BeginDate2 = _sharedCalc.ReassignDate(BeginDate2, EndDate2);

            EndDate2 = _sharedCalc.ReassignDate(EndDate2, BeginDate2);

            if (!IsDateRangeValid()) return false;

            if (!_sharedCalc.IsDateRangeValid(BeginDate2, EndDate2, Globals)) return false;

            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            SetReportParameters();
            Globals.SetParmValue(WhereCriteria.PREPOST, "1");
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);
            var udidExists = GlobalCalc.GetUdidNumber() > 0;
            var arrivalDateTripWhere = "";
            var isDepartureData = _calc.IsDepartureDateRange(Globals);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: !isDepartureData, inMemory: true, isRoutingBidirectional: false, legDit: true, ignoreTravel: false)) return false;
            if (isDepartureData)
            { 
                DepartureDateTripWhere = _creator.GetDepartureDateWhere(Globals.BeginDate.Value, Globals.EndDate.Value);
                arrivalDateTripWhere = _creator.GetArrivalDateWhere(Globals.BeginDate.Value, Globals.EndDate.Value);
            }

            //Build the where changes clause
            if (!BuildWhere.AddBuildWhereChanges())
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.RptMsg_InvalidChangeCodes;
                return false;
            }

            IncludeCancelledTrips = _calc.IncludeCancelledTrips(Globals, BuildWhere.WhereClauseChanges, BuildWhere.IncludeCancelled);

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }
            BuildWhere.AddSecurityChecks();

            var whereClauseOriginal = BuildWhere.WhereClauseFull + arrivalDateTripWhere;

            SpecWhere = string.IsNullOrWhiteSpace(BuildWhere.WhereClauseAdvanced)
                ? BuildWhere.WhereClauseFull
                : BuildWhere.WhereClauseFull + " AND " + BuildWhere.WhereClauseAdvanced;

            ChangeStampWhere = _creator.GetChangeStampWhere(Globals);
            var cancelStampWhere = _creator.GetCancelStampWhere(Globals, ChangeStampWhere);

            var rawDataSql = _creator.GetRawDataSql(whereClauseOriginal, udidExists);
            RawDataList = RetrieveRawData<RawData>(rawDataSql, true, true).ToList();

            if (!DataExists(RawDataList)) return false;

            RawDataList = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.First);

            if (UdidNumber1 > 0)
            {
                var udidOne = new UdidInformationRetriever(UdidNumber1, UdidLabel1);
                udidOne.SetUdidInformation(IncludeCancelledTrips, ClientStore.ClientQueryDb, BuildWhere, Globals, whereClauseOriginal);
                UdidLabel1 = udidOne.UdidLabel;
                UdidOneData = udidOne.UdidData;
            }

            if (UdidNumber2 > 0)
            {
                var udidTwo = new UdidInformationRetriever(UdidNumber2, UdidLabel2);
                udidTwo.SetUdidInformation(IncludeCancelledTrips, ClientStore.ClientQueryDb, BuildWhere, Globals, whereClauseOriginal);
                UdidLabel2 = udidTwo.UdidLabel;
                UdidTwoData = udidTwo.UdidData;
            }

            var beginHour = _calc.GetBeginHour(Globals);
            var beginMinute = _calc.GetBeginMinute(Globals);
            var beginAmpm = _calc.GetBeginAmOrPm(Globals);

            if (BeginDate2.HasValue && EndDate2.HasValue)
            {
                ChangeStampWhereOnBeginEndDate = _creator.GetChangeStampOnBeginEndDateWhereClause(BeginDate2.Value, EndDate2.Value, beginHour, beginMinute, beginAmpm);

                if (!string.IsNullOrEmpty(Globals.WhereText)) Globals.WhereText = Globals.WhereText + ";";

                Globals.WhereText += _calc.GetDateWhereText(beginHour, beginMinute, beginAmpm, BeginDate2.Value, EndDate2.Value);
            }

            if (IncludeCancelledTrips)
            {
                var whereClauseCancelled = rawDataSql.WhereClause + ChangeStampWhereOnBeginEndDate + cancelStampWhere;
                var cancelledSql = _creator.GetIncludeCancelledTripsSql(whereClauseCancelled, udidExists);
                CancelledRawDataList = RetrieveRawData<RawData>(cancelledSql, true, includeAllLegs: false).ToList();
                CancelledRawDataList = Collapser<RawData>.Collapse(CancelledRawDataList, Collapser<RawData>.CollapseType.First);
            }

            if (!DataExists(RawDataList) && !DataExists(CancelledRawDataList))
            {
                _calc.SetNoDataError(Globals);
                return false;
            }

            return true;
        }

        public override bool ProcessData()
        {
            //Apply Routing
            if (_calc.IsDepartureDateRange(Globals)) AddPaddingOnBeginAndEndDate();

            RawDataList = BuildWhere.ApplyWhereRoute(RawDataList, false, false);

            if (_calc.IsDepartureDateRange(Globals))
            {
                RemovePaddingFromBeginAndEndDate();
                RawDataList = _processor.GetDataWithArrivalBetweenDates(RawDataList, Globals.BeginDate.Value, Globals.EndDate.Value);
            }
            
            FinalDataList = _processor.MapRawToFinalData(RawDataList, UseAirportCodes, MasterStore, Globals);

            if (IncludeCancelledTrips)
            {
                if (_calc.IsDepartureDateRange(Globals)) AddPaddingOnBeginAndEndDate();

                CancelledRawDataList = BuildWhere.ApplyWhereRoute(CancelledRawDataList, false, false);

                if (_calc.IsDepartureDateRange(Globals))
                {
                    RemovePaddingFromBeginAndEndDate();
                    CancelledRawDataList = _processor.GetDataWithArrivalBetweenDates(CancelledRawDataList, Globals.BeginDate.Value, Globals.EndDate.Value);
                }

                CancelledFinalDataList = _processor.MapRawCancelledDataToFinalCancelledData(CancelledRawDataList, UseAirportCodes, SuppressChangeDetails,
                    MasterStore, Globals);
            }

            if (!DataExists(FinalDataList) && !DataExists(CancelledFinalDataList))
            {
                _calc.SetNoDataError(Globals);
                return false;
            }

            ChangeStampWhereOnBeginEndDate = _creator.GetChangeStampWhereClauseOnBeginEndDateForUseWithChangelog(ChangeStampWhereOnBeginEndDate);
            var whereChanges = _creator.GetWhereChangesClause(BuildWhere);

            var udidExists = GlobalCalc.GetUdidNumber() > 0;
            var excludeCancelledTrips = Globals.ParmValueEquals(WhereCriteria.CANCELCODE, "N");

            var changeLogSql = _creator.GetChangelogSql(udidExists, SpecWhere, whereChanges, ChangeStampWhereOnBeginEndDate, ChangeStampWhere,
                DepartureDateTripWhere, excludeCancelledTrips);
            var changes = RetrieveRawData<ChangesData>(changeLogSql, true, addFieldsFromLegsTable: false, includeAllLegs: false).ToList();

            var changeCodes = LookupFunctions.GetAllTripChangeCodes(MasterStore).Where(s => s.LanguageCode.EqualsIgnoreCase(Globals.UserLanguage));
            changes = _processor.CombineChangesAndChangeCodes(changes, changeCodes, SuppressChangeDetails, Globals);

            var tripsWithChanges = _processor.GetTripsWithChanges(FinalDataList, changes);

            var datesPresent = BeginDate2.HasValue && EndDate2.HasValue;
            var tripsWithNoChanges = _processor.GetTripsWithoutChanges(FinalDataList, tripsWithChanges, datesPresent, BeginDate2, SuppressChangeDetails);
            tripsWithChanges.AddRange(tripsWithNoChanges);

            if (IncludeCancelledTrips)
            {
                if (SuppressChangeDetails) tripsWithChanges.RemoveAll(s => CancelledFinalDataList.Any(t => t.Reckey == s.Reckey));
                tripsWithChanges.AddRange(CancelledFinalDataList);
            }

            FinalDataList = tripsWithChanges.ToList();

            if (!DataExists(FinalDataList)) return false;
            if (!IsUnderOfflineThreshold(FinalDataList)) return false;

            if (SuppressChangeDetails) FinalDataList = _processor.SuppressChangeDetails(FinalDataList);

            FinalDataList = _processor.SortFinalData(FinalDataList, Globals);
            FinalDataList = _processor.SetUdids(FinalDataList, UdidOneData, UdidTwoData, UdidNumber1, UdidNumber2);

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportDataList = new SendOffDataExportHandler().SetupFinalReportAndSetExportFields(ConsolidateChanges, FinalDataList, 
                        UseAirportCodes, IncludeEmailAddress, UdidLabel1, UdidLabel2, ref ExportFields);
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
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    //Create the ReportDocument object and load the .RPT File. 
                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

                    ReportSource.SetParameterValue("cUdidLbl1", UdidLabel1);
                    ReportSource.SetParameterValue("cUdidLbl2", UdidLabel2);
                    ReportSource.SetParameterValue("lLogGen1", IncludeSubTotalsByFlight);

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }

        private void SetReportParameters()
        {
            IncludeSubTotalsByFlight = Globals.IsParmValueOn(WhereCriteria.CBINCLSUBTOTSBYFLT);

            SuppressChangeDetails = Globals.IsParmValueOn(WhereCriteria.CBSUPPDETCHANGE);

            CrystalReportName = _calc.GetCrystalReportName(SuppressChangeDetails);

            ConsolidateChanges = Globals.IsParmValueOn(WhereCriteria.CBCONSOLIDATECHNGES);

            UseAirportCodes = Globals.IsParmValueOn(WhereCriteria.CBUSEAIRPORTCODES);

            IncludeEmailAddress = Globals.IsParmValueOn(WhereCriteria.CBINCLEMAILADDR);

            UdidLabel1 = Globals.GetParmValue(WhereCriteria.UDIDLBL1);
            UdidLabel2 = Globals.GetParmValue(WhereCriteria.UDIDLBL2);

            UdidNumber1 = _calc.GetUdidOnReport(Globals, WhereCriteria.UDIDONRPT1);
            UdidNumber2 = _calc.GetUdidOnReport(Globals, WhereCriteria.UDIDONRPT2);
        }

        private void RemovePaddingFromBeginAndEndDate()
        {
            Globals.BeginDate = Globals.BeginDate.Value.AddDays(45);
            Globals.EndDate = Globals.EndDate.Value.AddDays(-45);
        }

        private void AddPaddingOnBeginAndEndDate()
        {
            Globals.BeginDate = Globals.BeginDate.Value.AddDays(-45);
            Globals.EndDate = Globals.EndDate.Value.AddDays(45);
        }
    }

}
