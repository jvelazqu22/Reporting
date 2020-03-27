using Domain.Helper;

using iBank.Entities.ClientEntities;

namespace iBank.Server.Utilities.ReportCriteriaHandlers
{
    public abstract class AbstractAdvancedCriteriaRetriever
    {
        protected readonly string _multiUdidAndOr = "MUDANDOR";

        protected readonly string _mudPrefix = "MUD:";
        protected string CreateIteratedVariableName(string variablePrefix, int iteration)
        {
            return variablePrefix + iteration.ToString("D2");
        }

        protected AndOr GetAndOrFromReportCriteria(ReportCriteria andOr)
        {
            if (andOr == null) return AndOr.And;

            return andOr.VarValue == "1" ? AndOr.And : AndOr.Or;
        }

        protected AndOr GetAndOrFromSavedReport3Data(savedrpt3 andOr)
        {
            if (andOr == null) return AndOr.And;

            return andOr.value1.Trim() == "1" ? AndOr.And : AndOr.Or;
        }
    }
}
