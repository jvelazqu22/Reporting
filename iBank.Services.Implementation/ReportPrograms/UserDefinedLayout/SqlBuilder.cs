using Domain.Helper;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.UserDefinedLayout
{
    public static class SqlBuilder
    {
        public static string GetSql(ReportGlobals globals)
        {
            var reportKeys = globals.GetParmValue(WhereCriteria.REPORTKEY) + globals.GetParmValue(WhereCriteria.INREPORTKEY);

            var whereClause = string.Empty;
            if (!string.IsNullOrEmpty(reportKeys))
            {
                whereClause = " and t1.reportkey in (" + reportKeys + ")";
            }
            whereClause = whereClause + " and t1.agency = '" + globals.Agency + "' and t1.usernumber = " + globals.UserNumber;

            return "SELECT t1.reportkey, t1.crname, t1.crtitle, t1.crsubtit, " +
                "t1.crtype, t1.lastused, t2.colname, convert(int, t2.colorder) as colorder,convert(int, t2.sort) as sort, " + 
                "t2.pagebreak, t2.subtotal, t2.udidhdg1, t2.udidhdg2, t2.udidwidth, " +
                "convert(int, t2.udidtype) as udidtype, t2.horalign, convert(int, t2.grpbreak) as grpbreak, t3.lastname, t3.firstname " + 
                "FROM userrpts t1, userrpt2 t2, ibuser t3 WHERE " + 
                "t1.reportkey = t2.reportkey AND t1.usernumber = t3.usernumber " +
                whereClause + " ORDER BY t1.crtype, t2.crname, t1.reportkey, t2.colorder";
        }
    }
}
