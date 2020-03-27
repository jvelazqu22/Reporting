using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetAllTripChangeCodesQuery : IQuery<IList<TripChangeCodeInformation>>
    {
        private readonly IMastersQueryable _db;

        public GetAllTripChangeCodesQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<TripChangeCodeInformation> ExecuteQuery()
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
