using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;

using iBank.Services.Implementation.Shared;

using System;
using System.Linq;

using CODE.Framework.Core.Utilities.Extensions;

using Domain.Helper;
using Domain.Models.ReportPrograms.UpcomingPlans;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries;

using iBank.Server.Utilities.Helpers;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.UpcomingPlans
{

    public class UpcomingPlans : ReportRunner<RawData,FinalData>
    {
        private readonly UpcomingCalculations _calc = new UpcomingCalculations();

        private readonly UpcomingPlansData _processor = new UpcomingPlansData();
        private bool IsReservationReport { get
        {
            return true;
        } }
        
        public UserBreaks UserBreaks { get; set; }
        public bool AccountBreak { get; set; }

        private DateTime SundayDate { get; set; }

        public UpcomingPlans()
        {
        }

        public override bool InitialChecks()
        {
            if (!IsDateRangeValid()) return false;
            
            if (!IsUdidNumberSuppliedWithUdidText()) return false;
            
            if (!IsOnlineReport()) return false;

            return true;
        }

        private void SetProperties()
        {
            CrystalReportName = _calc.GetCrystalReportName();
            UserBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);
            AccountBreak = Globals.User.AccountBreak;
            SundayDate = _calc.GetSunday(Globals.BeginDate.Value);
            Globals.SetParmValue(WhereCriteria.PREPOST, "1");
        }

        public override bool GetRawData()
        {
            var creator = new UpcomingSqlCreator();

            SetProperties();
            
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true, isRoutingBidirectional: false, legDit: true, ignoreTravel: false))
            {
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
            
            var udid = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(0);
            var sql = creator.Create(BuildWhere.WhereClauseFull, udid);

            RawDataList = RetrieveRawData<RawData>(sql, IsReservationReport, true).ToList();
            
            if (!DataExists(RawDataList)) return false;

            RawDataList = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);
            RawDataList = GlobalCalc.IsAppliedToLegLevelData() ? BuildWhere.ApplyWhereRoute(RawDataList,true) : BuildWhere.ApplyWhereRoute(RawDataList,false);

            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold(RawDataList)) return false;
            
            
            return true;
        }

        public override bool ProcessData()
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            FinalDataList = _processor.MapRawToFinal(RawDataList, AccountBreak, clientFunctions, getAllMasterAccountsQuery,
                MasterStore, Globals, UserBreaks, SundayDate).ToList();
            
            if (!DataExists(FinalDataList)) return false;

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFieldList = _processor.GetExportFields(AccountBreak, UserBreaks, Globals.User);

                    if (Globals.OutputFormat == DestinationSwitch.Xls)
                    {
                        ExportHelper.ListToXlsx(FinalDataList, exportFieldList, Globals);
                    }
                    else
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, exportFieldList, Globals);
                    }
                    break;
                default:
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    //this is the path for the file we will create
                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);
                    
                    ReportSource.SetDataSource(FinalDataList);

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
                    ReportSource.SetParameterValue("SunDate", SundayDate);
                    
                    //generate a PDF file
                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }
    }
}
