using CODE.Framework.Core.Utilities.Extensions;
using com.ciswired.libraries.CISLogger;
using Domain.Orm.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Domain.Helper;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace iBank.BroadcastServer.Email
{
    public class LogoRetriever
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static string GetImageNumber(IQuery<StyleGroupExtras> getLogoByGroupNumAndAgencyQuery)
        {
            var logoInfo = getLogoByGroupNumAndAgencyQuery.ExecuteQuery();

            return (logoInfo != null) ? logoInfo.FieldData : string.Empty;
        }

        public static string GetAgencyLogo(IMasterDataStore masterDataStore, IClientDataStore clientDataStore, int styleGroupNumber, string agency, string defaultLogoPath, string reportOutputDirectory)
        {
            var logoInfoQuery = new GetLogoByGroupNumAndAgencyQuery(clientDataStore.ClientQueryDb, styleGroupNumber, agency, ReportType.BcstReport);
            var imageNumber = GetImageNumber(logoInfoQuery);

            if (string.IsNullOrEmpty(imageNumber)) return "";

            var getClientImageQuery = new GetClientImageByAgencyAndNumberQuery(masterDataStore.MastersQueryDb, agency, imageNumber.TryIntParse(0));
            var getEmailPathsQuery = new GetEmailLogoPathQuery(masterDataStore.MastersQueryDb);
            var getClientImageXDataQuery = new GetClientImageXDataByImgNumberQuery(masterDataStore.MastersQueryDb, imageNumber.TryIntParse(0));

            var agencyLogo = GetAgencyLogo(imageNumber, agency, defaultLogoPath, reportOutputDirectory, getClientImageQuery, getClientImageXDataQuery,
                getEmailPathsQuery);

            return agencyLogo;
        }

        private static string GetAgencyLogo(string imageNumber, string agency, string defaultLogoPath, string reportOutputDirectory, IQuery<ClientImages> getClientImageByAgencyAndNumberQuery, 
            IQuery<IList<ClientImageXData>> getClientImageXData, IQuery<EmailLogoPathInfo> getEmailPathQuery)
        {
            var agencyLogo = "";

            var imageData = getClientImageByAgencyAndNumberQuery.ExecuteQuery();

            if (imageData != null && !string.IsNullOrEmpty(imageData.ImageName) && !string.IsNullOrEmpty(imageData.EncodedData))
            {
                var imageName = agency.Trim() + "_" + imageNumber.Trim() + "_" + imageData.ImageName.Trim();
                var encodedData = imageData.EncodedData.Trim();
                //special handling for large pictures
                if (encodedData.Length > 63990)
                {
                    var imageExtras = getClientImageXData.ExecuteQuery();

                    foreach (var chunk in imageExtras)
                    {
                        encodedData += chunk.EncodedData;
                    }
                }

                var emailLogoPathInfo = getEmailPathQuery.ExecuteQuery();
                
                var logoPath = string.IsNullOrEmpty(emailLogoPathInfo.PhysicalPath)
                               ? defaultLogoPath.AddBS() + imageName
                               : emailLogoPathInfo.PhysicalPath.AddBS() + imageName;

#if !DEBUG
                // When running in debug mode the code fails to connect/see the network path: "\\\\keystonecf1\\wwwroot\\ibankv4\\images\\messaging\\DEMO_94_cis_logonew.gif"
                // However, when testing in debug mode most of the time the DEMO_94_cis_logonew.gif is already there so there is no need to try to create it there again.
                if (!File.Exists(logoPath)) File.WriteAllBytes(logoPath, Convert.FromBase64String(encodedData));
#endif

                agencyLogo = string.IsNullOrEmpty(emailLogoPathInfo.UrlPath)
                                 ? reportOutputDirectory.AddBS() + imageName
                                 : emailLogoPathInfo.UrlPath.AddBS() + imageName;

                agencyLogo = agencyLogo.Replace("\\", "/");
            }

            return agencyLogo;
        }

        public static string ReplaceHTMLLogoPlaceholder(string htmlVersion, string agencyLogo)
        {
            return htmlVersion.Replace("^agency_logo_url^", string.IsNullOrEmpty(agencyLogo) ? "&nbsp;" : string.Format(@"<img src=""{0}"">", agencyLogo));
        }
    }
}
