
using Domain.Models;
using Domain.Interfaces;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.DataSqlScripts
{
    class MiscSegSharedSqlScript : IUserDefinedSqlScript
    {
        public string SegType { get; set;}
        public SqlScript GetSqlScript(bool isTripTls, bool udidExists, bool isPreview, string whereClause)
        {
            string fromClause;
            if (udidExists)
            {
                fromClause = isPreview ? "ibtrips T1, ibmiscsegs TMS, ibudids T3" : "hibtrips T1, hibmiscsegs TMS, hibudids T3";
                whereClause = "T1.reckey = TMS.reckey and T1.reckey = T3.reckey and T1.agency = T3.agency and " + whereClause;
            }
            else
            {
                fromClause = isPreview ? "ibtrips T1, ibmiscsegs TMS" : "hibtrips T1, hibmiscsegs TMS";
                whereClause = "T1.reckey = TMS.reckey and " + whereClause;
            }
            if (isTripTls)
            {
                fromClause = isPreview ? fromClause.Replace("ibtrips", "vibtripstls") : fromClause.Replace("hibtrips", "vhibtripstls");
            }

            var fieldList = "T1.reckey, T1.agency, T1.recloc, T1.invoice, T1.invdate, pseudocity, agentid, T1.bookdate, TMS.moneytype, TMS.segtype, TMS.msorigin, TMS.msdestinat, TMS.msorgctry, TMS.msdestctry, TMS.vendorcode, TMS.svcidnbr, TMS.msdepdate,TMS.msdeptime, TMS.msarrdate, TMS.msarrtime, convert(int,TMS.msseqno) as msseqno, TMS.class, convert(int,TMS.msplusmin) as msplusmin, TMS.mstrantype, TMS.segamt, TMS.moneytype, TMS.msexcprate, TMS.msstndrate, TMS.mslosscode, TMS.mssvgcode, TMS.tax1, TMS.tax2, TMS.tax3, TMS.tax4, TMS.prodcode, TMS.numvehics, TMS.numadults, TMS.numchild, TMS.cabintype, TMS.confirmno, TMS.cabinseat, TMS.mxchaincod, TMS.mealdesc, convert(int,TMS.nitecount) as nitecount, TMS.opt, TMS.arrivermks, TMS.departrmks, TMS.mxtourcode, TMS.mxtourname, cast(TMS.trnsfrrmks as nvarchar(max)) trnsfrrmks,TMS.mxvendname, TMS.mxsgstatus, convert(int,TMS.tourcount) as tourcount, TMS.baseprice1, TMS.baseprice2, convert(int,TMS.nbrrooms) as nbrrooms, TMS.mscommisn, TMS.msorgcode, TMS.msdestcode, convert(int,TMS.mssegnum) as mssegnum, convert(int,TMS.msduration) as msduration, TMS.shipname, TMS.cabincateg, TMS.cabinnbr, TMS.cabindeck, TMS.pgmid, TMS.spclinfo, TMS.regionid, TMS.msratetype ";


            var orderBy = "order by T1.reckey, TMS.msseqno ";
            whereClause += " and TMS.segtype = '" + SegType + "' ";

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
