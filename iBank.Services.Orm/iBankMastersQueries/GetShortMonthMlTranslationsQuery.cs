using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetShortMonthMlTranslationsQuery : BaseiBankMastersQuery<IList<string>>
    {
        public string LanguageCode { get; set; }

        public GetShortMonthMlTranslationsQuery(IMastersQueryable db, string languageCode)
        {
            _db = db;
            LanguageCode = languageCode;
        }

        public override IList<string> ExecuteQuery()
        {
            using (_db)
            {
                return _db.LanguageTags.Where(s => s.VarName.Equals("lt_AbbrMthsofYear"))
                    .Join(_db.LanguageTranslations.Where(s => s.LangCode.Equals(LanguageCode)),
                        tags => tags.TagNo, languageTranslations => languageTranslations.TagLink,
                        (t, l) => l.Translatn).ToList();
            }
        }
    }
}
