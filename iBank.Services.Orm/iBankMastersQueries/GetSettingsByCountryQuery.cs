using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetSettingsByCountryQuery : BaseiBankMastersQuery<intlparm>
    {
        public string Country { get; set; }

        public GetSettingsByCountryQuery(IMastersQueryable db, string country)
        {
            _db = db;
            Country = country;
        }

        public override intlparm ExecuteQuery()
        {
            using (_db)
            {
                return _db.IntlParm.FirstOrDefault(x => x.country.Trim() == Country.ToUpper().Trim());
            }
        }
    }
}
