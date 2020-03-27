using System.Collections.Generic;
using System.Linq;

using Domain.Orm.Classes;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetAllMasterAccountsQuery : IQuery<IList<MasterAccountInformation>>
    {
        private readonly IClientQueryable _db;
        public string Agency { get; set; }

        public GetAllMasterAccountsQuery(IClientQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        ~GetAllMasterAccountsQuery()
        {
            _db?.Dispose();
        }

        public IList<MasterAccountInformation> ExecuteQuery()
        {
#if DEBUG
            return DebugExecuteQuery();
#endif
            return _db.AcctMast.Where(s => s.agency.Trim().Equals(Agency))
                .Select(s => new MasterAccountInformation
                {
                    AccountId = string.IsNullOrEmpty(s.acct) ? "" : s.acct.Trim(),
                    AccountName = string.IsNullOrEmpty(s.acctname) ? "" : s.acctname.Trim(),
                    Agency = s.agency.Trim(),
                    ParentAccount = string.IsNullOrEmpty(s.parentacct) ? "" : s.parentacct.Trim(),
                    ReasSetNbr = s.ReasSetNbr,
                    AcctCat1 = s.AcctCat1,
                    AcctCat2 = s.AcctCat2,
                    AcctCat3 = s.AcctCat3,
                    AcctCat4 = s.AcctCat4,
                    AcctCat5 = s.AcctCat5,
                    AcctCat6 = s.AcctCat6,
                    AcctCat7 = s.AcctCat7,
                    AcctCat8 = s.AcctCat8,
                    Address1 = string.IsNullOrEmpty(s.acctaddr1) ? "" : s.acctaddr1.Trim(),
                    Address2 = string.IsNullOrEmpty(s.acctaddr2) ? "" : s.acctaddr2.Trim(),
                    Address3 = string.IsNullOrEmpty(s.acctaddr3) ? "" : s.acctaddr3.Trim(),
                    Address4 = string.IsNullOrEmpty(s.acctaddr4) ? "" : s.acctaddr4.Trim(),
                }).ToList();
        }

        public IList<MasterAccountInformation> DebugExecuteQuery()
        {
            var masterAccounts = _db.AcctMast.ToList();

            return masterAccounts.Where(s => s.agency.Trim().Equals(Agency))
                .Select(s => new MasterAccountInformation
                {
                    AccountId = string.IsNullOrEmpty(s.acct) ? "" : s.acct.Trim(),
                    AccountName = string.IsNullOrEmpty(s.acctname) ? "" : s.acctname.Trim(),
                    Agency = s.agency.Trim(),
                    ParentAccount = string.IsNullOrEmpty(s.parentacct) ? "" : s.parentacct.Trim(),
                    ReasSetNbr = s.ReasSetNbr,
                    AcctCat1 = s.AcctCat1,
                    AcctCat2 = s.AcctCat2,
                    AcctCat3 = s.AcctCat3,
                    AcctCat4 = s.AcctCat4,
                    AcctCat5 = s.AcctCat5,
                    AcctCat6 = s.AcctCat6,
                    AcctCat7 = s.AcctCat7,
                    AcctCat8 = s.AcctCat8,
                    Address1 = string.IsNullOrEmpty(s.acctaddr1) ? "" : s.acctaddr1.Trim(),
                    Address2 = string.IsNullOrEmpty(s.acctaddr2) ? "" : s.acctaddr2.Trim(),
                    Address3 = string.IsNullOrEmpty(s.acctaddr3) ? "" : s.acctaddr3.Trim(),
                    Address4 = string.IsNullOrEmpty(s.acctaddr4) ? "" : s.acctaddr4.Trim(),
                }).ToList();
        }
    }
}
