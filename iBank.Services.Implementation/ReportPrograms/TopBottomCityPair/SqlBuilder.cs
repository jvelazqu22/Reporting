namespace iBank.Services.Implementation.ReportPrograms.TopBottomCityPair
{
    public class SqlBuilder
    {
        public string GetFromClause(bool isPreview, string udidNumber)
        {
            string fromClause = "";
            int udid;
            var goodUdid = int.TryParse(udidNumber, out udid);
            fromClause = goodUdid && udid != 0
                ? isPreview ? "ibtrips T1, iblegs T2, ibudids T3" : "hibtrips T1, hiblegs T2, hibudids T3"
                : isPreview ? "ibtrips T1, iblegs T2" : "hibtrips T1, hiblegs T2";

            return fromClause;
        }

        public string GetKeyWhereClause(string udidNumber)
        {
            int udid;
            var goodUdid = int.TryParse(udidNumber, out udid);

            return goodUdid && udid != 0
                ? "T1.reckey = T2.reckey and T1.reckey = T3.reckey and  airline not in ('$$','ZZ') and airline is not null and "
                : "T1.reckey = T2.reckey and airline not in ('$$','ZZ') and airline is not null and ";
        }

        public string GetWherelause(string whereClause, string udidNumber)
        {         
            return GetKeyWhereClause(udidNumber) + whereClause;
        }

        public string GetFieldList(bool isPreview)
        {
            return isPreview
                ? "T1.reckey, airchg, faretax, basefare, space(6) as orgdestemp, bktool,  convert(int, 1) as plusmin, 00.00 as NumTicks, 00.00 as onlineTkts, " +
                    "00.00 as agentTkts, 00000000 as RecordNo, exchange "
                : "T1.reckey, airchg, faretax, basefare, space(6) as orgdestemp, bktool, convert(int,plusmin) as Plusmin, 00.00 as NumTicks, 00.00 as OnlineTkts, " +
                    "00.00 as agentTkts, 00000000 as RecordNo, exchange ";
        }

        public string GetSpecialList(bool isCityPairMetro)
        {
            return "Origin, Destinat, ClassCat, Airline, Mode, ActFare, 0000000.0 as AirCO2, convert(int, miles) as Miles, " +
                "convert(int, SeqNo) as SeqNo, Ditcode, Connect ";
        }
    }
}
