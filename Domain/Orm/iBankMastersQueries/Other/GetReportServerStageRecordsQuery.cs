using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetReportServerStageRecordsQuery : IQuery<IList<report_server_stage>>
    {
        private IMastersQueryable _db;
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public GetReportServerStageRecordsQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<report_server_stage> ExecuteQuery()
        {
            using (_db)
            {
                var list = _db.ReportServerStage.Where(x => x != null).ToList();

                return list;
            }
        }
    }
}
