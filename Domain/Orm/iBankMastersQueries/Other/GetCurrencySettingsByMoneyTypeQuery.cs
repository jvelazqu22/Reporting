using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetCurrencySettingsByMoneyTypeQuery : IQuery<curcountry>
    {
        public string MoneyType { get; set; }
        private readonly IMastersQueryable _db;

        public GetCurrencySettingsByMoneyTypeQuery(IMastersQueryable db, string moneyType)
        {
            _db = db;
            MoneyType = moneyType;
        }

        public curcountry ExecuteQuery()
        {
            using (_db)
            {
                return _db.CurCountry.FirstOrDefault(x => x.curcode.Trim() == MoneyType);
            }
        }
    }
}
