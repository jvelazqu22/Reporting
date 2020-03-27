using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetApprovedUserMacroByKeyQuery : BaseiBankClientQueryable<ibUserMacroData>
    {
        public int Key { get; set; }
        public string Agency { get; set; }
        public int UserNumber { get; set; }

        public GetApprovedUserMacroByKeyQuery(IClientQueryable db, int macroKey, string agency, int userNumber)
        {
            _db = db;
            Key = macroKey;
            Agency = agency;
            UserNumber = userNumber;
        }

        public override ibUserMacroData ExecuteQuery()
        {
            using (_db)
            {
                return _db.ibUserMacroData.FirstOrDefault(x => x.agency == Agency
                                                               && x.usernumber == UserNumber
                                                               && x.macrokey == Key
                                                               && x.active
                                                               && x.certstatus == "APPROVED");
            }
        }
    }
}
