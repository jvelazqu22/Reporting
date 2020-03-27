using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.AirFareSavings
{
    public class AirFareSavingsSqlCreator
    {
        public SqlScript CreateScript(string existingWhereClause, int udid, bool isReservationReport, string udidWhere)
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

            sql.FieldList = @"T1.reckey, convert(int,seqno) seqno, acct, break1, break2, break3, reascode, invdate, ticket, passlast, passfrst, origin, destinat, " +
                        "rdepdate, airline, mode, connect, class, airchg, basefare, offrdchg, stndchg, savingcode, T1.SourceAbbr";

            sql.FieldList += isReservationReport ? ", convert(int, 1) as plusmin " : ", convert(int,plusmin) plusmin";

            sql.OrderBy = string.Empty;

            return sql;
        }
    }
}

