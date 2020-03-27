using Domain.Interfaces;
using iBank.Services.Orm.Databases.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetAllAirlinesQuery : IQuery<IList<airlines>>
    {
        private IMastersQueryable _db;
        public GetAllAirlinesQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<airlines> ExecuteQuery()
        {
            using (_db)
            {
                return _db.Airlines.OrderBy(x => x.airlinenbr).ToList();
            }
        }
    }
}
