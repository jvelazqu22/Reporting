using System;
using System.Data.Entity.Validation;
using System.Reflection;

using com.ciswired.libraries.CISLogger;

using Domain.Orm.iBankMastersCommands;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.Exceptions;
using iBankDomain.RepositoryInterfaces;

namespace iBank.Server.Utilities.Logging
{
    public interface ISqlLogger
    {
        void Log(string sqlText, object[] parameters, ICommandDb mastersCommandDb);

        void Log(string sqlText, ICommandDb mastersCommandDb);
    }

    public class SqlLogger : ISqlLogger
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private int ReportLogNumber { get; set; }
        private int UserNumber { get; set; }
        private string UserId { get; set; }
        private int ProcessKey { get; set; }
        // ReSharper disable once InconsistentNaming
        private int UDRKey { get; set; }

        private readonly bool _isLoggingRequired;

        public SqlLogger(int reportLogNumber, int userNumber, string userId, int processKey, int udrKey, IClientQueryable queryDb, string agency, bool forceLogDueToException = false)
        {
            ReportLogNumber = reportLogNumber;
            UserNumber = userNumber;
            UserId = userId;
            ProcessKey = processKey;
            UDRKey = udrKey;

            var isLoggingRequiredQuery = new IsSqlLoggingOnForUserQuery(queryDb, userNumber, agency);
            _isLoggingRequired = isLoggingRequiredQuery.ExecuteQuery() || forceLogDueToException;
        }

        public void Log(string sqlText, object[] parameters, ICommandDb mastersCommandDb)
        {
            if (!_isLoggingRequired) return;

            var sqlSave = sqlText;
            foreach (object obj in parameters)
            {
                var item = (System.Data.SqlClient.SqlParameter)obj;
                if (item.DbType == System.Data.DbType.String || item.DbType == System.Data.DbType.DateTime)
                {
                    sqlSave = sqlSave.Replace("@" + item.ParameterName.ToString(), "'" + item.Value.ToString() + "'");
                }
                else
                {
                    sqlSave = sqlSave.Replace("@" + item.ParameterName.ToString(), item.Value.ToString());
                }
            }
            Log(sqlSave, mastersCommandDb);
        }

        public void Log(string sqlText, ICommandDb mastersCommandDb)
        {
            var logRecord = new ibRptLogSQL
            {
                RptLogNo = ReportLogNumber,
                UserNumber = UserNumber,
                UserID = UserId,
                Processkey = ProcessKey,
                UDRKey = UDRKey,
                SavedRptKey = 0,
                SQLText = sqlText,
                LogDate = DateTime.Now
            };

            try
            {
                var addLogCommand = new AddRptLogSqlRecordCommand(mastersCommandDb, logRecord);
                addLogCommand.ExecuteCommand();
            }
            catch (DbEntityValidationException dbEx)
            {
                try
                {
                    EFExceptionHandling.HandleDbEntityValidationException(dbEx);
                }
                catch (UnrecoverableSqlException ex)
                {
                    LOG.Error(ex);
                }
            }
            catch (Exception e)
            {
                try { ErrorLogger.LogException(null, null, e, MethodBase.GetCurrentMethod(), ServerType.Unknown, LOG); }
                catch (Exception)
                {
                    //swallow any errors at this point so logging doesn't kill the process
                }
                
            }
        }
    }
}
