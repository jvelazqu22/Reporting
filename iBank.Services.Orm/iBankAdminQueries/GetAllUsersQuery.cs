using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;
using iBank.Services.Orm.iBankClientQueries;

namespace iBank.Services.Orm.iBankAdminQueries
{
    public class GetAllUsersQuery : BaseiBankClientQueryable<IList<ibuser>>
    {
        public GetAllUsersQuery(IClientQueryable db)
        {
            _db = db;
        }

        public override IList<ibuser> ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBUser.ToList();
            }
        }
    }
}
