using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iBank.Services.Implementation.ReportPrograms.TravelManagementSummary.DataSqlScripts
{
    public class CarSqlScript
    {
        public SqlScript GetSqlScript(string dateToUse, bool udid, bool isPreview, string whereClause)
        {
            string fromClause;
            string fieldList;
            if (udid)
            {
                fromClause = isPreview ? "ibtrips T1, ibudids T3, ibcar T4" : "hibtrips T1, hibudids T3, hibcars T4";
                whereClause = "T1.reckey = T3.reckey and T1.reckey = T4.reckey and " +
                                     whereClause.Replace("T1.trantype", "T4.CarTranTyp");
            }
            else
            {
                fromClause = isPreview ? "ibtrips T1, ibcar T4" : "hibtrips T1, hibcars T4";
                whereClause = "T1.reckey = T4.reckey and " +
                                     whereClause.Replace("T1.trantype", "T4.CarTranTyp");
            }

            if (isPreview)
            {
                fieldList = dateToUse +
                                   " as UseDate, 1 as cplusmin, T1.bookdate, convert(int,days) as days, abookrat,  convert(int,1) as Numcars, carType ";
            }
            else
            {
                fieldList = dateToUse +
                                   " as UseDate,RentDate,convert(int,cplusmin) as cplusmin,convert(int,days) as days, abookrat,convert(int,Numcars) as Numcars, carType ";
            }

            var orderBy = "";

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
