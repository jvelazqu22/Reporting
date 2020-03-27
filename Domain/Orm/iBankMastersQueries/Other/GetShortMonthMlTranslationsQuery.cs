using System.Collections.Generic;
using System.Linq;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetShortMonthMlTranslationsQuery : IQuery<IList<string>>
    {
        public string LanguageCode { get; set; }
        private readonly IMastersQueryable _db;

        public GetShortMonthMlTranslationsQuery(IMastersQueryable db, string languageCode)
        {
            _db = db;
            LanguageCode = languageCode;
        }

        public IList<string> ExecuteQuery()
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
