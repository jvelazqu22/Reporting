using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.ServiceFeeDetailByTrip
{
    public class SvcFeeDetailTripSqlCreator
    {
        public SqlScript CreateRawDataSql(string existingWhereClause, int udid, bool includeVoids)
        {
            var sql = new SqlScript();

            existingWhereClause = existingWhereClause.Replace("T1.SVCFEE", "T6A.SVCFEE");

            if (udid != 0)
            {
                sql.FromClause = "hibtrips T1, hibServices T6A, hibudids T3";
                sql.KeyWhereClause = "T1.reckey = T6A.reckey and T1.agency = T6A.agency " +
                                     "and T1.reckey = T3.reckey and T1.agency = T3.agency " +
                                     "and origValCar != 'SVCFEEONLY' and left(origValCar,3) != 'ZZ:' and T6A.svcCode = 'TSF' and ";
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }
            else
            {
                sql.FromClause = "hibtrips T1, hibServices T6A";
                sql.KeyWhereClause = "T1.reckey = T6A.reckey and T1.agency = T6A.agency and " +
                                 "origValCar != 'SVCFEEONLY' and left(origValCar,3) != 'ZZ:' and " +
                                 "T6A.svcCode = 'TSF' and ";
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }

            if (!includeVoids)
            {
                sql.KeyWhereClause += "T6A.sfTranType != 'V' and ";
            }

            sql.WhereClause =
                sql.WhereClause.Replace("T1.SVCFEE", "SVCAMT")
                    .Replace("T6.SVCFEE", "SVCAMT")
                    .Replace("DESCRIPT", "SVCDESC")
                    .Replace("T6.IATANBR", "IATANBR")
                    .Replace("TAX", "T6A.TAX");

            sql.FieldList = "T1.reckey, T6A.recordno as SFRecordNo, recloc, " +
                            "invoice, ticket, acct, break1, break2, break3, passlast, " +
                            "passfrst, airchg, depdate, invdate, trandate, T6A.svcAmt as svcfee, svcDesc as descript";

            sql.OrderBy = "";

            return sql;
        }

        public SqlScript CreateRouteItinerariesSql(string existingWhereClause, int udid)
        {
            var sql = new SqlScript();

            if (udid != 0)
            {
                sql.FromClause = "hibtrips T1, hiblegs T2, hibudids T3";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and " +
                                        "airline not in ('ZZ','$$') and valcarr not in ('ZZ','$$') and ";
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }
            else
            {
                sql.FromClause = "hibtrips T1, hiblegs T2";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and airline not in ('ZZ','$$') and " +
                                 "valcarr not in ('ZZ','$$') and ";
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }

            sql.FieldList = "T1.reckey, origin, destinat, connect, mode, origOrigin, origDest";
            sql.OrderBy = "";

            return sql;
        }
    }
}
