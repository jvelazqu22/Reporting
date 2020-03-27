using System.Collections.Generic;
using System.Linq;

using Domain.Orm.Classes;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetPicklistByRecordNumberAndListTypeQuery : IQuery<IList<PickListRow>>
    {
        public int RecordNumber { get; set; }
        public string ListType { get; set; }

        private readonly IClientQueryable _db;

        public GetPicklistByRecordNumberAndListTypeQuery(IClientQueryable db, int recordNumber, string listType)
        {
            _db = db;
            RecordNumber = recordNumber;
            ListType = listType;
        }

        public IList<PickListRow> ExecuteQuery()
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
