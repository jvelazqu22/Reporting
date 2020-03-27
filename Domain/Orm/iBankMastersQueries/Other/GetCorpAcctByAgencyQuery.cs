using System;
using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetCorpAcctByAgencyQuery : IQuery<MstrCorpAccts>
    {
        public string Agency { get; set; }

        private readonly IMastersQueryable _db;

        public GetCorpAcctByAgencyQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency.Trim();
        }

        public MstrCorpAccts ExecuteQuery()
        {
            using (_db)
            {
                return _db.MstrCorpAccts.FirstOrDefault(x => x.CorpAcct.Equals(Agency, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
