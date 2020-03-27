using System.Collections.Generic;
using System.Linq;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetAllReportTypesQuery : BaseiBankMastersQuery<List<IntKeyValue>>
    {
        public GetAllReportTypesQuery(IMastersQueryable db)
        {
            _db = db;
        }
        public override List<IntKeyValue> ExecuteQuery()
        {
            using(_db)
            {
                return _db.iBProcess.Select(s => new IntKeyValue
                                                     {
                                                         Key = s.processkey,
                                                         Value = s.rpttype.Trim()
                                                     }).ToList();
            }
        }
    }
}
