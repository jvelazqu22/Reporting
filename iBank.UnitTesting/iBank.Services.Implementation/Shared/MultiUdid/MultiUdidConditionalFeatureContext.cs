using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Helper;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.MultiUdid
{
    public class MultiUdidConditionalFeatureContext
    {
        public List<UdidRecord> UdidRecords;

        public List<AdvancedParameter> MultiUdidParameters;

        public IList<int> Result;

        public AndOr AndOr;

        public MultiUdidConditionalFeatureContext()
        {
            UdidRecords = new List<UdidRecord>();
            MultiUdidParameters = new List<AdvancedParameter>();
            Result = new List<int>();
        }
    }
}
