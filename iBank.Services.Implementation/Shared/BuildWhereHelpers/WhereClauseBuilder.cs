using System.Collections.Generic;
using System.Linq;

using iBank.Server.Utilities;
using iBank.Server.Utilities.Helpers;

namespace iBank.Services.Implementation.Shared.BuildWhereHelpers
{
    public interface IWhereClauseBuilder
    {
        string AddToWhereClause(string whereClause, string newItem);

        string AddToWhereClause(string whereClause, IList<string> newItems, string fieldName, bool isNotIn);
    }

    public class WhereClauseBuilder : IWhereClauseBuilder
    {
        public string AddToWhereClause(string whereClause, string newItem)
        {
            if (string.IsNullOrEmpty(newItem)) return whereClause;

            newItem = newItem.Trim();

            if (string.IsNullOrEmpty(whereClause))
            {
                return newItem;
            }
            else
            {
                if (whereClause.Trim().EndsWith("AND"))
                {
                    return $"{whereClause} {newItem}";
                }
                else
                {
                    return $"{whereClause} AND {newItem}";
                }
            }
        }

        public string AddToWhereClause(string whereClause, IList<string> newItems, string fieldName, bool isNotIn)
        {
            if (!newItems.Any()) return whereClause;

            //if there's only one item, we don't need a clause.
            if (newItems.Count == 1)
            {
                var item = newItems[0];
                var itemClause = "";

                if (item.HasWildCards()) return AddToWhereClause(whereClause, $"{fieldName} {SharedProcedures.FixWildcard(item)}");

                itemClause = isNotIn
                    ? string.Format("{0} <> '{1}'", fieldName, newItems[0])
                    : string.Format("{0} = '{1}'", fieldName, newItems[0]);

                return string.IsNullOrEmpty(whereClause) 
                    ? itemClause
                    : whereClause + " AND " + itemClause;
            }

            var listString = "'" + string.Join("','", newItems) + "'";
            listString = isNotIn ? string.Format("{0} Not In ({1})", fieldName, listString) : string.Format("{0} In ({1})", fieldName, listString);

            return string.IsNullOrEmpty(whereClause) ? listString : whereClause + " AND " + listString;
        }
    }
}
