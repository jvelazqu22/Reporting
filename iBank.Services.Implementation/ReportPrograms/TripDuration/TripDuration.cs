using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.TripDurationReport;
using Domain.Orm.iBankClientQueries;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.TripDuration
{
    public class TripDuration : ReportRunner<RawData, FinalData>
    {
        public bool AccountBreak { get; set; }
        public UserBreaks UserBreaks { get; set; }

        public TripDuration()
        {
            CrystalReportName = "ibTripDuration";
        }

        public override bool InitialChecks()
        {
            if (!IsDateRangeValid()) return false;
            
            if (!IsUdidNumberSuppliedWithUdidText()) return false;
            
            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true, isRoutingBidirectional: false, legDit: false, ignoreTravel: true)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }
            BuildWhere.AddSecurityChecks();

            var whereClause = string.IsNullOrWhiteSpace(BuildWhere.WhereClauseAdvanced)
                ? BuildWhere.WhereClauseFull
                : BuildWhere.WhereClauseFull + " AND " + BuildWhere.WhereClauseAdvanced;

            var udidNumber = GlobalCalc.GetUdidNumber();
            var isReservation = GlobalCalc.IsReservationReport();
            var sql = new TripDurationSqlCreator().Create(whereClause, udidNumber, isReservation);

            RawDataList = RetrieveRawData<RawData>(sql, isReservation, true).ToList();

            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold(RawDataList)) return false;
            
            PerformCurrencyConversion(RawDataList);

            return true;
        }

        public override bool ProcessData()
        {
            UserBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);
            if (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstOrigin || BuildWhere.HasFirstDestination)
            {
                if (GlobalCalc.IsAppliedToLegLevelData())
                {
                    RawDataList = GlobalCalc.IsAppliedToLegLevelData() ? BuildWhere.ApplyWhereRoute(RawDataList,true) : BuildWhere.ApplyWhereRoute(RawDataList,false);
                }
                else
                {
                    var segmentData = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);
                    segmentData = GlobalCalc.IsAppliedToLegLevelData() ? BuildWhere.ApplyWhereRoute(segmentData,true) : BuildWhere.ApplyWhereRoute(segmentData,false);
                    RawDataList = GetLegDataFromFilteredSegData(RawDataList, segmentData);
                }
            }

            FinalDataList = new FinalDataHandler().GetFinalList(Globals, RawDataList, ClientStore,clientFunctions, UserBreaks);
            //Check for no data
            if (!FinalDataList.Any())
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.RptMsg_NoData;
                return false;
            }

            if (!IsUnderOfflineThreshold(FinalDataList)) return false;

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = new TripDurationCalculations(). GetExportFields(Globals.User.AccountBreak, UserBreaks.UserBreak1, UserBreaks.UserBreak2, UserBreaks.UserBreak3, Globals.User.Break1Name, Globals.User.Break2Name, Globals.User.Break3Name);

                    FinalDataList = ZeroOut<FinalData>.Process(FinalDataList, new List<string> { "airchg" });
                    if (Globals.OutputFormat == DestinationSwitch.Xls)
                        ExportHelper.ListToXlsx(FinalDataList, exportFields, Globals);
                    else
                        ExportHelper.ConvertToCsv(FinalDataList, exportFields, Globals);
                    break;

                default:
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;
                    //Create the ReportDocument object and load the .RPT File. 
                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);
                    //set the datasource to auto-generated fake data
                    ReportSource.SetDataSource(FinalDataList);

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

                    //Generate a PDF file. 
                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }
    }
}
