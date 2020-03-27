using iBank.Server.Utilities;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Domain;

namespace iBank.Services.Implementation.Shared.BuildWhereHelpers
{
    public interface IParamsBuilder
    {
        int GetBaseCounterForNextFieldNameParameterToBeAdded(string clause, string fieldName);

        string AddOrListToWhereClause(string clause, List<string> pickList, string fieldName, bool isNotIn, List<SqlParameter> sqlParameters);

        string RemovePeriodFromFieldName(string fieldName);
    }

    public class ParamsBuilder : IParamsBuilder
    {
        //clause: T1.trantype <> @t1TranType1  AND  (T1.acct = @T1acct0 OR T1.acct = @T1acct1 OR T1.acct = @T1acct2)
        //picklist: COUNT = 0;  picklist[0] = "MARSUS"
        //fieldName: T1.acct
        public int GetBaseCounterForNextFieldNameParameterToBeAdded(string clause, string fieldName)
        {
            var parameter = "@" + RemovePeriodFromFieldName(fieldName);
            List<string> listOfStrings = clause.Split(' ').ToList();
            return listOfStrings.Where(s => s.Contains(parameter)).Count();
        }

        public string AddOrListToWhereClause(string clause, List<string> pickList, string fieldName, bool isNotIn, List<SqlParameter> sqlParameters)
        {
            var and = " AND ";
            var or = " OR ";
            var not = " NOT ";
            if (!pickList.Any()) return clause;

            var ors = new List<string>();
            int baseCounter = GetBaseCounterForNextFieldNameParameterToBeAdded(clause, fieldName);
            foreach(var param in pickList)
            {
                var parmName = RemovePeriodFromFieldName(fieldName) + baseCounter++;
                if (param.HasWildCards())
                {
                    //use the value directly because EF will escape '%' when pass it in parameter. eg, acct01
                    ors.Add($"{fieldName} LIKE '{param}'");
                    //ors.Add($"{fieldName} LIKE @{parmName}");
                    sqlParameters.Add(new SqlParameter(parmName, param.ReplaceWildcards()));
                }
                else
                {
                    ors.Add(fieldName + " = @" + parmName);
                    sqlParameters.Add(new SqlParameter(parmName, param));
                }
            }

            var orList = "(" + string.Join(or, ors) + ")";
            if (isNotIn) orList = not + orList;

            return string.IsNullOrEmpty(clause) ? orList : string.Format("{0} {1} {2}", clause, and, orList);
        }

        public string RemovePeriodFromFieldName(string fieldName)
        {
            return fieldName.Replace(".", string.Empty);
        }
    }
}
