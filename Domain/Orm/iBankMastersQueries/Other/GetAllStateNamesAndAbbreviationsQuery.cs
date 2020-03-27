using System.Collections.Generic;
using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetAllStateNamesAndAbbreviationsQuery : IQuery<IList<state_names>>
    {
        private readonly IMastersQueryable _db;

        public GetAllStateNamesAndAbbreviationsQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<state_names> ExecuteQuery()
        {
            using (_db)
            {
                return _db.StateNames.ToList();
            }
        }
    }
}
