using CODE.Framework.Core.Utilities.Extensions;
using com.ciswired.libraries.CISLogger;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Helper;
using Domain.Interfaces;
using Domain.Models;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Classes.Interfaces;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities;
using iBank.Services.Implementation.Utilities.ClientData;
using iBank.Services.Implementation.Utilities.CurrencyConversion;
using iBankDomain.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Services.Implementation.Shared.Client;
using System.Threading.Tasks;
using Domain;
using Domain.Orm.iBankAdministrationCommands;
using Domain.Orm.iBankAdministrationQueries.ConfigurationQueries;
using Domain.Services;
using iBank.Server.Utilities.Helpers;

namespace iBank.Services.Implementation.ReportPrograms
{
    public abstract class ReportRunner<TRawData, TFinalData> : IReportRunner
    {
        private readonly int _serverNumber = ConfigurationManager.AppSettings["ServerNumber"].TryIntParse(0);

        private readonly ReportRunnerErrorHandling _errorHandler;
        public ClientFunctions clientFunctions = new ClientFunctions();
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public BuildWhere BuildWhere { get; set; }
        public ReportGlobals Globals { get { return BuildWhere.ReportGlobals; } }
        public ReportDocument ReportSource { get; set; }
        public string CrystalReportName { get; set; }

        public List<TRawData> RawDataList { get; set; }
        public List<TFinalData> FinalDataList { get; set; }
        public GlobalsCalculator GlobalCalc { get; set; }

        private ReportRunConditionals Conditionals { get; }

        protected IMasterDataStore MasterStore { get { return new MasterDataStore(); } }

        protected IClientDataStore ClientStore { get { return new ClientDataStore(Globals.AgencyInformation.ServerName, Globals.AgencyInformation.DatabaseName); } }

        private readonly List<int> _excludeRawDataListCheck = new List<int> { 34, 36, 53, 146 };

        protected ReportRunner()
        {
            _errorHandler = new ReportRunnerErrorHandling(LOG);

            BuildWhere = new BuildWhere(clientFunctions);
            ReportSource = new ReportDocument();
            RawDataList = new List<TRawData>();
            FinalDataList = new List<TFinalData>();
            Conditionals = new ReportRunConditionals();

            CrystalReportName = string.Empty;
        }

        public bool RunReport()
        {
            if (Globals.IsOfflineServer) return RunReportWorker();

            //only need to worry about pushing offline if running on online report
            try
            {
#if DEBUG
                return RunReportWorker();
#else
                var cancelToken = CancellationTokenCreator.Create();
                var task = Task.Factory.StartNew(RunReportWorker, cancelToken);
                task.Wait(cancelToken);
                return task.Result;
#endif
            }
            catch (OperationCanceledException ex)
            {
                _errorHandler.PushOffline(ClientStore, MasterStore, Globals, LOG, ex.ToString());
                return false;
            }
        }

        private bool RunReportWorker()
        {
            using (var timer = new CustomTimer(Globals.ProcessKey, Globals.UserNumber, Globals.Agency, Globals.ReportLogKey, LOG, SubjectOfTiming.OnlineReport))
            {
                try
                {
                    GlobalCalc = new GlobalsCalculator(Globals);
                    //make sure directory exists
                    if (!Directory.Exists(Globals.ResultsDirectory))
                    {
                        Directory.CreateDirectory(Globals.ResultsDirectory);
                    }

                    if (!InitialChecks()) return false;

                    //get the user specific date format
                    Globals.DateDisplay = GetDateDisplay();
                    if (Globals.ClientType == ClientType.Sharer)
                    {
                        var retriever = new SharedDataSourceRetriever(MasterStore);
                        Globals.CorpAccountDataSources = retriever.GetDataSourcesForAllAgencies(Globals.Agency);
                    }

                    if (!GetRawData()) return false;

                    if (!_excludeRawDataListCheck.Contains(Globals.ProcessKey))
                    {
                        if (!DataExists(RawDataList)) return false;
                    }

                    if (!string.IsNullOrEmpty(Globals.TitleAcct)) Globals.AccountName = Globals.TitleAcct;

                    if (!ProcessData()) return false;

                    Globals.ReportInformation.RecordCount = FinalDataList.Count;
                    //This is for unit testing; -1 means no file will be generated. 
                    if (Globals.ParmValueEquals(WhereCriteria.OUTPUTTYPE, "-1")) return true;
                    var result = GenerateReport();
                    OptimizeListsForGC();
                    return result;
                }
                catch (Exception ex)
                {
                    ParseException(ex);
                    return false;
                }
            }
        }

        private void OptimizeListsForGC()
        {
            RawDataList.Clear();
            RawDataList.Capacity = 0;
            RawDataList = null;

            // this is needed as part of the report history
            //FinalDataList.Clear();
            //FinalDataList.Capacity = 0;
            //FinalDataList = null;
        }

        private void ParseException(Exception ex)
        {
            if (ex is PushTimedOutReportOfflineException offlineEx)
            {
                _errorHandler.PushOffline(ClientStore, MasterStore, Globals, LOG, offlineEx.ToString());
            }

            var recoverableException = ex as RecoverableSqlException;
            if (recoverableException != null)
            {
                _errorHandler.PushOffline(ClientStore, MasterStore, Globals, LOG, recoverableException.ToString());
                return;
            }

            var currencyConversionException = ex as CurrencyConversionException;
            if (currencyConversionException != null)
            {
                _errorHandler.HandleError(ex, Globals, _serverNumber, CrystalReportName);
                _errorHandler.SetErrorMessage(Globals, ex.Message);
                return;
            }

            var sqlException = ex as SqlException;
            if (sqlException != null)
            {
                //look for timeout errors
                if (sqlException.Number == (int)SqlErrorCode.Timeout)
                {
                    var pusher = new ReportDelayer(ClientStore, MasterStore, Globals);
                    pusher.PushReportOffline(_serverNumber.ToString());
                    LOG.Info(sqlException.Message.FormatMessageWithReportLogKey(Globals.ReportLogKey));
                }
                else
                {
                    _errorHandler.HandleError(sqlException, Globals, _serverNumber, CrystalReportName);
                    _errorHandler.SetGenericError(Globals);
                }

                return;
            }

            var colException = ex as MissingUserDefinedColumnsException;
            if (colException != null)
            {
                _errorHandler.HandleError(ex, Globals, _serverNumber, "User Defined Report");
                _errorHandler.SetErrorMessage(Globals, ex.Message);
                return;
            }

            var aggregateException = ex as AggregateException;
            if (aggregateException != null)
            {
                foreach (var except in aggregateException.InnerExceptions)
                {
                    if (except is SqlException && aggregateException.InnerExceptions.Count == 1 && except.Message.IndexOf("TIMEOUT", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        _errorHandler.PushOffline(new ClientDataStore(Globals.AgencyInformation.ServerName, Globals.AgencyInformation.DatabaseName),
                                new MasterDataStore(), Globals, LOG, except.ToString());
                        return;
                    }

                    if (except is PushTimedOutReportOfflineException)
                    {
                        _errorHandler.PushOffline(ClientStore, MasterStore, Globals, LOG, except.ToString());
                    }
                    else
                    {
                        _errorHandler.HandleError(except, Globals, _serverNumber, CrystalReportName);
                    }
                }

                _errorHandler.SetGenericError(Globals);

                return;
            }

            //Check if error is caused by Crystal Reports reached maximum job limits. 
            if (ex is CrystalDecisions.Shared.CrystalReportsException)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains(ErrorMessages.CRYSTAL_JOB_LIMIT_MAX_REACHED_MSG))
                {
                    try
                    {
                        var configurationName = string.Format(Configurations.REPORT_SERVER_RE_START_FLAG, _serverNumber);
                        var configNameCacheKey = GetServerNameCacheKey(configurationName);
                        var serverConfiguration = new GetConfigurationQuery(new CacheService(), new iBankAdministrationQueryable(), configNameCacheKey).ExecuteQuery();
                        serverConfiguration.Value = "true";
                        new UpdateConfigurationCommand(new iBankAdministrationCommandDb(), serverConfiguration).ExecuteCommand();
                        LOG.Info($"ParseException() for server number: {_serverNumber} - setting configuration to re-start");
                    }
                    catch (Exception ex2)
                    {
                        LOG.Error($"Error occurred during SetReStartToFalse() for server number: {_serverNumber}", ex2);
                    }
                }
            }

            //this will handle an Exception object that falls through
            _errorHandler.HandleError(ex, Globals, _serverNumber, CrystalReportName);
            _errorHandler.SetGenericError(Globals);
        }

        private CacheKeys GetServerNameCacheKey(string serverName)
        {
            try
            {
                return (CacheKeys)Enum.Parse(typeof(CacheKeys), serverName);
            }
            catch (Exception)
            {
                throw new InvalidDatabaseConfigurationException($"ServerName {serverName} is not contained in the list of CacheKeys. Please fix the servername or add it to the CacheKeys");
            }
        }


        public abstract bool InitialChecks();

        public abstract bool GetRawData();

        public abstract bool ProcessData();

        public abstract bool GenerateReport();

        public List<TLeg> GetLegDataFromFilteredSegData<TLeg, TSeg>(IList<TLeg> legData, IList<TSeg> filteredSegData) where TLeg : IRecKey where TSeg : IRecKey
        {
            var filteredSegmentDataRecKeys = filteredSegData.Select(x => x.RecKey).Distinct();

            return legData.Where(x => filteredSegmentDataRecKeys.Contains(x.RecKey)).ToList();
        }

        public IList<TRaw> RetrieveRawData<TRaw>(SqlScript sql, bool isReservationReport, bool addFieldsFromLegsTable = true, bool includeAllLegs = true, bool checkForDuplicatesAndRemoveThem = false,
            bool handleAdvanceParamsAtReportLevelOnly = false) where TRaw : IRecKey
        {

            return checkForDuplicatesAndRemoveThem
                ? ClientDataRetrieval.GetRawData<TRaw>(sql, isReservationReport, BuildWhere, Globals, addFieldsFromLegsTable, includeAllLegs, checkForDuplicatesAndRemoveThem, handleAdvanceParamsAtReportLevelOnly).RemoveDuplicates().ToList()
                : ClientDataRetrieval.GetRawData<TRaw>(sql, isReservationReport, BuildWhere, Globals, addFieldsFromLegsTable, includeAllLegs, checkForDuplicatesAndRemoveThem, handleAdvanceParamsAtReportLevelOnly);

        }

        /// <summary>
        /// Checks the current money type, and if it doesn't match the specified type calls the currency conversion
        /// </summary>
        /// <param name="listToConvert"></param>
        /// <returns></returns>
        public List<TY> PerformCurrencyConversion<TY>(List<TY> listToConvert)
        {
            using (var timer = new CustomTimer(Globals.ProcessKey, Globals.UserNumber, Globals.Agency, Globals.ReportLogKey, LOG, SubjectOfTiming.CurrencyConversion))
            {
                var moneyType = Globals.GetParmValue(WhereCriteria.MONEYTYPE);
                if (Conditionals.IsCurrencyConversionRequired(listToConvert, moneyType))
                {
                    var cc = new CurrencyConverter();
                    cc.ConvertCurrency(listToConvert, moneyType);
                }

                return listToConvert;
            }
        }

        public bool IsBeginDateSupplied()
        {
            return Conditionals.IsBeginDateSupplied(Globals);
        }

        public bool IsStartParseDateValid()
        {
            return Conditionals.IsStartParseDateValid(Globals);
        }

        public bool IsEndParseDateValid()
        {
            return Conditionals.IsEndParseDateValid(Globals);
        }

        public bool IsDateRangeValid()
        {
            return Conditionals.IsDateRangeValid(Globals);
        }

        public bool IsDataRangeUnderThreeMonths()
        {
            if (Globals.IsOfflineServer) return true; //this criteria only applies to online reports

            var pusher = new ReportDelayer(ClientStore, MasterStore, Globals);
            return Conditionals.IsDateRangeUnderThreeMonths(Globals, pusher);
        }

        public bool IsGoodCombo()
        {
            return Conditionals.IsGoodDateRangeReportTypeCombo(Globals);
        }

        public bool IsUdidNumberSuppliedWithUdidText()
        {
            return Conditionals.IsUdidNumberSuppliedWithUdidText(Globals);
        }
        public bool IsUdidNumberSupplied()
        {
            return Conditionals.IsUdidNumberSupplied(Globals);
        }

        public bool IsOnlineReport()
        {
            var pusher = new ReportDelayer(ClientStore, MasterStore, Globals);
            return Conditionals.IsOnlineReport(Globals, pusher);
        }

        public bool HasAccount()
        {
            var delayer = new ReportDelayer(ClientStore, MasterStore, Globals);
            return Conditionals.HasAccount(Globals, delayer);
        }

        public bool DataExists<TY>(List<TY> dataList)
        {
            return Conditionals.DataExists(dataList, Globals);
        }

        public bool IsUnderOfflineThreshold<TData>(List<TData> dataList)
        {
            var pusher = new ReportDelayer(ClientStore, MasterStore, Globals);
            return Conditionals.IsUnderOfflineThreshold(dataList.Count, Globals, pusher);
        }

        public bool IsUnderOfflineThreshold(int recordCount)
        {
            var pusher = new ReportDelayer(ClientStore, MasterStore, Globals);
            return Conditionals.IsUnderOfflineThreshold(recordCount, Globals, pusher);
        }

        public int GetFinalRecordsCount()
        {
            return FinalDataList?.Count ?? 0;
        }

        private static readonly List<string> _dateMarks = new List<string> { "/", ".", "-" };
        private string GetDateDisplay()
        {
            if (Globals.Agency.Equals("GSA")) return "M/d/yy";
            var country = Globals.GetParmValue(WhereCriteria.COUNTRY);

            var intlQuery = new GetSettingsByCountryAndLangCodeQuery(MasterStore.MastersQueryDb, country, Globals.UserLanguage);
            var intl = intlQuery.ExecuteQuery();

            if (!intl.DateDisplay.Contains(intl.DateMark))
            {
                //find the mark that is being used
                var currentDateMark = "";
                foreach (var mark in _dateMarks.Where(mark => intl.DateDisplay.Contains(mark)))
                {
                    currentDateMark = mark;
                    break;
                }

                //replace the current mark with the correct mark
                intl.DateDisplay = intl.DateDisplay.Replace(currentDateMark, intl.DateMark);
            }

            //reformat the date mask to be correct
            var tempCharArray = intl.DateDisplay.ToLower().ToCharArray();
            for (var i = 0; i < tempCharArray.Length; i++)
            {
                if (tempCharArray[i] == 'm')
                {
                    tempCharArray[i] = tempCharArray[i].ToString().ToUpper()[0];
                }
            }

            return new string(tempCharArray);
        }

    }
}
