using Domain.Interfaces;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetAllParentAccountsQuery : IQuery<IList<MasterAccountInformation>>
    {
        IClientQueryable _db;
        public GetAllParentAccountsQuery(IClientQueryable db)
        {
            _db = db;
        }

        public IList<MasterAccountInformation> ExecuteQuery()
        {
            using (_db)
            {
                return _db.AcctParent.Select(s => new MasterAccountInformation
                                                      {
                                                          AccountId = s.parentacct.Trim(),
                                                          AccountName = s.parentdesc.Trim()
                                                      }).ToList();
            }
        }
    }
}
