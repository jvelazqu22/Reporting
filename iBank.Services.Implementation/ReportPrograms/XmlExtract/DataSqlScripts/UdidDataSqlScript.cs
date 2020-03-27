using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.XmlExtract.DataSqlScripts
{
    public class UdidDataSqlScript
    {
        public SqlScript GetSqlScript(bool udidExists, bool isPreview, string whereClause)
        {
            var fromClause = isPreview ? "ibtrips T1, ibudids T3" : "hibtrips T1, hibudids T3";
            whereClause = "T1.reckey = T3.reckey and " + whereClause;

            var fieldList = " T1.reckey, T1.agency, convert(int,udidno) as udidno, udidtext ";

            if (isPreview)
            {
                fieldList += ", t1.recloc";
            }
            else
            {
                fieldList += ", ' ' as recloc";
            }
            var orderBy = "order by T1.reckey, udidno";
         
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
