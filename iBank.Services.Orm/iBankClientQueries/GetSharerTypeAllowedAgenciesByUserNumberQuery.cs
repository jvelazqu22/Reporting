using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetSharerTypeAllowedAgenciesByUserNumberQuery : BaseiBankClientQueryable<IList<string>>
    {
        public int UserNumber { get; set; }

        public GetSharerTypeAllowedAgenciesByUserNumberQuery(IClientQueryable db, int userNumber)
        {
            _db = db;
            UserNumber = userNumber;
        }
    
        public override IList<string> ExecuteQuery()
        {
            using (_db)
            {
                return _db.UserSource.Where(x => x.UserNumber == UserNumber).Select(x => x.Agency).ToList();
            }
        }
    }
}
