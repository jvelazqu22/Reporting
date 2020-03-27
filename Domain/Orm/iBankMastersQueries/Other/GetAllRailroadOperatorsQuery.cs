using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetAllRailroadOperatorsQuery : IQuery<IList<RailroadOperatorsInformation>>
    {
        private IMastersQueryable _db;
        public GetAllRailroadOperatorsQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<RailroadOperatorsInformation> ExecuteQuery()
        {
            using (_db)
            {
                return _db.RROperators.Select(x => new RailroadOperatorsInformation
                {
                    OperatorNumber = x.rrOperNbr,
                    OperatorCode = x.rrOperCode,
                    OperatorName = x.rrOperName
                }).ToList();
            }
        }
    }
}
