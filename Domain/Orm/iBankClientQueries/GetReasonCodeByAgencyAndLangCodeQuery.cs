using System.Collections.Generic;
using System.Linq;

using Domain.Orm.Classes;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetReasonCodeByAgencyAndLangCodeQuery : IQuery<IList<ReasonCode>>
    {
        public string Agency { get; set; }
        public string LanguageCode { get; set; }

        private readonly IClientQueryable _db;

        public GetReasonCodeByAgencyAndLangCodeQuery(IClientQueryable db, string agency, string languageCode)
        {
            _db = db;
            Agency = agency;
            LanguageCode = languageCode;
        }

        public IList<ReasonCode> ExecuteQuery()
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
