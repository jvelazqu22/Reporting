using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.ReportLog;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.ReportLog
{
    public static class LookupProcess
    {
        private static List<ProcessData> _processRecords;

        public static string LookupProcessCaption(int processKey ,string rptProgram, BuildWhere buildWhere)
        {
            var sql = "select rptname, pgmName, processkey, v4Caption from ibproces";
            if (_processRecords == null || !_processRecords.Any())
            {
                _processRecords = new OpenMasterQuery<ProcessData>(new iBankMastersQueryable(), sql, buildWhere.Parameters).ExecuteQuery();
            }

            var rec = _processRecords.FirstOrDefault(s => s.ProcessKey == processKey) ?? _processRecords.FirstOrDefault(s => s.PgmName.EqualsIgnoreCase(rptProgram));
            return rec == null ? rptProgram : rec.V4Caption;
        }
    }
}
