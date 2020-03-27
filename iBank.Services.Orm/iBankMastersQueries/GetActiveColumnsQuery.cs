using Domain.Interfaces;
using iBank.Services.Orm.Databases.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetActiveColumnsQuery : IQuery<IList<collist2>>
    {
        private IMastersQueryable _db;
        public GetActiveColumnsQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<collist2> ExecuteQuery()
        {
            using (_db)
            {
                return _db.Collist2.Where(s => !s.inactive).ToList();
            }
        }
    }
}
