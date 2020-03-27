using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Orm.Classes;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetActiveBroadcastAgenciesByAgencyQuery : IQuery<IList<AgencyInformation>>
    {
        public string Agency { get; set; }

        private readonly IMastersQueryable _db;

        public GetActiveBroadcastAgenciesByAgencyQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency.Trim();
        }

        public IList<AgencyInformation> ExecuteQuery()
        {
            using (_db)
            {
                return _db.MstrAgcy.Where(s => s.bcactive.HasValue 
                                                && s.bcactive.Value 
                                                && s.agency.Trim().Equals(Agency, StringComparison.OrdinalIgnoreCase))
                                .Select(s => new AgencyInformation
                                {
                                    Agency = s.agency.Trim().ToUpper(),
                                    DatabaseName = s.databasename.Trim().ToLower(),
                                    Active = s.active,
                                    BcActive = s.bcactive.HasValue && s.bcactive.Value,
                                    TimeZoneOffset = s.tzoffset ?? 0
                                }).ToList();
            }
        }
    }
}
