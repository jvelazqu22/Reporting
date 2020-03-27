using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.TopAccountAir
{
    public static class SqlBuilder
    {
        // Builds sql script
        public static SqlScript GetSql(bool hasUdid, bool isPreview, string whereClause)
        {
            var script = new SqlScript();
            if (hasUdid)
            {
                script.FromClause = isPreview
                    ? "ibtrips T1, ibudids T3"
                    : "hibtrips T1, hibudids T3";
                script.KeyWhereClause = "T1.reckey = T3.reckey and acct is not null and valcarr not in ('ZZ','$$') and ";
                script.WhereClause = script.KeyWhereClause + whereClause;
            }
            else
            {
                script.FromClause = isPreview
                   ? "ibtrips T1"
                   : "hibtrips T1";
                script.KeyWhereClause = "acct is not null and valcarr not in ('ZZ','$$') and ";
                script.WhereClause = script.KeyWhereClause + whereClause;
            }

            script.FieldList = isPreview
                 ? "T1.reckey, T1.SourceAbbr, invdate, bookdate, Acct, AirChg, OffrdChg, 0.00 as SvcFee, 0.00 as aCommisn, convert(int,1) as Plusmin "
                 : "T1.SourceAbbr, Acct, AirChg, invdate, bookdate, OffrdChg, aCommisn, SvcFee, convert(int,Plusmin) as Plusmin ";

            return script;

        }

        // Builds sql for leg data
        public static SqlScript GetSqlWithLegs(bool hasUdid, bool isPreview, string whereClause)
        {
            var script = new SqlScript();
            if (hasUdid)
            {
                script.FromClause = isPreview
                    ? "ibtrips T1, iblegs T2, ibudids T3"
                    : "hibtrips T1, hiblegs T2, hibudids T3";
                script.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and acct is not null and valcarr not in ('ZZ','$$') and ";
                script.WhereClause = script.KeyWhereClause + whereClause;
            }
            else
            {
                script.FromClause = isPreview
                   ? "ibtrips T1, iblegs T2"
                   : "hibtrips T1, hiblegs T2";
                script.KeyWhereClause = "T1.reckey = T2.reckey and acct is not null and valcarr not in ('ZZ','$$') and ";
                script.WhereClause = script.KeyWhereClause + whereClause;
            }

            script.FieldList = isPreview
                 ? "T1.reckey, T1.recloc, T1.SourceAbbr, passlast, passfrst, invoice, acct, airchg, OffrdChg, 0.00 as svcfee, 0.00 as acommisn, convert(int,1) as plusmin, origin, destinat, mode, connect, rdepdate, airline, fltno, space(15) as tktdesig, class, deptime, rarrdate, arrtime,convert(int,seqno) as seqno, convert(int,miles) as miles, actfare, miscamt, farebase, segstatus "
                 : "T1.reckey, T1.recloc, T1.SourceAbbr, passlast, passfrst, invoice, acct, airchg, OffrdChg, acommisn, svcfee,convert(int,Plusmin) as plusmin, origin, mode, connect, rdepdate, airline, fltno, destinat, tktdesig, class, deptime, rarrdate, arrtime,convert(int,seqno) as seqno,convert(int,miles) as miles, actfare, miscamt, farebase, ' ' as segstatus ";
            script.OrderBy = "order by T1.reckey, T2.seqno ";

            return script;

        }

        // Builds sql script for getting Service fees
        public static SqlScript GetSqlSvcFees(bool hasUdid, string whereClause)
        {
            var script = new SqlScript();
            if (hasUdid)
            {
                script.FromClause = "hibtrips T1, hibServices T6A, hibudids T3";
                script.KeyWhereClause = "T1.reckey = T6A.reckey and T1.agency = T6A.agency and T1.reckey = T3.reckey and T1.agency = T3.agency and T6A.svcCode = 'TSF' and ";
                script.WhereClause = script.KeyWhereClause + whereClause.Replace("T1.trantype", "T6A.sfTranType");
            }
            else
            {
                script.FromClause = "hibTrips T1, hibServices T6A ";
                script.KeyWhereClause = "T1.reckey = T6A.reckey and T1.agency = T6A.agency and T6A.svcCode = 'TSF' and ";
                script.WhereClause = script.KeyWhereClause + whereClause.Replace("T1.trantype", "T6A.sfTranType") + " and origvalcar != 'SVCFEEONLY' and left(origValCar,3) != 'ZZ:'";
            }

            script.FieldList = "T1.RecKey, SourceAbbr, Acct, sum(T6A.svcAmt) as svcAmt ";

            script.GroupBy = "group by SourceAbbr, Acct, T1.RecKey, T1.MoneyType, T6A.MoneyType, BookDate, InvDate";

            return script;

        }
    }
}
