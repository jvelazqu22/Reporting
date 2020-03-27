using Domain.Helper;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;
using System.Linq;

namespace iBank.Services.Implementation.Shared.BuildWhereHelpers
{
    public abstract class AbstractWhere
    {
        protected readonly IWhereClauseBuilder _whereClauseBuilder;

        protected readonly IWhereTextBuilder _whereTextBuilder;
        protected readonly IParamsBuilder _paramsBuilder;
        protected const string DEFAULT_WHERE_TEXT_DISPLAY_NAME = "'Caption Not Found'";
        protected const string FIELD_EQUALS_VALUE = "{0} = {1}; ";
        protected const string FIELD_NOT_EQUALS_VALUE = "{0} <> {1}; ";

        protected const string FIELD_LIKE_VALUE = "{0} LIKE {1}; ";

        protected AbstractWhere()
        {
            _whereClauseBuilder = new WhereClauseBuilder();
            _whereTextBuilder = new WhereTextBuilder();
            _paramsBuilder = new ParamsBuilder();
        }

        public string AddListWhere(ReportGlobals globals, string clause, WhereCriteria crit, WhereCriteria listCrit, WhereCriteria notInCrit, string pickListType, string fieldName, string displayName, string notInText)
        {
            var item = globals.GetParmValue(crit);
            var list = globals.GetParmValue(listCrit);
            var notIn = globals.IsParmValueOn(notInCrit);

            if (string.IsNullOrEmpty(list))
            {
                list = item;
            }

            var pl = new PickListParms(globals);
            pl.ProcessList(list, string.Empty, pickListType);

            if (pl.PickList.Any())
            {
                clause = _whereClauseBuilder.AddToWhereClause(clause, pl.PickList, fieldName, notIn);
                globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName, displayName, notIn, pl.PickList, notInText);
            }

            return clause;
        }

        protected string AddSimpleWhere(ReportGlobals globals, BuildWhere where, string whereClause, WhereCriteria crit, string fieldName, string displayName = "")
        {
            var parmValue = globals.GetParmValue(crit);
            if (!string.IsNullOrEmpty(parmValue))
            {
                if (parmValue.HasWildCards())
                {
                    whereClause = _whereClauseBuilder.AddToWhereClause(where.WhereClauseTrip, $"{fieldName} {SharedProcedures.FixWildcard(parmValue)}");
                }
                else
                {
                    whereClause = _whereClauseBuilder.AddToWhereClause(where.WhereClauseTrip, $"{fieldName} = '{parmValue}'");
                }

                if (!string.IsNullOrEmpty(displayName)) globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, "", displayName, false, parmValue);
            }

            return whereClause;
        }
    }
}
