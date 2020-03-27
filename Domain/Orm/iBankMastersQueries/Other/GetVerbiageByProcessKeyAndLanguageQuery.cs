using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetVerbiageByProcessKeyAndLanguageQuery : IQuery<ibProcVerbiage>
    {
        public int ProcessKey { get; set; }
        public string UserLanguage { get; set; }
        private readonly IMastersQueryable _db;

        public GetVerbiageByProcessKeyAndLanguageQuery(IMastersQueryable db, int processKey, string userLanguage)
        {
            _db = db;
            ProcessKey = processKey;
            UserLanguage = userLanguage;
        }
        public ibProcVerbiage ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBProcVerbiage.FirstOrDefault(x => x.processkey == ProcessKey & x.LangCode == UserLanguage);
            }
        }
    }
}
