using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetReasonCodeByAgencyAndLangCodeQuery : BaseiBankClientQueryable<IList<ReasonCode>>
    {
        public string Agency { get; set; }
        public string LanguageCode { get; set; }

        public GetReasonCodeByAgencyAndLangCodeQuery(IClientQueryable db, string agency, string languageCode)
        {
            _db = db;
            Agency = agency;
            LanguageCode = languageCode;
        }

        public override IList<ReasonCode> ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBReasCd.Where(x => x.agency.Trim() == Agency && x.LangCode.Trim() == LanguageCode)
                    .Select(x => new ReasonCode
                                     {
                                         Agency = x.agency.Trim(),
                                         ReasCode = x.reascode.Trim(),
                                         ReasDesc = x.reasdesc.Trim(),
                                         ParentAcct = x.parentAcct.Trim(),
                                         ReasSetNbr = x.ReasSetNbr,
                                         LangCode = x.LangCode.Trim(),
                                         LongDesc = x.LongDesc.Trim(),
                                         ExtendDesc = x.ExtendDesc.Trim()
                                     }).ToList();
            }
        }
    }
}
