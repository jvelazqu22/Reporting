using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models.ReportPrograms.AdvanceBookAir;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Linq;
using Domain.Orm.iBankMastersQueries.Other;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.AdvanceBookAir
{
    public class AdvanceBook : ReportRunner<RawData, FinalData>
    {
        private readonly AdvanceBookAirCalculations _calc = new AdvanceBookAirCalculations();

        private readonly AdvanceBookAirDataProcessor _processor = new AdvanceBookAirDataProcessor();

        private readonly AdvanceBookAirSqlCreator _creator = new AdvanceBookAirSqlCreator();
        public UserBreaks UserBreaks { get; set; }
        private bool IsReservationReport { get; set; }
        private bool IsSummaryPageOnly { get; set; }

        private void SetProperties()
        {
            IsReservationReport = GlobalCalc.IsReservationReport();
            UserBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);
            IsSummaryPageOnly = _calc.IsSummaryPageOnly(Globals);

            var baseChartOnTrips = _calc.IsBaseChartOnTrips(Globals);
            CrystalReportName = _calc.GetCrystalReportName(IsSummaryPageOnly, baseChartOnTrips);
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
            SetProperties();
            
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true, isRoutingBidirectional: false, legDit: false, ignoreTravel: false)) return false;
            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();
            
            int numberOfDays = _calc.GetNumberDaysInAdvance(Globals, _calc.IsRbInAdvanceRecordsEqualTwo(Globals));
            if (_calc.IsRbInAdvanceRecordsEqualTwo(Globals))
            {
                Globals.WhereText = _calc.AppendDayMessageToWhereTexgt(IsReservationReport, numberOfDays, Globals.WhereText);
            }

            var sql = _creator.Create(BuildWhere.WhereClauseFull, IsReservationReport, _calc.IsRbInAdvanceRecordsEqualTwo(Globals), numberOfDays, GlobalCalc.GetUdidNumber());

            RawDataList = RetrieveRawData<RawData>(sql, IsReservationReport, true).ToList();
            
            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            var isAppliedToLegData = GlobalCalc.IsAppliedToLegLevelData();

            if (isAppliedToLegData && (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstOrigin || BuildWhere.HasFirstDestination))
            {
                RawDataList = BuildWhere.ApplyWhereRoute(RawDataList, true);
            }

            RawDataList = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);

            if (!isAppliedToLegData && (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstOrigin || BuildWhere.HasFirstDestination))
            {
                RawDataList = BuildWhere.ApplyWhereRoute(RawDataList, true);
            }

            if (!DataExists(RawDataList)) return false;
            PerformCurrencyConversion(RawDataList);

            return true;
        }

        public override bool ProcessData()
        {
            FinalDataList =
                _processor.MapRawToFinalData(RawDataList, IsReservationReport, Globals.User.AccountBreak, clientFunctions, UserBreaks, Globals.Agency,
                    Globals, ClientStore, MasterStore).ToList();

            if (!DataExists(FinalDataList)) return false;

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = _calc.GetExportFields(_calc.IsDateRange1(Globals), UserBreaks, Globals.User.AccountBreak, Globals.User).ToList();
                    FinalDataList = _processor.ZeroOutFields(FinalDataList).ToList();
                    var buckets = FillBuckets();
                    FinalDataList = _processor.AddDataFromTimeBuckets(FinalDataList, buckets).ToList();

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, exportFields, Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, exportFields, Globals);
                    }
                    break;
                default:
                    var subReportData = BuildSubReport();

                    var colHead = Globals.ParmValueEquals(WhereCriteria.DATERANGE, "1") ? "Booked" : "Purchased";

                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    if (IsSummaryPageOnly)
                    {
                        ReportSource.SetDataSource(subReportData);
                    }
                    else
                    {
                        ReportSource.SetDataSource(FinalDataList);
                        ReportSource.Subreports[0].SetDataSource(subReportData);
                    }

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

                    SetParameterValues(subReportData, colHead, IsSummaryPageOnly);

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }

        private void SetParameterValues(IList<SubReportData> subReportData, string colHead, bool sumPageOnly)
        {
            //get the totals that are needed
            var totalTrips = subReportData.Sum(x => x.Trips);
            var totalCharges = subReportData.Sum(x => x.Totairchg);

            ReportSource.SetParameterValue("nTotCnt", totalTrips);
            ReportSource.SetParameterValue("nTotChg", totalCharges);
            if (!sumPageOnly) ReportSource.SetParameterValue("cColHead1", colHead);

        }
        
        private IList<SubReportData> BuildSubReport()
        {
            var handler = new SubReportHandler();
            var buckets = FillBuckets();
            FinalDataList = _processor.AddDataFromTimeBuckets(FinalDataList, buckets).ToList();

            var subReportList = handler.CreateSubReportCategories(buckets.ToList());

            return handler.FillSubReportCategories(subReportList, FinalDataList);
        }

        private IList<TimeBucket> FillBuckets()
        {
            var handler = new TimeBucketHandler();
            var fieldFunction = "ADVANCEDAYSBUCKETS";
            var advanceDayBucketsQuery = new GetClientExtrasByFieldFunctionAndAgencyQuery(MasterStore.MastersQueryDb, Globals.Agency, fieldFunction);
            return handler.FillTimeBuckets(FinalDataList, advanceDayBucketsQuery);
        }
    }
}
