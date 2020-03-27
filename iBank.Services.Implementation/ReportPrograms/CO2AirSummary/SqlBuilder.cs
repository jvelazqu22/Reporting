
using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.CO2AirSummary
{
    public static class SqlBuilder
    {
        public static SqlScript GetSql(bool hasUdid, bool isPreview, string whereClause, string useDate, string groupField)
        {
            var script = new SqlScript();
            if (hasUdid)
            {
                script.FromClause = isPreview
                    ? "ibtrips T1, iblegs T2, ibudids T3"
                    : "hibtrips T1, hiblegs T2, hibudids T3";
                script.WhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and airline not in ('$$','ZZ') and airline is not null and valcarr not in ('$$','ZZ') and " + whereClause;
                script.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and ";
            }
            else
            {
                script.FromClause = isPreview
                   ? "ibtrips T1, iblegs T2"
                   : "hibtrips T1, hiblegs T2";
                script.WhereClause = "T1.reckey = T2.reckey and " + whereClause;
                script.KeyWhereClause = "T1.reckey = T2.reckey and ";
            }

            script.FieldList = isPreview
                 ? "AirChg, T1.reckey, airline, basefare, class, fltno, mode,convert(int,miles) as miles, convert(int,1) as  plusmin, DITCode, classCat, " + useDate + " as UseDate, "
                 : "AirChg, T1.reckey, airline, basefare, class, fltno, mode,convert(int,miles) as miles, convert(int,plusmin) as plusmin, DITCode, classCat, " + useDate + " as UseDate, ";

            script.FieldList += groupField + " as groupfield";
            return script;

        }

    }
}
