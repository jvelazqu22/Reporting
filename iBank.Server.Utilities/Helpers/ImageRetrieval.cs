using System;
using System.Reflection;

using com.ciswired.libraries.CISLogger;

using Domain.Exceptions;
using Domain.Helper;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Repository.SQL.Interfaces;

namespace iBank.Server.Utilities.Helpers
{
    public class ImageRetrieval
    {
        private static ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private IMasterDataStore MasterDataStore { get; set; }
        private IClientDataStore ClientDataStore { get; set; }

        public ImageRetrieval(IMasterDataStore masterDb, IClientDataStore clientDb)
        {
            MasterDataStore = masterDb;
            ClientDataStore = clientDb;
        }

        public ImageInformation GetStandardReportImage(int styleGroupNumber, string agency)
        {
            ImageInformation imageInformation;
            try
            {
                var imageNumber = GetImageNumberByStyleGroup(ClientDataStore.ClientQueryDb, styleGroupNumber,
                    agency, ReportType.StandardReport);
                
                imageInformation = GetImageByNumber(MasterDataStore.MastersQueryDb, agency, imageNumber);
            }
            catch (ImageNotFoundException)
            {
                imageInformation = GetDefaultReportImage(MasterDataStore.MastersQueryDb);
            }

            return imageInformation;
        }

        private int GetImageNumberByStyleGroup(IClientQueryable db, int styleGroupNumber, string agency, ReportType reportType)
        {
            var query = new GetLogoByGroupNumAndAgencyQuery(db, styleGroupNumber, agency, reportType);
            var styleGroup = query.ExecuteQuery();

            if (styleGroup == null) throw new ImageNotFoundException($"Style group number [{styleGroupNumber}] not found for agency [{agency}]");

            int imageNumber;
            if (int.TryParse(styleGroup.FieldData, out imageNumber)) return imageNumber;

            throw new ImageNotFoundException($"Field data for style group [{styleGroupNumber}], agency [{agency}] was not a valid integer.");
        }

        private ImageInformation GetImageByNumber(IMastersQueryable db, string agency, int imageNumber)
        {
            var query = new GetClientImageByAgencyAndNumberQuery(db, agency, imageNumber);
            var imageInformation = query.ExecuteQuery();

            if (imageInformation == null) throw new ImageNotFoundException($"Image number [{imageNumber}] for agency [{agency}] not found.");

            LOG.Debug($"Retrieved logo image name [{imageInformation.ImageName}] using image number [{imageNumber}]");
            return new ImageInformation { ImageName = imageInformation.ImageName, ImageBytes = Convert.FromBase64String(imageInformation.EncodedData) };
        }

        private ImageInformation GetDefaultReportImage(IMastersQueryable db)
        {
            var query = new GetDefaultClientImageQuery(db);
            return query.ExecuteQuery();
        }
        
    }
}
