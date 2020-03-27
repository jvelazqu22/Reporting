using System;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class IsUserSettingEnabledQuery : BaseiBankClientQueryable<bool>
    {
        public string Agency { get; set; }
        public int UserNumber { get; set; }
        public string FieldFunction { get; set; }

        public IsUserSettingEnabledQuery(IClientQueryable db, string agency, int userNumber, string fieldFunction)
        {
            _db = db;
            Agency = agency;
            UserNumber = userNumber;
            FieldFunction = fieldFunction;
        }

        public override bool ExecuteQuery()
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
