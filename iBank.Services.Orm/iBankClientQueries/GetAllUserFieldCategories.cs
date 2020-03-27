using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetAllUserFieldCategories : BaseiBankClientQueryable<IList<UserFieldCategory>>
    {
        public GetAllUserFieldCategories(IClientQueryable db)
        {
            _db = db;
        }

        public override IList<UserFieldCategory> ExecuteQuery()
        {
            using (_db)
            {
                return _db.UserFieldCats.Select(s => new UserFieldCategory
                {
                    Key = s.UserFieldCatKey,
                    Description = s.CatDescription.Trim()
                }).ToList();
            }
        }
    }
}
