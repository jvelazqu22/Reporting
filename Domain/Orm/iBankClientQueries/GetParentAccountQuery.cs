using System.Linq;
using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetParentAccountQuery : IQuery<acctmast>
    {
        private string Acct { get; set; }
        private string Agency { get; set; }
        private readonly IClientQueryable _db;

        public GetParentAccountQuery(IClientQueryable db, string acct, string agency)
        {
            _db = db;
            Acct = acct;
            Agency = agency;
        }

        public acctmast ExecuteQuery()
        {
            using (_db)
            {
                return _db.AcctMast.FirstOrDefault(x => x.acct == Acct && x.agency == Agency);
            }
        }
    }
}
