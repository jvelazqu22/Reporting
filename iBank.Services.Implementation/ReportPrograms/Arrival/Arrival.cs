using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using iBank.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.ArrivalReport;
using Domain.Orm.iBankClientQueries;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Server.Utilities.Helpers;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.Arrival
{
    public class Arrival : ReportRunner<RawData, FinalData>
    {
        public bool FlightSort { get; set; }

        private IList<RawData> _rawDataContainingLegs;

        private readonly ArrivalCalculations _calc = new ArrivalCalculations();

        private readonly ArrivalDataProcessor _processor;

        private bool IncludePassengerCountByFlight { get; set; }
        private bool SortByName { get; set; }
        private bool IncludeAllLegs { get; set; }
        private bool UseConnectingLegs { get; set; }
        private bool IsReservationReport { get; set; }
        private bool UsePageBreak { get; set; }
        private UserBreaks UserBreaks { get; set; }

        public Arrival()
        {
            _processor = new ArrivalDataProcessor(clientFunctions);
        }

        private void SetProperties()
        {
            IncludeAllLegs = GlobalCalc.IncludeAllLegs();
            SortByName = _calc.SortByName(Globals);
            IncludePassengerCountByFlight = GlobalCalc.IncludePassengerCountByFlight();
            UseConnectingLegs = GlobalCalc.UseConnectingLegs();
            IsReservationReport = GlobalCalc.IsReservationReport();
            UsePageBreak = _calc.UseCrystalPageBreak(Globals, GlobalCalc.IncludePageBreakByDate());

            CrystalReportName = _calc.GetCrystalReportName(IncludeAllLegs);

            FlightSort = IncludePassengerCountByFlight && !IncludeAllLegs && !SortByName;
            UserBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);
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

            var specialWhere = string.Empty;
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);
            var rawCreator = new ArrivalRawDataSqlCreator();

            if (_calc.IsArrivalDateRangeSearch(Globals))
            {
                //** 08/15/2006 - THE 7TH PARAMETER DETERMINES WHETHER OR NOT TO BUILD **
                //** THE DATE PART OF THE WHERE CLAUSE.  TURN IT OFF HERE, SO WE CAN   **
                //** APPLY SPECIAL LOGIC.          
                if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: false, inMemory: true, isRoutingBidirectional: false, legDit: true, ignoreTravel: false)) return false;

                specialWhere = rawCreator.GetSpecialWhere(Globals.EndDate.Value, Globals.BeginDate.Value);
            }
            else
            {
                if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true, isRoutingBidirectional: false, legDit: true, ignoreTravel: false)) return false;
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

            var whereClause = BuildWhere.WhereClauseFull + specialWhere;
            var udid = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(0);
            var sql = rawCreator.CreateScript(udid, IsReservationReport, whereClause, UseConnectingLegs);
            
            RawDataList = RetrieveRawData<RawData>(sql, IsReservationReport, true).ToList();
            _rawDataContainingLegs = RawDataList;

            if (!DataExists(RawDataList)) return false;

            if (!UseConnectingLegs)
            {
                RawDataList = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);
            }

            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            if (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstOrigin || BuildWhere.HasFirstDestination)
            {
                var isAppliedToLegLevelData = GlobalCalc.IsAppliedToLegLevelData();
                RawDataList = isAppliedToLegLevelData ? BuildWhere.ApplyWhereRoute(RawDataList, true, false) : BuildWhere.ApplyWhereRoute(RawDataList, false, false);
            }

            AdvancedWhere<RawData>.ApplyAdvancedWhere(RawDataList, Globals.AdvancedParameters);

            if (_calc.IsArrivalDateRangeSearch(Globals))
            {
                RawDataList = RawDataList.Where(x => x.RArrDate >= Globals.BeginDate.Value
                                                     && x.RArrDate <= Globals.EndDate.Value).ToList();
            }
            
            if (!DataExists(RawDataList)) return false;
            
            return true;
        }

        public override bool ProcessData()
        {
            FinalDataList = _processor.ProcessRawIntoFinal(RawDataList, MasterStore, UserBreaks, Globals.User, FlightSort, SortByName, IncludeAllLegs, 
                IncludePassengerCountByFlight, UsePageBreak, ClientStore.ClientQueryDb, Globals).ToList();
            
            if (!IsReservationReport)
            {
                //if we have a matching invoice and credit, remove both. 
                var credits = FinalDataList.Where(s => s.PlusMin == -1).Select(s => s.Invoice.Trim());

                FinalDataList.RemoveAll(s => credits.Contains(s.Invoice.Trim()));
            }

            if (!DataExists(FinalDataList)) return false;
            
            if (IncludeAllLegs)
            {
                FinalDataList = _processor.CombineFinalDataWithLegData(FinalDataList, _rawDataContainingLegs, MasterStore, SortByName, Globals).ToList();
            }

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                    ExportHelper.ListToXlsx(FinalDataList, _calc.GetExportFields(UserBreaks, Globals.User.AccountBreak, Globals.User, IncludeAllLegs).ToList(), Globals);
                    break;
                case DestinationSwitch.Csv:
                    ExportHelper.ConvertToCsv(FinalDataList, _calc.GetExportFields(UserBreaks, Globals.User.AccountBreak, Globals.User, IncludeAllLegs).ToList(), Globals);

                    break;
                default:
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);
                    ReportSource.SetDataSource(FinalDataList);

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

                    ReportSource.SetParameterValue("lFltSort", FlightSort);
                    ReportSource.SetParameterValue("lPrevHist", Globals.ParmValueEquals(WhereCriteria.PREPOST, "1"));
                    ReportSource.SetParameterValue("pgBreak", UsePageBreak);


                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }
    }


}
