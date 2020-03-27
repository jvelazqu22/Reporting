using System.Collections.Generic;
using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetSharerTypeAllowedAgenciesByUserNumberQuery : IQuery<IList<string>>
    {
        public int UserNumber { get; set; }
        private readonly IClientQueryable _db;

        public GetSharerTypeAllowedAgenciesByUserNumberQuery(IClientQueryable db, int userNumber)
        {
            _db = db;
            UserNumber = userNumber;
        }
    
        public IList<string> ExecuteQuery()
        {
            using (_db)
            {
                return _db.UserSource.Where(x => x.UserNumber == UserNumber).Select(x => x.Agency).ToList();
            }
        }
    }
}
