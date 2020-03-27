using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.HotelFareSavings
{
    public static class SqlBuilder
    {
        public static SqlScript GetSql(bool hasUdid, bool isPreview, string whereClause)
        {
            var script = new SqlScript();
            if (hasUdid)
            {
                script.FromClause = isPreview
                    ? "ibtrips T1, ibHotel T5, ibudids T3"
                    : "hibtrips T1, hibHotel T5, hibudids T3";
                script.KeyWhereClause = "T1.reckey = T5.reckey and T1.reckey = T3.reckey and ";
                script.WhereClause = script.KeyWhereClause + whereClause;
            }
            else
            {
                script.FromClause = isPreview
                   ? "ibtrips T1, ibHotel T5"
                   : "hibtrips T1, hibHotel T5";
                script.KeyWhereClause = "T1.reckey = T5.reckey and ";
                script.WhereClause = script.KeyWhereClause + whereClause;
            }

            script.FieldList =
                "T1.reckey, acct, break1, break2, break3, T1.SourceAbbr, confirmNo, passlast, passfrst, invdate, dateIn, hotelnam, hotcity, hotstate, convert(int,nights) as nights, convert(int,rooms) as rooms, roomtype, hExcpRat, bookRate, reascodh";

            script.FieldList += isPreview 
                ? ",  convert(int,1) as  hplusmin, bookRate as hotStdRate, '     ' as hotSvgCode"
                : ",  convert(int,hplusmin) as hplusmin, hotStdRate, hotSvgCode";
            return script;
        }

        public static SqlScript GetUdidSqlScript(string whereClause, bool isReservationReport, List<Tuple<int, int, string>> udids)
        {
            var script = new SqlScript();

            if (isReservationReport)
            {
                script.FromClause = "ibudids T3";
                script.WhereClause =
                    "reckey in (select distinct T1.reckey from ibTrips T1, ibHotel T5 where T1.reckey = T5.reckey and " + whereClause + ")";
            }
            else
            {
                script.FromClause = "hibtrips T1, hibHotel T5, hibudids T3";
                script.WhereClause =
                    "T1.reckey = T5.reckey AND T5.reckey = T3.reckey AND T3.reckey in (select distinct T1.reckey from hibTrips T1, hibHotel T5 where T1.reckey = T5.reckey and " +
                    whereClause + ")";
            }

            if (udids.Count > 1)
            {
                var udidList = string.Join(",", udids.Select(s => s.Item2));
                script.WhereClause += " and UdidNo in (" + udidList + ") and udidtext is not null ";
            }
            else
            {
                script.WhereClause += " and UdidNo in (" + udids.First().Item2 + ") and udidtext is not null ";
            }
            script.FieldList = "T3.reckey, UdidNo, UdidText";

            return script;
        }

    }
}
