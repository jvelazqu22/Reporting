using System;
using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetBroadcastRecordsByBatchNumberAndDatabaseQuery : BaseiBankMastersQuery<IList<bcstque4>>
    {
        public int? BatchNumber { get; set; }
        public string DatabaseName { get; set; }

        public GetBroadcastRecordsByBatchNumberAndDatabaseQuery(IMastersQueryable db, int? batchNumber, string databaseName)
        {
            _db = db;
            BatchNumber = batchNumber;
            DatabaseName = databaseName;
        }

        public override IList<bcstque4> ExecuteQuery()
        {
            using (_db)
            {
                return _db.BcstQue4.Where(x => x.batchnum == BatchNumber
                                               && x.dbname.Trim().Equals(DatabaseName, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }
    }
}
