using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.ValidatingCarrier
{
    public static class SqlBuilder
    {

        public static SqlScript GetSql(bool hasUdid,string whereClause)
        {
            var script = new SqlScript();
            if (hasUdid)
            {
                script.FromClause = "hibtrips T1, hibudids T3";
                script.WhereClause = "T1.reckey = T2.reckey and valcarr not in ('ZZ','$$', 'XD') and " + whereClause;
                script.KeyWhereClause = "T1.reckey = T2.reckey and valcarr not in ('ZZ','$$', 'XD') and ";
                script.FieldList = "valcarr, 'A' as valcarMode, convert(int,plusmin) as plusmin, acommisn, airchg";
            }
            else
            {
                script.FromClause = "hibtrips T1";
                script.WhereClause = "valcarr not in ('ZZ','$$', 'XD') and " + whereClause;
                script.KeyWhereClause = "valcarr not in ('ZZ','$$', 'XD') and ";
                script.FieldList = "valcarr, valcarMode, convert(int,plusmin) as plusmin, acommisn, airchg";
            }
            
            return script;
            
        }
        
    }
}
