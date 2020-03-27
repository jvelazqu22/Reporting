using Domain.Helper;
using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.AccountSummaryAir12MonthTrend
{
    public class AcctSum12MonthSqlCreator
    {
        public SqlScript Create(DateType datetype, string existingWhereClause)
        {
            var sql = new SqlScript();

            var dateToUse = datetype == DateType.InvoiceDate ? "invdate" : "depdate";

            sql.FieldList = "reckey, acct, convert(int, plusmin) as plusmin, airchg," + dateToUse + " as UseDate ";

            sql.FromClause = "hibtrips T1";

            sql.KeyWhereClause = "valcarr NOT IN ('ZZ','$$') AND T1.trantype != 'V' AND ";
            sql.WhereClause = sql.KeyWhereClause + existingWhereClause;

            sql.OrderBy = "";

            return sql;
        }
    }
}
