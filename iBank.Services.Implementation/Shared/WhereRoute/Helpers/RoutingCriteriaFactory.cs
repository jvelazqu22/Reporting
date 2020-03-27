using System;

using Domain.Helper;

using iBank.Server.Utilities.Classes;

using iBankDomain.Interfaces;

namespace iBank.Services.Implementation.Shared.WhereRoute.Helpers
{
    public class RoutingCriteriaFactory : IFactory<RoutingCriteria>
    {
        public RoutingCriteriaType Type { get; set; }

        private readonly ReportGlobals _globals;

        public RoutingCriteriaFactory(RoutingCriteriaType type, ReportGlobals globals)
        {
            Type = type;
            _globals = globals;
        }

        public RoutingCriteria Build()
        {
            var destinationList = "";
            var originList = "";
            var origin = "";
            var destination = "";

            switch (Type)
            {
                case RoutingCriteriaType.Airport:
                    origin = _globals.GetParmValue(WhereCriteria.ORIGIN);
                    originList = _globals.GetParmValue(WhereCriteria.INORGS);
                    destination = _globals.GetParmValue(WhereCriteria.DESTINAT);
                    destinationList = _globals.GetParmValue(WhereCriteria.INDESTS);
                    break;
                case RoutingCriteriaType.Metro:
                    origin = _globals.GetParmValue(WhereCriteria.METROORG);
                    originList = _globals.GetParmValue(WhereCriteria.INMETROORGS);
                    destination = _globals.GetParmValue(WhereCriteria.METRODEST);
                    destinationList = _globals.GetParmValue(WhereCriteria.INMETRODESTS);
                    break;
                case RoutingCriteriaType.Region:
                    origin = _globals.GetParmValue(WhereCriteria.ORIGREGION);
                    originList = _globals.GetParmValue(WhereCriteria.INORIGREGION);
                    destination = _globals.GetParmValue(WhereCriteria.DESTREGION);
                    destinationList = _globals.GetParmValue(WhereCriteria.INDESTREGION);
                    break;
                case RoutingCriteriaType.Country:
                    origin = _globals.GetParmValue(WhereCriteria.ORIGCOUNTRY);
                    originList = _globals.GetParmValue(WhereCriteria.INORIGCOUNTRY);
                    destination = _globals.GetParmValue(WhereCriteria.DESTCOUNTRY);
                    destinationList = _globals.GetParmValue(WhereCriteria.INDESTCOUNTRY);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Criteria type of [{Type}] not handled.");
            }

            if (string.IsNullOrEmpty(originList)) originList = origin;
            if (string.IsNullOrEmpty(destinationList)) destinationList = destination;

            return new RoutingCriteria(originList, destinationList);
        }
    }
}
