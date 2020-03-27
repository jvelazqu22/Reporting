using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetBroadcastInfoByCorpAcctQuery :BaseiBankMastersQuery<MstrCorpAccts>
    {
        public string Agency { get; set; }

        public GetBroadcastInfoByCorpAcctQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        public override MstrCorpAccts ExecuteQuery()
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
