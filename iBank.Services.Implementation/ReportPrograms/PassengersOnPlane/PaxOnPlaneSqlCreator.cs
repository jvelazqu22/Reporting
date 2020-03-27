using Domain.Models;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.PassengersOnPlane
{
    public class PaxOnPlaneSqlCreator
    {
        public SqlScript Create(string existingWhereClause, int udid, bool isReservationReport)
        {
            var sql = new SqlScript();

            if (udid > 0)
            {
                sql.FromClause = isReservationReport 
                    ? "ibtrips T1, iblegs T2, ibudids T3" 
                    : "hibtrips T1, hiblegs T2, hibudids T3";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and ";
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2" : "hibtrips T1, hiblegs T2";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and ";
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }

            sql.FieldList = @"T1.reckey, T1.recloc, acct, break1, break2, break3, depdate, bookdate, passlast, passfrst, pseudocity, domintl, ticket, agentid ";

            if (isReservationReport)
            {
                sql.FieldList += @", 'I' as trantype ";
            }
            else
            {
                sql.FieldList += @", trantype ";
            }

            sql.OrderBy = "";

            return sql;
        }

        public SqlScript GetGantSql(string existingWhereClause, ReportGlobals globals)
        {
            var calc = new PaxOnPlaneCalculations();
            var sql = new SqlScript();

            sql.FromClause = calc.IsDateRange1(globals) ? "ibtrips T1, ibudids T3" : "hibtrips T1, hibudids T3";

            sql.KeyWhereClause = "T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and udidno = 71 and ";
            sql.WhereClause = sql.KeyWhereClause + existingWhereClause;

            sql.FieldList = "T1.reckey, udidtext";

            sql.OrderBy = "";

            return sql;
        }
    }
}
