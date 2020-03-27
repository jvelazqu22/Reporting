using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class LegRouting : IColumnValue
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
            if (_segmentOrLeg == SegmentOrLeg.Leg)
            {
                return LookupFunctions.LookupLegRoute(_reportLookups.Legs, _mainRec.RecKey);
            }
            else
            {
                return LookupFunctions.LookupLegRoute(_reportLookups.Segs, _mainRec.RecKey);
            }
        }
    }
}
