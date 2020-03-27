using System;

using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.Arrival
{
    public class ArrivalRawDataSqlCreator
    {
        public string GetSpecialWhere(DateTime endDate, DateTime beginDate)
        {
            return " and TripStart <= '" + endDate.ToShortDateString() + " 11:59:59 PM' and TripEnd >= '" + beginDate.ToShortDateString() + "' ";
        }

        public SqlScript CreateScript(int udid, bool isReservationReport, string existingWhereClause, bool useConnectingLegs)
        {
            var sql = new SqlScript { WhereClause = existingWhereClause };
            if (udid != 0)
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2, ibudids T3" : "hibtrips T1, hiblegs T2, hibudids T3";

                sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and ";
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2" : "hibtrips T1, hiblegs T2";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and ";
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }
            
            sql.OrderBy = "";
            
            sql.FieldList = @"T1.recloc, T1.reckey, invoice, ticket, pseudocity, acct, break1, break2, break3, passlast, passfrst, domintl, airchg, offrdchg, 000 as seg_cntr";
            
            if (isReservationReport)
            {
                sql.FieldList += ", convert(int,1) as plusmin";
            }
            else
            {
                sql.FieldList += ", convert(int,plusmin) as plusmin";
            }

            if (useConnectingLegs)
            {
                sql.FieldList += ", T2.connect";
            }

            return sql;
        }
    }
}
