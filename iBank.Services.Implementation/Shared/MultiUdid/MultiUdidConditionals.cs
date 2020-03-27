using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.Shared.MultiUdid
{
    public class MultiUdidConditionals
    {
        private readonly Dictionary<int, string> _recKeyUdids;

        private readonly IList<UdidParameter> _udidParameters;

        public MultiUdidConditionals(Dictionary<int, string> recKeyUdids, IList<UdidParameter> udidParameters)
        {
            _recKeyUdids = recKeyUdids;
            _udidParameters = udidParameters;
        }

        public bool KeepRecKey(MultiUdidCriteria.OperatorType operatorType, ReportGlobals globals)
        {
            switch (operatorType)
            {
                case MultiUdidCriteria.OperatorType.AllEqualsAndOperator:
                    return IsOkToKeepWithAllEqualsAndOperator();
                case MultiUdidCriteria.OperatorType.AllEqualsOrOperator:
                    return IsOkToKeepWithAllEqualsOrOperator();
                case MultiUdidCriteria.OperatorType.AllNotEqualAndOperator:
                    return IsOkToKeepWithAllNotEqualAndOperator();
                case MultiUdidCriteria.OperatorType.AllNotEqualOrOperator:
                    return IsOkToKeepWithAllNotEqualOrOperator();
                case MultiUdidCriteria.OperatorType.MixedTypeAndOperator:
                    return IsOkToKeepWithMixedTypeAndOperator(globals);
                case MultiUdidCriteria.OperatorType.MixedTypeOrOperator:
                    return IsOkToKeepWithMixedTypeOrOperator(globals);
                default:
                    throw new ArgumentException($"Unhandled operator argument of [{operatorType}] supplied.");
            }
        }

        private bool IsOkToKeepWithAllEqualsAndOperator()
        {
            var udidsInReckeyAndInParameters = GetUdidsInReckeysAndParameters();
            if (!udidsInReckeyAndInParameters.Any()) return false;

            foreach (var udid in udidsInReckeyAndInParameters)
            {
                var reckeyUdidText = GetUdidText(udid);
                foreach (var udidParam in _udidParameters.Where(x => x.UdidNumber == udid))
                {
                    var filterText = udidParam.UdidText;
                    if (filterText.HasWildCards())
                    {
                        if (!filterText.Like(reckeyUdidText)) return false;
                    }
                    else
                    {
                        if (!reckeyUdidText.EqualsIgnoreCase(filterText.Trim())) return false;
                    }
                }
            }

            return true;
        }

        private List<int> GetUdidsInReckeysAndParameters()
        {
            var udidsForReckeys = _recKeyUdids.Select(x => x.Key);
            //only want the udid numbers that are in both the reckeys and in the parameters
            return _udidParameters.Select(x => x.UdidNumber).Intersect(udidsForReckeys).ToList();
        }

        private string GetUdidText(int udid)
        {
            var reckeyUdidText = _recKeyUdids[udid];
            if (!string.IsNullOrEmpty(reckeyUdidText)) reckeyUdidText = reckeyUdidText.Trim();

            return reckeyUdidText;
        }

        private bool IsOkToKeepWithAllEqualsOrOperator()
        {
            var udidsInReckeyAndInParameters = GetUdidsInReckeysAndParameters();
            if (!udidsInReckeyAndInParameters.Any()) return false;

            foreach (var udid in udidsInReckeyAndInParameters)
            {
                var reckeyUdidText = GetUdidText(udid);
                foreach (var udidParam in _udidParameters.Where(x => x.UdidNumber == udid))
                {
                    var filterText = udidParam.UdidText;
                    if (filterText.HasWildCards())
                    {
                        if (filterText.Like(reckeyUdidText)) return true;
                    }
                    else
                    {
                        if (reckeyUdidText.EqualsIgnoreCase(filterText.Trim())) return true;
                    }
                }
            }

            return false;
        }

        private bool IsOkToKeepWithAllNotEqualAndOperator()
        {
            var udidsInReckeyAndInParameters = GetUdidsInReckeysAndParameters();
            if (!udidsInReckeyAndInParameters.Any()) return true;

            foreach (var udid in udidsInReckeyAndInParameters)
            {
                var reckeyUdidText = GetUdidText(udid);
                foreach (var udidParam in _udidParameters.Where(x => x.UdidNumber == udid))
                {
                    var filterText = udidParam.UdidText;
                    if (filterText.HasWildCards())
                    {
                        if (filterText.Like(reckeyUdidText)) return false;
                    }
                    else
                    {
                        if (reckeyUdidText.EqualsIgnoreCase(filterText.Trim())) return false;
                    }
                }                
            }

            return true;
        }

        private bool IsOkToKeepWithAllNotEqualOrOperator()
        {
            //if there are any udids that are in the parameters, but not belonging to the reckey, then 
            //then this will automatically pass - we are considering the absence of the udid to be the same as NOT EQUALS
            var udidsInReckeyAndInParameters = GetUdidsInReckeysAndParameters();
            if (!udidsInReckeyAndInParameters.Any() || udidsInReckeyAndInParameters.Count != _udidParameters.Count) return true;
            
            foreach (var udid in udidsInReckeyAndInParameters)
            {
                var reckeyUdidText = GetUdidText(udid);
                foreach (var udidParam in _udidParameters.Where(x => x.UdidNumber == udid))
                {
                    var filterText = udidParam.UdidText;
                    if (filterText.HasWildCards())
                    {
                        if (!filterText.Like(reckeyUdidText)) return true;
                        else return false;
                    }
                    else
                    {
                        if (!reckeyUdidText.EqualsIgnoreCase(filterText.Trim())) return true;
                        else return false;
                    }
                }                
            }

            return false;
        }

        private bool IsOkToKeepWithMixedTypeAndOperator(ReportGlobals globals)
        {
            var udidsInReckeyAndInParameters = GetUdidsInReckeysAndParameters();
            if (!udidsInReckeyAndInParameters.Any()) return false;

            var isAllOK = true;
            foreach (var udid in udidsInReckeyAndInParameters)
            {
                var reckeyUdidText = GetUdidText(udid);
                foreach (var udidParam in _udidParameters.Where(x => x.UdidNumber == udid))
                {
                    //get the operator that is paired with the udid number we are working
                    var oper = GetOperator(globals.MultiUdidParameters.Parameters, udid);
                    if (oper == null) continue;

                    var filterText = udidParam.UdidText;
                    if (oper.Operator == Operator.Equal)
                    {
                        if (udidParam.UdidText.HasWildCards())
                        {
                            if (!filterText.Like(reckeyUdidText)) return false;
                        }
                        else
                        {
                            if (!reckeyUdidText.Trim().EqualsIgnoreCase(filterText.Trim())) return false;
                        }
                    }
                    //operator is not equal
                    else
                    {
                        if (udidParam.UdidText.HasWildCards())
                        {
                            isAllOK = !filterText.Like(reckeyUdidText);
                            if (filterText.Like(reckeyUdidText)) break;
                        }
                        else
                        {
                            isAllOK = !reckeyUdidText.Trim().EqualsIgnoreCase(filterText.Trim());
                            if (reckeyUdidText.Trim().EqualsIgnoreCase(filterText.Trim())) break;
                        }
                    }
                }
            }

            return isAllOK;
        }

        private AdvancedParameter GetOperator(List<AdvancedParameter> parameters, int udid)
        {
            return parameters.FirstOrDefault(x => x.FieldName.Trim().EqualsIgnoreCase(udid.ToString().Trim()));
        }

        private bool IsOkToKeepWithMixedTypeOrOperator(ReportGlobals globals)
        {
            var udidsInReckeyAndInParameters = GetUdidsInReckeysAndParameters();
            if (!udidsInReckeyAndInParameters.Any()) return true;

            foreach (var udid in udidsInReckeyAndInParameters)
            {
                var reckeyUdidText = GetUdidText(udid);
                foreach (var udidParam in _udidParameters.Where(x => x.UdidNumber == udid))
                {
                    var oper = GetOperator(globals.MultiUdidParameters.Parameters, udid);
                    if (oper == null) continue;

                    var filterText = udidParam.UdidText;
                    if (oper.Operator == Operator.Equal)
                    {
                        if (filterText.HasWildCards())
                        {
                            if (filterText.Like(reckeyUdidText)) return true;
                        }
                        else
                        {
                            if (reckeyUdidText.Trim().EqualsIgnoreCase(filterText.Trim())) return true;
                        }
                    }
                    //operator is not equal
                    else
                    {
                        if (filterText.HasWildCards())
                        {
                            if (!filterText.Like(reckeyUdidText)) return true;
                        }
                        else
                        {
                            if (!reckeyUdidText.Trim().EqualsIgnoreCase(filterText.Trim())) return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
