using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.ExceptHotel
{
    public class ExceptHotelSqlCreator
    {
        public SqlScript Create(string existingWhereClause, int udidNumber, bool isReservationReport, string reasonExclude)
        {
            var sql = new SqlScript();

            if (udidNumber > 0)
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, ibhotel T5, ibudids T3"
                                       : "hibtrips T1, hibhotel T5, hibudids T3";

                sql.KeyWhereClause = string.Format(@"T1.reckey = T5.reckey and T1.reckey = T3.reckey and  
                                            reascodh is not null and reascodh != '  ' and 
                                            reascodh not in ({0}) AND ", reasonExclude);
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;

            }
            else
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, ibhotel T5"
                                       : "hibtrips T1, hibhotel T5";

                sql.KeyWhereClause = string.Format(@"T1.reckey = T5.reckey and reascodh is not null and
                                            reascodh != '  ' and reascodh not in ({0}) AND ", reasonExclude);

                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }

            sql.FieldList = @"T1.recloc, T1.reckey, acct, break1, break2, break3, passlast, passfrst, reascodh, datein, hotelnam, hotcity, hotstate, convert(int,nights) nights, convert(int,rooms) rooms, roomtype, bookrate, hexcprat,confirmno";

            if (isReservationReport)
            {
                sql.FieldList += @", convert(int,1) as hplusmin ";
            }
            else
            {
                sql.FieldList += @", convert(int,hplusmin) hplusmin ";
            }

            sql.OrderBy = "";

            return sql;
        }
    }
}