using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetStandardReportFontNameByUserNumberQuery : BaseiBankClientQueryable<string>
    {
        public int UserNumber { get; set; }

        public GetStandardReportFontNameByUserNumberQuery(IClientQueryable db, int userNumber)
        {
            _db = db;
            UserNumber = userNumber;
        }

        public override string ExecuteQuery()
        {
            using (_db)
            {
                var font = _db.iBUserExtra.FirstOrDefault(x => x.UserNumber == UserNumber
                                                           && x.FieldFunction.ToUpper().Trim() == "STDRPTFONTNAME");

                return font == null ? "" : font.FieldData.Trim().ToUpper();
            }
        }
    }
}
