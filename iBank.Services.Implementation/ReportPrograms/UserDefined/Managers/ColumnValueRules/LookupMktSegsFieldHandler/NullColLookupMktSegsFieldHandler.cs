using Domain.Models.ReportPrograms.UserDefinedReport;
using UserDefinedReports;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupMktSegsFieldHandler
{
    public class NullColLookupMktSegsFieldHandler : IColumnValue
    {
        private UserReportColumnInformation _column;
        private MarketSegmentRawData _marketSegmentRawData;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _marketSegmentRawData = colValRulesParams.MarketSegmentRawData;
            _column = colValRulesParams.Column;
        }

        public string CalculateColValue()
        {
            return ReportBuilder.GetValueAsString(_marketSegmentRawData, _column.Name);
        }
    }
}
