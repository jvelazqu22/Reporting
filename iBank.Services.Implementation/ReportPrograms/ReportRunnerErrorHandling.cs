using System;
using System.Configuration;
using System.Diagnostics;

using Domain.Helper;

using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Logging;

using com.ciswired.libraries.CISLogger;

using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities;

namespace iBank.Services.Implementation.ReportPrograms
{
    public class ReportRunnerErrorHandling
    {
        private readonly ReportLogLogger _rptLogger;
        private readonly ErrorLogger _errorLogger;

        private readonly string _serverNumber = ConfigurationManager.AppSettings["ServerNumber"];

        private readonly ILogger _log;
        public ReportRunnerErrorHandling(ILogger log)
        {
            _rptLogger = new ReportLogLogger();
            _errorLogger = new ErrorLogger();
            _log = log;
        }

        public void HandleError(Exception ex, ReportGlobals globals, int serverNumber, string programName)
        {
            var error = new ErrorInformation
            {
                Exception = ex,
                Agency = globals.Agency,
                UserNumber = globals.UserNumber,
                Version = globals.iBankVersion,
                ServerNumber = serverNumber,
                ErrorNumber = -1,
                ErrorProgram = programName,
                LineNumber = (short)(new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber(),
                ServerName = globals.IsOfflineServer ? ".NET Broadcast Server" : ".NET Report Server"
            };

            _errorLogger.Log(error);
            _log.Error(ex.Message.FormatMessageWithReportLogKey(globals.ReportLogKey), ex);

            if (globals.ReportLogKey != 0)
            {
                _rptLogger.UpdateLog(globals.ReportLogKey, ReportLogLogger.ReportStatus.SYSERROR);
            }
        }

        public void PushOffline(IClientDataStore clientDataStore, IMasterDataStore masterDataStore, ReportGlobals globals, ILogger log, string exceptionMessage)
        {
            //go ahead and push offline to give some more time for the database to have a chance to clear up
            var pusher = new ReportDelayer(clientDataStore, masterDataStore, globals);
            pusher.PushReportOffline(_serverNumber);

            log.Debug(exceptionMessage.FormatMessageWithReportLogKey(globals.ReportLogKey));
        }

        public void SetGenericError(ReportGlobals globals)
        {
            globals.ReportInformation.ReturnCode = 2;
            globals.ReportInformation.ErrorMessage = globals.ReportMessages.GenericErrorMessage;
        }

        public void SetErrorMessage(ReportGlobals globals, string errorMessage)
        {
            globals.ReportInformation.ReturnCode = 2;
            globals.ReportInformation.ErrorMessage = errorMessage;
        }
    }
}
