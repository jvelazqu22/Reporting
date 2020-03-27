using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using CODE.Framework.Core.Utilities.Extensions;
using Domain;
using Domain.Helper;
using Domain.Orm.Classes;
using iBank.Server.Utilities.Helpers;

namespace iBank.Server.Utilities.Classes
{
    /// <summary>
    /// Contains all the "Global" properties necessary to run a report. 
    /// </summary>
    public class ReportGlobals
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public ReportGlobals()
        {
            CrystalDirectory = string.Empty;
            //ResultsDirectory = string.Empty;
            iBankVersion = string.Empty;

            ReportId = string.Empty;
            ClientType = ClientType.Agency;
            ReportTitle = string.Empty;
            UserLanguage = "EN";
            Agency = string.Empty;
            OutputDestination = string.Empty;
            OutputType = string.Empty;
            TabBgColor = string.Empty;
            SavedReportName = string.Empty;
            TitleAcct = string.Empty;
            TitlePrefix = string.Empty;
            SuppressCriteria = string.Empty;
            WhereComplex = string.Empty;
            WhereComplexText = string.Empty;
            CompanyName = string.Empty;
            DateDisplayFormat = "OFF";
            HstPrePref = string.Empty;
            DefaultLanguage = string.Empty;

            ReportParameters = new Dictionary<int, ReportCriteria>();
            AdvancedParameters = new AdvancedParameters();
            MultiUdidParameters = new AdvancedParameters();
            User = new UserInformation();
            LanguageVariables = new List<LanguageVariableInfo>();
            ReportMessages = new ReportMessages();
            AgencyInformation = new MasterAgencyInformation();
            ReportInformation = new PendingReportInformation();
            InternationalSettings = new InternationalSettingsInformation();

            LogoBytes = new byte[0];
            WhereText = string.Empty;
        }

        public PendingReportInformation ReportInformation { get; set; }
        public string ReportId { get; set; }
        public int ServerNumber { get; set; }
        public bool IsOfflineServer { get; set; }

        public string CrystalDirectory { get; set; }

        public string ResultsDirectory
        {
            get
            {
                var dir = ConfigurationManager.AppSettings["GeneratedReportsDirectory"];

                if (IsItOkayToUseTestReportNameOrPath(TestReportPath)) dir = TestReportPath;

                if (string.IsNullOrEmpty(dir)) throw new Exception("Please configure a GeneratedReportsDirectory!");

                return IsItOkayToUseTestReportNameOrPath(TestReportPath) 
                    ? dir.AddBS() 
                    : dir.AddBS() + Agency.AddBS();
            }
        }

        public string iBankVersion { get; set; }

        public ClientType ClientType { get; set; }
        public UserInformation User { get; set; }
        public MasterAgencyInformation AgencyInformation { get; set; }
        public InternationalSettingsInformation InternationalSettings { get; set; }

        public Dictionary<int, ReportCriteria> ReportParameters { get; set; }
        public AdvancedParameters AdvancedParameters { get; set; }
        public AdvancedParameters MultiUdidParameters { get; set; }
        public List<LanguageVariableInfo> LanguageVariables { get; set; }
        public ReportMessages ReportMessages { get; set; }
        public bool ReplaceCAcctNameAndAccountInParamsWithPickListName { get; set; } = false;
        public string PickListName { get; set; } = string.Empty;

        public string CopyRight
        {
            get
            {
                var copyRight = LanguageVariables.FirstOrDefault(s => s.VariableName.ToUpper() == "XCOPYRIGHT");
                return copyRight == null
                    ? "Produced by iBank Travel Management © Cornerstone Information Systems 2001-2010 -- all data is unaudited"
                    : copyRight.Translation;
            }
        }

        //Parameters for all reports
        public int ReportLogKey { get; set; }
        public bool ReportingTravet { get; set; }
        public string BaseUrl { get; set; }
        public int EProfileNumber { get; set; }
        public string OutputDestination { get; set; }
        public string Agency { get; set; }
        public string UserLanguage { get; set; }
        public bool IsUnicode { get { return UserLanguage.Equals("JP", StringComparison.OrdinalIgnoreCase); } }//Only japanese uses unicode at this point. 
        public string OutputLanguage { get; set; }
        public int ImageNumber { get; set; }
        public int UserNumber { get; set; }
        public string OutputType { get; set; }
        public DestinationSwitch OutputFormat
        {
            get
            {
                var outputType = GetParmValue(WhereCriteria.OUTPUTTYPE);
                switch (outputType)
                {
                    case "2":
                    case "2X":
                        return DestinationSwitch.Xls;
                    case "5":
                        return DestinationSwitch.Csv;
                    case "9":
                        return DestinationSwitch.XML;
                    case "10":
                        return DestinationSwitch.PPT;
                    case "XX":
                        return DestinationSwitch.XlsxFormatted;
                    default:
                        return DestinationSwitch.ClassicPdf;
                }
            }
        }
        public int MacroKey { get; set; }
        public string TabBgColor { get; set; }
        public int PickRecNumber { get; set; }
        public int BatchNumber { get; set; }
        public int SavedReportKey { get; set; }
        public string SavedReportName { get; set; }
        public string TitleAcct { get; set; }
        public string ReportTitle { get; set; }
        public string TitlePrefix { get; set; }
        public string SuppressCriteria { get; set; }
        public string WhereComplex { get; set; }
        public string WhereComplexText { get; set; }
        public int ProcessKey { get; set; }
        public bool LegDIT { get; set; }
        public string FyStartMonth { get; set; } = string.Empty;

        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        
        public string CompanyName { get; set; }

        public string LogoFileName { get; set; }
        public byte[] LogoBytes { get; set; }
        public bool HasLogo { get { return LogoBytes.Length > 0; } }

        //public BackofficeOrReservation BackofficeOrReservation { get; set; }
        public string HstPrePref { get; set; }
        public string DateDisplayFormat { get; set; }
        public bool FixedDateCurrencyConversion { get; set; }
        public bool MultiLingual { get; set; }
        public string DefaultLanguage { get; set; }
        public bool UseHibServices { get; set; }
        public bool IsLoggingEnabled { get; set; }
        public bool IsListBreakoutEnabled { get; set; }
        public int RecordLimit { get; set; }

        public string AccountName { get; set; }
        public string WhereText { get; set; } //for display on reports as "Report Parameters"

        //These variables are used by reports we haven't written yet, but they are used in BuildWhere, so we need them. 
        public bool IncludeOrphanServiceFees { get; set; }

        public string DateDisplay { get; set; }
        public IList<CorpAccountDataSource> CorpAccountDataSources { get; set; }
        public string UserFontType { get; set; }
        public string TestReportName { get; set; } = string.Empty;
        public string TestReportPath { get; set; } = string.Empty;

        public bool IsEffectsDelivery
        {
            get
            {
                return OutputDestination == "3" && EProfileNumber > 0;
            }
        } 

        public void ConvertAdvancedRouteParmToReportParm(string key,  WhereCriteria reportParmKey, WhereCriteria reportParmOperatorKey)
        {
            var parm = AdvancedParameters.Parameters.Where(x => x.FieldName == key);

            ReportCriteria reportParameter;
            var item = parm.FirstOrDefault();
            if (item.FieldName == key)
            {
                reportParameter = new ReportCriteria() { VarName = reportParmKey.ToString(), VarValue = item.Value1 };
                ReportParameters.Add((int)reportParmKey, reportParameter);
                AdvancedParameters.Parameters.Remove(item);
            }
            if (item.FieldName == "Operator" && item.Value1 == "NotEqual")
            {
                reportParameter = new ReportCriteria() { VarName = reportParmOperatorKey.ToString(), VarValue = "ON" };
                ReportParameters.Add((int)reportParmOperatorKey, reportParameter);
            }
        }

        public string GetParmValue(WhereCriteria key)
        {
            ReportCriteria parm;
            var success = ReportParameters.TryGetValue((int)key, out parm);
            return success && !string.IsNullOrEmpty(parm.VarValue) ? parm.VarValue.Trim() : string.Empty;
        }

        public bool ParmValueEquals(WhereCriteria key, string val)
        {
            ReportCriteria parm;
            var success = ReportParameters.TryGetValue((int)key, out parm);
            return success && parm.VarValue.Trim().EqualsIgnoreCase(val);
        }

        private const string On = "On";
        public bool IsParmValueOn(WhereCriteria key)
        {
            ReportCriteria parm;
            var success = ReportParameters.TryGetValue((int)key, out parm);
            return success && parm.VarValue.Trim().EqualsIgnoreCase(On);
        }

        public void SetParmValue(WhereCriteria key, string value)
        {
            ReportCriteria parm;
            ReportParameters.TryGetValue((int)key, out parm);
            if (parm != null)
            {
                parm.VarValue = value;
                ReportParameters[(int)key] = parm;
            }
            else
            {
                parm = new ReportCriteria { VarName = key.ToString(), VarValue = value };
                ReportParameters.Add((int)key, parm);
            }

        }

        /// <summary>
        /// returns true if  the specified criteria is in the parameter list, and is not blank. 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ParmHasValue(WhereCriteria key)
        {
            ReportCriteria parm;
            var success = ReportParameters.TryGetValue((int)key, out parm);
            return success && !string.IsNullOrEmpty(parm.VarValue);
        }

        public string GetLanguageTranslation(string varName, string defaultCaption)
        {
            var trns = LanguageVariables.FirstOrDefault(s => s.VariableName.Equals(varName, StringComparison.InvariantCultureIgnoreCase));
            //other xLabels are translated in 
            return trns == null ? defaultCaption : trns.Translation;
        }

        public string TranslateDDMMMDate(DateTime? date)
        {
            if (!date.HasValue) return "";

            var defaultMonths = "Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec";
            var transMonth = LanguageVariables.FirstOrDefault(s => s.VariableName.Equals("lt_AbbrMthsofYear", StringComparison.InvariantCultureIgnoreCase));
            var month = date.Value.Month;
            var day = date.Value.ToString("dd");
            var defmonth = defaultMonths.Split(',')[month - 1].Trim();
            var trnMonth = transMonth.Translation.Split(',')[month - 1].Trim();
            trnMonth = trnMonth.Substring(0, 1).ToUpper() + trnMonth.Substring(1);
            return trnMonth == null ? day + defmonth : day + trnMonth;
        }

        public string BuildDateDesc()
        {
            return BuildDateDesc(Convert.ToDateTime(BeginDate), Convert.ToDateTime(EndDate));
        }

        public string BuildDateDesc(DateTime begin, DateTime end)
        {
            var dateRangeParm = GetParmValue(WhereCriteria.DATERANGE);

            var descriptionBuilder = new DateRangeDescriptionBuilder(LanguageVariables, DateDisplay);
            return descriptionBuilder.Build(begin, end, dateRangeParm);
        }
        
        public string GetDateRangeDesc(string concat)
        {
            var begin = Convert.ToDateTime(BeginDate).ToString(DateDisplay);
            var end = Convert.ToDateTime(EndDate).ToString(DateDisplay);

            return begin + concat + end;
        }

        private bool IsItOkayToUseTestReportNameOrPath(string testReportNameOrPath)
        {
            var reportServerlist = new List<int>() {100, 10, 11};

            if (string.IsNullOrEmpty(testReportNameOrPath)) return false;
            if (!reportServerlist.Contains(ServerNumber)) return false;

            return true;
        }

        public string GetFileName()
        {
            var ext = GetExtension(GetParmValue(WhereCriteria.OUTPUTTYPE));
            if (IsItOkayToUseTestReportNameOrPath(TestReportName))
            {
                SavedReportName = $"{TestReportName}{ext}";
            }
            else
            {
                var guid = Guid.NewGuid().ToString().ToUpper().Replace('-', '_');
                SavedReportName = $"IB{guid}{ext}";
            }

            //Also assign the href to the report information object for saving
            var hrefRoot = ConfigurationManager.AppSettings["HrefRoot"];
            if (string.IsNullOrEmpty(hrefRoot)) throw new Exception("Href Root must be configured!");
            hrefRoot = hrefRoot.Trim();
            if (!hrefRoot.Right(1).Equals("/")) hrefRoot += "/";

            ReportInformation.Href = hrefRoot + Agency + "/" + SavedReportName;
            var fileName = ResultsDirectory + SavedReportName;

            LOG.Debug($"ReportInformation.Href = [{ReportInformation.Href}] | File Name = [{fileName}]");

            return fileName;
        }

        private string GetExtension(string outputType)
        {
            var ext = ".pdf";
            switch (outputType.ToUpper())
            {
                case "2":
                case "2X":
                    ext = ".xlsx";
                    break;
                case "XG":
                case "X":
                case "XX":
                    ext = ".xls";
                    break;
                case "5":
                    ext = ".csv";
                    break;
                case "9":
                    ext = ".xml";
                    break;
                case "RG":
                case "R":
                    ext = ".doc";
                    break;
            }
            return ext;
        }
        
        
    }
}
