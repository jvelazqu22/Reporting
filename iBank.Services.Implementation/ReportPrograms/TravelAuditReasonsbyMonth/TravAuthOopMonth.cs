using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.TravelAuditReasonsbyMonthReport;
using Domain.Models.TravelAuditReasonsbyMonthReport;
using Domain.Orm.iBankClientQueries;
using iBank.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TravelAuditReasonsbyMonth
{
    public class TravAuthOopMonth : ReportRunner<RawData, FinalData>
    {
        private int _year;

        public List<SummaryFinalData> SummaryFinalDataList;

        public TravAuthOopMonth()
        {
            CrystalReportName = "ibTravAuthOOPMonth";
        }

        public override bool InitialChecks()
        {
            _year = TravAuthOopMonthHelpers.ProcessYear(Globals);
            if (_year == -1) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;
            if (!IsOnlineReport()) return false;

            if (Globals.IsParmValueOn(WhereCriteria.CBEXCLAPPRTVL) && Globals.IsParmValueOn(WhereCriteria.CBEXCLDECLINEDTVL)
                && Globals.IsParmValueOn(WhereCriteria.CBEXPIREDREQS) && Globals.IsParmValueOn(WhereCriteria.CBEXCLNOTIFONLY))
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.RptMsg_NoData;
                return false;
            }

            return true;
        }

        public override bool GetRawData()
        {
            Globals.SetParmValue(WhereCriteria.PREPOST, "1");
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            //if we apply routing in both directions "SpecRouting" is true. 
            BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false,
                buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true, isRoutingBidirectional: false,
                legDit: false, ignoreTravel: false);

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }
            BuildWhere.AddSecurityChecks();
            var udidNumber = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1);

            var sql = SqlBuilder.GetSql(udidNumber > 0, BuildWhere.WhereClauseFull);
            RawDataList = RetrieveRawData<RawData>(sql, GlobalCalc.IsReservationReport(), false, includeAllLegs: false, checkForDuplicatesAndRemoveThem: true).ToList();

            if (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstDestination || BuildWhere.HasFirstOrigin)
            {

                var sqlLegs = SqlBuilder.GetSqlLegs(udidNumber > 0, BuildWhere.WhereClauseFull);
                var legRawDataList = RetrieveRawData<LegRawData>(sqlLegs, GlobalCalc.IsReservationReport()).ToList();

                var segmentData = Collapser<LegRawData>.Collapse(legRawDataList, Collapser<LegRawData>.CollapseType.Both);
                segmentData = GlobalCalc.IsAppliedToLegLevelData() ? BuildWhere.ApplyWhereRoute(segmentData, true) : BuildWhere.ApplyWhereRoute(segmentData, false);
                legRawDataList = GetLegDataFromFilteredSegData(legRawDataList, segmentData);

                RawDataList.RemoveAll(s => !legRawDataList.Select(l => l.RecKey).Distinct().Contains(s.RecKey));

            }

            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            return true;
        }

        public override bool ProcessData()
        {
            RawDataList = TravAuthOopMonthHelpers.ProcessStatuses(RawDataList, Globals);

            RawDataList = TravAuthOopMonthHelpers.ProcessOutOfPolicy(RawDataList, Globals);

            var dateRange = Globals.GetParmValue(WhereCriteria.DATERANGE);
            var groupedData = RawDataList.Select(
                s => new { s.RecKey, s.Acct, s.OutPolCods, UseDate = TravAuthOopMonthHelpers.GetUseDate(s, dateRange), RecCntr = 1, s.AuthStatus })
                .GroupBy(s => new { s.RecKey, s.Acct, s.OutPolCods, s.UseDate, s.RecCntr }, (key, recs) => new GroupedData
                {
                    RecKey = key.RecKey,
                    Acct = key.Acct,
                    OutPolCods = key.OutPolCods,
                    UseDate = key.UseDate,
                    RecCntr = key.RecCntr,
                    AuthStatus = recs.Min(s => s.AuthStatus)
                }).ToList();

            groupedData = TravAuthOopMonthHelpers.SplitCodes(groupedData, Globals);

            if (!DataExists(groupedData)) return false;

            FinalDataList = TravAuthOopMonthHelpers.BuildMainReport(groupedData, _year, Globals, clientFunctions, ClientStore, MasterStore);

            SummaryFinalDataList = TravAuthOopMonthHelpers.BuildSummary(groupedData, _year, Globals);

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:

                    var exportFieldList = TravAuthOopMonthHelpers.GetExportFields();
                    FinalDataList = TravAuthOopMonthHelpers.CombineFinalAndSummary(FinalDataList, SummaryFinalDataList);
                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, exportFieldList, Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, exportFieldList, Globals);
                    }
                    break;
                default:

                    CreatePdf();
                    break;
            }
            return true;
        }

        private void CreatePdf()
        {
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(FinalDataList);
            ReportSource.Subreports[0].SetDataSource(SummaryFinalDataList);

            var missingParams = CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
            //Months are translated as a group. 
            var monthTranslation = LookupFunctions.LookupLanguageTranslation("lt_AbbrMthsofYear", "January,February,March,April,May,June,July,August,September,October,November,December", Globals.LanguageVariables);
            var months = monthTranslation.Split(',');
            ReportSource.SetParameterValue("XJAN", months[0]);
            ReportSource.SetParameterValue("XFEB", months[1]);
            ReportSource.SetParameterValue("XMAR", months[2]);
            ReportSource.SetParameterValue("XAPR", months[3]);
            ReportSource.SetParameterValue("XMAY", months[4]);
            ReportSource.SetParameterValue("XJUN", months[5]);
            ReportSource.SetParameterValue("XJUL", months[6]);
            ReportSource.SetParameterValue("XAUG", months[7]);
            ReportSource.SetParameterValue("XSEP", months[8]);
            ReportSource.SetParameterValue("XOCT", months[9]);
            ReportSource.SetParameterValue("XNOV", months[10]);
            ReportSource.SetParameterValue("XDEC", months[11]);

            CrystalFunctions.CreatePdf(ReportSource, Globals);
        }

    }
}
