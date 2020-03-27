using CODE.Framework.Core.Utilities;
using com.ciswired.libraries.CISLogger;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Domain.Models.ReportPrograms.ClassOfServiceReport;
using Domain.Orm.iBankClientQueries;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.ClassOfService
{
    public class ClassofSvc : ReportRunner<RawData, FinalData>
    {
        private static readonly com.ciswired.libraries.CISLogger.ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ClassOfSvcDataProcessor _processor = new ClassOfSvcDataProcessor();

        private readonly ClassOfSvcSqlCreator _creator = new ClassOfSvcSqlCreator();

        private readonly ClassofSvcCalculations _calc = new ClassofSvcCalculations();
        public ClassofSvc()
        {
            SubReportDataList = new List<SubReportFinalData>();
        }

        public IList<SubReportFinalData> SubReportDataList { get; set; }
        public UserBreaks UserBreaks { get; set; }
        public bool AccountBreak { get; set; }
        public int UdidNumber { get; set; }
        public string BreakColumnHeader { get; set; }
        private bool IsReservationReport { get; set; }
        private string GroupBy { get; set; }
        private bool IsGroupByHomeCountry { get; set; }
        private bool IsGroupByAirline { get; set; }

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
            UdidNumber = GlobalCalc.GetUdidNumber();
            UserBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);
            AccountBreak = Globals.User.AccountBreak;
            IsReservationReport = GlobalCalc.IsReservationReport();
            GroupBy = GlobalCalc.GetGroupBy();
            IsGroupByAirline = GroupBy == "1";
            IsGroupByHomeCountry = GroupBy == "2";

            CrystalReportName = _calc.GetCrystalReportName(GlobalCalc.ShowBreakByDomesticInternational(), GroupBy);
        }

        public override bool GetRawData()
        {
            SetProperties();

            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: true, ignoreTravel: true)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            var sql = _creator.Create(BuildWhere.WhereClauseFull, UdidNumber, IsReservationReport);
            RawDataList = RetrieveRawData<RawData>(sql, IsReservationReport, false).ToList();

            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            PerformCurrencyConversion(RawDataList);

            if (GlobalCalc.IgnoreBreakSettings())
            {
                AccountBreak = false;
                UserBreaks.UserBreak1 = false;
                UserBreaks.UserBreak2 = false;
                UserBreaks.UserBreak3 = false;
            }

            var inHomeCtry = Globals.GetParmValue(WhereCriteria.INHOMECTRY).Trim();
            var notInHomeCtry = Globals.GetParmValue(WhereCriteria.NOTINHOMECTRY).Trim();

            var segFareMilege = Globals.IsParmValueOn(WhereCriteria.SEGFAREMILEAGE);

            var mileageTable = Globals.IsParmValueOn(WhereCriteria.MILEAGETABLE);
            if (mileageTable) AirMileageCalculator<RawData>.CalculateAirMileageFromTable(RawDataList); 

            if (segFareMilege) FareByMileage<RawData>.CalculateFareByMileage(RawDataList);

            RawDataList = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);

            if (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstOrigin || BuildWhere.HasFirstDestination)
            {
                RawDataList = BuildWhere.ApplyWhereRoute(RawDataList, true, false);
            }
            RawDataList = _processor.FilterByHomeCountry(RawDataList, inHomeCtry, notInHomeCtry, Globals).ToList();

            return true;
        }


        public override bool ProcessData()
        {
            var useClassCategory = GlobalCalc.UseClassCategory();
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            //get the final data
            FinalDataList = _processor.MapRawToFinalData(RawDataList, AccountBreak, IsGroupByAirline, IsGroupByHomeCountry, useClassCategory, Globals, UserBreaks, clientFunctions,
                    getAllMasterAccountsQuery, MasterStore).ToList();
            FinalDataList = _processor.MapGroupedDataToFinalData(FinalDataList).ToList();

            //get the carrier data
            var groupedFinalDataForCarrierClass = _processor.GroupFinalDataForCarrierClass(FinalDataList);
            var carrierClass = _processor.MapGroupedDataToCarrierClass(groupedFinalDataForCarrierClass);

            FinalDataList = _processor.ReplaceFinalDataItemsWithCarrierItems(FinalDataList, carrierClass.ToList()).ToList();

            if (!DataExists(FinalDataList)) return false;

            //get subreport data
            var tempSubReportData = _processor.MapFinalDataToSubReport(FinalDataList).ToList();
            SubReportDataList = _processor.MapGroupedDataToSubReportData(tempSubReportData);
            SubReportDataList = _processor.SumTotalSegData(SubReportDataList);

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = _calc.GetExportFields(AccountBreak, UserBreaks, IsGroupByAirline, IsGroupByHomeCountry).ToList();

                    if (Globals.OutputFormat == DestinationSwitch.Xls)
                    {
                        ExportHelper.ListToXlsx(FinalDataList, exportFields, Globals);
                    }
                    else
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, exportFields, Globals);
                    }
                    break;
                default:
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);
                    if (GroupBy != "3" && ReportSource.Subreports.Count > 0)
                    {
                        ReportSource.Subreports[0].SetDataSource(SubReportDataList);
                    }

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
                    if (CrystalReportName == "ibClassofSvcDIT" || CrystalReportName == "ibClassofSvcDIT2" || CrystalReportName == "ibClassofSvc2")
                    {
                        ReportSource.SetParameterValue("lblTotalClasses", "Total all classes");
                    }

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }
            return true;
        }
    }
}