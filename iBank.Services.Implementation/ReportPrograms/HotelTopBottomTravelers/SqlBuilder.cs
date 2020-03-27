using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.HotelTopBottomTravelers
{
    public static class SqlBuilder
    {
        // Builds sql script for getting raw data
        public static SqlScript GetSql(bool hasUdid, bool isPreview, string whereClause)
        {
            var script = new SqlScript();
            if (hasUdid)
            {
                script.FromClause = isPreview
                    ? "ibtrips T1, ibhotel T5, ibudids T3"
                    : "hibtrips T1, hibhotel T5, hibudids T3";
                script.KeyWhereClause = "T1.reckey = T3.reckey and T1.reckey = T5.reckey and passlast is not null and ";
                script.WhereClause = script.KeyWhereClause + whereClause;
            }
            else
            {
                script.FromClause = isPreview
                   ? "ibtrips T1, ibhotel T5"
                   : "hibtrips T1, hibhotel T5";
                script.KeyWhereClause = "T1.reckey = T5.reckey and passlast is not null and ";
                script.WhereClause = script.KeyWhereClause + whereClause;
            }

            script.FieldList = isPreview
                 ? "passlast, passfrst,convert(int,nights) as nights,convert(int,rooms) as rooms, bookrate,convert(int,1) as hplusmin, datein"
                 : "passlast, passfrst,convert(int,nights) as nights,convert(int,rooms) as rooms, bookrate,convert(int,hplusmin) as hplusmin, datein";

            return script;

        }

       

    }
}
