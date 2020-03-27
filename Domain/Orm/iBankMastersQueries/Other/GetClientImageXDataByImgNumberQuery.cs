using System.Collections.Generic;
using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetClientImageXDataByImgNumberQuery : IQuery<IList<ClientImageXData>>
    {
        public int ImageNumber { get; set;}
        private readonly IMastersQueryable _db;

        public GetClientImageXDataByImgNumberQuery(IMastersQueryable db, int imageNumber)
        {
            _db = db;
            ImageNumber = imageNumber;
        }

        public IList<ClientImageXData> ExecuteQuery()
        {
            using (_db)
            {
                return _db.ClientImageXData.Where(x => x.ImageNbr == ImageNumber).OrderBy(x => x.recordnbr).ToList();
            }
        }
    }
}
