using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetVerbiageByProcessKeyAndLanguageQuery : BaseiBankMastersQuery<ibProcVerbiage>
    {
        public int ProcessKey { get; set; }
        public string UserLanguage { get; set; }

        public GetVerbiageByProcessKeyAndLanguageQuery(IMastersQueryable db, int processKey, string userLanguage)
        {
            _db = db;
            ProcessKey = processKey;
            UserLanguage = userLanguage;
        }
        public override ibProcVerbiage ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBProcVerbiage.FirstOrDefault(x => x.processkey == ProcessKey & x.LangCode == UserLanguage);
            }
        }
    }
}
