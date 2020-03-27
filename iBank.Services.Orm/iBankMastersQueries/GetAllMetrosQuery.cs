using System.Collections.Generic;
using System.Linq;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetAllMetrosQuery : BaseiBankMastersQuery<IList<MetroInformation>>
    {
        public GetAllMetrosQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public override IList<MetroInformation> ExecuteQuery()
        {
            using(_db)
            {
                return _db.Metro.Select(s => new MetroInformation()
                                                 {
                                                     RecordNo = s.RecordNo,
                                                     MetroCode = s.metrocode,
                                                     MetroCity = s.metrocity,
                                                     MetroState = s.metrostate,
                                                     CountryCode = s.CtryCode
                                                 }).ToList();
            }
        }
    }
}
