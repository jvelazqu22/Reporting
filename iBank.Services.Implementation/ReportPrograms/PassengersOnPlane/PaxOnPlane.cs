using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.PassengersOnPlaneReport;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using System;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.PassengersOnPlane
{

    public class PaxOnPlane : ReportRunner<RawData, FinalData>
    {
        private readonly PaxOnPlaneCalculations _calc = new PaxOnPlaneCalculations();

        private readonly PaxOnPlaneDataProcessor _processor = new PaxOnPlaneDataProcessor();

        private readonly PaxOnPlaneSqlCreator _creator = new PaxOnPlaneSqlCreator();

        private bool IsReservationReport { get; set; }
        private bool IsPrintBreakInfoInBodyOn { get; set; }
        private UserBreaks UserBreaks { get; set; }
        private bool AccountBreak { get; set; }
        private int NumberOfPassengers { get; set; }
        private string BreakColumnHeader { get; set; }

        public PaxOnPlane() {}

        public override bool InitialChecks()
        {
            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            return true;
        }
        
        public override bool GetRawData()
        {
            SetProperties();
            
            var holdDit = Globals.GetParmValue(WhereCriteria.DOMINTL);
            Globals.SetParmValue(WhereCriteria.DOMINTL, "1");

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

            //Return original parameter value
            Globals.SetParmValue(WhereCriteria.DOMINTL, holdDit);

            BuildWhere.AddSecurityChecks();

            var sql = _creator.Create(BuildWhere.WhereClauseFull, GlobalCalc.GetUdidNumber(), IsReservationReport);

            RawDataList = RetrieveRawData<RawData>(sql, IsReservationReport, true).ToList();

            if (!DataExists(RawDataList)) return false;

            if (!IsUnderOfflineThreshold(RawDataList)) return false;
            
            PerformCurrencyConversion(RawDataList);

            if (_calc.IsDateRange9(Globals)) RawDataList = RawDataList.Where(s => s.RDepDate <= Globals.EndDate && s.RArrDate >= Globals.BeginDate).ToList();

            var domesticInternational = _calc.GetDomesticInternationalValue(Globals);
            RawDataList = _processor.FilterForDomesticInternational(RawDataList, domesticInternational).ToList();
            Globals.WhereText += _calc.GetDomesticInternationalWhereText(domesticInternational);

            if (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstOrigin || BuildWhere.HasFirstDestination)
            {
                var isAppliedToLegData = GlobalCalc.IsAppliedToLegLevelData();

                RawDataList = isAppliedToLegData
                    ? BuildWhere.ApplyWhereRoute(RawDataList, true, true)
                    : BuildWhere.ApplyWhereRoute(RawDataList, true, false);
            }

            if (!DataExists(RawDataList)) return false;

            return true;
        }

        public override bool ProcessData()
        {
            FinalDataList = _processor.MapRawToFinalData(RawDataList, AccountBreak, UserBreaks, Globals).ToList();
            
            var groupedData = _processor.GroupFinalData(FinalDataList, NumberOfPassengers, Globals);
            
            var dateType = Convert.ToInt32(Globals.GetParmValue(WhereCriteria.DATERANGE));
            //remove flight data if it is out side of the date range
            if ((DateType)dateType == DateType.RoutingDepartureDate) _processor.SetDataRange(groupedData, Globals.BeginDate, Globals.EndDate);

            FinalDataList = _processor.MapGroupedDataToFinalData(groupedData, clientFunctions, MasterStore, ClientStore, Globals.Agency, Globals).ToList();

            if (!DataExists(FinalDataList)) return false;
            
            FinalDataList = _processor.OrderFinalData(FinalDataList, IsPrintBreakInfoInBodyOn).ToList();
            
            if (_calc.IsGantAgency(Globals.Agency))
            {
                var sql = _creator.GetGantSql(BuildWhere.WhereClauseFull, Globals);
                var gantData = RetrieveRawData<GantData>(sql, IsReservationReport, false);

                FinalDataList.ForEach(s =>
                {
                    s.Udidtext = _calc.GetUdid71(s.Reckey, gantData);
                });

                Globals.ReportTitle += _calc.GetGantReportTitle();
            }

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFieldList = _calc.GetExportFields(AccountBreak, UserBreaks, _calc.IsGantAgency(Globals.Agency), IsPrintBreakInfoInBodyOn).ToList();

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
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;
                    CreateReport(rptFilePath);
                    break;
            }
            
            return true;
        }

        private void CreateReport(string rptFilePath)
        {
            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(FinalDataList);

            var cPaxMsg = $"Only for flights with {NumberOfPassengers} or more passengers.";

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

            ReportSource.SetParameterValue("cPaxMsg", cPaxMsg);
            ReportSource.SetParameterValue("txtCredit", "Cred");
            ReportSource.SetParameterValue("lblDepDate", "Departure Date:");

            if (CrystalReportName.Right(1).EqualsIgnoreCase("2"))
                ReportSource.SetParameterValue("cBrkColHdr", BreakColumnHeader);

            CrystalFunctions.CreatePdf(ReportSource, Globals);
        }

        private void SetProperties()
        {
            IsReservationReport = GlobalCalc.IsReservationReport();
            Globals.AccountName = Globals.CompanyName;
            NumberOfPassengers = _calc.GetNumberOfPassengers(Globals);
            UserBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);
            AccountBreak = Globals.User.AccountBreak;
            IsPrintBreakInfoInBodyOn = _calc.IsPrintBreakInfoInBodyOn(Globals);
            CrystalReportName = _calc.GetCrystalReportName(IsPrintBreakInfoInBodyOn);
            BreakColumnHeader = "";

            if (_calc.IsIgnoreBreakSettingsOn(Globals))
            {
                AccountBreak = false;
                UserBreaks.UserBreak1 = false;
                UserBreaks.UserBreak2 = false;
                UserBreaks.UserBreak3 = false;
            }

            if (IsPrintBreakInfoInBodyOn) BreakColumnHeader = _calc.GetBreakColumnHeader(UserBreaks, Globals.User.Break1Name, Globals.User.Break2Name, Globals.User.Break3Name);
        }
    }
}
