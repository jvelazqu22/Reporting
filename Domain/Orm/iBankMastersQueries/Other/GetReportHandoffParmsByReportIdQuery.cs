using System.Collections.Generic;
using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetReportHandoffParmsByReportIdQuery : IQuery<IList<reporthandoff>>
    {
        public string ReportId { get; set; }
        private readonly IMastersQueryable _db;

        public GetReportHandoffParmsByReportIdQuery(IMastersQueryable db, string reportId)
        {
            _db = db;
            ReportId = reportId;
        }

        public IList<reporthandoff> ExecuteQuery()
        {
            using(_db)
            {
                return _db.ReportHandoff.Where(x => x.reportid == ReportId).ToList();
            }
        }
    }
}
