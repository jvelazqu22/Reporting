using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Models;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.PTATravelersDetail
{
    public class SqlBuilder
    {
        private readonly WhereClauseWithAdvanceParamsHandler _whereClauseWithAdvanceParamsHandler = new WhereClauseWithAdvanceParamsHandler();
        public SqlScript GetSql(bool hasUdid,BuildWhere buildWhere, DateTime? startDate, DateTime? endDate, ReportGlobals globals)
        {
            var script = new SqlScript();

            if (hasUdid)
            {
                script.FromClause = "ibtrips T1, iblegs T2, ibudids T3, ibTravAuth T7 ";
                script.WhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and T1.ValCarr != 'ZZ' and " + buildWhere.WhereClauseFull;
                script.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and T1.ValCarr != 'ZZ' and ";
            }
            else
            {
                script.FromClause = "ibtrips T1, iblegs T2, ibTravAuth T7 ";
                script.WhereClause = "T1.reckey = T2.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and T1.ValCarr != 'ZZ' and " + buildWhere.WhereClauseFull;
                script.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and T1.ValCarr != 'ZZ' and ";
            }

            script.FieldList = @"T1.acct, T1.break1, T1.break2, T1.break3, T1.depdate, " + 
            "T1.AirChg, T1.recloc, T1.passlast, T1.passfrst, T1.reckey, " + 
            "T1.Exchange, T1.PenaltyAmt, T1.TktOrgAmt, T1.AddCollAmt, " + 
            "T1.OffrdChg, T1.ReasCode, T7.TravAuthNo ";

            var excludes = new List<string>();
            if (!globals.IsParmValueOn(WhereCriteria.CBINCLAPPRTVL)) { excludes.Add("'A'");}
            if (!globals.IsParmValueOn(WhereCriteria.CBINCLDECLINEDTVL)) { excludes.Add("'D'"); }
            if (!globals.IsParmValueOn(WhereCriteria.CBINCLCANCTVL)) { excludes.Add("'C'"); }
            if (!globals.IsParmValueOn(WhereCriteria.CBINCLEXPREQS)) { excludes.Add("'E'"); }

            if (excludes.Any())
            {
                script.WhereClause += " and T7.authstatus not in (" + string.Join(",",excludes) + ")";
            }
            script.OrderBy = "order by T7.TravAuthNo, T1.reckey, seqno";

            script.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, true);
            return script;
            
        }

        public SqlScript GetSqlTripAuthorizer(bool hasUdid, bool summaryOnly, BuildWhere buildWhere,ReportGlobals globals)
        {
            var script = new SqlScript();

            if (summaryOnly)
            {
                if (hasUdid)
                {
                    script.FromClause = "ibtrips T1, ibudids T3, ibTravAuth T7 ";
                    script.WhereClause = "T1.reckey = T3.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and " + buildWhere.WhereClauseFull;
                    script.KeyWhereClause = "T1.reckey = T3.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and ";
                }
                else
                {
                    script.FromClause = "ibtrips T1, ibTravAuth T7 ";
                    script.WhereClause = "T1.reckey = T7.reckey and T1.agency = T7.agency and " + buildWhere.WhereClauseFull;
                    script.KeyWhereClause = "T1.reckey = T7.reckey and T1.agency = T7.agency and ";
                }
                script.FieldList = @"T1.acct, T1.break1, T1.break2, T1.break3, T1.depdate, " + 
                "T1.AirChg, T7.bookedgmt, T7.StatusTime, T1.recloc, T7.TravAuthNo, T7.rtvlcode, " + 
                "T7.OutPolCods, T7.AuthStatus, T1.reckey, T7.SGroupNbr, 1 as apSequence";

                script.OrderBy = "order by T1.reckey, T7.TravAuthNo";
            }
            else
            {
                if (hasUdid)
                {
                    script.FromClause = "ibtrips T1, ibudids T3, ibTravAuth T7, ibTravAuthorizers T8 ";
                    script.WhereClause = "T1.reckey = T3.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and T7.TravAuthNo = T8.TravAuthNo and " + buildWhere.WhereClauseFull;
                    script.KeyWhereClause = "T1.reckey = T3.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and T7.TravAuthNo = T8.TravAuthNo and ";
                }
                else
                {
                    script.FromClause = "ibtrips T1, ibTravAuth T7, ibTravAuthorizers T8 ";
                    script.WhereClause = "T1.reckey = T7.reckey and T1.agency = T7.agency and T7.TravAuthNo = T8.TravAuthNo and " + buildWhere.WhereClauseFull;
                    script.KeyWhereClause = "T1.reckey = T7.reckey and T1.agency = T7.agency and T7.TravAuthNo = T8.TravAuthNo and ";
                }
                script.FieldList = @"T1.acct, T1.break1, T1.break2, T1.break3, T1.depdate, " + 
                    "T1.AirChg, T7.bookedgmt, T7.StatusTime, T1.recloc, T1.passlast, " + 
                    "T1.passfrst, T7.TravAuthNo, T7.rtvlcode, T7.OutPolCods, T8.AuthrzrNbr, " + 
                    "T7.AuthStatus, T8.AuthStatus as DetlStatus, T8.ApvReason, " +
                    "convert(int,T8.ApSequence) as ApSequence, T8.statustime as DetStatTim, T1.reckey, " + 
                    "T7.SGroupNbr, T7.CliAuthNbr, T8.RecordNo, T8.Auth1Email, T7.authcomm ";

                script.OrderBy = "order by T1.reckey, T7.TravAuthNo, ApSequence ";
            }

            script.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, true);
            return script;

        }

        public SqlScript GetSqlCar(bool hasUdid,BuildWhere buildWhere, ReportGlobals globals)
        {
            var script = new SqlScript();

           
                if (hasUdid)
                {
                    script.FromClause = "ibtrips T1, ibCar T4, ibudids T3, ibTravAuth T7 ";
                    script.WhereClause = "T1.reckey = T4.reckey and T1.reckey = T3.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and " + buildWhere.WhereClauseFull;
                    script.KeyWhereClause = "T1.reckey = T4.reckey and T1.reckey = T3.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and ";
                }
                else
                {
                    script.FromClause = "ibtrips T1, ibCar T4, ibTravAuth T7 ";
                    script.WhereClause = "T1.reckey = T4.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and " + buildWhere.WhereClauseFull;
                    script.KeyWhereClause = "T1.reckey = T4.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and ";
                }
                script.FieldList = @"T1.acct, T1.break1, T1.break2, T1.break3, T1.recloc, " + 
                    "T1.passlast, T1.passfrst, T1.reckey, T4.Company, T4.RentDate, " + 
                    "T4.aBookRat, T4.Days, T4.AExcpRat, T4.ReasCodA, T7.TravAuthNo ";

                script.OrderBy = "order by T1.reckey, T7.TravAuthNo, RentDate ";

            script.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, true);
            return script;
        }

        public SqlScript GetSqlHotel(bool hasUdid, BuildWhere buildWhere, ReportGlobals globals)
        {
            var script = new SqlScript();


            if (hasUdid)
            {
                script.FromClause = "ibtrips T1, ibHotel T5, ibudids T3, ibTravAuth T7 ";
                script.WhereClause = "T1.reckey = T5.reckey and T1.reckey = T3.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and " + buildWhere.WhereClauseFull;
                script.KeyWhereClause = "T1.reckey = T5.reckey and T1.reckey = T3.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and ";
            }
            else
            {
                script.FromClause = "ibtrips T1, ibHotel T5, ibTravAuth T7 ";
                script.WhereClause = "T1.reckey = T5.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and " + buildWhere.WhereClauseFull;
                script.KeyWhereClause = "T1.reckey = T5.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and ";
            }
            script.FieldList = @"T1.acct, T1.break1, T1.break2, T1.break3, T1.recloc, " + 
                "T1.passlast, T1.passfrst, T1.reckey, T5.HotelNam, T5.HotCity, " + 
                "T5.Metro, T5.DateIn, T5.Rooms, T5.Nights, T5.BookRate, T5.HExcpRat, " + 
                "T5.ReasCodH, T7.TravAuthNo ";

            script.OrderBy = "order by T1.reckey, T7.TravAuthNo, DateIn ";

            script.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, true);
            return script;
        }

        public string GetCancelledTripFromClause(int udidNumber)
        {
            return udidNumber > 0
                ? "ibCancTrips T1, ibCancUdids T3, ibTravAuth T7, ibTravAuthorizers T8 "
                : "ibCancTrips T1, ibTravAuth T7, ibTravAuthorizers T8 ";
        }

        public string GetCancelledCarFromClause(int udidNumber)
        {
            return udidNumber > 0
                ? "ibCancTrips T1, ibCancCars T4, ibCancUdids T3, ibTravAuth T7 "
                : "ibCancTrips T1, ibCancCars T4, ibTravAuth T7 ";
        }

        public string GetCancelledHotelFromClause(int udidNumber)
        {
            return udidNumber > 0
                ? "ibCancTrips T1, ibCancHotels T5, ibCancUdids T3, ibTravAuth T7 "
                : "ibCancTrips T1, ibCancHotels T5, ibTravAuth T7 "; 
        }
    }
}
