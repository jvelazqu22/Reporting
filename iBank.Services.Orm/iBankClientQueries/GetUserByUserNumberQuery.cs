using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetUserByUserNumberQuery : BaseiBankClientQueryable<ibuser>
    {
        public int UserNumber { get; set; }

        public GetUserByUserNumberQuery(IClientQueryable db, int userNumber)
        {
            _db = db;
            UserNumber = userNumber;
        }

        public override ibuser ExecuteQuery()
        {
            using(_db)
            {
                return _db.iBUser.FirstOrDefault(x => x.UserNumber == UserNumber);
            }
        }
    }
}
