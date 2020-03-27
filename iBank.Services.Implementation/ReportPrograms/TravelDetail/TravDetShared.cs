using Domain.Helper;
using Domain.Models.ReportPrograms.TravelDetail;
using Domain.Orm.Classes;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities;
using iBank.Services.Implementation.Utilities.ClientData;
using iBankDomain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;
using Domain.Services;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.TravelDetail
{
    public class TravDetShared
    {
        private readonly ReportRunConditionals _conditionals = new ReportRunConditionals();

        private readonly TravDetSharedSqlCreator _creator = new TravDetSharedSqlCreator();

        private DataRetriever _retriever;

        /// <summary>
        /// Default is true, if not explicitly set
        /// </summary>
        private bool _applyToSegments
        {
            get { return ApplyToLegOrSegment.IsNullOrWhiteSpace() || ApplyToLegOrSegment.Equals("2"); }
        }

        private string ApplyToLegOrSegment { get; set; }

        private BuildWhere BuildWhere { get; set; }

        public bool UseServiceFees;

        public List<CarRawData> Cars { get; set; }

        private RoutingCriteria RoutingCriteria { get; set; }

        private ReportGlobals Globals { get; set; }
        private bool GoodUdid { get; set; }

        private IClientDataStore _clientStore;
        private IClientDataStore ClientDataStore
        {
            get
            {
                return _clientStore ?? (_clientStore = new ClientDataStore(Globals.AgencyInformation.ServerName,
                           Globals.AgencyInformation.DatabaseName));
            }
        }

        public bool ExcludeServiceFees { get; set; }

        public List<TravDetRawData> FilteredRouteData { get; set; }

        public List<HotelRawData> Hotels { get; set; }

        public bool IsReservation { get; set; }

        public List<LegRawData> Legs { get; set; }

        public List<TravDetRawData> RawDataList { get; set; }

        public List<SegmentRawData> Segments { get; set; }

        public List<ServiceFeesRawData> ServiceFees { get; set; }

        // Build raw data list
        public bool GetRawData(BuildWhere buildWhere)
        {
            BuildWhere = buildWhere;
            Globals = BuildWhere.ReportGlobals;
            InitialChecks();

            RawDataList = new List<TravDetRawData>();
            Segments = new List<SegmentRawData>();
            Legs = new List<LegRawData>();
            Cars = new List<CarRawData>();
            Hotels = new List<HotelRawData>();
            ServiceFees = new List<ServiceFeesRawData>();
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientDataStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false,
                buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false,
                legDit: false, ignoreTravel: false)) return false;

            BuildWhere.BuildAdvancedClauses();  
            
            BuildWhere.AddSecurityChecks();

            // Generate sql and execute
            var sql = _creator.GetTripDataSql(BuildWhere, GoodUdid, IsReservation);
            _retriever = new DataRetriever(ClientDataStore.ClientQueryDb);
            //RawDataList = _retriever.GetData<TravDetRawData>(sql, BuildWhere, false, false, IsReservation).ToList();
            RawDataList = _retriever.GetData<TravDetRawData>(sql, BuildWhere, false, false, IsReservation, includeAllLegs: true, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();

            // remove duplicate records
            var rawDataListGroup = RawDataList.GroupBy(g => new { g.RecKey, g.Invoice, g.Ticket, g.Break1, g.Break2, g.Break3, g.PassFrst, g.PassLast, g.ValCarMode, g.InvDate, g.DepDate, g.SvcFee });
            RawDataList = rawDataListGroup.Select(g => g.First()).ToList();

            if (!_conditionals.DataExists(RawDataList, Globals)) return false;
            
            var pusher = new ReportDelayer(ClientDataStore, new MasterDataStore(), Globals);
            if (!_conditionals.IsUnderOfflineThreshold(RawDataList.Count, Globals, pusher)) return false;

            // Get data that may not be in RawDataList
            Segments = GetSegmentData();
            // remove duplicate records
            var segmentGroup = Segments.GroupBy(g => new { g.AirCurrTyp, g.TripStart, g.SourceAbbr, g.SegStatus, g.RecLoc, g.PassLast, g.PassFrst, g.InvDate, g.fltno, g.MiscAmt, g.Connect, g.ClassCode, g.Break3, g.Break2, g.Break1, g.ActFare, g.Acct, g.DitCode, g.DomIntl, g.Origin, g.Destinat, g.Mode, g.DepTime, g.Airline, g.RDepDate, g.RArrDate, g.Seg_Cntr, g.Miles, g.RecKey, g.SeqNo, g.AirCo2 });
            Segments = segmentGroup.Select(g => g.First()).ToList();

            Legs = GetLegData();
            // remove duplicate records
            var legGroup = Legs.GroupBy(g => new { g.Connect, g.Class, g.ArrTime, g.DepTime, g.fltno, g.AirCurrTyp, g.TripStart, g.SourceAbbr, g.Break3, g.Break2, g.Break1, g.Origin, g.Destinat, g.Mode, g.Airline, g.RecKey, g.SeqNo, g.RDepDate, g.RArrDate, g.Exchange, g.Passlast, g.Passfrst, g.Invdate, g.ActFare, g.Plusmin, g.ClassCat, g.Miles, g.DitCode, g.AirCo2, g.AltCarCo2, g.AltRailCo2, g.HaulType, g.DomIntl, g.MiscAmt, g.ClassCode, g.Seg_Cntr });
            Legs = legGroup.Select(g => g.First()).ToList();

            Cars = GetCarData();
            // remove duplicate records
            var carGroup = Cars.GroupBy(g => new { g.BookDate, g.InvDate, g.RecKey, g.Company, g.Autostat, g.Autocity, g.RentDate, g.DateBack, g.ReasCoda, g.Abookrat, g.Milecost, g.RateType, g.Citycode, g.Confirmno, g.Cartrantyp, g.CarType, g.Days, g.CarCo2, g.CPlusMin, g.CarCurrTyp, g.PassFrst, g.PassLast, g.RecLoc, g.Acct, g.TripStart, g.TripEnd, g.DepDate, g.ArrDate, g.SourceAbbr });
            Cars = carGroup.Select(g => g.First()).ToList();

            Hotels = GetHotelData();
            // remove duplicate records
            var hotelGroup = Hotels.GroupBy(g => new { g.RecKey, g.ConfirmNo, g.HotCity, g.HotelNam, g.Metro, g.Acct, g.PassFrst, g.PassLast });
            Hotels = hotelGroup.Select(g => g.First()).ToList();

            if (UseServiceFees)
            {
                ServiceFees = GetServiceFeeData();
                //deleted "remove duplicate records" logic. See Bug 8853 - Defect 00431105 - Itinerary Detail Combined report failing to include all service fees for more information
            }

            return true;
        }

        // Get Car data without matching record in (h)ibtrips
        private List<CarRawData> GetCarData()
        {
            var sql = _creator.GetCarSql(BuildWhere, GoodUdid, IsReservation);
            //var cars = _retriever.GetData<CarRawData>(sql, BuildWhere, false, false, IsReservation).ToList();
            var cars = _retriever.GetData<CarRawData>(sql, BuildWhere, false, false, isReservationReport: IsReservation, includeAllLegs: true, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();

            return cars;
        }

        // Get Hotel data without matching record in (h)ibtrips
        private List<HotelRawData> GetHotelData()
        {
            var sql = _creator.GetHotelSql(BuildWhere, GoodUdid, IsReservation);
            //var hotels = _retriever.GetData<HotelRawData>(sql, BuildWhere, false, false, IsReservation).ToList();
            var hotels = _retriever.GetData<HotelRawData>(sql, BuildWhere, false, false, isReservationReport: IsReservation, includeAllLegs: true, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();

           return hotels;
        }

        // Get Leg data without matching record in (h)ibtrips
        private List<LegRawData> GetLegData()
        {
            var sql = _creator.GetLegSql(BuildWhere, GoodUdid, IsReservation);
            //var legs = _retriever.GetData<LegRawData>(sql, BuildWhere, true, false, IsReservation).ToList();
            var legs = _retriever.GetData<LegRawData>(sql, BuildWhere, true, false, IsReservation, includeAllLegs: true, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();

            return legs;
        }

        // Get segment data if filters are applied to segments
        private List<SegmentRawData> GetSegmentData()
        {
            var segments = new List<SegmentRawData>();
            if (!_applyToSegments) return segments;

            var tranTypeParm = BuildWhere.SqlParameters.FirstOrDefault(s => s.ParameterName == "t1TranType1");
            if (tranTypeParm != null) tranTypeParm.Value = "Z";

            var sql = _creator.GetLegSqlToCollapse(BuildWhere, GoodUdid, IsReservation);
            //var legDataToCollapse = _retriever.GetData<SegmentRawData>(sql, BuildWhere, true, false, IsReservation).ToList();
            var legDataToCollapse = _retriever.GetData<SegmentRawData>(sql, BuildWhere, true, false, IsReservation, includeAllLegs: true, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();

            return Collapser<SegmentRawData>.Collapse(legDataToCollapse, Collapser<SegmentRawData>.CollapseType.Both);
        }

        // Get service fee data
        private List<ServiceFeesRawData> GetServiceFeeData()
        {
            var serviceFees = new List<ServiceFeesRawData>();
            var useServiceFees = Globals.AgencyInformation.UseServiceFees && !IsReservation;
            var isTop10Version = Globals.ProcessKey == 59; // ibTopTravAll
            if (isTop10Version || !useServiceFees) return serviceFees;

            if (Globals.UseHibServices)
            {
                return GetServiceFeesFromHibServices();
            }

            var sql = _creator.GetServiceFeeFromSvcFeeTableSql(BuildWhere, GoodUdid, IsReservation);
            //var hibFees = _retriever.GetData<HibSvcFeesRawData>(sql, BuildWhere, false, false, IsReservation).ToList();
            var hibFees = _retriever.GetData<HibSvcFeesRawData>(sql, BuildWhere, false, false, IsReservation, includeAllLegs: true, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();
            if (!hibFees.Any()) return serviceFees;

            var recKeys =
                hibFees.Where(w => w.SvcFee != 0 && !w.RecLoc.IsNullOrWhiteSpace() && w.RecLoc != null)
                    .GroupBy(s => new { s.RecLoc, s.Invoice, s.Acct, s.PassLast, s.PassFrst },
                        (key, g) =>
                        {
                            var temp = g.ToList();
                            return new
                            {
                                key.Acct,
                                key.Invoice,
                                key.PassFrst,
                                key.PassLast,
                                key.RecLoc,
                                RecKey = temp.Min(r => r.RecKey)
                            };
                        });

            serviceFees = recKeys.Join(hibFees,
                rk => rk.RecKey,
                f => f.SvcFee,
                (recKey, fee) => new { recKey.RecKey, fee.SvcFee, fee.TranDate }
                ).ToList().GroupBy(s => s.RecKey).Select(s => new ServiceFeesRawData
                {
                    RecKey = s.Key,
                    SvcFee = s.Sum(x => x.SvcFee),
                    InvDate = s.First().TranDate
                }).ToList();

            return serviceFees;
        }

        // Get data from hibServices
        private List<ServiceFeesRawData> GetServiceFeesFromHibServices()
        {
            var sql = _creator.GetServiceFeesFromHibServices(BuildWhere, GoodUdid);
            //var serviceFees = _retriever.GetData<ServiceFeesRawData>(sql, BuildWhere, false, false, IsReservation).ToList();
            var serviceFees = _retriever.GetData<ServiceFeesRawData>(sql, BuildWhere, false, false, IsReservation, includeAllLegs: true, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();
            if (!ServiceFees.Any()) return serviceFees;
            serviceFees = serviceFees.GroupBy(s => s.RecKey).Select(s => new ServiceFeesRawData
            {
                RecKey = s.Key,
                SvcFee = s.Sum(x => x.SvcFee),
                InvDate = s.First().InvDate
            }).ToList();
            return serviceFees;
        }

        // Initialize and check variables
        private void InitialChecks()
        {
            ApplyToLegOrSegment = Globals.GetParmValue(WhereCriteria.RBAPPLYTOLEGORSEG);
            int udid;
            GoodUdid = int.TryParse(Globals.GetParmValue(WhereCriteria.UDIDNBR), out udid) && udid != 0;
            ExcludeServiceFees = IsReservation || Globals.IsParmValueOn(WhereCriteria.CBEXCLUDESVCFEES);
        }

        public bool ProcessDataShared(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery)
        {
            var applyRouteToCarHotelOnly = false;

            //** NEED TO FIND OUT IF THE ONLY CRITERIA IS DESTINATION **
            //** METRO/COUNTRY/REGION.  EASIEST WAY TO DO THAT IS TO  **
            //** SET THOSE CRITERIA TO BLANK, THEN CALL ibBldWhere    **
            //** AND SEE IF cWhereRout IS BLANK.                      **

            if (BuildWhere.HasRoutingCriteria && !BuildWhere.HasFirstOrigin && !BuildWhere.HasFirstDestination)
            {
                RoutingCriteria = RoutingCriteriaUtility.GetRoutingCriteria(BuildWhere.ReportGlobals);
                //Can't call BuildWhere.ClearRouteCriteria because not all criteria needs to be cleared.
                ClearRouteCriteria();

                if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: false, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: false, buildDateWhere: false,inMemory: true, isRoutingBidirectional: false, legDit: false, ignoreTravel: false)) return true;

                applyRouteToCarHotelOnly = !BuildWhere.HasRoutingCriteria;

                RoutingCriteriaUtility.RestoreRouteCriteria(BuildWhere.ReportGlobals, RoutingCriteria);
            }

            //We need to keep all of the rawData until after we see if there are hotel or car only records that need to be retained.

            //** 08/09/2013 - IF THERE IS ROUTING CRITERIA, BUT IT ONLY INVOLVES **
            //** DESTINATION METRO/COUNTRY/REGION, THEN WE NEED TO DEAL WITH IT  **
            //** AT THIS POINT BY ADDING RECORDS BACK TO THE curRecs CURSOR FOR  **
            //** HOTEL-ONLY AND/OR CAR-ONLY RECORDS THAT MATCH THAT CRITERIA.    **
            if (!BuildWhere.HasRoutingCriteria && !BuildWhere.HasFirstOrigin && !BuildWhere.HasFirstDestination)
            {
                FilteredRouteData = RawDataList;
                return _conditionals.DataExists(FilteredRouteData, Globals);
            }

            List<LegRawData> dataList;

            //TODO: when the GetCarHotelOnlyReckeys section is finished, this list will be helpful for
            //keeping RawData records that may need to be readded.
            //The FoxPro version of the report (as of 6/14/2016), GetCarHotelOnlyReckeys
            //does not execute because the website option to choose from is empty. The conversion is
            //left incomplete because the FoxPro code seems to have bugs.
            if (_applyToSegments)
            {
                Segments = BuildWhere.ApplyWhereRoute(Segments, false);
                FilteredRouteData =
                    RawDataList.Where(s => Segments.Exists(t => t.RecKey == s.RecKey)).ToList();
            }
            else
            {
                dataList = BuildWhere.ApplyWhereRoute(Legs, true, false);
                FilteredRouteData =
                    RawDataList.Where(s => dataList.Exists(t => t.RecKey == s.RecKey)).ToList();
            }

            if (!applyRouteToCarHotelOnly) return _conditionals.DataExists(FilteredRouteData, Globals);

            List<int> carHotelOnlyRecKeys = GetCarHotelOnlyReckeys();
            FilteredRouteData.AddRange(RawDataList.Where(s => s.ValCarr.Trim() == "ZZ" && carHotelOnlyRecKeys.IndexOf(s.RecKey) >= 0));

            return _conditionals.DataExists(FilteredRouteData, Globals);
        }

        /// <summary>
        /// This will include trips for which there is only hotel or car that match the destination filter.
        /// </summary>
        private List<int> GetCarHotelOnlyReckeys()
        {
            var recKeys = new List<int>();

            var inList = !Globals.IsParmValueOn(WhereCriteria.NOTINMETROORGS);
            var pickList = Globals.GetParmValue(WhereCriteria.INMETRODESTS).Trim(); //192
            if (pickList.IsNullOrWhiteSpace()) pickList = Globals.GetParmValue(WhereCriteria.METRODEST).Trim(); //191
            var pickListParms = new PickListParms(Globals);
            pickListParms.ProcessList(pickList, string.Empty, "METRO");
            var plist = pickListParms.PickList;
            //Note cars and hotels must be included if any car or hotel for a trip has the filter destination.
            //So, don't remove items from Cars and Hotels, but add trip to FilteredData from RawData if any 
            //Hotel or Car matches filter.
            if (plist.Any())
            {
                recKeys.AddRange(
                    Cars.Where(c => plist.Any(p => p == c.Citycode.Trim()) == inList).
                        Select(s => s.RecKey).Distinct());
                recKeys.AddRange(
                    Hotels.Where(h => plist.Any(p => p == h.Metro.Trim()) == inList && recKeys.IndexOf(h.RecKey) < 0).
                        Select(s => s.RecKey).Distinct());
                return recKeys;
            }

            inList = !Globals.IsParmValueOn(WhereCriteria.NOTINORIGCOUNTRY);
            pickList = Globals.GetParmValue(WhereCriteria.INDESTCOUNTRY).Trim();
            if (pickList.IsNullOrWhiteSpace())
                pickList = Globals.GetParmValue(WhereCriteria.DESTCOUNTRY).Trim();
            pickListParms.ProcessList(pickList, string.Empty, "COUNTRY");
            plist = pickListParms.PickList;

            var cache = new CacheService();
            var store = new MasterDataStore();

            var metroCountries = (from c in CountriesLookup.GetCountries(cache, store.MastersQueryDb)
                                  join m in MetrosLookup.GetMetros(cache, store.MastersQueryDb)
                                  on c.CountryCode equals m.CountryCode
                                  select new { m.MetroCode, m.CountryCode, c.RegionCode }).ToList();

            if (plist.Any())
            {
                var countries = metroCountries.Where(s => plist.Contains(s.CountryCode.Trim()) == inList).ToList();
                recKeys.AddRange(Cars.Where(c => countries.Exists(x => x.MetroCode.Trim() == c.Citycode.Trim()) == inList).
                        Select(s => s.RecKey).Distinct());
                recKeys.AddRange(
                    Hotels.Where(h => countries.Exists(x => x.MetroCode.Trim() == h.Metro.Trim()) == inList).
                        Select(s => s.RecKey).Distinct());
                return recKeys;
            }

            inList = !Globals.IsParmValueOn(WhereCriteria.NOTINORIGREGION);
            pickList = Globals.GetParmValue(WhereCriteria.INDESTREGION).Trim();
            if (pickList.IsNullOrWhiteSpace())
                pickList = Globals.GetParmValue(WhereCriteria.DESTREGION).Trim();
            pickListParms.ProcessList(pickList, string.Empty, "REGION");
            plist = pickListParms.PickList;

            if (!plist.Any()) return recKeys;
            var regions =
                metroCountries.Where(s => plist.Contains(s.RegionCode.Trim()) != inList).ToList();
            recKeys.AddRange(
               Cars.Where(c => regions.Exists(x => x.MetroCode.Trim() == c.Citycode.Trim()) == inList).
                   Select(s => s.RecKey).Distinct());
            recKeys.AddRange(
                Hotels.Where(h => regions.Exists(x => x.MetroCode.Trim() == h.Metro.Trim()) == inList).
                    Select(s => s.RecKey).Distinct());
            return recKeys;
        }

        private void ClearRouteCriteria()
        {
            Globals.SetParmValue(WhereCriteria.METRODEST, string.Empty);
            Globals.SetParmValue(WhereCriteria.INMETRODESTS, string.Empty);
            Globals.SetParmValue(WhereCriteria.DESTCOUNTRY, string.Empty);
            Globals.SetParmValue(WhereCriteria.INDESTCOUNTRY, string.Empty);
            Globals.SetParmValue(WhereCriteria.DESTREGION, string.Empty);
            Globals.SetParmValue(WhereCriteria.INDESTREGION, string.Empty);
        }
    }
}

