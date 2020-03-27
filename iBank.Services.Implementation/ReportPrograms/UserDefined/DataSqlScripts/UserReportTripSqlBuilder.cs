using Domain.Models;
using Domain.Helper;
using System;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.DataSqlScripts
{
    public class UserReportTripSqlBuilder
    {
        public string GetTripFromClause(int udidNumber, bool isPreview, bool tripTlsSwitch)
        {
            string fromClause;
            if (udidNumber > 0)
            {
                fromClause = isPreview ? "ibtrips T1, ibudids T3":  "hibtrips T1, hibudids T3";  
            }
            else
            {
                fromClause = isPreview ? "ibtrips T1" : "hibtrips T1";
            }

            if (tripTlsSwitch) fromClause = isPreview? fromClause.Replace("ibtrips", "vibtripstls") : fromClause.Replace("hibtrips", "vhibtripstls");

            return fromClause;
        }
        public string GetCarRentalFromClause(int udidNumber, bool isPreview, bool tripTlsSwitch)
        {
            string fromClause;
            if (udidNumber > 0)
            {
                 fromClause = isPreview ? "ibtrips T1, ibudids T3, ibcars T4 ":"hibtrips T1, hibudids T3, hibcars T4 ";                
            }
            else
            {
                 fromClause = isPreview ? "ibtrips T1, ibcars T4":"hibtrips T1, hibcars T4";
            }

            if (tripTlsSwitch) fromClause = isPreview ? fromClause.Replace("ibtrips", "vibtripstls") : fromClause.Replace("hibtrips", "vhibtripstls");

            return fromClause;
        }
        public string GetServiceFeesFromClause(int udidNumber, bool isPreview, bool tripTlsSwitch)
        {
            string fromClause;

            if (udidNumber > 0)
            {
                if (isPreview)
                {
                    fromClause = "ibtrips T1, ibudids T3, ibServices T6A  ";
                }
                else
                {
                    fromClause = "hibtrips T1, hibudids T3, hibServices T6A  ";
                    if (tripTlsSwitch) fromClause = fromClause.Replace("hibtrips", "vhibtripstls");
                }
            }
            else
            {
                if (isPreview)
                {
                    fromClause = "ibtrips T1, hibServices T6A ";
                }
                else
                {
                    fromClause = "hibtrips T1, hibServices T6A ";
                    if (tripTlsSwitch) fromClause = fromClause.Replace("hibtrips", "vhibtripstls");
                }
            }

            return fromClause;
        }
        public string GetHotelFromClause(int udidNumber, bool isPreview, bool tripTlsSwitch)
        {
            string fromClause;
            if (udidNumber > 0)
            {
                if (isPreview)
                {
                    fromClause = "ibtrips T1, ibudids T3, ibhotel T5 ";
                }
                else
                {
                    fromClause = "hibtrips T1, hibudids T3, hibhotel T5 ";
                    if (tripTlsSwitch) fromClause = fromClause.Replace("hibtrips", "vhibtripstls");
                }
            }
            else
            {
                if (isPreview)
                {
                    fromClause =  "ibtrips T1, ibhotel T5";
                }
                else
                {
                    fromClause = "hibtrips T1, hibhotel T5";
                    if (tripTlsSwitch) fromClause = fromClause.Replace("hibtrips", "vhibtripstls");
                }
            }
            return fromClause;
        }

        public string GetTripFieldList(bool isPreview, bool tripTlsSwitch)
        {
            var fieldList = @" T1.agency, T1.reckey, T1.recloc, branch, agentid, pseudocity, acct, invoice,  invdate, bookdate, domintl, ticket, 
                    passlast, passfrst, break1,  break2, break3, break4, airchg, credcard, cardnum, cast(ccnumber2 as nvarchar(max)) ccnumber2, arrdate,  mktfare, depdate, tripstart, tripend, 
                    stndchg, offrdchg, reascode, savingcode, valcarr+replicate(' ',10-datalength(valcarr)) as valcarr,tickettype, faretax, basefare, T1.corpacct, 
                    sourceabbr, T1.tax4, T1.tax3, T1.tax2, T1.tax1, origticket, exchange, T1.moneytype, iatanbr, 'UPD ' as chgindcatr ";

            if (isPreview)
            {
                fieldList += @",'I' as trantype, convert(int,1) as plusmin, 0.00 as svcfee, 0.00 as acommisn, T1.pnrcrdtgmt,T1.parsestamp, ' ' as valcarmode, ' ' as origvalcar,
                    refundable, ' ' as tourcode, cancelcode, phone, lastupdate, bktool, bkagent, tkagent, gds, T1.changedby, T1.changstamp, ' ' as agcontact, emailaddr ";
                if (tripTlsSwitch)
                {
                    fieldList += @", '' as invctrycod, '' as netremit, '' as primprodcd, '' as clientid, '' as formofpay, 0 as passseqno, 
                        '' as trpvendcod, 0.00 as invamt, 0.00 as invcommis,'' as bkPseudo, '' as tkPseudo, 0 as smartCtrTr, '' as bkAgtName, '' as tkAgtName, 
                        cast ('1900-01-01' as datetime) as trpRetDate, '' as returnFlt,'' as retExchInd, '' as origInvoic, cast ('1900-01-01' as datetime) as origInvDat, 
                        '' as origCurr, 0.00 as exchRate, '' as tktTypeInd, '' as discount, '' as docStatus, '' as exchCode, 0 as trpHotSeq,0 as trpCarSeq, 0 as trpTourSeq, 
                        0 as trpOthSeq, 0 as trpAuxSeq, cast ('1900-01-01' as datetime) as voidDate, '' as valCarrNo, '' as vendRevGrp, '' as vendName,convert(decimal, fees) as fees, maxagentid, 
                        atktstatus, farebasis, predomcarr, trpctypair, tktnbrsuff, fopindic, 
                        numsegs, trpODmiles, TrPRDClass, TrpPRDDest, TrpPRDOrig, PRDCarComp, PRDFareBas, PRDHchain";
                }
            }
            else
            {
                //FoxPro use invdate as placeholder then later update to '1900-01-01' in line 824-825
                //update curTrips set lastupdate = date(1900, 1, 1), changstamp = date(1900, 1, 1), ;
                //parsestamp = date(1900, 1, 1), pnrcrdtgmt = date(1900, 1, 1)
                fieldList += @", trantype, convert(int,plusmin) as plusmin, svcfee, acommisn, ' ' as refundable, tourcode, ' ' as cancelcode, cast ('1900-01-01' as datetime)  as pnrcrdtgmt, 
                        cast ('1900-01-01' as datetime)  as parsestamp,valcarmode, origvalcar, ' ' as phone, bktool, bkagent, tkagent, gds, ' ' as changedby, cast ('1900-01-01' as datetime)  as changstamp, ' ' as agcontact, ' ' as emailaddr, cast ('1900-01-01' as datetime)  as lastupdate ";
                if (tripTlsSwitch)
                {
                    fieldList += @", invctrycod, netremit, primprodcd, clientid, formofpay, convert(int,passseqno) as passseqno, trpvendcod, invamt, invcommis, bkPseudo, tkPseudo, 
                        convert(int,smartCtrTr) as smartCtrTr, bkAgtName, tkAgtName, trpRetDate, returnFlt, retExchInd, origInvoic, origInvDat, origCurr, convert(decimal, exchRate) exchRate, tktTypeInd, 
                        discount, docStatus, exchCode, convert(int,trpHotSeq) trpHotSeq, convert(int,trpCarSeq) trpCarSeq, convert(int,trpTourSeq) trpTourSeq, convert(int,trpOthSeq) trpOthSeq, convert(int,trpAuxSeq) trpAuxSeq, voidDate, valCarrNo, vendRevGrp, vendName, " +
                        "convert(decimal, fees) as fees, maxagentid, atktstatus, farebasis, predomcarr, trpctypair, tktnbrsuff, fopindic, " +
                        "numsegs, trpODmiles, TrPRDClass, TrpPRDDest, TrpPRDOrig, PRDCarComp, PRDFareBas, PRDHchain";
                }
            }
            return fieldList;
        }

        public string GetTripWhereClause(string whereClause, int udidNumber)
        {
            return (udidNumber>0) 
                ? "T1.reckey = T3.reckey and T1.agency = T3.agency and " + whereClause
                : whereClause;
        }

        public string GetHotelWhereClause(string whereClause, bool isPreview)
        {
            var type =  !isPreview? "and T5.HotTranTyp != 'V' " : "";
            return "t1.reckey = t5.reckey " + type + "and " + whereClause;
        }

        public string GetCarRentalWhereClause(string whereClause)
        {
            return " t1.reckey = t4.reckey and  T4.CarTranTyp != 'V' and " + whereClause;
        }

        public string GetServiceFeesWhereClause(string whereClause)
        {
            return " T1.reckey = T6A.reckey and T1.agency = T6A.agency and " + whereClause;
        }

        public string GetFieldName(int processKey, string dateRange)
        {
            if (dateRange == "") return "depdate";

            string result;
            switch ((DateType)Convert.ToInt32(dateRange))
            {
                case DateType.InvoiceDate:
                    result = "invdate";
                    break;
                case DateType.BookedDate:
                    result = "bookdate";
                    break;
                case DateType.CarRentalDate:
                    result = "rentdate";
                    break;
                case DateType.TransactionDate:
                    result = processKey == (int)ReportTitles.ServiceFeeUserDefinedReports ? "trandate" : "datein";
                    break;
                case DateType.HotelCheckInDate:
                    result = "datein";
                    break;
                case DateType.OnTheRoadDatesHotel:
                    result = "arrdate";
                    break;
                default:
                    result = "depdate";
                    break;
            }
            return result;
        }

        public SqlScript CreateScript(string whereClause, int udidNumber, bool isPreview, bool tripTlsSwitch, int processKey, string dateRange, BuildWhere buildWhere, bool includeAllLegs)
        {
            var fromClause = GetTripFromClause(udidNumber, isPreview, tripTlsSwitch);
            var fieldList = GetTripFieldList(isPreview, tripTlsSwitch);
            whereClause = GetTripWhereClause(whereClause, udidNumber);

            var dateType = GetFieldName(processKey, dateRange);

            switch (processKey)
            {
                //case (int)ReportTitles.AirUserDefinedReports:
                //    whereClause = whereClause.Replace("arrdate", "depdate");
                //    break;
                case (int)ReportTitles.HotelUserDefinedReports: 
                    whereClause = GetHotelWhereClause(whereClause, isPreview);
                    fromClause = GetHotelFromClause(udidNumber, isPreview, tripTlsSwitch);
                    break;
                case (int)ReportTitles.CarUserDefinedReports:
                    whereClause = GetCarRentalWhereClause(whereClause);
                    fromClause = GetCarRentalFromClause(udidNumber, isPreview, tripTlsSwitch);
                    if (dateType == "rentdate") fieldList += ", T4." + dateType;
                    break;
                case (int)ReportTitles.ServiceFeeUserDefinedReports:
                    whereClause = GetServiceFeesWhereClause(whereClause);
                    fromClause = GetServiceFeesFromClause(udidNumber, isPreview, tripTlsSwitch);
                    if (dateType == "trandate") fieldList += ", T6A." + dateType;
                    break;
            }

            var sql = new SqlScript
            {
                FieldList = fieldList,
                FromClause = fromClause,
                WhereClause = whereClause,
                KeyWhereClause = whereClause + " and ",
                OrderBy = "",
                GroupBy = ""
            };

            sql.WhereClause = new WhereClauseWithAdvanceParamsHandler().GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildWhere, includeAllLegs);

            return sql;
        }
    }
}
  
