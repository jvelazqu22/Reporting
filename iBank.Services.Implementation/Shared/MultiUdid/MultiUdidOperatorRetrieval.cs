using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.Shared.MultiUdid
{
    public class MultiUdidOperatorRetrieval
    {
        public MultiUdidCriteria.OperatorType GetOpertorType(ReportGlobals globals)
        {
            if (AllOperatorsAreSetToEqual(globals.MultiUdidParameters.Parameters))
            {
                return OperatorsAreCombinedOnAnd(globals.MultiUdidParameters)
                           ? MultiUdidCriteria.OperatorType.AllEqualsAndOperator
                           : MultiUdidCriteria.OperatorType.AllEqualsOrOperator;
            }

            if (AllOperatorsAreSetToNotEqual(globals.MultiUdidParameters.Parameters))
            {
                return OperatorsAreCombinedOnAnd(globals.MultiUdidParameters)
                           ? MultiUdidCriteria.OperatorType.AllNotEqualAndOperator
                           : MultiUdidCriteria.OperatorType.AllNotEqualOrOperator;
            }

            return OperatorsAreCombinedOnAnd(globals.MultiUdidParameters)
                       ? MultiUdidCriteria.OperatorType.MixedTypeAndOperator
                       : MultiUdidCriteria.OperatorType.MixedTypeOrOperator;
        }

        private bool AllOperatorsAreSetToEqual(List<AdvancedParameter> parameters)
        {
            return parameters.All(x => x.Operator == Operator.Equal);
        }

        private bool AllOperatorsAreSetToNotEqual(List<AdvancedParameter> parameters)
        {
            return parameters.All(x => x.Operator == Operator.NotEqual);
        }

        private bool OperatorsAreCombinedOnAnd(AdvancedParameters parameter)
        {
            return parameter.AndOr == AndOr.And;
        }
    }
}
