using System;
using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries
{
    public class IsBroadcastLogOnSecurityOnQuery : IQuery<bool>
    {
        private readonly IMastersQueryable _db;

        private readonly string _agency;
        public IsBroadcastLogOnSecurityOnQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            _agency = agency.Trim();
        }

        public bool ExecuteQuery()
        {
            using (_db)
            {
                var security = _db.ClientExtras.FirstOrDefault(x => x.ClientCode.Trim().Equals(_agency, StringComparison.OrdinalIgnoreCase)
                                                             && x.FieldFunction.Trim().Equals("BCSTLOGONSECURITY", StringComparison.OrdinalIgnoreCase));

                return (security != null) && security.FieldData.Trim().ToUpper().Equals("YES");
            }
        }
    }
}
