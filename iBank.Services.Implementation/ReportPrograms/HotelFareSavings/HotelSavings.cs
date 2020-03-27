using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models.ReportPrograms.HotelFareSavings;
using Domain.Orm.Classes;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities;
using System.Collections.Generic;
using System.Linq;
using Domain.Services;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.HotelFareSavings
{
    public class HotelSavings : ReportRunner<RawData, FinalData>
    {
        private bool _summaryOnly;
        private UserBreaks _userBreaks;

        public List<SubReportData> SubReportList { get; set; }

        private List<KeyValue> _homeCountries;

        public HotelSavings()
        {
            CrystalReportName = "ibHotelSavings";
            SubReportList = new List<SubReportData>();
            _homeCountries = new List<KeyValue>();
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
            var server = Globals.AgencyInformation.ServerName;
            var db = Globals.AgencyInformation.DatabaseName;
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);
            BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: true, buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false);

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            var hasUdid = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1) > 0;
            var sqlScript = SqlBuilder.GetSql(hasUdid, GlobalCalc.IsReservationReport(), BuildWhere.WhereClauseFull);

            RawDataList = RetrieveRawData<RawData>(sqlScript, GlobalCalc.IsReservationReport(), false, false).ToList();
            RawDataList = PerformCurrencyConversion(RawDataList);

            return true;
        }

        public override bool ProcessData()
        {
            var homeCountryBreak = Globals.IsParmValueOn(WhereCriteria.CBPGBRKHOMECTRY);

            _userBreaks = SharedProcedures.SetUserBreaks(BuildWhere.ReportGlobals.User.ReportBreaks);
            if (homeCountryBreak)
            {
                Globals.User.AccountBreak = false;
                _userBreaks.UserBreak1 = false;
                _userBreaks.UserBreak2 = false;
                _userBreaks.UserBreak3 = false;
            }

            var deriveSavingCode = !Globals.IsParmValueOn(WhereCriteria.CBDONOTDERIVESVGCODE);
            
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            var hotelSavingsFinalData = new HotelSavingsFinalData();
            FinalDataList = hotelSavingsFinalData.GetFinalDataList(Globals, getAllMasterAccountsQuery, deriveSavingCode, RawDataList, 
                homeCountryBreak, _userBreaks, MasterStore, clientFunctions);

            var homeCountry = Globals.GetParmValue(WhereCriteria.HOMECTRY);
            var inHomeCountry = Globals.GetParmValue(WhereCriteria.INHOMECTRY);
            var notIn = Globals.IsParmValueOn(WhereCriteria.NOTINHOMECTRY);

            if (homeCountryBreak || !string.IsNullOrEmpty(homeCountry) || !string.IsNullOrEmpty(inHomeCountry))
            {
                _homeCountries = HomeCountriesLookup.GetHomeCountries(new CacheService(), MasterStore.MastersQueryDb, Globals.ClientType, Globals.Agency);
            }

            if (!string.IsNullOrEmpty(homeCountry) || !string.IsNullOrEmpty(inHomeCountry))
            {
                var homeCountries = (homeCountry + inHomeCountry).Split(',');
                var sourceAbbrs = notIn
                    ? _homeCountries.Where(s => !homeCountries.Contains(s.Value)).Select(s => s.Key.Trim())
                    : _homeCountries.Where(s => homeCountries.Contains(s.Value)).Select(s => s.Key.Trim());
                FinalDataList = FinalDataList.Where(s => sourceAbbrs.Contains(s.Sourceabbr.Trim())).ToList();
            }

            if (!DataExists(FinalDataList)) return false;

            if (!IsUnderOfflineThreshold(FinalDataList)) return false;

            FinalDataList = hotelSavingsFinalData.UpdateFinalDataList(FinalDataList, Globals, deriveSavingCode);

            var udids = UdidLoader.GatherUdids(Globals);
            if(udids.Any())
                UdidLoader.LoadUdids(FinalDataList, this, SqlBuilder.GetUdidSqlScript(BuildWhere.WhereClauseFull, GlobalCalc.IsReservationReport(), udids));

            var excludeSavings = Globals.IsParmValueOn(WhereCriteria.CBEXCLPUBFARE);
            if (excludeSavings)
                CrystalReportName += "2";

            //build summary data
            HotelSavingsSummaryData hotelFareSavingsSummaryData = new HotelSavingsSummaryData();
            var savingsCodes = hotelFareSavingsSummaryData.GetSavingCodes(excludeSavings, FinalDataList, clientFunctions, getAllMasterAccountsQuery, Globals, ClientStore, MasterStore);
            var lossCodes = hotelFareSavingsSummaryData.GetLossCodes(FinalDataList, clientFunctions, getAllMasterAccountsQuery, Globals, ClientStore, MasterStore);

            _summaryOnly = Globals.IsParmValueOn(WhereCriteria.CBSUMPAGEONLY);

            if (_summaryOnly && !savingsCodes.Any() && !lossCodes.Any())
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.RptMsg_NoData;
                return false;
            }

            SubReportList = hotelFareSavingsSummaryData.AddSummaryData(SubReportList, savingsCodes, lossCodes);

            return true;
        }

        public override bool GenerateReport()
        {
            if (_summaryOnly)
                CrystalReportName = "ibHotelSavingSum";

            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFieldList = new HotelSavingsHelper().GetExportFields(Globals.User.AccountBreak, _userBreaks, Globals);

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                        ExportHelper.ConvertToCsv(FinalDataList, exportFieldList, Globals);
                    else
                        ExportHelper.ListToXlsx(FinalDataList, exportFieldList, Globals);
                    break;
                default:

                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;
                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    if (_summaryOnly)
                        ReportSource.SetDataSource(SubReportList);
                    else
                    {
                        ReportSource.SetDataSource(FinalDataList);
                        ReportSource.Subreports[0].SetDataSource(SubReportList);
                    }

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
                    SetUpParameters();

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }

        private void SetUpParameters()
        {
            if (!_summaryOnly)
            {
                var lossAmount = Globals.LanguageVariables.FirstOrDefault(s => s.VariableName.Equals("xLossAmount"));
                if (lossAmount == null)
                {
                    ReportSource.SetParameterValue("xLossAmount1", "Loss");
                    ReportSource.SetParameterValue("xLossAmount2", "Amount");
                }
                else
                {
                    var lossAmountPieces = lossAmount.Translation.Replace("<br>", "|").Split('|');
                    if (lossAmountPieces.Length != 2)
                    {
                        ReportSource.SetParameterValue("xLossAmount1", "Loss");
                        ReportSource.SetParameterValue("xLossAmount2", "Amount");
                    }
                    else
                    {
                        ReportSource.SetParameterValue("xLossAmount1", lossAmountPieces[0]);
                        ReportSource.SetParameterValue("xLossAmount2", lossAmountPieces[1]);
                    }
                }
                ReportSource.SetParameterValue("lLogGen1", Globals.IsParmValueOn(WhereCriteria.CBINCLPCTLOSS));
                ReportSource.SetParameterValue("xHomeCountry", "Home Country");
            }
        }

    }
}
