using Domain.Interfaces;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Orm.iBankMastersQueries
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
