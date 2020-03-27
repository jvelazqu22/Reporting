using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetAllAccountsByParentAcctQuery : BaseiBankClientQueryable<IList<string>>
    {
        public IList<string> ParentAcct { get; set; }

        public GetAllAccountsByParentAcctQuery(IClientQueryable db, string parentAcct)
        {
            _db = db;
            ParentAcct = new List<string> { parentAcct };
        }

        public GetAllAccountsByParentAcctQuery(IClientQueryable db, IList<string> parentAccts)
        {
            _db = db;
            ParentAcct = parentAccts;
        }
        ~GetAllAccountsByParentAcctQuery()
        {
            _db.Dispose();
        }

        public override IList<string> ExecuteQuery()
        {
            var parentAccount = ParentAcct[0];
            return ParentAcct.Count == 1
                ? _db.AcctMast.Where(x => x.parentacct.Trim() == parentAccount).Select(x => x.acct.Trim()).Distinct().ToList()
                : _db.AcctMast.Where(x => ParentAcct.Contains(x.parentacct)).Select(x => x.acct.Trim()).Distinct().ToList();
        }
    }
}
