using Domain.Models.ReportPrograms.UserDefinedReport;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class RDepDtDoW : IColumnValue
    {
        private LegRawData _legRawData;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _legRawData = colValRulesParams.LegRawData;
        }

        public string CalculateColValue()
        {
            return _legRawData.RDepDate.GetValueOrDefault().DayOfWeek.ToString().Substring(0, 3);
        }
    }
}
