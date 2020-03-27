using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetApprovedUserMacroByKeyQuery : IQuery<ibUserMacroData>
    {
        public int Key { get; set; }
        public string Agency { get; set; }
        public int UserNumber { get; set; }

        private readonly IClientQueryable _db;

        public GetApprovedUserMacroByKeyQuery(IClientQueryable db, int macroKey, string agency, int userNumber)
        {
            _db = db;
            Key = macroKey;
            Agency = agency;
            UserNumber = userNumber;
        }

        public ibUserMacroData ExecuteQuery()
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
