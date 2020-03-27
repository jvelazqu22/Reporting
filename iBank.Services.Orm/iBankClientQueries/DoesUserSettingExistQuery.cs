using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class DoesUserSettingExistQuery : BaseiBankClientQueryable<bool>
    {
        public string FieldFunction { get; set; }
        public int UserNumber { get; set; }
        public string Agency { get; set; }

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

        public override bool ExecuteQuery()
        {
            return _db.iBUserExtra.Any(x => x.UserNumber == UserNumber && x.agency.Trim() == Agency && x.FieldFunction.Trim() == FieldFunction);
        }
    }
}
