using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetBroadcastEmailMasterInfoQuery : BaseiBankMastersQuery<BroadcastEmailMasterInfo>
    {
        public string Agency { get; set; }

        public GetBroadcastEmailMasterInfoQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        public override BroadcastEmailMasterInfo ExecuteQuery()
        {
            using (_db)
            {
                var mstr = _db.MstrAgcy.FirstOrDefault(s => s.agency.ToUpper().Trim().Equals(Agency)
                                                            && !string.IsNullOrEmpty(s.bcsenderemail.Trim()) && !string.IsNullOrEmpty(s.bcsendername));

                if (mstr == null) return new BroadcastEmailMasterInfo();

                return new BroadcastEmailMasterInfo
                           {
                               SenderEmail = string.IsNullOrEmpty(mstr.bcsenderemail.Trim()) ? "" : mstr.bcsenderemail.Trim(),
                               SenderName = string.IsNullOrEmpty(mstr.bcsendername.Trim()) ? "" : mstr.bcsendername.Trim()
                           };
            }
        }
    }
}
