using System;
using System.Linq;
using iBankDomain.RepositoryInterfaces;
using iBank.Repository.SQL.Interfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class IsSqlLoggingOnForUserQuery : IQuery<bool>
    {
        private readonly IClientQueryable _db;

        private readonly int _userNumber;

        private readonly string _agency;

        public IsSqlLoggingOnForUserQuery(IClientQueryable db, int userNumber, string agency)
        {
            _db = db;
            _userNumber = userNumber;
            _agency = agency.Trim();
        }

        public bool ExecuteQuery()
        {
            using (_db)
            {
                var rec = _db.iBUserExtra.FirstOrDefault(x => x.agency.Trim().Equals(_agency, StringComparison.OrdinalIgnoreCase)
                                                              && x.UserNumber == _userNumber
                                                              && x.FieldFunction.Equals("SQL_LOGGING_ON"));

                return rec != null;
            }
        }
    }
}
