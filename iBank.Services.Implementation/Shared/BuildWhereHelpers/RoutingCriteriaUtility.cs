using Domain.Helper;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Classes;

namespace iBank.Services.Implementation.Shared.BuildWhereHelpers
{
    public class RoutingCriteriaUtility
    {
        public static void ClearRouteCriteria(ReportGlobals globals)
        {
            globals.SetParmValue(WhereCriteria.ORIGIN, string.Empty);
            globals.SetParmValue(WhereCriteria.DESTINAT, string.Empty);
            globals.SetParmValue(WhereCriteria.METROORG, string.Empty);
            globals.SetParmValue(WhereCriteria.METRODEST, string.Empty);
            globals.SetParmValue(WhereCriteria.ORIGCOUNTRY, string.Empty);
            globals.SetParmValue(WhereCriteria.DESTCOUNTRY, string.Empty);
            globals.SetParmValue(WhereCriteria.ORIGREGION, string.Empty);
            globals.SetParmValue(WhereCriteria.DESTREGION, string.Empty);
            globals.SetParmValue(WhereCriteria.INORGS, string.Empty);
            globals.SetParmValue(WhereCriteria.INDESTS, string.Empty);
            globals.SetParmValue(WhereCriteria.INMETROORGS, string.Empty);
            globals.SetParmValue(WhereCriteria.INMETRODESTS, string.Empty);
            globals.SetParmValue(WhereCriteria.INORIGCOUNTRY, string.Empty);
            globals.SetParmValue(WhereCriteria.INDESTCOUNTRY, string.Empty);
            globals.SetParmValue(WhereCriteria.INORIGREGION, string.Empty);
            globals.SetParmValue(WhereCriteria.INDESTREGION, string.Empty);
        }

        public static void RestoreRouteCriteria(ReportGlobals globals, RoutingCriteria routingCriteria)
        {
            globals.SetParmValue(WhereCriteria.ORIGIN, routingCriteria.Origins);
            globals.SetParmValue(WhereCriteria.DESTINAT, routingCriteria.Destinations);
            globals.SetParmValue(WhereCriteria.METROORG, routingCriteria.OriginMetros);
            globals.SetParmValue(WhereCriteria.METRODEST, routingCriteria.DestinationMetros);
            globals.SetParmValue(WhereCriteria.ORIGCOUNTRY, routingCriteria.OriginCountries);
            globals.SetParmValue(WhereCriteria.DESTCOUNTRY, routingCriteria.DestinationCountries);
            globals.SetParmValue(WhereCriteria.ORIGREGION, routingCriteria.OriginRegions);
            globals.SetParmValue(WhereCriteria.DESTREGION, routingCriteria.DestinationRegions);
            globals.SetParmValue(WhereCriteria.INORGS, routingCriteria.Origins);
            globals.SetParmValue(WhereCriteria.INDESTS, routingCriteria.Destinations);
            globals.SetParmValue(WhereCriteria.INMETROORGS, routingCriteria.OriginMetros);
            globals.SetParmValue(WhereCriteria.INMETRODESTS, routingCriteria.DestinationMetros);
            globals.SetParmValue(WhereCriteria.INORIGCOUNTRY, routingCriteria.OriginCountries);
            globals.SetParmValue(WhereCriteria.INDESTCOUNTRY, routingCriteria.DestinationCountries);
            globals.SetParmValue(WhereCriteria.INORIGREGION, routingCriteria.OriginRegions);
            globals.SetParmValue(WhereCriteria.INDESTREGION, routingCriteria.DestinationRegions);
        }

        public static RoutingCriteria GetRoutingCriteria(ReportGlobals globals)
        {
            var rc = new RoutingCriteria { Origins = globals.GetParmValue(WhereCriteria.ORIGIN) };

            if (string.IsNullOrEmpty(rc.Origins))
            {
                rc.Origins = globals.GetParmValue(WhereCriteria.INORGS);
            }
            rc.Destinations = globals.GetParmValue(WhereCriteria.DESTINAT);
            if (string.IsNullOrEmpty(rc.Destinations))
            {
                rc.Destinations = globals.GetParmValue(WhereCriteria.INDESTS);
            }
            rc.OriginMetros = globals.GetParmValue(WhereCriteria.METROORG);
            if (string.IsNullOrEmpty(rc.OriginMetros))
            {
                rc.OriginMetros = globals.GetParmValue(WhereCriteria.INMETROORGS);
            }
            rc.DestinationMetros = globals.GetParmValue(WhereCriteria.METRODEST);
            if (string.IsNullOrEmpty(rc.DestinationMetros))
            {
                rc.DestinationMetros = globals.GetParmValue(WhereCriteria.INMETRODESTS);
            }
            rc.OriginCountries = globals.GetParmValue(WhereCriteria.ORIGCOUNTRY);
            if (string.IsNullOrEmpty(rc.OriginCountries))
            {
                rc.OriginCountries = globals.GetParmValue(WhereCriteria.INORIGCOUNTRY);
            }
            rc.DestinationCountries = globals.GetParmValue(WhereCriteria.DESTCOUNTRY);
            if (string.IsNullOrEmpty(rc.DestinationCountries))
            {
                rc.DestinationCountries = globals.GetParmValue(WhereCriteria.INDESTCOUNTRY);
            }
            rc.OriginRegions = globals.GetParmValue(WhereCriteria.ORIGREGION);
            if (string.IsNullOrEmpty(rc.OriginRegions))
            {
                rc.OriginRegions = globals.GetParmValue(WhereCriteria.INORIGREGION);
            }
            rc.DestinationRegions = globals.GetParmValue(WhereCriteria.DESTREGION);
            if (string.IsNullOrEmpty(rc.DestinationRegions))
            {
                rc.DestinationRegions = globals.GetParmValue(WhereCriteria.INDESTREGION);
            }

            rc.NotInOrigin = globals.IsParmValueOn(WhereCriteria.NOTINORIGIN);
            rc.NotInDestination = globals.IsParmValueOn(WhereCriteria.NOTINDESTS);
            rc.NotInOriginMetros = globals.IsParmValueOn(WhereCriteria.NOTINMETROORGS);
            rc.NotInDestinationMetros = globals.IsParmValueOn(WhereCriteria.NOTINMETRODESTS);
            rc.NotInOriginCountries = globals.IsParmValueOn(WhereCriteria.NOTINORIGCOUNTRY);
            rc.NotInDestinationCountries = globals.IsParmValueOn(WhereCriteria.NOTINDESTCOUNTRY);
            rc.NotInOriginRegions = globals.IsParmValueOn(WhereCriteria.NOTINORIGREGION);
            rc.NotInDestinationRegions = globals.IsParmValueOn(WhereCriteria.NOTINDESTREGION);

            return rc;
        }
    }
}
