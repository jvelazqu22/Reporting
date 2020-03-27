using Domain.Models;
using System;

namespace iBank.Services.Implementation.ReportPrograms.SameCity
{
    public class SameCitySqlCreator
    {
        public SqlScript Create(string existingWhereClause, int udidNumber, bool isReservationReport, string whereTripClause)
        {
            var sql = new SqlScript();

            if (udidNumber > 0)
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2, ibudids T3" : "hibtrips T1, hiblegs T2, hibudids T3";

                sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and " +
                              "airline not in ('ZZ','$$') and valcarr not in ('ZZ','$$') " +
                              "and airline is not null and ";

                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2" : "hibtrips T1, hiblegs T2";

                sql.KeyWhereClause = "T1.reckey = T2.reckey and airline not in ('ZZ','$$') and " +
                              "valcarr not in ('ZZ','$$') and airline is not null and ";

                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }

            if (!string.IsNullOrEmpty(whereTripClause)) sql.WhereClause = sql.WhereClause + whereTripClause;

            sql.FieldList = isReservationReport
                                ? "T1.recloc, T1.reckey, invoice, ticket, passlast, passfrst, convert(int,1) as plusmin"
                                : "T1.recloc, T1.reckey, invoice, ticket, passlast, passfrst, convert(int,plusmin) as plusmin";

            sql.OrderBy = "";

            return sql;
        }

        public string CreateWhereTripClause(DateTime beginDate, DateTime endDate)
        {
            return string.Format(" and rarrdate <= '{0} 11:59:59 PM' and rarrdate >= '{1}' ", endDate.ToShortDateString(), beginDate.ToShortDateString());
        }
    }
}