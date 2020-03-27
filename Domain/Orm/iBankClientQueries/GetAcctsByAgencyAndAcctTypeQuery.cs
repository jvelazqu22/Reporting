using System.Collections.Generic;
using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetAcctsByAgencyAndAcctTypeQuery : IQuery<IList<string>>
    {
        public enum AcctType
        {
            Standard = 1,
            Custom = 2
        }

        public int AccountType { get; set; }

        public string Agency { get; set; }

        private readonly IClientQueryable _db;

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

        public IList<string> ExecuteQuery()
        {
            return _db.AcctMast.Where(x => x.agency == Agency && x.acctType == AccountType)
                .Select(x => x.acct.Trim())
                .Distinct()
                .ToList();
        }
    }
}
