using System.Linq;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetBroadcastInfoByAgencyQuery : IQuery<mstragcy>
    {
        public string Agency { get; set; }
        private readonly IMastersQueryable _db;

        public GetBroadcastInfoByAgencyQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        public mstragcy ExecuteQuery()
        {
            using (_db)
            {
                return _db.MstrAgcy.FirstOrDefault(s => s.agency.Trim().ToUpper().Equals(Agency.ToUpper())
                                                    && s.bcsenderemail != null 
                                                    && s.bcsendername != null);
            }
        }
    }
}
