using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetAcctsByAgencyAndAcctTypeQuery : BaseiBankClientQueryable<IList<string>>
    {
        public enum AcctType
        {
            Standard = 1,
            Custom = 2
        }

        public int AccountType { get; set; }

        public string Agency { get; set; }

        public GetAcctsByAgencyAndAcctTypeQuery(IClientQueryable db, AcctType acctType, string agency)
        {
            _db = db;
            AccountType = (int)acctType;
            Agency = agency;
        }

        ~GetAcctsByAgencyAndAcctTypeQuery()
        {
            _db.Dispose();
        }

        public override IList<string> ExecuteQuery()
        {
            return _db.AcctMast.Where(x => x.agency == Agency && x.acctType == AccountType)
                .Select(x => x.acct.Trim())
                .Distinct()
                .ToList();
        }
    }
}
