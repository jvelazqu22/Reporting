using System;
using System.Linq;
using Domain.Helper;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetDefaultClientImageQuery : IQuery<ImageInformation>
    {
        private IMastersQueryable _db;

        public GetDefaultClientImageQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public ImageInformation ExecuteQuery()
        {
            using (_db)
            {
                var image = _db.ClientImages.FirstOrDefault(x => x.Agency.Equals("[defRpt]"));

                if (image == null) throw new Exception("Default report image not found.");

                return new ImageInformation { ImageName = image.ImageName, ImageBytes = Convert.FromBase64String(image.EncodedData) };
            }
        
        }
    }
}
