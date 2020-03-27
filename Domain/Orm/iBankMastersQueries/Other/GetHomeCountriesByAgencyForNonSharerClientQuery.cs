using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetHomeCountriesByAgencyForNonSharerClientQuery : IQuery<IList<KeyValue>>
    {
        private IMastersQueryable _db;
        private bool _disposed = false;

        public string Agency { get; set; }

        public GetHomeCountriesByAgencyForNonSharerClientQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        ~GetHomeCountriesByAgencyForNonSharerClientQuery()
        {
            if (!_disposed) _db.Dispose();
        }

        public IList<KeyValue> ExecuteQuery()
        {
            _disposed = true;
            using (_db)
            {
                return _db.MstrAgcySources.Where(x => x.agency.Equals(Agency)).Select(x => new KeyValue
                                                                                               {
                                                                                                   Key = x.SourceAbbr,
                                                                                                   Value = x.HomeCtry
                                                                                               }).ToList();
            }
        }
    }
}
