using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetSiteDefaultLanguageQuery : BaseiBankMastersQuery<string>
    {
        public string Agency { get; set; }

        public GetSiteDefaultLanguageQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }
        public override string ExecuteQuery()
        {
            using (_db)
            {
                var defaultLang = _db.ClientExtras.FirstOrDefault(x => x.ClientCode == Agency && x.FieldFunction == "SITEDEFAULTLANGUAGE");

                return defaultLang == null ? "EN" : defaultLang.FieldData;
            }
        }
    }
}
