using System.Collections.Generic;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;

//When C# 6.0 is implemented, this syntax can be used.
//using ExcelInterop = Microsoft.Office.Interop.Excel;

namespace iBank.Services.Implementation.Shared.MacroHelpers
{

    public class PowerMacroHandling
    {
        private static readonly com.ciswired.libraries.CISLogger.ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        const string MacroDataIsMissing = "Macro data missing for a macro in the set";
        const string MacroIsNotAllowed = "Not allowed";

        private static ReportGlobals _globals;

        public static string FileName { get; set; }

        public static List<string> MacroDataList { get; set; }

        public static void LoadMacro(string macroKey)
        {
            var key = macroKey.TryIntParse(-1);
            if (key == -1)
            {
                var msg = "Specified macrokey " + macroKey + " not valid: " + MacroDataIsMissing;
                LOG.Info(msg);
                LoggingMediator.Log(msg, LogEventType.Exception);
                return;
            }

            var clientServer = _globals.AgencyInformation.ServerName;
            var clientDb = _globals.AgencyInformation.DatabaseName;
            var getApprovedMacroDataQuery = new GetApprovedUserMacroByKeyQuery(new iBankClientQueryable(clientServer, clientDb), key, _globals.Agency,
                _globals.UserNumber);
            var macroData = getApprovedMacroDataQuery.ExecuteQuery();

            if (macroData == null || macroData.macrodata == string.Empty)
            {
                var msg = "Specified macrokey " + macroKey + " not valid: " + MacroDataIsMissing;
                LOG.Info(msg);
                LoggingMediator.Log("Specified macrokey " + macroKey + " not valid: " + MacroDataIsMissing, LogEventType.Exception);
                return;
            }

            var macroString = macroData.macrodata;

            //MacroString is either a macro, or it's a list of MacroKeys that have to be looked up.
            //Note that any embedded keys may point to more embedded keys.

            const string chainedNotation = "!SET:";
            if (macroString.Left(5) != chainedNotation)
            {
                MacroDataList.Add(macroString);
                return;
            }

            var macroKeys = macroString.Replace(chainedNotation, string.Empty).Split(',');
            foreach (var mk in macroKeys)
            {
                LoadMacro(mk);
            }
        }

        public static bool RunMacros(ReportGlobals globals, string fileName)
        {
            if (!globals.ParmHasValue(WhereCriteria.POWERMACRO)) return true;

            _globals = globals;
            var macroKey = _globals.GetParmValue(WhereCriteria.POWERMACRO);

            if (!AgencyMacrosAreEnabled())
            {
                var msg = "Specified macrokey " + macroKey + " not valid: " + MacroIsNotAllowed;
                LOG.Warn(msg);
                LoggingMediator.Log(msg, LogEventType.Information);
                return false;
            }

            if (!UserMacrosAreEnabled())
            {
                var msg = "Specified macrokey " + macroKey + " not valid: " + MacroIsNotAllowed;
                LOG.Warn(msg);
                LoggingMediator.Log(msg, LogEventType.Information);
                return false;
            }

            FileName = fileName;

            MacroDataList = new List<string>();

            LoadMacro(macroKey);

            if (MacroDataList.Count == 0)
            {
                var msg = "Specified macrokey " + macroKey + " not valid: " + MacroIsNotAllowed;
                LOG.Warn(msg);
                LoggingMediator.Log(msg, LogEventType.Information);
                return false;
            }

            return new ExcelFunctions().ApplyMacros(fileName, MacroDataList, macroKey);
        }

        public static bool AgencyMacrosAreEnabled()
        {
            

            const string fieldFunction = "ALLOW_MACROS";
            const string powerMacroIsOn = "YES";
            var getAgencyMacrosQuery = new GetClientExtrasByFieldFunctionAndAgencyQuery(new iBankMastersQueryable(), _globals.Agency, fieldFunction);
            var clientExtraPowerMacro = getAgencyMacrosQuery.ExecuteQuery();

            return !string.IsNullOrEmpty(clientExtraPowerMacro)
                    && clientExtraPowerMacro.EqualsIgnoreCase(powerMacroIsOn);
        }

        private static bool UserMacrosAreEnabled()
        {
            

            var clientServer = _globals.AgencyInformation.ServerName;
            var clientDb = _globals.AgencyInformation.DatabaseName;
            const string fieldFunction = "ALLOW_MACROS";
            var isUserMacroEnabledQuery = new IsUserSettingEnabledQuery(new iBankClientQueryable(clientServer, clientDb), _globals.Agency, _globals.UserNumber,
                fieldFunction);
            return isUserMacroEnabledQuery.ExecuteQuery();
        }
    }
}
