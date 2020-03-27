using System.Linq;

using Domain.Orm.Classes;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetBroadcastEmailMasterInfoQuery : IQuery<BroadcastEmailMasterInfo>
    {
        public string Agency { get; set; }
        private readonly IMastersQueryable _db;

        public GetBroadcastEmailMasterInfoQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        public BroadcastEmailMasterInfo ExecuteQuery()
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
