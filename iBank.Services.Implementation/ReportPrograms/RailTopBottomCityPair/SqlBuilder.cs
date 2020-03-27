using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.RailTopBottomCityPair
{
    public static class SqlBuilder
    {

        public static SqlScript GetSql(bool hasUdid, bool isPreview, string whereClause)
        {
            var script = new SqlScript();
            if (hasUdid)
            {
                script.FromClause = isPreview
                    ? "ibtrips T1, iblegs T2, ibudids T3"
                    : "hibtrips T1, hiblegs T2, hibudids T3";
                script.WhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and airline not in ('$$','ZZ') and airline is not null and mode = 'R' and " + whereClause;
                script.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and ";
            }
            else
            {
                script.FromClause = isPreview
                   ? "ibtrips T1, iblegs T2"
                   : "hibtrips T1, hiblegs T2 ";
                script.WhereClause = "T1.reckey = T2.reckey and airline not in ('$$','ZZ') and airline is not null and mode = 'R' and " + whereClause;
                script.KeyWhereClause = "T1.reckey = T2.reckey and ";
            }
            
            script.FieldList = isPreview
                ? "T1.reckey, airchg, faretax, basefare, space(6) as orgdestemp, convert(int, 1) as plusmin, 00.000 as NumTicks, 00000000 as RecordNo"
                : "T1.reckey, airchg, faretax, basefare, space(6) as orgdestemp, convert(int, plusmin) as plusmin, 00.000 as NumTicks, 00000000 as RecordNo";

            script.OrderBy = "order by T1.reckey";
            return script;

        }

    }
}
