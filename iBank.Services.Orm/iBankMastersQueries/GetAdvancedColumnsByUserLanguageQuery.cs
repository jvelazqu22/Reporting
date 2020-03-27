using System.Collections.Generic;
using System.Linq;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetAdvancedColumnsByUserLanguageQuery : BaseiBankMastersQuery<List<AdvancedColumnInformation>>
    {
        public string LanguageCode { get; set; }

        public GetAdvancedColumnsByUserLanguageQuery(IMastersQueryable db, string languageCode)
        {
            _db = db;
            LanguageCode = languageCode.Trim();
        }

        public override List<AdvancedColumnInformation> ExecuteQuery()
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
