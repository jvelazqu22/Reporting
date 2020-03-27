using Domain.Helper;
using Domain.Models;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities.ClientData;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.AirActivity
{
    public class UdidDataRetriever
    {
        public IList<UdidRecord> GetUdids(List<int> udidNumber, string whereClause, ReportGlobals globals, BuildWhere buildWhere, bool isReservationReport)
        {
            var sql = new SqlScript();
            var udidNoList = string.Join(",", udidNumber.Where(x => x > 0));

            sql.FromClause = isReservationReport ? "ibtrips T1, ibudids T3" : "hibtrips T1, hibudids T3";
            sql.WhereClause = "T1.reckey = T3.reckey and udidno in (" + udidNoList + ") and " + whereClause;
            sql.FieldList = "T1.reckey, convert(int,UdidNo) as UdidNumber, UdidText as UdidText, count(*) as dummy"; //no longer just one udid, it could be up to 10
            sql.OrderBy = "";
            
            var processedUdidSql = SqlProcessor.ProcessSql(sql.FieldList, false, sql.FromClause, sql.WhereClause, sql.OrderBy, globals);
            processedUdidSql += " group by T1.reckey, UdidNo, UdidText";
            return ClientDataRetrieval.GetOpenQueryData<UdidRecord>(processedUdidSql, globals, buildWhere.Parameters);
        }
    }
}
