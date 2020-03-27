using System.Collections.Generic;
using System.Linq;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetTranslationsByLanguageCodeQuery : BaseiBankMastersQuery<IList<KeyValue>>
    {
        public string LanguageCode { get; set; }

        public GetTranslationsByLanguageCodeQuery(IMastersQueryable db, string languageCode)
        {
            _db = db;
            LanguageCode = languageCode;
        }

        public override IList<KeyValue> ExecuteQuery()
        {
            using (_db)
            {
                return _db.LanguageTranslations.Where(s => s.LangCode.Equals(LanguageCode)).Join(_db.LanguageTags, l => l.TagLink, t => t.TagNo,
                    (translations, tags) => new KeyValue
                                                {
                                                    Key = tags.ShortDesc.Trim(),
                                                    Value = translations.Translatn.Trim()
                                                }).ToList();
            }
        }
    }
}
