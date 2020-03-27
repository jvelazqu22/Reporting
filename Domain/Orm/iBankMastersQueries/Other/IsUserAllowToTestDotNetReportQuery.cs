using System;
using System.Linq;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class IsUserAllowToTestDotNetReportQuery : IQuery<bool>
    {
        private readonly IMastersQueryable _db;
        private readonly int _processKey;
        private readonly string _agency;
        private readonly int _userNumber;


        public IsUserAllowToTestDotNetReportQuery(IMastersQueryable db, int processKey, string agency, int userNumber)
        {
            _db = db;
            _processKey = processKey;
            _agency = agency;
            _userNumber = userNumber;
        }

        public bool ExecuteQuery()
        {
            using (_db)
            {
                if (_processKey <= 0 || string.IsNullOrWhiteSpace(_agency) || _userNumber <= 0) return false;

                var record = _db.ConversionEnabledUsers.FirstOrDefault(w => w.processkey == _processKey
                                        && w.agency.Equals(_agency,StringComparison.OrdinalIgnoreCase)
                                            && w.user_number == _userNumber
                                                && w.currently_enabled);

                return record != null;
            }
        }
    }
}
