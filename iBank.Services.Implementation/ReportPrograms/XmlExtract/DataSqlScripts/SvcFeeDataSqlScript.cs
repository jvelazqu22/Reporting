using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.XmlExtract.DataSqlScripts
{
    public class SvcFeeDataSqlScript
    {
        public SqlScript GetSqlScript(bool udidExists, bool isPreview, string whereClause)
        {
            string fromClause;
            whereClause = "T1.reckey = T6A.reckey and T1.agency = T6A.agency and " +
                                    "T6A.svcCode = 'TSF' and " + whereClause.Replace("T1.trantype", "T6A.sfTranType");

            if (udidExists)
            {
                fromClause = "hibtrips T1, hibServices T6A, hibudids T3";
                whereClause += " and T1.reckey = T3.reckey";
            }
            else
            {
                fromClause = "hibtrips T1, hibServices T6A";
            }

            var fieldList = "T1.reckey, T1.recloc, T1.invoice, T1.acct, T1.passlast,T1.passfrst, T6A.svcAmt as SvcFee, T6A.svcDesc, T1.agency, T6A.mco, T6A.sfCardnum, T6A.trandate, T6A.sfTranType ";
            
            return new SqlScript
            {
                FieldList = fieldList,
                FromClause = fromClause,
                WhereClause = whereClause,
                KeyWhereClause = whereClause + " and ",
                OrderBy = "",
                GroupBy = ""
            };
        }
    }
}
