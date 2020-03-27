using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Models;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.DocumentDeliveryLog
{
    public static class SqlBuilder
    {
        private static readonly WhereClauseWithAdvanceParamsHandler _whereClauseWithAdvanceParamsHandler = new WhereClauseWithAdvanceParamsHandler();

        public static SqlScript GetSql(BuildWhere buildWhere, double offset, ReportGlobals globals)
        {
            var script = new SqlScript();

            script.FromClause = "ibtrips T1, ibTravAuth T7, ibTravAuthLog T8, docStatusLog T9 ";
            script.KeyWhereClause = "T1.reckey = T7.reckey and T1.agency = T7.agency and T7.TravAuthNo = T8.TravAuthNo and T8.recordno = T9.travauthlognbr " ;
             script.FieldList = "T1.reckey, T1.acct, T1.depdate, T1.recloc, T1.passlast, T1.passfrst, " + 
                "T7.bookedgmt, T7.TravAuthNo, T7.rtvlcode, T7.OutPolCods, T7.sgroupnbr, " + 
                "T8.AuthStatus, T8.statustime, T7.gds, T9.travAuthLogNbr as authlognbr, " + 
                "T9.docStatusNbr as statusnbr, T9.docStatustime as docStatTim, T9.docSuccess, " + 
                "T9.docType, T9.docRecips, T9.docSubject, T9.docText, T9.docHtml, T9.dlvrResponse as dlvRespons";

            var begHour = globals.GetParmValue(WhereCriteria.BEGHOUR).TryIntParse(-1);
            var begMin = globals.GetParmValue(WhereCriteria.BEGMINUTE).TryIntParse(-1);
            var isBegPm = globals.ParmValueEquals(WhereCriteria.BEGAMPM, "2");
            begHour = begHour.IsBetween(0, 12) ? begHour : 0;
            begMin = begMin.IsBetween(0, 59) ? begMin : 0;

            var endHour = globals.GetParmValue(WhereCriteria.ENDHOUR).TryIntParse(-1);
            var endMin = globals.GetParmValue(WhereCriteria.ENDHOUR).TryIntParse(-1);
            var isEndPm = globals.ParmValueEquals(WhereCriteria.ENDAMPM, "2");
            endHour = endHour.IsBetween(0, 12) ? endHour : 0;
            endMin = endMin.IsBetween(0, 59) ? endMin : 0;

            var begTime = globals.BeginDate.Value.Date.AddHours(begHour + (isBegPm?12:0) + offset).AddMinutes(begMin);
            var endTime = globals.EndDate.Value.Date.AddHours(endHour + (isEndPm ? 12 : 0) + offset).AddMinutes(endMin);

            script.KeyWhereClause += " and T8.statustime between '" + begTime.ToShortDateString() + " " + begTime.ToShortTimeString() + "' and '" + endTime.ToShortDateString() + " " + endTime.ToShortTimeString() + "'";

            if (globals.IsParmValueOn(WhereCriteria.CBONLYMSGSWITHERRS))
            {
                script.KeyWhereClause += " and T9.docsuccess = '0' ";
            }

            var authReqNumber = globals.GetParmValue(WhereCriteria.TXTAUTHREQNBR).TryIntParse(-1);
            if (authReqNumber > 0)
            {
                script.KeyWhereClause += " and T7.travauthno = " + authReqNumber;
                globals.WhereText += "; Auth. Request Number: " + authReqNumber;
            }

            script.KeyWhereClause += " and ";
            script.WhereClause = script.KeyWhereClause + buildWhere.WhereClauseFull.Replace("bookdate", "bookedgmt");

            script.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, false);
            return script;
            
        }
    }
}
