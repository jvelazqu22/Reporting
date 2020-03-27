using System.Collections.Generic;

namespace Domain.Helper
{
    public class AdvancedParameters
    {
        public AdvancedParameters()
        {
            AndOr = AndOr.And;
            Parameters = new List<AdvancedParameter>();
        }
        public AndOr AndOr { get; set; }
        public List<AdvancedParameter> Parameters { get; set; }

    }
}
