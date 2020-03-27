
using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.CO2CombinedSummary
{
    public static class SqlBuilder
    {
        public static SqlScript GetSqlTripsAndLegs(bool hasUdid, bool isPreview, string whereClause, string useDate)
        {
            var script = new SqlScript();
            if (hasUdid)
            {
                script.FromClause = isPreview
                    ? "ibtrips T1, iblegs T2, ibudids T3"
                    : "hibtrips T1, hiblegs T2, hibudids T3";
                script.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and ";
                script.WhereClause = script.KeyWhereClause + whereClause;
            }
            else
            {
                script.FromClause = isPreview
                   ? "ibtrips T1, iblegs T2"
                   : "hibtrips T1, hiblegs T2";
                script.KeyWhereClause = "T1.reckey = T2.reckey and ";
                script.WhereClause = script.KeyWhereClause + whereClause;
            }

            script.FieldList = isPreview
                 ? $"AirChg, T1.reckey, airline, basefare, class as classcode, fltno, mode,convert(int,miles) as miles, convert(int,1) as  plusmin, DITCode, classCat, {useDate} as UseDate"
                 : $"AirChg, T1.reckey, airline, basefare, class as classcode, fltno, mode,convert(int,miles) as miles, convert(int,plusmin) as plusmin, DITCode, classCat, {useDate} as UseDate";
            return script;

        }

        public static SqlScript GetSqlCar(bool hasUdid, bool isPreview, string whereClause, string useDate)
        {
            var script = new SqlScript();
            if (hasUdid)
            {
                script.FromClause = isPreview
                    ? "ibtrips T1, ibcar T4, ibudids T3"
                    : "hibtrips T1, hibcars T4, hibudids T3";
                script.KeyWhereClause = "T1.reckey = T4.reckey and T1.reckey = T3.reckey and ";
                script.WhereClause = script.KeyWhereClause + whereClause.Replace("T1.trantype", "T4.CarTranTyp");
            }
            else
            {
                script.FromClause = isPreview
                   ? "ibtrips T1, ibCar T4"
                   : "hibtrips T1, hibCars T4";
                script.KeyWhereClause = "T1.reckey = T4.reckey and ";
                script.WhereClause = script.KeyWhereClause + whereClause.Replace("T1.trantype", "T4.CarTranTyp");
            }

            script.FieldList = isPreview
                 ? $"convert(int, days) as days, convert(int, 1) as cplusmin, abookrat, autocity, autostat, carType, {useDate} as UseDate"
                 : $"convert(int, days) as days, convert(int, cplusmin) as cplusmin, abookrat, autocity, autostat, carType, {useDate} as UseDate";
            return script;

        }

        public static SqlScript GetSqlHotel(bool hasUdid, bool isPreview, string whereClause, string useDate)
        {
            var script = new SqlScript();
            if (hasUdid)
            {
                script.FromClause = isPreview
                    ? "ibtrips T1, ibudids T3, ibhotel T5"
                    : "hibtrips T1, hibudids T3, hibhotel T5";
                script.KeyWhereClause = "T1.reckey = T5.reckey and T1.reckey = T3.reckey and ";
                script.WhereClause = script.KeyWhereClause + whereClause.Replace("T1.trantype", "T5.HotTranTyp");
            }
            else
            {
                script.FromClause = isPreview
                   ? "ibtrips T1,ibhotel T5"
                   : "hibtrips T1,hibhotel T5";
                script.KeyWhereClause = "T1.reckey = T5.reckey and ";
                script.WhereClause = script.KeyWhereClause + whereClause.Replace("T1.trantype", "T5.HotTranTyp");
            }

            script.FieldList = isPreview
                 ? $"chaincod, convert(int, 1) as hplusmin, convert(int, nights) as nights, convert(int, rooms) as rooms, bookrate, hotcity, hotstate, {useDate} as UseDate"
                 : $"chaincod, convert(int, hplusmin) as hplusmin, convert(int, nights) as nights, convert(int, rooms) as rooms, bookrate, hotcity, hotstate, {useDate} as UseDate";
            return script;

        }
    }
}
