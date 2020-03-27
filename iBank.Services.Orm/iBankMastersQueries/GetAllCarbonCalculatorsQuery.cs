using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetAllCarbonCalculatorsQuery : BaseiBankMastersQuery<IList<CarbonCalculatorInformation>>
    {
        public GetAllCarbonCalculatorsQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public override IList<CarbonCalculatorInformation> ExecuteQuery()
        {
            using (_db)
            {
                return _db.CarbonCalculators.Select(x => new CarbonCalculatorInformation
                {
                    Id = x.CarbCalcID.ToString().Trim(),
                    Name = x.CarbCalcName.ToString().Trim(),
                    Html = x.CarbCalcHTML,
                    Link = x.CarbCalcLink
                }).ToList();
            }
        }
    }
}
