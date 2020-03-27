using System.Collections.Generic;
using System.Linq;
using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries.UserDefinedQueries
{
    public class GetHibTripsDerivedDataByRecKeyListQuery : IQuery<List<hibTripsDerivedData>>
    {
        IClientQueryable _db;
        private List<int> _recKeyList;

        public GetHibTripsDerivedDataByRecKeyListQuery(IClientQueryable db, List<int> recKeyList)
        {
            _db = db;
            _recKeyList = recKeyList;
        }

        public List<hibTripsDerivedData> ExecuteQuery()
        {
            var hibTripsDerivedDataList = new List<hibTripsDerivedData>();
            using (_db)
            {
                hibTripsDerivedDataList = _db.HibTripsDerivedData.ToList();
            }
            //hibTripsDerivedDataList = hibTripsDerivedDataList.Where(w => _recKeyList.Contains(w.reckey)).ToList();
            var results = (from hDerived in hibTripsDerivedDataList
                            join key in _recKeyList on hDerived.reckey equals key
                            select new hibTripsDerivedData
                            {
                                reckey = hDerived.reckey,
                                tripTransactionID = hDerived.tripTransactionID
                            }).ToList();

            return results;
        }
    }
}

