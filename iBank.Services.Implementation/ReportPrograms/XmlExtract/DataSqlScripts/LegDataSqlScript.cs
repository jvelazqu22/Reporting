using Domain.Helper;
using Domain.Interfaces;
using Domain.Models;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.XmlExtract.DataSqlScripts
{
    public class LegDataSqlScript : IXmlSqlScript
    {
        private readonly ReportGlobals _globals;
        private readonly bool _air;
        private readonly bool _rail;
        public LegDataSqlScript(ReportGlobals globals, bool air, bool rail)
        {
            _globals = globals;
            _air = air;
            _rail = rail;
        }
        
        public SqlScript GetSqlScript(bool udidExists, bool isPreview, string whereClause)
        {
            string fromClause;
            if (udidExists)
            {
                fromClause = isPreview ? "ibtrips T1, iblegs T2, ibudids T3" : "hibtrips T1, hiblegs T2, hibudids T3";
                if (_globals.IsParmValueOn(WhereCriteria.CBINCLCARRIERZZ))
                {
                    whereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and " + whereClause;
                }
                else
                {
                    whereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and " +
                    "airline not in ('ZZ','$$') and valcarr not in ('ZZ','$$') and " + whereClause;
                }

            }
            else
            {
                fromClause = isPreview ? "ibtrips T1, iblegs T2" : "hibtrips T1, hiblegs T2";
                if (_globals.IsParmValueOn(WhereCriteria.CBINCLCARRIERZZ))
                {
                    whereClause = "T1.reckey = T2.reckey and " + whereClause;
                }
                else
                {
                    whereClause = "T1.reckey = T2.reckey and airline not in ('ZZ','$$') and valcarr not in ('ZZ','$$') and " + whereClause;
                }
            }
            if (!_rail)
            {
                whereClause += " and t2.mode != 'R'";
            }

            if (!_air)
            {
                whereClause += " and t2.mode != 'A'";
            }

            var fieldList = " T1.reckey as reckey, T1.recloc, T1.agency as agency, passlast, passfrst, origin, " +
            " destinat, airline, fltno, rdepdate, deptime, rarrdate, arrtime, class, classcat, " +
            " connect, mode, convert(int,seqno) as seqno, convert(int,miles) as miles, actfare, miscamt, farebase, ditcode, agentid, pseudocity, branch ";

            if (isPreview)
            {
                fieldList += ",convert(int,1) as rplusmin, ' ' as  tktdesig,convert(int,segnum) as  segnum, seat,  stops, segstatus, emailaddr, gds ";
            }
            else
            {
                fieldList += ",convert(int,rplusmin) as rplusmin, tktdesig, 0 as segnum, ' ' as seat, ' ' as stops, ' ' as segstatus, ' ' as emailaddr, ' ' as gds ";
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
