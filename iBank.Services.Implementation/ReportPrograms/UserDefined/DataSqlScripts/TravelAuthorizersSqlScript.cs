
using Domain.Models;
using Domain.Interfaces;


namespace iBank.Services.Implementation.ReportPrograms.UserDefined.DataSqlScripts
{
    public class TravelAuthorizersSqlScript : IUserDefinedSqlScript
    {
        public SqlScript GetSqlScript(bool isTripTls, bool udidExists, bool isPreview, string whereClause)
        {
            string fromClause;
            if (udidExists)
            {
                fromClause = "ibtrips T1, ibtravauth TA1, ibtravauthorizers TA2, ibudids T3";
                whereClause = "T1.reckey = TA1.reckey and TA1.travauthno = TA2.travauthno and T1.reckey = T3.reckey and T1.agency = T3.agency and " + whereClause;
            }
            else
            {
                fromClause = "ibtrips T1, ibtravauth TA1, ibtravauthorizers TA2";
                whereClause = "T1.reckey = TA1.reckey and TA1.travauthno = TA2.travauthno and " + whereClause;
            }

            var fieldList = " T1.reckey, T1.agency, T1.recloc, T1.invoice, T1.invdate, pseudocity, agentid, TA2.authrzrnbr, TA2.authstatus, TA2.statustime, TA2.auth1email, TA2.auth2email, TA2.apvreason ";

            var orderBy = "order by T1.reckey, TA1.travauthno, TA2.apsequence";

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

