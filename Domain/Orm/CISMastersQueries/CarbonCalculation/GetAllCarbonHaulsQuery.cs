using System.Collections.Generic;
using System.Linq;

using iBank.Entities.CISMasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.CISMastersQueries.CarbonCalculation
{
    public class GetAllCarbonHaulsQuery : IQuery<IList<CarbonCalculationHaul>>
    {
        private readonly ICisMastersQueryable _db;

        public GetAllCarbonHaulsQuery(ICisMastersQueryable db)
        {
            _db = db;
        }

        public IList<CarbonCalculationHaul> ExecuteQuery()
        {
            using (_db)
            {
                //null check solely for unit testing - Moq gets mad if not there for some reason
                return _db.CarbonCalculationHaul.Where(x => x != null).ToList();
            }
        }
    }
}
