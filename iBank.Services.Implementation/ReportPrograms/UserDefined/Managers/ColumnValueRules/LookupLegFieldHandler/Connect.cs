using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class Connect : IColumnValue
    {
        private LegRawData _legRawData;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _legRawData = colValRulesParams.LegRawData;
        }

        public string CalculateColValue()
        {
            return _legRawData.Connect.EqualsIgnoreCase("X") ? "Y" : "N";
        }
    }
}
