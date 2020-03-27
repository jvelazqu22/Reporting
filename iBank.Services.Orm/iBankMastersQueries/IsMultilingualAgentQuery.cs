using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class IsMultilingualAgentQuery : BaseiBankMastersQuery<bool>
    {
        public string Agency { get; set; }

        public IsMultilingualAgentQuery(IMastersQueryable db, string agency)
        {
            _db = db;
        }
        public override bool ExecuteQuery()
        {
            using (_db)
            {
                var multilingual = _db.ClientExtras.FirstOrDefault(x => x.ClientCode == Agency && x.FieldFunction == "MULTILINGUAL");

                return multilingual != null && multilingual.FieldData == "YES";
            }
        }
    }
}
