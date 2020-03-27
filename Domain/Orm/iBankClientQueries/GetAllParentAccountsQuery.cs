using System.Collections.Generic;
using System.Linq;

using Domain.Orm.Classes;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
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
                                                          AccountName = s.parentdesc.Trim(),
                                                          Agency = s.agency.Trim()                                                        
                                                      }).ToList();
            }
        }
    }
}
