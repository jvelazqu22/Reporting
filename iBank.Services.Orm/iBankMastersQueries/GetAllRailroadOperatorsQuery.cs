using Domain.Interfaces;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Orm.iBankMastersQueries
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
