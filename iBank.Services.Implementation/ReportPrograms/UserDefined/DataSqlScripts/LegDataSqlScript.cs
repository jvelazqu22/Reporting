using Domain.Models;
using Domain.Interfaces;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.DataSqlScripts
{
    public class LegDataSqlScript:IUserDefinedSqlScript
    {
        public SqlScript GetSqlScript(bool isTripTls, bool udidExists, bool isPreview, string whereClause)
        {
            string fromClause;
            if (udidExists)
            {
                fromClause = isPreview ? "ibtrips T1, iblegs T2, ibudids T3" : "hibtrips T1, hiblegs T2, hibudids T3";
                whereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and T1.agency = T3.agency and airline not in ('ZZ','$$') and valcarr not in ('ZZ','$$') and " + whereClause;
            }
            else
            {
                fromClause = isPreview ? "ibtrips T1, iblegs T2" : "hibtrips T1, hiblegs T2";
                whereClause = "T1.reckey = T2.reckey and airline not in ('ZZ','$$') and valcarr not in ('ZZ','$$') and " + whereClause;
            }
            if (isTripTls)
            {
                fromClause = isPreview ? fromClause.Replace("ibtrips", "vibtripstls") : fromClause.Replace("hibtrips", "vhibtripstls");
            }

            var fieldList = " T1.reckey as reckey, T1.recloc, T1.agency as agency, T1.exchange, passlast, passfrst, basefare, origin, destinat, " +
                            "left(airline+replicate(' ',10-datalength(airline)),10) as airline, fltno, rdepdate, deptime, rarrdate, arrtime, " +
                            "class as classcode, classcat,  connect, mode,convert(int,seqno) as  seqno,convert(int,miles) as miles, actfare, " +
                            "miscamt, farebase, ditcode, mktpair,convert(int,smartctrfl) as smartctrfl, carriertyp, tktsegstat, endodflag, " +
                            "isnull(flduration,0) as flduration, agentid, pseudocity, branch, moneytype, invdate, bookdate, domintl ";
            if (isPreview)
            {
                fieldList += ",1 as rplusmin, ' ' as  tktdesig, convert(int,segnum) as segnum, seat, origin as origorigin, destinat as origdest, " +
                             "airline as origcarr, stops, segstatus, emailaddr, gds, 0.00 as fltgrsfare, 0.00 as fltprofare, equip, flduration ";
            }
            else
            {
                fieldList += ",convert(int,rplusmin) as rplusmin, tktdesig, 0 as segnum, origorigin, origdest, origcarr, ' ' as seat, ' ' as stops, " +
                             "' ' as segstatus, ' ' as emailaddr, ' ' as gds, fltgrsfare, fltprofare, ' ' as equip, isnull(flduration,0) as flduration ";
            }

            var orderBy = "order by T1.reckey, seqno";

            return new SqlScript
            {
                FieldList = fieldList,
                FromClause = fromClause,
                WhereClause = whereClause,
                KeyWhereClause = whereClause + " and ",
                OrderBy = orderBy,
                GroupBy = ""
            };
        }
    }
}
