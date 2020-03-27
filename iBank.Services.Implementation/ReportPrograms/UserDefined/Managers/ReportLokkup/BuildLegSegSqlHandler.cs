using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Models;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ReportLokkup
{
    public class BuildLegSegSqlHandler
    {
        private readonly WhereClauseWithAdvanceParamsHandler whereClauseWithAdvanceParamsHandler = new WhereClauseWithAdvanceParamsHandler();

        public string BuildLegSegSql(bool tripTlsSwitch, ReportGlobals globals, string whereClause, BuildWhere buildWhere)
        {
            var udid = globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(0);
            var isReservationReport = globals.ParmValueEquals(WhereCriteria.PREPOST, "1");

            var sql = new SqlScript();

            if (udid != 0)
            {
                sql = GetSqlWithUdid(globals, whereClause);
            }
            else
            {
                sql = GetSqlWithoutUdid(globals, whereClause);
            }

            if (tripTlsSwitch)
            {
                sql.FromClause = isReservationReport ? sql.FromClause.Replace("ibtrips", "vibtripstls") : sql.FromClause.Replace("hibtrips", "vhibtripstls");
            }

            sql.FieldList = "T1.reckey, connect,origin,destinat,airline,class,classcat, convert(int,miles) as miles,mode,farebase, " +
                "TD.segment_itinerary as DerivedSegRouting, TD.leg_itinerary as DerivedLegRouting, TD.trip_class as DerivedTripClass, TD.trip_class_category as DerivedTripClassCat, ";

            //Reservation table doesn't have this field refer to transid.
            sql.FieldList += isReservationReport ?
                   "'' as DerivedTransId" : "TD.tripTransactionID as DerivedTransId";

            sql.OrderBy = "order by T1.reckey, T2.seqno";

            sql.WhereClause = whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildWhere, true);

            return SqlProcessor.ProcessSql(sql.FieldList, false, sql.FromClause, sql.WhereClause, sql.OrderBy, buildWhere.ReportGlobals);
        }

        private SqlScript GetSqlWithUdid(ReportGlobals globals, string whereClause)
        {
            var isReservationReport = globals.ParmValueEquals(WhereCriteria.PREPOST, "1");

            var processkey = globals.ProcessKey;
            var sql = new SqlScript();
            if (processkey == (int)ReportTitles.HotelUserDefinedReports)
            {
                sql.FromClause = isReservationReport
                    ? "ibtrips T1, iblegs T2, ibudids T3, ibhotel T5, ibTripsDerivedData TD"
                    : "hibtrips T1, hiblegs T2, hibudids T3, hibhotel T5, hibTripsDerivedData TD";

                sql.KeyWhereClause = "T1.reckey = t2.reckey and T1.reckey = T3.reckey and T1.reckey = T5.reckey and T1.reckey=TD.reckey " +
                    "and valcarr != 'ZZ' and valcarr != '$$' and airline != 'ZZ' and airline != '$$' and ";
            }
            else if (processkey == (int)ReportTitles.CarUserDefinedReports)
            {
                sql.FromClause = isReservationReport
                    ? "ibtrips T1, iblegs T2, ibudids T3, ibcar T4, ibTripsDerivedData TD"
                    : "hibtrips T1, hiblegs T2, hibudids T3, hibcars T4, hibTripsDerivedData TD";

                sql.KeyWhereClause = "T1.reckey = t2.reckey and T1.reckey = T3.reckey and T1.reckey = T4.reckey and T1.reckey=TD.reckey " +
                    "and valcarr != 'ZZ' and valcarr != '$$' and airline != 'ZZ' and airline != '$$' and ";
            }
            else
            {
                sql.FromClause = isReservationReport
                    ? "ibtrips T1, iblegs T2, ibudids T3, ibTripsDerivedData TD"
                    : "hibtrips T1, hiblegs T2, hibudids T3, hibTripsDerivedData TD";

                sql.KeyWhereClause = "T1.reckey = t2.reckey and T1.reckey = T3.reckey and T1.reckey=TD.reckey " +
                    "and valcarr != 'ZZ' and valcarr != '$$' and airline != 'ZZ' and airline != '$$' and ";
            }

            if (processkey == (int)ReportTitles.ServiceFeeUserDefinedReports)
            {
                sql.WhereClause = sql.KeyWhereClause + $" t1.reckey in (select reckey from hibServices where {whereClause})";
            }
            else
            {
                sql.WhereClause = sql.KeyWhereClause + whereClause;
            }

            return sql;
        }

        private SqlScript GetSqlWithoutUdid(ReportGlobals Globals, string WhereClause)
        {
            var isReservationReport = Globals.ParmValueEquals(WhereCriteria.PREPOST, "1");
            var processkey = Globals.ProcessKey;
            var sql = new SqlScript();

            if (processkey == (int)ReportTitles.HotelUserDefinedReports)
            {
                sql.FromClause = isReservationReport
                ? "ibtrips T1, iblegs T2, ibhotel T5, ibTripsDerivedData TD"
                : "hibtrips T1, hiblegs T2, hibhotel T5, hibTripsDerivedData TD";

                sql.KeyWhereClause = "T1.reckey = t2.reckey and T1.reckey = T5.reckey and T1.reckey=TD.reckey and valcarr != 'ZZ' and "
                    + "valcarr != '$$' and airline != 'ZZ' and airline != '$$'  and ";
            }
            else if (processkey == (int)ReportTitles.CarUserDefinedReports)
            {
                sql.FromClause = isReservationReport
                ? "ibtrips T1, iblegs T2, ibcar T4, ibTripsDerivedData TD"
                : "hibtrips T1, hiblegs T2, hibcars T4, hibTripsDerivedData TD";

                sql.KeyWhereClause = "T1.reckey = t2.reckey and T1.reckey = T4.reckey and T1.reckey=TD.reckey and valcarr != 'ZZ' and "
                    + "valcarr != '$$' and airline != 'ZZ' and airline != '$$'  and ";
            }
            else
            {
                sql.FromClause = isReservationReport
                ? "ibtrips T1, iblegs T2, ibTripsDerivedData TD"
                : "hibtrips T1, hiblegs T2, hibTripsDerivedData TD";

                sql.KeyWhereClause = "T1.reckey = t2.reckey and T1.reckey=TD.reckey and valcarr != 'ZZ' and "
                    + "valcarr != '$$' and airline != 'ZZ' and airline != '$$'  and ";
            }

            if (processkey == (int)ReportTitles.ServiceFeeUserDefinedReports)
            {
                sql.WhereClause = sql.KeyWhereClause + $" t1.reckey in (select reckey from hibServices where {WhereClause})";
            }
            else
            {
                sql.WhereClause = sql.KeyWhereClause + WhereClause;
            }
            return sql;
        }
    }
}
