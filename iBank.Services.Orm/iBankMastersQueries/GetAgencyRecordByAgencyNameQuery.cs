using Domain.Interfaces;
using iBank.Services.Orm.Databases.Interfaces;
using System;
using System.Linq;

namespace iBank.Services.Orm.iBankMastersQueries
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
