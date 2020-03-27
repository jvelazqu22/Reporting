using Domain.Helper;

using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.Shared.WhereRoute.Helpers
{
    public class NotInDecider
    {
        public static bool IsMetroNotInSet(RoutingCriteria routing, ReportGlobals globals)
        {
            if (routing.IsOnlyOneWayCriteriaSpecified)
            {
                if (!string.IsNullOrEmpty(routing.Origin) && globals.IsParmValueOn(WhereCriteria.NOTINMETROORGS))
                {
                    return true;
                }
                else if (!string.IsNullOrEmpty(routing.Destination) && globals.IsParmValueOn(WhereCriteria.NOTINMETRODESTS))
                {
                    return true;
                }
            }
            else if (routing.AreBothWaysCriteriaSpecified)
            {
                return globals.IsParmValueOn(WhereCriteria.NOTINMETROORGS) || globals.IsParmValueOn(WhereCriteria.NOTINMETRODESTS);
            }

            return false;
        }

        public static bool IsCountryNotInSet(RoutingCriteria routing, ReportGlobals globals)
        {
            if (routing.IsOnlyOneWayCriteriaSpecified)
            {
                if (!string.IsNullOrEmpty(routing.Origin) && globals.IsParmValueOn(WhereCriteria.NOTINORIGCOUNTRY))
                {
                    return true;
                }
                else if (!string.IsNullOrEmpty(routing.Destination) && globals.IsParmValueOn(WhereCriteria.NOTINDESTCOUNTRY))
                {
                    return true;
                }
            }
            else if (routing.AreBothWaysCriteriaSpecified)
            {
                return globals.IsParmValueOn(WhereCriteria.NOTINORIGCOUNTRY) || globals.IsParmValueOn(WhereCriteria.NOTINDESTCOUNTRY);
            }

            return false;
        }

        public static bool IsRegionNotInSet(RoutingCriteria routing, ReportGlobals globals)
        {
            if (routing.IsOnlyOneWayCriteriaSpecified)
            {
                if (!string.IsNullOrEmpty(routing.Origin) && globals.IsParmValueOn(WhereCriteria.NOTINORIGREGION))
                {
                    return true;
                }
                else if (!string.IsNullOrEmpty(routing.Destination) && globals.IsParmValueOn(WhereCriteria.NOTINDESTREGION))
                {
                    return true;
                }
            }
            else if (routing.AreBothWaysCriteriaSpecified)
            {
                return globals.IsParmValueOn(WhereCriteria.NOTINORIGREGION) || globals.IsParmValueOn(WhereCriteria.NOTINDESTREGION);
            }

            return false;
        }
    }
}
