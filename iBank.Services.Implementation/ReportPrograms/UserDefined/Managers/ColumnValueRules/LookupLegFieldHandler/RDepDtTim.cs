using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities;
using System.Globalization;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class RDepDtTim : IColumnValue
    {
        private LegRawData _legRawData;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _legRawData = colValRulesParams.LegRawData;
        }

        public string CalculateColValue()
        {
            return _legRawData.RDepDate.GetValueOrDefault().MakeDateTime(_legRawData.DepTime).ToString(CultureInfo.InvariantCulture);
        }
    }
}
