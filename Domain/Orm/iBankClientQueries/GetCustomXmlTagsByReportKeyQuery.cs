using System.Collections.Generic;
using System.Linq;

using Domain.Orm.Classes;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetCustomXmlTagsByReportKeyQuery : IQuery<IList<XmlTag>>
    {
        public int ReportKey { get; set; }

        private readonly IClientQueryable _db;

        public GetCustomXmlTagsByReportKeyQuery(IClientQueryable db, int reportKey)
        {
            _db = db;
            ReportKey = reportKey;
        }

        public IList<XmlTag> ExecuteQuery()
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
                        IsOn = s._switch.ToUpper().Trim().Equals("ON")
                    }).ToList();
            }
        }
    }
}
