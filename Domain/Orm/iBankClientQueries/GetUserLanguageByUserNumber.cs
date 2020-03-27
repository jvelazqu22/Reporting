using System;
using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetUserLanguageByUserNumber : IQuery<ibUserExtras>
    {
        public int UserNumber { get; set; }
        private readonly IClientQueryable _db;

        public GetUserLanguageByUserNumber(IClientQueryable db, int userNumber)
        {
            _db = db;
            UserNumber = userNumber;
        }

        public ibUserExtras ExecuteQuery()
        {
            using (_db)
            {
                return
                    _db.iBUserExtra.FirstOrDefault(
                        x => x.UserNumber == UserNumber && x.FieldFunction.Trim().Equals("USERLANGUAGE", StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
