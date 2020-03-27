using System.Linq;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class IsMultilingualAgentQuery : IQuery<bool>
    {
        public string Agency { get; set; }
        private readonly IMastersQueryable _db;

        public IsMultilingualAgentQuery(IMastersQueryable db, string agency)
        {
            _db = db;
        }
        public bool ExecuteQuery()
        {
            using (_db)
            {
                var multilingual = _db.ClientExtras.FirstOrDefault(x => x.ClientCode == Agency && x.FieldFunction == "MULTILINGUAL");

                return multilingual != null && multilingual.FieldData == "YES";
            }
        }
    }
}
