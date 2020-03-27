using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iBank.Services.Implementation.ReportPrograms.TravelManagementSummary.DataSqlScripts
{
    public class AirSqlScript
    {
        public SqlScript GetSqlScript(string dateToUse, bool udid, bool isPreview, string whereClause)
        {
            string fromClause;
            string fieldList;
            if (udid)
            {
                fromClause = isPreview ? "ibtrips T1, ibudids T3" : "hibtrips T1, hibudids T3";
                whereClause = "T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and " + whereClause;
            }
            else
            {
                fromClause = isPreview ? "ibtrips T1" : "hibtrips T1";
                whereClause = "valcarr not in ('ZZ','$$') and " + whereClause;
            }
            
            if (isPreview)
            {
                fieldList = " convert(int,1) as plusmin, exchange, airchg, stndchg, mktfare, basefare, " +
                               "'I' as trantype, offrdchg, bookdate, invdate, depdate, domintl, " +
                               "bktool, valcarr, ' ' as valcarmode, " + dateToUse + " as UseDate ";
            }
            else
            {
                fieldList = "convert(int,plusmin) as plusmin, exchange, airchg, stndchg, mktfare, basefare, trantype, " +
                    "offrdchg, invdate, depdate, domintl, bktool, valcarr, valcarMode, " +
                    dateToUse + " as UseDate "; 
            }

            var orderBy = "";

            return new SqlScript
            {
                FieldList = fieldList,
                FromClause = fromClause,
                WhereClause = whereClause,
                KeyWhereClause = whereClause + " and ",
                OrderBy = orderBy,
                GroupBy = ""
            };
        }
    }
}
