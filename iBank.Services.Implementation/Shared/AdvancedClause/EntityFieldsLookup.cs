using Domain.Helper;
using iBank.Entities.ClientEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Services;
using Domain;

namespace iBank.Services.Implementation.Shared.AdvancedClause
{
    public class EntityFieldsLookup
    {
        private readonly ICacheService _cache;

        // reservation table fields
        private readonly IList<string> _ibLegs;
        private readonly IList<string> _ibTrips;
        private readonly IList<string> _ibCars;
        private readonly IList<string> _ibHotel;
        private readonly IList<string> _ibMiscSegs;
        private readonly IList<string> _ibMktSegs;
        private readonly IList<string> _ibSvcFees;
        private readonly IList<string> _ibUdids;
        private readonly IList<string> _ibTravauth;
        private readonly IList<string> _ibTravauthorizers;
        private readonly IList<string> _changelog;
        private readonly List<string> _reservationTableList = new List<string>() { "ibtrips", "iblegs", "ibcar", "ibhotel", "ibmiscsegs", "ibmktsSegs", "ibsvcfees", "ibudids", "ibtravauth", "ibtravauthorizers", "changelog"};
        private readonly List<string> _reservationTablePrefixList = new List<string>() { "T1", "T2", "T4", "T5", "TMS", "TMK", "T6A", "T3", "TA1", "TA2", "TCL" };
        // back office table fields
        private readonly IList<string> _hibLegs;
        private readonly IList<string> _hibTrips;
        private readonly IList<string> _hibCars;
        private readonly IList<string> _hibHotel;
        private readonly IList<string> _hibMiscSegs;
        private readonly IList<string> _hibMktSegs;
        private readonly IList<string> _hibSvcFees;
        private readonly IList<string> _hibUdids;
        private readonly IList<string> _hibServices;
        private readonly List<string> _backOfficeTableList;
        private readonly List<string> _backOfficeTablePrefixList;

        public List<string> FieldsNotFound = new List<string>();

        public EntityFieldsLookup()
        {
            _cache = new CacheService();
            // reservation table fields
            _ibLegs = GetLookupFields<iblegs>(CacheKeys.ibLegsFields);
            _ibTrips = GetLookupFields<ibtrips>(CacheKeys.ibTripsFields);
            _ibCars = GetLookupFields<ibcar>(CacheKeys.ibCarsFields);
            _ibHotel = GetLookupFields<ibhotel>(CacheKeys.ibHotelFields);
            _ibMiscSegs = GetLookupFields<ibMiscSegs>(CacheKeys.ibMiscSegsFields);
            _ibMktSegs = GetLookupFields<ibMktSegs>(CacheKeys.ibMktSegsFields);
            _ibSvcFees = GetLookupFields<ibSvcFees>(CacheKeys.ibSvcFeesFields);
            _ibUdids = GetLookupFields<ibudids>(CacheKeys.ibUdidsFields);
            _ibTravauth = GetLookupFields<ibtravauth>(CacheKeys.ibTravauthFields);
            _ibTravauthorizers = GetLookupFields<ibTravAuthorizers>(CacheKeys.ibTravauthorizersFields);
            _changelog = GetLookupFields<changelog>(CacheKeys.changeLogFields);

            // back office table fields
            _hibLegs = GetLookupFields<hiblegs>(CacheKeys.hibLegsFields);
            _hibTrips = GetLookupFields<hibtrips>(CacheKeys.hibTripsFields);
            _hibCars = GetLookupFields<hibcars>(CacheKeys.hibCarsFields);
            _hibHotel = GetLookupFields<hibhotel>(CacheKeys.hibHotelFields);
            _hibMiscSegs = GetLookupFields<hibMiscSegs>(CacheKeys.hibMiscSegsFields);
            _hibMktSegs = GetLookupFields<hibMktSegs>(CacheKeys.hibMktSegsFields);
            _hibSvcFees = GetLookupFields<hibSvcFees>(CacheKeys.hibSvcFeesFields);
            _hibUdids = GetLookupFields<hibudids>(CacheKeys.hibUdidsFields);
            _hibServices = GetLookupFields<hibudids>(CacheKeys.hibServicesFields);

            if (Features.FieldPreFix.IsEnabled())
            {
                _backOfficeTableList = new List<string>() { "hibtrips", "hiblegs", "hibcars", "hibhotel", "hibmiscsSegs", "hibmktsegs", "hibsvcfees", "hibudids", "hibservices", "hibtripsderiveddata" };
                _backOfficeTablePrefixList = new List<string>() { "T1", "T2", "T4", "T5", "TMS", "TMK", "T6A", "T3", "T6A", "TD" };
            }
            else
            {
                _backOfficeTableList = new List<string>() { "hibtrips", "hiblegs", "hibcars", "hibhotel", "hibmiscsSegs", "hibmktsegs", "hibsvcfees", "hibudids", "hibservices" };
                _backOfficeTablePrefixList = new List<string>() { "T1", "T2", "T4", "T5", "TMS", "TMK", "T6A", "T3", "T6A" };
            }
        }

        public IList<string> GetTablesNeeedForParameters(List<string> parameters, bool isReservationReport)
        {
            return isReservationReport 
                ? GetTablesForReservationReport(parameters) 
                : GetTablesForBackOfficeReport(parameters);
        }
        
        public IList<string> GetTablesForReservationReport(List<string> parameters, List<string> tablesToTryFirst = null)
        {
            FieldsNotFound = new List<string>();
            tablesToTryFirst = tablesToTryFirst ?? new List<string>(); // if null set to new list so we don't have to check if it is null later
            parameters = parameters ?? new List<string>();
            var tablesNeeded = new List<string>();

            foreach (var param in parameters)
            {
                var isFieldInTablesToTryFirst = false;
                // First we check existing tables in case the fields/parameters exists in multiple tables. For example the confirmno field exists in both
                // the ibcar and the ibhotel tables. So, if we are using the ibhotel table in the from clause, then we want to return that table and not
                // the ibcar table.
                foreach (var table in tablesToTryFirst)
                {
                    if (!IsFieldFoundInReservationTable(table, param)) continue;
                    tablesNeeded.Add(table);
                    isFieldInTablesToTryFirst = true;
                    break;
                }

                if (isFieldInTablesToTryFirst) continue;

                // Remove tables that have already been checked
                tablesToTryFirst.ForEach(table => _reservationTableList.Remove(table.ToLower()));

                foreach (var table in _reservationTableList)
                {
                    // If the field is not in any of the existing tables, then look in all the other tables to find out where that field belongs.
                    if (!IsFieldFoundInReservationTable(table, param)) continue;
                    tablesNeeded.Add(table);
                    break;
                }
            }

            return tablesNeeded.Distinct().ToList();
        }

        private bool IsFieldFoundInReservationTable(string tableName, string param)
        {
            switch (tableName.ToLower())
            {
                case "ibtrips":
                    if (_ibTrips.Contains(param, StringComparer.OrdinalIgnoreCase)) return true;
                    break;
                case "iblegs":
                    if (_ibLegs.Contains(param, StringComparer.OrdinalIgnoreCase)) return true;
                    break; 
                case "ibcar":
                    if (_ibCars.Contains(param, StringComparer.OrdinalIgnoreCase)) return true;
                    break; 
                case "ibhotel":
                    if (_ibHotel.Contains(param, StringComparer.OrdinalIgnoreCase)) return true;
                    break; 
                case "ibmiscsegs":
                    if (_ibMiscSegs.Contains(param, StringComparer.OrdinalIgnoreCase)) return true;
                    break; 
                case "ibmktsegs":
                    if (_ibMktSegs.Contains(param, StringComparer.OrdinalIgnoreCase)) return true;
                    break; 
                case "ibsvcfees":
                    if (_ibSvcFees.Contains(param, StringComparer.OrdinalIgnoreCase)) return true;
                    break;
                case "ibudids":
                    if (_ibUdids.Contains(param, StringComparer.OrdinalIgnoreCase)) return true;
                    break;
                case "ibtravauth":
                    if (_ibTravauth.Contains(param, StringComparer.OrdinalIgnoreCase)) return true;
                    break;
                case "ibtravauthorizers":
                    if (_ibTravauthorizers.Contains(param, StringComparer.OrdinalIgnoreCase)) return true;
                    break;
                case "changelog":
                    if (_changelog.Contains(param, StringComparer.OrdinalIgnoreCase)) return true;
                    break;

                default: 
                    return false;
            }
            return false;
        }

        public IList<string> GetTablesForBackOfficeReport(List<string> parameters, List<string> tablesToTryFirst = null)
        {
            FieldsNotFound = new List<string>();
            tablesToTryFirst = tablesToTryFirst ?? new List<string>(); // if null set to new list so we don't have to check if it is null later
            parameters = parameters ?? new List<string>();
            var tablesNeeded = new List<string>();
            foreach (var param in parameters)
            {
                var isFieldInTablesToTryFirst = false;
                // First we check existing tables in case the fields/parameters exists in multiple tables. For example the confirmno field exists in both
                // the hibcar and the hibhotel tables. So, if we are using the ibhotel table in the from clause, then we want to return that table and not
                // the hibcar table.
                foreach (var table in tablesToTryFirst)
                {
                    if (!IsFieldFoundInBackOfficeTable(table, param)) continue;
                    tablesNeeded.Add(table);
                    isFieldInTablesToTryFirst = true;
                    break;
                }

                if (isFieldInTablesToTryFirst) continue;

                // Remove tables that have already been checked
                tablesToTryFirst.ForEach(table => _backOfficeTableList.Remove(table.ToLower()));

                bool wasParameterFound = false;
                foreach (var table in _backOfficeTableList)
                {
                    // If the field is not in any of the existing tables, then look in all the other tables to find out where that field belongs.
                    if (!IsFieldFoundInBackOfficeTable(table, param)) continue;
                    tablesNeeded.Add(table);
                    wasParameterFound = true;
                    break;
                }
                // there are some parameters that only exist in the _reservationTableList. I.e, the seat field only exists in the iblegs table 
                // but not in the hiblegs talbe
                if (!wasParameterFound) FieldsNotFound.Add(param);
            }

            return tablesNeeded.Distinct().ToList();
        }

        private bool IsFieldFoundInBackOfficeTable(string tableName, string param)
        {
            switch (tableName.ToLower())
            {
                case "hibtrips":
                    if (_hibTrips.Contains(param, StringComparer.OrdinalIgnoreCase)) return true;
                    break;
                case "hiblegs":
                    if (_hibLegs.Contains(param, StringComparer.OrdinalIgnoreCase)) return true;
                    break; 
                case "hibcars":
                    if (_hibCars.Contains(param, StringComparer.OrdinalIgnoreCase)) return true;
                    break; 
                case "hibhotel":
                    if (_hibHotel.Contains(param, StringComparer.OrdinalIgnoreCase)) return true;
                    break; 
                case "hibmiscsegs":
                    if (_hibMiscSegs.Contains(param, StringComparer.OrdinalIgnoreCase)) return true;
                    break; 
                case "hibmktsegs":
                    if (_hibMktSegs.Contains(param, StringComparer.OrdinalIgnoreCase)) return true;
                    break; 
                case "hibsvcfees":
                    if (_hibSvcFees.Contains(param, StringComparer.OrdinalIgnoreCase)) return true;
                    break;
                case "hibudids":
                    if (_hibUdids.Contains(param, StringComparer.OrdinalIgnoreCase)) return true;
                    break;
                case "hiservices":
                    if (_hibServices.Contains(param, StringComparer.OrdinalIgnoreCase)) return true;
                    break;

                default: 
                    return false;
            }
            return false;
        }

        public string GetTableThatFieldBelongsTo(string field, bool isReservationReport, List<string> fromClauseTables)
        {
            //lets make use of the existing code here
            var tables = isReservationReport
                             ? GetTablesForReservationReport(new List<string> { field }, fromClauseTables)
                             : GetTablesForBackOfficeReport(new List<string> { field }, fromClauseTables);

            //we only want the first item, as we sort of faked the list in order to reuse code
            return tables.Any() 
                ? tables[0] 
                : "";
        }

        private IList<string> GetLookupFields<T>(CacheKeys key) where T : new()
        {
            IList<string> names;
            if (!_cache.TryGetValue(key, out names))
            {
                names = GetFields(new T());
                _cache.Set(key, names, DateTime.Now.AddDays(1));
            }

            return names;
        }
        
        private List<string> GetFields<T>(T entity)
        {
            var props = entity.GetType().GetProperties();
            //make sure the field names are in lower case
            return props.Select(x => x.Name.ToLower().Replace("_","")).ToList();  //keywords will have an underscore on them, we want to remove those
        }

        public List<string> GetTableFields(string tableName)
        {
            // reservation table fields
            if (tableName.Equals("iblegs", StringComparison.OrdinalIgnoreCase)) return _ibLegs.ToList();
            if (tableName.Equals("ibTrips", StringComparison.OrdinalIgnoreCase)) return _ibTrips.ToList();
            if (tableName.Equals("ibCar", StringComparison.OrdinalIgnoreCase)) return _ibCars.ToList();
            if (tableName.Equals("ibHotel", StringComparison.OrdinalIgnoreCase)) return _ibHotel.ToList();
            if (tableName.Equals("ibMiscSegs", StringComparison.OrdinalIgnoreCase)) return _ibMiscSegs.ToList();
            if (tableName.Equals("ibMktSegs", StringComparison.OrdinalIgnoreCase)) return _ibMktSegs.ToList();
            if (tableName.Equals("ibSvcFees", StringComparison.OrdinalIgnoreCase)) return _ibSvcFees.ToList();
            if (tableName.Equals("ibUdids", StringComparison.OrdinalIgnoreCase)) return _ibUdids.ToList();
            if (tableName.Equals("ibTravauth", StringComparison.OrdinalIgnoreCase)) return _ibTravauth.ToList();
            if (tableName.Equals("ibtravauthorizers", StringComparison.OrdinalIgnoreCase)) return _ibTravauthorizers.ToList();
            if (tableName.Equals("changelog", StringComparison.OrdinalIgnoreCase)) return _changelog.ToList();

            // back office table fields
            if (tableName.Equals("hibLegs", StringComparison.OrdinalIgnoreCase)) return _hibLegs.ToList();
            if (tableName.Equals("hibTrips", StringComparison.OrdinalIgnoreCase)) return _hibTrips.ToList();
            if (tableName.Equals("hibCars", StringComparison.OrdinalIgnoreCase)) return _hibCars.ToList();
            if (tableName.Equals("hibHotel", StringComparison.OrdinalIgnoreCase)) return _hibHotel.ToList();
            if (tableName.Equals("hibMiscSegs", StringComparison.OrdinalIgnoreCase)) return _hibMiscSegs.ToList();
            if (tableName.Equals("hibMktSegs", StringComparison.OrdinalIgnoreCase)) return _hibMktSegs.ToList();
            if (tableName.Equals("hibSvcFees", StringComparison.OrdinalIgnoreCase)) return _hibSvcFees.ToList();
            if (tableName.Equals("hibUdids", StringComparison.OrdinalIgnoreCase)) return _hibUdids.ToList();
            if (tableName.Equals("hibServices", StringComparison.OrdinalIgnoreCase)) return _hibServices.ToList();

            return new List<string>();
        }
        
        public string GetTablePrefix(string tableName, bool isReservation)
        {
            if (isReservation)
            {
                return _reservationTablePrefixList[_reservationTableList.IndexOf(tableName.ToLower())];
            }
            else
            {
                return _backOfficeTablePrefixList[_backOfficeTableList.IndexOf(tableName.ToLower())];
            }                       
        }
        
        public bool HasRecKeyField(string tableName)
        {
            var fieldList = GetTableFields(tableName);
            return fieldList.IndexOf("reckey") > -1;
        }
    }
}
