using com.ciswired.libraries.CISLogger;

using Domain.Exceptions;
using Domain.Interfaces.Query;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Repository;

namespace iBank.Server.Utilities
{
    // ReSharper disable once InconsistentNaming
    public class iBankServerInformationRetrieval
    {
        private static readonly com.ciswired.libraries.CISLogger.ILogger LOG =
            new LogManagerWrapper().GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string GetServerName(string databaseName)
        {
            return GetServerName(new GetDatabaseInfoByDatabaseNameQuery(new iBankMastersQueryable(), databaseName.Trim()));
        }

        public static string GetServerName(IDatabaseInfoQuery getDatabaseQuery)
        {
            var dbActive = getDatabaseQuery.ExecuteQuery();

            var serverAddress = "";
            var isDatabaseGood = IsDatabaseGoodToUse(dbActive);
            if (!isDatabaseGood)
            {
                serverAddress = "";
            }
            else
            {
                serverAddress = dbActive.server_address.Trim();
            }

            if (string.IsNullOrEmpty(serverAddress))
            {
                var databaseGood = isDatabaseGood ? "OK" : "NOT OK";
                throw new ServerAddressNotFoundException($"The server address was not found for database [{getDatabaseQuery.DatabaseName}]. The database was [{databaseGood}]");
            }

            return dbActive.server_address.Trim();
        }

        public static bool IsDatabaseGoodToUse(iBankDatabases database)
        {
            if (database == null)
            {
                var errMsg = "Database not found in IBANKDTABASES table";
                LOG.Warn(errMsg);
                return false;
            }

            if (!database.active)
            {
                var errMsg = string.Format("Database flagged as Inactive:{0} - Broadcast Report Timer (2C)", database.databasename);
                LOG.Warn(errMsg);
                return false;
            }

            if (database.StopBcst)
            {
                var msg = string.Format("Database [{0}] flagged as Stop Broadcast", database.databasename);
                LOG.Warn(msg);
                return false; //down for maintenance, etc. 
            }

            return true;
        }
    }
}
