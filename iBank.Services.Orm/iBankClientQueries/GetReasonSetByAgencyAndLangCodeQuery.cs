using System.Collections.Generic;
using System.Linq;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetReasonSetByAgencyAndLangCodeQuery : BaseiBankClientQueryable<IList<ReasonSetInfo>>
    {
        public string Agency { get; set; }
        public string LanguageCode { get; set; }
        public GetReasonSetByAgencyAndLangCodeQuery(IClientQueryable db, string agency, string languageCode)
        {
            _db = db;
            Agency = agency;
            LanguageCode = languageCode;
        }

        public override IList<ReasonSetInfo> ExecuteQuery()
        {
            using (_db)
            {
                return _db.ReasonSet.Where(x => x.agency.Trim() == Agency && x.DefLang.Trim() == LanguageCode)
                    .Select(x => new ReasonSetInfo
                    {
                        Agency = x.agency.Trim(),
                        DefLang = x.DefLang.Trim(),
                        ReasSetDesc = x.ReasSetDesc,
                        ReasSetNbr = x.ReasSetNbr
                    }).ToList();
            }
        }
    }
}
