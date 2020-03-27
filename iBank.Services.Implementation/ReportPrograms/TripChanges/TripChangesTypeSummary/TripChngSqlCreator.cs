using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesTypeSummary
{
    public class TripChngSqlCreator
    {
        public SqlScript CreateScript(bool GoodUdid, int UdidNumber, string WhereClauseOriginal, string Where2, string whereChanges, string ChangeStampWhere)
        {
            var sql = new SqlScript();

            if (GoodUdid && UdidNumber != 0)
            {
                sql.FromClause = "ibtrips T1, ibudids T3, changelog TCL";
                sql.KeyWhereClause = "T1.reckey = T3.reckey and T1.reckey = TCL.reckey and ";
                sql.WhereClause = sql.KeyWhereClause + WhereClauseOriginal + Where2 + whereChanges + ChangeStampWhere;
            }
            else
            {
                sql.FromClause = "ibtrips T1, changelog TCL";
                sql.KeyWhereClause = "T1.reckey = TCL.reckey and ";
                sql.WhereClause = sql.KeyWhereClause + WhereClauseOriginal + Where2 + whereChanges + ChangeStampWhere;
            }

            sql.FieldList = "T1.reckey, TCL.ChangeCode ";

            return sql;
        }

        public SqlScript CreateScriptForIncludeCancelTripsAndUpdateWhere2(bool GoodUdid, int UdidNumber, string WhereClauseOriginal, ref string Where2, string CancelStampWhere)
        {
            var sql = new SqlScript();

            if (!string.IsNullOrEmpty(Where2))
                Where2 = Where2.Replace("TCL.changstamp", "T1.changstamp");

            if (GoodUdid && UdidNumber != 0)
            {
                sql.FromClause = "ibCancTrips T1, ibCancUdids T3";
                sql.KeyWhereClause = "T1.reckey = T3.reckey and ";
                sql.WhereClause = sql.KeyWhereClause;
            }
            else
            {
                sql.FromClause = "ibCancTrips T1";
            }

            sql.WhereClause += WhereClauseOriginal + Where2 + CancelStampWhere;

            sql.FieldList = "T1.reckey";
            return sql;
        }

        public SqlScript CreateScriptForDistinctTrip(bool GoodUdid, int UdidNumber, string WhereClauseOriginal)
        {
            var sql = new SqlScript();

            /** FOR THE "TOTAL TRIPS IN DATA SET" FIGURE, **
            ** WE NEED TO GO BACK TO THE DATABASE AGAIN. **/
            if (GoodUdid && UdidNumber != 0)
            {
                sql.FromClause = "ibtrips T1, ibudids T3";
                sql.KeyWhereClause = "T1.reckey = T3.reckey and ";
                sql.WhereClause = sql.KeyWhereClause + WhereClauseOriginal;
            }
            else
            {
                sql.FromClause = "ibtrips T1";
                sql.WhereClause = WhereClauseOriginal;
            }

            //sql.WhereClause += WhereClauseOriginal;
            sql.FieldList = "T1.reckey";

            return sql;
        }
    }
}
