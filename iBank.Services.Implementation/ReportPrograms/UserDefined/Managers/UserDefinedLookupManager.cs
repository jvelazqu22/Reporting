using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Linq;
using UserDefinedReports;
using System.Globalization;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;
using com.ciswired.libraries.CISLogger;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using Domain.Helper;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.LookupFieldHandlers;
using iBank.Services.Implementation.Shared.Client;
using Domain;
using System.Threading.Tasks;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers
{
    public class UserDefinedLookupManager
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        enum LoadTypes
        {
            Legs,
            Segs,
            Udids,
            TripCarbon,
            VendorTypes,
            TripsDerivedData,
        }

        private readonly UserDefinedParameters _userDefinedParams;
        private readonly bool _isDdTime;
        private readonly ReportGlobals _globals;
        private readonly ClientFunctions _clientFunctions = new ClientFunctions();
        
        private readonly BuildWhere _buildWhere;
        public SegmentOrLeg _segmentOrLeg;
        public List<Tuple<string, string>> TripSummaryLevel { get; set; }

        private readonly IMasterDataStore _masterStore;
        private readonly IClientDataStore _clientStore;

        private ReportLookups ReportLookups { get; set; }

        private readonly bool _isTitleCaseRequired;

        private LookupAuthorizerFieldHandler _lookupAuthorizerFieldHandler;
        private LookupHotelFieldHandler _lookupHotelFieldHandler;
        private LookupLegAirFieldHandler _lookupLegAirFieldHandler;
        private LookupLegCarFieldHandler _lookupLegCarFieldHandler;
        private LookupLegFieldHandler _lookupLegFieldHandler;
        private LookupLegRailFieldHandler _lookupLegRailFieldHandler;
        private LookupMiscSegsCruiseFieldHandler _lookupMiscSegsCruiseFieldHandler;
        private LookupMiscSegsLimoFieldHandler _lookupMiscSegsLimoFieldHandler;
        private LookupMiscSegsTourFieldHandler _lookupMiscSegsTourFieldHandler;
        private LookupMktSegsFieldHandler _lookupMktSegsFieldHandler;
        private LookupMktSegsRailTickerFieldHandler _lookupMktSegsRailTickerFieldHandler;
        private LookupServiceFeeFieldHandler _lookupServiceFeeFieldHandler;
        private LookupTravAuhtFieldHandler _lookupTravAuhtFieldHandler;
        private LookupTripFieldHandler _lookupTripFieldHandler;
        private LookupMiscSegsFieldHandler _lookupMiscSegsFieldHandler;

        private void SetupLookupHandlers(UserDefinedParameters userDefinedParams, IMasterDataStore masterStore, IClientDataStore clientStore, bool isTitleCaseRequired,
            ReportGlobals globals, SegmentOrLeg segmentOrLeg, ReportLookups reportLookups, bool isDdTime, BuildWhere buildWhere, List<UserReportColumnInformation> columns)
        {
            _lookupLegFieldHandler = new LookupLegFieldHandler(userDefinedParams, masterStore, clientStore, isTitleCaseRequired, globals, isDdTime, reportLookups, segmentOrLeg);
            _lookupLegAirFieldHandler = new LookupLegAirFieldHandler(userDefinedParams, masterStore, clientStore, isTitleCaseRequired, globals, isDdTime, reportLookups, segmentOrLeg);
            _lookupLegRailFieldHandler = new LookupLegRailFieldHandler(userDefinedParams, masterStore, clientStore, isTitleCaseRequired, globals, isDdTime, reportLookups, segmentOrLeg);
            _lookupTripFieldHandler = new LookupTripFieldHandler(userDefinedParams, masterStore, clientStore, isTitleCaseRequired, globals, segmentOrLeg, reportLookups, buildWhere);
            _lookupLegCarFieldHandler = new LookupLegCarFieldHandler(userDefinedParams, masterStore, clientStore, isTitleCaseRequired, globals, reportLookups, segmentOrLeg);
            _lookupHotelFieldHandler = new LookupHotelFieldHandler(userDefinedParams, masterStore, clientStore, globals, segmentOrLeg, reportLookups, buildWhere, columns);
            _lookupMktSegsFieldHandler = new LookupMktSegsFieldHandler(userDefinedParams, masterStore, isTitleCaseRequired, globals, segmentOrLeg);
            _lookupMiscSegsTourFieldHandler = new LookupMiscSegsTourFieldHandler(userDefinedParams, masterStore, isTitleCaseRequired, segmentOrLeg);
            _lookupMiscSegsCruiseFieldHandler = new LookupMiscSegsCruiseFieldHandler(userDefinedParams, masterStore, isTitleCaseRequired, segmentOrLeg);
            _lookupMiscSegsLimoFieldHandler = new LookupMiscSegsLimoFieldHandler(userDefinedParams, segmentOrLeg);
            _lookupMktSegsRailTickerFieldHandler = new LookupMktSegsRailTickerFieldHandler(userDefinedParams, segmentOrLeg, globals);
            _lookupTravAuhtFieldHandler = new LookupTravAuhtFieldHandler(userDefinedParams, clientStore, globals, segmentOrLeg, reportLookups, buildWhere, columns);
            _lookupAuthorizerFieldHandler = new LookupAuthorizerFieldHandler(userDefinedParams, clientStore, segmentOrLeg);
            _lookupServiceFeeFieldHandler = new LookupServiceFeeFieldHandler(userDefinedParams, masterStore, clientStore, globals, segmentOrLeg);
            _lookupMiscSegsFieldHandler = new LookupMiscSegsFieldHandler(userDefinedParams);
        }

        public UserDefinedLookupManager(UserDefinedParameters userDefinedParams, bool isDdTime, ReportGlobals globals, BuildWhere buildWhere, 
            List<UserReportColumnInformation> columns, SegmentOrLeg segmentOrLeg)
        {
            _segmentOrLeg = segmentOrLeg;
            _masterStore = new MasterDataStore();
            _clientStore = new ClientDataStore(globals.AgencyInformation.ServerName, globals.AgencyInformation.DatabaseName);
            _userDefinedParams = userDefinedParams;
            _isDdTime = isDdTime;
            _globals = globals;
            _buildWhere = buildWhere;

            TripSummaryLevel = new List<Tuple<string, string>>();
            _isTitleCaseRequired = _globals.Agency.Equals("GSA");

            ReportLookups = new ReportLookups(columns, buildWhere, globals);
      }

        public void LoadReportSpecificLookups(List<UserReportColumnInformation> columns, bool isReservationData, SwitchManager switchManager, bool udidExists, List<int> recKeyList)
        {
            var assignTripClass = columns.Any(s => UserReportCheckLists.TripClassColumns.Contains(s.Name.Trim()));

            List<LoadTypes> loadTypes;
            if (Features.RoutingUseTripsDerivedDataTable.IsEnabled())
            {
                //derivedData is joined with legs/segs
                loadTypes = new List<LoadTypes>() { LoadTypes.Legs, LoadTypes.Segs, LoadTypes.Udids, LoadTypes.TripCarbon, LoadTypes.VendorTypes};
            }
            else
            {
                loadTypes = new List<LoadTypes>() { LoadTypes.Legs, LoadTypes.Segs, LoadTypes.Udids, LoadTypes.TripCarbon, LoadTypes.VendorTypes, LoadTypes.TripsDerivedData };
            }
            try
            {
                #if DEBUG
                    foreach (var loadType in loadTypes)
                    {
                        LoadRequiredLookupLists(assignTripClass, loadType, isReservationData, switchManager, udidExists, recKeyList);
                    }
                #else
                    Parallel.ForEach(loadTypes, new ParallelOptions { MaxDegreeOfParallelism = GetMaxThreads() }, loadType =>
                        {
                            LoadRequiredLookupLists(assignTripClass, loadType, isReservationData, switchManager, udidExists, recKeyList);
                        });
                #endif
                //at this point ReportLookups is loaded.
                SetupLookupHandlers(_userDefinedParams, _masterStore, _clientStore, _isTitleCaseRequired, _globals, _segmentOrLeg, ReportLookups, _isDdTime, _buildWhere, columns);
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;//this throw doesn't actually get executed, but the compiler complains if it isn't there
            }
            catch (Exception e)
            {
                if (e.InnerException != null) ExceptionDispatchInfo.Capture(e.InnerException).Throw();

                throw;
            }
        }

        private static int GetMaxThreads()
        {
#if DEBUG
                return 1;
#else
                return -1;
#endif
        }

        private void LoadRequiredLookupLists(bool assignTripClass, LoadTypes loadType, bool isReservationData, SwitchManager switchManager, bool udidExists, List<int> recKeyList)
        {
            switch (loadType)
            {
                case LoadTypes.Legs:
                    // There used to be an if statement to check for legs or segs to only load what is needed. However, is is possible for the report
                    // filter to be at the leg level, but be seg level at the db level per this sql: select segmentleg from userrpts where reportkey = 3429
                    // see comments in the UserReportDefinitionRetriever.LoadUserReportInformation method
                    var sw1 = new Stopwatch();
                    sw1.Start();
                    ReportLookups.SetLegs(assignTripClass, _clientStore, switchManager.TripTlsSwitch);
                    sw1.Stop();
                    LOG.Debug($"Leg Lookup took {sw1.Elapsed.TotalSeconds} (sec)");
                    break;
                case LoadTypes.Segs:
                    // There used to be an if statement to check for legs or segs to only load what is needed. However, is is possible for the report
                    // filter to be at the leg level, but be seg level at the db level per this sql: select segmentleg from userrpts where reportkey = 3429
                    // see comments in the UserReportDefinitionRetriever.LoadUserReportInformation method
                    var sw2 = new Stopwatch();
                    sw2.Start();
                    ReportLookups.SetSegs(assignTripClass, _clientStore, switchManager.TripTlsSwitch);
                    sw2.Stop();
                    LOG.Debug($"Segs Lookup took {sw2.Elapsed.TotalSeconds} (sec)");
                    break;                   
                case LoadTypes.Udids:
                    var sw3 = new Stopwatch();
                    sw3.Start();
                    ReportLookups.SetUdids(isReservationData, switchManager);
                    sw3.Stop();
                    LOG.Debug($"Udids Lookup took {sw3.Elapsed.TotalSeconds} (sec)");
                    break;
                case LoadTypes.TripCarbon:
                    var sw4 = new Stopwatch();
                    sw4.Start();
                    ReportLookups.SetTripCarbon(_userDefinedParams.LegDataList);                    
                    sw4.Stop();
                    LOG.Debug($"TripCarbon Lookup took {sw4.Elapsed.TotalSeconds} (sec)");
                    break;
                case LoadTypes.VendorTypes:
                    var sw = new Stopwatch();
                    sw.Start();
                    ReportLookups.SetVendorTypes(isReservationData, udidExists);
                    sw.Stop();
                    LOG.Debug($"VendorTypes Lookup took {sw.Elapsed.TotalSeconds} (sec)");
                    break;
                case LoadTypes.TripsDerivedData:
                    var sw5 = new Stopwatch();
                    sw5.Start();
                    ReportLookups.SetTripsDerivedData(switchManager.TransIdGsaSwitch, recKeyList, _clientStore);
                    sw5.Stop();
                    LOG.Debug($"HibTripsDerivedData Lookup took {sw5.Elapsed.TotalSeconds} (sec)");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(loadType), loadType, null);
            }
        }

        public string HandleLookupFieldLeg(UserReportColumnInformation column, RawData mainRec, int seqNo, ColumnValueRulesFactory factory)
        {
            return _lookupLegFieldHandler.HandleLookupFieldLeg(column, mainRec, seqNo, factory);
        }

        public string HandleLookupFieldLegAir(UserReportColumnInformation column, RawData mainRec, int seqNo)
        {
            return _lookupLegAirFieldHandler.HandleLookupFieldLegAir(column, mainRec, seqNo);
        }

        public string HandleLookupFieldLegRail(UserReportColumnInformation column, RawData mainRec, int seqNo)
        {
            return _lookupLegRailFieldHandler.HandleLookupFieldLegRail(column, mainRec, seqNo);
        }

        public string HandleLookupFieldTrip(UserReportColumnInformation column, RawData mainRec, int tripSummaryLevel, ColumnValueRulesFactory factory)
        {
            return _lookupTripFieldHandler.HandleLookupFieldTrip(column, mainRec, tripSummaryLevel, factory);
        }

        public string HandleLookupFieldCar(UserReportColumnInformation column, RawData mainRec, int seqNo)
        {
            return _lookupLegCarFieldHandler.HandleLookupFieldCar(column, mainRec, seqNo);
        }

        public string HandleLookupFieldHotel(bool isPreview, UserReportColumnInformation column, RawData mainRec, int seqNo = 0)
        {
            return _lookupHotelFieldHandler.HandleLookupFieldHotel(isPreview, column, mainRec, seqNo);
        }

        public string HandleLookupFieldChangeLog(UserReportColumnInformation column, RawData mainRec, int segctr)
        {
            var rec = _userDefinedParams.ChangeLogLookup[mainRec.RecKey].FirstOrDefault(x => x.Segctr == segctr);

            if (rec == null) return string.Empty;

            switch (column.Name)
            {
                case "CANCELTIME":
                    //iif(TCL.changecode=101,ttoc(TCL.changstamp),'')
                    return rec.Changecode.ToString(CultureInfo.InvariantCulture).Equals("101") ? rec.Changstamp.GetValueOrDefault().ToShortDateString() : string.Empty;
                case "CHANGREASN":
                    //luChgDesc2(TCL.changecode)
                    return LookupFunctions.LookupChangeDescirption(rec.Changecode, _globals.UserLanguage, _masterStore);
                default:
                    return ReportBuilder.GetValueAsString(rec, column.Name);
            }
        }

        public string HandleLookupFieldMktSegs(UserReportColumnInformation column, RawData mainRec, int segnum, ColumnValueRulesFactory factory)
        {
            return _lookupMktSegsFieldHandler.HandleLookupFieldMktSegs(column, mainRec, segnum, factory);
        }

        public string HandleLookupFieldMiscSegs(UserReportColumnInformation column, RawData mainRec)
        {
            return _lookupMiscSegsFieldHandler.HandleLookupFieldMiscSegsLegacy(column, mainRec);
        }


        public string HandleLookupFieldMiscSegsTour(UserReportColumnInformation column, RawData mainRec, int seqNo)
        {
            return _lookupMiscSegsTourFieldHandler.HandleLookupFieldMiscSegsTour(column, mainRec, seqNo);
        }

        public string HandleLookupFieldMiscSegsCruise(UserReportColumnInformation column, RawData mainRec, int seqNo)
        {
            return _lookupMiscSegsCruiseFieldHandler.HandleLookupFieldMiscSegsCruise(column, mainRec, seqNo);
        }

        public string HandleLookupFieldMiscSegsLimo(UserReportColumnInformation column, RawData mainRec, int seqNo)
        {
            return _lookupMiscSegsLimoFieldHandler.HandleLookupFieldMiscSegsLimo(column, mainRec, seqNo);
        }

        public string HandleLookupFieldMiscSegsRailTicket(UserReportColumnInformation column, RawData mainRec, int seqNo, ColumnValueRulesFactory factory)
        {
            return _lookupMktSegsRailTickerFieldHandler.HandleLookupFieldMiscSegsRailTicket(column, mainRec, seqNo, factory);
        }

        public string HandleLookupFieldTravAuth(UserReportColumnInformation column, RawData mainRec)
        {
            return _lookupTravAuhtFieldHandler.HandleLookupFieldTravAuth(column, mainRec);
        }

        public string HandleLookupFieldAuthorizer(UserReportColumnInformation column, RawData mainRec)
        {
            return _lookupAuthorizerFieldHandler.HandleLookupFieldAuthorizer(column, mainRec);
        }

        public string HandleLookupFieldServiceFee(UserReportColumnInformation column, RawData mainRec, int seqNo)
        {
            return _lookupServiceFeeFieldHandler.HandleLookupFieldServiceFee(column, mainRec, seqNo);
        }
    }
}
