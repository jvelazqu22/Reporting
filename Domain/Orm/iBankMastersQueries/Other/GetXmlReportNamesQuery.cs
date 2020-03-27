using System.Collections.Generic;
using System.Linq;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetXmlReportNamesQuery : IQuery<Dictionary<int, string>>
    {
        private readonly IMastersQueryable _db;

        public GetXmlReportNamesQuery(IMastersQueryable db)
        {
            _db = db;
        }
        public Dictionary<int, string> ExecuteQuery()
        {
            using (_db)
            {
                return _db.XmlRpt.ToDictionary(x => x.reportkey, x => x.crname);
            }
        }
    }
}
