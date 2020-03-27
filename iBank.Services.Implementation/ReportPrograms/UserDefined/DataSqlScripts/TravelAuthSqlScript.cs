using Domain.Interfaces;
using Domain.Models;


namespace iBank.Services.Implementation.ReportPrograms.UserDefined.DataSqlScripts
{
    public class TravelAuthSqlScript : IUserDefinedSqlScript
    {
        public SqlScript GetSqlScript(bool isTripTls, bool udidExists, bool isPreview, string whereClause)
        {
            string fromClause;
            if (udidExists)
            {
                fromClause = "ibtrips T1, ibtravauth TA1, ibudids T3";
                whereClause = "T1.reckey = TA1.reckey and T1.reckey = T3.reckey and T1.agency = T3.agency and " + whereClause;
            }
            else
            {
                fromClause = "ibtrips T1, ibtravauth TA1";
                whereClause = "T1.reckey = TA1.reckey and " + whereClause;
            }

            var fieldList = " T1.reckey, T1.agency, T1.recloc, T1.invoice, T1.invdate, TA1.acct, pseudocity, agentid, TA1.travauthno,TA1.authstatus, TA1.statustime, cast(TA1.trvlremail as nvarchar(max)) trvlremail , cast(TA1.tvlrccaddr as nvarchar(max)) tvlrccaddr, TA1.rtvlcode, TA1.outpolcods, cast(TA1.authcomm as nvarchar(max)) authcomm, TA1.bookedgmt, TA1.cliauthnbr ";


            var orderBy = "order by T1.reckey, TA1.travauthno ";

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
