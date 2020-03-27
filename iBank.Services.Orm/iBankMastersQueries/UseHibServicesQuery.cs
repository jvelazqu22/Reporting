using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class UseHibServicesQuery : BaseiBankMastersQuery<bool>
    {
        public string Agency { get; set; }
        public IList<string> SourceAbbrs { get; set; }

        public UseHibServicesQuery(IMastersQueryable db, string agency, IList<string> sourceAbbrs)
        {
            _db = db;
            Agency = agency;
            SourceAbbrs = sourceAbbrs;
        }

        public override bool ExecuteQuery()
        {
            var hibSvcsCheck = _db.MstrAgcySources.FirstOrDefault(x => x.agency == Agency && !SourceAbbrs.Contains(x.SourceAbbr));

            return hibSvcsCheck == null;
        }
    }
}
