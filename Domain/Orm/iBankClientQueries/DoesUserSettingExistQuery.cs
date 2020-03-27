using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class DoesUserSettingExistQuery : IQuery<bool>
    {
        public string FieldFunction { get; set; }
        public int UserNumber { get; set; }
        public string Agency { get; set; }

        private readonly IClientQueryable _db;

        public DoesUserSettingExistQuery(IClientQueryable db, string fieldFunction, int userNumber, string agency)
        {
            _db = db;
            FieldFunction = fieldFunction;
            UserNumber = userNumber;
            Agency = agency;
        }

        ~DoesUserSettingExistQuery()
        {
            _db.Dispose();
        }

        public bool ExecuteQuery()
        {
            return _db.iBUserExtra.Any(x => x.UserNumber == UserNumber && x.agency.Trim() == Agency && x.FieldFunction.Trim() == FieldFunction);
        }
    }
}
