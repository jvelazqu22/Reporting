using System.Collections.Generic;
using System.Linq;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;
using System.Data.SqlClient;

namespace Domain.Orm.iBankClientQueries
{
    public class OpenClientQuery<T> : IQuery<List<T>>
    {
        private IClientQueryable _db;
        public string Sql { get; set; }
        public object[] Parameters { get; set; }

        //public OpenClientQuery(IClientQueryable db, string sql, object[] parameters)
        //{
        //    _db = db;
        //    Sql = sql;
        //    //Parameters = parameters;
        //    //The reason we copy this to new SqlParameter array is to void 
        //    //"The SqlParameter is already contained by another SqlParameterCollection" error
        //    //We need to refactor this later.
        //    Parameters = new object[parameters.Length];
        //    int counter = 0;
        //    foreach (var param in parameters)
        //    {
        //        var temp = (SqlParameter)param;
        //        var newParam = new SqlParameter(temp.ParameterName, temp.Value);
        //        Parameters[counter++] = newParam;
        //    }
        //}

        public OpenClientQuery(IClientQueryable db, string sql, object[] parameters)
        {
            _db = db;
            Sql = sql;
            ParametersSetUp(parameters);
        }

        public void ParametersSetUp(object[] parameters)
        {
            //Parameters = parameters;
            //The reason we copy this to new SqlParameter array is to void 
            //"The SqlParameter is already contained by another SqlParameterCollection" error
            var paramDictionary = new Dictionary<string, object>();
            foreach (var param in parameters)
            {
                var temp = (SqlParameter)param;
                var paramKeyName = temp.ParameterName.ToUpper();
                if (paramDictionary.ContainsKey(paramKeyName)) continue; // do not add duplicate values
                var newParam = new SqlParameter(paramKeyName, temp.Value);
                paramDictionary.Add(paramKeyName, newParam);
            }

            Parameters = new object[paramDictionary.Count];
            var counter = 0;
            foreach (var param in paramDictionary)
            {
                Parameters[counter++] = param.Value;
            }
        }

        public List<T> ExecuteQuery()
        {
            using (_db)
            {
                var results = _db.Database.SqlQuery<T>(Sql, Parameters).ToList();
                return results;
            }
        }
    }
}
