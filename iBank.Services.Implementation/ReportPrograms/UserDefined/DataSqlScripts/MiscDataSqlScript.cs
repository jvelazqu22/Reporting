using Domain.Interfaces;
using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.DataSqlScripts
{
    public class MiscDataSqlScript : IUserDefinedSqlScript
    {
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

            var fieldList = " T1.reckey, T1.agency, T1.recloc, T1.invoice, T1.invdate, pseudocity, agentid, T1.bookdate,  TMS.segtype, TMS.msorigin, TMS.msdestinat, TMS.msorgctry, TMS.msdestctry,  TMS.vendorcode, TMS.svcidnbr, TMS.msdepdate, TMS.msdeptime, TMS.msarrdate, TMS.msarrtime,  TMS.msseqno, TMS.class, TMS.msplusmin, TMS.mstrantype, TMS.segamt, TMS.moneytype,  TMS.msexcprate, TMS.msstndrate, TMS.mslosscode, TMS.mssvgcode,  TMS.tax1, TMS.tax2, TMS.tax3, TMS.tax4, TMS.cabintype, TMS.confirmno,  TMS.cabinseat, TMS.mxchaincod, TMS.mealdesc, TMS.nitecount, TMS.opt, TMS.arrivermks, TMS.departrmks, TMS.mxtourcode, TMS.mxtourname,  cast(TMS.trnsfrrmks as nvarchar(max)) trnsfrrmks, TMS.mxvendname, TMS.mxsgstatus, TMS.tourcount ";

            var orderBy = "order by T1.reckey, TMS.msdepdate, TMS.msseqno";

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
