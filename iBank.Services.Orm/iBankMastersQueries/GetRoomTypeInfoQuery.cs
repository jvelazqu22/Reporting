using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetRoomTypeInfoQuery : BaseiBankMastersQuery<IList<RoomTypeInfo>>
    {
        public GetRoomTypeInfoQuery(IMastersQueryable db)
        {
            _db = db;
        }
        public override IList<RoomTypeInfo> ExecuteQuery()
        {
            using (_db)
            {
                return _db.RoomType.OrderBy(s => s.LangCode).ThenBy(s => s.roomtype1).Select(s => new RoomTypeInfo
                                                                                                      {
                                                                                                          RoomType = s.roomtype1.Trim(),
                                                                                                          Description = s.typedesc.Trim(),
                                                                                                          LanguageCode = s.LangCode.Trim()
                                                                                                      }).ToList();
            }
        }
    }
}
