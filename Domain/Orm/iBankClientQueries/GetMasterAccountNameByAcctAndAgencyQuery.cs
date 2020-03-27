using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetMasterAccountNameByAcctAndAgencyQuery : IQuery<string>
    {
        public string Agency { get; set; }
        public string Acct { get; set; }

        private readonly IClientQueryable _db;

        public GetMasterAccountNameByAcctAndAgencyQuery(IClientQueryable db, string agency, string acct)
        {
            _db = db;
            Agency = agency.Trim().ToUpper();
            Acct = acct.Trim().ToUpper();
        }

        public string ExecuteQuery()
        {
            using (_db)
            {
                var acctName = _db.AcctMast.FirstOrDefault(x => x.agency.Trim().ToUpper().Equals(Agency)
                                                                && x.acct.Trim().ToUpper().Equals(Acct));

                return acctName == null ? "" : acctName.acctname;
            }
        }
    }
}
