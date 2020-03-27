using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetMasterAgencySourcesQuery : IQuery<IList<MasterSourceInformation>>
    {
        private IMastersQueryable _db;
        public GetMasterAgencySourcesQuery(IMastersQueryable db)
        {
            _db = db;
        }

        ~GetMasterAgencySourcesQuery()
        {
            _db.Dispose();
        }

        public IList<MasterSourceInformation> ExecuteQuery()
        {
            return _db.MstrAgcySources.Select(s => new MasterSourceInformation
                                                        {
                                                            SourceAbbreviation = s.SourceAbbr.Trim(),
                                                            SourceDescription = s.SourceDesc.Trim(),
                                                            Agency = s.agency.Trim()
                                                        }).ToList();
        }
    }
}
