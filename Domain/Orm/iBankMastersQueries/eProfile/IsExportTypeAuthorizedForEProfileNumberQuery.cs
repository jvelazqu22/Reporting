using System;
using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.eProfile
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
