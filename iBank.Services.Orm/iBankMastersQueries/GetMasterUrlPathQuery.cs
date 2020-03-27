using System;
using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetMasterUrlPathQuery : BaseiBankMastersQuery<MasterUrlInfo>
    {
        public GetMasterUrlPathQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public override MasterUrlInfo ExecuteQuery()
        {
            using (_db)
            {
                var pathInfo = new MasterUrlInfo();

                var parm = _db.MiscParams.FirstOrDefault(x => x.parmcat.Trim().Equals("IBANKURL", StringComparison.OrdinalIgnoreCase));

                pathInfo.UrlPath = parm == null || string.IsNullOrEmpty(
                    parm.parmdesc)
                    ? @"http://localhost/ibankV4/"
                    : parm.parmdesc.Trim();

                return pathInfo;
            }
        }
    }
}
