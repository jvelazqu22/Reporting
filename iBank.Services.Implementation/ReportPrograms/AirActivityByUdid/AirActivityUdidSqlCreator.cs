using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.AirActivityByUdid
{
    public class AirActivityUdidSqlCreator
    {
        public SqlScript Create(bool isReservationReport, string existingWhereClause)
        {
            var sql = new SqlScript();

            sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2, ibudids T3" : "hibtrips T1, hiblegs T2, hibudids T3";
            
            sql.KeyWhereClause =
                "T1.reckey = T2.reckey and T1.reckey = T3.reckey and T1.agency = T3.agency and T2.agency = T3.agency and airline != 'ZZ' and valcarr not in ('ZZ','$$') and ";

            sql.WhereClause = sql.KeyWhereClause + existingWhereClause;

            sql.FieldList = "T1.reckey,convert(int,seqno) as seqno , udidtext, passlast, passfrst, ticket, airchg, origin, destinat, connect, depdate, rdepdate, airline, class as classcode, fltno, mode, acct, break1, break2, break3";
            
            sql.FieldList += isReservationReport ? @", 'I' as trantype, airline as origCarr" : @", trantype, origCarr";

            sql.OrderBy = "";

            return sql;
        }
    }
}
