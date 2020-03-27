using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Services.Implementation.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class ActFare : IColumnValue
    {
        private LegRawData _legRawData;
        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _legRawData = colValRulesParams.LegRawData;
        }

        public string CalculateColValue()
        {
            return $"{(MathHelper.Round(_legRawData.ActFare + _legRawData.MiscAmt, 2)):0.00}";
        }
    }
}
