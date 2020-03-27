using System;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using Domain.Models;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Shared.AdvancedClause;

namespace iBank.Services.Implementation.ReportPrograms.Invoice
{
    public class InvoiceSqlProcessor
    {
        private readonly WhereClauseWithAdvanceParamsHandler _whereClauseWithAdvanceParamsHandler = new WhereClauseWithAdvanceParamsHandler();
        private Object _thisLock = new Object();
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public SqlScript CreateSqlScript(BuildWhere buildWhere, string whereClause, bool includeAllLegs)
        {
            const string fieldList = "T1.reckey, T1.recloc, T1.pseudocity, T1.agentid, T1.acct, T1.invoice, T1.invdate, T1.bookdate, T1.trantype, T1.domintl, T1.ticket, T1.passlast, T1.passfrst, T1.break1, T1.break2, "
                                     + " T1.break3, T1.break4, T1.airchg, T1.credcard, T1.cardnum, T1.depdate, T1.valcarr, T1.exchange, T1.origticket, T1.tax1, T1.tax2, T1.tax3, T1.tax4, T1.valcarMode, T1.svcFee ";

            var fromClause = "hibtrips T1";

            var keyWhereClause = string.Empty;

            if (buildWhere.HasRoutingCriteria)
            {
                fromClause += ", hiblegs T2";
                keyWhereClause = "T1.reckey = T2.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and ";
            }

            var sql = new SqlScript
            {
                FieldList = fieldList,
                FromClause = fromClause,
                KeyWhereClause = keyWhereClause,
                WhereClause = keyWhereClause + whereClause,
                OrderBy = string.Empty
            };

            lock (_thisLock)
            {
                sql.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildWhere, includeAllLegs);
            }

            return sql;
        }

        public SqlScript CreateKeysAndLocatorsSqlScript(BuildWhere buildWhere, string whereX, bool includeAllLegs)
        {
            var fieldList = "T1.recloc, T1.reckey";
            var orderBy = string.Empty;

            var keyWhereClause = "";

            var fromClause = "hibtrips T1";
            var whereClause = whereX;
            if (buildWhere.HasRoutingCriteria)
            {
                fieldList += ", T2.origin, T2.destinat";
                fromClause = "hibtrips T1, hiblegs T2";
                keyWhereClause = "T1.reckey = T2.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and ";
            }
            whereClause = keyWhereClause + whereClause;

            var sql = new SqlScript
            {
                FromClause = fromClause,
                KeyWhereClause = keyWhereClause,
                WhereClause = whereClause,
                FieldList = fieldList,
                OrderBy = ""
            };

            lock (_thisLock)
            {
                sql.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildWhere, includeAllLegs);
            }

            return sql;
        }
    }
}
