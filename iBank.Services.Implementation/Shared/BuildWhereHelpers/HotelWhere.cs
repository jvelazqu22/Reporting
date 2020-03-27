using Domain.Helper;

using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.Shared.BuildWhereHelpers
{
    public class HotelWhere : AbstractWhere
    {
        public void GetHotelWhere(ReportGlobals globals, BuildWhere where, string notInText)
        {
            where.WhereClauseHotel = AddListWhere(globals, where.WhereClauseHotel, WhereCriteria.HOTCHAIN, WhereCriteria.INHOTCHAINS, WhereCriteria.NOTINHOTCHAINS, "CHAINS", "chaincod", "Hotel Chain", notInText);
            where.WhereClauseHotel = AddListWhere(globals, where.WhereClauseHotel, WhereCriteria.ROOMTYPE, WhereCriteria.INROOMTYPES, WhereCriteria.NOTINROOMTYPE, "ROOMTYPE", "roomtype", "Room Type", notInText);

            where.WhereClauseHotel = AddSimpleWhere(globals, where, where.WhereClauseHotel, WhereCriteria.HOTELNAM, "hotelnam ", "Hotel Name");
            where.WhereClauseHotel = AddSimpleWhere(globals, where, where.WhereClauseHotel, WhereCriteria.HOTCITY, "hotcity", "Hotel City");
            where.WhereClauseHotel = AddSimpleWhere(globals, where, where.WhereClauseHotel, WhereCriteria.HOTSTATE, "hotstate", "Hotel State");
        }
    }
}
