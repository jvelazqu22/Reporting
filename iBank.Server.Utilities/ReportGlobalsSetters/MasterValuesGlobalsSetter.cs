using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Exceptions;
using Domain.Helper;
using Domain.Orm.Classes;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Classes;

namespace iBank.Server.Utilities.ReportGlobalsSetters
{
    public class MasterValuesGlobalsSetter
    {
        private readonly List<string> TravAuthAgencies = new List<string> { "AXI", "UNITED", "WTSMC" };

        public IMasterDataStore MasterDataStore { get; set; }

        public MasterValuesGlobalsSetter(IMasterDataStore masterDataStore)
        {
            MasterDataStore = masterDataStore;
        }
        
        public void SetMasterDatabaseGlobals(ReportGlobals globals)
        {
            var mstrAgency = GetMasterAgencySetClientType(globals);
            var databaseInformation = GetDatabaseInformation(mstrAgency.DatabaseName);

            mstrAgency.ServerName = databaseInformation.server_address;
            globals.AgencyInformation = mstrAgency;
            globals.AgencyInformation.ReasonExclude = FormatExclusionReasons(globals.AgencyInformation.ReasonExclude);

            //should always true because every agency has migrated to use hibservices.
            globals.UseHibServices = true;

            globals.MultiLingual = IsMultilingual(globals.Agency);
            globals.DefaultLanguage = GetDefaultLanguage(globals);
            
            var reportDetails = GetReportDetails(globals.ProcessKey);

            var outputLanguage = globals.GetParmValue(WhereCriteria.OUTPUTLANGUAGE);
            if (string.IsNullOrEmpty(outputLanguage))
            {
                outputLanguage = globals.UserLanguage;
            }

            globals.ReportTitle = GetReportTitle(globals, outputLanguage, reportDetails.v4caption);

            globals.RecordLimit = reportDetails.reclimit.ZeroIfNull();
            globals.LanguageVariables = SetLanguageVariables(globals, outputLanguage);

            if (string.IsNullOrEmpty(globals.Agency))
            {
                throw new Exception("Agency not supplied!");
            }
            
            globals.FixedDateCurrencyConversion = IsFixedDataCurrencyConversion(globals);

            //retrieve any language specific to the agency, and then the user
            AddUserAgencyLanguage(0, globals);
            AddUserAgencyLanguage(globals.UserNumber, globals);

            UpdateReportMessages(globals);
        }

        private MasterAgencyInformation GetMasterAgencySetClientType(ReportGlobals globals)
        {
            var mstrAgency = new GetMasterAgencyByAgencyQuery(MasterDataStore.MastersQueryDb, globals.Agency).ExecuteQuery();

            //Check to see if the agency is a "sharer"
            if (mstrAgency == null)
            {
                globals.ClientType = ClientType.Sharer;
                mstrAgency = new GetCorpAcctsByAgencyQuery(MasterDataStore.MastersQueryDb, globals.Agency).ExecuteQuery();
            }

            if (mstrAgency == null)
            {
                throw new InvalidAgencyCodeException("INVALID AGENCY CODE");
            }

            return mstrAgency;
        }

        private iBankDatabases GetDatabaseInformation(string databaseName)
        {
            //Get the database address
            var dbInfo = new GetDatabaseInfoByDatabaseNameQuery(MasterDataStore.MastersQueryDb, databaseName).ExecuteQuery();
            if (dbInfo == null)
            {
                throw new DatabaseNotFoundException("DATABASE NOT FOUND");
            }

            return dbInfo;
        }

        private static string FormatExclusionReasons(string exclusionReason)
        {
            if (string.IsNullOrEmpty(exclusionReason))
            {
                //FoxPro code says this value "Has to have something"
                return "'#9'";
            }

            return "'" + exclusionReason.Replace(",", "','") + "'";
        }

        private bool IsMultilingual(string agency)
        {
            var query = new IsMultilingualAgentQuery(MasterDataStore.MastersQueryDb, agency);
            return query.ExecuteQuery();
        }

        private string GetDefaultLanguage(ReportGlobals globals)
        {
            if (globals.MultiLingual)
            {
                var query = new GetSiteDefaultLanguageQuery(MasterDataStore.MastersQueryDb, globals.Agency);
                var defaultLang = query.ExecuteQuery();

                if (defaultLang != null)
                {
                    return defaultLang;
                }
            }
            
            return globals.DefaultLanguage;
        }

        private ibproces GetReportDetails(int processKey)
        {
            //TODO: look for bug - xml 
            var query = new GetReportByProcessIdQuery(MasterDataStore.MastersQueryDb, processKey);
            var reportDetails = query.ExecuteQuery();
            if (reportDetails == null)
            {
                throw new ReportNotFoundException("Process key not found!");
            }

            return reportDetails;
        }

        private string GetReportTitle(ReportGlobals globals, string outputLanguage, string v4Caption)
        {
            //report title need to use outputlanguage, not user lanugage.
            var reportVerbiageQuery = new GetVerbiageByProcessKeyAndLanguageQuery(MasterDataStore.MastersQueryDb, globals.ProcessKey, outputLanguage);
            var reportVerbiage = reportVerbiageQuery.ExecuteQuery();

            if (string.IsNullOrEmpty(globals.ReportTitle) && !globals.ProcessKey.IsBetween(500, 507) &&
                !globals.ProcessKey.IsBetween(510, 599) && !globals.ProcessKey.IsBetween(7051, 7059))
            {
                globals.ReportTitle = (reportVerbiage != null)
                                          ? reportVerbiage.V4Caption
                                          : v4Caption;

                // 05/08/2009 - SPECIAL PROCESSING FOR THE 2 TRAV AUTH REPORTS.      **
                // THEY ARE REFERRED TO AS "PTA" BY AXI AND A COUPLE OTHER AGENCIES, **
                // BUT "PCM" BY EVERYONE ELSE.                                       **
                if ((globals.ProcessKey == 241 || globals.ProcessKey == 243) &&
                    (TravAuthAgencies.Contains(globals.Agency)))
                {
                    globals.ReportTitle = globals.ReportTitle.Replace("PTA", "PCM");
                }

                if (globals.ProcessKey == 247 && globals.ProcessKey == 249 && globals.Agency == "AXI" &&
                    globals.ReportTitle.Left(3) != "AXO")
                {
                    globals.ReportTitle = "AXO" + globals.ReportTitle;
                }
            }

            return globals.ReportTitle;
        }

        private bool IsFixedDataCurrencyConversion(ReportGlobals globals)
        {
            if (!string.IsNullOrEmpty(globals.GetParmValue(WhereCriteria.MONEYTYPE)))
            {
                var fixedDateQuery = new IsCurrencyConversionOnFixedDateQuery(MasterDataStore.MastersQueryDb, globals.Agency);
                return fixedDateQuery.ExecuteQuery();
            }

            return globals.FixedDateCurrencyConversion;
        }

        private static List<LanguageVariableInfo> SetLanguageVariables(ReportGlobals globals, string langCode)
        {
            //Globals.LanguageVariables is used in CrystalFunctions, that translate all crystal p. Need to user Outputlanguage not userlanguage
            var langQuery = new GetMasterLanguageTagsByProcessKeyAndOutputLangQuery(new iBankMastersQueryable(), globals.ProcessKey, langCode).ExecuteQuery();
            return langQuery;
        }
        
        private static void AddUserAgencyLanguage(int userNumber, ReportGlobals globals)
        {
            var userLangQuery = new GetUserLanguageQuery(new iBankMastersQueryable(), globals.ProcessKey, globals.UserLanguage, userNumber,
                globals.Agency);
            var userLanguage = userLangQuery.ExecuteQuery();

            //grab any records that don't already appear in the language variables
            foreach (var langVar in userLanguage)
            {
                var foundRec = globals.LanguageVariables.FirstOrDefault(s => s.VariableName == langVar.VariableName);
                if (foundRec == null)
                {
                    globals.LanguageVariables.Add(langVar);
                }
                else
                {
                    foundRec.Translation = langVar.Translation;
                }
            }
        }

        private static void UpdateReportMessages(ReportGlobals globals)
        {
            foreach (var langVar in globals.LanguageVariables)
            {
                var varName = langVar.VariableName.ToUpper();
                switch (varName)
                {
                    case "XRPTMSG_DATERANGE":
                        globals.ReportMessages.RptMsg_DateRange = langVar.Translation;
                        break;
                    case "XRPTMSG_UDIDNBRREQD":
                        globals.ReportMessages.RptMsg_UDIDNbrReqd = langVar.Translation;
                        break;
                    case "XRPTMSG_OFFLINE":
                        globals.ReportMessages.RptMsg_Offline = langVar.Translation;
                        break;
                    case "XRPTMSG_NODATA":
                        globals.ReportMessages.RptMsg_NoData = langVar.Translation;
                        break;
                    case "XRPTMSG_BIGVOLUME":
                        globals.ReportMessages.RptMsg_BigVolume = langVar.Translation;
                        break;
                    case "XNOTFOUND":
                        globals.ReportMessages.NotFound = langVar.Translation;
                        break;
                    case "XERRHANDMSG1":
                        globals.ReportMessages.ErrHandlMsg1 = langVar.Translation;
                        break;
                    case "XERRHANDMSG2":
                        globals.ReportMessages.ErrHandlMsg2 = langVar.Translation;
                        break;
                    case "CURRENCY":
                        globals.ReportMessages.Currency = langVar.Translation;
                        break;
                    case "XERRORBADCOMBO":
                        globals.ReportMessages.ErrorBadCombo = langVar.Translation;
                        break;
                    case "XINCLUDINGVOIDS":
                        globals.ReportMessages.IncludingVoids = langVar.Translation;
                        break;
                    case "LT_CREDITSONLY":
                        globals.ReportMessages.CreditsOnly = langVar.Translation;
                        break;
                    case "LT_INVOICESONLY":
                        globals.ReportMessages.InvoicesOnly = langVar.Translation;
                        break;
                    case "XINVOICES_VOIDS":
                        globals.ReportMessages.InvoicesAndVoids = langVar.Translation;
                        break;
                    case "XCREDITS_VOIDS":
                        globals.ReportMessages.CreditsAndVoids = langVar.Translation;
                        break;
                    case "XVOIDSONLY":
                        globals.ReportMessages.VoidsOnly = langVar.Translation;
                        break;
                    case "XCARBCALCPROVBY":
                        globals.ReportMessages.CarbCalcProvBy = langVar.Translation;
                        break;
                    case "XBADCOMPAREDATERANGE":
                        globals.ReportMessages.BadCompareDateRange = langVar.Translation;
                        break;
                    case "XFROMDATENOTVALID":
                        globals.ReportMessages.FromDateNotValid = langVar.Translation;
                        break;
                    case "XTODATENOTVALID":
                        globals.ReportMessages.ToDateNotValid = langVar.Translation;
                        break;
                    case "XSPECIFYMONTHYEAR":
                        globals.ReportMessages.ToDateNotValid = langVar.Translation;
                        break;
                    case "XRESDATA":
                        if (globals.ParmValueEquals(WhereCriteria.PREPOST, "1"))
                        {
                            globals.HstPrePref = langVar.Translation;
                        }
                        break;
                    case "XBACKOFFDATA":
                        if (globals.ParmValueEquals(WhereCriteria.PREPOST, "2"))
                        {
                            globals.HstPrePref = langVar.Translation;
                        }
                        globals.ReportMessages.ToDateNotValid = langVar.Translation;
                        break;
                }


            }
        }
    }
}
