using Domain.Models;


namespace iBank.Services.Implementation.ReportPrograms.XmlExtract.DataSqlScripts
{
    public class TripDataSqlScript
    {
        public SqlScript GetSqlScript(bool udidExists, bool isPreview, string whereClause)
        {
            string fromClause;
            if (udidExists)
            {
                fromClause = isPreview ? "ibtrips T1, ibudids T3" : "hibtrips T1, hibudids T3";
                whereClause = "T1.reckey = T3.reckey and " + whereClause;
            }
            else
            {
                fromClause = isPreview ? "ibtrips T1" : "hibtrips T1";
            }

            var fieldList = @" T1.agency, T1.reckey, T1.recloc,  branch, agentid, pseudocity, acct, invoice, " +
                            " invdate, bookdate, domintl, ticket, passlast, passfrst, break1, " +
                            " break2, break3, break4, airchg, credcard, cardnum, arrdate, " +
                            " mktfare, valcarr, depdate, stndchg, offrdchg, reascode, savingcode, bktool, bkagent, tkagent, " +
                            " tickettype, faretax, basefare, corpacct, sourceabbr, tax4, tax3, " +
                            " tax2, tax1, origticket, exchange, moneytype, iatanbr, 'UPD ' as chgindcatr ";
            if (isPreview)
            {
                fieldList += @", 'I' as trantype, convert(int,1) as plusmin, 0.00 as svcfee, 0.00 as acommisn, " +
                "' ' as tourcode, cancelcode, phone, lastupdate, " +
                "gds, changedby, changstamp, parsestamp, ' ' as agcontact, emailaddr, lastupdate ";
            }
            else
            {
                //*IN HIBTRIPS, WE DO NOT HAVE THESE DATE COLUMNS: lastupdate,changstamp,parsestamp.
                // * WE NEED A DATETIME DATA TYPE PLACEHOLDER IN THE OUTPUT CURSOR.  HERE WE SELECT
                // * INVDATE AS EACH OF THESE COLUMNS.BELOW, WE WILL CHANGE THE VALUE IN THESE
                //* COLUMNS TO 01 / 01 / 1900, WHICH IS OUR IBANK EMPTY DATE VALUE.
                fieldList += @", trantype, convert(int,plusmin) as plusmin, svcfee, acommisn, tourcode, ' ' as cancelcode, ' ' as phone, " +
                            "' ' as gds, ' ' as changedby, invdate as changstamp, invdate as parsestamp, ' ' as agcontact, ' ' as emailaddr, invdate as lastupdate ";
            }

            var orderBy = "order by T1.reckey";

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
