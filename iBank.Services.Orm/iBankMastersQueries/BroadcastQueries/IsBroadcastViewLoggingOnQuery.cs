using System;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries.BroadcastQueries
{
    public class IsBroadcastViewLoggingOnQuery : BaseiBankMastersQuery<bool>
    {
        public string Agency { get; set; }

        public IsBroadcastViewLoggingOnQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        public override bool ExecuteQuery()
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
