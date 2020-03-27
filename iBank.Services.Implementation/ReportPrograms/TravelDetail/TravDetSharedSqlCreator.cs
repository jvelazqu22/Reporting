using System;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using Domain.Models;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.TravelDetail
{
    public class TravDetSharedSqlCreator
    {
        private readonly WhereClauseWithAdvanceParamsHandler _whereClauseWithAdvanceParamsHandler = new WhereClauseWithAdvanceParamsHandler();
        private Object _thisLock = new Object();
        private readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public SqlScript GetTripDataSql(BuildWhere buildWhere, bool udidExists, bool isReservationReport)
        {
            var sql = new SqlScript();

            //do not join legs when retrieve trip data
            if (udidExists)
            {
                if (isReservationReport)
                {


                    sql.FromClause = "ibTrips T1, ibudids T3";
                    sql.KeyWhereClause = "T1.reckey = T3.reckey and ";
                }
                else
                {
                    sql.FromClause = "hibTrips T1, hibudids T3";
                    sql.KeyWhereClause = "T1.reckey = T3.reckey and ";
                }

                sql.FieldList = @"T1.RecLoc, T1.reckey, invoice, ticket, acct, break1, break2, break3, passlast, passfrst, valcarr, airchg, " +
                                "offrdchg, stndchg, invdate,depdate, arrDate, TripStart, TripEnd, reascode, SourceAbbr, exchange, origticket, udidtext ";
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibTrips T1" : "hibTrips T1";
                sql.FieldList = @"T1.RecLoc, T1.reckey, invoice, ticket, acct, break1, break2, break3, passlast, passfrst, valcarr, airchg, " +
                                "offrdchg, stndchg, invdate,depdate, arrDate, TripStart, TripEnd, reascode, SourceAbbr, exchange, origticket ";
            }

            sql.WhereClause = sql.KeyWhereClause + buildWhere.WhereClauseFull;
            sql.FieldList += isReservationReport 
                ? ", 'I' as TranType, convert(int, 1) as plusmin, 0.00 as SvcFee, 'A' as ValCarMode "
                : ", convert(int,plusmin) as plusmin, SvcFee, ValCarMode ";
            sql.OrderBy = "order by T1.reckey";

            lock (_thisLock)
            {
                sql.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildWhere, true);
            }
            return sql;
        }

        public SqlScript GetCarSql(BuildWhere buildwhere, bool udidExists, bool isReservationReport)
        {
            var sql = new SqlScript();

            //do not join legs when retrieve car data
            if (udidExists)
            {
                sql.FromClause = isReservationReport ? "ibTrips T1, ibcar T4, ibudids T3" : "hibTrips T1, hibcars T4, hibudids T3";
                sql.KeyWhereClause = "T1.reckey = T4.reckey and T1.reckey = T3.reckey and ";
                sql.FieldList = @"abookrat, autocity, autostat, cartype, cityCode, company, ConfirmNo, DateBack, "
                                + "convert(int, days) as days, milecost, ratetype, reascoda, rentdate, T1.reckey, udidtext";
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibTrips T1, ibcar T4" : "hibTrips T1, hibcars T4";
                sql.KeyWhereClause = "T1.reckey = T4.reckey and ";
                sql.FieldList = @"abookrat, autocity, autostat, cartype, cityCode, company, ConfirmNo, DateBack, " +
                                "convert(int, days) as days, milecost, ratetype, reascoda, rentdate, T1.reckey";
            }
            sql.WhereClause = sql.KeyWhereClause + buildwhere.WhereClauseFull.Replace("T1.trantype", "T4.CarTranTyp");

            sql.FieldList += isReservationReport ? ", 'I' as CarTranTyp, convert(int,1) as cplusmin" : ", CarTranTyp, convert(int, cplusmin) as cplusmin";
            //trip fields
            sql.FieldList += ",acct, break1, break2, break3, T1.recloc,passfrst,passlast, tripstart, tripend, arrdate, depdate,sourceabbr";
            sql.OrderBy = "order by rentdate";

            lock (_thisLock)
            {
                sql.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildwhere, false);
            }
            return sql;
        }

        public SqlScript GetLegSql(BuildWhere buildWhere, bool udidExists, bool isReservationReport)
        {
            var sql = new SqlScript();

            if(udidExists)
            {
                sql.FromClause = isReservationReport ? "ibTrips T1, iblegs T2, ibudids T3" : "hibTrips T1, hiblegs T2, hibudids T3";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and ";
                sql.FieldList = @"T1.Acct, Break1, Break2, Break3, class, exchange, invdate, passlast, passfrst, T1.reckey, SourceAbbr, TripStart, "
                                + "fltno, mode,convert(int,miles) as miles, convert(int,1) as  plusmin, DITCode, classCat, udidtext ";
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibTrips T1, iblegs T2" : "hibTrips T1, hiblegs T2";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and ";
                sql.FieldList = @"T1.Acct, Break1, Break2, Break3, class, exchange, invdate, passlast, passfrst, T1.reckey, SourceAbbr, TripStart, " 
                                + "fltno, mode,convert(int,miles) as miles, convert(int,1) as  plusmin, DITCode, classCat ";
            }

            sql.WhereClause = sql.KeyWhereClause + buildWhere.WhereClauseFull;
            sql.OrderBy = "order by T2.seqno";

            lock (_thisLock)
            {
                sql.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildWhere, true);
            }
            return sql;
        }

        public SqlScript GetHotelSql(BuildWhere buildWhere, bool udidExists, bool isReservationReport)
        {
            var sql = new SqlScript();

            //do not join legs when retrieve hotel data
            if (udidExists)
            {
                sql.FromClause = isReservationReport ? "ibTrips T1, ibhotel T5, ibudids T3" : "hibTrips T1, hibhotel T5, hibudids T3";
                sql.KeyWhereClause = "T1.reckey = T5.reckey and T1.reckey = T3.reckey and ";
                sql.FieldList = "bookrate, ConfirmNo, datein, DateOut, guarante, hotcity, hotelnam, hotphone, hotstate, Metro, "
                                + "convert(int, nights) as nights, reascodh, convert(int, rooms) as rooms, roomtype, T1.reckey, udidtext";
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibTrips T1, ibhotel T5" : "hibTrips T1, hibhotel T5";
                sql.KeyWhereClause = "T1.reckey = T5.reckey and ";
                sql.FieldList = "bookrate, ConfirmNo, datein, DateOut, guarante, hotcity, hotelnam, hotphone, hotstate, Metro, " 
                                + "convert(int, nights) as nights, reascodh, convert(int, rooms) as rooms, roomtype, T1.reckey";
            }

            sql.WhereClause = sql.KeyWhereClause + buildWhere.WhereClauseFull.Replace("T1.trantype", "T5.HotTranTyp");

            //trip fields
            sql.FieldList += ",acct, break1, break2, break3,T1.recloc,passfrst,passlast, tripstart, tripend, arrdate, depdate, sourceabbr";

            sql.FieldList += isReservationReport ? ", 'I' as HotTranTyp, convert(int,1) as hplusmin" : ", HotTranTyp, convert(int, hplusmin) as hplusmin";

            sql.OrderBy = "order by T5.seqno";            
            lock (_thisLock)
            {
                sql.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildWhere, false);
            }
            return sql;
        }

        public SqlScript GetServiceFeeFromSvcFeeTableSql(BuildWhere buildWhere, bool udidExists, bool isReservationReport)
        {
            var sql = new SqlScript();

            //do not join legs when retrieve svcfee data
            if (udidExists)
            {
                if (isReservationReport)
                {
                    sql.FromClause = "ibTrips T1, ibSvcFees T6, ibudids T3";
                    sql.KeyWhereClause = "T1.Agency = T6.Agency and " +
                                         "T6.recloc is not null and T6.recloc <> ' ' and T1.recloc = T6.recloc and " +
                                         "T1.Acct = T6.Acct and " +
                                         "T1.Invoice = T6.Invoice and " +
                                         "T1.PassLast = T6.PassLast and " +
                                         "T1.PassFrst = T6.PassFrst and " +
                                         "T1.Reckey = T3.reckey and T1.agency = T3.agency and ";
                }
                else
                {
                    sql.FromClause = "hibTrips T1, hibSvcFees T6, hibudids T3";
                    sql.KeyWhereClause = "T1.Agency = T6.Agency and " +
                                         "T6.recloc is not null and T6.recloc <> ' ' and T1.recloc = T6.recloc and " +
                                         "T1.Acct = T6.Acct and " +
                                         "T1.Invoice = T6.Invoice and " +
                                         "T1.PassLast = T6.PassLast and " +
                                         "T1.PassFrst = T6.PassFrst and " +
                                         "T1.Reckey = T3.reckey and T1.agency = T3.agency and ";
                }
            }
            else
            {
                if (isReservationReport)
                {
                    sql.FromClause = "ibTrips T1, ibSvcFees T6";
                    sql.KeyWhereClause = "T1.Agency = T6.Agency and " +
                                         "T6.recloc is not null and T6.recloc <> ' ' and T1.recloc = T6.recloc and " +
                                         "T1.Acct = T6.Acct and " +
                                         "T1.Invoice = T6.Invoice and " +
                                         "T1.PassLast = T6.PassLast and " +
                                         "T1.PassFrst = T6.PassFrst and ";
                }
                else
                {
                    sql.FromClause = "hibTrips T1, hibSvcFees T6";
                    sql.KeyWhereClause = "T1.Agency = T6.Agency and " +
                                         "T6.recloc is not null and T6.recloc <> ' ' and T1.recloc = T6.recloc and " +
                                         "T1.Acct = T6.Acct and " +
                                         "T1.Invoice = T6.Invoice and " +
                                         "T1.PassLast = T6.PassLast and " +
                                         "T1.PassFrst = T6.PassFrst and ";
                }
            }

            sql.WhereClause = sql.KeyWhereClause + buildWhere.WhereClauseFull.Replace("T1.trantype", "T6.TranType");
            sql.FieldList = "T1.RecKey, T1.RecLoc, T1.Invoice, T6.Acct, T6.PassLast, T6.PassFrst, T6.SvcFee, T6.MoneyType as AirCurrTyp, T6.TranDate ";
            sql.OrderBy = "";

            lock (_thisLock)
            {
                sql.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildWhere, false);
            }
            return sql;
        }

        public SqlScript GetServiceFeesFromHibServices(BuildWhere buildWhere, bool udidExists)
        {
            var sql = new SqlScript();

            if(udidExists)
            {
                sql.FromClause = "hibtrips T1, hibservices T6A, hibudids T3";
                sql.KeyWhereClause = "T1.reckey = T6A.reckey and T1.agency = T6A.agency and T6A.svccode = 'TSF' and T1.reckey = T3.reckey and T1.agency = T3.agency and ";
            }
            else
            {
                sql.FromClause = "hibtrips T1, hibservices T6A";
                sql.KeyWhereClause = "T1.reckey = T6A.reckey and T1.agency = T6A.agency and T6A.svccode = 'TSF' and ";
            }

            sql.WhereClause = sql.KeyWhereClause + buildWhere.WhereClauseFull.Replace("T1.trantype", "T6A.sfTranType");
            sql.FieldList = "T1.reckey, T6A.svcAmt as svcfee ";
            sql.OrderBy = "";

            lock (_thisLock)
            {
                sql.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildWhere, false);
            }

            return sql;
        }

        public SqlScript GetLegSqlToCollapse(BuildWhere buildWhere, bool udidExists, bool isReservationReport)
        {
            var sql = new SqlScript();

            if(udidExists)
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2, ibudids T3" : "hibtrips T1, hiblegs T2, hibudids T3";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and ";
                sql.FieldList = "T1.reckey, T1.passlast, T1.passfrst, T1.acct, T1.Break1,T1.Break2, T1.Break3, T1.recloc, '   ' as segstatus, "
                                + "T1.TripStart, T1.SourceAbbr, T1.InvDate, T3.udidtext ";
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2" : "hibtrips T1, hiblegs T2";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and ";
                sql.FieldList = "T1.reckey, T1.passlast, T1.passfrst, T1.acct, T1.Break1,T1.Break2, T1.Break3, T1.recloc, '   ' as segstatus, "
                                +"T1.TripStart, T1.SourceAbbr, T1.InvDate ";
            }

            sql.WhereClause = sql.KeyWhereClause + buildWhere.WhereClauseFull;

            sql.OrderBy = "Order by T2.seqno";
            lock (_thisLock)
            {
                sql.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildWhere, true);
            }

            return sql;
        }
    }
}
