using System.Collections.Generic;
using System.Linq;

using Domain.Orm.Classes;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetAllClientsTlsQuery : IQuery<IList<ClientsTLInformation>>
    {
        private readonly IClientQueryable _db;
        public GetAllClientsTlsQuery(IClientQueryable db)
        {
            _db = db;
        }

        public IList<ClientsTLInformation> ExecuteQuery()
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
