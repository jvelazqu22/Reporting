using Domain.Models.ReportPrograms.CCReconReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities.ClientData;
using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.CCRecon
{
    public class CcReconDataRetriever
    {
        public IList<BreakRecord> GetBreakRecords(string whereClauseWithoutCreditCardCriteria, ReportGlobals globals, BuildWhere buildWhere)
        {
            var breaksSql = new CcReconSqlCreator().GetBreaksSql(whereClauseWithoutCreditCardCriteria);
            return ClientDataRetrieval.GetOpenQueryData<BreakRecord>(breaksSql, globals, buildWhere.Parameters, false);
        }

        public IList<T> GetTripCcRecs<T>(string whereClauseWithoutCreditCardCriteria, ReportGlobals globals, BuildWhere buildWhere, bool includeLegs)
        {
            var tableName = string.Empty;
            var creditCardFieldFound = false;
            foreach (var item in buildWhere.AdvancedParameterQueryTableRefList)
            {
                if (item.AdvancedQuerySnip.ToUpper().Contains("SFCARDNUM"))
                {
                    item.AdvancedQuerySnip = item.AdvancedQuerySnip.ToUpper().Replace("SFCARDNUM", "CARDNUM");
                    tableName = item.TableName;
                    item.TableName = "";
                    creditCardFieldFound = true;
                }
            }

            if (!creditCardFieldFound) return new List<T>();

            var tripCcSql = includeLegs
                ? new CcReconSqlCreator().GetTripCcAndLegsSql(buildWhere, whereClauseWithoutCreditCardCriteria)
                : new CcReconSqlCreator().GetTripCcSql(buildWhere, whereClauseWithoutCreditCardCriteria);
            var results = ClientDataRetrieval.GetOpenQueryData<T>(tripCcSql, globals, buildWhere.Parameters, false);

            foreach (var item in buildWhere.AdvancedParameterQueryTableRefList)
            {
                if (item.AdvancedQuerySnip.ToUpper().Contains("CARDNUM"))
                {
                    item.AdvancedQuerySnip = item.AdvancedQuerySnip.ToUpper().Replace("CARDNUM", "SFCARDNUM");
                    item.TableName = tableName;
                }
            }

            return results;
        }

        public IList<Udid> GetUdids(List<int> udidNumber, string whereClause, ReportGlobals globals, BuildWhere buildWhere, IClientQueryable clientQueryDb)
        {
            var udidSql = new CcReconSqlCreator().GetUdidSql(udidNumber, whereClause);
            var processedUdidSql = SqlProcessor.ProcessSql(udidSql.FieldList, false, udidSql.FromClause, udidSql.WhereClause, udidSql.OrderBy, globals);
            processedUdidSql += " group by T1.reckey, UdidNo, UdidText";
            return ClientDataRetrieval.GetOpenQueryData<Udid>(processedUdidSql, globals, buildWhere.Parameters);
        }
    }
}
