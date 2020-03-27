using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.CarrierConcentration
{
    public class CarrierConcentrationSqlCreator
    {
        public SqlScript Create(string existingWhereClause, int udidNumber, bool isReservationReport)
        {
            var sql = new SqlScript();

            if (udidNumber > 0)
            {
                sql.FromClause = isReservationReport
                    ? "ibtrips T1, iblegs T2, ibudids T3"
                    : "hibtrips T1, hiblegs T2, hibudids T3";

                sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and valcarr not in ('ZZ','$$')  and ";
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }
            else
            {
                sql.FromClause = isReservationReport
                    ? "ibtrips T1, iblegs T2"
                    : "hibtrips T1, hiblegs T2";

                sql.KeyWhereClause = "T1.reckey = T2.reckey and valcarr not in ('ZZ','$$')  and ";
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }

            sql.FieldList = "T1.reckey, T1.recloc, valcarr, airchg, basefare, faretax, 00 as seg_cntr, acct, space(13) as flt_mkt," +
                            " space(13) as flt_mkt2, space(6) as orgdestemp, 00000000 as RecordNo";

            sql.FieldList += isReservationReport ? ",  convert(int,1) as plusmin" : ", convert(int,plusmin) as plusmin";

            sql.OrderBy = "";

            return sql;
        }
    }
}