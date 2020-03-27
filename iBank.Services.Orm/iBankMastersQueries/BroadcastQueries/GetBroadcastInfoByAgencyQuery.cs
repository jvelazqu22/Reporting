using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetBroadcastInfoByAgencyQuery : BaseiBankMastersQuery<mstragcy>
    {
        public string Agency { get; set; }
        public GetBroadcastInfoByAgencyQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        public override mstragcy ExecuteQuery()
        {
            using (_db)
            {
                return _db.MstrAgcy.FirstOrDefault(s => s.agency.Trim().ToUpper() == Agency.ToUpper() 
                                                    && s.bcsenderemail != null 
                                                    && s.bcsendername != null);
            }
        }
    }
}
