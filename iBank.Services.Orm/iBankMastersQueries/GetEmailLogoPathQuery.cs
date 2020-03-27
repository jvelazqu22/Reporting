using System;
using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetEmailLogoPathQuery : BaseiBankMastersQuery<EmailLogoPathInfo>
    {
        public GetEmailLogoPathQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public override EmailLogoPathInfo ExecuteQuery()
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
