using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetMasterLanguageTagsByProcessKeyAndOutputLangQuery : IQuery<List<LanguageVariableInfo>>
    {
        public int ProcessKey { get; set; }
        public string OutputLanguage { get; set; }
        private readonly IMastersQueryable _db;

        public GetMasterLanguageTagsByProcessKeyAndOutputLangQuery(IMastersQueryable db, int processKey, string outputLanguage)
        {
            _db = db;
            ProcessKey = processKey;
            OutputLanguage = outputLanguage;
        }

        public List<LanguageVariableInfo> ExecuteQuery()
        {
            using(_db)
            {
                return _db.ibFuncLangTags.Where(s =>
                                                (s.FunctionLink == 9999 || s.FunctionLink == ProcessKey) &&
                                                (s.TagUsage == "B" || s.TagUsage == "R"))
                    .Join(_db.LanguageTags, f => f.TagLink, t => t.TagNo, (f, t) => new
                                                                                        {
                                                                                            f.TagLink,
                                                                                            t.VarName,
                                                                                            t.NbrLines,
                                                                                            t.TagType
                                                                                        })
                    .Join(_db.LanguageTranslations.Where(s => s.LangCode == OutputLanguage),
                        f => f.TagLink, t => t.TagLink, (f, t) => new LanguageVariableInfo
                                                                      {
                                                                          VariableName = f.VarName.Trim(),
                                                                          NumberOfLines = f.NbrLines,
                                                                          Translation = t.Translatn.Trim(),
                                                                          TagType = f.TagType.Trim()
                                                                      }).ToList();
            }
        }
    }
}
