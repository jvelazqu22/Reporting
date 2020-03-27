using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetUserBreaks1ByUserNumberQuery : BaseiBankClientQueryable<IList<string>>
    {
        public int UserNumber { get; set; }

        public GetUserBreaks1ByUserNumberQuery(IClientQueryable db, int userNumber)
        {
            _db = db;
            UserNumber = userNumber;
        }

        public override IList<string> ExecuteQuery()
        {
            using(_db)
            {
                return _db.UserBrks1.Where(x => x.UserNumber == UserNumber).Select(x => x.break1).Distinct().ToList();
            }
        }
    }
}
