using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.ciswired.libraries.CISLogger;

using Domain.Helper;
using Domain.Interfaces;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.Shared.WhereRoute
{
    public class BothOriginAndDestCriteria
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly WhereTextBuilder _whereTextBuilder = new WhereTextBuilder();

        public BothOriginAndDestCriteria()
        {
        }

        //it is called from ApplyWhereRoute from BuildWhere class is in IsRoutingBidirectional is true condition.
        public List<T> HandleBothOriginAndDestSpecifiedInWhereRoute<T>(List<T> rawData, PickListParms pl, string orgList, string destList, string displayName, bool returnAllLegs, ReportGlobals globals)
            where T : class, IRouteWhere
        {
            var whereRoute = new WhereRouteApplier<T>();
            pl.ProcessList(orgList, string.Empty, "AIRPORTS");
            var orgPickList = pl.PickList;
            var orgPickName = pl.PickName;

            pl.ProcessList(destList, string.Empty, "AIRPORTS");
            var destPickList = pl.PickList;
            var destPickName = pl.PickName;

            string pickName;
            if (string.IsNullOrEmpty(orgPickName) || string.IsNullOrEmpty(destPickName))
            {
                pickName = orgPickName + destPickName;
            }
            else
            {
                pickName = orgPickName + ", " + destPickName;
            }

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
            //bi directional
            rawData = whereRoute.GetDataBasedOnOriginAndDestinationCriteria(rawData, orgPickList, destPickList, notIn, returnAllLegs).ToList();

            globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pickName, displayName, notIn, orgList + ", " + destList);
            return rawData;
        }

    }
}
