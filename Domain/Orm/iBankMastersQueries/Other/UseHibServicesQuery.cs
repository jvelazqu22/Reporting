using System.Collections.Generic;
using System.Linq;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class UseHibServicesQuery : IQuery<bool>
    {
        public string Agency { get; set; }
        public IList<string> SourceAbbrs { get; set; }
        private readonly IMastersQueryable _db;

        public UseHibServicesQuery(IMastersQueryable db, string agency, IList<string> sourceAbbrs)
        {
            _db = db;
            Agency = agency;
            SourceAbbrs = sourceAbbrs;
        }

        public bool ExecuteQuery()
        {
            var hibSvcsCheck = _db.MstrAgcySources.FirstOrDefault(x => x.agency == Agency && !SourceAbbrs.Contains(x.SourceAbbr));

            return hibSvcsCheck == null;
        }
    }
}
