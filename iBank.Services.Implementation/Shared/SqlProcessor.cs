using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Domain.Helper;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.Shared
{
    public static class SqlProcessor
    {

        private static readonly List<string> CurrencyFields = new List<string>
        {
            "abookrat",
			"aexcprat",
			"compamt",
			"ccommissn",
			"carstdrate",
			"bookrate",
			"hcommissn",
			"hotstdrate",
            "actfare",
			"miscamt",
			"segFare",
			"segMiscAmt",
			"svcfee",
			"svcamt",
			"tax1",
			"tax2",
			"tax3",
			"tax4",
			"airchg",
			"stndchg",
			"mktfare",
			"offrdchg",
			"acommisn",
			"basefare",
			"faretax",
			"reissuefee"
        };
 

        /// <summary>
        /// Given the various pieces of the SQL statement created in a report program, this function creates the full SQL statement. 
        /// The field list returned can be used to build a data class for the SqlQuery function. 
        /// </summary>
        /// <param name="fieldList"></param>
        /// <param name="addFieldsFromLegsTable">If true, adds extra fields from the Legs table.</param>
        /// <param name="fromClause"></param>
        /// <param name="whereClause"></param>
        /// <param name="orderBy"></param>
        /// <param name="globals"></param>
        /// <returns></returns>
        public static string ProcessSql(string fieldList, bool addFieldsFromLegsTable, string fromClause, string whereClause, string orderBy,ReportGlobals globals)
        {
            fromClause = AddNoLock(fromClause);

            if (addFieldsFromLegsTable)
            {
                if (fromClause.IndexOf("mktsegs", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    // EPS/nf: When Mode is Rail, then the origin/dest needs to be a concatenation of the segOrigin/dest + airline columns
                    // fieldList += @",segOrigin as origin, segDest as destinat, airline, mode, fltno,sDepDate as rDepDate ,sdeptime as deptime,sarrdate as rarrdate,sarrtime as arrtime, class as classCode, classcat,  convert(int,miles) as miles, segfare as actfare,segmiscamt as miscamt, ditcode";   
                    fieldList += @", case when mode in ('r','R') then rtrim(segorigin) + 
                                     case when rtrim(airline)='' or rtrim(segorigin)='' then '' else '-' + rtrim(airline) end 
                                     else segorigin end as origin, 
                                     case when mode in ('r','R') then rtrim(segdest) +
	                                 case when rtrim(airline)='' or rtrim(segdest)='' then '' else '-' + rtrim(airline) end 
                                     else segdest end
	                                 as destinat, airline, mode, fltno,sDepDate as rDepDate ,sdeptime as deptime,sarrdate as rarrdate,sarrtime as arrtime, class, class as classCode, classcat,  convert(int,miles) as miles, segfare as actfare,segmiscamt as miscamt, ditcode, segnum as seqno";
                }
                else
                {
                    fieldList +=
                        @", origin, destinat, airline, mode, connect, rdepdate, fltno, deptime, rarrdate, arrtime, class, class as classCode, classcat, convert(int,seqno) as seqno, convert(int,miles) as miles, actfare, miscamt, farebase, ditcode";
                    if (globals.ParmValueEquals(WhereCriteria.PREPOST, "1"))
                    {
                        fieldList += @", convert(int,segnum) as segnum, seat, stops, segstatus, ' ' as tktdesig, convert(int,1) as rplusmin, origin as origOrigin, destinat as origDest, airline as origCarr";
                    }
                    else
                    {
                        fieldList += @", ' ' as seat, ' ' as stops, ' ' as segstatus, tktdesig, convert(int,rplusmin) as rplusmin, origOrigin, origDest, origCarr ";
                    }
                }
            }

            fieldList = fieldList.Replace("[comma]", ",");

            //handle currency fields
            fieldList = HandleCurrencyConversion(fieldList,fromClause,addFieldsFromLegsTable,globals);
            
            return string.Format("select {0} from {1} where {2} {3}", fieldList, fromClause, whereClause, orderBy);
        }

        private static readonly DateTime CurrencyConversionThreshold = new DateTime(2003,1,1);
        private static string HandleCurrencyConversion(string fieldList, string fromClause, bool addFieldsFromLegsTable, ReportGlobals globals)
        {

            //TODO: If date is prior to 2003, don't do conversion. FoxPro code returns an error, which seems unecessary. Check on this. 
            var doCurrencyConversion = (CurrencyFields.Any(fieldList.ToLower().Contains) || addFieldsFromLegsTable ) && (globals.BeginDate >= CurrencyConversionThreshold );

            if (!doCurrencyConversion) return fieldList;

            //add moneytype fields for all included tables. Note that checking for "ib" also checks for "hib"
            if (fromClause.ToUpper().Contains("IBTRIPS") && !fieldList.ToUpper().Contains("AIRCURRTYP"))
            {
                fieldList = "T1.MoneyType as AirCurrTyp, " + fieldList;
            }
            if (fromClause.ToUpper().Contains("IBCAR") && !fieldList.ToUpper().Contains("CARCURRTYP"))
            {
                fieldList = "T4.MoneyType as CarCurrTyp, RentDate + days as CarExchangeDate, " + fieldList;
            }
            if (fromClause.ToUpper().Contains("IBHOTEL") && !fieldList.ToUpper().Contains("HOTCURRTYP"))
            {
                fieldList = "T5.MoneyType as HotCurrTyp, DateIn + nights as HotelExchangeDate, " + fieldList;
            }
            if (fromClause.ToUpper().Contains("HIBSERVICES") && !fieldList.ToUpper().Contains("FEECURRTYP"))
            {
                //TODO: See if we can stop checking for this when we convert ibQView3
                //** 10/20/2009 - SOMETIMES WHEN THIS IS CALLED FROM ibQView3, **
                //** THE TABLE ALIAS FOR hibSvcFees IS "T1", NOT "T6".         **
                if (fromClause.ToUpper().Contains("HIBSERVICES T1"))
                {
                    fieldList = "T1.MoneyType as FeeCurrTyp, " + fieldList;
                }
                else
                {
                    fieldList = "T6A.MoneyType as FeeCurrTyp, " + fieldList;
                }
                    
            }

            if (fromClause.ToUpper().Contains("IBCANCTRIPS") && !fieldList.ToUpper().Contains("AIRCURRTYP"))
            {
                fieldList = "T1.MoneyType as AirCurrTyp, " + fieldList;
            }
            if (fromClause.ToUpper().Contains("IBCANCCARS") && !fieldList.ToUpper().Contains("CARCURRTYP"))
            {
                fieldList = "T4.MoneyType as CarCurrTyp, " + fieldList;
            }
            if (fromClause.ToUpper().Contains("IBCANCHOTELS") && !fieldList.ToUpper().Contains("HOTCURRTYP"))
            {
                fieldList = "T5.MoneyType as HotCurrTyp, " + fieldList;
            }
            if (fromClause.ToUpper().Contains("TTRTRIPS") && !fieldList.ToUpper().Contains("AIRCURRTYP"))
            {
                fieldList = "T1.MoneyType as AirCurrTyp, " + fieldList;
            }

            if (globals.ProcessKey == 36) //special case for ibQView4
            {
                if (fromClause.ToUpper().Contains("TRIPS"))
                {
                    fieldList = "InvDate, " + fieldList;
                }
                else
                {
                    fieldList = "TranDate as InvDate, " + fieldList;
                }
            }

            if ((fromClause.ToUpper().Contains("IBTRIPS") || fromClause.ToUpper().Contains("IBCANCTRIPS")) && !fieldList.ToUpper().Contains("INVDATE"))
            {
                fieldList = "InvDate, " + fieldList;
            }

            if ((fromClause.ToUpper().Contains("IBTRIPS") || fromClause.ToUpper().Contains("IBCANCTRIPS")) && !fieldList.ToUpper().Contains("BOOKDATE"))
            {
                fieldList = "BookDate, " + fieldList;
            }

            if (fromClause.ToUpper().Contains("TTRTRIPS"))
            {
                fieldList = "TktDate as InvDate, TktDate as BookDate, " + fieldList;
            }

            return fieldList;
        }

        /// <summary>
        /// add nolock to all table references
        /// </summary>
        /// <param name="fromClause"></param>
        /// <returns></returns>
        private static string AddNoLock(string fromClause)
        {
            fromClause = fromClause.Replace(" T1", " T1 WITH (nolock) ");
            fromClause = fromClause.Replace(" T2", " T2 WITH (nolock) ");
            fromClause = fromClause.Replace(" T3", " T3 WITH (nolock) ");
            fromClause = fromClause.Replace(" T4", " T4 WITH (nolock) ");
            fromClause = fromClause.Replace(" T5", " T5 WITH (nolock) ");
            fromClause = fromClause.Replace(" T6A", " T6A WITH (nolock) ");
            if (!fromClause.Contains("T6A"))
            {
                fromClause = fromClause.Replace(" T6", " T6 WITH (nolock) ");
            }

            fromClause = fromClause.Replace(" TMS", " TMS WITH (nolock) ");
            fromClause = fromClause.Replace(" T51", " T51 WITH (nolock) ");
            fromClause = fromClause.Replace(" T52", " T52 WITH (nolock) ");
            fromClause = fromClause.Replace(" T61", " T61 WITH (nolock) ");
            fromClause = fromClause.Replace(" T62", " T62 WITH (nolock) ");
            fromClause = fromClause.Replace(" T7", " T7 WITH (nolock) ");
            fromClause = fromClause.Replace(" T8", " T8 WITH (nolock) ");
            fromClause = fromClause.Replace(" T9", " T9 WITH (nolock) ");
            fromClause = fromClause.Replace(" TR61", " TR61 WITH (nolock) ");
            fromClause = fromClause.Replace(" TR62", " TR62 WITH (nolock) ");
            fromClause = fromClause.Replace(" TR651", " TR651 WITH (nolock) ");
            fromClause = fromClause.Replace(" T021", " T021 WITH (nolock) ");
            fromClause = fromClause.Replace(" TCL", " TCL WITH (nolock) ");

            return fromClause;
        }

                                                                                                    
    }
}
