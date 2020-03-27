using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetErrorMessageQuery : IQuery<string>
    {
        private readonly IMastersQueryable _db;

        private readonly int _recordNumber;

        public GetErrorMessageQuery(IMastersQueryable db, int recordNumber)
        {
            _db = db;
            _recordNumber = recordNumber;
        }

        public string ExecuteQuery()
        {
            using (_db)
            {
                var rec = _db.ErrorLog.FirstOrDefault(x => x.recordno == _recordNumber);

                return rec != null ? rec.errormsg : "";
            }
        }
    }
}
