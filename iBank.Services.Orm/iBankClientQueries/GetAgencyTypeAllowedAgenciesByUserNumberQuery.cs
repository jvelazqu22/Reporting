using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetAgencyTypeAllowedAgenciesByUserNumberQuery : BaseiBankClientQueryable<IList<string>>
    {
        public int UserNumber { get; set; }

        public GetAgencyTypeAllowedAgenciesByUserNumberQuery(IClientQueryable db, int userNumber)
        {
            _db = db;
            UserNumber = userNumber;
        }

        ~GetAgencyTypeAllowedAgenciesByUserNumberQuery()
        {
            _db.Dispose();
        }

        public override IList<string> ExecuteQuery()
        {
            return _db.UserSource.Where(x => x.UserNumber == UserNumber).Select(x => x.SourceAbbr).ToList();
        }
    }
}
