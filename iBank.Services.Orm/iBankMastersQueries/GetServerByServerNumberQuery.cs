using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetServerByServerNumberQuery : BaseiBankMastersQuery<iBankServers>
    {
        public int ServerNumber { get; set; }

        public GetServerByServerNumberQuery(IMastersQueryable db, int serverNumber)
        {
            _db = db;
            ServerNumber = serverNumber;
        }

        public override iBankServers ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBankServers.FirstOrDefault(x => x.ServerNbr == ServerNumber);
            }
        }
    }
}
