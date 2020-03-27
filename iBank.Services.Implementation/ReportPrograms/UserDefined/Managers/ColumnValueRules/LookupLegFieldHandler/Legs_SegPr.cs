using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class Legs_SegPr : IColumnValue
    {
        private LegRawData _legRawData;
        private UserDefinedParameters _userDefinedParams;
        private RawData _mainRec;
        private SegmentOrLeg _segmentOrLeg;
        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _legRawData = colValRulesParams.LegRawData;
            _userDefinedParams = colValRulesParams.UserDefinedParams;
            _mainRec = colValRulesParams.MainRec;
            _segmentOrLeg = colValRulesParams.SegmentOrLeg;
        }

        public string CalculateColValue()
        {
            return _segmentOrLeg == SegmentOrLeg.Leg
                ? _legRawData.Farebase?.Trim()
                : LookupHelpers.GetPredominantFareBasisByHighMiles(_mainRec.RecKey, _userDefinedParams.LegDataList);
        }
    }
}
