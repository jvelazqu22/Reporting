using Domain.Models;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomHotels
{
    public class SqlBuilder
    {
        public static SqlScript GetSql(int udidNo, bool isPreview, string whereClause, BuildWhere BuildWhere)
        {
            var script = new SqlScript();
            if (udidNo > 0)
            {
                script.FromClause = isPreview
                    ? "ibtrips T1, ibhotel T5, ibudids T3"
                    : "hibtrips T1, hibhotel T5, hibudids T3";
                script.KeyWhereClause = "T1.reckey = T5.reckey and T1.reckey = T3.reckey and ";
                script.WhereClause = script.KeyWhereClause + BuildWhere.WhereClauseFull;// + BuildWhere.WhereClauseAdvanced;
            }
            else
            {
                script.FromClause = isPreview
                    ? "ibtrips T1, ibhotel T5"
                    : "hibtrips T1, hibhotel T5";
                script.KeyWhereClause = "T1.reckey = T5.reckey and ";
                script.WhereClause = script.KeyWhereClause + BuildWhere.WhereClauseFull;// + BuildWhere.WhereClauseAdvanced;
            }
            script.FieldList = isPreview
                ? "T1.reckey, chaincod,hotcity,hotstate,'' as hotcountry,metro,hotelnam,bookrate,convert(int,nights) as nights,convert(int,rooms) as  rooms,  convert(int,1) as hplusmin"
                : "T1.reckey, chaincod,hotcity,hotstate,hotcountry,metro,hotelnam,bookrate,convert(int,nights) as nights,convert(int,rooms) as  rooms,  convert(int, hplusmin) as hplusmin";

            return script;
        }
    }
}
