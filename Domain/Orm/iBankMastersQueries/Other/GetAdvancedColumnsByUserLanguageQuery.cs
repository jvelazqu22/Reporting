using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetAdvancedColumnsByUserLanguageQuery : IQuery<List<AdvancedColumnInformation>>
    {
        public string LanguageCode { get; set; }
        private readonly IMastersQueryable _db;

        public GetAdvancedColumnsByUserLanguageQuery(IMastersQueryable db, string languageCode)
        {
            _db = db;
            LanguageCode = languageCode.Trim();
        }

        public List<AdvancedColumnInformation> ExecuteQuery()
        {
            using (_db)
            {
                return _db.Collist2.Join(_db.Collist2Captions.Where(s => s.LangCode == LanguageCode),
                    l => l.colname, c => c.ColName,
                    (l, c) => new AdvancedColumnInformation
                                  {
                                      ColName = l.colname.Trim(),
                                      AdvancedColName = l.advcolname.Trim(),
                                      ColType = l.coltype.Trim(),
                                      ColTable = l.coltable.Trim(),
                                      IsLookup = l.islookup,
                                      LookupFunction = l.fnlookup.Trim(),
                                      Usage = l.usage.Trim(),
                                      BigName = c.BigName.Trim()
                                  }).ToList();
            }
        }
    }
}
