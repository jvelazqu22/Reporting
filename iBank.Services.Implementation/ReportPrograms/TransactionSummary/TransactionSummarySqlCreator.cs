using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.TransactionSummary
{
    public class TransactionSummarySqlCreator
    {
        public SqlScript CreateScript(int _begYear, int _endYear, int _begMonthNumber, int _endMonthNumber, string whereTrip, string agency)
        {
            var sql = new SqlScript();

            sql.KeyWhereClause = _begYear == _endYear
                ? sql.KeyWhereClause = "year = " + _begYear + " and month between " + _begMonthNumber + " and " + _endMonthNumber
                :
                sql.KeyWhereClause = "year between " + _begYear + " and " + _endYear;

            sql.KeyWhereClause = "T1.agency = T2.agency and " + "T1.agency = '" + agency + "' and PrelimFinal = 'F' and " + sql.KeyWhereClause;
            sql.WhereClause += sql.KeyWhereClause + whereTrip;
            sql.FieldList = "T2.sourceAbbr, T2.acct, T2.source, convert(int,year) as year, convert(int,month) as month, T2.category, T2.catcount ";
            sql.FromClause = "mstragcy T1, Transtrack T2";

            return sql;
        }
    }
}
