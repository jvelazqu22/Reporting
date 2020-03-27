using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetAllChainsQuery : BaseiBankMastersQuery<IList<ChainsInformation>>
    {
        public GetAllChainsQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public override IList<ChainsInformation> ExecuteQuery()
        {
            using(_db)
            {
                return _db.Chains.Select(s => new ChainsInformation
                                                  {
                                                      ChainCode = s.chaincode,
                                                      ChainClass = s.chainClass,
                                                      ChainDescription = s.chaindesc,
                                                      ChainParent = s.chParent
                                                  }).ToList();
            }
        }
    }
}
