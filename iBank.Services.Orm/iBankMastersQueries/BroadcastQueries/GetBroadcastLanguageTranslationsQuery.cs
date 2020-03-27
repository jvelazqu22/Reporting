using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetBroadcastLanguageTranslationsQuery : BaseiBankMastersQuery<IList<LanguageVariableInfo>>
    {
        public string BroadcastLanguage { get; set; }

        public GetBroadcastLanguageTranslationsQuery(IMastersQueryable db, string broadcastLanguage)
        {
            _db = db;
            BroadcastLanguage = broadcastLanguage;
        }

        public override IList<LanguageVariableInfo> ExecuteQuery()
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
