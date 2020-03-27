using System;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetEmailLogoPathQuery : IQuery<EmailLogoPathInfo>
    {
        private readonly IMastersQueryable _db;

        public GetEmailLogoPathQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public EmailLogoPathInfo ExecuteQuery()
        {
            using (_db)
            {
                var pathInfo = new EmailLogoPathInfo();

                var physPath = _db.MiscParams.FirstOrDefault(x => x.parmcat.Trim().Equals("EMAIL_LOGO", StringComparison.OrdinalIgnoreCase)
                                                   && x.parmcode.Trim().Equals("PHYS_PATH", StringComparison.OrdinalIgnoreCase));

                var urlPath = _db.MiscParams.FirstOrDefault(x => x.parmcat.Trim().Equals("EMAIL_LOGO", StringComparison.OrdinalIgnoreCase)
                                                   && x.parmcode.Trim().Equals("URL", StringComparison.OrdinalIgnoreCase));

                pathInfo.PhysicalPath = physPath == null ? "" : physPath.parmdesc.Trim();
                pathInfo.UrlPath = urlPath == null ? "" : urlPath.parmdesc.Trim();

                return pathInfo;
            }
        }
    }
}
