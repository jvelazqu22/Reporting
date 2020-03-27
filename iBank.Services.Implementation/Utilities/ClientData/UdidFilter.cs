using System.Collections.Generic;
using System.Linq;

using Domain.Helper;
using Domain.Interfaces;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.MultiUdid;

namespace iBank.Services.Implementation.Utilities.ClientData
{
    public class UdidFilter
    {
        public IList<T> GetUdidFilteredData<T>(IList<T> data, ReportGlobals globals, bool isReservationReport, TripUdidRetriever udidRetriever) where T : IRecKey
        {
            if (!globals.MultiUdidParameters.Parameters.Any()) return data;

            var udids = udidRetriever.GetUdids(data, globals, isReservationReport);

            //filter on the udid criteria
            var multiUdidCriteria = new MultiUdidCriteria();
            var recKeysToKeep = multiUdidCriteria.GetRecKeysToKeep(globals, udids);
            var filteredData = data.Where(x => recKeysToKeep.Contains(x.RecKey)).ToList();

            filteredData.AddRange(GetRecordsWithoutAssociatedUdids(globals, data, udids));

            return filteredData;
        }

        private IEnumerable<T> GetRecordsWithoutAssociatedUdids<T>(ReportGlobals globals, IList<T> data, IList<UdidRecord> udids) where T : IRecKey
        {
            var operatorRetriever = new MultiUdidOperatorRetrieval();
            var operatorType = operatorRetriever.GetOpertorType(globals);

            if (operatorType == MultiUdidCriteria.OperatorType.AllNotEqualOrOperator
                || operatorType == MultiUdidCriteria.OperatorType.AllNotEqualAndOperator
                || operatorType == MultiUdidCriteria.OperatorType.MixedTypeOrOperator)
            {
                //if we are getting reckeys that are not equal to the udid text, then we need to get any reckeys that did not have any associated udids
                return data.Where(x => !udids.Select(y => y.RecKey).Contains(x.RecKey));
            }

            return new List<T>();
        }
    }
}
