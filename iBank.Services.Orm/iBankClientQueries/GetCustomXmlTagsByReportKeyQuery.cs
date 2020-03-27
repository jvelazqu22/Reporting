using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetCustomXmlTagsByReportKeyQuery : BaseiBankClientQueryable<IList<XmlTag>>
    {
        public int ReportKey { get; set; }
        public GetCustomXmlTagsByReportKeyQuery(IClientQueryable db, int reportKey)
        {
            _db = db;
            ReportKey = reportKey;
        }

        public override IList<XmlTag> ExecuteQuery()
        {
            using (_db)
            {
                return _db.XmlUserRpt2.Where(s => s.reportkey.Equals(ReportKey))
                    .Select(s => new XmlTag
                    {
                        TagName = s.xmltagname.Trim(),
                        AlternateTagName = s.dispname.Trim(),
                        Mask = s.mask == null ? false : s.mask.Value,
                        TagType = s.xmltagtype.Trim(),
                        IsOn = s.@switch.ToUpper().Trim().Equals("ON")
                    }).ToList();
            }
        }
    }
}
