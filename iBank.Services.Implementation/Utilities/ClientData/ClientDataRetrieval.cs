using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Interfaces;
using Domain.Models;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Logging;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using System.Collections.Generic;
using System.Linq;
using iBank.Repository.SQL.Interfaces;
using System;
using Domain;

namespace iBank.Services.Implementation.Utilities.ClientData
{
    public class ClientDataRetrieval
    {
        public static IList<T> GetRawData<T>(SqlScript sql, bool isReservationReport, BuildWhere where, ReportGlobals globals, bool addFieldsFromLegsTable,
            bool includeAllLegs = true, bool checkForDuplicatesAndRemoveThem = false, bool handleAdvanceParamsAtReportLevelOnly = false) where T : IRecKey
        {
            //Do not check shared across data here to allowed pulling the same data multiple times
            //Sharing data logic it's done in GetOpenQueryData<T>(string sql, ReportGlobals globals, object[] parameters) 
            var retriever = new DataRetriever(new iBankClientQueryable(globals.AgencyInformation.ServerName, globals.AgencyInformation.DatabaseName));

            return retriever.GetData<T>(sql, where, addFieldsFromLegsTable, false, isReservationReport, includeAllLegs, checkForDuplicatesAndRemoveThem, handleAdvanceParamsAtReportLevelOnly);
        }

        public static IList<T> GetOpenQueryData<T>(SqlScript sql, ReportGlobals globals, object[] parameters, bool addFieldsFromLegsTable)
        {
            var fullSql = SqlProcessor.ProcessSql(sql.FieldList, addFieldsFromLegsTable, sql.FromClause, sql.WhereClause, sql.OrderBy, globals);
            if (!string.IsNullOrEmpty(sql.GroupBy)) fullSql += $" {sql.GroupBy}";
            return GetOpenQueryData<T>(fullSql, globals, parameters);
        }

        public static IList<T> GetOpenQueryData<T>(string sql, ReportGlobals globals, object[] parameters)
        {
            if (globals.ClientType == ClientType.Sharer)
            {
                var corpAccountDataSources = globals.CorpAccountDataSources;
                var rawData = new List<T>();
                
                var databases = corpAccountDataSources.GroupBy(x => x.DataSource.DatabaseName).Distinct().ToList();
                foreach (var db in databases)
                {
                    var serverAddress = db.First(x => x.DataSource.DatabaseName == db.Key).DataSource.ServerAddress;

                    var clientStore = new ClientDataStore(serverAddress, db.Key);
                    //according to Defect 00178957 - Offline Reports never received from shared site LCCSHARE, it should not include account etc in the query
                    //var sqlAddSource = InjectCorpAccountQuery(sql, corpAccountDataSource.Acct.Trim(), corpAccountDataSource.SourceAbbr, corpAccountDataSource.Agency, globals.Agency);
                    var sqlAddSource = InjectCorpAccountQuery(sql, corpAccountDataSources.Where(x => x.DataSource.DatabaseName == db.Key).Select(x => x.DataSource.Agency).ToList(), globals.Agency);

                    LogSql(sqlAddSource, parameters, globals, clientStore.ClientQueryDb);
                    var data = GetDataFromDbAndLogSqlIfException<T>(clientStore, globals, sqlAddSource, parameters);
                    rawData.AddRange(data);
                }
                return rawData;
            }
            else
            {
                var clientStore = new ClientDataStore(globals.AgencyInformation.ServerName, globals.AgencyInformation.DatabaseName);
                LogSql(sql, parameters, globals, clientStore.ClientQueryDb);
                return GetDataFromDbAndLogSqlIfException<T>(clientStore, globals, sql, parameters);
            }
        }

        private static IList<T> GetDataFromDbAndLogSqlIfException<T>(IClientDataStore clientStore, ReportGlobals globals, string sql, object[] parameters)
        {
            try
            {
                return new OpenClientQuery<T>(clientStore.ClientQueryDb, sql, parameters).ExecuteQuery();
            }
            catch (Exception ex)
            {
                LogSql(sql, parameters, globals, clientStore.ClientQueryDb, forceLogDueToException: true);
                throw ex;
            }
        }

        public static string InjectCorpAccountQuery(string sql, List<string> agencies, string corpacct)
        {
            var resultSql = sql;
            var pos = sql.ToLower().IndexOf("where ");
            var where = "";
            if (pos > -1)
            {
                resultSql = sql.Substring(0, pos);
                where = sql.Substring(pos + 5);
            }
            resultSql += $"where ";

            //if not trip related data, eg ibuser
            if (IsT1Trip(sql))
            {
                resultSql += $"T1.CorpAcct = '{corpacct}' and T1.agency in ('{string.Join("','", agencies)}') and ";
            }
            else if (sql.IndexOf(" T1") > 0 )
            {
                resultSql += $"T1.Agency = '{corpacct}' and ";
            }else
            {
                resultSql += $"Agency = '{corpacct}' and ";
            }
            
            resultSql += $"{where.TrimStart().TrimEnd()}";
            return resultSql;
        }

        private static bool IsT1Trip(string sql)
        {
            var pos = sql.IndexOf("T1", sql.IndexOf("from", 0, StringComparison.InvariantCultureIgnoreCase));
            if (pos < 7) return false;

            var table = sql.Substring(pos - 8, 7);
            if (table.Equals("ibTrips", StringComparison.InvariantCultureIgnoreCase)) return true;

            return false;
        }
        
        private static void LogSql(string sql, object[] parameters, ReportGlobals globals, IClientQueryable clientDb, bool forceLogDueToException = false)
        {
            var udrKey = globals.GetParmValue(WhereCriteria.UDRKEY).TryIntParse(-1);
            var sqlLog = new SqlLogger(globals.ReportLogKey, globals.UserNumber, globals.User.UserId, globals.ProcessKey, udrKey, clientDb, globals.Agency, forceLogDueToException);
            sqlLog.Log(sql, parameters, new iBankMastersCommandDb());
        }
        
        public static IList<T> GetUdidFilteredOpenQueryData<T>(string sql, ReportGlobals globals, object[] parameters, bool isReservationReport) where T : IRecKey
        {
            var data = GetOpenQueryData<T>(sql, globals, parameters);

            if (!globals.MultiUdidParameters.Parameters.Any()) return data;

            var filter = new UdidFilter();
            return filter.GetUdidFilteredData(data, globals, isReservationReport, new TripUdidRetriever());
        }
    }
}
