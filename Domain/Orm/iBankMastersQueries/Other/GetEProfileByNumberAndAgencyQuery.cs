using System;
using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetEProfileByNumberAndAgencyQuery : IQuery<eProfiles>
    {
        public string Agency { get; set; }
        public int EProfileNumber { get; set; }
        private readonly IMastersQueryable _db;

        public GetEProfileByNumberAndAgencyQuery(IMastersQueryable db, string agency, int eProfileNumber)
        {
            _db = db;
            Agency = agency;
            EProfileNumber = eProfileNumber;
        }

        public eProfiles ExecuteQuery()
        {
            using (_db)
            {
                return _db.EProfiles.FirstOrDefault(s => s.eProfileNo == EProfileNumber &&
                        s.agency.Equals(Agency, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
