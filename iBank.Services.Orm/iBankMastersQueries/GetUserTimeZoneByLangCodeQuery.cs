using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetUserTimeZoneByLangCodeQuery : BaseiBankMastersQuery<TimeZones>
    {
        public string LanguageCode { get; set; }
        public string UserTimeZone { get; set; }

        public GetUserTimeZoneByLangCodeQuery(IMastersQueryable db, string userTimeZone, string languageCode)
        {
            _db = db;
            UserTimeZone = userTimeZone.Trim();
            LanguageCode = languageCode.Trim();
        }

        public override TimeZones ExecuteQuery()
        {
            using (_db)
            {
                return _db.TimeZones.FirstOrDefault(x => x.TimeZoneCode.Trim().Equals(UserTimeZone) && x.LangCode.Trim().Equals(LanguageCode));
            }
        }
    }
}
