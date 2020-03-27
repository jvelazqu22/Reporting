using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetLanguageByLangCodeQuery : BaseiBankMastersQuery<Languages>
    {
        public string LanguageCode { get; set; }

        public GetLanguageByLangCodeQuery(IMastersQueryable db, string langCode)
        {
            _db = db;
            LanguageCode = langCode;
        }

        public override Languages ExecuteQuery()
        {
            using (_db)
            {
                return _db.Languages.FirstOrDefault(s => s.LangCode == LanguageCode);
            }
        }
    }
}
