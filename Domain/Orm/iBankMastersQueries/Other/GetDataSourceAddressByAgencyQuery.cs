using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetDataSourceAddressByAgencyQuery : IQuery<IList<DataSource>>
    {
        private readonly IMastersQueryable _db;

        private readonly IList<string> _agencies;

        public GetDataSourceAddressByAgencyQuery(IMastersQueryable db, IList<string> agencies)
        {
            _db = db;
            _agencies = agencies;
        }

        public IList<DataSource> ExecuteQuery()
        {
            using (_db)
            {
                return _db.MstrAgcy.Where(x => _agencies.Contains(x.agency))
                                       .Join(_db.iBankDatabases, x => x.databasename, y => y.databasename, (x, y) =>
                                                 new DataSource
                                                 {
                                                     Agency = x.agency.Trim(),
                                                     DatabaseName = y.databasename.Trim(),
                                                     ServerAddress = y.server_address.Trim()
                                                 })
                                                 .OrderBy(x=>x.DatabaseName)
                                                 .ToList();
            }
        }
    }
}
