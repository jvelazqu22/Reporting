using System;
using System.Configuration;
using System.Reflection;

using com.ciswired.libraries.CISLogger;

using CODE.Framework.Core.Utilities.Extensions;

using Domain.Helper;
using Domain.Orm.iBankMastersCommands;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Repository;

namespace iBank.Server.Utilities.Logging
{
    public class ErrorLogger
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public static int LogException(int? userNumber, string agency, Exception ex, MethodBase methodBase, ServerType serverType, ILogger logger)
        {
            try
            {
                var recordNumber = AddErrorLogRecord(methodBase.DeclaringType, userNumber, agency, ex.ToString(), serverType.ToString());

                //log to the regular logging table
                logger.Error(ex.Message.FormatMessageWithErrorNumber(recordNumber), ex);
                return recordNumber;
            }
            catch (Exception e)
            {
                LOG.Error(e.Message, e);
                return 0;
            }
        }

        public static int LogException(int? userNumber, string agency, Exception ex, string customErrorMsg, MethodBase methodBase, ServerType serverType, ILogger logger)
        {
            try
            {
                var recordNumber = AddErrorLogRecord(methodBase.DeclaringType, userNumber, agency, ex.ToString(), serverType.ToString());

                //log to the regular logging table
                var exceptionMessage = string.Format("[{0}] [{1}]", customErrorMsg, ex.Message);
                logger.Error(exceptionMessage.FormatMessageWithErrorNumber(recordNumber), ex);
                return recordNumber;
            }
            catch (Exception e)
            {
                LOG.Error(e.Message, e);
                return 0;
            }
        }

        public static int LogError(int? userNumber, string agency, string errorMsg, MethodBase methodBase, ServerType serverType, ILogger logger)
        {
            try
            {
                var recordNumber = AddErrorLogRecord(methodBase.DeclaringType, userNumber, agency, errorMsg, serverType.ToString());

                //log to the regular logging table
                logger.Error(errorMsg.FormatMessageWithErrorNumber(recordNumber));

                return recordNumber;
            }
            catch (Exception e)
            {
                LOG.Error(e.Message, e);
                return 0;
            }
        }

        public static int LogWarning(int? userNumber, string agency, string warningMessage, MethodBase methodBase, ServerType serverType, ILogger logger)
        {
            try
            {
                var recordNumber = AddErrorLogRecord(methodBase.DeclaringType, userNumber, agency, warningMessage, serverType.ToString());

                //log to the regular logging table
                logger.Warn(warningMessage.FormatMessageWithErrorNumber(recordNumber));

                return recordNumber;
            }
            catch (Exception e)
            {
                LOG.Error(e.Message, e);
                return 0;
            }
        }
        
        public errorlog Log(ErrorInformation errorInfo)
        {
            var errorLog = new errorlog();
            try
            {
                errorLog = CreateErrorLogRecord(errorInfo);

                var addErrorRecCmd = new AddErrorLogRecordCommand(new iBankMastersCommandDb(), errorLog);
                addErrorRecCmd.ExecuteCommand();
            }
            catch (Exception e)
            {
                LOG.Error(e.Message, e);
            }

            return errorLog;
        }

        private static errorlog CreateErrorLogRecord(int? userNumber, string agency, string exceptionMsg, string callingClass, string serverType)
        {
            var serverNumber = ConfigurationManager.AppSettings["ServerNumber"].TryIntParse(0);
            return new errorlog
            {
                UserNumber = userNumber,
                agency = agency,
                errdate = DateTime.Now,
                errormsg = exceptionMsg.Left(250),
                errornbr = (byte)0,
                errorpgm = callingClass.Left(100),
                iBankVer = ".NET",
                svrnbr = (byte)serverNumber,
                pgmlinenbr = (byte)0,
                serverName = (serverType + serverNumber).Left(40)
            };
        }
        
        private errorlog CreateErrorLogRecord(ErrorInformation errorInfo)
        {
            return new errorlog
            {
                UserNumber = errorInfo.UserNumber,
                agency = errorInfo.Agency,
                errdate = errorInfo.ErrorDate,
                errormsg = errorInfo.Exception.ToString().Left(250),
                errornbr = (byte)errorInfo.ErrorNumber,
                errorpgm = errorInfo.ErrorProgram.Left(99),
                iBankVer = errorInfo.Version,
                svrnbr = (byte)errorInfo.ServerNumber,
                pgmlinenbr = (byte)errorInfo.LineNumber,
                serverName = errorInfo.ServerName.Left(39)
            };
        }

        private static int AddErrorLogRecord(Type callingType, int? userNumber, string agency, string errorMessage, string serverType)
        {
            var caller = callingType != null ? callingType.FullName : "";
            var rec = CreateErrorLogRecord(userNumber, agency, errorMessage, caller, serverType);

            //add to the errorlog table
            new AddErrorLogRecordCommand(new iBankMastersCommandDb(), rec).ExecuteCommand();

            return rec.recordno;
        }
    }
}
