using System.Collections.Generic;
using System.Linq;

using Domain.Helper;
using Domain.Interfaces;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.Shared.WhereRoute
{
    public class OnlyOriginOrDestinationCriteria
    {
        private static readonly WhereTextBuilder _whereTextBuilder = new WhereTextBuilder();

        //it is called from ApplyWhereRoute from BuildWhere class is in IsRoutingBidirectional is true condition.
        public List<T> HandleOnlyOriginOrDestSpecifiedInWhereRoute<T>(List<T> rawData, string orgList, string destList, PickListParms pl, string displayName, bool returnAllLegs, ReportGlobals globals)
            where T : class, IRouteWhere
        {
            var whereRoute = new WhereRouteApplier<T>();
            var placeList = orgList + destList;
            pl.ProcessList(placeList, string.Empty, "AIRPORTS");

            var notIn = false;
            if (!string.IsNullOrEmpty(orgList) && globals.IsParmValueOn(WhereCriteria.NOTINORIGIN))
            {
                notIn = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(destList) && globals.IsParmValueOn(WhereCriteria.NOTINDESTS))
                {
                    notIn = true;
                }
            }

            rawData = whereRoute.GetDataBasedOnOriginOrDestinationCriteria(rawData, pl.PickList, notIn, returnAllLegs).ToList();

            globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName, displayName, notIn, placeList);
            return rawData;
        }

    }
}
