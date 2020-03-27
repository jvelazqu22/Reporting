using System;
using System.Linq;
using Domain.Helper;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Agency
{
    public class GetAgencyDotNetSettingsQuery : IQuery<AgencyDotNetSettings>
    {
        private readonly IMastersQueryable _db;
        private readonly string _agency;

        public GetAgencyDotNetSettingsQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            _agency = agency;
        }

        public AgencyDotNetSettings ExecuteQuery()
        {
            using (_db)
            {
                var rec = _db.ClientExtras.FirstOrDefault(x => x.ClientCode.Trim().Equals(_agency, StringComparison.OrdinalIgnoreCase)
                                                            && x.FieldFunction.Trim().Equals("DOT_NET_RPTSVR", StringComparison.OrdinalIgnoreCase));

                return rec == null 
                    ? new AgencyDotNetSettings(_agency, isDotNetEnabled: false, isSharingAgency: false) 
                    : new AgencyDotNetSettings(rec);
            }
        }
    }
}
