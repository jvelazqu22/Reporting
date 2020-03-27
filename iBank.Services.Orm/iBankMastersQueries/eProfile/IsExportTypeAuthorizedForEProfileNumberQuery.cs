using Domain.Interfaces;
using iBank.Services.Orm.Databases.Interfaces;
using System;
using System.Linq;

namespace iBank.Services.Orm.iBankMastersQueries.eProfile
{
    public class IsExportTypeAuthorizedForEProfileNumberQuery : IQuery<bool>
    {
        private readonly IMastersQueryable _db;
        private string ExportType { get; }
        private int EProfileNumber { get; }

        public IsExportTypeAuthorizedForEProfileNumberQuery(IMastersQueryable db, string exportType, int eProfileNumber)
        {
            _db = db;
            ExportType = exportType.Trim();
            EProfileNumber = eProfileNumber;
        }

        public bool ExecuteQuery()
        {
            using (_db)
            {
                var rec = _db.EProfExpTypes.FirstOrDefault(x => x.eProfileNo == EProfileNumber
                                                                && x.ExportType.Trim().Equals(ExportType, StringComparison.OrdinalIgnoreCase));

                return rec != null;
            }
        }
    }
}
