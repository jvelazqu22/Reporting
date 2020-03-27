using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetMasterXmlTagsByReportKeyQuery : BaseiBankMastersQuery<IList<XmlTag>>
    {
        public int ReportKey { get; set; }
        public GetMasterXmlTagsByReportKeyQuery(IMastersQueryable db, int reportKey)
        {
            _db = db;
            ReportKey = reportKey;
        }

        public override IList<XmlTag> ExecuteQuery()
        {
            using (_db)
            {
                return _db.XmlRpt2.Where(s => s.reportkey.Equals(ReportKey))
                    .Select(s => new XmlTag
                    {
                        TagName = s.xmltagname.Trim(),
                        AlternateTagName = s.dispname.Trim(),
                        Mask = s.mask,
                        TagType = s.xmltagtype.Trim(),
                        IsOn = s.@switch.ToUpper().Trim().Equals("ON")
                    }).ToList();
            }
        }
    }
}
