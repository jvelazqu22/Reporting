using System.Collections.Generic;
using System.Linq;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetUserLanguageQuery : BaseiBankMastersQuery<IList<LanguageVariableInfo>>
    {
        public int ProcessKey { get; set; }
        public string UserLanguage { get; set; }
        public int UserNumber { get; set; }
        public string Agency { get; set; }

        public GetUserLanguageQuery(IMastersQueryable db, int processKey, string userLanguage, int userNumber, string agency)
        {
            _db = db;
            ProcessKey = processKey;
            UserLanguage = userLanguage;
            UserNumber = userNumber;
            Agency = agency;
        }

        public override IList<LanguageVariableInfo> ExecuteQuery()
        {
            using (_db)
            {
                return _db.ibFuncLangTags.Where(s =>
                                                (s.FunctionLink == 9999 || s.FunctionLink == ProcessKey) &&
                                                (s.TagUsage == "A" || s.TagUsage == "R"))
                    .Join(_db.LanguageTags, f => f.TagLink, t => t.TagNo, (f, t) => new
                                                                                        {
                                                                                            f.TagLink,
                                                                                            t.VarName,
                                                                                            t.NbrLines
                                                                                        })
                    .Join(_db.UserTranslations.Where(s => s.langcode == UserLanguage && s.usernumber == UserNumber &&
                                                          s.agency == Agency), f => f.TagLink, t => t.taglink, (f, t) => new LanguageVariableInfo
                                                                                                                             {
                                                                                                                                 VariableName = f.VarName.Trim(),
                                                                                                                                 NumberOfLines = f.NbrLines,
                                                                                                                                 Translation = t.translatn.Trim()
                                                                                                                             }).ToList();
            }
        }
    }
}
