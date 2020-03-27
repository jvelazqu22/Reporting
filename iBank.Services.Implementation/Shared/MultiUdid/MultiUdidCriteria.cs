using System.Collections.Generic;
using System.Linq;

using CODE.Framework.Core.Utilities.Extensions;

using Domain.Helper;

using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.Shared.MultiUdid
{
    public class MultiUdidCriteria
    {
        public enum OperatorType
        {
            AllEqualsAndOperator,
            AllEqualsOrOperator,
            AllNotEqualAndOperator,
            AllNotEqualOrOperator,
            MixedTypeAndOperator,
            MixedTypeOrOperator
        }

        public IList<int> GetRecKeysToKeep(ReportGlobals globals, IList<UdidRecord> udidRecords)
        {
            //all the udids for the reckeys in the trips
            var tripUdids = GetTripUdids(udidRecords);

            var udidParameters = GetUdidParameters(globals.MultiUdidParameters.Parameters);

            var reckeysToKeep = new List<int>();
            var operatorType = GetOperatorType(globals);

            foreach (var reckeyUdids in tripUdids)
            {
                var conditionals = new MultiUdidConditionals(reckeyUdids.Value, udidParameters);
                if (conditionals.KeepRecKey(operatorType, globals))
                {
                    reckeysToKeep.Add(reckeyUdids.Key);
                }
            }

            return reckeysToKeep;
        }

        private Dictionary<int, Dictionary<int, string>> GetTripUdids(IList<UdidRecord> udidRecords)
        {
            return udidRecords.GroupBy(x => x.RecKey)
                                .ToDictionary(y => y.Key, y => y.ToDictionary(z => z.UdidNumber, z => z.UdidText));
        }

        private List<UdidParameter> GetUdidParameters(List<AdvancedParameter> multiUdidParameters)
        {
            //FieldName is the udid number, Value1 is the udid 
            var udidParameters = new List<UdidParameter>();
            var i = 0;
            foreach (var udid in multiUdidParameters)
            {
                udidParameters.Add(new UdidParameter { Seq = i++, UdidNumber = udid.FieldName.TryIntParse(-1), UdidText = udid.Value1 });
            }

            return udidParameters;
        }

        private OperatorType GetOperatorType(ReportGlobals globals)
        {
            var operatorRetriever = new MultiUdidOperatorRetrieval();
            return operatorRetriever.GetOpertorType(globals);
        }
    }

    
}
