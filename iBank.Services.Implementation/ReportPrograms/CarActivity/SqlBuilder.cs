using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.CarActivity
{
    public static class SqlBuilder
    {
        public static SqlScript GetSql(int udidNumber, bool isReservationReport, string WhereClauseFull)
        {
            var sqlScript = new SqlScript();

            if (udidNumber > 0)
            {
                sqlScript.FromClause = isReservationReport
                    ? "ibtrips T1, ibcar T4, ibudids T3"
                    : "hibtrips T1, hibcars T4, hibudids T3";
                sqlScript.KeyWhereClause = "T1.reckey = T4.reckey and T1.reckey = T3.reckey and ";
                sqlScript.WhereClause = sqlScript.KeyWhereClause + WhereClauseFull;
            }
            else
            {
                sqlScript.FromClause = isReservationReport
                    ? "ibtrips T1, ibcar T4"
                    : "hibtrips T1, hibcars T4";
                sqlScript.KeyWhereClause = "T1.reckey = T4.reckey and ";
                sqlScript.WhereClause = sqlScript.KeyWhereClause + WhereClauseFull;
            }

            var plusMin = isReservationReport ? "convert(int,1) as cplusmin" : "convert(int,cplusmin) as cplusmin";

            sqlScript.FieldList =
                "T1.reckey, acct, break1, break2, break3, T1.recloc, passlast, passfrst, autocity, autostat, rentdate, company, cartype, days, abookrat, milecost, ratetype, " +
                plusMin;

            return sqlScript;
        }
    }
}
