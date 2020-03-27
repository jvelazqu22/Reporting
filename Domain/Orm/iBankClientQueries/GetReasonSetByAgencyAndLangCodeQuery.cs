using System.Collections.Generic;
using System.Linq;

using Domain.Orm.Classes;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetReasonSetByAgencyAndLangCodeQuery : IQuery<IList<ReasonSetInfo>>
    {
        public string Agency { get; set; }
        public string LanguageCode { get; set; }

        private readonly IClientQueryable _db;

        public GetReasonSetByAgencyAndLangCodeQuery(IClientQueryable db, string agency, string languageCode)
        {
            _db = db;
            Agency = agency;
            LanguageCode = languageCode;
        }

        public IList<ReasonSetInfo> ExecuteQuery()
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
