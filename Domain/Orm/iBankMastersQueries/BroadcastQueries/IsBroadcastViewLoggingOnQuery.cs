using System;
using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries
{
    public class IsBroadcastViewLoggingOnQuery : IQuery<bool>
    {
        public string Agency { get; set; }
        private readonly IMastersQueryable _db;

        public IsBroadcastViewLoggingOnQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        public bool ExecuteQuery()
        {
            using (_db)
            {
                var logging = _db.ClientExtras.FirstOrDefault(x => x.ClientCode.Trim().Equals(Agency.Trim(), StringComparison.OrdinalIgnoreCase)
                                                             && x.FieldFunction.Trim().Equals("BCSTVIEWLOGGING", StringComparison.OrdinalIgnoreCase));

                return (logging != null) && logging.FieldData.Trim().ToUpper().Equals("YES");
            }
        }
    }
}
