using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetHomeCountriesByAgencyForSharerClientQuery : IQuery<IList<KeyValue>>
    {
        private IMastersQueryable _db;
        private bool _disposed = false;
        public string Agency { get; set; }

        public GetHomeCountriesByAgencyForSharerClientQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        ~GetHomeCountriesByAgencyForSharerClientQuery()
        {
            if (!_disposed) _db.Dispose();
        }

        public IList<KeyValue> ExecuteQuery()
        {
            _disposed = true;
            using (_db)
            {
                return _db.MstrAgcySources.Join(_db.JunctionAgcyCorp
                    .Where(s => s.CorpAcct.Equals(Agency)), m => m.agency, j => j.agency, (m, j) => new KeyValue
                                                                                                        {
                                                                                                            Key = m.SourceAbbr,
                                                                                                            Value = m.HomeCtry
                                                                                                        }).ToList();
            }
        }
    }
}
