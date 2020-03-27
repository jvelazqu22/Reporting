using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Models;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.BroadcastDetails
{
    public static class SqlBuilder
    {
        public static SqlScript CreateScript(ReportGlobals globals)
        {
            var script = new SqlScript();
            var rptUserNumber = globals.GetParmValue(WhereCriteria.USERNMBR).TryIntParse(-1);
            if (rptUserNumber < 1) rptUserNumber = -1;

            script.WhereClause = rptUserNumber == -1
                ? " and t1.agency = '" + globals.Agency + "'"
                : " and t1.agency = '" + globals.Agency + "' and t1.usernumber = " + rptUserNumber;

            var batchNum = globals.GetParmValue(WhereCriteria.DDGENERIC2);
            if (!string.IsNullOrEmpty(batchNum)) script.WhereClause += " and t3.batchnum = " + batchNum;

            script.WhereClause = "t3.batchnum = t4.batchnum and t3.usernumber = t1.usernumber and (batchname not like 'sysDR:%')" + script.WhereClause;

            switch (globals.GetParmValue(WhereCriteria.DDLASTRPTSUCCESS))
            {
                case "YES":
                    script.WhereClause += " and t3.errflag = 0";
                    break;
                case "NO":
                    script.WhereClause += " and t3.errflag = 1";
                    break;
            }

            script.FieldList =
                "t3.UserNumber,t3.agency,t3.batchnum,t3.batchname,t3.emailaddr ,t3.acctlist ,t3.prevhist ,t3.weekmonth ,t3.monthstart ,t3.monthrun ," +
                "t3.weekstart ,t3.weekrun ,t3.nxtdstart ,t3.nxtdend ,t3.lastrun ,t3.lastdstart ,t3.lastdend ,t3.errflag ,t3.runspcl,t3.spclstart ,t3.spclend ," +
                "t3.pagebrklvl ,t3.titleacct ,t3.bcsenderemail ,t3.bcsendername ,t3.nextrun ,t3.setby ,t3.holdrun ,t3.reportdays ,t3.rptusernum ,t3.usespcl ," +
                "t3.nodataoptn ,t3.emailsubj ,t3.mailformat ,t3.outputtype ,t3.displayuid ,t3.outputdest ,t3.eProfileNo ,t3.emailccadr ,t3.LangCode ," +
                "convert(int,t3.RunNewData) as RunNewData ,t3.fystartmo ,t3.timezone ,t3.gmtdiff ,t3.unilangcode ,t3.send_error_email, t4.savedrptnum, t4.udrkey, t4.processkey, " +
                "t4.datetype, t1.lastname, t1.firstname";
            script.FromClause = "ibuser t1, ibbatch t3, ibbatch2 t4 ";
            script.OrderBy = " order by batchname";

            return script;
        }

        public static string GetSql(ReportGlobals globals)
        {
            var rptUserNumber = globals.GetParmValue(WhereCriteria.USERNMBR).TryIntParse(-1);
            if (rptUserNumber < 1) rptUserNumber = -1;

            var whereClause = rptUserNumber == -1
                ? " and t1.agency = '" + globals.Agency + "'"
                : " and t1.agency = '" + globals.Agency + "' and t1.usernumber = " + rptUserNumber;

            var batchNum = globals.GetParmValue(WhereCriteria.DDGENERIC2);
            if (!string.IsNullOrEmpty(batchNum)) whereClause += " and t3.batchnum = " + batchNum;

            whereClause = "t3.batchnum = t4.batchnum and t3.usernumber = t1.usernumber and (batchname not like 'sysDR:%')" + whereClause;

            switch (globals.GetParmValue(WhereCriteria.DDLASTRPTSUCCESS))
            {
                case "YES":
                    whereClause += " and t3.errflag = 0";
                    break;
                case "NO":
                    whereClause += " and t3.errflag = 1";
                    break;
            }

            return
                "select t3.UserNumber,t3.agency,t3.batchnum,t3.batchname,t3.emailaddr ,t3.acctlist ,t3.prevhist ,t3.weekmonth ,t3.monthstart ,t3.monthrun ," +
                "t3.weekstart ,t3.weekrun ,t3.nxtdstart ,t3.nxtdend ,t3.lastrun ,t3.lastdstart ,t3.lastdend ,t3.errflag ,t3.runspcl,t3.spclstart ,t3.spclend ," +
                "t3.pagebrklvl ,t3.titleacct ,t3.bcsenderemail ,t3.bcsendername ,t3.nextrun ,t3.setby ,t3.holdrun ,t3.reportdays ,t3.rptusernum ,t3.usespcl ," +
                "t3.nodataoptn ,t3.emailsubj ,t3.mailformat ,t3.outputtype ,t3.displayuid ,t3.outputdest ,t3.eProfileNo ,t3.emailccadr ,t3.LangCode ," +
                "convert(int,t3.RunNewData) as RunNewData ,t3.fystartmo ,t3.timezone ,t3.gmtdiff ,t3.unilangcode ,t3.send_error_email, t4.savedrptnum, t4.udrkey, t4.processkey, " +
                "t4.datetype, t1.lastname, t1.firstname from ibuser t1, ibbatch t3, ibbatch2 t4 where " +
                whereClause + " order by batchname";
        }
    }
}
