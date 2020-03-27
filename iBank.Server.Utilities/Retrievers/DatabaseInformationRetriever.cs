using Domain.Orm.Classes;
using System.Collections.Generic;
using System.Linq;

using iBankDomain.RepositoryInterfaces;

namespace iBank.Server.Utilities.Retrievers
{
    public class DatabaseInformationRetriever
    {
        public IList<DatabaseInformation> GetDatabases(IList<IQuery<IList<DatabaseInformation>>> getDatabaseQueries)
        {
            var validDiscreteDatabases = new List<DatabaseInformation>();

            foreach (var query in getDatabaseQueries)
            {
                AddDatabases(validDiscreteDatabases, query);
            }

            return validDiscreteDatabases;
        }

        private void AddDatabases(IList<DatabaseInformation> validDiscreteDatabases, IQuery<IList<DatabaseInformation>> getDatabasesQuery)
        {
            var possibleDatabases = getDatabasesQuery.ExecuteQuery();
            foreach (var db in possibleDatabases.Where(db => !validDiscreteDatabases.Any(x => x.DatabaseName == db.DatabaseName && x.TimeZoneOffset == db.TimeZoneOffset)))
            {
                validDiscreteDatabases.Add(db);
            }
        }
    }
}
