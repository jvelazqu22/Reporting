using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetClientImageByAgencyAndNumberQuery : IQuery<ClientImages>
    {
        public string Agency { get; set; }
        public int ImageNumber { get; set; }
        private readonly IMastersQueryable _db;

        public GetClientImageByAgencyAndNumberQuery(IMastersQueryable db, string agency, int imageNumber)
        {
            _db = db;
            Agency = agency;
            ImageNumber = imageNumber;
        }

        public ClientImages ExecuteQuery()
        {
            using(_db)
            {
                return _db.ClientImages.FirstOrDefault(x => x.Agency == Agency && x.ImageNbr == ImageNumber);
            }
        }
    }
}
