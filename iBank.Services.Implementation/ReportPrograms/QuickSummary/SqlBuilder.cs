using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.QuickSummary
{
    public static class SqlBuilder
    {
        public static SqlScript BuildTripSql(bool isPreview, bool hasUdid, string whereClause)
        {
            var script = new SqlScript();

            if (isPreview)
            {
                script.FieldList = "T1.RecKey, convert(int,1) as plusmin, domintl, reascode, savingcode, valcarMode, mktfare, airchg, offrdchg, stndchg, offrdchg as savings, offrdchg as negosvngs, offrdchg as lostamt ";
                if (hasUdid)
                {
                    script.FromClause = "ibtrips T1, ibudids T3";
                    script.WhereClause = "T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and " + whereClause;
                    script.KeyWhereClause = "T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and ";
                }
                else
                {
                    script.FromClause = "ibtrips T1";
                    script.WhereClause = "valcarr not in ('ZZ','$$') and " + whereClause;
                    script.KeyWhereClause = "valcarr not in ('ZZ','$$') and ";
                }
            }
            else
            {
                script.FieldList = "T1.RecKey, convert(int,plusmin) as plusmin, domintl, reascode, savingcode, mktfare, airchg, offrdchg, stndchg, offrdchg as savings, valcarMode, offrdchg as negosvngs, offrdchg as lostamt ";
                if (hasUdid)
                {
                    script.FromClause = "hibtrips T1, hibudids T3";
                    script.WhereClause = "T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and " + whereClause;
                    script.KeyWhereClause = "T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and ";
                }
                else
                {
                    script.FromClause = "hibtrips T1";
                    script.WhereClause = "valcarr not in ('ZZ','$$') and " + whereClause;
                    script.KeyWhereClause = "valcarr not in ('ZZ','$$') and ";
                }
            }

            return script;
            
        }

        public static SqlScript BuildCarSql(bool isPreview, bool hasUdid, string whereClause)
        {
            var script = new SqlScript();

            if (isPreview)
            {
                script.FieldList = "T1.RecKey, convert(int,1) as cplusmin, domintl,convert(int,days) as days, abookrat, reascoda, aexcprat";
                if (hasUdid)
                {
                    script.FromClause = "ibtrips T1, ibudids T3, ibcar T4";
                    script.WhereClause = "T1.reckey = T3.reckey and T1.reckey = T4.reckey and " + whereClause;
                    script.KeyWhereClause = "T1.reckey = T3.reckey and T1.reckey = T4.reckey and ";
                }
                else
                {
                    script.FromClause = "ibtrips T1, ibcar T4";
                    script.WhereClause = "T1.reckey = T4.reckey and " + whereClause;
                    script.KeyWhereClause = "T1.reckey = T4.reckey and ";
                }
            }
            else
            {
                script.FieldList = "T1.RecKey,convert(int,cplusmin) as cplusmin, domintl,convert(int,days) as days, abookrat, reascoda, aexcprat";
                if (hasUdid)
                {
                    script.FromClause = "hibtrips T1, hibudids T3, hibcars T4";
                    script.WhereClause = "T1.reckey = T3.reckey and T1.reckey = T4.reckey and " + whereClause.Replace("T1.trantype", "T4.CarTranTyp");
                    script.KeyWhereClause = "T1.reckey = T3.reckey and T1.reckey = T4.reckey and ";
                }
                else
                {
                    script.FromClause = "hibtrips T1, hibcars T4";
                    script.WhereClause = "T1.reckey = T4.reckey and " + whereClause.Replace("T1.trantype", "T4.CarTranTyp");
                    script.KeyWhereClause = "T1.reckey = T4.reckey and ";
                }
            }

            return script;

        }

        public static SqlScript BuildHotelSql(bool isPreview, bool hasUdid, string whereClause)
        {
            var script = new SqlScript();

            if (isPreview)
            {
                script.FieldList = "T1.RecKey, convert(int,1) as hplusmin,domintl,convert(int,nights) as nights,convert(int,rooms) as rooms,bookrate,reascodh,hexcprat";
                if (hasUdid)
                {
                    script.FromClause = "ibtrips T1, ibudids T3, ibhotel T5";
                    script.WhereClause = "T1.reckey = T3.reckey and T1.reckey = T5.reckey and " + whereClause;
                    script.KeyWhereClause = "T1.reckey = T3.reckey and T1.reckey = T5.reckey and ";
                }
                else
                {
                    script.FromClause = "ibtrips T1, ibhotel T5";
                    script.WhereClause = "T1.reckey = T5.reckey and " + whereClause;
                    script.KeyWhereClause = "T1.reckey = T5.reckey and ";
                }
            }
            else
            {
                script.FieldList = "T1.RecKey,convert(int,hplusmin) as hplusmin,domintl,convert(int,nights) as nights,convert(int,rooms) as rooms,bookrate,reascodh,hexcprat";
                if (hasUdid)
                {
                    script.FromClause = "hibtrips T1, hibudids T3, hibhotel T5";
                    script.WhereClause = "T1.reckey = T3.reckey and T1.reckey = T5.reckey and " + whereClause.Replace("T1.trantype", "T5.HotTranTyp");
                    script.KeyWhereClause = "T1.reckey = T3.reckey and T1.reckey = T5.reckey and ";
                }
                else
                {
                    script.FromClause = "hibtrips T1, hibhotel T5";
                    script.WhereClause = "T1.reckey = T5.reckey and " + whereClause.Replace("T1.trantype", "T5.HotTranTyp");
                    script.KeyWhereClause = "T1.reckey = T5.reckey and ";
                }
            }

            return script;

        }

    }
}
