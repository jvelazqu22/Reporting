using Domain.Models;
using Domain.Interfaces;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.DataSqlScripts
{
    public class CarDataSqlScript : IUserDefinedSqlScript
    {
        public SqlScript GetSqlScript(bool isTripTls, bool udidExists, bool isPreview, string whereClause)
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
            if (isTripTls)
            {
                fromClause = isPreview ? fromClause.Replace("ibtrips", "vibtripstls") : fromClause.Replace("hibtrips", "vhibtripstls");
            }

            var fieldList = "T1.reckey, T1.agency, T1.recloc, T1.invoice, T1.invdate, pseudocity, agentid,  invdate, bookdate, company, autostat, autocity, rentdate, dateback,  cartype, reascoda, abookrat, aexcprat, milecost, ratetype, citycode,  carcode, confirmno as aconfirmno, invbyagcy as ainvbyagcy, ";

            fieldList += "convert(int, ROW_NUMBER() OVER(PARTITION BY T1.reckey ORDER BY T1.reckey, rentdate) - 1) seqno ";

            if (isPreview)
            {
                fieldList += ",0.00 as compamt, convert(int,1) as cplusmin, 0.00 as ccommisn, days, numcars, '' as carsvgcode, 0.00 as carstdrate, '' as cartrantyp, T4.moneytype, emailaddr, gds, carctrycod, '' as carvendcod, carbktype, convert(int, isnull(smartctrc,0)) as smartctrc, comisablec, carratetyp, carsegstat, carvoiddat, isnull(trdtrxnumc,0) trdtrxnumc";
            }
            else
            {
                fieldList += " , compamt,convert(int,cplusmin) as cplusmin, ccommisn, (cplusmin*days) as days, (cplusmin*numcars) as numcars, carsvgcode, carstdrate, cartrantyp, T4.moneytype, ' ' as emailaddr, ' ' as gds, carctrycod, carvendcod, carbktype, convert(int,isnull(smartctrc,0)) as smartctrc, comisablec, carratetyp, carsegstat, carvoiddat, isnull(trdtrxnumc,0) trdtrxnumc";
            }

            //if orderBy is change, the seqno field used ROW_NUMBER() order by needs to change too
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
