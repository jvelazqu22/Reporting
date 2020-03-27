using Domain.Helper;
using Domain.Interfaces;
using iBank.Services.Orm.Databases.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Orm.iBankClientQueries.Udids
{
    public class GetUdidsByReckeyAndUdidNumbersQuery : IQuery<IList<UdidRecord>>
    {
        private IEnumerable<int> RecKeys { get; set; }

        private IEnumerable<int> UdidNumbers { get; set; }
        private bool IsReservationData { get; set; }

        private IClientQueryable _db;

        public GetUdidsByReckeyAndUdidNumbersQuery(IClientQueryable clientQueryDb, IEnumerable<int> recKeys, IEnumerable<int> udidNumbers, bool isReservationData)
        {
            _db = clientQueryDb;
            RecKeys = recKeys;
            UdidNumbers = udidNumbers;
            IsReservationData = isReservationData;
        }
        public IList<UdidRecord> ExecuteQuery()
        {
            using (_db)
            {
                if (IsReservationData)
                {
                    return _db.ibUdid.Where(x => RecKeys.Contains(x.reckey)
                                              && x.udidno.HasValue
                                              && UdidNumbers.Contains(x.udidno.Value))
                    .Select(x => new UdidRecord
                    {
                        RecKey = x.reckey,
                        UdidNumber = (int)x.udidno,
                        UdidText = x.udidtext
                    }).ToList();
                }
                else
                {
                    return _db.HibUdid.Where(x => RecKeys.Contains(x.reckey)
                                              && x.udidno.HasValue
                                              && UdidNumbers.Contains(x.udidno.Value))
                    .Select(x => new UdidRecord
                    {
                        RecKey = x.reckey,
                        UdidNumber = (int)x.udidno,
                        UdidText = x.udidtext
                    }).ToList();
                }
            }
        }
    }
}
