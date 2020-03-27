using System;
using System.Linq;

using Domain.Exceptions;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankAdministrationQueries
{
    public class IsFeatureFlagOnQuery : IQuery<bool>
    {
        private readonly IAdministrationQueryable _db;

        private readonly int _serverNumber;

        private readonly string _feature;

        public IsFeatureFlagOnQuery(IAdministrationQueryable db, Feature feature, int serverNumber)
        {
            _db = db;
            _serverNumber = serverNumber;
            _feature = feature.ToString();
        }

        //for unit testing
        public IsFeatureFlagOnQuery(IAdministrationQueryable db, string feature, int serverNumber)
        {
            _db = db;
            _serverNumber = serverNumber;
            _feature = feature;
        }

        public bool ExecuteQuery()
        {
            using (_db)
            {
                var flag = _db.FeatureFlag.FirstOrDefault(x => x.toggle_name.Equals(_feature, StringComparison.OrdinalIgnoreCase));

                if (flag == null) throw new FeatureFlagDoesNotExistException($"Expected feature flag {_feature} does not exist.");

                //get broadcast server numbers that are stage servers
                var bcstStageServers = from bs in _db.BroadcastServers
                                        join bsf in _db.BroadcastServerFunction on bs.server_function_id equals bsf.id
                                        where bsf.server_function.Equals("STAGE", StringComparison.OrdinalIgnoreCase)
                                        select bs.server_number;

                if (bcstStageServers.Any()) if (bcstStageServers.Contains(_serverNumber)) return flag.toggle_on_stage;

                //get report server numbers that are stage servers
                var rptStageServers = from rs in _db.ReportServers
                                        join rsf in _db.ReportServerFunction on rs.server_function_id equals rsf.id
                                        where rsf.server_function.Equals("STAGE", StringComparison.OrdinalIgnoreCase)
                                        select rs.server_number;

                if(rptStageServers.Any()) if (rptStageServers.Contains(_serverNumber)) return flag.toggle_on_stage;
                
                //if it isn't a stage server just check for normal flag toggle
                return flag.toggle_on;
            }
        }
    }
}
