using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Orm.iBankMastersQueries.Agency
{
    public class GetAgenciesByJunctionAgcyCorpQuery : IQuery<IList<string>>
    {
        private readonly IMastersQueryable _db;

        private readonly string _corpAcctName;
        public GetAgenciesByJunctionAgcyCorpQuery(IMastersQueryable db, string corpAcctName)
        {
            _db = db;
            _corpAcctName = corpAcctName.Trim();
        }

        public IList<string> ExecuteQuery()
        {
            using (_db)
            {
                return _db.JunctionAgcyCorp.Where(x => x.CorpAcct.Trim().Equals(_corpAcctName, StringComparison.OrdinalIgnoreCase))
                                        .Select(x => x.agency.Trim())
                                        .Distinct()
                                        .ToList();
                
            }
        }
    }
}
