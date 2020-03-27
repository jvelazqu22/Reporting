using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.DeparturesReport;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.AdvancedClause;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.Departures
{
    public class Depart : ReportRunner<RawData, FinalData>
    {
        private readonly DepartDataProcessor _processor = new DepartDataProcessor();

        private readonly DepartCalculations _calc = new DepartCalculations();
        private List<RawData> _rawDataContainingLegs;

        private bool IsReservationReport { get; set; }
        private bool UseConnectingLegs { get; set; }
        private bool IncludePassengerCountByFlight { get; set; }
        private bool SortByName { get; set; }
        private bool FlightSort { get; set; }
        private bool IncludeAllLegs { get; set; }
        private bool UsePageBreak { get; set; }
        private UserBreaks UserBreaks { get; set; }

        public Depart() {}
        
        public override bool InitialChecks()
        {
            if (!IsGoodCombo()) return false;

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            return true;
        }

        private void SetProperties()
        {
            UseConnectingLegs = GlobalCalc.UseConnectingLegs();
            IsReservationReport = GlobalCalc.IsReservationReport();
            IncludePassengerCountByFlight = GlobalCalc.IncludePassengerCountByFlight();
            SortByName = _calc.SortByName(Globals);
            IncludeAllLegs = GlobalCalc.IncludeAllLegs();
            UsePageBreak = _calc.UseCrystalPageBreak(Globals, GlobalCalc.IncludePageBreakByDate());

            CrystalReportName = _calc.GetCrystalReportName(IncludeAllLegs);

            FlightSort = IncludePassengerCountByFlight && !IncludeAllLegs && !SortByName;
            UserBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);

        }

        public override bool GetRawData()
        {
            SetProperties();

            var creator = new DepartRawDataSqlCreator();
            var specialWhere = string.Empty;
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);
            
            if (_calc.IsDepartureDateRangeSearch(Globals))
            {
                if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere:true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: false, inMemory: true, isRoutingBidirectional: false, legDit: true, ignoreTravel: false)) return false;

                specialWhere = creator.GetSpecialWhere(Globals.EndDate.Value, Globals.BeginDate.Value);
            }
            else
            {
                if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, true, true, false, false, true, true, true, false, true, false)) return false;
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
            var sql = creator.CreateScript(udid, IsReservationReport, whereClause, UseConnectingLegs);

            RawDataList = RetrieveRawData<RawData>(sql, IsReservationReport, true).ToList();

            //Re-Adjust departure window.
            if ((DateType)Convert.ToInt32(Globals.GetParmValue(WhereCriteria.DATERANGE)) == DateType.RoutingDepartureDate)
            {
                ReAdjustDepartureData(udid, creator);
            }
            
            if (!DataExists(RawDataList)) return false;

            _rawDataContainingLegs = RawDataList.ToList();

            if (!UseConnectingLegs)
            {
                RawDataList = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.First);
            }

            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            if (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstOrigin || BuildWhere.HasFirstDestination)
            {
                RawDataList = BuildWhere.ApplyWhereRoute(RawDataList, GlobalCalc.IsAppliedToLegLevelData(), false);
            }

            AdvancedWhere<RawData>.ApplyAdvancedWhere(RawDataList, Globals.AdvancedParameters);

            if (!DataExists(RawDataList)) return false;

            return true;
        }

        public override bool ProcessData()
        {
            FinalDataList = _processor.ProcessRawIntoFinal(RawDataList, MasterStore, Globals.User, UserBreaks, FlightSort, SortByName, IncludeAllLegs, IncludePassengerCountByFlight, UsePageBreak, Globals).ToList();
            
            if (!IsReservationReport)
            {
                //if we have a matching invoice and credit, remove both.
                var credits = FinalDataList.Where(s => s.PlusMin == -1).Select(s => s.Invoice.Trim()).Distinct().ToList();

                FinalDataList.RemoveAll(s => credits.Contains(s.Invoice.Trim()));
            }

            if (!DataExists(FinalDataList)) return false;
            
            if (IncludeAllLegs)
            {
                FinalDataList = _processor.CombineFinalDataWithLegData(FinalDataList, _rawDataContainingLegs, MasterStore, SortByName, Globals).ToList();
            }

            FinalDataList = _processor.SortData(FinalDataList, SortByName, IncludePassengerCountByFlight, IncludeAllLegs).ToList();
            
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
                    ReportSource.SetParameterValue("pgBreak", UsePageBreak);

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }
            return true;
        }

        private void ReAdjustDepartureData(int udid, DepartRawDataSqlCreator creator)
        {
            //Remove the legs that are not within rdepart window.
            RawDataList = RawDataList.Where(x => x.RDepDate >= Globals.BeginDate && x.RDepDate <= Globals.EndDate).ToList();

            //Add missing legs that are not already included in the RawDataList even if the search window has been expended.
            //example case: search begin and end date: 7/29/2017
            //trip starts at 5/30/2017, and return trip is on 7/29/2017
            //above search covers depdate >='6/29/2017 12:00:00 AM' and depdate <= '8/28/2017 11:59:59 PM' 
            //so we need to use rdepdate with select begin and end date window on legs.  
            var cWhereRout = BuildWhere.WhereClauseFull.Replace("depdate", "rdepdate").Replace("@t1BeginDate",$"'{Globals.BeginDate}'").Replace("@t1EndDate", $"'{Globals.EndDate}'");
            var sql = creator.CreateScript(udid, IsReservationReport, cWhereRout, UseConnectingLegs);
            var rawDataList = RetrieveRawData<RawData>(sql, IsReservationReport, false).ToList();
            var notInRawDataList = rawDataList.Where(x => !RawDataList.Select(s => s.RecKey).Contains(x.RecKey)).ToList();

            //Add records that exist in legs but not already in RawDataList
            RawDataList.AddRange(notInRawDataList);
        }
    }
}