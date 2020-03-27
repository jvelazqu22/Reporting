using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using iBank.Services.Implementation.Shared;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

using Domain.Models.ReportPrograms.WeeklyTravelerActivity;
using Domain.Orm.iBankClientQueries;

using iBank.Repository.SQL.Repository;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.WeeklyTravelerActivity
{
    public class WeeklyTravelerActivity : ReportRunner<RawData, FinalData>
    {
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsPreview { get; set; }
        public int UdidNumber { get; set; }
        public List<UdidData> ReportSettingsUdidInfoList { get; set; }

        private CommaDelimitedStringCollection ReportSettingsUdidsString { get; set; }
        private readonly string[] DelimStrings = { DelimString };
        private const string CityAlert = "Y";
        private const string DelimString = "|";
        private UdidCalculator _udidCalculator = new UdidCalculator();
        private WeeklyTravelerActivityFinalDataCalculator _finalDataCalculator = new WeeklyTravelerActivityFinalDataCalculator();
        private WeeklyTravelerActivityData _weeklyTravelerActivitydata = new WeeklyTravelerActivityData();
        private WTAHotelData _wTAHotelData = new WTAHotelData();
        private WTAAirportRailAndLocationData _wTAAirportRailAndLocationData = new WTAAirportRailAndLocationData();

        public WeeklyTravelerActivity()
        {
            CrystalReportName = "ibWeeklyActivity";
            BeginDate = DateTime.Today;
            EndDate = DateTime.Today;
        }

        public override bool InitialChecks()
        {
            if (!IsBeginDateSupplied()) return false;
            
            // ReSharper disable once PossibleInvalidOperationException
            BeginDate = Globals.BeginDate.Value;
            EndDate = BeginDate.AddDays(6);
            Globals.EndDate = EndDate;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            SetProperties();

            return true;
        }

        private void SetProperties()
        {
            Globals.SetParmValue(WhereCriteria.DATERANGE, "9");
            IsPreview = GlobalCalc.IsReservationReport();
            var udid = Globals.GetParmValue(WhereCriteria.UDIDNBR);
            UdidNumber = udid.TryIntParse(0);
        }

        public override bool GetRawData()
        {
            var server = Globals.AgencyInformation.ServerName;
            var db = Globals.AgencyInformation.DatabaseName;
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(new iBankClientQueryable(server, db), Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true, isRoutingBidirectional: false, legDit: false, ignoreTravel: true))
                return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }
            BuildWhere.AddSecurityChecks();

            var sql = new WeeklyTravelerActivitySqlCreator().CreateScript(BuildWhere.WhereClauseFull, UdidNumber, IsPreview);
            RawDataList = RetrieveRawData<RawData>(sql, IsPreview, true).ToList();

            if (!DataExists(RawDataList)) return false;

            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            PerformCurrencyConversion(RawDataList);
            return true;
        }

        public override bool ProcessData()
        {
            var includeHotel = !Globals.IsParmValueOn(WhereCriteria.CBEXCLUDEHOTELINFO);
            var hotelDataList = new List<HotelData>();
            
            if (includeHotel)
            {
                hotelDataList = _wTAHotelData.GetHotelData(BuildWhere, UdidNumber, IsPreview, Globals, ClientStore.ClientQueryDb);
            }

            RawDataList = GlobalCalc.IsAppliedToLegLevelData() ? BuildWhere.ApplyWhereRoute(RawDataList, true) : BuildWhere.ApplyWhereRoute(RawDataList, false);
            if (!DataExists(RawDataList)) return false;

            AddReportUdids();

            var reportSettingsUdidDataList = _udidCalculator.GetReportSettingsUdidData(ReportSettingsUdidsString, IsPreview, Globals, BuildWhere, ClientStore.ClientQueryDb);
            FinalDataList = _finalDataCalculator.BuildReportData(hotelDataList, includeHotel, reportSettingsUdidDataList, Globals, RawDataList, BeginDate, ReportSettingsUdidInfoList, 
                ReportSettingsUdidsString, DelimStrings, DelimString, CityAlert);

            if (!DataExists(FinalDataList)) return false;
            CrystalReportName = _weeklyTravelerActivitydata.GetCrystalReportName(reportSettingsUdidDataList, Globals.IsParmValueOn(WhereCriteria.CBSUPPRPTBRKS));

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFieldList = _weeklyTravelerActivitydata.GetExportFields(ReportSettingsUdidsString, ReportSettingsUdidInfoList, BeginDate);

                    var exportData = _weeklyTravelerActivitydata.GetExportData(FinalDataList, ReportSettingsUdidInfoList);

                    if (Globals.OutputFormat == DestinationSwitch.Xls)
                    {
                        // Single macro: b1-4953F948-CA10-4AC8-14EB48D1E7074B5D_102_64644.keystonecf1
                        // Chained (2) macros: c1-5493BE1F-E105-ACB7-85C12B02097A6AE3_117_75925.keystonecf1
                        ExportHelper.ListToXlsx(exportData, exportFieldList, Globals);
                    }
                    else
                    {
                        ExportHelper.ConvertToCsv(exportData, exportFieldList, Globals);
                    }
                    break;
                default:
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);
                    ReportSource.SetDataSource(FinalDataList);

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
                    SetParameters();

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }
            return true;
        }

        private void SetParameters()
        {
            ReportSource.SetParameterValue("dBegDate", BeginDate);
            ReportSource.SetParameterValue("cDateDesc", BeginDate.Year == EndDate.Year
               ? @"Week of " + BeginDate.ToString("MMMM", CultureInfo.InvariantCulture) + " " + BeginDate.Day + " - " + EndDate.ToString("MMMM", CultureInfo.InvariantCulture) + " " + EndDate.Day + ", " + BeginDate.Year
               : @"Week of " + BeginDate.ToString("MMMM", CultureInfo.InvariantCulture) + " " + BeginDate.Day + ", " + BeginDate.Year + " - " + EndDate.ToString("MMMM", CultureInfo.InvariantCulture) + " " + EndDate.Day + ", " + EndDate.Year);
        }

        /// <summary>
        /// Fills ReportSettingsUdidsString and ReportSettingsUdidInfoList
        /// </summary>
        private void AddReportUdids()
        {
            ReportSettingsUdidsString = new CommaDelimitedStringCollection();
            ReportSettingsUdidInfoList = new List<UdidData>();
            //Builds a list of UDIDS to select for.
            //These are the UDIDs that come from the Report Settings
            _udidCalculator.AddReportSettingsUdidItem(Globals.GetParmValue(WhereCriteria.UDIDONRPT1).TryIntParse(0), Globals.GetParmValue(WhereCriteria.UDIDLBL1), ReportSettingsUdidInfoList, ReportSettingsUdidsString);
            _udidCalculator.AddReportSettingsUdidItem(Globals.GetParmValue(WhereCriteria.UDIDONRPT2).TryIntParse(0), Globals.GetParmValue(WhereCriteria.UDIDLBL2), ReportSettingsUdidInfoList, ReportSettingsUdidsString);
            _udidCalculator.AddReportSettingsUdidItem(Globals.GetParmValue(WhereCriteria.UDIDONRPT3).TryIntParse(0), Globals.GetParmValue(WhereCriteria.UDIDLBL3), ReportSettingsUdidInfoList, ReportSettingsUdidsString);
            _udidCalculator.AddReportSettingsUdidItem(Globals.GetParmValue(WhereCriteria.UDIDONRPT4).TryIntParse(0), Globals.GetParmValue(WhereCriteria.UDIDLBL4), ReportSettingsUdidInfoList, ReportSettingsUdidsString);
            _udidCalculator.AddReportSettingsUdidItem(Globals.GetParmValue(WhereCriteria.UDIDONRPT5).TryIntParse(0), Globals.GetParmValue(WhereCriteria.UDIDLBL5), ReportSettingsUdidInfoList, ReportSettingsUdidsString);
            _udidCalculator.AddReportSettingsUdidItem(Globals.GetParmValue(WhereCriteria.UDIDONRPT6).TryIntParse(0), Globals.GetParmValue(WhereCriteria.UDIDLBL6), ReportSettingsUdidInfoList, ReportSettingsUdidsString);
            _udidCalculator.AddReportSettingsUdidItem(Globals.GetParmValue(WhereCriteria.UDIDONRPT7).TryIntParse(0), Globals.GetParmValue(WhereCriteria.UDIDLBL7), ReportSettingsUdidInfoList, ReportSettingsUdidsString);
            _udidCalculator.AddReportSettingsUdidItem(Globals.GetParmValue(WhereCriteria.UDIDONRPT8).TryIntParse(0), Globals.GetParmValue(WhereCriteria.UDIDLBL8), ReportSettingsUdidInfoList, ReportSettingsUdidsString);
            _udidCalculator.AddReportSettingsUdidItem(Globals.GetParmValue(WhereCriteria.UDIDONRPT9).TryIntParse(0), Globals.GetParmValue(WhereCriteria.UDIDLBL9), ReportSettingsUdidInfoList, ReportSettingsUdidsString);
            _udidCalculator.AddReportSettingsUdidItem(Globals.GetParmValue(WhereCriteria.UDIDONRPT10).TryIntParse(0), Globals.GetParmValue(WhereCriteria.UDIDLBL10), ReportSettingsUdidInfoList, ReportSettingsUdidsString);
        }
    }
}