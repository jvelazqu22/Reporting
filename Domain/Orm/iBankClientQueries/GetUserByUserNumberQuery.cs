using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetUserByUserNumberQuery : IQuery<ibuser>
    {
        public int UserNumber { get; set; }
        private IClientQueryable _db;

        public GetUserByUserNumberQuery(IClientQueryable db, int userNumber)
        {
            _db = db;
            UserNumber = userNumber;
        }

        public ibuser ExecuteQuery()
        {
            using(_db)
            {
                return _db.iBUser.FirstOrDefault(x => x.UserNumber == UserNumber);
            }
        }
    }
}
