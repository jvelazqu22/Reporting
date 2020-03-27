using System.Collections.Generic;

using Domain.Helper;

namespace iBank.Server.Utilities.Classes
{
    public class CriteriaComparer : IEqualityComparer<ReportCriteria>
    {

        public bool Equals(ReportCriteria x, ReportCriteria y)
        {
            return x.VarName.EqualsIgnoreCase(y.VarName);
        }

        public int GetHashCode(ReportCriteria obj)
        {
            return obj.ToString().ToLower().GetHashCode();
        }
    }
}
