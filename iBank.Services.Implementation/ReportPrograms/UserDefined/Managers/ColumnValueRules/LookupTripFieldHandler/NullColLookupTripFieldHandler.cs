using Domain.Models.ReportPrograms.UserDefinedReport;
using UserDefinedReports;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class NullColLookupTripFieldHandler : IColumnValue
    {
        private RawData _mainRec;
        private UserReportColumnInformation _column;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _column = colValRulesParams.Column;
        }

        public string CalculateColValue()
        {
            return ReportBuilder.GetValueAsString(_mainRec, _column.Name);
        }
    }
}
