using Domain;
using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.MissedHotelOpportunities
{
    public class MissedHotelSqlCreator
    {
        public SqlScript CreateLegSql(string existingWhereClause, bool includeTripsWithHotels, bool isReservationReport, int udidNumber)
        {
            var sql = new SqlScript();

            if (udidNumber > 0)
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2, ibudids T3" : "hibtrips T1, hiblegs T2, hibudids T3";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and ";
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2" : "hibtrips T1, hiblegs T2";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and ";
            }

            sql.WhereClause = sql.KeyWhereClause + existingWhereClause;

            sql.FieldList = isReservationReport
                ? "T1.reckey, 'I' as TranType, Invoice, InvDate, acct, break1, break2, break3, passlast, passfrst, TripStart, TripEnd, agentID, sourceAbbr, T1.recloc "
                : "T1.reckey, TranType, Invoice, InvDate, acct, break1, break2, break3, passlast, passfrst, TripStart, TripEnd, agentID, sourceAbbr, T1.recloc ";

            sql.OrderBy = "";

            return sql;
        }
        
        public SqlScript AddInExcludeHotelClause(SqlScript sql, string originalWhereClause, bool isReservationReport)
        {
            sql.WhereClause += GetExcludeHotelsSql(isReservationReport) + originalWhereClause + ")";
            return sql;
        }

        private string GetExcludeHotelsSql(bool isReservationReport)
        {
            var tripsTable = isReservationReport ? "ibtrips" : "hibtrips";
            var hotelTable = isReservationReport ? "ibhotel" : "hibhotel";

            //it may have multiple invoices for the same trip, use recloc to check hotel uniqueness
            if (Features.MissedHotelUseReclocCheck.IsEnabled())
            {
                return $" and T1.recloc not in (select T91.recloc from {tripsTable} T91 WITH (nolock), {hotelTable} T92 WITH (nolock) where T91.reckey = T92.reckey and ";
            }
            else
            {
                return $" and T1.reckey not in (select T91.reckey from {tripsTable} T91 WITH (nolock), {hotelTable} T92 WITH (nolock) where T91.reckey = T92.reckey and ";
            }
        }


        public SqlScript CreateCarSql(string existingWhereClause,  bool isReservationReport, int udidNumber, string existingFieldList)
        {
            var sql = new SqlScript();

            if (udidNumber > 0)
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, ibCar T2, ibudids T3" : "hibtrips T1, hibCars T2, hibudids T3";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and valcarr = 'ZZ' and ";
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, ibCar T2" : "hibtrips T1, hibCars T2";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and valcarr = 'ZZ' and ";
            }

            sql.WhereClause = sql.KeyWhereClause + existingWhereClause;

            sql.FieldList = existingFieldList;
            sql.OrderBy = "";
            
            return sql;
        }

        public SqlScript CreateHotelSql(string existingWhereClause, bool isReservationReport, int udidNumber, string existingFieldList)
        {
            var sql = new SqlScript();

            if (udidNumber > 0)
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, ibHotel T2, ibudids T3" : "hibtrips T1, hibHotel T2, hibudids T3";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and ";
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, ibHotel T2" : "hibtrips T1, hibHotel T2";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and ";
            }

            sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            sql.FieldList = existingFieldList;
            sql.OrderBy = "";

            return sql;
        }
    }
}
