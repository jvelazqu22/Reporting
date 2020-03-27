using System.Linq;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetBroadcastInfoByCorpAcctQuery : IQuery<MstrCorpAccts>
    {
        public string Agency { get; set; }
        private readonly IMastersQueryable _db;

        public GetBroadcastInfoByCorpAcctQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        public MstrCorpAccts ExecuteQuery()
        {
            using (_db)
            {
                return _db.MstrCorpAccts.FirstOrDefault(s => s.CorpAcct.Trim().ToUpper() == Agency.ToUpper() 
                                                            && s.bcsenderemail != null 
                                                            && s.bcsendername != null);
            }
        }
    }
}
