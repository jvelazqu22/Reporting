using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities.Helpers;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class DepTime : IColumnValue
    {
        private bool _isDdTime;
        private LegRawData _legRawData;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _legRawData = colValRulesParams.LegRawData;
            _isDdTime = colValRulesParams.IsDdTime;
        }

        public string CalculateColValue()
        {
            return _isDdTime ? SharedProcedures.ConvertTime(_legRawData.DepTime) : _legRawData.DepTime;
        }
    }
}
