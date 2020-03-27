using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class IsCurrencyConversionOnFixedDateQuery : BaseiBankMastersQuery<bool>
    {
        public string Agency { get; set; }

        public IsCurrencyConversionOnFixedDateQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        public override bool ExecuteQuery()
        {
            using (_db)
            {
                var currConv = _db.ClientExtras.FirstOrDefault(x => x.ClientCode == Agency && x.FieldFunction == "CURRCONVFIXEDDATE");
                return currConv != null && currConv.FieldData == "YES";
            }
        }
    }
}
