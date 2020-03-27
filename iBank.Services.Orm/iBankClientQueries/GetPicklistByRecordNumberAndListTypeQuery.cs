using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetPicklistByRecordNumberAndListTypeQuery : BaseiBankClientQueryable<IList<PickListRow>>
    {
        public int RecordNumber { get; set; }
        public string ListType { get; set; }

        public GetPicklistByRecordNumberAndListTypeQuery(IClientQueryable db, int recordNumber, string listType)
        {
            _db = db;
            RecordNumber = recordNumber;
            ListType = listType;
        }

        public override IList<PickListRow> ExecuteQuery()
        {
            using (_db)
            {
                return _db.PickList.Where(x => x.recordnum == RecordNumber && x.listtype == ListType)
                    .Select(x => new PickListRow
                                     {
                                         RecordNum = x.recordnum,
                                         ListName = x.listname,
                                         ListType = x.listtype,
                                         UserNumber = x.usernumber,
                                         Agency = x.agency,
                                         ListData = x.listdata
                                     }).ToList();
            }
        }
    }
}
