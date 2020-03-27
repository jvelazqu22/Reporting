using System.Collections.Generic;
using System.Linq;

using Domain.Orm.Classes;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetBroadcastLanguageTranslationsQuery : IQuery<IList<LanguageVariableInfo>>
    {
        public string BroadcastLanguage { get; set; }
        private readonly IMastersQueryable _db;

        public GetBroadcastLanguageTranslationsQuery(IMastersQueryable db, string broadcastLanguage)
        {
            _db = db;
            BroadcastLanguage = broadcastLanguage;
        }

        public IList<LanguageVariableInfo> ExecuteQuery()
        {
            using (_db)
            {
                return _db.ibFuncLangTags.Where(x => x.FunctionLink == 9801).Join(_db.LanguageTags, f => f.TagLink, t => t.TagNo, (f, t) => new
                {
                    f.TagLink,
                    t.VarName,
                    t.NbrLines
                }).Join( _db.LanguageTranslations.Where( x => x.LangCode == BroadcastLanguage),
                    f => f.TagLink, t => t.TagLink, (f, t) => new LanguageVariableInfo
                        {
                            VariableName = f.VarName.Trim(),
                            NumberOfLines = f.NbrLines,
                            Translation = t.Translatn.Trim()
                        })
                    .ToList();
            }
        }
    }
}
