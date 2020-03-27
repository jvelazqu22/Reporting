using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetCurrencySettingsByMoneyTypeQuery : BaseiBankMastersQuery<curcountry>
    {
        public string MoneyType { get; set; }

        public GetCurrencySettingsByMoneyTypeQuery(IMastersQueryable db, string moneyType)
        {
            _db = db;
            MoneyType = moneyType;
        }

        public override curcountry ExecuteQuery()
        {
            using (_db)
            {
                return _db.CurCountry.FirstOrDefault(x => x.curcode.Trim() == MoneyType);
            }
        }
    }
}
