using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.CarFareSavings
{
    public static class SqlBuilder
    {
        public static SqlScript GetSql(bool hasUdid, bool isPreview, string whereClause)
        {
            var script = new SqlScript();
            if (hasUdid)
            {
                script.FromClause = isPreview
                    ? "ibtrips T1, ibCar T4, ibudids T3"
                    : "hibtrips T1, hibCars T4, hibudids T3";
                script.WhereClause = "T1.reckey = T4.reckey and T1.reckey = T3.reckey and " + whereClause;
                script.KeyWhereClause = "T1.reckey = T4.reckey and T1.reckey = T3.reckey and ";
            }
            else
            {
                script.FromClause = isPreview
                   ? "ibtrips T1, ibCar T4"
                   : "hibtrips T1, hibCars T4";
                script.WhereClause = "T1.reckey = T4.reckey and " + whereClause;
                script.KeyWhereClause = "T1.reckey = T4.reckey and ";
            }

            script.FieldList =
                "T1.reckey, acct, break1, break2, break3, T1.SourceAbbr, confirmNo, passlast, passfrst, invdate, rentDate, company, autocity, autostat, convert(int,days) as days, cartype, aExcpRat, aBookRat, reascoda";
                

            script.FieldList += isPreview 
                ? ", convert(int,1) as  cplusmin, abookrat as carStdRate, '     ' as carSvgCode"
                : ", convert(int,cplusmin) as cplusmin, carStdRate, carSvgCode";
            return script;
            
        }

        public static SqlScript GetUdidSqlScript(string whereClause, bool isReservationReport, List<Tuple<int, int, string>> udids) 
        {
            var script = new SqlScript();

            if (isReservationReport)
            {
                script.FromClause = "ibudids T3";
                script.WhereClause =
                    "reckey in (select distinct T1.reckey from ibTrips T1, ibCar T4 where T1.reckey = T4.reckey and " +
                    whereClause + ")";
            }
            else
            {
                script.FromClause = "hibudids T3";
                script.WhereClause =
                    "reckey in (select distinct T1.reckey from hibTrips T1, hibCars T4 where T1.reckey = T4.reckey and " +
                    whereClause + ")";
            }

            if (udids.Count > 1)
            {
                var udidList = string.Join(",", udids.Select(s => s.Item1));
                script.WhereClause += " and UdidNo in (" + udidList + ") and udidtext is not null ";
            }
            else
            {
                script.WhereClause += " and UdidNo in (" + udids.First().Item1 + ") and udidtext is not null ";
            }
            script.FieldList = "reckey, UdidNo, UdidText";

            return script;
        }

    }
}
