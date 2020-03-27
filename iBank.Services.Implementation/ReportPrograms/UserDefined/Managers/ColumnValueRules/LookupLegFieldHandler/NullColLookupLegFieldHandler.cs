using Domain.Models.ReportPrograms.UserDefinedReport;
using UserDefinedReports;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class NullColLookupLegFieldHandler : IColumnValue
    {
        private LegRawData _legRawData;

        private UserReportColumnInformation _column;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _column = colValRulesParams.Column;
            _legRawData = colValRulesParams.LegRawData;
        }

        public string CalculateColValue()
        {
            return ReportBuilder.GetValueAsString(_legRawData, _column.Name);
        }
    }
}
