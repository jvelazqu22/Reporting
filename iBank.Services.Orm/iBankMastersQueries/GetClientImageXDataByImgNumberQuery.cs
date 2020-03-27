using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetClientImageXDataByImgNumberQuery : BaseiBankMastersQuery<IList<ClientImageXData>>
    {
        public int ImageNumber { get; set;}

        public GetClientImageXDataByImgNumberQuery(IMastersQueryable db, int imageNumber)
        {
            _db = db;
            ImageNumber = imageNumber;
        }

        public override IList<ClientImageXData> ExecuteQuery()
        {
            using (_db)
            {
                return _db.ClientImageXData.Where(x => x.ImageNbr == ImageNumber).OrderBy(x => x.recordnbr).ToList();
            }
        }
    }
}
