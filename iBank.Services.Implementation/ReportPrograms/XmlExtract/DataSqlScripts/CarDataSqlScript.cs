using Domain.Models;
using Domain.Interfaces;

namespace iBank.Services.Implementation.ReportPrograms.XmlExtract.DataSqlScripts
{
    public class CarDataSqlScript : IXmlSqlScript
    {
        public SqlScript GetSqlScript(bool udidExists, bool isPreview, string whereClause)
        {
            string fromClause;
            if (udidExists)
            {
                fromClause = isPreview ? "ibtrips T1, ibcar T4, ibudids T3" : "hibtrips T1, hibcars T4, hibudids T3";
                whereClause = "T1.reckey = T4.reckey and T1.reckey = T3.reckey and T1.agency = T3.agency and " + whereClause;
            }
            else
            {
                fromClause = isPreview ? "ibtrips T1, ibcar T4" : "hibtrips T1, hibcars T4";
                whereClause = "T1.reckey = T4.reckey and " + whereClause;
            }

            var fieldList = " T1.reckey, T1.agency, T1.recloc, T1.invoice, T1.invdate, pseudocity, agentid, company, autostat, autocity, convert(int,days) as days, rentdate, " +
                " cartype, carcode, reascoda, abookrat, aexcprat, milecost, ratetype, citycode,convert(int,numcars) as numcars, confirmno ";

            if (isPreview)
            {
                fieldList += ",0.00 as compamt, convert(int,1) as cplusmin, 0.00 as ccommisn, 'I' as cartrantyp, T4.moneytype, emailaddr, gds ";
            }
            else
            {
                fieldList += ", compamt, convert(int,cplusmin) as cplusmin, ccommisn, cartrantyp, invbyagcy, ' ' as moneytype, ' ' as emailaddr, ' ' as gds ";
            }

            var orderBy = "order by T1.reckey, rentdate";

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
