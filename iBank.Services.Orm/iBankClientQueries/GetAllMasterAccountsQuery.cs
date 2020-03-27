using Domain.Interfaces;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetAllMasterAccountsQuery : IQuery<IList<MasterAccountInformation>>
    {
        protected IClientQueryable _db;
        public string Agency { get; set; }

        public GetAllMasterAccountsQuery(IClientQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        ~GetAllMasterAccountsQuery()
        {
            _db.Dispose();
        }

        public IList<MasterAccountInformation> ExecuteQuery()
        {
            return _db.AcctMast.Where(s => s.agency.Trim().Equals(Agency))
                .Select(s => new MasterAccountInformation
            {
                AccountId = s.acct.Trim(),
                AccountName = s.acctname.Trim(),
                Agency = s.agency.Trim(),
                ParentAccount = s.parentacct.Trim(),
                ReasSetNbr = s.ReasSetNbr,
                AcctCat1 = s.AcctCat1,
                AcctCat2 = s.AcctCat2,
                AcctCat3 = s.AcctCat3,
                AcctCat4 = s.AcctCat4,
                AcctCat5 = s.AcctCat5,
                AcctCat6 = s.AcctCat6,
                AcctCat7 = s.AcctCat7,
                AcctCat8 = s.AcctCat8,
                Address1 = s.acctaddr1.Trim(),
                Address2 = s.acctaddr2.Trim(),
                Address3 = s.acctaddr3.Trim(),
                Address4 = s.acctaddr4.Trim()
            }).ToList();
        }
    }
}
