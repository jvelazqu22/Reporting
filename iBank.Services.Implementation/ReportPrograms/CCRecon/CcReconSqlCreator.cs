using Domain.Models;
using iBank.Server.Utilities.Helpers;
using System;
using System.Collections.Generic;
using Domain.Helper;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.CCRecon
{
    public class CcReconSqlCreator
    {
        private readonly WhereClauseWithAdvanceParamsHandler _whereClauseWithAdvanceParamsHandler = new WhereClauseWithAdvanceParamsHandler();

        public SqlScript GetBreaksSql(string existingWhereClause)
        {
            var sql = new SqlScript();

            sql.FromClause = "hibTrips T1";
            sql.FieldList = "acct, break1, break2, break3, T1.reckey, invoice, invdate,passlast, passfrst, recloc";
            sql.WhereClause = existingWhereClause;
            sql.OrderBy = "";

            return sql;
        }

        public SqlScript GetTripCcAndLegsSql(BuildWhere buildWhere, string existingWhereClause)
        {
            var sql = new SqlScript();

            sql.FromClause = "hibTrips T1, hibLegs T2";
            sql.FieldList = "acct, break1, break2, break3, T1.reckey, cardnum, valcarr, ticket, trantype, invoice, invdate, invdate as trandate, passlast "
                            + "passfrst, airchg, origin, destinat, connect, convert(int,T2.seqno) as seqno, recloc, mode, origOrigin, origDest, depdate, arrdate";
            sql.KeyWhereClause = "T1.reckey = T2.reckey and ";
            sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            sql.OrderBy = "";

            sql.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildWhere, false);
            sql.WhereClause = sql.WhereClause.Replace("sfcardnum", "cardnum");
            return sql;
        }

        public SqlScript GetTripCcSql(BuildWhere buildWhere, string existingWhereClause)
        {
            var sql = new SqlScript();

            sql.FromClause = "hibTrips T1";
            sql.FieldList = "reckey, MoneyType, acct, break1, break2, break3, recloc, cardnum, valcarr, acct, invoice, ";
            sql.FieldList += " ticket, trantype, passlast, passfrst, airchg, depdate, arrdate";
            sql.WhereClause = existingWhereClause;
            sql.OrderBy = "";

            sql.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildWhere, false);
            sql.WhereClause = sql.WhereClause.Replace("sfcardnum", "cardnum");
            return sql;
        }

        public SqlScript GetServiceFeeSql(string existingWhereClause)
        {
            var sql = new SqlScript();

            sql.FromClause = "hibtrips T1, hibServices T6A";
            sql.KeyWhereClause = "T1.reckey = T6A.reckey and ";

            var tempWhere = "T1.agency = T6A.agency and svcAmt != 0 and sfCardnum is not null and sfCardnum <> ' ' and T6A.svcCode = 'TSF' and "
                            + existingWhereClause.Replace("T1.trantype", "T6A.sfTranType").Replace("T1.cardnum", "T6A.sfCardnum");
            sql.WhereClause = sql.KeyWhereClause + tempWhere;
            sql.FieldList = "T1.reckey, acct, break1, break2, break3, sfCardNum, mco, sfTrantype as trantype, trandate, invoice, passlast, passfrst, svcAmt as svcFee, svcDesc as Descript";

            return sql;
        }

        public SqlScript GetAirCreditCardSql(string whereTrip)
        {
            var sql = new SqlScript();

            sql.FromClause = "ccTrans T1, ccAir T2, ccLegs T3";
            sql.WhereClause = whereTrip + " and recType = 'AIR' and T1.RecLink = T2.RecLink and T2.RecLink = T3.RecLink ";
            sql.FieldList = "T1.RecLink as RecKey, 'A' as Mode, origin as OrigOrigin, destinat as OrigDest, CardType, CardNum, TransAmt, RefNbr, Ticket, " 
                            + "TranType, PassName, TranDate, Origin, Destinat, Connect, convert(int,seqno) as Seqno ";
            sql.OrderBy = "";

            return sql;
        }

        public SqlScript GetSvcFeeCreditCardSql(string whereTrip)
        {
            var sql = new SqlScript();

            sql.FromClause = "ccTrans T1, ccSvcFees T2";
            sql.WhereClause = whereTrip + " and recType = 'SVCFEE' and T1.RecLink = T2.RecLink ";
            sql.FieldList = "CardType, CardNum, TransAmt, RefNbr, MCO as Ticket, TranType, PassName, TranDate, MCO as Descript ";
            sql.OrderBy = "";

            return sql;
        }

        public SqlScript GetUdidSql(List<int> udidNumber, string whereClause3)
        {
            var sql = new SqlScript();
            var udidNoList = string.Join(",", udidNumber.Where(x => x > 0)); 

            sql.FromClause = "hibtrips T1, hibudids T3";
            sql.WhereClause = "T1.reckey = T3.reckey and udidno in (" + udidNoList + ") and " + whereClause3;
            sql.FieldList = "T1.reckey, convert(int,UdidNo) as UdidNbr, UdidText as UdidText, count(*) as dummy"; //no longer just one udid, it could be up to 10
            sql.OrderBy = "";

            return sql;
        }
        
        public SqlScript GetRawDataSql(BuildWhere buildWhere, string existingWhereClause, List<AdvancedParameter> parameters)
        {
            var sql = new SqlScript();

            //this fix blows up the retrieval of service fees which happens next
            //TODO: remove hibServices when Advanced Parameter global fix is applied
            //hibServices is added solely so that if there is an advanced parameter of "Service Code" the SQL won't bomb
            //devs were told to fix this on a defect by defect, report by report basis, and that a global fix would be made in the future
            //the date of this comment is 9/22/2017 - let's see how long the global fix takes
            // 12/20/2017 - Not including hibServices causes the MCO filter to not work
            sql.FromClause = "hibTrips T1, hibLegs T2";
            sql.KeyWhereClause = "T1.reckey = T2.reckey and airline not in ('ZZ','$$') and valcarr not in ('ZZ','$$') and airchg != 0 and ";

            //sql.FromClause = "hibTrips T1, hibLegs T2";
            //sql.KeyWhereClause = "T1.reckey = T2.reckey and airline not in ('ZZ','$$') and valcarr not in ('ZZ','$$') and airchg != 0 and ";
            sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            sql.FieldList = "acct, break1, break2, break3, T1.reckey, cardnum, valcarr, ticket, trantype, invoice, invdate, invdate as trandate, passlast, "
                            + "passfrst, airchg, origin, destinat, connect, convert(int,T2.seqno) as seqno, recloc, mode, origOrigin, origDest, depdate, arrdate";
            sql.OrderBy = "";

            sql.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildWhere, false);

            return sql;
        }

        public string GetCreditCardCompanyWhereClause(string creditCardCompany)
        {
            switch (creditCardCompany)
            {
                case "MC":
                    return " and left(T1.cardnum,2) in ('MC','CA') ";
                case "VI":
                    return " and left(T1.cardnum,2) in ('VI','BA') ";
                case "DI":
                    return " and left(T1.cardnum,2) in ('DI','DS') ";
                default:
                    return " and left(T1.cardnum,2) = '" + creditCardCompany + "' ";
            }
        }

        public string GetNoCreditCardCompanyWhereClause()
        {
            return " and T1.cardnum != ' ' and T1.cardnum is not null and T1.cardnum not like '%VOID%' ";
        }

        public string GetCreditCardNumberSql(string creditCardNumber)
        {
            var tempCreditCardNumber = SharedProcedures.FixWildcard(creditCardNumber);
            if (creditCardNumber.Length == 4) return " and right(ltrim(rtrim(T1.cardnum)),4) " + tempCreditCardNumber.Trim();

            return " and right(ltrim(rtrim(T1.cardnum)),5) " + tempCreditCardNumber.Trim();
        }

        public string GetCreditCardWhereText(string creditCardNumber, bool whereTextIsEmpty)
        {
            if (whereTextIsEmpty) return "Card # = " + creditCardNumber;

            return "; Card # = " + creditCardNumber;
        }

        public string ReplaceTrandateWithInvoiceDate(string existingClause)
        {
            return existingClause.Replace("trandate", "invdate");
        }

        public string GetWhereTrip(DateTime beginDate, DateTime endDate, string creditCardCompany, string creditCardNumber, bool includeVoids, bool isInvoice, bool isCredit)
        {
            var whereTrip = "trandate between '" + beginDate.ToShortDateString() + "' and '" +
                                endDate.ToShortDateString() + " 11:59:59 PM'";
            
            if (isInvoice)
            {
                whereTrip += includeVoids ? " and T1.trantype in ('I','V')" : " and T1.trantype = 'I'";
            }
            else if (isCredit)
            {
                whereTrip += includeVoids ? " and T1.trantype in ('C','V')" : " and T1.trantype = 'C'";
            }
            else
            {
                if (includeVoids) whereTrip += " and T1.trantype != 'V'";
            }

            switch (creditCardCompany)
            {
                case "MC":
                    whereTrip += " and CardType in ('MC','CA')";
                    break;
                case "VI":
                    whereTrip += " and CardType in ('VI','BA')";
                    break;
                default:
                    whereTrip += " and CardType = '" + creditCardCompany + "'";
                    break;
            }
            
            if (!string.IsNullOrEmpty(creditCardNumber)) whereTrip += GetCreditCardNumberSql(creditCardNumber);

            return whereTrip;
        }
    }
}
