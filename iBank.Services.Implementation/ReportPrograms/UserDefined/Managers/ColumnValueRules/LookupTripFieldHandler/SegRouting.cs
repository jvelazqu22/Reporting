using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class SegRouting : IColumnValue
    {
        private RawData _mainRec;
        private ReportLookups _reportLookups;
        private SegmentOrLeg _segmentOrLeg;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _reportLookups = colValRulesParams.ReportLookups;
            _segmentOrLeg = colValRulesParams.SegmentOrLeg;
        }

        public string CalculateColValue()
        {
            if (Features.RoutingUseTripsDerivedDataTable.IsEnabled())
            {
                if (_segmentOrLeg == SegmentOrLeg.Leg)
                {
                    return LookupFunctions.LookupSegRoute(_reportLookups.Legs, _mainRec.RecKey);
                }
                else
                {
                    return LookupFunctions.LookupSegRoute(_reportLookups.Segs, _mainRec.RecKey);
                }
            }
            else
            {
                return LookupFunctions.LookupSegRouteDeprecate(_reportLookups.Segs, _mainRec.RecKey);
            }
        }
    }
}
