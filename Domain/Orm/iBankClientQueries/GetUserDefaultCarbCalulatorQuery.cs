using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetUserDefaultCarbCalculatorQuery : IQuery<string>
    {
        public int UserNumber { get; set; }
        private readonly IClientQueryable _db;

        public GetUserDefaultCarbCalculatorQuery(IClientQueryable db, int userNumber)
        {
            _db = db;
            UserNumber = userNumber;
        }

        public string ExecuteQuery()
        {
            using (_db)
            {
                var font = _db.iBUserExtra.FirstOrDefault(x => x.UserNumber == UserNumber
                                                           && x.FieldFunction.ToUpper().Trim() == "CARBONCALC_DEFAULT");

                return font == null ? "" : font.FieldData.Trim().ToUpper();
            }
        }
    }
}
