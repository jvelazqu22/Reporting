using Domain;
using Domain.Interfaces;
using Domain.Models;


namespace iBank.Services.Implementation.ReportPrograms.UserDefined.DataSqlScripts
{
    public class MarketSegmentSqlScript : IUserDefinedSqlScript
    {
        public SqlScript GetSqlScript(bool isTripTls, bool udidExists, bool isPreview, string whereClause)
        {
            string fromClause;
            var fieldList = " T1.reckey, T1.agency, T1.recloc, T1.invoice, T1.moneytype, T1.invdate, pseudocity, agentid, T1.bookdate, convert(int, TMK.segnum) as segnum, TMK.segorigin, TMK.segdest, TMK.mktseg, TMK.mktsegboth, TMK.mktctry, TMK.mktreg, TMK.mktCtry2, TMK.mktReg2,convert(int, TMK.miles) as miles, convert(int, TMK.stops) as stops, TMK.mode, TMK.airline, TMK.fltno, TMK.sdepdate, TMK.sarrdate, TMK.sdeptime, TMK.sarrtime,  TMK.segfare, TMK.segmiscamt, TMK.ditcode, TMK.class, TMK.classcat, convert(int, TMK.splusmin) as splusmin, TMK.flduration,  TMK.seggrsfare, TMK.segprofare, TMK.firstfltno, TMK.grossamt, TMK.prdairline, TMK.prdclass, TMK.prdclscat, TMK.segcommisn, TMK.firstaline, TMK.prdfbase, TMK.samealine, TMK.samefbase, TMK.sfaretax, TMK.connectime, TMK.sfltstatus, ";
            if (udidExists)
            {
                fromClause = isPreview ? "ibtrips T1, ibmktsegs TMK, ibudids T3" : "hibtrips T1, hibmktsegs TMK, hibudids T3";
                whereClause = "T1.reckey = TMK.reckey and T1.reckey = T3.reckey and T1.agency = T3.agency and " + whereClause;
            }
            else
            {
                fromClause = isPreview ? "ibtrips T1, ibmktsegs TMK" : "hibtrips T1, hibmktsegs TMK";
                whereClause = "T1.reckey = TMK.reckey and " + whereClause;
            }
            if (isTripTls)
            {
                fromClause = isPreview ? fromClause.Replace("ibtrips", "vibtripstls") : fromClause.Replace("hibtrips", "vhibtripstls");
            }
            if ( isPreview || !Features.SegtransactionidFeautureFlag.IsEnabled() )
            {
                fieldList += "'' as segtransid ";
            }
            else
            {
                fieldList += "TMK.segtransactionid as segtransid ";
            }

            var orderBy = "order by T1.reckey, segnum ";

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
