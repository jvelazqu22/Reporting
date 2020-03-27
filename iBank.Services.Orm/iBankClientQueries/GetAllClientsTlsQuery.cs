using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetAllClientsTlsQuery : BaseiBankClientQueryable<IList<ClientsTLInformation>>
    {
        public GetAllClientsTlsQuery(IClientQueryable db)
        {
            _db = db;
        }

        public override IList<ClientsTLInformation> ExecuteQuery()
        {
            using (_db)
            {
                return _db.ClientsTl.Select(s => new ClientsTLInformation
                                                     {
                                                         RecordNo = s.recordNo,
                                                         Agency = s.agency,
                                                         ClientId = s.clientID,
                                                         ClientName = s.clientName
                                                     }).ToList();
            }
        }
    }
}
