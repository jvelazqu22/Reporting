using Domain.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Constants;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using Domain;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.Shared.AdvancedClause
{
    public class AdvancedParameterAppender
    {
        private readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly List<string> _reservationTables = new List<string>
                                                                      {
                                                                        "iblegs",
                                                                        "ibtrips",
                                                                        "ibcar",
                                                                        "ibhotel",
                                                                        "ibMiscSegs",
                                                                        "ibMktSegs",
                                                                        "ibSvcFees",
                                                                        "ibudids",
                                                                        "ibtravauth",
                                                                        "ibtravauthorizers",
                                                                        "changelog"
                                                                      };

        private static readonly List<string> _backOfficeTables = new List<string>
                                                                     {
                                                                         "hiblegs",
                                                                         "hibtrips",
                                                                         "hibcars",
                                                                         "hibhotel",
                                                                         "hibMiscSegs",
                                                                         "hibMktSegs",
                                                                         "hibSvcFees",
                                                                         "hibudids",
                                                                         "hibServices"
                                                                     };

        public string FromClause { get; set; }

        public string KeyWhereClause { get; set; }

        public string SelectClause { get; set; }

        public string WhereClause { get; set; }

        private readonly bool _isReservationReport;
        public List<string> FieldsNotFound = new List<string>();

        private readonly ClauseParser _clauseParser = new ClauseParser();

        public AdvancedParameterAppender(string fromClause, string keyWhereClause, string selectClause, string whereClause)
        {
            FromClause = fromClause;
            KeyWhereClause = keyWhereClause;
            _isReservationReport = IsReservationReport(fromClause);
            SelectClause = selectClause;
            WhereClause = whereClause;
        }

        private bool IsReservationReport(string fromClause)
        {
            //if the from clause has a 'hib' prefixed table we know it is looking at history tables, therefore it is not a reservation report
            return !fromClause.ToLower().Contains("hib");
        }

        public void AppendMissingTableParamPairs(BuildWhere buildWhere)
        {
            List<AdvancedParameter> advancedParams = buildWhere.ReportGlobals.AdvancedParameters.Parameters;

            if (!advancedParams.Any()) return;
            var advParameters = advancedParams.Select(x => x.AdvancedFieldName).ToList();
            
            var tablesAlreadyInFromClause = GetTablesAlreadyInFromClause(FromClause, _isReservationReport);

            IList<string> tablesNeededForAdvancedParams;
            if (buildWhere.AdvancedParameterQueryTableRefList.Count != 0)
            {
                //need to change from hibsvcfees to hibservices table if UseHibServices is trued on
                tablesNeededForAdvancedParams = GetTableNames(buildWhere.AdvancedParameterQueryTableRefList, _isReservationReport, buildWhere.ReportGlobals.UseHibServices);
            }
            else
            {
                tablesNeededForAdvancedParams = GetTablesNeededForAdvancedParamsAndSaveMissingFields(advParameters, _isReservationReport);
            }

            if (!tablesNeededForAdvancedParams.Any()) return;

            var tablesThatNeedToBeAdded = GetTablesThatDontExist(tablesAlreadyInFromClause, tablesNeededForAdvancedParams);
            tablesThatNeedToBeAdded = CleanTablesNeededIfThereAreCorrespondingViewsInPlay(FromClause, tablesThatNeedToBeAdded, _isReservationReport);

            if (!tablesThatNeedToBeAdded.Any()) return;

            //before we can add table(s) we need to prefix all the existing fields, so that we are sure we aren't going to end up with any ambiguous fields due to the new table(s)

            var tablePrefixPairs = new Dictionary<string, string>();
            tablePrefixPairs = _clauseParser.BuildTablePrefixPairsUseEntityFieldsLookup(FromClause, IsReservationReport(FromClause));
            
            
            SelectClause = _clauseParser.PrefixSelectClause(SelectClause, _isReservationReport, tablePrefixPairs);

            var defaultPrefix = tablePrefixPairs.Values.FirstOrDefault();
                       
            FromClause = AddTablesToFromClauseUsingEntityFieldsLookup(tablesThatNeedToBeAdded, FromClause, _isReservationReport);
            KeyWhereClause = AddReferencesToDefaultPrefixUsingEntityFieldsLookup(tablesThatNeedToBeAdded, KeyWhereClause, defaultPrefix, _isReservationReport);
            

            WhereClause = _clauseParser.PrefixWhereClause(WhereClause, _isReservationReport, tablePrefixPairs);
        }

        public IList<string> GetTableNames(List<AvancedParameterQueryTableRef> tableRefList, bool isReservationReport, bool useHibservices)
        {
            IList<string> tableNameList = new List<string>();
            foreach (var item in tableRefList)
            {
                var name = GetTableName(item, isReservationReport, useHibservices);
                if (!string.IsNullOrEmpty(name)) tableNameList.Add(name);
            }
            return tableNameList;
        }

        public string GetTableName(AvancedParameterQueryTableRef tableRef, bool isReservationReport, bool useHibservices)
        {
            if (isReservationReport)
            {
                return GetReservationTableName(tableRef);
            }
            else
            {
                return GetBackofficeTableName(tableRef, useHibservices);
            }
        }

        public string GetReservationTableName(AvancedParameterQueryTableRef tableRef)
        {
            switch (tableRef.TableName)
            {
                case "HOTEL":
                    return "ibHotel";
                case "CAR":
                case "AUTO":
                    return "ibcar";
                case "SVCFEE":
                    return "ibSvcFees";
                case "LEGS":
                case "AIRLEG":
                    return "ibLegs";
                case "CHGLOG":
                    return "changelog";
                case "AUTHRZR":
                    return "ibTravAuthorizers";
                case "TRAVAUTH":
                    return "ibtravauth";
                case "MISCSEGS":
                    return "ibMiscSegs";
                case "ONDMSEGS":
                    return "ibMktSegs";
                default:
                    return "";
            }
        }

        public string GetBackofficeTableName(AvancedParameterQueryTableRef tableRef, bool useHibservices)
        {
            switch (tableRef.TableName)
            {
                case "HOTEL":
                    return "hibHotel";
                case "CAR":
                case "AUTO":
                    return "hibcars";
                case "SVCFEE"://need to use hibservices
                    return useHibservices ? "hibservices" : "hibSvcFees";
                case "LEGS":
                case "AIRLEG":
                    return "hibLegs";
                case "CHGLOG":
                    return "changelog";
               //removed non-existing tables
                case "MISCSEGS":
                    return "hibMiscSegs";
                case "ONDMSEGS":
                    return "hibMktSegs";
                default:
                    return "";
            }
        }

        private IList<string> CleanTablesNeededIfThereAreCorrespondingViewsInPlay(string existingFromClause, IList<string> tablesThatNeedToBeAdded, bool isReservationReport)
        {
            var viewsAlreadyInFromClause = GetViewsAlreadyInFromClause(existingFromClause, isReservationReport);
            IList<string> updatedTablesThatNeedToBeAdded = new List<string>();

            foreach (var table in tablesThatNeedToBeAdded)
            {
                var matchesFound = viewsAlreadyInFromClause.Where(view => view.ToLower().Contains(table.ToLower())).ToList();
                if (!matchesFound.Any()) updatedTablesThatNeedToBeAdded.Add(table);
            }

            return updatedTablesThatNeedToBeAdded;
        }

        private List<string> GetTablesAlreadyInFromClause(string existingFromClause, bool isReservationReport)
        {
            return isReservationReport
                ? _clauseParser.GetTablesThatExistInClause(existingFromClause, _reservationTables).ToList()
                : _clauseParser.GetTablesThatExistInClause(existingFromClause, _backOfficeTables).ToList();
        }

        private List<string> GetViewsAlreadyInFromClause(string existingFromClause, bool isReservationReport)
        {
            return isReservationReport
                ? _clauseParser.GetTablesThatExistInClause(existingFromClause, DbViews.DV_VIEW_NAMES_RESERVATION).ToList()
                : _clauseParser.GetTablesThatExistInClause(existingFromClause, DbViews.DV_VIEW_NAMES_BACK_OFFICE).ToList();
        }

        private IList<string> GetTablesNeededForAdvancedParamsAndSaveMissingFields(List<string> advancedParams, bool isReservationReport)
        {
            var lookup = new EntityFieldsLookup();
            var tables = lookup.GetTablesNeeedForParameters(advancedParams, isReservationReport);
            FieldsNotFound = lookup.FieldsNotFound;
            return tables;
        }

        private IList<string> GetTablesNeededForAdvancedParams(List<string> advancedParams, bool isReservationReport)
        {
            var lookup = new EntityFieldsLookup();
            var tables = lookup.GetTablesNeeedForParameters(advancedParams, isReservationReport);
            FieldsNotFound = lookup.FieldsNotFound;
            return tables;
        }
        private IList<string> GetTablesThatDontExist(List<string> tablesAlreadyInFromClause, IList<string> tablesNeededForAdvancedParams)
        {
            return tablesNeededForAdvancedParams.Except(tablesAlreadyInFromClause, StringComparer.InvariantCultureIgnoreCase).ToList();
        }

        private string AddTablesToFromClause(IList<string> tablesThatNeedToBeAdded, string existingFromClause)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < tablesThatNeedToBeAdded.Count; i++)
            {
                sb.Append(i == tablesThatNeedToBeAdded.Count - 1 
                                ? $"{tablesThatNeedToBeAdded[i]} WITH (nolock)" 
                                : $"{tablesThatNeedToBeAdded[i]} WITH (nolock), ");
            }

            return existingFromClause.Trim().EndsWith(",", StringComparison.OrdinalIgnoreCase) 
                ? $"{existingFromClause} {sb} " 
                : $"{existingFromClause}, {sb}";
        }

        private string AddRecKeyReferencesToReckKeyWhereClause(IList<string> tablesThatNeedToBeAdded, string existingKeyWhereClause, string defaultPrefix)
        {
            //get an existing table prefix so we have something to join with
            var existingPrefix = _clauseParser.GetExistingReckeyPrefix(existingKeyWhereClause);

            if (string.IsNullOrEmpty(existingPrefix)) existingPrefix = defaultPrefix;


            /*
             * The following sql happend in prod under broadcast reports, but was not able to reproduce in keystone. Possible culprit could be the code below since it matches
             * this sql output:
             *  from ibTrips T1 WITH(nolock) , iblegs T2 WITH(nolock) where T1.reckey in (select DISTINCT t1.reckey from, ibtrips WITH (nolock) where  .reckey = ibtrips.reckey and  ) 
                from ibTrips T1 WITH (nolock)  where  T1.reckey in (select DISTINCT t1.reckey from , ibtrips WITH (nolock) where  .reckey = ibtrips.reckey and  and (ticket = '') ) order by T1.reckey
             */

            var sb = new StringBuilder();
            foreach (var table in tablesThatNeedToBeAdded)
            {
                //TODO: check if table doesn't have reckey field, then use recloc.
                //TODO item is fixed in AddReferencesToDefaultPrefixUsingEntityFieldsLookup
                if (table.EqualsIgnoreCase("hibSvcFees"))
                {
                    sb.Append($"{existingPrefix}.recloc = {table}.recloc and ");
                }
                else
                {
                    sb.Append($"{existingPrefix}.reckey = {table}.reckey and ");
                }
            }

            return existingKeyWhereClause.Trim().EndsWith("and", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(existingKeyWhereClause)
                       ? $"{existingKeyWhereClause} {sb}"
                       : $"{existingKeyWhereClause} and {sb}";
        }
        

        private string AddTablesToFromClauseUsingEntityFieldsLookup(IList<string> tablesThatNeedToBeAdded, string existingFromClause, bool isReservation)
        {
            //Make sure FromClause include table prefix too
            var finalFromClause = FixFromClausePrefix(existingFromClause, isReservation);

            var sb = new StringBuilder();
            for (var i = 0; i < tablesThatNeedToBeAdded.Count; i++)
            {
                sb.Append(i == tablesThatNeedToBeAdded.Count - 1
                                ? $"{tablesThatNeedToBeAdded[i]} {GetTablePrefixFromEntityFieldsLookup(tablesThatNeedToBeAdded[i], isReservation)}"
                                : $"{tablesThatNeedToBeAdded[i]} {GetTablePrefixFromEntityFieldsLookup(tablesThatNeedToBeAdded[i], isReservation)}, ");
            }

            return existingFromClause.Trim().EndsWith(",", StringComparison.OrdinalIgnoreCase)
                ? $"{finalFromClause} {sb} "
                : $"{finalFromClause}, {sb}";
        }

        private string FixFromClausePrefix(string existingFromClause, bool isReservation)
        {
            var finalFromClause = string.Empty;
            //make sure there is no missing table prefix in existingFromClause
            foreach (var table in existingFromClause.SplitAndRemoveEmptyStrings(','))
            {
                if (!string.IsNullOrWhiteSpace(finalFromClause)) finalFromClause += ", ";
                //then we want to split them on the space -> end up with { "ibtrips", "T1", "WITH", "(nolock)", ... }
                var splitClause = table.SplitAndRemoveEmptyStrings(' ').ToList();

                if (!splitClause.Any()) continue;

                var lookup = new EntityFieldsLookup();
                var prefix = lookup.GetTablePrefix(splitClause[0], isReservation);

                //should match if not missing prefix
                if (splitClause[1] != prefix)
                {
                    //add prefix
                    finalFromClause += $"{splitClause[0]} {prefix}";
                    for (int i = 1; i < splitClause.Count; i++)
                    {
                        finalFromClause += $" {splitClause[i]}";
                    }
                }
                else finalFromClause += table;
            }
            return finalFromClause;
        }

        public string GetTablePrefixFromEntityFieldsLookup(string tableName, bool isReservation)
        {
            var lookup = new EntityFieldsLookup();
            return lookup.GetTablePrefix(tableName, isReservation);
        }

        private string AddReferencesToDefaultPrefixUsingEntityFieldsLookup(IList<string> tablesThatNeedToBeAdded, string existingKeyWhereClause, string defaultPrefix, bool isReservation)
        {
            //get an existing table prefix so we have something to join with
            var existingPrefix = _clauseParser.GetExistingReckeyPrefix(existingKeyWhereClause);

            if (string.IsNullOrEmpty(existingPrefix)) existingPrefix = defaultPrefix;

            var sb = new StringBuilder();
            foreach (var table in tablesThatNeedToBeAdded)
            {
                var lookup = new EntityFieldsLookup();
                var tablePrefix = lookup.GetTablePrefix(table.ToLower(), isReservation);

                if (lookup.HasRecKeyField(table))
                {                    
                    sb.Append($"{existingPrefix}.reckey = {tablePrefix}.reckey and ");
                }
                else
                {
                    sb.Append($"{existingPrefix}.recloc = {tablePrefix}.recloc and ");
                }
            }

            return existingKeyWhereClause.Trim().EndsWith("and", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(existingKeyWhereClause)
                       ? $"{existingKeyWhereClause} {sb}"
                       : $"{existingKeyWhereClause} and {sb}";
        }
        
    }
}
