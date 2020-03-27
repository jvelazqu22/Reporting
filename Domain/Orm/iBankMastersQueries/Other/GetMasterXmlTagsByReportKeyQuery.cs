using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetMasterXmlTagsByReportKeyQuery : IQuery<IList<XmlTag>>
    {
        public int ReportKey { get; set; }
        private readonly IMastersQueryable _db;

        public GetMasterXmlTagsByReportKeyQuery(IMastersQueryable db, int reportKey)
        {
            _db = db;
            ReportKey = reportKey;
        }

        public IList<XmlTag> ExecuteQuery()
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
                        IsOn = s._switch.ToUpper().Trim().Equals("ON")
                    }).ToList();
            }
        }
    }
}
