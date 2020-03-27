
using Domain.Models;
using Domain.Interfaces;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.DataSqlScripts
{
    class ServiceFeeSqlScript : IUserDefinedSqlScript
    {
        public SqlScript GetSqlScript(bool isTripTls, bool udidExists, bool isPreview, string whereClause)
        {
            whereClause = "T1.reckey = T6A.reckey and " + whereClause + " and T1.agency = T6A.agency ";

            var fromClause = "hibtrips T1, hibservices T6A";
            if (udidExists)
            {
                fromClause = "hibtrips T1, hibservices T6A, hibudids T3";
                whereClause += " and t1.reckey = t3.reckey and T1.agency = T3.agency ";
            }

            if (isTripTls)
            {
                fromClause = isPreview ? fromClause.Replace("ibtrips", "vibtripstls") : fromClause.Replace("hibtrips", "vhibtripstls");
            }

            var fieldList = "T1.reckey, T6A.svcamt as svcfee, T6A.svcdesc as descript, T1.agency, T6A.mco as mco, T6A.sfcardnum as cardnum, T6A.TranDate, T6A.SfTranType, T6A.moneytype, T6A.tax1 as stax1, T6A.tax2 as stax2, T6A.tax3 as stax3, T6A.tax4 as stax4, T1.iatanbr, T6A.svccode, T6A.vendorcode ";
            whereClause = whereClause.Replace(" TAX", " T6A.TAX");
            var orderBy = string.Empty;

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
