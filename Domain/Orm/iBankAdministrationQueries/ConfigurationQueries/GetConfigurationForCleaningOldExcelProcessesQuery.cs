using System;
using System.Linq;
using Domain.Exceptions;
using iBank.Entities.AdministrationEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankAdministrationQueries.ConfigurationQueries
{
    public class GetConfigurationForCleaningOldExcelProcessesQuery : IQuery<ReportingConfiguration>
    {
        private readonly IAdministrationQueryable _db;
        private static readonly int _cacheExpirationInSec = 60;
        private string _configurationName = string.Empty;
        public GetConfigurationForCleaningOldExcelProcessesQuery(IAdministrationQueryable db, string configurationName)
        {
            _db = db;
            _configurationName = configurationName;
        }

        public ReportingConfiguration ExecuteQuery()
        {
            var config = _db.ReportingConfiguration.FirstOrDefault(x => x.Name.Trim().Equals(_configurationName, StringComparison.OrdinalIgnoreCase));
            _db.Dispose();

            if (config == null) throw new InvalidDatabaseConfigurationException("Missing " + _configurationName + " from the Configurations database");

            return config;
        }
    }
}
