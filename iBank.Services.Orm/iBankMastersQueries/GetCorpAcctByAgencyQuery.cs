using Domain.Interfaces;
using iBank.Services.Orm.Databases.Interfaces;
using System;
using System.Linq;

namespace iBank.Services.Orm.iBankMastersQueries
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
