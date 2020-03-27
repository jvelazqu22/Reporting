using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iBank.Services.Implementation.ReportPrograms.TravelManagementSummary.DataSqlScripts
{
    public class LegSqlScript
    {
        public SqlScript GetSqlScript(string dateToUse, bool udid, bool isPreview, string whereClause)
        {
            string fromClause;
            string fieldList;
            if (udid)
            {
                fromClause = isPreview ? "ibtrips T1, ibLegs T2, ibudids T3" : "hibtrips T1, hibLegs T2, hibudids T3";
                whereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and " + whereClause;
            }
            else
            {
                fromClause = isPreview ? "ibtrips T1, ibLegs T2" : "hibtrips T1, hibLegs T2";
                whereClause = "T1.reckey = T2.reckey and valcarr not in ('ZZ','$$') and " + whereClause;
            }

            if (isPreview)
            {
                fieldList = " convert(int,1) as plusmin, T1.domintl, T1.bookdate, exchange, " + dateToUse +
                               " as UseDate ";
            }
            else
            {
                fieldList = "convert(int,T1.plusmin) as plusmin, T1.domintl, exchange, " + dateToUse + " as UseDate ";
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
