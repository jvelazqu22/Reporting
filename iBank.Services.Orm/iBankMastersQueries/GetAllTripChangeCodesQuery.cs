using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetAllTripChangeCodesQuery : BaseiBankMastersQuery<IList<TripChangeCodeInformation>>
    {
        public GetAllTripChangeCodesQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public override IList<TripChangeCodeInformation> ExecuteQuery()
        {
            using(_db)
            {
                return _db.TripChangeCodes.Select(s => new TripChangeCodeInformation
                                                           {
                                                               RecordNo = s.RecordNo,
                                                               Active = s.active,
                                                               ChangeCode = s.ChangeCode,
                                                               ChangeGroup = s.ChangeGrp,
                                                               CodeDescription = s.codedesc,
                                                               LanguageCode = s.LangCode,
                                                               Priority = s.priority
                                                           }).ToList();
            }
        }
    }
}
