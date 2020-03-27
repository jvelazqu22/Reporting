using Domain.Models.ReportPrograms.UserDefinedReport;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class SegCost : IColumnValue
    {
        private LegRawData _legRawData;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _legRawData = colValRulesParams.LegRawData;
        }

        public string CalculateColValue()
        {
            return $"{(_legRawData.ActFare + _legRawData.MiscAmt):0.00}";
        }
    }
}
