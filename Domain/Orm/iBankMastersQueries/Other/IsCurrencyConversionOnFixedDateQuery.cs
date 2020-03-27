using System.Linq;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class IsCurrencyConversionOnFixedDateQuery : IQuery<bool>
    {
        public string Agency { get; set; }
        private readonly IMastersQueryable _db;

        public IsCurrencyConversionOnFixedDateQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        public bool ExecuteQuery()
        {
            using (_db)
            {
                var currConv = _db.ClientExtras.FirstOrDefault(x => x.ClientCode == Agency && x.FieldFunction == "CURRCONVFIXEDDATE");
                return currConv != null && currConv.FieldData == "YES";
            }
        }
    }
}
