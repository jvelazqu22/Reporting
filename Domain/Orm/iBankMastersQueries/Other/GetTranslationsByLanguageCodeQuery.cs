using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetTranslationsByLanguageCodeQuery : IQuery<IList<KeyValue>>
    {
        public string LanguageCode { get; set; }
        private readonly IMastersQueryable _db;

        public GetTranslationsByLanguageCodeQuery(IMastersQueryable db, string languageCode)
        {
            _db = db;
            LanguageCode = languageCode;
        }

        public IList<KeyValue> ExecuteQuery()
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
