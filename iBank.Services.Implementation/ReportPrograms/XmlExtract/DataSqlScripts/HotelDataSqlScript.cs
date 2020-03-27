using Domain.Interfaces;
using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.XmlExtract.DataSqlScripts
{
    public class HotelDataSqlScript : IXmlSqlScript
    {
        public SqlScript GetSqlScript(bool udidExists, bool isPreview, string whereClause)
        {
            string fromClause;
            if (udidExists)
            {
                fromClause = isPreview ? "ibtrips T1, ibhotel T5, ibudids T3" : "hibtrips T1, hibhotel T5, hibudids T3";
                whereClause = "T1.reckey = T5.reckey and T1.reckey = T3.reckey and " + whereClause;
            }
            else
            {
                fromClause = isPreview ? "ibtrips T1, ibhotel T5" : "hibtrips T1, hibhotel T5";
                whereClause = "T1.reckey = T5.reckey and " + whereClause;
            }
            
            var fieldList = "T1.reckey, pseudocity, agentid, T1.agency, T1.recloc,  chaincod, metro, hotelnam, hotcity, hotstate, HotelAddr1, HotelAddr2, "
                + " convert(int,nights) as nights,convert(int,rooms) as rooms , datein, bookrate , T5.moneytype, roomtype, hexcprat , guarante , reascodh, hotphone , confirmno, hotpropid ";

            if (isPreview)
            {
                fieldList += ", convert(int,0) as  numguests, 0.00 as compamt, 'I' as hottrantyp, convert(int,1) as hplusmin, 0.00 as hcommissn, invbyagcy, emailaddr, gds ";
            }
            else
            {
                fieldList += ",convert(int,numguests) as  numguests, compamt, hottrantyp, convert(int,hplusmin) as hplusmin, hcommissn, invbyagcy, ' ' as emailaddr, ' ' as gds  ";
            }
            var orderBy = "order by T1.reckey, datein";

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
