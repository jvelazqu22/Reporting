using Domain.Helper;
using Domain.Models;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.BroadcastStatus
{
    public static class SqlBuilder
    {
        public static SqlScript CreateScript(ReportGlobals globals)
        {
            var script = new SqlScript
            {
                FromClause = "bcreportlog t1, ibuser t2, ibbatch t3",
                WhereClause = "t1.batchnum = t3.batchnum and t1.usernumber = t2.usernumber " +
                              "and t1.agency = '" + globals.Agency + "' " +
                              "and t1.rundatetime between '" + globals.BeginDate.Value.ToShortDateString() +
                              "' and '" +
                              globals.EndDate.Value.ToShortDateString() + " 11:59:59 PM'"
            };

            if (globals.User.AdminLevel != 1) script.WhereClause += " and T2.OrgKey = " + globals.User.OrganizationKey;

            if (globals.IsParmValueOn(WhereCriteria.CBONLYBCASTERRORS)) script.WhereClause += "  and (runokay = '0' or t3.errflag = '1' or emaillog like '%unable to send%')";

            if (globals.IsParmValueOn(WhereCriteria.CBEXCLOFFLINERPTS)) script.WhereClause += " and (batchname not like 'sysDR:%')";

            script.FieldList = "t1.rundatetime, t1.runokay, t1.agency, t1.usernumber, " +
                               "t1.batchnum, t1.startdate, t1.enddate, t1.emailaddr, t1.emailmsg, " +
                               "t1.emaillog, t3.bcsenderemail, t3.bcsendername, t2.lastname, " +
                               "t2.firstname, t3.batchname";
            script.OrderBy = "order by rundatetime";

            return script;
        }

        public static string GetSql(ReportGlobals globals)
        {
            var script = new SqlScript
            {
                FromClause = "bcreportlog t1, ibuser t2, ibbatch t3",
                WhereClause = "t1.batchnum = t3.batchnum and t1.usernumber = t2.usernumber " +
                              "and t1.agency = '" + globals.Agency + "' " +
                              "and t1.rundatetime between '" + globals.BeginDate.Value.ToShortDateString() +
                              "' and '" +
                              globals.EndDate.Value.ToShortDateString() + " 11:59:59 PM'"
            };

            if (globals.User.AdminLevel != 1) script.WhereClause += " and T2.OrgKey = " + globals.User.OrganizationKey;

            if (globals.IsParmValueOn(WhereCriteria.CBONLYBCASTERRORS)) script.WhereClause += "  and (runokay = '0' or t3.errflag = '1' or emaillog like '%unable to send%')";

            if (globals.IsParmValueOn(WhereCriteria.CBEXCLOFFLINERPTS)) script.WhereClause += " and (batchname not like 'sysDR:%')";

            script.FieldList = "t1.rundatetime, t1.runokay, t1.agency, t1.usernumber, " + 
                "t1.batchnum, t1.startdate, t1.enddate, t1.emailaddr, t1.emailmsg, " + 
                "t1.emaillog, t3.bcsenderemail, t3.bcsendername, t2.lastname, " + 
                "t2.firstname, t3.batchname";
            script.OrderBy = "order by rundatetime";

            return $"select {script.FieldList} from {script.FromClause} where {script.WhereClause} {script.OrderBy}";
        }
    }
}
