using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.SvcFeeSumTran
{
    public class SvcFeeSumTranSqlCreator
    {
        public SqlScript Create(string existingWhereClause, bool useHibServices)
        {
            var sql = new SqlScript();

            if (useHibServices)
            {
                sql.FromClause = "hibTrips T1, hibServices T6A";
                sql.KeyWhereClause = "T1.reckey = T6A.reckey and T1.agency = T6A.agency and ";

                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;

                sql.FieldList = "svcDesc as descript, svcAmt, null as trandate";
            }
            else
            {
                sql.FromClause = "hibsvcfees T6";
                sql.KeyWhereClause = "";
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }

            sql.OrderBy = "";

            return sql;
        }

        public string GetReplacedWhereClause(string whereClause)
        {
            return whereClause.Replace("FTRANTYPE", "SFTRANTYPE")
                                    .Replace("T1.trantype", "T6A.sfTrantype")
                                    .Replace("T1.SVCFEE", "SVCAMT")
                                    .Replace("T6.SVCFEE", "SVCAMT")
                                    .Replace("DESCRIPT", "SVCDESC")
                                    .Replace("T6.IATANBR", "IATANBR")
                                    .Replace("TAX", "T6A.TAX");
        }
    }
}