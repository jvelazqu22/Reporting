using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetRoomTypeInfoQuery : IQuery<IList<RoomTypeInfo>>
    {
        private readonly IMastersQueryable _db;

        public GetRoomTypeInfoQuery(IMastersQueryable db)
        {
            _db = db;
        }
        public IList<RoomTypeInfo> ExecuteQuery()
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
