using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Domain.Helper;
using Domain.Models;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.PTARequestActivity
{
    public class SqlBuilder
    {
        private readonly WhereClauseWithAdvanceParamsHandler _whereClauseWithAdvanceParamsHandler = new WhereClauseWithAdvanceParamsHandler();

        public SqlScript GetSql(bool hasUdid, BuildWhere buildWhere, DateTime? startDate, DateTime? endDate, ReportGlobals globals)
        {
            var script = new SqlScript();

            if (hasUdid)
            {
                script.FromClause = "ibtrips T1, iblegs T2, ibudids T3, ibTravAuth T7, ibTravAuthorizers T8 ";
                script.WhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and T7.TravAuthNo = T8.TravAuthNo and " + buildWhere.WhereClauseFull;
                script.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and T7.TravAuthNo = T8.TravAuthNo and ";
            }
            else
            {
                script.FromClause = "ibtrips T1, iblegs T2, ibTravAuth T7, ibTravAuthorizers T8 ";
                script.WhereClause = "T1.reckey = T2.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and T7.TravAuthNo = T8.TravAuthNo and " + buildWhere.WhereClauseFull;
                script.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and T7.TravAuthNo = T8.TravAuthNo and ";
            }

            script.FieldList = @"T1.acct, T1.break1, T1.break2, T1.break3, T1.depdate, " + 
                "T7.bookedgmt, T7.statustime, T1.recloc, T1.passlast, T1.passfrst, " + 
                "T7.TravAuthNo, T7.rtvlcode, T7.OutPolCods, T8.AuthrzrNbr, " + 
                "T7.AuthStatus, T8.AuthStatus as DetlStatus, T8.ApvReason, " +
                "convert(int,T8.ApSequence) as ApSequence, T8.statustime as DetStatTim, T1.reckey, " + 
                "T7.sgroupnbr, T8.RecordNo, T8.Auth1Email, T1.AirChg, T1.OffrdChg, " + 
                "T7.CliAuthNbr, T2.deptime, convert(int,T2.seqno) seqno ";

            var excludes = new List<string>();
            if (!globals.IsParmValueOn(WhereCriteria.CBINCLAPPRTVL)) { excludes.Add("'A'");}
            if (!globals.IsParmValueOn(WhereCriteria.CBINCLDECLINEDTVL)) { excludes.Add("'D'"); }
            if (!globals.IsParmValueOn(WhereCriteria.CBINCLCANCTVL)) { excludes.Add("'C'"); }
            if (!globals.IsParmValueOn(WhereCriteria.CBINCLEXPREQS)) { excludes.Add("'E'"); }

            if (excludes.Any()) script.WhereClause += " and T7.authstatus not in (" + string.Join(",",excludes) + ")";

            if (Features.ExcludeParseStampWhereClause.IsEnabled())
            {
                if (startDate.Value.Year != Constants.ModifiedDateTimeMinValue.Year && endDate.Value.Year != Constants.ModifiedDateTimeMinValue.Year)
                {
                    script.WhereClause += " and T1.parseStamp between '" + GetDateString(startDate.Value) + "' and '" + GetDateString(endDate.Value) + "'";
                    globals.WhereText += "; ParseStamp between " + GetDateString(startDate.Value) + " and " + GetDateString(endDate.Value);
                }
                else if (startDate.Value.Year != Constants.ModifiedDateTimeMinValue.Year)
                {
                    script.WhereClause += " and T1.parseStamp >= '" + GetDateString(startDate.Value) + "'";
                    globals.WhereText += "; ParseStamp >= " + GetDateString(startDate.Value);
                }
                else if (endDate.Value.Year != Constants.ModifiedDateTimeMinValue.Year)
                {
                    script.WhereClause += " and T1.parseStamp <= '" + GetDateString(endDate.Value) + "'";
                    globals.WhereText += "; ParseStamp <= " + GetDateString(endDate.Value);
                }
            }
            script.OrderBy = "order by T1.reckey, seqno, ApSequence ";

            script.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, true);
            return script;
            
        }

        private string GetDateString(DateTime val)
        {
            return val.ToShortDateString() + " " + val.ToLongTimeString();
        }

        public string GetCancelledTripFromClause(int udidNumber)
        {
            return udidNumber > 0
                    ? "ibCancTrips T1, ibCancLegs T2, ibCancUdids T3, ibTravAuth T7, ibTravAuthorizers T8 "
                    : "ibCancTrips T1, ibCancLegs T2, ibTravAuth T7, ibTravAuthorizers T8 ";
        }
    }
}
