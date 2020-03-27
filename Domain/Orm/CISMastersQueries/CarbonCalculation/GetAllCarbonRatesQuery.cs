using System.Collections.Generic;
using System.Linq;

using iBank.Entities.CISMasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.CISMastersQueries.CarbonCalculation
{
    public class GetAllCarbonRatesQuery : IQuery<IList<CarbonCalculationRate>>
    {
        private readonly ICisMastersQueryable _db;

        public GetAllCarbonRatesQuery(ICisMastersQueryable db)
        {
            _db = db;
        }

        public IList<CarbonCalculationRate> ExecuteQuery()
        {
            using (_db)
            {
                //null check solely for unit testing - Moq gets mad if not there for some reason
                return _db.CarbonCalculationRate.Where(x => x != null).ToList();
            }
        }
    }
}
