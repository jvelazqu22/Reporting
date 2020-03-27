using System;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetDotNetAgentByAgencyQuery : BaseiBankMastersQuery<ClientExtras>
    {
        public string Agency { get; set; }

        public GetDotNetAgentByAgencyQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }
        
        public override ClientExtras ExecuteQuery()
        {
            using(_db)
            {
                return _db.ClientExtras.FirstOrDefault(x => x.ClientCode.Trim().Equals(Agency, StringComparison.OrdinalIgnoreCase)
                                                            && x.FieldFunction.Trim().Equals("DOT_NET_RPTSVR", StringComparison.OrdinalIgnoreCase)
                                                            && x.ClientType.Trim().Equals("A", StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
