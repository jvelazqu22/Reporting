using System;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetEProfileByNumberAndAgencyQuery : BaseiBankMastersQuery<eProfiles>
    {
        public string Agency { get; set; }
        public int EProfileNumber { get; set; }

        public GetEProfileByNumberAndAgencyQuery(IMastersQueryable db, string agency, int eProfileNumber)
        {
            _db = db;
            Agency = agency;
            EProfileNumber = eProfileNumber;
        }

        public override eProfiles ExecuteQuery()
        {
            using (_db)
            {
                return _db.EProfiles.FirstOrDefault(s => s.eProfileNo == EProfileNumber &&
                        s.agency.Equals(Agency, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
