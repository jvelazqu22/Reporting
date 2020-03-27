using System;
using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Configuration
{
    public class IsAgencyStagedForReportQuery : IQuery<bool>
    {
        private readonly IMastersQueryable _db;

        private readonly int _processKey;

        private readonly string _agency;

        public IsAgencyStagedForReportQuery(IMastersQueryable db, int processKey, string agency)
        {
            _db = db;
            _processKey = processKey;
            _agency = agency.Trim();
        }

        public bool ExecuteQuery()
        {
            using (_db)
            {
                //first look to see if the agency is on stage by itself
                var agencyRecordExists = _db.ReportRolloutStage.Any(x => x.agency != null
                                                                     && x.agency.Equals(_agency, StringComparison.OrdinalIgnoreCase)
                                                                     && x.currently_staged
                                                                     && x.process_key == _processKey);

                if (agencyRecordExists) return true;

                //now look and see if the agency is part of a database that is staged
                var databasesOnStage = _db.ReportRolloutStage.Where(x => x.currently_staged
                                                                              && !string.IsNullOrEmpty(x.database_name)).Select(x => x.database_name).ToList();

                if (!databasesOnStage.Any()) return false;

                //get what database the agency belongs to
                var databaseForAgency = _db.MstrAgcy.Where(x => x.agency.Equals(_agency, StringComparison.OrdinalIgnoreCase)).Select(x => x.databasename).FirstOrDefault();

                return (databasesOnStage.Contains(databaseForAgency));
            }
        }
    }
}
