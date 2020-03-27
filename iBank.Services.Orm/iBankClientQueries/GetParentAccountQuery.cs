using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetParentAccountQuery : BaseiBankClientQueryable<acctmast>
    {
        public string Acct { get; set; }

        public GetParentAccountQuery(IClientQueryable db, string acct)
        {
            _db = db;
            Acct = acct;
        }

        public override acctmast ExecuteQuery()
        {
            using (_db)
            {
                return _db.AcctMast.FirstOrDefault(x => x.acct == Acct);
            }
        }
    }
}
