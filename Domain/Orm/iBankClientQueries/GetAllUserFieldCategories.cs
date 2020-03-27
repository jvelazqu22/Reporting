using System.Collections.Generic;
using System.Linq;

using Domain.Orm.Classes;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetAllUserFieldCategories : IQuery<IList<UserFieldCategory>>
    {
        private readonly IClientQueryable _db;

        public GetAllUserFieldCategories(IClientQueryable db)
        {
            _db = db;
        }

        public IList<UserFieldCategory> ExecuteQuery()
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
