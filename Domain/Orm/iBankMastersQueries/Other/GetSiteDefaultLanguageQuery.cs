using System.Linq;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetSiteDefaultLanguageQuery : IQuery<string>
    {
        public string Agency { get; set; }
        private readonly IMastersQueryable _db;

        public GetSiteDefaultLanguageQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }
        public string ExecuteQuery()
        {
            using (_db)
            {
                var defaultLang = _db.ClientExtras.FirstOrDefault(x => x.ClientCode == Agency && x.FieldFunction == "SITEDEFAULTLANGUAGE");

                return defaultLang == null ? "EN" : defaultLang.FieldData;
            }
        }
    }
}
