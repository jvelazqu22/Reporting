using Domain.Models.ReportPrograms.UserDefinedReport;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupMktSegsFieldHandler
{
    public class MAirLine : IColumnValue
    {
        private MarketSegmentRawData _marketSegmentRawData;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _marketSegmentRawData = colValRulesParams.MarketSegmentRawData;
        }

        public string CalculateColValue()
        {
            //cbuf2 = "curMktsegs.airline"
            return _marketSegmentRawData.Airline;
        }
    }
}
