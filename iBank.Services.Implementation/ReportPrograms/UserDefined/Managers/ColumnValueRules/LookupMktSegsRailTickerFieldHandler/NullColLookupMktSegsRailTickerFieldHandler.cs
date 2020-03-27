using Domain.Models.ReportPrograms.UserDefinedReport;
using UserDefinedReports;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupMktSegsRailTickerFieldHandler
{
    public class NullColLookupMktSegsRailTickerFieldHandler : IColumnValue
    {
        private MiscSegSharedRawData _miscSegSharedRawData;
        private UserReportColumnInformation _column;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _miscSegSharedRawData = colValRulesParams.MiscSegSharedRawData;
            _column = colValRulesParams.Column;
        }

        public string CalculateColValue()
        {
            return ReportBuilder.GetValueAsString(_miscSegSharedRawData, _column.Name);
        }
    }
}
