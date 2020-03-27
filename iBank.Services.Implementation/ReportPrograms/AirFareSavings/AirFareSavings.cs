using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities;
using System.Linq;
using Domain.Models.ReportPrograms.AirFareSavingsReport;
using Domain.Orm.iBankClientQueries;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.AirFareSavings
{
    public class AirFareSavings : ReportRunner<RawData, FinalData>
    {
        private UserBreaks _userBreaks { get; set; }
        private bool _accountBreak { get; set; }
        private readonly ReportChecker _checker = new ReportChecker();
        private bool ApplyToSegment { get; set; }
        private bool _lExSavings;
        private bool _lExNegoSvgs;
        private bool _isPageSummaryOnly;
        private bool _useBaseFare;
        private bool _isPreview;
        private int _udidNumber;

        private Udid _udid = new Udid();
        private AirFareSavingsCalculations _afsCalculations = new AirFareSavingsCalculations();
        private AirFareSavingsData _data = new AirFareSavingsData();

        private void AddReportUdids()
        {
            //Builds a list of UDIDS to select for.
            //These are the UDIDs that come from the Report Settings
            _udid.AddReportSettingsUdidItem(Globals.GetParmValue(WhereCriteria.UDIDONRPT1).TryIntParse(0), Globals.GetParmValue(WhereCriteria.UDIDLBL1));
            _udid.AddReportSettingsUdidItem(Globals.GetParmValue(WhereCriteria.UDIDONRPT2).TryIntParse(0), Globals.GetParmValue(WhereCriteria.UDIDLBL2));
            _udid.AddReportSettingsUdidItem(Globals.GetParmValue(WhereCriteria.UDIDONRPT3).TryIntParse(0), Globals.GetParmValue(WhereCriteria.UDIDLBL3));
            _udid.AddReportSettingsUdidItem(Globals.GetParmValue(WhereCriteria.UDIDONRPT4).TryIntParse(0), Globals.GetParmValue(WhereCriteria.UDIDLBL4));
            _udid.AddReportSettingsUdidItem(Globals.GetParmValue(WhereCriteria.UDIDONRPT5).TryIntParse(0), Globals.GetParmValue(WhereCriteria.UDIDLBL5));
            _udid.AddReportSettingsUdidItem(Globals.GetParmValue(WhereCriteria.UDIDONRPT6).TryIntParse(0), Globals.GetParmValue(WhereCriteria.UDIDLBL6));
            _udid.AddReportSettingsUdidItem(Globals.GetParmValue(WhereCriteria.UDIDONRPT7).TryIntParse(0), Globals.GetParmValue(WhereCriteria.UDIDLBL7));
            _udid.AddReportSettingsUdidItem(Globals.GetParmValue(WhereCriteria.UDIDONRPT8).TryIntParse(0), Globals.GetParmValue(WhereCriteria.UDIDLBL8));
            _udid.AddReportSettingsUdidItem(Globals.GetParmValue(WhereCriteria.UDIDONRPT9).TryIntParse(0), Globals.GetParmValue(WhereCriteria.UDIDLBL9));
            _udid.AddReportSettingsUdidItem(Globals.GetParmValue(WhereCriteria.UDIDONRPT10).TryIntParse(0), Globals.GetParmValue(WhereCriteria.UDIDLBL10));
        }

        public AirFareSavings()
        {
            CrystalReportName = "ibFareSave";
        }

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
            _isPageSummaryOnly = Globals.IsParmValueOn(WhereCriteria.CBSUMPAGEONLY);
            _useBaseFare = Globals.IsParmValueOn(WhereCriteria.CBUSEBASEFARE);
            _isPreview = Globals.ParmValueEquals(WhereCriteria.PREPOST, "1");
            _udidNumber = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(0);
            _lExSavings = Globals.ParmValueEquals(WhereCriteria.CBEXCLPUBFARE, "ON");
            _lExNegoSvgs = Globals.ParmValueEquals(WhereCriteria.CBEXCLUDENEGOT, "ON");
            ApplyToSegment = _checker.IsAppliedToSegment(Globals);
        }

        public override bool GetRawData()
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, 
                buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            SetProperties();
            CrystalReportName = _afsCalculations.GetCrystalReportName(_isPageSummaryOnly, _lExSavings, _lExNegoSvgs);
            var sqlCreator = new AirFareSavingsSqlCreator();
            var sql = sqlCreator.CreateScript(BuildWhere.WhereClauseFull, _udidNumber, _isPreview, BuildWhere.WhereClauseUdid);
            RawDataList = RetrieveRawData<RawData>(sql, _isPreview, false).ToList();

            if (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstOrigin || BuildWhere.HasFirstDestination)
            {
                RawDataList = BuildWhere.ApplyWhereRoute(RawDataList, false);
            }

            if (!DataExists(RawDataList)) return false;

            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            if (!DataExists(RawDataList)) return false;

            PerformCurrencyConversion(RawDataList);

            return true;
        }

        public override bool ProcessData()
        {
            AddReportUdids();
            var reportSettingsUdidDataList = _udid.GetReportSettingsUdidData(_isPreview, ClientStore.ClientQueryDb, BuildWhere.Parameters, BuildWhere.WhereClauseFull, Globals);
            
            _userBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);

            var usePageBreakHomeCountry = Globals.IsParmValueOn(WhereCriteria.CBPGBRKHOMECTRY);
            
            if (usePageBreakHomeCountry) ResetBreaks();

            var options = new ReportOptions(_userBreaks, usePageBreakHomeCountry, Globals.User.AccountBreak, _useBaseFare, !Globals.IsParmValueOn(WhereCriteria.CBDONOTDERIVESVGCODE));

            FinalDataList = RawDataList.ToFinalData(MasterStore, ClientStore, Globals, options, clientFunctions)
                                        .Sort(options.UseDerivedSavingsCode);

            foreach (var finalData in FinalDataList)
            {
                _udid.SetUdids(reportSettingsUdidDataList, finalData, IsOutputXlsOrCsv(), _udid.ReportSettingsUdidInfoList);
            }

            if (!DataExists(FinalDataList)) return false;

            return true;
        }

        private bool IsOutputXlsOrCsv()
        {
            return Globals.OutputFormat == DestinationSwitch.Xls || Globals.OutputFormat == DestinationSwitch.Csv;
        }

        private void ResetBreaks()
        {
            Globals.User.AccountBreak = false;
            _userBreaks.UserBreak1 = false;
            _userBreaks.UserBreak2 = false;
            _userBreaks.UserBreak3 = false;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:

                    FinalDataList = _data.SuppressDuplicateFareInfo(FinalDataList);
                    var exportFields = _data.GetExportFields(Globals.User.AccountBreak, _userBreaks.UserBreak1, _userBreaks.UserBreak2, _userBreaks.UserBreak3,
                        Globals.User, FinalDataList);

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
                    var subReport = new AirFareSavingsSubReport();
                    if (!subReport.BuildSummaryData(clientFunctions, _lExSavings, _lExNegoSvgs, FinalDataList, Globals, _isPageSummaryOnly, ClientStore, MasterStore)) return false;
                    CreatePdf(subReport);
                    break;
            }

            return true;
        }

        private void CreatePdf(AirFareSavingsSubReport subReport)
        {
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            if (_isPageSummaryOnly)
            {
                ReportSource.SetDataSource(subReport.GetSubReportDataList());
            }
            else
            {
                ReportSource.SetDataSource(FinalDataList);
                ReportSource.Subreports[0].SetDataSource(subReport.GetSubReportDataList());
                ReportSource.SetParameterValue("LLOGGEN1", Globals.IsParmValueOn(WhereCriteria.CBINCLPCTLOSS));
            }

            if (_useBaseFare)
            {
                ReportSource.SetParameterValue("cColHead1", Globals.GetLanguageTranslation("xBaseFare", "Base Fare"));
            }
            else
            {
                ReportSource.SetParameterValue("cColHead1", Globals.GetLanguageTranslation("xPaidFare", "Paid Fare"));
            }
            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

            CrystalFunctions.CreatePdf(ReportSource, Globals);
        }

    }
}
