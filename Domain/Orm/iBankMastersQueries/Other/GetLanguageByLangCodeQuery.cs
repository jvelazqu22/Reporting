using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetLanguageByLangCodeQuery : IQuery<Languages>
    {
        public string LanguageCode { get; set; }
        private readonly IMastersQueryable _db;

        public GetLanguageByLangCodeQuery(IMastersQueryable db, string langCode)
        {
            _db = db;
            LanguageCode = langCode;
        }

        public Languages ExecuteQuery()
        {
            using (_db)
            {
                return _db.Languages.FirstOrDefault(s => s.LangCode == LanguageCode);
            }
        }
    }
}
