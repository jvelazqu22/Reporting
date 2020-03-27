using System;
using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetStyleGroupByNumberAndClientCodeQuery : IQuery<StyleGroup>
    {
        private readonly IClientQueryable _db;
        private int StyleGroupNumber { get; }
        private string Agency { get; }

        public GetStyleGroupByNumberAndClientCodeQuery(IClientQueryable db, int styleGroupNumber, string agency)
        {
            _db = db;
            StyleGroupNumber = styleGroupNumber;
            Agency = agency.Trim();
        }

        public StyleGroup ExecuteQuery()
        {
            using (_db)
            {
                return _db.StyleGroup.FirstOrDefault(x => x.SGroupNbr == StyleGroupNumber 
                                                        && x.ClientCode.Trim().Equals(Agency, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
