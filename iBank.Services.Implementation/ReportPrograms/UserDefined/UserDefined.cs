using CODE.Framework.Core.Utilities.Extensions;
using com.ciswired.libraries.CISLogger;
using Domain.Helper;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.XmlReport;
using iBank.Services.Implementation.Utilities.CurrencyConversion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UserDefinedReports;
using UserDefinedReports.Classes;
using System.Xml.Linq;
using Domain;
using Domain.Models.ReportPrograms.UserDefinedReport;
using Domain.Orm.Classes;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities.ExportHelpers;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined
{
    public class UserDefined : ReportRunner<RawData, FinalData>
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private UserReportInformation _userReport;

        private RoutingCriteria _routingCriteria;
        private ChangeLogManager _changeLogManager;
        private SwitchManager _switchManager;
        private bool _isPreview;
        private bool _udidExists;
        private string _moneyType;
        private bool _findTravelerLocation;
        private bool _originalIncludeVoids;
        private bool _includeCancelTrip;
        private int _rowCount;
        private int _distinctCount;
        private List<List<string>> _reportRows;
        private DataManager _dataManager;
        private string _xmlns = "";
        private AdvancedParameters _originalAdvancedParameters;


        public int ReportKey { get; set; }
        public UserDefinedParameters UserDefinedParams = new UserDefinedParameters();
        private readonly LoadedListsParams _loadedParams;

        public UserDefined(LoadedListsParams loadedParams)
        {
            _userReport = new UserReportInformation();
            _routingCriteria = new RoutingCriteria();
            _moneyType = "USD";

            _reportRows = new List<List<string>>();
            _loadedParams = loadedParams;
        }

        public override bool InitialChecks()
        {
            ReportKey = Globals.GetParmValue(WhereCriteria.UDRKEY).TryIntParse(-1);

            if (!IsGoodCombo()) return false;

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            return IsValidReportKey();
        }

        public override bool GetRawData()
        {
            using (var timer = new CustomTimer(Globals.ProcessKey, Globals.UserNumber, Globals.Agency, Globals.ReportLogKey, LOG, "UDR: GetRawData", ReportKey))
            {
                _isPreview = Globals.ParmValueEquals(WhereCriteria.PREPOST, "1");
                _udidExists = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1) > 0;

                # region Notes on exclude CBINCLVOIDS
                //*03 / 03 / 2010 - MUST REVISE HOW WE HANDLE VOIDS IN THIS PROGRAM:
                //*THE DEFAULT ACTION OF ibbldwhere.prg IS TO EXCLUDE VOIDS BY ADDING THIS TO
                //* THE "WHERE" CLAUSE:  ...and TRANTYPE != 'V'
                //* PROBLEM: trips TRANTYPE COLUMN ONLY APPLIES TO THE AIR PORTION OF A TRIP. 
                //*THE AIR CAN BE VOIDED, BY THE CAR OR HOTEL IS NOT VOIDED.THIS DEFAULT
                //*LOGIC THUS EXCLUDES SOME CAR OR HOTEL DATA THAT SHOULD NOT BE EXCLUDED.
                //*
                //*IN THIS PROGRAM, WE WILL SAVE THE USER'S OPTION FOR INCLUDE/EXCLUDE VOIDS, AND
                //* THEN SET THE "INCLUDE VOIDS" CRITERIA VARIABLE TO "ON".THIS WILL INSTRUCT
                //*ibbldwhere TO NOT EXCLUDE VOIDS.
                //*WHEN WE PROCESS THE DATA RETURNED FROM THE DATABASE QUERIES, WE WILL LOOK AT THE
                //* USER'S OPTION RE. VOIDS.  IF THEY ARE NOT INCLUDING VOIDS, THEN WE LOOK AT THE
                //* TRIPS LEVEL TRANTYPE FOR VOIDED AIR DATA, AND THE HIBHOTEL.HOTTRANTYP AND
                //*HIBCARS.CARTRANTYP TO SEE IF THAT DATA IS VOID.            
                #endregion
                _originalIncludeVoids = Globals.IsParmValueOn(WhereCriteria.CBINCLVOIDS);
                Globals.SetParmValue(WhereCriteria.CBINCLVOIDS, "ON");

                if (Globals.ProcessKey == (int)ReportTitles.AirUserDefinedReports)
                {
                    /*                
                * 07/29/2016 - OPTION: Use Connecting Legs
                * Explanation: With this option, we process leg/routing table criteria at the SEGMENT level,
                *  then dislay only the flight legs that belong to those SEGMENTS.
                cUseConnectLegs = getACritVal(toForm,"cbUseConnectLegs")
                if cUseConnectLegs = "ON"
                  * we turn off option for all legs on trips that meet routing criteria
                  =setACritVal(toForm,"cbIncludeAllLegs","OFF")  
                  gCrSegLeg = 1    && collapse data to segments, then will get flight legs
                endif
                */
                    //added above logic on 3/21/2017, DON't know where to use it yet.
                    if (Globals.IsParmValueOn(WhereCriteria.CBUSECONNECTLEGS)) Globals.SetParmValue(WhereCriteria.CBINCLUDEALLLEGS, "ON");

                }
                LOG.Debug($"GetRawData - LoadUserDefinedReport {ReportKey}");
                _userReport = UserDefinedReportLoader.LoadUserDefinedReport(Globals, ReportKey, ClientStore, MasterStore);
                if (!_userReport.IsValidReport) return false;

                SetDataSwitches();

                var userReportValidator = new UserReportValidator(Globals, ReportKey);
#if ! DEBUG
                if (!Globals.IsOfflineServer)
                {
                    if (userReportValidator.IsNeededToDelay(_userReport)) return false;
                }
#endif

                _findTravelerLocation = Globals.IsParmValueOn(WhereCriteria.CBFINDTRAVELERLOCN);
                if (_findTravelerLocation) _routingCriteria = RoutingCriteriaUtility.GetRoutingCriteria(BuildWhere.ReportGlobals);

                var conditional = new UserDefinedConditional();
                if (conditional.IsCarbonMetricHeadersOn(Globals, _userReport)) EnforceHeader.ChangeHeadersToMetric(_userReport.Columns);
                
                var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);
                if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, 
                        buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, 
                        inMemory: true, isRoutingBidirectional: false, legDit: false, ignoreTravel: true)) return false;

                RemoveAdvancedParameters();

                BuildWhere.BuildAdvancedClauses();
                
                BuildWhere.AddSecurityChecks();

                BuildOtherWhereClause();

                SetCancelTripSwith();

                _dataManager = new DataManager(Globals, BuildWhere, _switchManager, UserDefinedParams, _udidExists, _isPreview, _userReport);
                _dataManager.SetTripDataList(_includeCancelTrip);

                if (!IsUnderOfflineThreshold(UserDefinedParams.TripDataList)) return false;
                if (!DataExists(UserDefinedParams.TripDataList)) return false;

                DoTripCurrencyConvertion();

                _dataManager.SetOtherRawData(_changeLogManager, _findTravelerLocation, _routingCriteria, _switchManager);
                RawDataList = UserDefinedParams.TripDataList;
                return true;
            }
        }

        public override bool ProcessData()
        {
            LOG.Debug("ProcessData - Start");
            var sw = Stopwatch.StartNew();

            LOG.Debug("ProcessTripData");
            ProcessTripData();

            if (!DataExists(UserDefinedParams.TripDataList)) return false;
            LOG.Debug("FormatWhereText");
            FormatWhereText();

            //We need to remove trips if it doesn't have 
            //1. air segment while the report is of 501-ibCstmAir;
            //2. hotel segment while the report is of 502-ibCstmHotel;
            //3. car segment while the report is of 503-ibCstmCar;
            //4. svcfee segment while the report is of 504- ibCstmSvcFee  
            if (Globals.ProcessKey != (int)ReportTitles.CombinedUserDefinedReports)
            {
                var handler = new NoneCombineDetailReportHandler(UserDefinedParams, Globals.ProcessKey);
                handler.CleanTripData();
            }

            LOG.Debug("BuildReportRows");
            var rowBuilder = new UserDefinedRowBuilder();
            var helper = new ReportRowHelper();

            helper = rowBuilder.BuildReportRows(UserDefinedParams, _originalIncludeVoids, Globals, _userReport, BuildWhere, _isPreview, 
                _originalAdvancedParameters, _switchManager, _loadedParams.ColumnValueRulesFactory);

            _rowCount = helper.RowCount;
            _reportRows = helper.ReportRows;

            if (!DataExists(_reportRows)) return false;

            LOG.Debug("SortReportRows");
            SortReportRows(Globals.OutputFormat);

            //remove sort column.
            RemoveTheSortColumn(_reportRows, _userReport.Columns, Globals.OutputFormat);

            //so to get record court;
            FinalDataList = _reportRows.Select(s => new FinalData { RecKey = 0 }).Distinct().ToList();

            sw.Stop();
            LOG.Debug($"ProcessData - End | Elapsed:[{sw.Elapsed}]");
            return true;
        }

        private void RemoveTheSortColumn(List<List<string>> data, List<UserReportColumnInformation> header, DestinationSwitch outputFormat)
        {
           var i = 0;
            if (outputFormat == DestinationSwitch.ClassicPdf)
            {
                i = header.Count - 3; //remove reckey, legcntr and sort
            }
            else
            {
                i = header.Count - 1;
            }

            for (var j = i; i < header.Count; j++)
            {
                header.RemoveAt(i);
            }

            //sort data is in last to second
            foreach (List<string> row in data)
            {
                if (outputFormat == DestinationSwitch.ClassicPdf)
                {
                    for (var j = i; i < row.Count - 1; j++)
                    {
                        row.RemoveAt(i);
                    }
                }
                else
                {
                    for (var j = i; i < row.Count; j++)
                    {
                        row.RemoveAt(i);
                    }
                }
            }
          
        }

        public override bool GenerateReport()
        {
            LOG.Debug("GenerateReport - Start");
            var sw = Stopwatch.StartNew();

            var reportBuilder = new ReportBuilder
            {
                SuppressDetail = _userReport.SuppressDetail,
                FileName = Globals.GetFileName()
            };

            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                    StringListToXlsxHandler.StringListToXlsx(reportBuilder.FileName, _reportRows, _userReport.Columns, Globals);
                    break;
                case DestinationSwitch.Csv:
                    StringListToCsvHandler.StringListToCsv(reportBuilder.FileName, _reportRows, _userReport.Columns, Globals);
                    break;
                case DestinationSwitch.XML:
                    BuildXml(reportBuilder.FileName);
                    break;
                default:

                    var whereText = Globals.WhereText.Trim().Right(1).Equals(";")
                         ? Globals.WhereText.Trim().RemoveLastChar()
                         : Globals.WhereText.Trim();

                    whereText = whereText.Left(1).Equals(";")
                        ? whereText.RemoveFirstChar()
                        : whereText;

                    PackagingReport(Globals, reportBuilder, whereText);

                    if (!reportBuilder.BuildReport(Globals.OutputFormat))
                    {
                        Globals.ReportInformation.ReturnCode = 2;
                        Globals.ReportInformation.ErrorMessage = reportBuilder.ErrorMessage;
                        return false;
                    }

                    var mutatedFileName = Path.GetFileName(reportBuilder.FileName);
                    var originalFileName = Path.GetFileName(Globals.ReportInformation.Href);
                    Globals.ReportInformation.Href = Globals.ReportInformation.Href.Replace(originalFileName, mutatedFileName);
                    
                    break;
            }

            CustomReportEffects.ApplyEffects(reportBuilder.FileName, Globals);

            sw.Stop();
            LOG.Debug($"GenerateReport - End | Elapsed:[{sw.Elapsed}]");
            return true;
        }

        private void ProcessTripData()
        {
            LOG.Debug($"ProcessData - Call ModeHelper:[{Globals.GetParmValue(WhereCriteria.MODE) != "0" && Globals.GetParmValue(WhereCriteria.MODE) != ""}]");
            if (Globals.GetParmValue(WhereCriteria.MODE) != "0" && Globals.GetParmValue(WhereCriteria.MODE) != "")
            {
                var helper = new ModeHelper(Globals, BuildWhere.Parameters, BuildWhere.WhereClauseFull, _isPreview);
                UserDefinedParams.TripDataList = helper.ApplyFilter(UserDefinedParams.TripDataList);
                Globals.WhereText = helper.ModeText + " " + Globals.WhereText;
            }
            LOG.Debug($"ProcessData - TripDataList Count:[{UserDefinedParams.TripDataList.Count}]");

            LOG.Debug($"ProcessData - Call LocateTravelerManager:[{_findTravelerLocation}]");
            if (_findTravelerLocation)
            {
                RoutingCriteriaUtility.ClearRouteCriteria(BuildWhere.ReportGlobals);
                var locateTravalManager = new LocateTravelerManager(UserDefinedParams, _routingCriteria, Globals);
                locateTravalManager.LocateTraveler(Globals.IsParmValueOn(WhereCriteria.CBEXCLTRAVSARRIVINGHOME)); //this iterates over each record in the trip
            }

            RawDataList = UserDefinedParams.TripDataList;
            _distinctCount = UserDefinedParams.TripDataList.Select(x => x.RecKey).Distinct().ToList().Count;
        }

        private void FormatWhereText()
        {
            if (!_originalIncludeVoids)
            {
                Globals.WhereText = Globals.WhereText.Replace("; Including Voids", string.Empty).Replace("Including Voids", string.Empty);
                if (Globals.WhereText.Right(1).Equals(";"))
                    Globals.WhereText = Globals.WhereText.RemoveLastChar();
            }
        }

        private void SortReportRows(DestinationSwitch outputFormat)
        {
            var sortManager = new UserReportSortManager(_reportRows, _userReport.Columns, outputFormat);
            _reportRows = sortManager.Sort();
        }

        private void SetCancelTripSwith()
        {
            _changeLogManager = new ChangeLogManager(Globals);
            if ((_switchManager.ChangeLogSwitch || _changeLogManager.ChangeLogCriteriaPresent) && _isPreview) _includeCancelTrip = true;
        }

        private void PackagingReport(ReportGlobals globals, ReportBuilder reportBuilder, string whereText)
        {
            LOG.Debug("PackagingReport | Start");
            var sw = Stopwatch.StartNew();

            var bHasSummaryColumn = false;
            foreach (var column in _userReport.Columns)
            {
                if (!bHasSummaryColumn && column.ColumnType.EqualsIgnoreCase("NUMERIC"))
                {
                    bHasSummaryColumn = true;
                    LOG.Debug($"PackagingReport - bHasSummaryColumn:[{bHasSummaryColumn}]");
                }

                var conditional = new UserDefinedConditional();
                if (conditional.SuppressColumn(globals, column.Name)) continue;

                reportBuilder.Columns.Add(BuildCustomColumn(column));
            }

            var reportTile = !string.IsNullOrWhiteSpace(globals.ReportTitle) ? globals.ReportTitle : _userReport.ReportTitle;

            reportBuilder.AddParameter("ReportTitle", reportTile);
            reportBuilder.AddParameter("ReportTitle2", _userReport.ReportSubtitle);
            reportBuilder.AddParameter("gHstPrePref", Globals.HstPrePref);
            reportBuilder.AddParameter("AccountName", Globals.AccountName);
            reportBuilder.AddParameter("WhereText", whereText);
            reportBuilder.AddParameter("DateDescription", Globals.BuildDateDesc());
            reportBuilder.AddParameter("Copyright", Globals.CopyRight);
            reportBuilder.AddParameter("Footer", _userReport.PageFooterText);
            reportBuilder.AddParameter("ProcessId", Globals.ProcessKey.ToString());
            reportBuilder.AddParameter("UserId", Globals.User.UserId);
            reportBuilder.AddParameter("Printed", DateTime.Now.ToString(Globals.DateDisplay + " hh:mm:ss tt"));
            reportBuilder.LogoBytes = Globals.LogoBytes;
            reportBuilder.Style = _userReport.Theme;
            reportBuilder.Rows = _reportRows;

            //NET RowCount and ReportTotalText do not always match FoxPro. Amanda advices to leave NET as it is, and will discuss it if becomes an issue for customers.
            reportBuilder.RowCount = (bHasSummaryColumn)
                    ? _rowCount
                    : _distinctCount;
            reportBuilder.ReportTotalText = bHasSummaryColumn
                    ? "*** REPORT TOTAL ***"
                    : "** END OF REPORT **";

            sw.Stop();
            LOG.Debug($"PackagingReport - End | Elapsed:[{sw.Elapsed}]");
        }

        private void BuildXml(string fileName)
        {
            LOG.Debug("BuildXml - Start");
            var sw = Stopwatch.StartNew();

            var xmlReportBuilder = new XMLReportBuilder(Globals, _userReport);
            var xDoc = new XDocument();
            var xmlResult = new XElement(_xmlns + "XmlResult");
            var tag = new XmlTag() { TagName = "iBankReportCriteria" };
            xDoc.AddFirst(xmlResult);
            LOG.Debug($"BuildXml - Added:[XmlResult]");
            xmlResult.Add(xmlReportBuilder.BuildCriteria(tag, _userReport.ReportName, "XML"));
            LOG.Debug($"BuildXml - Added:[iBankReportCriteria]");
            xmlResult.Add(xmlReportBuilder.BuildColumeStructure("ColumnStructure", "Column"));
            LOG.Debug($"BuildXml - Added:[ColumnStructure]");
            xmlResult.Add(xmlReportBuilder.BuildResultRows(_reportRows, "ResultRows", "Row", "Column", _userReport.DefinedColumnCount));
            LOG.Debug($"BuildXml - Added:[ResultRows]");
            xDoc.Save(fileName);
            LOG.Debug($"BuildXml - Saved:[{fileName}]");

            sw.Stop();
            LOG.Debug($"BuildXml - End | Elapsed:[{sw.Elapsed}]");
        }

        private CustomColumnInformation BuildCustomColumn(UserReportColumnInformation column)
        {
            var customColumnMapper = new CustomColumnMapper();
            var newCol = customColumnMapper.BuildCustomColumn(column, _userReport.Theme);

            return newCol;
        }

        private void SetDataSwitches()
        {
            LOG.Debug($"SetDataSwitches");
            _switchManager = new SwitchManager(_userReport.Columns, Globals);
            _switchManager.PopulateSwitches(new GetActiveColumnsQuery(MasterStore.MastersQueryDb).ExecuteQuery());
        }

        private void BuildOtherWhereClause()
        {
            var begDate = Globals.BeginDate ?? DateTime.MinValue;
            var endDate = Globals.EndDate ?? DateTime.MinValue;
            var tranDateWithinRange = Globals.IsParmValueOn(WhereCriteria.CBTRANDATEWITHINRANGE);
            if (tranDateWithinRange)
            {
                BuildWhere.WhereClauseSvcFee += "and trandate between '" +
                                                begDate.ToString(Globals.DateDisplay) + "' and '" +
                                                endDate.ToString(Globals.DateDisplay) + " 11:59:59 PM'";
            }
            
            /*WhereClauseTrip has already have lastupdate clause when Last Update is selected, there is no need to add it again. 
             *However hibtrips doesn't have a lastupdate field, this is to prevent the report being called from Backoffice - replace lastupdate with cast('1900-01-01' as datetime)
             */
            if (IsLastUpdatedDateRange && !_isPreview)
            {
                BuildWhere.WhereClauseTrip = BuildWhere.WhereClauseTrip.Replace("lastupdate", "cast('1900-01-01' as datetime)");             
            }
        }

        private bool IsLastUpdatedDateRange => Globals.ParmValueEquals(WhereCriteria.DATERANGE, "21");

        private void DoTripCurrencyConvertion()
        {
            _moneyType = Globals.GetParmValue(WhereCriteria.MONEYTYPE);
            if (UserDefinedParams.TripDataList.Select(s => s.Moneytype).Distinct().Any(s => s != _moneyType && _moneyType != ""))
            {
                LOG.Debug("DoTripCurrencyConvertion - Start");
                LOG.Debug($"DoTripCurrencyConvertion - Count:[{UserDefinedParams.TripDataList.Count}]");
                var sw = Stopwatch.StartNew();

                var cc = new CurrencyConverter();
                cc.ConvertCurrency(UserDefinedParams.TripDataList, _moneyType);

                sw.Stop();
                LOG.Debug($"DoTripCurrencyConvertion - End | Elapsed:[{sw.Elapsed}]");
            }
        }

        private void RemoveAdvancedParameters()
        {
            //store off advanced parms for later use
            _originalAdvancedParameters = new AdvancedParameters { AndOr = Globals.AdvancedParameters.AndOr };

            //Remove all parameters that need to be applied later. Save them for later use. 
            _originalAdvancedParameters.Parameters.AddRange(Globals.AdvancedParameters.Parameters.Where(s => UserReportCheckLists.TripTlsAdvancedCrit.Contains(s.FieldName.Trim())));
            Globals.AdvancedParameters.Parameters.RemoveAll(s => UserReportCheckLists.TripTlsAdvancedCrit.Contains(s.FieldName.Trim()));

            _originalAdvancedParameters.Parameters.AddRange(Globals.AdvancedParameters.Parameters.Where(s => UserReportCheckLists.MiscSegsAdvancedCrit.Contains(s.FieldName.Trim())));
            Globals.AdvancedParameters.Parameters.RemoveAll(s => UserReportCheckLists.MiscSegsAdvancedCrit.Contains(s.FieldName.Trim()));

            _originalAdvancedParameters.Parameters.AddRange(Globals.AdvancedParameters.Parameters.Where(s => UserReportCheckLists.HotelAdvancedCrit.Contains(s.FieldName.Trim())));
            Globals.AdvancedParameters.Parameters.RemoveAll(s => UserReportCheckLists.HotelAdvancedCrit.Contains(s.FieldName.Trim()));

            _originalAdvancedParameters.Parameters.AddRange(Globals.AdvancedParameters.Parameters.Where(s => UserReportCheckLists.CarAdvancedCrit.Contains(s.FieldName.Trim())));
            Globals.AdvancedParameters.Parameters.RemoveAll(s => UserReportCheckLists.CarAdvancedCrit.Contains(s.FieldName.Trim()));

            _originalAdvancedParameters.Parameters.AddRange(Globals.AdvancedParameters.Parameters.Where(s => UserReportCheckLists.MarketSegsAdvancedCrit.Contains(s.FieldName.Trim())));
            Globals.AdvancedParameters.Parameters.RemoveAll(s => UserReportCheckLists.MarketSegsAdvancedCrit.Contains(s.FieldName.Trim()));

            _originalAdvancedParameters.Parameters.AddRange(Globals.AdvancedParameters.Parameters.Where(s => UserReportCheckLists.TripsAdvancedCrit.Contains(s.FieldName.Trim())));
            Globals.AdvancedParameters.Parameters.RemoveAll(s => UserReportCheckLists.TripsAdvancedCrit.Contains(s.FieldName.Trim()));

            _originalAdvancedParameters.Parameters.AddRange(Globals.AdvancedParameters.Parameters.Where(s => UserReportCheckLists.OnDmSegsAdvancedCrit.Contains(s.FieldName.Trim())));
            Globals.AdvancedParameters.Parameters.RemoveAll(s => UserReportCheckLists.OnDmSegsAdvancedCrit.Contains(s.FieldName.Trim()));

            _originalAdvancedParameters.Parameters.AddRange(Globals.AdvancedParameters.Parameters.Where(s => UserReportCheckLists.LimoAdvancedCrit.Contains(s.FieldName.Trim())));
            Globals.AdvancedParameters.Parameters.RemoveAll(s => UserReportCheckLists.LimoAdvancedCrit.Contains(s.FieldName.Trim()));

            _originalAdvancedParameters.Parameters.AddRange(Globals.AdvancedParameters.Parameters.Where(s => UserReportCheckLists.RailTicketAdvancedCrit.Contains(s.FieldName.Trim())));
            Globals.AdvancedParameters.Parameters.RemoveAll(s => UserReportCheckLists.RailTicketAdvancedCrit.Contains(s.FieldName.Trim()));

            _originalAdvancedParameters.Parameters.AddRange(Globals.AdvancedParameters.Parameters.Where(s => UserReportCheckLists.CruiseAdvancedCrit.Contains(s.FieldName.Trim())));
            Globals.AdvancedParameters.Parameters.RemoveAll(s => UserReportCheckLists.CruiseAdvancedCrit.Contains(s.FieldName.Trim()));

            _originalAdvancedParameters.Parameters.AddRange(Globals.AdvancedParameters.Parameters.Where(s => UserReportCheckLists.TourAdvancedCrit.Contains(s.FieldName.Trim())));
            Globals.AdvancedParameters.Parameters.RemoveAll(s => UserReportCheckLists.TourAdvancedCrit.Contains(s.FieldName.Trim()));
        }

        private bool IsValidReportKey()
        {
            if (ReportKey < 0)
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = "Invalid ReportKey" + Globals.GetParmValue(WhereCriteria.UDRKEY);
                return false;
            }
            return true;
        }
    }
}

