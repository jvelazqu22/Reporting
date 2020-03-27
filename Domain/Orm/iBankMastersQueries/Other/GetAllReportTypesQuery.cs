using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetAllReportTypesQuery : IQuery<List<ProcessInformation>>
    {
        private readonly IMastersQueryable _db;

        public GetAllReportTypesQuery(IMastersQueryable db)
        {
            _db = db;
        }
        public List<ProcessInformation> ExecuteQuery()
        {
            using(_db)
            {
                return _db.iBProcess.Select(s => new ProcessInformation
                {
                                                         ProcessKey = s.processkey,
                                                         RptType = s.rpttype.Trim(),
                                                         Caption = s.caption
                                                     }).ToList();
            }
        }
    }
}
