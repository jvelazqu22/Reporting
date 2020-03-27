using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetUserTimeZoneByLangCodeQuery : IQuery<TimeZones>
    {
        public string LanguageCode { get; set; }
        public string UserTimeZone { get; set; }
        private readonly IMastersQueryable _db;

        public GetUserTimeZoneByLangCodeQuery(IMastersQueryable db, string userTimeZone, string languageCode)
        {
            _db = db;
            UserTimeZone = userTimeZone.Trim();
            LanguageCode = languageCode.Trim();
        }

        public TimeZones ExecuteQuery()
        {
            using (_db)
            {
                return _db.TimeZones.FirstOrDefault(x => x.TimeZoneCode.Trim().Equals(UserTimeZone) && x.LangCode.Trim().Equals(LanguageCode));
            }
        }
    }
}
