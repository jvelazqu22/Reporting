using System.Collections.Generic;

using Domain.Exceptions;
using Domain.Helper;
using Domain.Orm.iBankClientQueries;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;

namespace iBank.Server.Utilities.ReportGlobalsSetters
{
    public class UserGlobalValuesSetter
    {
        private static readonly List<int> _notMultilingual = new List<int> { 2, 18, 24, 27, 30, 32, 36, 38, 70, 74, 76, 86, 90, 92, 146, 241, 243, 247, 249 };

        public ibuser GetUser(IClientDataStore clientDataStore, int userNumber)
        {
            var user = new GetUserByUserNumberQuery(clientDataStore.ClientQueryDb, userNumber)
                .ExecuteQuery();
            if (user == null)
            {
                throw new UserDoesNotExistException("Invalid User!");
            }

            return user;
        }

        public void MapToGlobalUserValues(ReportGlobals globals, ibuser user)
        {
            globals.User.CompanyName = user.compname?.Trim() ?? "";
            globals.User.FirstName = user.firstname?.Trim();
            globals.User.LastName = user.lastname?.Trim();
            globals.User.UserId = user.userid.Trim();
            globals.User.AllAccounts = (AllRecords)user.allaccts.ZeroIfNull();
            globals.User.AllBreaks1 = user.allbrks1;
            globals.User.AllBreaks2 = user.allbrks2;
            globals.User.Break1Name = user.brkname1?.Trim() ?? "";
            globals.User.Break2Name = user.brkname2?.Trim() ?? "";
            globals.User.Break3Name = user.brkname3?.Trim() ?? "";
            globals.User.ReportBreaks = user.reportbrks.ZeroIfNull();
            globals.User.PageBreakLevel = user.pagebrklvl.ZeroIfNull();
            globals.User.AccountBreak = user.acctbrk;
            globals.User.AccountPageBreak = user.acctpgbrk;
            globals.User.Tax1Name = user.tax1name?.Trim();
            globals.User.Tax2Name = user.tax2name?.Trim();
            globals.User.Tax3Name = user.tax3name?.Trim();
            globals.User.Tax4Name = user.tax4name?.Trim();
            globals.User.AllSources = (AllRecords)user.AllSources.ZeroIfNull();
            globals.User.SGroupNumber = user.SGroupNbr;
            globals.User.AdminLevel = user.AdminLvl;
            globals.User.OrganizationKey = user.OrgKey;
            globals.User.UserNumber = user.UserNumber;
            globals.User.AllowAgencyReports = user.agencyrpts;
            globals.User.TimeZone = user.TimeZone?.Trim() ?? "";
        }

        public void SetCountry(ReportGlobals globals, ibuser user)
        {
            var userCountry = (string.IsNullOrEmpty(user.country.Trim())) ? "UNITED STATES" : user.country.Trim();
            if (!globals.ParmHasValue(WhereCriteria.COUNTRY))
            {
                globals.SetParmValue(WhereCriteria.COUNTRY, userCountry);
            }
        }

        public void SetGlobalDateFormat(ReportGlobals globals, ibuser user)
        {
            var globalDateFormat = user.GblDateFmt ? "ON" : "OFF";

            if (!globals.ParmHasValue(WhereCriteria.GLOBALDATEFMT))
            {
                globals.SetParmValue(WhereCriteria.GLOBALDATEFMT, globalDateFormat);
            }
        }

        public void SetPageBreakLevel(ReportGlobals globals)
        {
            //various modifications to user values pulled from Fox code
            if (globals.User.PageBreakLevel == 1)
            {
                switch (globals.User.ReportBreaks)
                {
                    case 0:
                        globals.User.PageBreakLevel = 0;
                        break;
                    case 20:
                    case 21:
                        globals.User.PageBreakLevel = 2;
                        break;
                    case 30:
                        globals.User.PageBreakLevel = 3;
                        break;
                }
            }
        }

        public void SetTaxNames(ReportGlobals globals)
        {
            if (string.IsNullOrEmpty(globals.User.Tax1Name))
            {
                globals.User.Tax1Name = "Tax 1";
            }
            if (string.IsNullOrEmpty(globals.User.Tax2Name))
            {
                globals.User.Tax2Name = "Tax 2";
            }
            if (string.IsNullOrEmpty(globals.User.Tax3Name))
            {
                globals.User.Tax3Name = "Tax 3";
            }
            if (string.IsNullOrEmpty(globals.User.Tax4Name))
            {
                globals.User.Tax4Name = "Tax 4";
            }
        }

        public string GetUserFontType(ReportGlobals globals, IClientDataStore clientDataStore)
        {
            //set user preferred font - font type does not apply to user defined reports
            if (IsUserDefinedReport(globals.ProcessKey))
            {
                return Constants.UserDefinedReport;
            }
            else
            {
                var font = new GetStandardReportFontNameByUserNumberQuery(clientDataStore.ClientQueryDb,
                    globals.User.UserNumber).ExecuteQuery();
                if (string.IsNullOrEmpty(font))
                {
                    font = Constants.DefaultFontType;
                }

                return font;
            }
        }

        public string GetUserLanguage(ReportGlobals globals)
        {
            //Some reports are not ready for multilingual
            if (_notMultilingual.Contains(globals.ProcessKey))
            {
                return Constants.English;
            }

            if (!string.IsNullOrEmpty(globals.OutputLanguage))
            {
                return globals.OutputLanguage;
            }

            if (!globals.MultiLingual)
            {
                return Constants.English;
            }

            return globals.UserLanguage;
        }

        public bool DoesUserSettingExist(IClientDataStore clientDataStore, int userNumber, string agency, string setting)
        {
            var query = new DoesUserSettingExistQuery(clientDataStore.ClientQueryDb, setting, userNumber, agency);
            return query.ExecuteQuery();
        }

        private static bool IsUserDefinedReport(int processKey)
        {
            return processKey >= 500 && processKey <= 599;
        }
    }
}
