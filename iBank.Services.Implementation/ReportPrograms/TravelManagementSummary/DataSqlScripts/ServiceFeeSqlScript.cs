using Domain.Models;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.TravelManagementSummary.DataSqlScripts
{
    public class ServiceFeeSqlScript
    {
        public SqlScript GetSqlScript(string dateToUse, bool udidExists, string whereClause, bool includeOrphanSvcFees)
        {
            var sql = new SqlScript();

            if (udidExists)
            {
                sql.FromClause = "hibtrips T1, hibServices T6A, hibudids T3";
                sql.KeyWhereClause = "T1.reckey = T6A.reckey and T1.agency = T6A.agency and T1.reckey = T3.reckey and T1.agency = T3.agency and T6A.svcCode = 'TSF' and ";
            }
            else
            {
                sql.FromClause = "hibtrips T1, hibServices T6A";
                sql.KeyWhereClause = "T1.reckey = T6A.reckey and T1.agency = T6A.agency and T6A.svcCode = 'TSF' and ";
            }

            sql.WhereClause = $"{sql.KeyWhereClause} {whereClause.Replace("T1.trantype", "T6A.sfTrantype")}";
            
            if (includeOrphanSvcFees)
            {
                sql.FieldList = "T6A.svcAmt, trandate, trandate as UseDate, recordno as reckey, recloc, invoice, acct, trantype, passlast, passfrst, svcfee";
            }
            else
            {
                sql.FieldList = dateToUse.EqualsIgnoreCase("depdate")
                                    ? "T6A.svcAmt, trandate, depdate as UseDate "
                                    : "T6A.svcAmt, trandate, trandate as UseDate ";
            }

            if (!includeOrphanSvcFees)
            {
                sql.WhereClause += " and origValCar not in ('SVCFEEONLY','ZZ:S')";
            }

            sql.OrderBy = "";
            
            return sql;
        }
    }
}
