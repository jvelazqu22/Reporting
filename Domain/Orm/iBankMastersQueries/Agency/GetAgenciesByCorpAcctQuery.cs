using System;
using System.Collections.Generic;
using System.Linq;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;
using iBank.Entities.MasterEntities;

namespace Domain.Orm.iBankMastersQueries.Agency
{
    public class GetAgenciesByCorpAcctQuery : IQuery<IList<CorpAcctNbrs>>
    {
        private readonly IMastersQueryable _db;

        private readonly string _corpAcct;

        public GetAgenciesByCorpAcctQuery(IMastersQueryable db, string corpAcct)
        {
            _db = db;
            _corpAcct = corpAcct.Trim();
        }

        public IList<CorpAcctNbrs> ExecuteQuery()
        {
            using (_db)
            {
                return _db.CorpAcctNbrs.Where(x => x.CorpAcct.Trim().Equals(_corpAcct, StringComparison.OrdinalIgnoreCase))
                                        .Distinct()
                                        .ToList();
            }
        }

    }
}
