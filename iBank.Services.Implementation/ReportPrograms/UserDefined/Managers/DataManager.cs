using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;

using com.ciswired.libraries.CISLogger;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Models;
using Domain.Models.ReportPrograms.UserDefinedReport;

using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined.DataSqlScripts;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities.ClientData;
using iBank.Repository.SQL.Repository;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.DataSetHandler;
using System.Threading.Tasks;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers
{
    public class DataManager
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ReportGlobals _globals;
        private readonly SwitchManager _switchManager;
        private readonly UserDefinedParameters _userDefinedParams;
        private readonly bool _isReservation;
        private readonly bool _udidExists;
        private readonly BuildWhere _buildWhere;
        private readonly UserReportInformation _userReport;
        private readonly string _moneyType;

        private readonly IClientDataStore _clientStore;

        public DataManager(ReportGlobals globals, BuildWhere buildWhere, SwitchManager switchManager, UserDefinedParameters parameters, bool udidExists, bool isReservation, UserReportInformation userReport)
        {
            _globals = globals;
            _switchManager = switchManager;
            _userDefinedParams = parameters;
            _isReservation = isReservation;
            _udidExists = udidExists;
            _buildWhere = buildWhere;
            _moneyType = globals.GetParmValue(WhereCriteria.MONEYTYPE);
            _userReport = userReport;

            _clientStore = new ClientDataStore(_globals.AgencyInformation.ServerName, _globals.AgencyInformation.DatabaseName);
        }

        public void SetTripDataList(bool includeCancelTrip)
        {
            using (var timer = new CustomTimer(_globals.ProcessKey, _globals.UserNumber, _globals.Agency, _globals.ReportLogKey, LOG, $"SetTripDataList (includeCancelTrip: {includeCancelTrip}", _userReport.ReportKey))
            {
                var whereClause = _buildWhere.WhereClauseFull;
                if (_globals.ParmValueEquals(WhereCriteria.DATERANGE, "9"))
                {
                    whereClause = whereClause.Replace(" depdate <=", "tripstart <=").Replace("arrdate >=", "tripend >=");
                    LOG.Debug($"SetTripDataList | whereClause:" + whereClause);
                }

                var udidNumber = _globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1);
                var sqlBuilder = new UserReportTripSqlBuilder();
                var sql = sqlBuilder.CreateScript(whereClause, udidNumber, _isReservation, _switchManager.TripTlsSwitch, _globals.ProcessKey, _globals.GetParmValue(WhereCriteria.DATERANGE), _buildWhere, true);

                LOG.Debug($"SetTripDataList - ClientDataRetrieval.GetRawData");
                _userDefinedParams.TripDataList = ClientDataRetrieval.GetRawData<RawData>(sql, _isReservation, _buildWhere, _globals, false, includeAllLegs: true,
                        checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();


                LOG.Debug($"SetTripDataList | TripDataList record count:[{_userDefinedParams.TripDataList.Count}]");

                if (includeCancelTrip)
                {
                    SetCancelledTrip(sql);
                }
            }
        }
        
        private void SetCancelledTrip(SqlScript sql)
        {
            var cancSql = sql;
            cancSql.FromClause = sql.FromClause.Replace("ibtrips", "ibcanctrips").Replace("ibudids", "ibcancudids");
            cancSql.FieldList = sql.FieldList.Replace("'UPD ' as", "'CANC' as");

            _userDefinedParams.CancelledTripDataList = ClientDataRetrieval.GetRawData<RawData>(sql, _isReservation, _buildWhere, _globals, false).ToList();
            LOG.Debug($"SetCancelledTrip - CancelledTripDataList Count:[{_userDefinedParams.CancelledTripDataList.Count}]");
            _userDefinedParams.TripDataList.AddRange(_userDefinedParams.CancelledTripDataList);
            LOG.Debug($"SetCancelledTrip - AddRange to TripDataList Count:[{_userDefinedParams.TripDataList.Count}]");
        }
        
        public void SetOtherRawData(ChangeLogManager changeLogManager, bool findTravelerLocation, RoutingCriteria routingCriteria, SwitchManager switchManager)
        {
            LOG.Debug($"SetOtherRawData - Start");
            var sw = Stopwatch.StartNew();

            var loadTypes = new List<string> { "Udid", "Leg", "Car", "Hotel", "Misc", "Market", "TravelAuth","TravelAuthorizers","ServiceFee", "ChangeLog", "Other" };
            try
            {
#if DEBUG
                foreach(var loadType in loadTypes)
                {
                    SetOtherRawData(changeLogManager, findTravelerLocation, routingCriteria, loadType, switchManager);
                }
#else
               Parallel.ForEach(loadTypes, new ParallelOptions { MaxDegreeOfParallelism = GetMaxThreads() }, loadType =>
                {
                    LOG.Debug($"SetOtherRawData - LoadType: {loadType}");
                    SetOtherRawData(changeLogManager, findTravelerLocation, routingCriteria, loadType, switchManager);
                });
#endif
            }
            catch (Exception e)
            {
                //don't want to throw the AggregateException - just the inner exception
                ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            }
            
            sw.Stop();
            LOG.Debug($"SetOtherRawData - End | Elapsed:[{sw.Elapsed}]");
        }

        private static int GetMaxThreads()
        {
            /*
             * The MaxDegreeOfParallelism property affects the number of concurrent operations run by Parallel 
             * method calls that are passed this ParallelOptions instance. A positive property value limits the 
             * number of concurrent operations to the set value. If it is -1, there is no limit on the number of 
             * concurrently running operations.
             */
            #if DEBUG
                return 1;
            #else
                return -1;
            #endif
        }

        private void SetOtherRawData(ChangeLogManager changeLogManager, bool findTravelerLocation, RoutingCriteria routingCriteria, string loadType, SwitchManager switchManager)
        {
            var whereClauseFull = _buildWhere.WhereClauseFull;
            if (loadType == "Udid" && _switchManager.UdidSwitch)
            {
                var handler = new UdidDataSetHandler();
                _userDefinedParams.UdidDataList = handler.GetUdidData(whereClauseFull, _isReservation, _globals, _buildWhere, switchManager);
            }           
            
            if (loadType == "Leg" && IsLegDataLoad(findTravelerLocation))
            {
                var handler = new LegDataSetHandler(_isReservation, _globals, _buildWhere, _clientStore);
                ProcessLegData(handler, whereClauseFull, findTravelerLocation, routingCriteria);
                _userDefinedParams.AirLegDataList = _userDefinedParams.LegDataList.Where(s => s.Mode.EqualsIgnoreCase("A")).ToList();
                _userDefinedParams.RailLegDataList = _userDefinedParams.LegDataList.Where(s => s.Mode.EqualsIgnoreCase("R")).ToList();
            }

            if (loadType == "Car" && (_switchManager.CarSwitch || _switchManager.CarSwitch2))
            {
                var handler = new CarDataSetHandler(_isReservation, _globals, _buildWhere);
                _userDefinedParams.CarDataList = handler.GetCarData(whereClauseFull, _switchManager.TripTlsSwitch, _udidExists,
                    _moneyType, _userReport);
            }

            if (loadType == "Hotel" && (_switchManager.HotelSwitch || _switchManager.HotelSwitch2))
            {
                var handler = new HotelDataSetHandler();
                _userDefinedParams.HotelDataList = handler.GetHotelData(whereClauseFull, _switchManager.TripTlsSwitch, _udidExists, _isReservation,
                    _buildWhere, _globals, _userReport, _moneyType);

                if (_switchManager.HotelOnly)
                {
                    var recKeys = _userDefinedParams.HotelLookup.Select(x => x.Key).ToList();
                    _userDefinedParams.TripDataList = _userDefinedParams.TripDataList
                                                                        .Where(x => recKeys.Contains(x.RecKey))
                                                                        .ToList();
                }
            }

            if (loadType == "Misc" && _switchManager.MiscSegmentsSwitch)
            {
                var handler = new MiscDataSetHandler();
                _userDefinedParams.MiscDataList = handler.GetMiscData(whereClauseFull, _switchManager.TripTlsSwitch, _udidExists, _isReservation, _buildWhere,
                    _globals, _moneyType);
            }

            if (loadType == "Market" && (_switchManager.MarketSegmentsSwitch || _switchManager.LongestSegSwitch))
            {
                var handler = new MarketSegDataSetHandler();
                _userDefinedParams.MarketSegmentDataList = handler.GetMarketSegmentData(whereClauseFull, _switchManager.TripTlsSwitch, _udidExists,
                    _isReservation, _buildWhere, _globals, _moneyType);
            }

            if (loadType == "TravelAuth" && (_switchManager.TraveAuthSwitch && _isReservation))
            {
                SetTravelAuthData(whereClauseFull);
            }

            if (loadType == "TravelAuthorizers" && _switchManager.TravelAuthorizersSwitch && _isReservation)
            {
                SetTravelAuthorizersData(whereClauseFull);
            }
            
            if (loadType == "ChangeLog" && ((_switchManager.ChangeLogSwitch || changeLogManager.ChangeLogCriteriaPresent) && _isReservation))
            {
                ProcessChangeLogLoad(changeLogManager, whereClauseFull);
            }

            if (loadType == "ServiceFee" && ((_switchManager.ServiceFeeSwitch || _switchManager.ServiceFeeSwitch2) && !_isReservation))
            {
                var handler = new ServiceFeeDataSetHandler();
                _userDefinedParams.ServiceFeeDataList = handler.GetServiceFeeData(whereClauseFull, _switchManager.TripTlsSwitch, _udidExists, _isReservation,
                    _buildWhere, _globals, _moneyType);
            }

            if (loadType == "Other")
            {
                //These four data types are the same
                SetMiscSegSharedData(whereClauseFull);
                LOG.Debug($"Misc Crus Count:{_userDefinedParams.MiscSegCruiseDataList.Count}");
                LOG.Debug($"Misc Limo Count:{_userDefinedParams.MiscSegLimoDataList.Count}");
                LOG.Debug($"Misc Rail Count:{_userDefinedParams.MiscSegRailTicketDataList.Count}");
                LOG.Debug($"Misc Tour Count:{_userDefinedParams.MiscSegTourDataList.Count}");
            }
        }

        private void ProcessLegData(LegDataSetHandler handler, string whereClause, bool findTravelerLocation, RoutingCriteria routingCriteria)
        {
            _userDefinedParams.LegDataList = handler.GetLegData(_switchManager.TripTlsSwitch, _udidExists, whereClause);

            var tempWhereText = handler.GetTempWhereTextForLeg(_globals.WhereText);

            if (_buildWhere.HasRoutingCriteria)
            {
                _userDefinedParams.LegDataList = handler.GetRoutingCriteriaFilteredData(_userDefinedParams.LegDataList);

                var recKeys = _userDefinedParams.LegLookup.Select(x => x.Key).ToList();
                _userDefinedParams.TripDataList = handler.RemoveFilteredOutDataFromTrip(recKeys, _userDefinedParams.TripDataList);
            }

            if (findTravelerLocation) _globals.WhereText = handler.GetTravelerLocationText(tempWhereText, routingCriteria);

            if (_userReport.HasCarbonFields && _userReport.CarbonCalculator != null)
            {
                _userDefinedParams.LegDataList = handler.GetLegCarbon(_userDefinedParams.LegDataList, _userReport.CarbonCalculator);
            }

            if (_userReport.SegmentOrLeg == SegmentOrLeg.Segment)
            {
                _userDefinedParams.LegDataList = handler.GetCollapsedLegData(_userDefinedParams.LegDataList, _userReport.Columns);
            }

            _userDefinedParams.LegDataList = handler.ConvertCurrency(_userDefinedParams.LegDataList, _moneyType);
        }

        private bool IsLegDataLoad(bool findTravelerLocation)
        {
            return _switchManager.LegSwitch || _switchManager.LegSwitch2 || _buildWhere.HasRoutingCriteria || _buildWhere.HasFirstDestination
                   || _buildWhere.HasFirstDestination || findTravelerLocation;
        }

        private void ProcessChangeLogLoad(ChangeLogManager changeLogManager, string whereClauseFull)
        {
            var handler = new ChangeLogDataSetHandler(_isReservation, _udidExists, _buildWhere, _globals, changeLogManager);
            if (_switchManager.ChangeLogSwitch || changeLogManager.ChangeLogCriteriaPresent)
            {
                _userDefinedParams.ChangeLogDataList = handler.GetChangeLogData(whereClauseFull);
                _globals.WhereText += changeLogManager.WhereChangeLogText;
            }

            if (_userDefinedParams.CancelledTripDataList.Count != 0)
            {
                var changeLog = handler.GetCancelledTripsChangeLog(whereClauseFull);
                _userDefinedParams.ChangeLogDataList.AddRange(changeLog);
            }

            if (changeLogManager.TripCancelYn == "N")
            {
                var cancelledTripReckeys = _userDefinedParams.CancelledTripLookup.Select(x => x.Key);
                _userDefinedParams.TripDataList = handler.GetNonCancelledTripData(_userDefinedParams.TripDataList, cancelledTripReckeys);
            }
            else if (changeLogManager.TripCancelYn == "Y")
            {
                _userDefinedParams.ChangeLogDataList.RemoveAll(x => x.Changecode != 101);
                var changeLogReckeys = _userDefinedParams.ChangeLogLookup.Select(x => x.Key);
                _userDefinedParams.TripDataList = handler.GetTripsWithChangeLogInfo(_userDefinedParams.TripDataList, changeLogReckeys);
            }

            //remove change log when it's not used for display 
            if (!_switchManager.ChangeLogSwitch) _userDefinedParams.ChangeLogDataList.RemoveAll(x => x.RecKey != 0);
            _userDefinedParams.ChangeLogDataList = handler.OrderChangeLogDataSegCtr(_userDefinedParams.ChangeLogDataList);

            LOG.Debug($"ChangeLog Count:{_userDefinedParams.ChangeLogDataList.Count}");
        }

        private void SetTravelAuthorizersData(string whereClause)
        {
            var sqlScript = new TravelAuthorizersSqlScript();
            var sql = sqlScript.GetSqlScript(_switchManager.TripTlsSwitch, _udidExists, _isReservation, whereClause);
            _userDefinedParams.TravAuthorizerDataList = ClientDataRetrieval.GetRawData<TravAuthorizerRawData>(sql, _isReservation, _buildWhere, _globals, false).ToList();
        }

        private void SetTravelAuthData(string whereClause)
        {
            var sqlScript = new TravelAuthSqlScript();
            var sql = sqlScript.GetSqlScript(_switchManager.TripTlsSwitch, _udidExists, _isReservation, whereClause);

            _userDefinedParams.TravAuthDataList = ClientDataRetrieval.GetRawData<TravAuthRawData>(sql, _isReservation, _buildWhere, _globals, false).ToList();

            LOG.Debug($"TravelAuth Count:{_userDefinedParams.TravAuthDataList.Count}");
        }

        private void SetMiscSegSharedData(string whereClause)
        {
            var handler = new MiscSegmentDataSetHandler();

            if (_switchManager.MiscSegmentTourSwitch)
            {
                _userDefinedParams.MiscSegTourDataList = handler.GetMiscSegmentData("TUR", whereClause, _switchManager.TripTlsSwitch, _udidExists,
                    _isReservation, _buildWhere, _globals, _moneyType);
            }

            if (_switchManager.MiscSegmentCruiseSwitch)
            {
                _userDefinedParams.MiscSegCruiseDataList = handler.GetMiscSegmentData("SEA", whereClause, _switchManager.TripTlsSwitch, _udidExists,
                    _isReservation, _buildWhere, _globals, _moneyType);
            }

            if (_switchManager.MiscSegmentLimoSwitch)
            {
                _userDefinedParams.MiscSegLimoDataList = handler.GetMiscSegmentData("LIM", whereClause, _switchManager.TripTlsSwitch, _udidExists,
                    _isReservation, _buildWhere, _globals, _moneyType);
            }

            if (_switchManager.MiscSegmentRalSwitch)
            {
                _userDefinedParams.MiscSegRailTicketDataList = handler.GetMiscSegmentData("RAL", whereClause, _switchManager.TripTlsSwitch, _udidExists,
                    _isReservation, _buildWhere, _globals, _moneyType);
            }
        }
    }
}
