using System.Collections.Generic;
using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetAgencyTypeAllowedAgenciesByUserNumberQuery : IQuery<IList<string>>
    {
        public int UserNumber { get; set; }

        private readonly IClientQueryable _db;

        public GetAgencyTypeAllowedAgenciesByUserNumberQuery(IClientQueryable db, int userNumber)
        {
            _db = db;
            UserNumber = userNumber;
        }

        ~GetAgencyTypeAllowedAgenciesByUserNumberQuery()
        {
            _db.Dispose();
        }

        public IList<string> ExecuteQuery()
        {
            return _db.UserSource.Where(x => x.UserNumber == UserNumber).Select(x => x.SourceAbbr).ToList();
        }
    }
}
