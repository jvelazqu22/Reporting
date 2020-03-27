using System;
using System.Configuration;
using System.Reflection;

using com.ciswired.libraries.CISLogger;

using Domain.Helper;
using Domain.Orm.iBankClientQueries;

using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;

namespace iBank.Server.Utilities.ReportGlobalsSetters
{
    public class ClientValuesGlobalsSetter
    {
        private static ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public IClientDataStore ClientDataStore { get; set; }
        public IMasterDataStore MasterDataStore { get; set; }

        public ClientValuesGlobalsSetter(IClientDataStore clientDataStore, IMasterDataStore masterDataStore)
        {
            ClientDataStore = clientDataStore;
            MasterDataStore = masterDataStore;
        }

        public void SetClientDatabaseGlobals(ReportGlobals globals)
        {
            SetUserValues(globals);

            globals.CompanyName = GetOrganizationName(globals.User.OrganizationKey);
            
            if (globals.SavedReportKey > 0)
            {
                SetSavedReportName(globals);
            }

            SetLogoInformation(globals);
        }
        
        private void SetUserValues(ReportGlobals globals)
        {
            var userGlobals = new UserGlobalValuesSetter();

            var user = userGlobals.GetUser(ClientDataStore, globals.UserNumber);

            userGlobals.MapToGlobalUserValues(globals, user);

            userGlobals.SetCountry(globals, user);
            userGlobals.SetGlobalDateFormat(globals, user);

            userGlobals.SetPageBreakLevel(globals);

            userGlobals.SetTaxNames(globals);

            globals.UserFontType = userGlobals.GetUserFontType(globals, ClientDataStore);

            globals.UserLanguage = userGlobals.GetUserLanguage(globals);

            globals.IsLoggingEnabled = userGlobals.DoesUserSettingExist(ClientDataStore, globals.User.UserNumber, globals.Agency, "SQL_LOGGING_ON");
            globals.IsListBreakoutEnabled = userGlobals.DoesUserSettingExist(ClientDataStore, globals.User.UserNumber, globals.Agency, "PICKLIST_BREAKOUT");
        }

        private string GetOrganizationName(int organizationKey)
        {
            var query = new GetOrganizationNameByKeyQuery(ClientDataStore.ClientQueryDb, organizationKey);
            return query.ExecuteQuery();
        }

        private void SetSavedReportName(ReportGlobals globals)
        {
            var savedReport = new GetSavedReport1ByKeyQuery(ClientDataStore.ClientQueryDb, globals.SavedReportKey).ExecuteQuery();
            if (savedReport != null)
            {
                globals.SavedReportName = savedReport.userrptnam.Trim();
            }
        }

        private void SetLogoInformation(ReportGlobals globals)
        {
            //clear out any logo pictures that may still exist (unless we are not generating a file)
            if (!globals.ParmValueEquals(WhereCriteria.OUTPUTTYPE, "-1"))
            {
                var folder = ConfigurationManager.AppSettings["LogoTempDirectory"];
                var threshold = DateTime.Now.AddHours(-24);
                SharedProcedures.DeleteOldFiles(folder, threshold);
            }
            //Clear out the last logo
            globals.LogoBytes = new byte[0];

            LOG.Debug($"Retrieving logo for agency [{globals.Agency}], user [{globals.User.UserNumber}], style group number [{globals.User.SGroupNumber}]");
            var imageRetrieval = new ImageRetrieval(MasterDataStore, ClientDataStore);
            var imageInformation = imageRetrieval.GetStandardReportImage(globals.User.SGroupNumber, globals.Agency);

            globals.LogoFileName = imageInformation.ImageName;
            globals.LogoBytes = imageInformation.ImageBytes;
        }
    }
}
