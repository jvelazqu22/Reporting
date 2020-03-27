using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetClientImageByAgencyAndNumberQuery : BaseiBankMastersQuery<ClientImages>
    {
        public string Agency { get; set; }
        public int ImageNumber { get; set; }

        public GetClientImageByAgencyAndNumberQuery(IMastersQueryable db, string agency, int imageNumber)
        {
            _db = db;
            Agency = agency;
            ImageNumber = imageNumber;
        }

        public override ClientImages ExecuteQuery()
        {
            using(_db)
            {
                return _db.ClientImages.FirstOrDefault(x => x.Agency == Agency && x.ImageNbr == ImageNumber);
            }
        }
    }
}
