using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.ExceptCar
{
    public class ExceptCarSqlCreator
    {
        public SqlScript Create(string existingWhereClause, int udidNumber, bool isReservationReport, string reasonExclude)
        {
            var sql = new SqlScript();

            if (udidNumber > 0)
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, ibcar T4, ibudids T3"
                                       : "hibtrips T1, hibcars T4, hibudids T3";

                sql.KeyWhereClause = string.Format(@"T1.reckey = T4.reckey and T1.reckey = T3.reckey and " +
                                            "reascoda is not null and reascoda != '  ' and " +
                                            "reascoda not in ({0}) AND ",
                                            reasonExclude);
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, ibcar T4" : "hibtrips T1, hibcars T4";

                sql.KeyWhereClause = string.Format(@"T1.reckey = T4.reckey and reascoda is not null and " +
                                            "reascoda != '  ' and reascoda not in ({0}) AND ",
                                            reasonExclude);

                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }

            sql.FieldList = @"T1.recloc, T1.reckey, acct, break1, break2, break3, passlast, passfrst, reascoda, rentdate, company, autocity, autostat, days, cartype, abookrat, aexcprat ";

            if (isReservationReport)
            {
                sql.FieldList += @", convert(int,1) as cplusmin ";
            }
            else
            {
                sql.FieldList += @", convert(int,cplusmin) as cplusmin ";
            }

            sql.OrderBy = "";

            return sql;
        }
    }
}
