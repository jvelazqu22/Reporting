using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetAllChainsQuery : IQuery<IList<ChainsInformation>>
    {
        private readonly IMastersQueryable _db;

        public GetAllChainsQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<ChainsInformation> ExecuteQuery()
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
