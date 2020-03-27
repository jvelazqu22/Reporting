using System;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetMasterUrlPathQuery : IQuery<MasterUrlInfo>
    {
        private readonly IMastersQueryable _db;

        public GetMasterUrlPathQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public MasterUrlInfo ExecuteQuery()
        {
            using (_db)
            {
                var pathInfo = new MasterUrlInfo();

                var parm = _db.MiscParams.FirstOrDefault(x => x.parmcat.Trim().Equals("IBANKURL", StringComparison.OrdinalIgnoreCase));

                pathInfo.UrlPath = string.IsNullOrEmpty(parm?.parmdesc)
                    ? @"http://localhost/ibankV4/"
                    : parm.parmdesc.Trim();

                return pathInfo;
            }
        }
    }
}
