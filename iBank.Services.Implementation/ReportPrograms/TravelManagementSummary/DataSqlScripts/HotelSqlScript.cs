using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iBank.Services.Implementation.ReportPrograms.TravelManagementSummary.DataSqlScripts
{
    public class HotelSqlScript
    {
        public SqlScript GetSqlScript(string dateToUse, bool udid, bool isPreview, string whereClause)
        {
            string fromClause;
            string fieldList;
            if (udid)
            {
                fromClause = isPreview ? "ibtrips T1, ibudids T3, ibhotel T5" : "hibtrips T1, hibudids T3, hibhotel T5";
                whereClause = "T1.reckey = T3.reckey and T1.reckey = T5.reckey and " +
                                   whereClause.Replace("T1.trantype", "T5.HotTranTyp");
            }
            else
            {
                fromClause = isPreview ? "ibtrips T1, ibhotel T5" : "hibtrips T1, hibhotel T5";
                whereClause = "T1.reckey = T5.reckey and " +
                                   whereClause.Replace("T1.trantype", "T5.HotTranTyp");
            }

            if (isPreview)
            {
                fieldList = dateToUse +
                                 " as UseDate,  convert(int,1) as hplusmin, T1.bookdate,convert(int,nights) as nights, convert(int,1) as rooms, bookrate ";
            }
            else
            {
                fieldList = dateToUse +
                                 " as UseDate, convert(int,hplusmin) as hplusmin,convert(int,nights) as nights, convert(int,rooms) as rooms, bookrate ";
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
