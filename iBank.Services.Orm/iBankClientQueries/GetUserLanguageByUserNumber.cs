using System;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetUserLanguageByUserNumber : BaseiBankClientQueryable<ibUserExtra>
    {
        public int UserNumber { get; set; }

        public GetUserLanguageByUserNumber(IClientQueryable db, int userNumber)
        {
            _db = db;
            UserNumber = userNumber;
        }

        public override ibUserExtra ExecuteQuery()
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
