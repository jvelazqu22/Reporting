using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class TripMiles : IColumnValue
    {
        private RawData _mainRec;
        private ReportLookups _reportLookups;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _reportLookups = colValRulesParams.ReportLookups;
        }

        public string CalculateColValue()
        {
            return (LookupFunctions.LookupTripMiles(_reportLookups.Segs, _mainRec.RecKey) * _mainRec.PlusMin).ToString();
        }
    }
}
