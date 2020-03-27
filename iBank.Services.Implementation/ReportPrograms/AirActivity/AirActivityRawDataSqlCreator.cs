using Domain.Models;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.AirActivity
{
    public class AirActivityRawDataSqlCreator
    {
        private readonly WhereClauseWithAdvanceParamsHandler _whereClauseWithAdvanceParamsHandler = new WhereClauseWithAdvanceParamsHandler();

        public SqlScript CreateScript(string existingWhereClause, int udid, bool isReservationReport, string udidWhere, BuildWhere buildWhere)
        {
            var sql = new SqlScript { WhereClause = existingWhereClause };
            
            if (udid != 0)
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2, ibudids T3" : "hibtrips T1, hiblegs T2, hibudids T3";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and ";
                sql.WhereClause = sql.KeyWhereClause + sql.WhereClause;
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2" : "hibtrips T1, hiblegs T2";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and ";
                sql.WhereClause = sql.KeyWhereClause + sql.WhereClause;
            }

            sql.FieldList = @"T1.recloc, T1.reckey, invoice, ticket, acct, break1, break2, break3, passlast, passfrst, airchg, " +
                            "offrdchg, 00000000.0000 as svcfee, 'I' as SFTranType, depdate, invdate, bookdate, cardnum ";

            sql.FieldList = isReservationReport
                ? sql.FieldList += @", 'I' as trantype, exchange, pseudocity, origticket, convert(int,1) as plusmin, space(13) as flt_mkt, space(13) as flt_mkt2 "
                : sql.FieldList += @", ' ' as pseudocity, trantype, exchange, origticket, convert(int,plusmin) as plusmin, space(13) as flt_mkt, space(13) as flt_mkt2 ";

            sql.OrderBy = string.Empty;
            //sql.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildWhere, false);
            return sql;
        }
    }
}
