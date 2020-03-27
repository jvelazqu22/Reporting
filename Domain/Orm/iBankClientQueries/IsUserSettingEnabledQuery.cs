using System;
using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class IsUserSettingEnabledQuery : IQuery<bool>
    {
        public string Agency { get; set; }
        public int UserNumber { get; set; }
        public string FieldFunction { get; set; }
        private readonly IClientQueryable _db;

        public IsUserSettingEnabledQuery(IClientQueryable db, string agency, int userNumber, string fieldFunction)
        {
            _db = db;
            Agency = agency;
            UserNumber = userNumber;
            FieldFunction = fieldFunction;
        }

        public bool ExecuteQuery()
        {
            using (_db)
            {
                var extra = _db.iBUserExtra.FirstOrDefault(x => x.agency == Agency
                                                           && x.UserNumber == UserNumber
                                                           && x.FieldFunction.Trim().Equals(FieldFunction, StringComparison.OrdinalIgnoreCase)
                                                           && x.FieldData.Contains("YES"));

                return extra != null;
            }
        }
    }
}
