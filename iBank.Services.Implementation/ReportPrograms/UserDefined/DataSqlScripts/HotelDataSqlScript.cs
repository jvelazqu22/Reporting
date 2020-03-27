using Domain.Models;
using Domain.Interfaces;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.DataSqlScripts
{
    public class HotelDataSqlScript : IUserDefinedSqlScript
    {
        public SqlScript GetSqlScript(bool isTripTls, bool udidExists, bool isPreview, string whereClause)
        {
            string fromClause;
            if (udidExists)
            {
                fromClause = isPreview ? "ibtrips T1, ibhotel T5, ibudids T3" : "hibtrips T1, hibhotel T5, hibudids T3";
                whereClause = "T1.reckey = T5.reckey and T1.reckey = T3.reckey and T1.agency = T3.agency and " + whereClause;
            }
            else
            {
                fromClause = isPreview ? "ibtrips T1, ibhotel T5" : "hibtrips T1, hibhotel T5";
                whereClause = "T1.reckey = T5.reckey and " + whereClause;
            }
            if (isTripTls)
            {
                fromClause = isPreview ? fromClause.Replace("ibtrips", "vibtripstls") : fromClause.Replace("hibtrips", "vhibtripstls");
            }

            var fieldList = " T1.reckey, pseudocity, agentid, T1.agency, T1.recloc, T1.invdate, T1.bookdate, chaincod, metro, hotelnam, hotcity,  hotstate, datein, dateout, bookrate, T5.moneytype, roomtype, hexcprat , guarante , reascodh, hotphone , confirmno, invbyagcy as hinvbyagcy, hotpropid, T5.seqno ";

            if (isPreview)
            {
                fieldList += " , convert(int,0) as numguests, 0.00 as compamt, convert(int,1) as hplusmin, 0.00 as hcommissn,convert(int,nights) as  nights,convert(int,rooms) as rooms, hoteladdr1, hoteladdr2, hotelzip, hotcountry,'' as hotsvgcode, 0.00 as hotstdrate, '' as hottrantyp, emailaddr, gds, hotctrycod, hotcitycod, hotvendcod, hotbktype, convert(int,isnull(smartctrh,0)) as smartctrh, comisableh, hotratetyp, hotsegstat, hotvoiddat, convert(int,isnull(trdtrxnumh,0)) trdtrxnumh, hotsegnum ";
            }
            else
            {
                fieldList += ",convert(int,numguests) as numguests, compamt, convert(int,hplusmin) as hplusmin, hcommissn, convert(int,(hplusmin*nights)) as nights,convert(int,(hplusmin*rooms)) as rooms,hoteladdr1,hoteladdr2,hotelzip,hotcountry,hotsvgcode, hotstdrate, hottrantyp,' ' as emailaddr, ' ' as gds, hotctrycod, hotcitycod, hotvendcod, hotbktype, convert(int,isnull(smartctrh,0)) as smartctrh, comisableh, hotratetyp, hotsegstat, hotvoiddat, convert(int,isnull(trdtrxnumh,0)) trdtrxnumh,0 hotsegnum ";
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
