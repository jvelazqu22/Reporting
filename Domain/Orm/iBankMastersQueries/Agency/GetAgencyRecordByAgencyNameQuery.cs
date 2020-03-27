using System;
using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Agency
{
    public class GetAgencyRecordByAgencyNameQuery : IQuery<mstragcy>
    {
        public string Agency { get; set; }

        private readonly IMastersQueryable _db;

        public GetAgencyRecordByAgencyNameQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency.Trim();
        }

        public mstragcy ExecuteQuery()
        {
            using (_db)
            {
                return _db.MstrAgcy.FirstOrDefault(x => x.agency.Equals(Agency, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
