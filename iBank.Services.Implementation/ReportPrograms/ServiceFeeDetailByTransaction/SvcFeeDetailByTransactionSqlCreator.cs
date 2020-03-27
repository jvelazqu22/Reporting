using Domain.Models;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.ServiceFeeDetailByTransaction
{
    public class SvcFeeDetailByTransactionSqlCreator
    {
        private readonly WhereClauseWithAdvanceParamsHandler _whereClauseWithAdvanceParamsHandler = new WhereClauseWithAdvanceParamsHandler();
        public SqlScript CreateRawDataSql(BuildWhere BuildWhere)
        {
            var sql = new SqlScript();

            var whereClause = BuildWhere.WhereClauseFull;
            //var whereClause = string.IsNullOrWhiteSpace(BuildWhere.WhereClauseAdvanced)
            //               ? BuildWhere.WhereClauseFull
            //               : BuildWhere.WhereClauseFull + " AND " + BuildWhere.WhereClauseAdvanced;

            sql.FromClause = "hibtrips T1, hibServices T6A";
            sql.FieldList = "T1.reckey, acct, passlast, passfrst, depdate, trandate, ticket, invoice, invdate, recloc, T6A.svcDesc, T6A.svcAmt ";
            sql.KeyWhereClause = "T1.reckey = T6A.reckey and T1.agency = T6A.agency and T6A.svcCode = 'TSF' and ";
            whereClause = whereClause.Replace("FTRANTYPE", "SFTRANTYPE")
                            .Replace("SSFTRANTYPE", "SFTRANTYPE")
                            .Replace("T1.trantype", "T6A.sfTrantype")
                            .Replace("SVCFEE", "SVCAMT")
                            .Replace("DESCRIPT", "SVCDESC")
                            .Replace("T6.IATANBR", "IATANBR")
                            .Replace("TAX", "T6A.TAX");

            sql.WhereClause = sql.KeyWhereClause + whereClause;

            sql.OrderBy = "";

            return sql;
        }

        public SqlScript CreateProcessDataSql(BuildWhere BuildWhere, string serviceFeeWhereClause)
        {
            var sql = new SqlScript();

            var whereClause = BuildWhere.WhereClauseFull;

            sql.FromClause = "hibTrips T1, hibLegs T2";
            sql.FieldList = "T1.reckey, acct, passlast, passfrst, depdate, ticket, invoice, invdate, recloc, origin, destinat, connect, seqno, mode, origOrigin, origDest ";
            sql.KeyWhereClause = "T1.reckey = T2.reckey and airline not in ('ZZ','$$') and valcarr not in ('ZZ','$$') and ";
            whereClause = whereClause.Replace("trandate", "invdate");

            sql.WhereClause = sql.KeyWhereClause + whereClause;

            sql.OrderBy = "order by T1.reckey, seqno";

            sql.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, BuildWhere, false, serviceFeeWhereClause);

            return sql;
        }


    }
}
